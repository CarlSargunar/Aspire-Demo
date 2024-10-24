using System.Text;
using System.Text.Json;
using DemoLib.Models;
using RabbitMQ.Client; 

namespace UmbWebsite.Services
{
    public class MessageService : IMessageService, IDisposable
    {
        private readonly ILogger<MessageService> _logger;
        private readonly IConnection _connection;
        private readonly IConfiguration _configuration;
        private readonly IModel _channel;

        public MessageService(ILogger<MessageService> logger, IConfiguration configuration, IConnection connection)
        {
            _configuration = configuration;
            _logger = logger;

            // No Longer Needed
            //var rabbitMqConfig = _configuration.GetSection("RMQConfig").Get<RmqConfig>();
            //if (rabbitMqConfig == null)
            //{
            //    throw new ArgumentNullException("Rabbit MQ Config not set");
            //}
            //_logger.LogInformation("RabbitMQ Config: {0}", rabbitMqConfig.HostName);

            //var factory = new ConnectionFactory
            //{
            //    HostName = rabbitMqConfig.HostName,
            //    UserName = rabbitMqConfig.UserName,
            //    Password = rabbitMqConfig.Password
            //};

            _connection = connection;
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "demo-message-queue",
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        public void SendMessage(ServiceMessage message)
        {
            var messageText = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(messageText);
            var keyName = RoutingKeyName(message);

            _channel.BasicPublish(exchange: "",
                                 routingKey: "demo-message-queue",
                                 basicProperties: null,
                                 body: body);

            _logger.LogInformation("Message sent to RabbitMQ with Routing Key {0}", keyName);
        }

        private static string RoutingKeyName(ServiceMessage message)
        {
            var key = message.MessageType switch
            {
                DemoLib.Enumerations.MessageType.Email => "emails",
                DemoLib.Enumerations.MessageType.Analytics => "analytics",
                _ => "demo-message-queue"
            };
            return key;
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }

    public interface IMessageService
    {
        void SendMessage(ServiceMessage message);
    }

}