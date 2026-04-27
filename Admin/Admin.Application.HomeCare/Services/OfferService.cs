using Admin.Application.HomeCare.Interfaces;
using Admin.Domain.HomeCare.DataModels.Request.Offer;
using Admin.Domain.HomeCare.DataModels.Response.Offer;
using Admin.Domain.HomeCare.Interface;
using AutoMapper;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Resources;
using Shared.HomeCare.Services;
using System.Security.Claims;

namespace Admin.Application.HomeCare.Services
{
    public class OfferService : GenericService<Offer>, IOfferService
    {
        private readonly IOfferRepository _repository;

        public OfferService(
            IOfferRepository repository,
            ClaimsPrincipal principal,
            IUnitOfWork unitOfWork,
            IMapper mapper)
            : base(repository, unitOfWork, mapper, principal)
        {
            _repository = repository;
        }

        public async Task<FilteredDataQueryResponseModel<GetOfferResponseModel>> GetOffersAsync(
     FilterOfferRequestModel filter)
        {
            var result = await _repository.GetOffersWithMetaAsync(filter);

            return new FilteredDataQueryResponseModel<GetOfferResponseModel>
            {
                TotalRecords = result.TotalRecords,
                Records = result.Records,
                FilterMeta = result.FilterMeta
            };
        }
        public async Task<GetOfferResponseModel> GetOfferByIdAsync(int id)
        {
            var entity = await FindOrThrowAsync(
                x => x.Id == id && !x.IsDeleted,
                string.Format(Messages.NotFound, Messages.Offer));

            return ToResponseModel<GetOfferResponseModel>(entity);
        }

        public async Task<GetOfferResponseModel> CreateOfferAsync(CreateOfferRequestModel request)
        {
            await ThrowIfDuplicateAsync(
                x => x.CouponCode == request.CouponCode && !x.IsDeleted,
                string.Format(Messages.DuplicateRecord, Messages.Offer));

            var entity = ToEntity(request);
            await AddAsync(entity);

            return ToResponseModel<GetOfferResponseModel>(entity);
        }

        public async Task<GetOfferResponseModel> UpdateOfferAsync(UpdateOfferRequestModel request)
        {
            var entity = await FindOrThrowAsync(
                x => x.Id == request.Id && !x.IsDeleted,
                string.Format(Messages.NotFound, Messages.Offer));

            await ThrowIfDuplicateAsync(
                x => x.Id != request.Id && x.CouponCode == request.CouponCode && !x.IsDeleted,
                string.Format(Messages.DuplicateRecord, Messages.Offer));

            Map<Offer, UpdateOfferRequestModel>(request, entity);

            await UpdateAsync(entity);

            return ToResponseModel<GetOfferResponseModel>(entity);
        }

        public async Task<bool> SoftDeleteOfferAsync(int id)
        {
            var entity = await FindOrThrowAsync(
                x => x.Id == id && !x.IsDeleted,
                string.Format(Messages.NotFound, Messages.Offer));

            await SoftDeleteAsync(entity);
            return true;
        }
    }
}