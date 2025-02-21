using UmbracoSite.Enumerations;

namespace DemoLib.Models;

// Duplicate of DemoLib.Models.ServiceMessage - becuase if I reference the DemoLib project, throws an error
// System.TypeLoadException: 'Method 'get_LockReleaseBehavior' in type 'Microsoft.EntityFrameworkCore.Sqlite.Migrations.Internal.SqliteHistoryRepository' from assembly 'Microsoft.EntityFrameworkCore.Sqlite, Version=8.0.11.0, Culture=neutral, PublicKeyToken=adb9793829ddae60' does not have an implementation.'
public class ServiceMessage
{
    public int Id { get; set; }
    public Guid MessageGuid { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Name { get; set; }
    public string MessageBody { get; set; }

    public MessageType MessageType { get; set; }
}
