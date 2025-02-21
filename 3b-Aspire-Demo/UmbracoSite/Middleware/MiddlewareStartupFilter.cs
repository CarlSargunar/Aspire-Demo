namespace UmbracoSite.Middleware;

public class MiddlewareStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => app =>
    {
        app.UseMiddleware<AnalyticsMiddleware>();
        next(app);
    };
}