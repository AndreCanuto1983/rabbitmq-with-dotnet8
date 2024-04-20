using WorkingWithRabbitMq.Application.Model.Configurations;

namespace WorkingWithRabbitMq.Svc.Configurations
{
    public static class RabbitMq
    {
        public static void RabbitMqSettings(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMqConfiguration"));
        }
    }
}