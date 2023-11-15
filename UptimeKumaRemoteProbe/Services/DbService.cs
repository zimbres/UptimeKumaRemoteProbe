namespace UptimeKumaRemoteProbe.Services;

public class DbService(ILogger<TcpService> logger, PushService pushService)
{
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
                    logger.LogError("Brand must be MSSQL, MYSQL or PGSQL");
                    break;
            }
        }
        catch
        {
            logger.LogError("Error trying get {endpoint.Brand}", endpoint.Brand);
        }

        if (status is not null)
        {
            await pushService.PushAsync(endpoint.PushUri, stopwatch.ElapsedMilliseconds);
        }
    }
}
