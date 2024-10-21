using Microsoft.AspNetCore.Http;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public class AnalyticsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConnectionFactory _connectionFactory;

    public AnalyticsMiddleware(RequestDelegate next, IConnectionFactory connectionFactory)
    {
        _next = next;
        _connectionFactory = connectionFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if the request is for static assets (CSS/JS/images)
        if (!IsStaticAsset(context.Request.Path))
        {
            // This is an Umbraco content route, send message to RabbitMQ
            SendMessageToQueue(context.Request.Path);
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

    private void SendMessageToQueue(string path)
    {
        using (var connection = _connectionFactory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "content_routes", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var body = Encoding.UTF8.GetBytes($"Content route accessed: {path}");

            channel.BasicPublish(exchange: "", routingKey: "content_routes", basicProperties: null, body: body);
        }
    }
}