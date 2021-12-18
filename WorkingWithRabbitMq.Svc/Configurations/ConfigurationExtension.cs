using System.Text.Json.Serialization;

namespace WorkingWithRabbitMq.Svc.Configurations
{
    public static class ConfigurationExtension
    {
        public static void Configurations(IServiceCollection services)
        {
            services.AddControllers()
                    .AddJsonOptions(options =>
                        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true)
                    .AddJsonOptions(options =>
                        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault);
        }
    }
}
