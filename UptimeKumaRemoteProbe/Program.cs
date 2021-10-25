using UptimeKumaRemoteProbe;
using UptimeKumaRemoteProbe.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHttpClient("Default");
        services.AddHttpClient("IgnoreSSL")
        .ConfigurePrimaryHttpMessageHandler(() =>
        {
            return new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (m, c, ch, e) => true
            };
        });
        services.AddSingleton<PingService>();
        services.AddSingleton<HttpService>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
