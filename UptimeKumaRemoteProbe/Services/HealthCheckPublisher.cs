namespace UptimeKumaRemoteProbe.Services;

public class HealthCheckPublisher(ILogger<HealthCheckPublisher> logger) : IHealthCheckPublisher
{
    private readonly string _fileName = "./health";
    private HealthStatus _prevStatus = HealthStatus.Unhealthy;

    public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
    {
        var fileExists = _prevStatus == HealthStatus.Healthy;
        if (report.Status == HealthStatus.Healthy)
        {
            using var _ = File.Create(_fileName);
        }
        else if (fileExists)
        {
            File.Delete(_fileName);
            logger.LogWarning("{status}", report.Status.ToString());
        }

        _prevStatus = report.Status;
        return Task.CompletedTask;
    }
}
