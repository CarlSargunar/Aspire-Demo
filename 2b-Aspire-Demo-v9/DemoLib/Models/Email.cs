using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoLib.Models;

public class Email
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int MessageId { get; set; }
    public string EmailAddress { get; set; }
    public string Name { get; set; }
    public string Message { get; set; }

    // Navigation property
    public ServiceMessage ServiceMessage { get; set; }
}