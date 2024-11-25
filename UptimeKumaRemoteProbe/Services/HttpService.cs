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
            string status = result.StatusCode == HttpStatusCode.OK ? "up" : "down";
            var pushUri_http = new Uri($"{endpoint.PushUri_http}status={status}&msg={result.StatusCode}&ping=");

            await _pushService.PushAsync(PushUri_http, stopwatch.ElapsedMilliseconds);
        }
    }
}