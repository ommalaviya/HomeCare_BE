using System.Security.Claims;
using System.Text.Json;
using AutoMapper;
using Public.Application.HomeCare.Interfaces;
using Public.Domain.HomeCare.DataModels.Request.Address;
using Public.Domain.HomeCare.DataModels.Response.Address;
using Public.Domain.HomeCare.Interface;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Resources;
using Shared.HomeCare.Services;

namespace Public.Application.HomeCare.Services
{
    public class AddressService(
        IGenericRepository<UserAddress> genericRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ClaimsPrincipal principal,
        IAddressRepository addressRepository,
        IHttpClientFactory httpClientFactory)
        : GenericService<UserAddress>(genericRepository, unitOfWork, mapper, principal), IAddressService
    {

        private async Task<UserAddress> GetAddressOrThrowAsync(int addressId)
            => await GetOrThrowAsync(addressId, string.Format(Messages.NotFound, Messages.Address));

        public async Task<DataQueryResponseModel<AddressResponseModel>> GetMyAddressesAsync()
        {
            var addresses = await addressRepository.GetByUserIdAsync(CurrentUserId);
            var records = addresses.Select(a => Map<AddressResponseModel, UserAddress>(a)).ToList();

            return new DataQueryResponseModel<AddressResponseModel>
            {
                TotalRecords = records.Count,
                Records = records
            };
        }

        public async Task<AddressResponseModel> GetAddressByIdAsync(int addressId)
        {
            var address = await GetAddressOrThrowAsync(addressId);
            return Map<AddressResponseModel, UserAddress>(address);
        }

        public async Task<AddressResponseModel> CreateAddressAsync(CreateAddressRequestModel request)
        {
            var entity = ToEntity(request);
            entity.UserId = CurrentUserId;
            entity.CreatedBy = CurrentUserId;
            entity.CreatedAt = DateTime.UtcNow;
            entity.ModifiedAt = DateTime.UtcNow;

            await AddAsync(entity);
            return Map<AddressResponseModel, UserAddress>(entity);
        }

        public async Task<AddressResponseModel> UpdateAddressAsync(int addressId, UpdateAddressRequestModel request)
        {
            var entity = await GetAddressOrThrowAsync(addressId);

            mapper.Map(request, entity);
            entity.ModifiedAt = DateTime.UtcNow;
            entity.ModifiedBy = CurrentUserId;

            await UpdateAsync(entity);
            return Map<AddressResponseModel, UserAddress>(entity);
        }

        public async Task<bool> DeleteAddressAsync(int addressId)
        {
            var entity = await addressRepository.GetByIdAsync(addressId);
            if (entity == null) return false;

            entity.IsDeleted = true;
            entity.ModifiedAt = DateTime.UtcNow;
            entity.ModifiedBy = CurrentUserId;
            await UpdateAsync(entity);
            return true;
        }

        public async Task<ReverseGeocodeResponseModel> ReverseGeocodeAsync(ReverseGeocodeRequestModel request)
        {
            try
            {
                var client = httpClientFactory.CreateClient("Nominatim");
                var url = $"/reverse?format=json&lat={request.Latitude}&lon={request.Longitude}&addressdetails=1&zoom=18";

                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return FailedGeocode(request, Messages.GeocodingUnavailable);

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (!root.TryGetProperty("address", out var addr))
                    return FailedGeocode(request, Messages.NoAddressDataReturned);

                string Get(string key) =>
                    addr.TryGetProperty(key, out var v) ? v.GetString() ?? "" : "";

                var road = Get("road");
                var houseNum = Get("house_number");
                var suburb = Get("suburb");
                var city = Get("city").IfEmpty(Get("town")).IfEmpty(Get("village"));
                var state = Get("state");
                var country = Get("country");
                var countryCode = Get("country_code").ToUpper();
                var postcode = Get("postcode");
                var displayName = root.TryGetProperty("display_name", out var dn)
                    ? dn.GetString() ?? "" : "";

                var titleParts = new[] { road, houseNum, suburb }.Where(s => !string.IsNullOrEmpty(s));
                var subParts = new[] { city, state, country }.Where(s => !string.IsNullOrEmpty(s));

                return new ReverseGeocodeResponseModel
                {
                    DisplayTitle = string.Join(", ", titleParts).IfEmpty(displayName.Split(',').FirstOrDefault() ?? ""),
                    DisplaySubtitle = string.Join(", ", subParts),
                    HouseNumber = houseNum,
                    Road = road,
                    Suburb = suburb,
                    City = city,
                    State = state,
                    Country = country,
                    CountryCode = countryCode,
                    PostCode = postcode,
                    FullAddress = displayName,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return FailedGeocode(request, ex.Message);
            }
        }

        public async Task<List<NominatimSearchResultModel>> SearchAddressAsync(string query)
        {
            var client = httpClientFactory.CreateClient("Nominatim");
            var encoded = Uri.EscapeDataString(query);
            var url = $"/search?q={encoded}&format=json&addressdetails=1&limit=6&accept-language=en";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return [];

            var json = await response.Content.ReadAsStringAsync();
            var results = System.Text.Json.JsonSerializer.Deserialize<List<NominatimSearchResultModel>>(
           json,
           new System.Text.Json.JsonSerializerOptions
           {
               PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower,
               PropertyNameCaseInsensitive = true
           });

            return results ?? [];
        }

        private static ReverseGeocodeResponseModel FailedGeocode(ReverseGeocodeRequestModel req, string msg) => new()
        {
            Success = false,
            ErrorMessage = msg,
            Latitude = req.Latitude,
            Longitude = req.Longitude,
            DisplayTitle = $"{req.Latitude:F5}, {req.Longitude:F5}"
        };

    }

    internal static class StringExt
    {
        public static string IfEmpty(this string s, string fallback) =>
            string.IsNullOrEmpty(s) ? fallback : s;
    }
}