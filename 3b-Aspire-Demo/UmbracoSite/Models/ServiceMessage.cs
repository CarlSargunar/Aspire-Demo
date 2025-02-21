using UmbracoSite.Enumerations;

namespace DemoLib.Models;

public class ServiceMessage
{
    public int Id { get; set; }
    public Guid MessageGuid { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Name { get; set; }
    public string MessageBody { get; set; }

    public MessageType MessageType { get; set; }
}
