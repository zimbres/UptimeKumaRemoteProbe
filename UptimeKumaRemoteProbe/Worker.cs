namespace UptimeKumaRemoteProbe;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly PingService _pingService;
    private readonly HttpService _httpService;
    private readonly TcpService _tcpService;
    private readonly CertificateService _certificateService;
    private readonly DbService _dbService;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, PingService pingService, HttpService httpService,
        TcpService tcpService, CertificateService certificateService, DbService dbService)
    {
        _logger = logger;
        _configuration = configuration;
        _pingService = pingService;
        _httpService = httpService;
        _tcpService = tcpService;
        _certificateService = certificateService;
        _dbService = dbService;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var configurations = _configuration.GetSection(nameof(Configurations)).Get<Configurations>();

        if (configurations.UpDependency == "")
        {
            _logger.LogWarning("Up Dependency is not set.");
            Environment.Exit(0);
        }

        Ping ping = new();
        PingReply pingReply = null;

        while (!stoppingToken.IsCancellationRequested)
        {
            if (configurations.UpDependency != "")
            {
                try
                {
                    pingReply = ping.Send(configurations.UpDependency, configurations.Timeout);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Network is unreachable. {ex}", ex.Message);
                }
            }

            if (pingReply?.Status == IPStatus.Success)
            {
                await LoopAsync(configurations);
            }
            else
            {
                _logger.LogError("Up Dependency is unreachable.");
            }
            await Task.Delay(configurations.Delay, stoppingToken);
        }
    }

    private async Task LoopAsync(Configurations configurations)
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
                case "DataBase":
                    await _dbService.CheckDbAsync(item);
                    break;
                default:
                    break;
            }
        }
    }
}
