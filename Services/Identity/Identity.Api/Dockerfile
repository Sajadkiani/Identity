FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY "MicroService.sln" "MicroService.sln"
COPY ["Identity/Identity.Api/Identity.Api.csproj", "Identity/Identity.Api/Identity.Api.csproj"]
COPY ["Identity/Identity.Domain/Identity.Domain.csproj", "Identity/Identity.Domain/Identity.Domain.csproj"]
COPY ["Identity/Identity.Infrastructure/Identity.Infrastructure.csproj", "Identity/Identity.Infrastructure/Identity.Infrastructure.csproj"]
COPY ["EventBus/EventBus.csproj", "EventBus/EventBus.csproj"]
COPY ["IntegrationEventLogEF/IntegrationEventLogEF.csproj", "IntegrationEventLogEF/IntegrationEventLogEF.csproj"]
RUN dotnet restore
COPY . .
WORKDIR Identity/Identity.Api

RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Identity.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity.Api.dll"]

