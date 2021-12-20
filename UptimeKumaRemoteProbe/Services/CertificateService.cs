using System.Diagnostics;
using UptimeKumaRemoteProbe.Models;

namespace UptimeKumaRemoteProbe.Services;

public class CertificateService
{
    private readonly ILogger<CertificateService> _logger;
    private readonly PushService _pushService;

    public CertificateService(ILogger<CertificateService> logger, PushService pushService)
    {
        _logger = logger;
        _pushService = pushService;
    }

    public async Task CheckCertificateAsync(Endpoint endpoint)
    {
        var stopwatch = Stopwatch.StartNew();

        DateTime notAfter = DateTime.UtcNow;

        var httpClientHandler = new HttpClientHandler {
            ServerCertificateCustomValidationCallback = (request, cert, chain, policyErrors) => {
                notAfter = cert.NotAfter;
                return true;
            }
        };

        using HttpClient httpClient = new(httpClientHandler);

        try
        {
            var result = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, endpoint.Destination));
            _logger.LogWarning($"Certificate: {endpoint.Destination} {result.StatusCode} at: {DateTimeOffset.Now}");

            if (notAfter >= DateTime.UtcNow.AddDays(10))
            {
                await _pushService.PushAsync(endpoint.PushUri, stopwatch.ElapsedMilliseconds);
            }
        }
        catch
        {
            _logger.LogError($"Error trying get {endpoint.Destination} at: {DateTimeOffset.Now}");
        }
    }
}
