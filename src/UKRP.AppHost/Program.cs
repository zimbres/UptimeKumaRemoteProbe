var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.UptimeKumaRemoteProbe>("uptimekumaremoteprobe");

builder.Build().Run();
