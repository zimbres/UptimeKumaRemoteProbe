# Uptime Kuma Remote Probe / Push Agent

>Uptime Kuma repository https://github.com/louislam/uptime-kuma

---

## Pre built container

>https://hub.docker.com/r/zimbres/uptime-kuma-remote-probe


---

Services configuration is done by editing the file appsettings.json and restarting application.

`"UpDependency": "192.168.1.1"` should be a trustable IP in your network, your ISP gateway for example. In case of this IP is not available, no other checks will be executed.

`"Delay": 60000` is the delay time between checks. It is expressed in milliseconds, in this example 1 minute between each round.

---

Username and Password for UK need to be set on appsettings.json "Configurations.Username/Password" also UK Url. Account with 2FA is not supported.

Ex:

- Tag Name: "Probe" / Tag Value: "House" -> This value also must be set in appsettings.json on field "Configurations.ProbeName"
- Tag Name: "Type" / Tag Value: "Ping"
- Tag Name: "Address" / Tag Value: "1.1.1.1"
- Tag Name: "Method" / Tag Value: "GET"
- Tag Name: "CertificateExpiration" / Tag Value: "7"
- Tag Name: "IgnoreSSL" / Tag Value: "False"


---
Available monitors type are:

- Ping
- Http, with or whithout Keyword. Tag Name "Keyword" must be applied to also check this
- Tcp
- Certificate
- Database

`Tags and Values are case sensitive.`

---

Pré compiled package is available for Windows and Linux. It requires .Net Runtime 10.x.

[Download .NET 10.0](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
