FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# COPY "Services/Identity/Identity.sln" "Identity.sln"
COPY ["Services/Identity/Identity.Api/Identity.Api.csproj", "Identity.Api/Identity.Api.csproj"]
COPY ["Services/Identity/Identity.Domain/Identity.Domain.csproj", "Identity.Domain/Identity.Domain.csproj"]
COPY ["Services/Identity/Identity.Infrastructure/Identity.Infrastructure.csproj", "Identity.Infrastructure/Identity.Infrastructure.csproj"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/EventBus.csproj"]
COPY ["BuildingBlocks/IntegrationEventLogEF/IntegrationEventLogEF.csproj", "BuildingBlocks/IntegrationEventLogEF/IntegrationEventLogEF.csproj"]
RUN dotnet restore Identity.Api

WORKDIR /app
# COPY "src" .

RUN dotnet build /src -c Release -o /build

FROM build AS publish
RUN dotnet publish "Identity.Api.csproj" -c Release -o /publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity.Api.dll"]

