using Public.Domain.HomeCare.DataModels.Request.Offer;
using Public.Domain.HomeCare.DataModels.Response.Offer;

namespace Public.Application.HomeCare.Interfaces
{
    public interface IOfferService
    {
        Task<List<GetOfferResponseModel>> GetActiveOffersAsync();

        Task<CheckoutSummaryResponseModel> GetCheckoutSummaryAsync(int serviceId);

        Task<CheckoutSummaryResponseModel> ValidateCouponAsync(ValidateCouponRequestModel request);
       
        Task<decimal> CalculateFinalAmountAsync(decimal servicePrice, int? offerId);
    }
}