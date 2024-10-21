using UmbWebsite.Middleware;
using UmbWebsite.Services;

namespace UmbWebsite.Configuration;

public static class MyCustomBuilderExtensions
{
    public static IUmbracoBuilder RegisterCustomServices(this IUmbracoBuilder builder)
    {
        builder.Services.AddSingleton<IMessageService, MessageService>();
        builder.Services.AddSingleton<IStartupFilter, MiddlewareStartupFilter>();
        return builder;
    }

    public static IUmbracoBuilder AddCustomServices(this IUmbracoBuilder builder)
    {
        builder.RegisterCustomServices();
        return builder;
    }
}