namespace UptimeKumaRemoteProbe.Services;

public class MonitorsServiceV3 : IMonitorsService
{
    private readonly ILogger<MonitorsServiceV3> _logger;
    private readonly AppSettings _appSettings;
    private readonly HttpClient _httpClient;
    private readonly UptimeKumaSession _session;
    private readonly SemaphoreSlim _loginLock = new(1, 1);

    public MonitorsServiceV3(ILogger<MonitorsServiceV3> logger, AppSettings appSettings, IHttpClientFactory httpClientFactory, UptimeKumaSession session)
    {
        _logger = logger;
        _appSettings = appSettings;
        _session = session;
        _httpClient = httpClientFactory.CreateClient("UptimeKuma");
    }

    private async Task EnsureAuthenticatedAsync()
    {
        var uri = new Uri(_appSettings.Url);

        if (_session.IsAuthenticated(uri))
        {
            return;
        }

        await _loginLock.WaitAsync();

        try
        {
            if (_session.IsAuthenticated(uri))
            {
                return;
            }
            await LoginAsync();
        }
        finally
        {
            _loginLock.Release();
        }
    }

    private async Task LoginAsync()
    {
        var loginUrl = $"{_appSettings.Url.TrimEnd('/')}/api/auth/sign-in/username";

        var loginData = new
        {
            username = _appSettings.Username,
            password = _appSettings.Password
        };

        using var response = await _httpClient.PostAsJsonAsync(loginUrl, loginData);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            _logger.LogError("Uptime Kuma login failed. Status: {StatusCode}, Body: {Body}", response.StatusCode, body);
            return;
        }
    }

    public async Task<List<Monitors>> GetMonitorsAsync()
    {
        SocketIOClient.SocketIO socket = null;
        try
        {
            await EnsureAuthenticatedAsync();

            var cookieHeader = _session.Cookies.GetCookieHeader(new Uri(_appSettings.Url));

            socket = new SocketIOClient.SocketIO(_appSettings.Url, new SocketIOClient.SocketIOOptions
            {
                ReconnectionAttempts = 3,
                Path = "/socket.io/",
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                Transport = SocketIOClient.Transport.TransportProtocol.WebSocket,
                ExtraHeaders = new Dictionary<string, string>
                {
                    ["Cookie"] = cookieHeader
                }
            });

            JsonElement monitorsRaw = new();

            socket.On("monitorList", response =>
            {
                monitorsRaw = response.GetValue<JsonElement>();
            });

            await socket.ConnectAsync();

            int round = 0;
            while (monitorsRaw.ValueKind == JsonValueKind.Undefined)
            {
                round++;
                await Task.Delay(1000);
                if (round >= 10) break;
            }

            await socket.DisconnectAsync();
            var monitors = JsonSerializer.Deserialize<Dictionary<string, Monitors>>(monitorsRaw);
            return monitors.Values.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error trying to get monitors");
            return null;
        }
        finally
        {
            if (socket != null)
            {
                socket.Off("monitorList");
                socket.Dispose();
            }
        }
    }
    public async Task<List<Monitors>> GetMonitorsApiAsync()
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _appSettings.ApiToken);
            var response = await _httpClient.GetAsync($"{_appSettings.ApiUrl}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error trying to get monitors from API, *** {} ***", response.ReasonPhrase);
            }
            if (response.IsSuccessStatusCode)
            {
                var monitorsApi = JsonSerializer.Deserialize<MonitorsApi>(await response.Content.ReadAsStringAsync());
                return monitorsApi.Data;
            }
            return null;
        }
        catch
        {
            _logger.LogError("Error trying to get monitors from API");
            return null;
        }
    }
}
