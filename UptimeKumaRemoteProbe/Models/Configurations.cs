namespace UptimeKumaRemoteProbe.Models;

public class Configurations
{
    public List<Services> Services { get; set; }
    public string UpDependency { get; set; }
    public int Timeout { get; set; }
    public int Delay { get; set; }
}

public class Services
{
    public Uri PushUri { get; set; }
    public string Destination { get; set; }
}
