namespace UptimeKumaRemoteProbe.Services;

public class DomainService
{
    private readonly ILogger<DomainService> _logger;
    private HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly PushService _pushService;
    private readonly AppSettings _appSettings;

    public DomainService(ILogger<DomainService> logger, HttpClient httpClient, IHttpClientFactory httpClientFactory,
        PushService pushService, AppSettings appSettings)
    {
        _logger = logger;
        _httpClient = httpClient;
        _httpClientFactory = httpClientFactory;
        _pushService = pushService;
        _appSettings = appSettings;
    }

    public async Task CheckDomainAsync(Endpoint endpoint)
    {
        _httpClient = _httpClientFactory.CreateClient(endpoint.IgnoreSSL ? "IgnoreSSL" : "Default");

        HttpResponseMessage result;
        int daysToExpire;
        bool closeToExpire;

        try
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"TOKEN={_appSettings.WhoisApiToken}");
            result = await _httpClient.GetAsync($"{_appSettings.WhoisApiUrl.Replace("keep.this", endpoint.Domain)}");
            var content = await result.Content.ReadAsStringAsync();
            var domain = JsonSerializer.Deserialize<Domain>(content);

            var expiration = DateTime.ParseExact(domain.Expires, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            var expires = expiration - DateTime.UtcNow;
            daysToExpire = expires.Days;
            closeToExpire = daysToExpire < 30;

            _logger.LogInformation("Domain: {endpoint.Destination} expires: {domain.Expires}", endpoint.Domain, domain.Expires);
        }
        catch
        {
            _logger.LogError("Error trying get domain expiration for {endpoint.Domain}", endpoint.Domain);
            return;
        }

        if (!closeToExpire && result is not null && result.IsSuccessStatusCode)
        {
            await _pushService.PushAsync(endpoint.PushUri, daysToExpire);
        }
    }
}
