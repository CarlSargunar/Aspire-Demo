using UmbracoSite.Middleware;
using UmbracoSite.Services;

namespace UmbracoSite.Configuration;
public static class MyBuilderExtensions
{
    public static IUmbracoBuilder RegisterCustomServices(this IUmbracoBuilder builder)
    {
        builder.Services.AddSingleton<IStartupFilter, MiddlewareStartupFilter>();
        builder.Services.AddSingleton<IMessageService, MessageService>();
        return builder;
    }

    public static IUmbracoBuilder AddCustomServices(this IUmbracoBuilder builder)
    {
        builder.RegisterCustomServices();
        return builder;
    }
}