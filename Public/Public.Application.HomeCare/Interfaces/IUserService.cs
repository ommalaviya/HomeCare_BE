using Public.Domain.HomeCare.DataModels.Request.Users;
using Public.Domain.HomeCare.DataModels.Response.Users;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.Interfaces.Services;

namespace Public.Application.HomeCare.Interfaces
{
    public interface IUserService : IGenericService<User>
    {
        Task<DataQueryResponseModel<GetUserResponseModel>> GetUsersAsync();

        Task<GetUserResponseModel?> GetUserByEmailAsync(string email);

        Task<GetUserResponseModel> CreateOrUpdateUserAsync(CreateOrUpdateUserRequestModel request);

        Task<GetUserResponseModel> GetProfileAsync();

        Task SendEmailUpdateOtpAsync(string newEmail);

        Task<GetUserResponseModel> UpdateEmailAsync(UpdateEmailRequestModel request);

        Task<GetUserResponseModel> UpdatePhoneAsync(UpdatePhoneRequestModel request);
    }
}