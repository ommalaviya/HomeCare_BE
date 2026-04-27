using Admin.Domain.HomeCare.DataModels.Request.Offer;
using Admin.Domain.HomeCare.DataModels.Response.Offer;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;

namespace Admin.Domain.HomeCare.Interface
{
    public interface IOfferRepository : IGenericRepository<Offer>
    {
        Task<FilteredDataQueryResponse<GetOfferResponseModel>> GetOffersWithMetaAsync(
                    FilterOfferRequestModel filter);
    }
}