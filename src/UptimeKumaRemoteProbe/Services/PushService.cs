namespace UptimeKumaRemoteProbe.Services;

public class PushService
{
    private readonly ILogger<PushService> _logger;
    private readonly HttpClient _httpClient;

    public PushService(ILogger<PushService> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("IgnoreSSL");
    }

    public async Task PushAsync(Uri uri, long elapsedMilliseconds)
    {
        try
        {
            await _httpClient.GetAsync($"{uri}{elapsedMilliseconds}");
        }
        catch
        {
            _logger.LogError("Error trying to push results to {uri}", uri);
        }
    }
}
