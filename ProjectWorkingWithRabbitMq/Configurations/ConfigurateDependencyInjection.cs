using WorkingWithRabbitMq.Application;
using WorkingWithRabbitMq.Infra.Services;

namespace WorkingWithRabbitMq.Svc.Configurations
{
    public static class ConfigurateDependencyInjection
    {
        public static void Configurations(IServiceCollection services)
        {            
            services.AddScoped<IWorkingWithRabbitMqService, WorkingWithRabbitMqService>();         
        }
    }
}
