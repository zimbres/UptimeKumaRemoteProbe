using System.Net.Http.Json;

namespace UptimeKumaRemoteProbe.Services;

public class MonitorsService
{
    private readonly ILogger<MonitorsService> _logger;
    private readonly IConfiguration _configuration;
    private HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;

    public MonitorsService(ILogger<MonitorsService> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<List<Monitors>> GetMonitorsAsync()
    {
        var configurations = _configuration.GetSection(nameof(Configurations)).Get<Configurations>();

        _httpClient = _httpClientFactory.CreateClient("Default");

        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<Monitors>>(configurations.MonitorsApi);
            return result;
        }
        catch
        {
            _logger.LogError("Error trying get monitors at: {DateTimeOffset.Now}", DateTimeOffset.Now);
            return null;
        }
    }
}
