namespace UptimeKumaRemoteProbe.Services;

public class CertificateService(ILogger<CertificateService> logger, PushService pushService, IHttpClientFactory httpClientFactory, HttpClient httpClient)
{
    public async Task CheckCertificateAsync(Endpoint endpoint)
    {
        DateTime notAfter = DateTime.UtcNow;

        var httpClientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (request, cert, chain, policyErrors) =>
            {
                notAfter = cert.NotAfter;
                return true;
            }
        };

        httpClient = httpClientFactory.CreateClient("IgnoreSSL");
        httpClient = new HttpClient(httpClientHandler);

        try
        {
            var result = await httpClient.SendAsync(new HttpRequestMessage(new HttpMethod(endpoint.Method ?? "Head"), endpoint.Destination));

            if (notAfter >= DateTime.UtcNow.AddDays(endpoint.CertificateExpiration))
            {
                await pushService.PushUpAsync(endpoint.PushUpUri, (notAfter - DateTime.UtcNow).Days);
                logger.LogInformation("Certificate: {endpoint.Destination} {result.StatusCode}",
                    endpoint.Destination, result.StatusCode);
                return;
            }
            logger.LogWarning("Certificate: {endpoint.Destination} expiration date: {notAfter}", endpoint.Destination, notAfter);
        }
        catch
        {
            logger.LogError("Error trying get {endpoint.Destination}", endpoint.Destination);
        }
    }
}
