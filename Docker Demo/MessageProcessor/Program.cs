using MessageProcessor.Data;
using MessageProcessor.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddDbContext<MessageProcessorDBContext>(options =>
            options.UseSqlServer("DefaultConnection"));

        services.AddHostedService<Processor>();
    });

var app = builder.Build();
await app.RunAsync();