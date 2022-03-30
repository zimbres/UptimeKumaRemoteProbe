namespace UptimeKumaRemoteProbe.Services;

public class HttpService
{
    private readonly ILogger<HttpService> _logger;
    private HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly PushService _pushService;

    public HttpService(ILogger<HttpService> logger, HttpClient httpClient, IHttpClientFactory httpClientFactory, PushService pushService)
    {
        _logger = logger;
        _httpClient = httpClient;
        _httpClientFactory = httpClientFactory;
        _pushService = pushService;
    }

    public async Task CheckHttpAsync(Endpoint endpoint)
    {
        _httpClient = _httpClientFactory.CreateClient(endpoint.IgnoreSSL ? "IgnoreSSL" : "Default");

        var stopwatch = Stopwatch.StartNew();

        string content;

        HttpResponseMessage result;

        try
        {
            result = await _httpClient.GetAsync(endpoint.Destination);
            content = await result.Content.ReadAsStringAsync();

            _logger.LogWarning("Http: {endpoint.Destination} {result.StatusCode} at: {DateTimeOffset.Now}",
                endpoint.Destination, result.StatusCode, DateTimeOffset.Now);

            if (endpoint.Keyword != "" && !content.Contains(endpoint.Keyword)) throw new ArgumentNullException(nameof(endpoint), "Keyword not found.");
        }
        catch
        {
            _logger.LogError("Error trying get {endpoint.Destination} at: {DateTimeOffset.Now}", endpoint.Destination, DateTimeOffset.Now);
            return;
        }

        if (result is not null && result.IsSuccessStatusCode)
        {
            await _pushService.PushAsync(endpoint.PushUri, stopwatch.ElapsedMilliseconds);
        }
    }
}
