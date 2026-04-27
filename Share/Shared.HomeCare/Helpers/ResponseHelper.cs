using Shared.HomeCare.Entities;
using Shared.HomeCare.Resources;

namespace Shared.Helpers
{
    public static class ResponseHelper
    {

        public static ApiResponse CreateResponse(object data, string? message = null)
        {
            return new()
            {
                StatusCode = System.Net.HttpStatusCode.Created,
                Data = data,
                IsSuccess = true,
                Message = !string.IsNullOrEmpty(message) ? message : Messages.CreatedSuccessfully,
            };
        }

        public static ApiResponse SuccessResponse(object data, string? message = null)
        {
            return new()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Data = data,
                IsSuccess = true,
                Message = message ?? string.Empty,
            };
        }

        public static ApiResponse FailedResponse(object data, string? message = null, System.Net.HttpStatusCode? statusCode = null)
        {
            return new()
            {
                StatusCode = statusCode ?? System.Net.HttpStatusCode.BadRequest,
                Data = data,
                IsSuccess = false,
                Message = !string.IsNullOrEmpty(message) ? message : Messages.FailedResponse,
            };
        }

    }
}