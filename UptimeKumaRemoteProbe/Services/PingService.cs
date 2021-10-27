using System.Net.NetworkInformation;
using UptimeKumaRemoteProbe.Models;

namespace UptimeKumaRemoteProbe.Services;

public class PingService
{
    private readonly ILogger<PingService> _logger;
    private readonly PushService _pushService;

    public PingService(ILogger<PingService> logger, PushService pushService)
    {
        _logger = logger;
        _pushService = pushService;
    }

    public async Task CheckPing(Endpoint endpoint)
    {
        Ping ping = new();
        PingReply pingReply = ping.Send(endpoint.Destination, endpoint.Timeout);

        if (pingReply.Status == IPStatus.Success)
        {
            await _pushService.Push(endpoint.PushUri, pingReply.RoundtripTime);
        }
        _logger.LogWarning($"Ping: {pingReply.Address} {pingReply.Status} at: {DateTimeOffset.Now}");
    }
}
