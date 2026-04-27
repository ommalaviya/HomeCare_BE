namespace Admin.Domain.HomeCare.DataModels.Response.Auth
{
    public class LoginResponseModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? ProfileImageName { get; set; }
        public bool IsSuperAdmin { get; set; }
        public string Token { get; set; } = string.Empty;


        [System.Text.Json.Serialization.JsonIgnore]
        public string RefreshToken { get; set; } = string.Empty;
    }
}