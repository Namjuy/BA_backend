#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src


COPY ["BA_GPS.Host/BA_GPS.Host.csproj", "BA_GPS.Host/"]
COPY ["BA_GPS.Application/BA_GPS.Application.csproj", "BA_GPS.Application/"]
COPY ["BA_GPS.Domain/BA_GPS.Domain.csproj", "BA_GPS.Domain/"]
COPY ["BA_GPS.Common/BA_GPS.Common.csproj", "BA_GPS.Common/"]
COPY ["BA_GPS.Infrastructure/BA_GPS.Infrastructure.csproj", "BA_GPS.Infrastructure/"]
RUN dotnet restore "BA_GPS.Host/BA_GPS.Host.csproj"
COPY . .
WORKDIR "/src/BA_GPS.Host"
RUN dotnet build "BA_GPS.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BA_GPS.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BA_GPS.Host.dll"]
