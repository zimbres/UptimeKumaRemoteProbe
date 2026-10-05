namespace UptimeKumaRemoteProbe.Services;

public interface IMonitorsService
{
    Task<List<Monitors>> GetMonitorsApiAsync();
    Task<List<Monitors>> GetMonitorsAsync();
}