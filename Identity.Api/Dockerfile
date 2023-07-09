FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY "Identity.sln" "Identity.sln"
COPY ["Identity.Api/Identity.Api.csproj", "Identity.Api/Identity.Api.csproj"]
COPY ["Identity.Domain/Identity.Domain.csproj", "Identity.Domain/Identity.Domain.csproj"]
COPY ["Identity.Infrastructure/Identity.Infrastructure.csproj", "Identity.Infrastructure/Identity.Infrastructure.csproj"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/EventBus.csproj"]
COPY ["BuildingBlocks/IntegrationEventLogEF/IntegrationEventLogEF.csproj", "BuildingBlocks/IntegrationEventLogEF/IntegrationEventLogEF.csproj"]
RUN dotnet restore Identity.Api

WORKDIR /app
COPY . .

RUN dotnet build "Identity.Api/Identity.Api.csproj" -c Release -o /build

FROM build AS publish
RUN dotnet publish "Identity.Api/Identity.Api.csproj" -c Release -o /publish

FROM base AS final
WORKDIR /app/publish
COPY --from=publish /publish .
ENTRYPOINT ["dotnet", "Identity.Api.dll"]

