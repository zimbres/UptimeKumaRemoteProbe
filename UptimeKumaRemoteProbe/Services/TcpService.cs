namespace UptimeKumaRemoteProbe.Services;

public class TcpService
{
    private readonly ILogger<TcpService> _logger;
    private readonly PushService _pushService;

    public TcpService(ILogger<TcpService> logger, PushService pushService)
    {
        _logger = logger;
        _pushService = pushService;
    }

    public async Task CheckTcpAsync(Endpoint endpoint)
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
            await _pushService.PushAsync(endpoint.PushUri, stopwatch.ElapsedMilliseconds);
        }
        _logger.LogWarning($"Tcp: {endpoint.Destination}:{endpoint.Port} {tcpClient.Connected} at: {DateTimeOffset.Now}");
    }
}
