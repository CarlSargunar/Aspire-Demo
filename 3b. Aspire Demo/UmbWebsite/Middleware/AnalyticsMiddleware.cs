using DemoLib.Models;
using System.Text.RegularExpressions;
using DemoLib.Enumerations;
using UmbWebsite.Services;

namespace UmbWebsite.Middleware;


public class AnalyticsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMessageService _messageService;

    public AnalyticsMiddleware(RequestDelegate next, IMessageService messageService)
    {
        _next = next;
        _messageService = messageService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if the request is for static assets (CSS/JS/images)
        if (context.Request.Method == HttpMethods.Get && !IsStaticAsset(context.Request.Path))
        {
            var message = new ServiceMessage
            {
                Name = "URL",
                MessageBody = context.Request.Path,
                MessageType = MessageType.Analytics
            };
            _messageService.SendMessage(message);
        }

        // Call the next middleware in the pipeline
        await _next(context);
    }

    private bool IsStaticAsset(string path)
    {
        // Regex pattern to match typical static asset extensions
        var staticAssetPattern = @"\.(css|js|png|jpg|jpeg|gif|svg|ico)$";
        return Regex.IsMatch(path, staticAssetPattern, RegexOptions.IgnoreCase);
    }
}