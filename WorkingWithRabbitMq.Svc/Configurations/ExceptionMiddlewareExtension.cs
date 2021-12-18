using WorkingWithRabbitMq.Svc.Configurations.Services;

namespace WorkingWithRabbitMq.Svc.Configurations
{
    public static class ExceptionMiddlewareExtension
    {
        public static void UseCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomExceptionMiddleware>();
        }
    }
}
