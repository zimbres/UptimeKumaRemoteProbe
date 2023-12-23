var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHttpClient("Default");
builder.Services.AddHttpClient("IgnoreSSL")
.ConfigurePrimaryHttpMessageHandler(() =>
{
    return new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (m, c, ch, e) => true
    };
});
builder.Services.AddSingleton<PingService>();
builder.Services.AddSingleton<HttpService>();
builder.Services.AddSingleton<TcpService>();
builder.Services.AddSingleton<PushService>();
builder.Services.AddSingleton<DbService>();
builder.Services.AddSingleton<CertificateService>();
builder.Services.AddSingleton<MonitorsService>();
builder.Services.AddSingleton<DomainService>();
builder.Services.AddHostedService<Worker>().Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});
builder.Services.RemoveAll<IHttpMessageHandlerBuilderFilter>(); //Disable HttpClient Logging
builder.Services.AddHealthChecks().AddCheck("HealthCheck", () => HealthCheckResult.Healthy());
builder.Services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();
builder.Services.Configure<HealthCheckPublisherOptions>(options =>
{
    options.Delay = TimeSpan.FromSeconds(5);
    options.Period = TimeSpan.FromSeconds(20);
});
builder.Services.AddSingleton<VersionService>();
builder.Services.AddSingleton<AppSettings>();
    
var host = builder.Build();
host.Run();
