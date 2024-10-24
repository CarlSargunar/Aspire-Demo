using DemoLib.Enumerations;
using DemoLib.Models;
using MessageProcessor.Data;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;

public class Processor
{
    private readonly ILogger<Processor> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private IConnection _connection;
    private IModel _channel;

    public Processor(ILogger<Processor> logger, IServiceScopeFactory scopeFactory, IConnection connection)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;

        _connection = connection;
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "demo-message-queue",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
    }

    public async Task RunProcessorAsync(CancellationToken stoppingToken)
    {
        await WriteTestMessageAsync();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                _logger.LogInformation("Message received from RabbitMQ.");
                var body = ea.Body.ToArray();
                var message = System.Text.Json.JsonSerializer.Deserialize<ServiceMessage>(body);

                _logger.LogInformation("Received message: {0}", message?.MessageBody);
                await ProcessMessageAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message.");
            }
        };

        var consumerTag = _channel.BasicConsume(queue: "demo-message-queue", autoAck: true, consumer: consumer);

        stoppingToken.Register(() =>
        {
            _logger.LogInformation("Stopping RabbitMQ consumer.");
            _channel.BasicCancel(consumerTag);
        });

        while (!stoppingToken.IsCancellationRequested)
        {
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
                MessageBody = "Worker connected to the database",
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

            if (message.MessageType == MessageType.Analytics)
            {
                var pageView = dbContext.PageViews.FirstOrDefault(pv => pv.URL == message.MessageBody);
                if (pageView != null)
                {
                    pageView.Count++;
                    pageView.LastAccessed = DateTime.UtcNow;
                    _logger.LogInformation("PageView updated for URL: {0}", message.MessageBody);
                }
                else
                {
                    pageView = new PageView
                    {
                        URL = message.MessageBody,
                        LastAccessed = DateTime.UtcNow,
                        Count = 1
                    };
                    dbContext.PageViews.Add(pageView);
                    _logger.LogInformation("New PageView created for URL: {0}", message.MessageBody);
                }
            }

            await dbContext.SaveChangesAsync();
            _logger.LogInformation("Message processed and saved to the database. ID: {0}", message.Id);
        }
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}
