namespace UptimeKumaRemoteProbe.Services;

public class MonitorsService
{
    private readonly ILogger<MonitorsService> _logger;
    private readonly AppSettings _appSettings;

    public MonitorsService(ILogger<MonitorsService> logger, AppSettings appSettings)
    {
        _logger = logger;
        _appSettings = appSettings;
    }

    public async Task<List<Monitors>> GetMonitorsAsync()
    {
        try
        {
            using var socket = new SocketIOClient.SocketIO(_appSettings.Url, new SocketIOClient.SocketIOOptions
            {
                ReconnectionAttempts = 3
            });

            var data = new
            {
                username = _appSettings.Username,
                password = _appSettings.Password,
                token = ""
            };

            JsonElement monitorsRaw = new();

            socket.On("monitorList", response =>
            {
                monitorsRaw = response.GetValue<JsonElement>();
            });

            socket.OnConnected += async (sender, e) =>
            {
                await socket.EmitAsync("login", (ack) =>
                {
                    var result = JsonNode.Parse(ack.GetValue<JsonElement>(0).ToString());
                    if (result["ok"].ToString() != "true")
                    {
                        _logger.LogError("Uptime Kuma login failure");
                    }
                }, data);
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
    }
}
