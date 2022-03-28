﻿namespace UptimeKumaRemoteProbe.Services;

public class CertificateService
{
    private readonly ILogger<CertificateService> _logger;
    private readonly PushService _pushService;
    private readonly IHttpClientFactory _httpClientFactory;
    private HttpClient _httpClient;


    public CertificateService(ILogger<CertificateService> logger, PushService pushService, IHttpClientFactory httpClientFactory, HttpClient httpClient)
    {
        _logger = logger;
        _pushService = pushService;
        _httpClientFactory = httpClientFactory;
        _httpClient = httpClient;
    }

    public async Task CheckCertificateAsync(Endpoint endpoint)
    {
        var stopwatch = Stopwatch.StartNew();

        DateTime notAfter = DateTime.UtcNow;

        var httpClientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (request, cert, chain, policyErrors) =>
            {
                notAfter = cert.NotAfter;
                return true;
            }
        };

        _httpClient = _httpClientFactory.CreateClient("IgnoreSSL");
        _httpClient = new HttpClient(httpClientHandler);

        try
        {
            var result = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, endpoint.Destination));

            if (notAfter >= DateTime.UtcNow.AddDays(10))
            {
                await _pushService.PushAsync(endpoint.PushUri, stopwatch.ElapsedMilliseconds);
                _logger.LogInformation("Certificate: {endpoint.Destination} {result.StatusCode} at: {DateTimeOffset.Now}",
                    endpoint.Destination, result.StatusCode, DateTimeOffset.Now);
            }
            _logger.LogWarning("Certificate: {endpoint.Destination} expiration date: {notAfter}", endpoint.Destination, notAfter);
        }
        catch
        {
            _logger.LogError("Error trying get {endpoint.Destination} at: {DateTimeOffset.Now}", endpoint.Destination, DateTimeOffset.Now);
        }
    }
}
