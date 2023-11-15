namespace UptimeKumaRemoteProbe.Services;

public class PushService(ILogger<PushService> logger, HttpClient httpClient, IHttpClientFactory httpClientFactory)
{
    public async Task PushAsync(Uri uri, long elapsedMilliseconds)
    {
        httpClient = httpClientFactory.CreateClient();

        try
        {
            await httpClient.GetAsync($"{uri}{elapsedMilliseconds}");
        }
        catch
        {
            logger.LogError("Error trying to push results to {uri}", uri);
        }
    }
}
