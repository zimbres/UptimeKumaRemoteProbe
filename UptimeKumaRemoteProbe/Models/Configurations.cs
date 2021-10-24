namespace UptimeKumaRemoteProbe.Models;

public class Configurations
{
    public Uri Uri { get; set; }
    public string Destination { get; set; }
    public string UpDependency { get; set; }
    public int Timeout { get; set; }
    public int Delay { get; set; }
}
