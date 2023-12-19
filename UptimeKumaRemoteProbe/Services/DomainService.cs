namespace UptimeKumaRemoteProbe.Services;

public class DomainService(ILogger<DomainService> logger, HttpClient httpClient, IHttpClientFactory httpClientFactory,
    PushService pushService, IConfiguration configuration)
{
    private readonly Configurations _configuration = configuration.GetSection(nameof(Configurations)).Get<Configurations>();

    public async Task CheckDomainAsync(Endpoint endpoint)
    {
        httpClient = httpClientFactory.CreateClient(endpoint.IgnoreSSL ? "IgnoreSSL" : "Default");

        HttpResponseMessage result;
        int daysToExpire;
        bool closeToExpire;

        try
        {
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"TOKEN={_configuration.WhoisApiToken}");
            result = await httpClient.GetAsync($"{_configuration.WhoisApiUrl.Replace("keep.this", endpoint.Domain)}");
            var content = await result.Content.ReadAsStringAsync();
            var domain = JsonSerializer.Deserialize<Domain>(content);

            var expiration = DateTime.ParseExact(domain.Expires, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            var expires = expiration - DateTime.UtcNow;
            daysToExpire = expires.Days;
            closeToExpire = daysToExpire < 30;

            logger.LogInformation("Domain: {endpoint.Destination} expires: {domain.Expires}", endpoint.Domain, domain.Expires);
        }
        catch
        {
            logger.LogError("Error trying get domain expiration for {endpoint.Domain}", endpoint.Domain);
            return;
        }

        if (!closeToExpire && result is not null && result.IsSuccessStatusCode)
        {
            await pushService.PushUpAsync(endpoint.PushUpUri, daysToExpire);
        }
    }
}
