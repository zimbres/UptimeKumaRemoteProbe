using System.Globalization;

namespace UptimeKumaRemoteProbe.Services;

public class DomainService
{
    private readonly ILogger<HttpService> _logger;
    private HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly PushService _pushService;
    private readonly Configurations _configuration;

    public DomainService(ILogger<HttpService> logger, HttpClient httpClient, IHttpClientFactory httpClientFactory,
        PushService pushService, IConfiguration configuration)
    {
        _logger = logger;
        _httpClient = httpClient;
        _httpClientFactory = httpClientFactory;
        _pushService = pushService;
        _configuration = configuration.GetSection(nameof(Configurations)).Get<Configurations>();
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
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"TOKEN={_configuration.WhoisApiToken}");
            result = await _httpClient.GetAsync($"{_configuration.WhoisApiUrl.Replace("keep.this", endpoint.Domain)}");
            var content = await result.Content.ReadAsStringAsync();
            var domain = JsonSerializer.Deserialize<Domain>(content);

            var now = DateTime.ParseExact(DateTime.UtcNow.ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            var expiration = DateTime.ParseExact(domain.Expires, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            var expires = expiration - now;
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
