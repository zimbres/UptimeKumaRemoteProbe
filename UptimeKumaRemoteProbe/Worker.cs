using System.Net.NetworkInformation;
using UptimeKumaRemoteProbe.Models;
using UptimeKumaRemoteProbe.Services;

namespace UptimeKumaRemoteProbe;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly PingService _pingService;
    private readonly HttpService _httpService;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, PingService pingService, HttpService httpService)
    {
        _logger = logger;
        _configuration = configuration;
        _pingService = pingService;
        _httpService = httpService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var configurations = _configuration.GetSection(nameof(Configurations)).Get<Configurations>();

        Ping ping = new();
        PingReply upReply = null;

        while (!stoppingToken.IsCancellationRequested)
        {
            if (configurations.UpDependency != "")
            {
                upReply = ping.Send(configurations.UpDependency, configurations.Timeout);
            }

            if (upReply is not null && upReply.Status == IPStatus.Success)
            {
                foreach (var item in configurations.Endpoints)
                {
                    switch (item.Type)
                    {
                        case "Ping":
                            await _pingService.CheckPing(item);
                            break;
                        case "Http":
                            await _httpService.CheckHttp(item);
                            break;
                        default:
                            break;
                    }
                }
            }
            await Task.Delay(configurations.Delay, stoppingToken);
        }
    }
}
