namespace UptimeKumaRemoteProbe.Models;

public class AppSettings
{
    private readonly Configurations _configuration;

    public string Url { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string ProbeName { get; set; }
    public string UpDependency { get; set; }
    public int Timeout { get; set; }
    public int Delay { get; set; }
    public string WhoisApiUrl { get; set; }
    public string WhoisApiToken { get; set; }

    public AppSettings(IConfiguration configuration)
    {
        _configuration = configuration.GetSection(nameof(Configurations)).Get<Configurations>();

        _ = bool.TryParse(Environment.GetEnvironmentVariable("UseEnvironmentVariables"), out bool useEnv);

        Url = Environment.GetEnvironmentVariable("Url") is not null && useEnv ? Environment.GetEnvironmentVariable("Url") : _configuration.Url;
        Username = Environment.GetEnvironmentVariable("Username") is not null && useEnv ? Environment.GetEnvironmentVariable("Username") : _configuration.Username;
        Password = Environment.GetEnvironmentVariable("Password") is not null && useEnv ? Environment.GetEnvironmentVariable("Password") : _configuration.Password;
        ProbeName = Environment.GetEnvironmentVariable("ProbeName") is not null && useEnv ? Environment.GetEnvironmentVariable("ProbeName") : _configuration.ProbeName;
        UpDependency = Environment.GetEnvironmentVariable("UpDependency") is not null && useEnv ? Environment.GetEnvironmentVariable("UpDependency") : _configuration.UpDependency;
        Timeout = Environment.GetEnvironmentVariable("Timeout") is not null && useEnv ? int.Parse(Environment.GetEnvironmentVariable("Timeout")) : _configuration.Timeout;
        Delay = Environment.GetEnvironmentVariable("Delay") is not null && useEnv ? int.Parse(Environment.GetEnvironmentVariable("Delay")) : _configuration.Delay;
        WhoisApiUrl = Environment.GetEnvironmentVariable("WhoisApiUrl") is not null && useEnv ? Environment.GetEnvironmentVariable("WhoisApiUrl") : _configuration.WhoisApiUrl;
        WhoisApiToken = Environment.GetEnvironmentVariable("WhoisApiToken") is not null && useEnv ? Environment.GetEnvironmentVariable("WhoisApiToken") : _configuration.WhoisApiToken;
    }
}
