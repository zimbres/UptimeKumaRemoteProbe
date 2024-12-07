namespace UptimeKumaRemoteProbe.Services;

public class DbService
{
    private readonly ILogger<DbService> _logger;
    private readonly PushService _pushService;

    public DbService(ILogger<DbService> logger, PushService pushService)
    {
        _logger = logger;
        _pushService = pushService;
    }

    public async Task CheckDbAsync(Endpoint endpoint)
    {
        var stopwatch = Stopwatch.StartNew();

        var dbContext = new ApplicationDbContext(endpoint);

        string status = null;

        try
        {
            switch (endpoint.Brand)
            {
                case "MSSQL":
                    status = dbContext.DbVersion?.FromSqlRaw("Select @@VERSION AS Version").First().Version;
                    break;
                case "MYSQL":
                    status = dbContext.DbVersion?.FromSqlRaw("Select VERSION() AS Version").First().Version;
                    break;
                case "PGSQL":
                    status = dbContext.DbVersion?.FromSqlRaw("Select Version()").ToString();
                    break;
                default:
                    _logger.LogError("Brand must be MSSQL, MYSQL or PGSQL");
                    break;
            }
        }
        catch
        {
            _logger.LogError("Error trying get {endpoint.Brand}", endpoint.Brand);
        }

        if (status is not null)
        {
            await _pushService.PushAsync(endpoint.PushUri, stopwatch.ElapsedMilliseconds);
        }
    }
}
