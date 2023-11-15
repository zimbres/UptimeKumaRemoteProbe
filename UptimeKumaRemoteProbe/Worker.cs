namespace UptimeKumaRemoteProbe;

public class Worker(ILogger<Worker> logger, IConfiguration configuration, PingService pingService, HttpService httpService,
    TcpService tcpService, CertificateService certificateService, DbService dbService, MonitorsService monitorsService,
    DomainService domainService) : BackgroundService
{
    private readonly Configurations _configurations = configuration.GetSection(nameof(Configurations)).Get<Configurations>();
    private static DateOnly lastDailyExecution;

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogWarning("App version: {version}", Assembly.GetExecutingAssembly().GetName().Version.ToString());

        if (_configurations.UpDependency == "")
        {
            logger.LogError("Up Dependency is not set.");
            Environment.Exit(0);
        }

        Ping ping = new();
        PingReply pingReply = null;

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_configurations.UpDependency != "")
            {
                try
                {
                    pingReply = ping.Send(_configurations.UpDependency, _configurations.Timeout);
                }
                catch (Exception ex)
                {
                    logger.LogError("Network is unreachable. {ex}", ex.Message);
                }
            }

            if (pingReply?.Status == IPStatus.Success)
            {
                var monitors = await monitorsService.GetMonitorsAsync();
                if (monitors is not null)
                {
                    var endpoints = ParseEndpoints(monitors);
                    await LoopAsync(endpoints);
                }
            }
            else
            {
                logger.LogError("Up Dependency is unreachable.");
            }
            await Task.Delay(_configurations.Delay, stoppingToken);
        }
    }

    private List<Endpoint> ParseEndpoints(List<Monitors> monitors)
    {
        var endpoints = new List<Endpoint>();

        foreach (var monitor in monitors)
        {
            var probe = monitor.Tags.Where(w => w.Name == "Probe").Select(s => s.Value).FirstOrDefault() == _configurations.ProbeName;
            if (monitor.Active && monitor.Maintenance is false && monitor.Type == "push" && probe)
            {
                var endpoint = new Endpoint
                {
                    Type = monitor.Tags.Where(w => w.Name == "Type").Select(s => s.Value).First(),
                    Destination = monitor.Tags.Where(w => w.Name == "Address").Select(s => s.Value).FirstOrDefault() ?? string.Empty,
                    Timeout = 1000,
                    PushUri = new Uri($"{_configurations.Url}api/push/{monitor.PushToken}?status=up&msg=OK&ping="),
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
        return endpoints;
    }

    private async Task LoopAsync(List<Endpoint> endpoints)
    {
        foreach (var item in endpoints)
        {
            switch (item.Type)
            {
                case "Ping":
                    await pingService.CheckPingAsync(item);
                    break;
                case "Http":
                    await httpService.CheckHttpAsync(item);
                    break;
                case "Tcp":
                    await tcpService.CheckTcpAsync(item);
                    break;
                case "Certificate":
                    await certificateService.CheckCertificateAsync(item);
                    break;
                case "Database":
                    item.ConnectionString = $"{_configurations.ConnectionStrings}.{item.Brand}";
                    await dbService.CheckDbAsync(item);
                    break;
                case "Domain":
                    if (await CheckDailyExecutionAsync()) break;
                    await domainService.CheckDomainAsync(item);
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
