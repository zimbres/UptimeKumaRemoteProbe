using System.Net.NetworkInformation;
using UptimeKumaRemoteProbe.Models;

namespace UptimeKumaRemoteProbe.Services;

public class PingService
{
    private readonly ILogger<PingService> _logger;
    private readonly HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;

    public PingService(ILogger<PingService> logger, HttpClient httpClient, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClient;
        _httpClientFactory = httpClientFactory;
        _httpClient = _httpClientFactory.CreateClient();
    }

    public async Task CheckPing(Endpoint endpoint)
    {
        Ping ping = new();
        PingReply pingReply = ping.Send(endpoint.Destination, endpoint.Timeout);

        if (pingReply.Status == IPStatus.Success)
        {
            try
            {
                await _httpClient.GetAsync($"{endpoint.PushUri}{pingReply.RoundtripTime}");
            }
            catch
            {
                _logger.LogError($"Error trying to push results to {endpoint.PushUri} at: {DateTimeOffset.Now}");
            }
        }
        _logger.LogWarning($"Ping: {pingReply.Address} {pingReply.Status} at: {DateTimeOffset.Now}");
    }
}
