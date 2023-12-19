namespace UptimeKumaRemoteProbe.Services;

public class PushService(ILogger<PushService> logger, HttpClient httpClient, IHttpClientFactory httpClientFactory)
{
    public async Task PushUpAsync(Uri uri, long elapsedMilliseconds)
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

    public async Task PushDownAsync(Uri uri, long elapsedMilliseconds, string msg = "")
    {
        httpClient = httpClientFactory.CreateClient();

        try
        {
            await httpClient.GetAsync($"{uri}{elapsedMilliseconds}&msg={msg ?? ""}");
        }
        catch
        {
            logger.LogError("Error trying to push results to {uri}", uri);
        }
    }
}
