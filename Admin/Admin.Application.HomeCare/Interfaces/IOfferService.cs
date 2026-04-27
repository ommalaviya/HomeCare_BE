using Admin.Domain.HomeCare.DataModels.Request.Offer;
using Admin.Domain.HomeCare.DataModels.Response.Offer;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.Interfaces.Services;

namespace Admin.Application.HomeCare.Interfaces
{
    public interface IOfferService : IGenericService<Offer>
    {
        Task<FilteredDataQueryResponseModel<GetOfferResponseModel>> GetOffersAsync(
                    FilterOfferRequestModel filter);
        Task<GetOfferResponseModel> GetOfferByIdAsync(int id);

        Task<GetOfferResponseModel> CreateOfferAsync(
            CreateOfferRequestModel request);

        Task<GetOfferResponseModel> UpdateOfferAsync(
            UpdateOfferRequestModel request);

        Task<bool> SoftDeleteOfferAsync(int id);
    }
}