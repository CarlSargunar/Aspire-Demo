using DemoLib.Enumerations;
using DemoLib.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using MessageProcessor.Data;
using DemoLib.Config;

namespace MessageProcessor.Workers;
public class Processor : BackgroundService
{
    private readonly ILogger<Processor> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private IConnection _connection;
    private IConfiguration _configuration;
    private IModel _channel;

    public Processor(ILogger<Processor> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _configuration = configuration;

        var rabbitMqConfig = _configuration.GetSection("RMQConfig").Get<RmqConfig>();
        if (rabbitMqConfig == null)
        {
            throw new ArgumentNullException("Rabbit MQ Config not set");
        }
        _logger.LogInformation("RabbitMQ Config: {0}", rabbitMqConfig.HostName);


        var factory = new ConnectionFactory()
        {
            HostName = rabbitMqConfig.HostName,
            UserName = rabbitMqConfig.UserName,
            Password = rabbitMqConfig.Password
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
            try
            {
                _logger.LogInformation("Message received from RabbitMQ.");
                var body = ea.Body.ToArray();
                var message = System.Text.Json.JsonSerializer.Deserialize<ServiceMessage>(body);

                _logger.LogInformation("Received message: {0}", message?.MessageBody);

                // Process the message and write to the database
                await ProcessMessageAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message.");
            }
        };

        _channel.BasicConsume(queue: "demo-message-queue", autoAck: true, consumer: consumer);

        // Keep the service alive
        int i = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            if (i % 10 == 0)
            {
                _logger.LogInformation("Processor running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task WriteTestMessageAsync()
    {
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