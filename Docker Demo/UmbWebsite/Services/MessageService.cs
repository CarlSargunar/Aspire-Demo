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
                HostName = "rabbitmq",
                UserName = "guest",
                Password = "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "messages",
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        public void SendMessage(Message message)
        {
            var messageText = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(messageText);

            _channel.BasicPublish(exchange: "",
                                 routingKey: "messages",
                                 basicProperties: null,
                                 body: body);

            _logger.LogInformation("Message sent to RabbitMQ");
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}