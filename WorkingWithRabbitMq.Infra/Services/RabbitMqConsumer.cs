using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using WorkingWithRabbitMq.Application.Model;
using WorkingWithRabbitMq.Application.Model.Configurations;

namespace WorkingWithRabbitMq.Infra.Services
{
    public class RabbitMqConsumer : BackgroundService
    {
        private readonly int _activeWorkerMessageInterval;
        private readonly RabbitMqConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMqConsumer> _logger;

        public RabbitMqConsumer(
            IConfiguration configuration,
            IOptions<RabbitMqConfiguration> option,
            ILogger<RabbitMqConsumer> logger)
        {
            _activeWorkerMessageInterval =
                Convert.ToInt32(configuration["ActiveWorkerMessageInterval"]);
            _configuration = option.Value;
            _logger = logger;

            var factory = new ConnectionFactory
            {
                HostName = _configuration.Host
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(
                        queue: _configuration.Queue,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Waiting for messages...");

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (sender, eventArgs) =>
            {
                var contentArray = eventArgs.Body.ToArray();
                var contentString = Encoding.UTF8.GetString(contentArray);
                var message = JsonSerializer.Deserialize<RabbitMqTask>(contentString);

                NotifyUser(message);

                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            _channel.BasicConsume(_configuration.Queue, false, consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation(
                    $"Active Worker: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                await Task.Delay(_activeWorkerMessageInterval, stoppingToken);
            }
        }

        public void NotifyUser(RabbitMqTask? message)
        {
            _logger.LogInformation("User Information: Message = {message}", JsonSerializer.Serialize(message));

            //There are several forms of notification, it will depend on the application and what you need, just implement.
        }
    }
}
