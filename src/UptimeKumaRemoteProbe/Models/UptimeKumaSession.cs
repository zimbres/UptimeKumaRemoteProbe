namespace UptimeKumaRemoteProbe.Models;

public sealed class UptimeKumaSession
{
    public CookieContainer Cookies { get; } = new();

    public bool IsAuthenticated(Uri baseUri)
    {
        return Cookies.GetCookies(baseUri)["better-auth.session_token"] != null;
    }
}
