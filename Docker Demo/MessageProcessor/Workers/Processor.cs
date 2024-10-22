using DemoLib.Enumerations;
using DemoLib.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using MessageProcessor.Data;

namespace MessageProcessor.Workers;
public class Processor : BackgroundService
{
    private readonly ILogger<Processor> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private IConnection _connection;
    private IModel _channel;

    public Processor(ILogger<Processor> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;

        var factory = new ConnectionFactory()
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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Write a test message to the database at startup
        await WriteTestMessageAsync();

        // Set up RabbitMQ message consumption
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            _logger.LogInformation("Message received from RabbitMQ.");
            var body = ea.Body.ToArray();
            var message = System.Text.Json.JsonSerializer.Deserialize<ServiceMessage>(body);

            _logger.LogInformation("Received message: {0}", message?.MessageBody);

            // Process the message and write to the database
            await ProcessMessageAsync(message);
        };

        _channel.BasicConsume(queue: "demo-message-queue", autoAck: true, consumer: consumer);

        // Keep the service alive
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Processor running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task WriteTestMessageAsync()
    {
        _logger.LogInformation("Writing test message to the database.");
        using (var scope = _scopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<WorkerContext>();
            var testMessage = new ServiceMessage
            {
                Name = "Test",
                MessageBody = "This is a test message",
                MessageType = MessageType.Information
            };
            dbContext.ServiceMessages.Add(testMessage);
            await dbContext.SaveChangesAsync();
        }

        _logger.LogInformation("Test message written to the database.");
    }

    private async Task ProcessMessageAsync(ServiceMessage message)
    {
        _logger.LogInformation("Processing message: {0}", message.MessageBody);
        using (var scope = _scopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<WorkerContext>();
            dbContext.ServiceMessages.Add(message);
            await dbContext.SaveChangesAsync();

            _logger.LogInformation("Message processed and saved to the database.");
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}