using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DemoLib.Enumerations;

namespace DemoLib.Models;

public class ServiceMessage
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public Guid MessageGuid { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Name { get; set; }
    public string MessageBody { get; set; }

    public MessageType MessageType { get; set; }
}
