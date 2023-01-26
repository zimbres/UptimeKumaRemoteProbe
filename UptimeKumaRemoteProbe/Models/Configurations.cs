namespace UptimeKumaRemoteProbe.Models;

public class Configurations
{
    public List<Endpoint> Endpoints { get; set; }
    public string MonitorsApi { get; set; }
    public string ProbeName { get; set; }
    public string BasePushUri { get; set; }
    public string UpDependency { get; set; }
    public int Timeout { get; set; }
    public int Delay { get; set; }
}

public class Endpoint
{
    public string Type { get; set; }
    public Uri PushUri { get; set; }
    public string Destination { get; set; }
    public int Port { get; set; }
    public string Method { get; set; }
    public string Keyword { get; set; }
    public bool IgnoreSSL { get; set; }
    public int Timeout { get; set; }
    public int CertificateExpiration { get; set; }
    public string ConnectionString { get; set; }
    public string Brand { get; set; }
}
