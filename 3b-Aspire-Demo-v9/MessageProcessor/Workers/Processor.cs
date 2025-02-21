using DemoLib.Enumerations;
using DemoLib.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using MessageProcessor.Data;
using DemoLib.Config;
using Microsoft.EntityFrameworkCore;

namespace MessageProcessor.Workers;
public class Processor : BackgroundService
{
    private readonly ILogger<Processor> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private IConnection _connection;
    private IConfiguration _configuration;
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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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

        // Consume messages
        var consumerTag = _channel.BasicConsume(queue: "demo-message-queue", autoAck: true, consumer: consumer);

        // Wait until cancellation is requested
        stoppingToken.Register(() =>
        {
            _logger.LogInformation("Stopping RabbitMQ consumer.");
            _channel.BasicCancel(consumerTag); // Gracefully stop consuming
        });

        // Keep the service alive until cancellation is requested
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);  // Ensure graceful exit on cancellation
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


            // Todo : handle other message types
            if (message.MessageType == MessageType.Analytics)
            {
                // Try to find the existing PageView by URL (matching message.MessageBody)
                var pageView = await dbContext.PageViews.FirstOrDefaultAsync(pv => pv.URL == message.MessageBody);

                if (pageView != null)
                {
                    // If found, increment the view count and update the LastAccessed field
                    pageView.Count++;
                    pageView.LastAccessed = DateTime.UtcNow;
                    _logger.LogInformation("PageView updated for URL: {0}", message.MessageBody);
                }
                else
                {
                    // If not found, create a new PageView entry
                    pageView = new PageView
                    {
                        URL = message.MessageBody,
                        LastAccessed = DateTime.UtcNow,
                        Count = 1
                    };
                    dbContext.PageViews.Add(pageView);
                    _logger.LogInformation("New PageView created for URL: {0}", message.MessageBody);
                }
                _logger.LogInformation("Analytics message processed and PageView saved to the database. URL: {0}", message.MessageBody);
            }

            await dbContext.SaveChangesAsync();

            _logger.LogInformation("Message processed and saved to the database. ID: {0}", message.Id);
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}