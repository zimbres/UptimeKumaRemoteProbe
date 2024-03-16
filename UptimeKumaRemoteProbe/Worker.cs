namespace UptimeKumaRemoteProbe;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly Configurations _configurations;
    private readonly PingService _pingService;
    private readonly HttpService _httpService;
    private readonly TcpService _tcpService;
    private readonly CertificateService _certificateService;
    private readonly DbService _dbService;
    private readonly MonitorsService _monitorsService;
    private readonly AppSettings _appSettings;
    private readonly DomainService _domainService;
    private readonly VersionService _versionService;
    private static DateOnly lastDailyExecution;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, AppSettings appSettings, PingService pingService, HttpService httpService,
        TcpService tcpService, CertificateService certificateService, DbService dbService, MonitorsService monitorsService,
        DomainService domainService, VersionService versionService)
    {
        _logger = logger;
        _configurations = configuration.GetSection(nameof(Configurations)).Get<Configurations>();
        _appSettings = appSettings;
        _pingService = pingService;
        _httpService = httpService;
        _tcpService = tcpService;
        _certificateService = certificateService;
        _dbService = dbService;
        _monitorsService = monitorsService;
        _domainService = domainService;
        _versionService = versionService;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogWarning("App version: {version}", Assembly.GetExecutingAssembly().GetName().Version.ToString());

        if (await _versionService.CheckVersionAsync())
        {
            Environment.Exit(0);
        }

        if (_appSettings.UpDependency == "")
        {
            _logger.LogError("Up Dependency is not set.");
            Environment.Exit(0);
        }

        Ping ping = new();
        PingReply pingReply = null;

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_appSettings.UpDependency != "")
            {
                try
                {
                    pingReply = ping.Send(_appSettings.UpDependency, _appSettings.Timeout);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Network is unreachable. {ex}", ex.Message);
                }
            }

            if (pingReply?.Status == IPStatus.Success)
            {
                var monitors = await _monitorsService.GetMonitorsAsync();
                if (monitors is not null)
                {
                    var endpoints = ParseEndpoints(monitors);
                    await LoopAsync(endpoints);
                }
            }
            else
            {
                _logger.LogError("Up Dependency is unreachable.");
            }
            await Task.Delay(_appSettings.Delay, stoppingToken);
        }
    }

    private List<Endpoint> ParseEndpoints(List<Monitors> monitors)
    {
        var endpoints = new List<Endpoint>();
        bool hasProbeMonitor = false;

        foreach (var monitor in monitors)
        {
            var probe = monitor.Tags.Where(w => w.Name == "Probe").Select(s => s.Value).FirstOrDefault() == _appSettings.ProbeName;

            if (probe)
            {
                hasProbeMonitor = true;
            }

            if (monitor.Active && monitor.Maintenance is false && monitor.Type == "push" && probe)
            {
                var endpoint = new Endpoint
                {
                    Type = monitor.Tags.Where(w => w.Name == "Type").Select(s => s.Value).First(),
                    Destination = monitor.Tags.Where(w => w.Name == "Address").Select(s => s.Value).FirstOrDefault() ?? string.Empty,
                    Timeout = 1000,
                    PushUri = new Uri($"{_appSettings.Url}api/push/{monitor.PushToken}?status=up&msg=OK&ping="),
                    Keyword = monitor.Tags.Where(w => w.Name == "Keyword").Select(s => s.Value).FirstOrDefault() ?? string.Empty,
                    Method = monitor.Tags.Where(w => w.Name == "Method").Select(s => s.Value).FirstOrDefault(),
                    Brand = monitor.Tags.Where(w => w.Name == "Brand").Select(s => s.Value).FirstOrDefault() ?? string.Empty,
                    Port = int.Parse(monitor.Tags.Where(w => w.Name == "Port").Select(s => s.Value).FirstOrDefault() ?? "0"),
                    Domain = monitor.Tags.Where(w => w.Name == "Domain").Select(s => s.Value).FirstOrDefault() ?? string.Empty,
                    CertificateExpiration = int.Parse(monitor.Tags.Where(w => w.Name == "CertificateExpiration").Select(s => s.Value).FirstOrDefault() ?? "3")
                };
                endpoints.Add(endpoint);
            }
        }

        if (!hasProbeMonitor)
        {
            _logger.LogWarning("No monitors with the specified Probe tag and value {_configurations.ProbeName} were found.", _appSettings.ProbeName);
        }

        return endpoints;
    }

    private async Task LoopAsync(List<Endpoint> endpoints)
    {
        foreach (var item in endpoints)
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
                case "Database":
                    item.ConnectionString = $"{_configurations.ConnectionStrings}.{item.Brand}";
                    await _dbService.CheckDbAsync(item);
                    break;
                case "Domain":
                    if (await CheckDailyExecutionAsync()) break;
                    await _domainService.CheckDomainAsync(item);
                    break;
                default:
                    break;
            }
        }
    }

    private static async Task<bool> CheckDailyExecutionAsync()
    {
        if (lastDailyExecution == DateOnly.FromDateTime(DateTime.Now))
        {
            return await Task.FromResult(true);
        }
        else
        {
            lastDailyExecution = DateOnly.FromDateTime(DateTime.Now);
            return await Task.FromResult(false);
        }
    }
}
