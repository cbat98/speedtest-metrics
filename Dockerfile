FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /App

COPY . ./

RUN dotnet restore

RUN dotnet publish -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /App

RUN apt-get update && \
    apt-get install -y curl tar && \
    curl -O https://install.speedtest.net/app/cli/ookla-speedtest-1.2.0-linux-armhf.tgz && \
    tar -xzf ookla-speedtest-1.2.0-linux-armhf.tgz speedtest && \
    rm ookla-speedtest-1.2.0-linux-armhf.tgz

COPY --from=build /App/out .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080

ENTRYPOINT ["dotnet", "speedtest-metrics.dll"]
