namespace UptimeKumaRemoteProbe.Models;

public class Configurations
{
    public ConnectionStrings ConnectionStrings { get; set; }
    public string Url { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string ProbeName { get; set; }
    public string UpDependency { get; set; }
    public int Timeout { get; set; }
    public int Delay { get; set; }
    public string WhoisApiUrl { get; set; }
    public string WhoisApiToken { get; set; }
}

public class Endpoint
{
    public string Type { get; set; }
    public Uri PushUri { get; set; }
    public string Destination { get; set; }
    public int Port { get; set; }
    public string Method { get; set; }
    public int SuccessStatusCode { get; set; }
    public string Keyword { get; set; }
    public bool IgnoreSSL { get; set; }
    public int Timeout { get; set; }
    public int CertificateExpiration { get; set; }
    public string ConnectionString { get; set; }
    public string Brand { get; set; }
    public string Domain { get; set; }
}

public class ConnectionStrings
{
    public string PGSQL { get; set; }
    public string MYSQL { get; set; }
    public string MSSQL { get; set; }
}