namespace UptimeKumaRemoteProbe.Models;

public class Domain
{
    [JsonPropertyName("server")]
    public string Server { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("idnName")]
    public string IdnName { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("nameserver")]
    public string[] Nameserver { get; set; }

    [JsonPropertyName("ips")]
    public string Ips { get; set; }

    [JsonPropertyName("created")]
    public string Created { get; set; }

    [JsonPropertyName("changed")]
    public string Changed { get; set; }

    [JsonPropertyName("expires")]
    public string Expires { get; set; }

    [JsonPropertyName("registered")]
    public bool Registered { get; set; }

    [JsonPropertyName("dnssec")]
    public bool Dnssec { get; set; }

    [JsonPropertyName("whoisserver")]
    public string Whoisserver { get; set; }
}
