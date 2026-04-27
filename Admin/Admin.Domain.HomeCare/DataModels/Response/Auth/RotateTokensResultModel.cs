namespace Admin.Domain.HomeCare.DataModels.Response.Auth
{
    public class RotateTokensResultModel
    {
        public string AccessToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;
        public int AdminId { get; init; }
    }
}