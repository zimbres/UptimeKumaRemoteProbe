namespace UptimeKumaRemoteProbe.Services;

public class HealthCheckPublisher : IHealthCheckPublisher
{
    private readonly ILogger<HealthCheckPublisher> _logger;
    private readonly string _fileName;
    private HealthStatus _prevStatus = HealthStatus.Unhealthy;

    public HealthCheckPublisher(ILogger<HealthCheckPublisher> logger)
    {
        _fileName = "./health";
        _logger = logger;
    }

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
            _logger.LogWarning("{status}", report.Status.ToString());
        }

        _prevStatus = report.Status;
        return Task.CompletedTask;
    }
}
