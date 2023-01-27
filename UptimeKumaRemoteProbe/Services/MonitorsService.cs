namespace UptimeKumaRemoteProbe.Services;

public class MonitorsService
{
    private readonly ILogger<MonitorsService> _logger;
    private readonly Configurations _configuration;
    private readonly HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;
    private string token;

    public MonitorsService(ILogger<MonitorsService> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration.GetSection(nameof(Configurations)).Get<Configurations>(); ;
        _httpClient = _httpClientFactory.CreateClient("Default");
    }

    public async Task<List<Monitors>> GetMonitorsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_configuration.MonitorsApi}/monitors");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _httpClient.DefaultRequestHeaders.Clear();
                await AuthenticateAsync();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                response = await _httpClient.GetAsync($"{_configuration.MonitorsApi}/monitors");
            }
            var stringJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<Monitors>>(stringJson);
            return result;
        }
        catch
        {
            _logger.LogError("Error trying get monitors at: {DateTimeOffset.Now}", DateTimeOffset.Now);
            return null;
        }
    }

    private async Task AuthenticateAsync()
    {
        var dict = new Dictionary<string, string>
        {
            { "Content-Type" , "application/x-www-form-urlencoded" },
            { "username" , $"{_configuration.Username}" },
            { "password" , $"{_configuration.Password}" }
        };

        var result = await _httpClient.PostAsync($"{_configuration.MonitorsApi}/token", new FormUrlEncodedContent(dict));
        var tokenResult = await result.Content.ReadAsStringAsync();
        var tokenObject = JsonDocument.Parse(tokenResult);
        token = tokenObject.RootElement.GetProperty("access_token").ToString();
    }
}
