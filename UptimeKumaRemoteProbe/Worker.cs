namespace UptimeKumaRemoteProbe;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly PingService _pingService;
    private readonly HttpService _httpService;
    private readonly TcpService _tcpService;
    private readonly CertificateService _certificateService;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, PingService pingService,
        HttpService httpService, TcpService tcpService, CertificateService certificateService)
    {
        _logger = logger;
        _configuration = configuration;
        _pingService = pingService;
        _httpService = httpService;
        _tcpService = tcpService;
        _certificateService = certificateService;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var configurations = _configuration.GetSection(nameof(Configurations)).Get<Configurations>();

        Ping ping = new();
        PingReply? upReply = null;

        while (!stoppingToken.IsCancellationRequested)
        {
            if (configurations.UpDependency != "")
            {
                upReply = ping.Send(configurations.UpDependency, configurations.Timeout);
            }

            if (upReply is null || upReply.Status != IPStatus.Success)
            {
                _logger.LogError("Up Dependency is either not set or unreachable.");
            }

            if (upReply is not null && upReply.Status == IPStatus.Success)
            {
                foreach (var item in configurations.Endpoints)
                {
                    switch (item.Type)
                    {
                        case "Ping":
                            await _pingService.CheckPingAsync(item);
                            break;
                        case "Http":
                            await _httpService.CheckHttpAsync(item);
                            break;
                        case "Tcp":
                            await _tcpService.CheckTcpAsync(item);
                            break;
                        case "Certificate":
                            await _certificateService.CheckCertificateAsync(item);
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
