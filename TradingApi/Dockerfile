#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apt-get update && \
    apt-get install -yq tzdata && \
    ln -fs /usr/share/zoneinfo/Asia/Tehran /etc/localtime && \
    dpkg-reconfigure -f noninteractive tzdata

 # Install Chrome
RUN apt update && apt install -y  \
 apt-transport-https \
 ca-certificates \
 curl \
 gnupg \
 hicolor-icon-theme \
 libcanberra-gtk* \
 libgl1-mesa-dri \
 libgl1-mesa-glx \
 libpango1.0-0 \
 libpulse0 \
 libv4l-0 \
 fonts-symbola \
 --no-install-recommends \
&& curl -sSL https://dl.google.com/linux/linux_signing_key.pub | apt-key add - \
&& echo "deb [arch=amd64] https://dl.google.com/linux/chrome/deb/ stable main" > /etc/apt/sources.list.d/google.list \
&& apt-get update && apt-get install -y \
google-chrome-stable \
--no-install-recommends \
&& apt-get purge --auto-remove -y curl \
&& rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TradingApi/TradingApi.csproj", "TradingApi/"]
RUN dotnet restore "TradingApi/TradingApi.csproj"
COPY . .
WORKDIR "/src/TradingApi"
RUN dotnet build "TradingApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TradingApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TradingApi.dll"]