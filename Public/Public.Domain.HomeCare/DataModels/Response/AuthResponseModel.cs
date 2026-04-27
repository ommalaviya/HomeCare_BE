namespace Public.Domain.HomeCare.DataModels.Response.Users
{
    public class AuthResponseModel
    {
        public GetUserResponseModel? User { get; set; }
        public string? Token { get; set; }
    }
}