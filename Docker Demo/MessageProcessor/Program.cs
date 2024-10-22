using MessageProcessor.Data;
using MessageProcessor.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var connectionString = hostContext.Configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<MessageProcessorDBContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddHostedService<Processor>();
    });

var app = builder.Build();
await app.RunAsync();