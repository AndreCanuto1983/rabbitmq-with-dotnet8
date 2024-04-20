using System.Net;
using System.Text.Json;

namespace WorkingWithRabbitMq.Svc.Configurations.Services
{
    public class CustomExceptionMiddleware(RequestDelegate requestDelegate)
    {
        private readonly RequestDelegate _requestDelegate = requestDelegate;

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _requestDelegate(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex).ConfigureAwait(false);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            int statusCode = (int)HttpStatusCode.InternalServerError;

            var result = JsonSerializer.Serialize(
                new
                {
                    StatusCode = statusCode,
                    ErrorMessage = exception.Message
                });

            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(result);
        }
    }
}
