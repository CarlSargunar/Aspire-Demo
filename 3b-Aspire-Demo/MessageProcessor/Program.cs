using MessageProcessor.Data;
using MessageProcessor.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Processor>();
builder.AddRabbitMQClient(connectionName: "messaging");

// Wait for SQL Server to be ready - horrible hack
// Will no longer be required in Aspire 9, with WaitFor
Thread.Sleep(20000);


builder.AddServiceDefaults();

builder.Services.AddDbContextPool<WorkerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("reportingdb"), sqlOptions =>
    {
        // Workaround for https://github.com/dotnet/aspire/issues/1023
        sqlOptions.ExecutionStrategy(c => new RetryingSqlServerRetryingExecutionStrategy(c));
    }));


var app = builder.Build();
await app.RunAsync();