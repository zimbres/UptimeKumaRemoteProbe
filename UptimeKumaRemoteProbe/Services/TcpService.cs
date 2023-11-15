namespace UptimeKumaRemoteProbe.Services;

public class TcpService(ILogger<TcpService> logger, PushService pushService)
{
    public async Task CheckTcpAsync(Endpoint endpoint)
    {
        var stopwatch = Stopwatch.StartNew();

        TcpClient tcpClient = new();

        try
        {
            await tcpClient.ConnectAsync(endpoint.Destination, endpoint.Port);
        }
        catch
        {
            // Ignore
        }

        if (tcpClient.Connected)
        {
            await pushService.PushAsync(endpoint.PushUri, stopwatch.ElapsedMilliseconds);
        }
        logger.LogInformation("Tcp: {endpoint.Destination}:{endpoint.Port} Success={tcpClient.Connected}",
            endpoint.Destination, endpoint.Port, tcpClient.Connected);

        tcpClient.Dispose();
    }
}
