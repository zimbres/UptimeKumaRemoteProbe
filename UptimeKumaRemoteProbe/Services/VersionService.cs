namespace UptimeKumaRemoteProbe.Services;

public class VersionService(ILogger<VersionService> logger, IConfiguration configuration)
{
    private readonly Configurations _configuration = configuration.GetSection(nameof(Configurations)).Get<Configurations>();
    public async Task<bool> CheckVersionAsync()
    {
        var url = _configuration.Url;
        if (url == null)
        {
            logger.LogError("*** The appsettings.json being used is not compatible with the current version of the application. Please check the repository https://github.com/zimbres/UptimeKumaRemoteProbe for the latest version. ***");
            return  await Task.FromResult(true);
        }
        return await Task.FromResult(false);
    }
}
