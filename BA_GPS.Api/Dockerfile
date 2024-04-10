#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BA_GPS.Api/BA_GPS.Api.csproj", "BA_GPS.Api/"]
COPY ["BA_GPS.Domain/BA_GPS.Domain.csproj", "BA_GPS.Domain/"]
COPY ["BA_GPS.Common/BA_GPS.Common.csproj", "BA_GPS.Common/"]
COPY ["BA_GPS.Infrastructure/BA_GPS.Infrastructure.csproj", "BA_GPS.Infrastructure/"]
COPY ["BA_GPS.Application/BA_GPS.Application.csproj", "BA_GPS.Application/"]
RUN dotnet restore "BA_GPS.Api/BA_GPS.Api.csproj"
COPY . .
WORKDIR "/src/BA_GPS.Api"
RUN dotnet build "BA_GPS.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BA_GPS.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BA_GPS.Api.dll"]
