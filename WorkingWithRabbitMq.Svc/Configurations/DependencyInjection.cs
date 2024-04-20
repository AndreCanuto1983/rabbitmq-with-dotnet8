using WorkingWithRabbitMq.Infra.Interface;
using WorkingWithRabbitMq.Infra.Services;

namespace WorkingWithRabbitMq.Svc.Configurations
{
    public static class DependencyInjection
    {
        public static void DependencyInjectionSettings(this IServiceCollection services)
        {
            services.AddScoped<IWorkingWithRabbitMqService, WorkingWithRabbitMqService>();
        }
    }
}