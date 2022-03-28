namespace UptimeKumaRemoteProbe.Services;

public class PushService
{
    private readonly ILogger<PushService> _logger;
    private readonly HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;
    public PushService(ILogger<PushService> logger, HttpClient httpClient, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClient;
        _httpClientFactory = httpClientFactory;
        _httpClient = _httpClientFactory.CreateClient();
    }

    public async Task PushAsync(Uri uri, long elapsedMilliseconds)
    {
        try
        {
            await _httpClient.GetAsync($"{uri}{elapsedMilliseconds}");
        }
        catch
        {
            _logger.LogError("Error trying to push results to {uri} at: {DateTimeOffset.Now}", uri, DateTimeOffset.Now);
        }
    }
}
