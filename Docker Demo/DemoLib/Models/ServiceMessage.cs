using System.ComponentModel.DataAnnotations;
using DemoLib.Enumerations;

namespace DemoLib.Models;

public class ServiceMessage
{
    [Key]
    public Guid MessageId { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Name { get; set; }
    public string MessageBody { get; set; }

    public MessageType MessageType { get; set; }
}
