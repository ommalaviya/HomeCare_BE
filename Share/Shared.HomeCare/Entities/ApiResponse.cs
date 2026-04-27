using System.Net;
using System.Text.Json.Serialization;
 
namespace Shared.HomeCare.Entities
{
    public class ApiResponse
    {
        public ApiResponse()
        {
            ErrorMessages = new List<string>();
        }
        [JsonPropertyName("isSuccess")]
        public bool IsSuccess { get; set; }
        [JsonPropertyName("data")]
        public object Data { get; set; } = null!;
        [JsonPropertyName("statusCode")]
        public HttpStatusCode StatusCode { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; } = null!;
        [JsonPropertyName("title")]
        public string Title { get; set; } = null!;
        [JsonPropertyName("errorMessages")]
        public List<string> ErrorMessages { get; set; }
    }
}