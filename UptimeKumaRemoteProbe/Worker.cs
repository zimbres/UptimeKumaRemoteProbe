using System.Net.NetworkInformation;
using UptimeKumaRemoteProbe.Models;

namespace UptimeKumaRemoteProbe;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;

    public Worker(ILogger<Worker> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _httpClient = _httpClientFactory.CreateClient();
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
                foreach (var item in configurations.Services)
                {
                    PingReply pingReply = ping.Send(item.Destination, configurations.Timeout);

                    if (pingReply.Status == IPStatus.Success)
                    {
                        try
                        {
                            await _httpClient.GetAsync($"{item.PushUri}{pingReply.RoundtripTime}");
                        }
                        catch
                        {
                            _logger.LogError($"Error trying to push results to {item.PushUri} at: {DateTimeOffset.Now}");
                        }
                    }
                    _logger.LogWarning($"Ping: {pingReply.Address} {pingReply.Status} at: {DateTimeOffset.Now}");
                }
            }
            await Task.Delay(configurations.Delay, stoppingToken);
        }
    }
}
