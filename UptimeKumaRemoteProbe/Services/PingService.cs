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
            // ex: ping gateway <1ms
            await pushService.PushUpAsync(endpoint.PushUpUri, pingReply.RoundtripTime == 0 ? 1 : pingReply.RoundtripTime);
        }
        else if (pingReply is not null)
        {
            await pushService.PushDownAsync(endpoint.PushDownUri, pingReply.RoundtripTime, pingReply.Status.ToString());
        }
        logger.LogInformation("Ping: {pingReply.Address} {pingReply.Status}", pingReply?.Address, pingReply?.Status);
    }
}
