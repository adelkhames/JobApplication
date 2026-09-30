using JobApplication.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace JobApplication.API.Middleware
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
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, message) = exception switch
            {
                NotFoundException => (HttpStatusCode.NotFound, exception.Message),
                ForbiddenException => (HttpStatusCode.Forbidden, exception.Message),
                BusinessRuleException => (HttpStatusCode.BadRequest, exception.Message),
                ConflictException => (HttpStatusCode.Conflict, exception.Message),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var body = JsonSerializer.Serialize(new
            {
                statusCode = (int)statusCode,
                message
            });

            return context.Response.WriteAsync(body);
        }
    }
}
