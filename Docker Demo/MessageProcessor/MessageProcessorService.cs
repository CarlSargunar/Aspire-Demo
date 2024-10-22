using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json; 
using DemoLib.Models;
using MessageProcessor.Data;

namespace MessageProcessor;
public class RabbitMqListenerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private IConnection _connection;
    private IModel _channel;

    public RabbitMqListenerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        var factory = new ConnectionFactory() { HostName = "rabbitmq" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "message_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var messageString = Encoding.UTF8.GetString(body);
            var message = JsonSerializer.Deserialize<ServiceMessage>(messageString);

            // Handle message - e.g., save to DB
            await SaveMessageAsync(message);
        };

        _channel.BasicConsume(queue: "message_queue", autoAck: true, consumer: consumer);
        return Task.CompletedTask;
    }

    private async Task SaveMessageAsync(ServiceMessage message)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<MessageProcessorDBContext>();
            dbContext.ServiceMessages.Add(message);
            await dbContext.SaveChangesAsync();
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}