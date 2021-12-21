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

        public void SendMessage(RabbitMqTask task)
        {
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
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
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[WorkingWithRabbitMqService][SendMessage]");
                throw new ArgumentException("Error trying to send message to queue", ex);
            }
        }

        public RabbitMqTask GetMessage()
        {
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: _configuration.Queue,
                                             durable: true,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                        channel.BasicQos(0, 1, false); //Ask the rabbitMq to deliver only one message per call

                        var consumer = new EventingBasicConsumer(channel);

                        var rabbitMqTask = new RabbitMqTask();

                        string? message = null;

                        consumer.Received += (model, ea) =>
                        {
                            try
                            {
                                message = Encoding.UTF8.GetString(ea.Body.ToArray());

                                var rabbitMqTaskDeserialized = JsonSerializer.Deserialize<RabbitMqTask>(message);

                                rabbitMqTask.Id = rabbitMqTaskDeserialized.Id;
                                rabbitMqTask.Name = rabbitMqTaskDeserialized.Name;
                                rabbitMqTask.Phone = rabbitMqTaskDeserialized.Phone;

                                if (message != null)
                                    channel.BasicAck(ea.DeliveryTag, false);
                            }
                            catch (Exception ex)
                            {
                                channel.BasicNack(ea.DeliveryTag, false, true);
                                _logger.LogError(ex, @$"[WorkingWithRabbitMqService][GetMessage][consumer.Received] => 
                                        DeliveryTag: {ea.DeliveryTag}, 
                                        ConsumerTag: {ea.ConsumerTag}, 
                                        Body: {ea.Body}, 
                                        Message: {message}");
                            }
                        };

                        channel.BasicConsume(queue: _configuration.Queue,
                                             autoAck: false,
                                             consumer: consumer);

                        return rabbitMqTask;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[WorkingWithRabbitMqService][GetMessage]");
                throw new ArgumentException("Error trying to fetch message from queue", ex);
            }            
        }
    }
}
