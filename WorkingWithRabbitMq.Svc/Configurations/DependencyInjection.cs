using WorkingWithRabbitMq.Application;
using WorkingWithRabbitMq.Infra.Services;

namespace WorkingWithRabbitMq.Svc.Configurations
{
    public static class DependencyInjection
    {
        public static void Configurations(IServiceCollection services)
        {            
            services.AddScoped<IWorkingWithRabbitMqService, WorkingWithRabbitMqService>();         
        }
    }
}
