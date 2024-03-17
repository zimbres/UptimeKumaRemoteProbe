[![Publish](https://github.com/zimbres/UptimeKumaRemoteProbe/actions/workflows/dotnet.yml/badge.svg?event=release)](https://github.com/zimbres/UptimeKumaRemoteProbe/actions/workflows/dotnet.yml) [![.CodeQL](https://github.com/zimbres/UptimeKumaRemoteProbe/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/zimbres/UptimeKumaRemoteProbe/actions/workflows/codeql-analysis.yml)


# Uptime Kuma Remote Probe

>Uptime Kuma repository https://github.com/louislam/uptime-kuma

---

### Pre built container

>https://hub.docker.com/r/zimbres/uptime-kuma-remote-probe

---

Services configuration is done by editing the file appsettings.json and restarting application.

`"UpDependency": "192.168.1.1"` should be a trustable IP in your network, your ISP gateway for example. In case of this IP is not available, no other checks will be executed.

`"Delay": 60000` is the delay time between checks. It is expressed in milliseconds, in this example 1 minute between each round.

---

From version > 3.0 the services configuration is not done by adding it to appsettings.json, services to be executed on the probe will be auto discovered by tags set in UK.

Username and Password for UK need to be set on appsettings.json "Configurations.Username/Password" also UK Url. Account with 2FA is not supported.

Ex:

- Tag Name: "Probe" / Tag Value: "House" -> This value also must be set in appsettings.json on field "Configurations.ProbeName"
- Tag Name: "Type" / Tag Value: "Ping"
- Tag Name: "Address" / Tag Value: "1.1.1.1"
- Tag Name: "Domain" / Tag Value: "domain.com"
- Tag Name: "Method" / Tag Value: "GET"
- Tag Name: "CertificateExpiration" / Tag Value: "7"

![image](https://github.com/zimbres/UptimeKumaRemoteProbe/assets/29772043/a4a9fd07-4f33-4f4f-9c27-24b59be42b28)

---
Available monitors type are:

- Ping
- Http, with or whithout Keyword. Tag Name "Keyword" must be applied to also check this
- Tcp
- Certificate
- Database
- Domain

`Tags and Values are case sensitive.`

Service for Domain check is [Whois Json](https://whoisjson.com/). You need an account and replace the "WhoisApiToken" field with your token on appsettings.json.

This service has a api call limit of 500 per month, this would be enough since this check will run once a day only, or at the probe restart.

By default if the domain expiration date is < 30 days, probe will not push to UK and generate an alert.

---

Pré compiled package is available for Windows and Linux. It requires .Net Runtime 8.x.

[Download .NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
