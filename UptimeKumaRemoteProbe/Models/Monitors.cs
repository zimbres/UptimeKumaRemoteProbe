using System.Text.Json.Serialization;

namespace UptimeKumaRemoteProbe.Models;

public class Monitors
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("method")]
    public string Method { get; set; }

    [JsonPropertyName("hostname")]
    public object Hostname { get; set; }

    [JsonPropertyName("port")]
    public object Port { get; set; }

    [JsonPropertyName("maxretries")]
    public long Maxretries { get; set; }

    [JsonPropertyName("weight")]
    public long Weight { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("interval")]
    public long Interval { get; set; }

    [JsonPropertyName("retryInterval")]
    public long RetryInterval { get; set; }

    [JsonPropertyName("resendInterval")]
    public long ResendInterval { get; set; }

    [JsonPropertyName("keyword")]
    public object Keyword { get; set; }

    [JsonPropertyName("expiryNotification")]
    public bool ExpiryNotification { get; set; }

    [JsonPropertyName("ignoreTls")]
    public bool IgnoreTls { get; set; }

    [JsonPropertyName("upsideDown")]
    public bool UpsideDown { get; set; }

    [JsonPropertyName("maxredirects")]
    public long Maxredirects { get; set; }

    [JsonPropertyName("accepted_statuscodes")]
    public string[] AcceptedStatuscodes { get; set; }

    [JsonPropertyName("dns_resolve_type")]
    public string DnsResolveType { get; set; }

    [JsonPropertyName("dns_resolve_server")]
    public string DnsResolveServer { get; set; }

    [JsonPropertyName("dns_last_result")]
    public object DnsLastResult { get; set; }

    [JsonPropertyName("docker_container")]
    public string DockerContainer { get; set; }

    [JsonPropertyName("docker_host")]
    public object DockerHost { get; set; }

    [JsonPropertyName("proxyId")]
    public object ProxyId { get; set; }

    [JsonPropertyName("notificationIDList")]
    public object[] NotificationIdList { get; set; }

    [JsonPropertyName("tags")]
    public Tag[] Tags { get; set; }

    [JsonPropertyName("maintenance")]
    public bool Maintenance { get; set; }

    [JsonPropertyName("mqttTopic")]
    public string MqttTopic { get; set; }

    [JsonPropertyName("mqttSuccessMessage")]
    public string MqttSuccessMessage { get; set; }

    [JsonPropertyName("databaseQuery")]
    public object DatabaseQuery { get; set; }

    [JsonPropertyName("authMethod")]
    public object AuthMethod { get; set; }

    [JsonPropertyName("grpcUrl")]
    public object GrpcUrl { get; set; }

    [JsonPropertyName("grpcProtobuf")]
    public object GrpcProtobuf { get; set; }

    [JsonPropertyName("grpcMethod")]
    public object GrpcMethod { get; set; }

    [JsonPropertyName("grpcServiceName")]
    public object GrpcServiceName { get; set; }

    [JsonPropertyName("grpcEnableTls")]
    public bool GrpcEnableTls { get; set; }

    [JsonPropertyName("radiusCalledStationId")]
    public object RadiusCalledStationId { get; set; }

    [JsonPropertyName("radiusCallingStationId")]
    public object RadiusCallingStationId { get; set; }

    [JsonPropertyName("headers")]
    public object Headers { get; set; }

    [JsonPropertyName("body")]
    public object Body { get; set; }

    [JsonPropertyName("grpcBody")]
    public object GrpcBody { get; set; }

    [JsonPropertyName("grpcMetadata")]
    public object GrpcMetadata { get; set; }

    [JsonPropertyName("basic_auth_user")]
    public object BasicAuthUser { get; set; }

    [JsonPropertyName("basic_auth_pass")]
    public object BasicAuthPass { get; set; }

    [JsonPropertyName("pushToken")]
    public string PushToken { get; set; }

    [JsonPropertyName("databaseConnectionString")]
    public object DatabaseConnectionString { get; set; }

    [JsonPropertyName("radiusUsername")]
    public object RadiusUsername { get; set; }

    [JsonPropertyName("radiusPassword")]
    public object RadiusPassword { get; set; }

    [JsonPropertyName("radiusSecret")]
    public object RadiusSecret { get; set; }

    [JsonPropertyName("mqttUsername")]
    public string MqttUsername { get; set; }

    [JsonPropertyName("mqttPassword")]
    public string MqttPassword { get; set; }

    [JsonPropertyName("authWorkstation")]
    public object AuthWorkstation { get; set; }

    [JsonPropertyName("authDomain")]
    public object AuthDomain { get; set; }

    [JsonPropertyName("includeSensitiveData")]
    public bool IncludeSensitiveData { get; set; }
}

public class Tag
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("monitor_id")]
    public long MonitorId { get; set; }

    [JsonPropertyName("tag_id")]
    public long TagId { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("color")]
    public string Color { get; set; }
}