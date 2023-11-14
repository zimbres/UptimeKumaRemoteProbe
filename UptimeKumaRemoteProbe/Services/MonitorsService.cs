namespace UptimeKumaRemoteProbe.Services;

public class MonitorsService
{
    private readonly ILogger<MonitorsService> _logger;
    private readonly Configurations _configuration;

    public MonitorsService(ILogger<MonitorsService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration.GetSection(nameof(Configurations)).Get<Configurations>();
    }

    public async Task<List<Monitors>> GetMonitorsAsync()
    {
        try
        {
            using var socket = new SocketIOClient.SocketIO(_configuration.Url);

            var data = new
            {
                username = _configuration.Username,
                password = _configuration.Password,
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
