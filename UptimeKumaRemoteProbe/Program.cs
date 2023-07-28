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
        services.AddSingleton<TcpService>();
        services.AddSingleton<PushService>();
        services.AddSingleton<DbService>();
        services.AddSingleton<CertificateService>();
        services.AddSingleton<MonitorsService>();
        services.AddSingleton<DomainService>();
        services.AddHostedService<Worker>().Configure<HostOptions>(options =>
        {
            options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
        });
        services.RemoveAll<IHttpMessageHandlerBuilderFilter>(); //Disable HttpClient Logging
        services.AddHealthChecks();
        services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();
        services.Configure<HealthCheckPublisherOptions>(options =>
        {
            options.Delay = TimeSpan.FromSeconds(5);
            options.Period = TimeSpan.FromSeconds(20);
        });
    })
    .Build();

await host.RunAsync();
