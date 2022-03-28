IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => {
        services.AddHttpClient("Default");
        services.AddHttpClient("IgnoreSSL")
        .ConfigurePrimaryHttpMessageHandler(() => {
            return new HttpClientHandler {
                ServerCertificateCustomValidationCallback = (m, c, ch, e) => true
            };
        });
        services.AddSingleton<PingService>();
        services.AddSingleton<HttpService>();
        services.AddSingleton<TcpService>();
        services.AddSingleton<PushService>();
        services.AddSingleton<CertificateService>();
        services.AddHostedService<Worker>();
        services.RemoveAll<IHttpMessageHandlerBuilderFilter>(); //Disable HttpClient Logging
    })
    .Build();

await host.RunAsync();
