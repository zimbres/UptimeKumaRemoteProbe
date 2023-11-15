namespace UptimeKumaRemoteProbe.Services;

public class PingService(ILogger<PingService> logger, PushService pushService)
{
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
            await pushService.PushAsync(endpoint.PushUri, pingReply.RoundtripTime);
        }
        logger.LogInformation("Ping: {pingReply.Address} {pingReply.Status}", pingReply?.Address, pingReply?.Status);
    }
}
