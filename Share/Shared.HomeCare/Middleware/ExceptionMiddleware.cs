using Microsoft.AspNetCore.Http;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Exceptions;
using Shared.HomeCare.Resources;
using System.Net;
using System.Text.Json;

namespace Shared.HomeCare.Middleware
{
    public class ExceptionMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (AccountInactiveException ex)
            {
                await WriteResponse(context, HttpStatusCode.Forbidden, ex.Message);
            }
            catch (DuplicateRecordException ex)
            {
                await WriteResponse(context, HttpStatusCode.Conflict, ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                await WriteResponse(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                await WriteResponse(context, HttpStatusCode.Unauthorized, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                await WriteResponse(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                await WriteResponse(context, HttpStatusCode.InternalServerError,
                   ex.Message ?? Messages.FailedResponse);
            }
        }

        private static async Task WriteResponse(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new ApiResponse
            {
                IsSuccess = false,
                StatusCode = statusCode,
                Message = message,
                Data = null!
            };

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}