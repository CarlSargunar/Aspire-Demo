namespace AnalyticsApp.Client;

public class PageView
{
    public int Id { get; set; }
    public string Url { get; set; }
    public DateTime LastAccessed { get; set; }
    public int Count { get; set; }
}