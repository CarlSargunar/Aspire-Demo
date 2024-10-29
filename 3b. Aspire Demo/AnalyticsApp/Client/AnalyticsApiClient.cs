using System.Net.Http;

namespace AnalyticsApp.Client
{
    public class AnalyticsApiClient(HttpClient httpClient)
    {
        public async Task<PageView[]> GetPageViews(int maxItems = 10, CancellationToken cancellationToken = default)
        {
            List<PageView>? pageViews = null;

            await foreach (var forecast in httpClient.GetFromJsonAsAsyncEnumerable<PageView>("/api/Analytics/pageviews", cancellationToken))
            {
                if (pageViews?.Count >= maxItems)
                {
                    break;
                }
                if (forecast is not null)
                {
                    pageViews ??= [];
                    pageViews.Add(forecast);
                }
            }

            return pageViews?.ToArray() ?? [];
        }
    }
}
