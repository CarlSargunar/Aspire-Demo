using MessageProcessor.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

using MessageProcessor.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using System;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder();

// Add RabbitMQ client
builder.AddRabbitMQClient(connectionName: "messaging");

// Add DbContext for the processor
builder.Services.AddDbContext<WorkerContext>(options => options.UseSqlServer("YourConnectionString"));

// Add logging
builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());

var app = builder.Build();

// Resolve services
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
var logger = app.Services.GetRequiredService<ILogger<Processor>>();
var connection = app.Services.GetRequiredService<IConnection>();

var processor = new Processor(logger, scopeFactory, connection);

// Create a CancellationTokenSource to manage cancellation
var cancellationTokenSource = new CancellationTokenSource();

Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true;
    cancellationTokenSource.Cancel();
};

// Run the processor
await processor.RunProcessorAsync(cancellationTokenSource.Token);

// Wait for the user to manually exit
Console.WriteLine("Press Ctrl+C to exit...");
await Task.Delay(Timeout.Infinite, cancellationTokenSource.Token);

processor.Dispose();
