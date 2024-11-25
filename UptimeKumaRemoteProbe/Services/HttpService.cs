namespace UptimeKumaRemoteProbe.Services;
using System.Net;

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

            _logger.LogInformation("Http: {endpoint.Destination} {result.StatusCode}",
                endpoint.Destination, result.StatusCode);

            if (endpoint.Keyword != "" && !content.Contains(endpoint.Keyword)) throw new ArgumentNullException(nameof(endpoint), "Keyword not found.");
        }
        catch
        {
            _logger.LogError("Error trying get {endpoint.Destination}", endpoint.Destination);
            return;
        }

        if (result is not null )
        {
            if (endpoint.AllResults && !result.IsSuccessStatusCode)
            {
                var pushUri = new Uri($"{endpoint.BasePushUri}status=down&msg={result.StatusCode}&ping=");
                await _pushService.PushAsync(pushUri, stopwatch.ElapsedMilliseconds);
            }
            else if (result.IsSuccessStatusCode)
            {
                await _pushService.PushAsync(endpoint.PushUri, stopwatch.ElapsedMilliseconds);
            }
        }
    }
}