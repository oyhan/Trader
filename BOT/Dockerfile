#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN apt-get update && \
    apt-get install -yq tesseract-ocr 
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BOT/Bot.csproj", "BOT/"]
RUN dotnet restore "BOT/Bot.csproj"
COPY . .
WORKDIR "/src/BOT"
RUN dotnet build "Bot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bot.csproj" -c Release -o /app/publish /p:UseAppHost=false

RUN apt-get update && apt-get install -y python-dev tesseract-ocr g++ python-pip
    
RUN pip install https://github.com/goulu/pdfminer/zipball/e6ad15af79a26c31f4e384d8427b375c93b03533#egg=pdfminer.six

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bot.dll"]