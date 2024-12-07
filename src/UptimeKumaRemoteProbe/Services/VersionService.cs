namespace UptimeKumaRemoteProbe.Services;

public class VersionService
{
    private readonly ILogger<VersionService> _logger;
    private readonly Configurations _configurations;

    public VersionService(ILogger<VersionService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configurations = configuration.GetSection(nameof(Configurations)).Get<Configurations>();
    }

    public async Task<bool> CheckVersionAsync()
    {
        var url = _configurations.Url;
        if (url == null)
        {
            _logger.LogError("*** The appsettings.json being used is not compatible with the current version of the application. Please check the repository https://github.com/zimbres/UptimeKumaRemoteProbe for the latest version. ***");
            return await Task.FromResult(true);
        }
        return await Task.FromResult(false);
    }
}
