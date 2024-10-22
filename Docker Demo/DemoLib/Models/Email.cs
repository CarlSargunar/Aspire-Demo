using System.ComponentModel.DataAnnotations;

namespace DemoLib.Models;

public class Email
{
    [Key]
    public int Id { get; set; }
    public Guid MessageId { get; set; }
    public string EmailAddress { get; set; }
    public string Name { get; set; }
    public string Message { get; set; }

    // Navigation property
    public ServiceMessage ServiceMessage { get; set; }
}