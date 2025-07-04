namespace UmbracoSite.Enumerations
{
    // Duplicate of DemoLib.Enumerations.MessageType - becuase if I reference the DemoLib project, throws an error
    // System.TypeLoadException: 'Method 'get_LockReleaseBehavior' in type 'Microsoft.EntityFrameworkCore.Sqlite.Migrations.Internal.SqliteHistoryRepository' from assembly 'Microsoft.EntityFrameworkCore.Sqlite, Version=8.0.11.0, Culture=neutral, PublicKeyToken=adb9793829ddae60' does not have an implementation.'

    public enum MessageType
    {
        Undefines = 0,
        Analytics = 1,
        Email = 2,
        Exception = 3,
        Information = 4
    }
}
