using System.Diagnostics;
using System.Net.Sockets;
using UptimeKumaRemoteProbe.Models;

namespace UptimeKumaRemoteProbe.Services;

public class TcpService
{
    private readonly ILogger<TcpService> _logger;
    private readonly HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;

    public TcpService(ILogger<TcpService> logger, HttpClient httpClient, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClient;
        _httpClientFactory = httpClientFactory;
        _httpClient = _httpClientFactory.CreateClient();
    }

    public async Task CheckTcp(Endpoint endpoint)
    {
        var stopwatch = Stopwatch.StartNew();

        TcpClient tcpClient = new();

        try
        {
            await tcpClient.ConnectAsync(endpoint.Destination, endpoint.Port);
        }
        catch { }

        if (tcpClient.Connected)
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
        _logger.LogWarning($"Tcp: {endpoint.Destination}:{endpoint.Port} {tcpClient.Connected} at: {DateTimeOffset.Now}");
    }
}
