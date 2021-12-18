using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using WorkingWithRabbitMq.Application;
using WorkingWithRabbitMq.Application.Model;
using WorkingWithRabbitMq.Application.Model.Configurations;

namespace WorkingWithRabbitMq.Infra.Services
{
    public class WorkingWithRabbitMqService : IWorkingWithRabbitMqService
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly RabbitMqConfiguration _configuration;
        private readonly ILogger<IWorkingWithRabbitMqService> _logger;

        public WorkingWithRabbitMqService(
            IOptions<RabbitMqConfiguration> options,
            ILogger<IWorkingWithRabbitMqService> logger)
        {
            _configuration = options.Value;
            _logger = logger;

            _connectionFactory = new ConnectionFactory
            {
                HostName = _configuration.Host,
                UserName = _configuration.Username,
                Password = _configuration.Password
            };
        }

        public bool SendMessage(RabbitMqTask task, CancellationToken cancellationToken)
        {
            using (var connection = _connectionFactory.CreateConnection())

            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _configuration.Queue,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                var stringfiedMessage = JsonSerializer.Serialize(task);

                var bytesMessage = Encoding.UTF8.GetBytes(stringfiedMessage);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(
                    exchange: "",
                    routingKey: _configuration.Queue,
                    basicProperties: properties,
                    body: bytesMessage);
            }

            return true;
        }

        public async Task<RabbitMqTask> GetMessage(CancellationToken cancellationToken)
        {
            using (var connection = _connectionFactory.CreateConnection())

            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _configuration.Queue,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.BasicQos(0, 1, false); //fala pro rabbit entregar apenas uma msg pro chamada

                var consumer = new EventingBasicConsumer(channel);

                var rabbitMqTask = new RabbitMqTask();

                consumer.Received += (sender, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();

                        var message = Encoding.UTF8.GetString(body);

                        rabbitMqTask = JsonSerializer.Deserialize<RabbitMqTask>(message);

                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        channel.BasicNack(ea.DeliveryTag, false, true);
                        _logger.LogError(ex, $"[WorkingWithRabbitMqService][GetMessage] => DeliveryTag: {ea.DeliveryTag}, ConsumerTag: {ea.ConsumerTag}, Body: {ea.Body}");
                        throw new ArgumentException($"[WorkingWithRabbitMqService][GetMessage] => DeliveryTag: {ea.DeliveryTag}, ConsumerTag: {ea.ConsumerTag}, Body: {ea.Body}");
                    }
                };

                channel.BasicConsume(queue: _configuration.Queue,
                                     autoAck: false,
                                     consumer: consumer);

                return rabbitMqTask;
            }
        }
    }
}
