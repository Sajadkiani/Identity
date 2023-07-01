##FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
##WORKDIR /app
##EXPOSE 80
##EXPOSE 443
#
#FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
#WORKDIR /MicroService
#
#COPY "MicroService.sln" "MicroService.sln"
##COPY ["Identity.Api.csproj", "Identity.Api.csproj/"]
##COPY ["Identity/Identity.Domain/Identity.Domain.csproj", "Identity/Identity.Domain/"]
##COPY ["Identity/Identity.Infrastructure/Identity.Infrastructure.csproj", "Identity/Identity.Infrastructure/"]
##COPY ["BuildingBlockes/EventBus/EventBus.csproj", "EventBus/"]
##COPY ["BuildingBlockes/IntegrationEventLogEF/IntegrationEventLogEF.csproj", "IntegrationEventLogEF/"]
#RUN dotnet restore
#COPY . .
#WORKDIR Identity/Identity.Api
#
#RUN dotnet build -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "Identity.Api.csproj" -c Release -o /app/publish
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "Identity.Api.dll"]


FROM mcr.microsoft.com/dotnet/sdk:7.0 as build-env
WORKDIR /Identity
COPY "MicroService.sln" "MicroService.sln"
RUN dotnet restore
RUN dotnet publish -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 as runtime
WORKDIR /publish
COPY --from=build-env /publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "myWebApp.dll"]
