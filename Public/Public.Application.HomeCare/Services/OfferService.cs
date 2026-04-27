using System.Security.Claims;
using AutoMapper;
using Public.Application.HomeCare.Interfaces;
using Public.Domain.HomeCare.DataModels.Request.Offer;
using Public.Domain.HomeCare.DataModels.Response.Offer;
using Public.Domain.HomeCare.Interface;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Resources;
using Shared.HomeCare.Services;

namespace Public.Application.HomeCare.Services
{
    public class OfferService(
        IOfferRepository offerRepository,
        IServiceRepository serviceRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ClaimsPrincipal principal)
        : GenericService<Offer>(offerRepository, unitOfWork, mapper, principal), IOfferService
    {
        private const decimal TaxRate = 0.05m;
        private const int MaxCouponUsage = 10;
        private async Task<ServicesOfSubCategory> GetServiceOrThrowAsync(int serviceId)
            => await serviceRepository.FindDataAsync(s => s.Id == serviceId && !s.IsDeleted)
               ?? throw new KeyNotFoundException(
                      string.Format(Messages.NotFound, Messages.Services));

        private async Task<Offer> GetActiveOfferOrThrowAsync(int offerId)
            => await FindOrThrowAsync(
                   o => o.Id == offerId && o.IsActive && !o.IsDeleted,
                   string.Format(Messages.NotFound, Messages.Offer));


        public async Task<List<GetOfferResponseModel>> GetActiveOffersAsync()
        {
            var result = await GetAllAsync(
                o => o.IsActive && !o.IsDeleted && o.AppliedCount < MaxCouponUsage);

            return MapToList<GetOfferResponseModel>(
                result.Records.OrderByDescending(o => o.DiscountPercentage));
        }

        public async Task<CheckoutSummaryResponseModel> GetCheckoutSummaryAsync(int serviceId)
        {
            if (serviceId <= 0)
                throw new KeyNotFoundException(
                    string.Format(Messages.GreaterThanZero, Messages.ServiceId));

            var service = await GetServiceOrThrowAsync(serviceId);

            if (!service.IsAvailable)
                throw new KeyNotFoundException(
                    string.Format(Messages.Unavailable, Messages.Services));

            return BuildSummary(service.Price);
        }

        public async Task<CheckoutSummaryResponseModel> ValidateCouponAsync(
            ValidateCouponRequestModel request)
        {
            if (request.ServiceId <= 0)
                throw new KeyNotFoundException(
                    string.Format(Messages.GreaterThanZero, Messages.ServiceId));

            if (request.OfferId <= 0)
                throw new KeyNotFoundException(
                    string.Format(Messages.GreaterThanZero, Messages.Offer));

            var service = await GetServiceOrThrowAsync(request.ServiceId);

            if (!service.IsAvailable)
                throw new KeyNotFoundException(
                    string.Format(Messages.Unavailable, Messages.Services));

            var offer = await GetActiveOfferOrThrowAsync(request.OfferId);

            if (offer.AppliedCount >= MaxCouponUsage)
                throw new InvalidOperationException(Messages.CouponUsageLimitReached);

            var alreadyUsed = await offerRepository.HasUserUsedCouponAsync(
                CurrentUserId, offer.Id);

            if (alreadyUsed)
                throw new InvalidOperationException(Messages.CouponAlreadyUsed);

            var discountAmount = Math.Min(
                Math.Round(service.Price * (offer.DiscountPercentage / 100m), 2),
                service.Price);

            return BuildSummary(
                service.Price,
                offer.Id,
                offer.CouponCode,
                offer.DiscountPercentage,
                discountAmount);
        }

        private static CheckoutSummaryResponseModel BuildSummary(
            decimal itemsTotal,
            int? appliedOfferId = null,
            string? appliedCouponCode = null,
            decimal discountPercentage = 0,
            decimal discountAmount = 0)
        {
            var taxAmount = Math.Round(itemsTotal * TaxRate, 2);
            var total = Math.Round(itemsTotal + taxAmount - discountAmount, 2);

            return new CheckoutSummaryResponseModel
            {
                ItemsTotal = itemsTotal,
                TaxPercentage = TaxRate * 100,
                TaxAmount = taxAmount,
                AppliedOfferId = appliedOfferId,
                AppliedCouponCode = appliedCouponCode,
                DiscountPercentage = discountPercentage,
                DiscountAmount = discountAmount,
                TotalAmount = total,
            };
        }
            public async Task<decimal> CalculateFinalAmountAsync(decimal servicePrice, int? offerId)
        {
            var taxAmount = Math.Round(servicePrice * TaxRate, 2);
 
            if (!offerId.HasValue || offerId.Value <= 0)
                return Math.Round(servicePrice + taxAmount, 2);
 
            var offer = await offerRepository.FindDataAsync(
                o => o.Id == offerId.Value && o.IsActive && !o.IsDeleted);
 
            if (offer is null)
                return Math.Round(servicePrice + taxAmount, 2);
 
            var discountAmount = Math.Min(
                Math.Round(servicePrice * (offer.DiscountPercentage / 100m), 2),
                servicePrice);
 
            return Math.Round(servicePrice + taxAmount - discountAmount, 2);
        }
    }
}