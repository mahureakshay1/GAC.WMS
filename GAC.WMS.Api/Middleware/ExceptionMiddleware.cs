using static GAC.WMS.Application.DTOs.ErrorResponseDto;
using System.Net;
using FluentValidation;
using System.Text.Json;
using GAC.WMS.Domain.Exceptions;

namespace GAC.WMS.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            HttpStatusCode status = HttpStatusCode.InternalServerError;
            var response = new ErrorResponse();
            switch (exception)
            {
                case ArgumentNullException:
                    status = HttpStatusCode.BadRequest;
                    response.StatusCode = (int)status;
                    response.Message = "Invalid request.";
                    break;
                case ArgumentException:
                    status = HttpStatusCode.BadRequest;
                    response.StatusCode = (int)status;
                    response.Message = exception.Message;
                    break;
                case InvalidOperationException:
                    status = HttpStatusCode.InternalServerError;
                    response.StatusCode = (int)status;
                    response.Message = exception.Message;
                    break;
                case ValidationException:
                    status = HttpStatusCode.BadRequest;
                    response.StatusCode = (int)status;
                    response.Message = "Validation failed.";
                    response.Errors = ((ValidationException)exception).Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();
                    break;
                case ItemNotFoundException:
                    status = HttpStatusCode.NotFound;
                    response.StatusCode = (int)status;
                    response.Message = exception.Message;
                    break;
                case UnauthorizedAccessException:
                    status = HttpStatusCode.Unauthorized;
                    response.StatusCode = (int)status;
                    response.Message = exception.Message;
                    break;
                default:
                    status = HttpStatusCode.InternalServerError;
                    response.StatusCode = (int)status;
                    response.Message = "An unexpected error occurred.";
                    break;
            }
            _logger.LogError(exception, exception.Message);

            context.Response.StatusCode = (int)status;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
