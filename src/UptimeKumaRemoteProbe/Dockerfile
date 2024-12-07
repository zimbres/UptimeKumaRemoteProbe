#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/UptimeKumaRemoteProbe/UptimeKumaRemoteProbe.csproj", "src/UptimeKumaRemoteProbe/"]
RUN dotnet restore "./src/UptimeKumaRemoteProbe/UptimeKumaRemoteProbe.csproj"
COPY . .
WORKDIR "/src/src/UptimeKumaRemoteProbe"
RUN dotnet build "./UptimeKumaRemoteProbe.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./UptimeKumaRemoteProbe.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./UptimeKumaRemoteProbe"]