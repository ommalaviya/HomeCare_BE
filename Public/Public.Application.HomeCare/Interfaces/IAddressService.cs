using Public.Domain.HomeCare.DataModels.Request.Address;
using Public.Domain.HomeCare.DataModels.Response.Address;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.Interfaces.Services;

namespace Public.Application.HomeCare.Interfaces
{
    public interface IAddressService : IGenericService<UserAddress>
    {
        Task<DataQueryResponseModel<AddressResponseModel>> GetMyAddressesAsync();

        Task<AddressResponseModel> GetAddressByIdAsync(int addressId);

        Task<AddressResponseModel> CreateAddressAsync(CreateAddressRequestModel request);

        Task<AddressResponseModel> UpdateAddressAsync(int addressId, UpdateAddressRequestModel request);

        Task<bool> DeleteAddressAsync(int addressId);

        Task<ReverseGeocodeResponseModel> ReverseGeocodeAsync(ReverseGeocodeRequestModel request);

        Task<List<NominatimSearchResultModel>> SearchAddressAsync(string query);
    }
}