namespace UptimeKumaRemoteProbe.Services;

public class MonitorsService
{
    private readonly ILogger<MonitorsService> _logger;
    private readonly AppSettings _appSettings;
    private readonly WebSocketOptions _webSocketOptions;

    public MonitorsService(ILogger<MonitorsService> logger, AppSettings appSettings, WebSocketOptions webSocketOptions)
    {
        _logger = logger;
        _appSettings = appSettings;
        _webSocketOptions = webSocketOptions;
    }

    public async Task<List<Monitors>> GetMonitorsAsync()
    {
        SocketIOClient.SocketIO socket = null;
        try
        {
            socket = new SocketIOClient.SocketIO(
                new Uri(_appSettings.Url),
                new SocketIOClient.SocketIOOptions
                {
                    ReconnectionAttempts = 3
                },
                services =>
                {
                    services.AddSingleton(_webSocketOptions);
                });

            var data = new
            {
                username = _appSettings.Username,
                password = _appSettings.Password,
                token = ""
            };

            JsonElement monitorsRaw = new();

            socket.On("monitorList", ctx =>
            {
                monitorsRaw = ctx.GetValue<JsonElement>(0);
                return Task.CompletedTask;
            });

            socket.OnConnected += async (sender, e) =>
            {
                await socket.EmitAsync("login", new object[] { data }, ack =>
                {
                    var result = JsonNode.Parse(ack.GetValue<JsonElement>(0).ToString());
                    if (result["ok"].ToString() != "true")
                    {
                        _logger.LogError("Uptime Kuma login failure");
                    }
                    return Task.CompletedTask;
                });
            };

            await socket.ConnectAsync();

            int round = 0;
            while (monitorsRaw.ValueKind == JsonValueKind.Undefined)
            {
                round++;
                await Task.Delay(1000);
                if (round >= 10) break;
            }

            await socket.DisconnectAsync();
            var monitors = JsonSerializer.Deserialize<Dictionary<string, Monitors>>(monitorsRaw);
            return monitors.Values.ToList();
        }
        catch
        {
            _logger.LogError("Error trying to get monitors");
            return null;
        }
        finally
        {
            socket?.Dispose();
        }
    }
}