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

    public async Task CheckPingAsync(Endpoint endpoint)
    {
        Ping ping = new();
        PingReply pingReply = null;

        try
        {
            pingReply = ping.Send(endpoint.Destination, endpoint.Timeout);
        }
        catch
        {
            // Ignore
        }

        if (pingReply?.Status == IPStatus.Success)
        {
            await _pushService.PushAsync(endpoint.PushUri, pingReply.RoundtripTime);
        }
        _logger.LogInformation("Ping: {pingReply.Address} {pingReply.Status}", pingReply?.Address, pingReply?.Status);
    }
}
