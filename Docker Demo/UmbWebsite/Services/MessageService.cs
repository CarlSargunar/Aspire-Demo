using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace UmbWebsite.Services
{
    public class MessageService : IMessageService, IDisposable
    {
        private readonly ILogger<MessageService> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageService(ILogger<MessageService> logger)
        {
            _logger = logger;


            // TODO: Move connection details to configuration
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _connection = factory.CreateConnection();
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
                                 routingKey: keyName,
                                 basicProperties: null,
                                 body: body);

            _logger.LogInformation("Message sent to RabbitMQ with Routing Key {0}", keyName);
        }

        private static string RoutingKeyName(ServiceMessage message)
        {
            var key = message.MessageType switch
            {
                MessageType.Email => "emails",
                MessageType.Analytics => "analytics",
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
}