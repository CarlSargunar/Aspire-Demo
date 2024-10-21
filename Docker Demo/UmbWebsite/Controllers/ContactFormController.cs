using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;
using UmbWebsite.Models;

namespace UmbWebsite.Controllers
{
    public class ContactFormController : SurfaceController
    {
        private readonly ILogger<ContactFormController> _logger;

        public ContactFormController(ILogger<ContactFormController> logger,
                                     IUmbracoContextAccessor umbracoContextAccessor,
                                     IUmbracoDatabaseFactory databaseFactory,
                                     ServiceContext services,
                                     AppCaches appCaches,
                                     IProfilingLogger profilingLogger,
                                     IPublishedUrlProvider publishedUrlProvider) : base(umbracoContextAccessor,
                                                                                        databaseFactory,
                                                                                        services,
                                                                                        appCaches,
                                                                                        profilingLogger,
                                                                                        publishedUrlProvider)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Submit(ContactFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ModelState"] = ModelState;
                _logger.LogWarning("Invalid form submission");
                return CurrentUmbracoPage();
            }

            try
            {
                // Work with form data here
                var message = new Message
                {
                    Name = model.Name,
                    MessageText = model.Message,
                    MessageType = MessageType.Email
                };

                // Send message to RabbitMQ
                SendMessageToQueue(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting form");
                TempData["Message"] = "There was an error submitting your form, please try again later.";
                return RedirectToCurrentUmbracoPage();
            }

            TempData["Message"] = "Thanks for your email, someone will get back to you ASAP!";
            return RedirectToCurrentUmbracoPage();
        }

        private void SendMessageToQueue(Message message)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" }; // Replace with your RabbitMQ hostname
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "contact_form_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var messageBody = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(messageBody);

                channel.BasicPublish(exchange: "", routingKey: "contact_form_queue", basicProperties: null, body: body);
            }
        }

    }
}
