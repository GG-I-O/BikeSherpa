FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Ggio.BikeSherpa.BackendSaaS/Ggio.BikeSherpa.BackendSaaS.csproj", "Ggio.BikeSherpa.BackendSaaS/"]
RUN dotnet restore "Ggio.BikeSherpa.BackendSaaS/Ggio.BikeSherpa.BackendSaaS.csproj"
COPY . .
WORKDIR "/src/Ggio.BikeSherpa.BackendSaaS"
RUN dotnet build "./Ggio.BikeSherpa.BackendSaaS.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ggio.BikeSherpa.BackendSaaS.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ggio.BikeSherpa.BackendSaaS.dll"]
