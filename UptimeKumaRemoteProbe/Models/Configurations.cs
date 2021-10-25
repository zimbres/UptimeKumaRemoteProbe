namespace UptimeKumaRemoteProbe.Models;

public class Configurations
{
    public List<Endpoint> Endpoints { get; set; }
    public string UpDependency { get; set; }
    public int Timeout { get; set; }
    public int Delay { get; set; }
}

public class Endpoint
{
    public string Type { get; set; }
    public Uri PushUri { get; set; }
    public string Destination { get; set; }
    public string Keyword { get; set; }
    public bool IgnoreSSL { get; set; }
    public int Timeout { get; set; }
}
