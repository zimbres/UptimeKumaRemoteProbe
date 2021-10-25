using System.Diagnostics;
using UptimeKumaRemoteProbe.Models;

namespace UptimeKumaRemoteProbe.Services;

public class HttpService
{
    private readonly ILogger<HttpService> _logger;
    private HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;

    public HttpService(ILogger<HttpService> logger, HttpClient httpClient, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClient;
        _httpClientFactory = httpClientFactory;
    }

    public async Task CheckHttp(Endpoint endpoint)
    {
        _httpClient = _httpClientFactory.CreateClient(endpoint.IgnoreSSL ? "IgnoreSSL" : "Default");

        var stopwatch = Stopwatch.StartNew();

        string content;

        HttpResponseMessage result;

        try
        {
            result = await _httpClient.GetAsync(endpoint.Destination);
            content = await result.Content.ReadAsStringAsync();

            _logger.LogWarning($"Http: {endpoint.Destination} {result.StatusCode} at: {DateTimeOffset.Now}");

            if (endpoint.Keyword != "" && !content.Contains(endpoint.Keyword)) throw new Exception();
        }
        catch
        {
            _logger.LogError($"Error trying get {endpoint.Destination} at: {DateTimeOffset.Now}");
            return;
        }

        if (result is not null && result.IsSuccessStatusCode)
        {
            try
            {
                await _httpClient.GetAsync($"{endpoint.PushUri}{stopwatch.ElapsedMilliseconds}");
            }
            catch
            {
                _logger.LogError($"Error trying to push results to {endpoint.PushUri} at: {DateTimeOffset.Now}");
            }
        }
    }
}
