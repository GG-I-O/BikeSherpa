FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["sources/core/Ggio.BikeSherpa.Backend/Ggio.BikeSherpa.Backend.csproj", "sources/core/Ggio.BikeSherpa.Backend/"]
COPY ["sources/core/Ggio.BikeSherpa.Backend.Domain/Ggio.BikeSherpa.Backend.Domain.csproj", "sources/core/Ggio.BikeSherpa.Backend.Domain/"]
COPY ["sources/core/Ggio.DddCore/Ggio.DddCore.csproj", "sources/core/Ggio.DddCore/"]
COPY ["sources/core/Ggio.BikeSherpa.Backend.Infrastructure/Ggio.BikeSherpa.Backend.Infrastructure.csproj", "sources/core/Ggio.BikeSherpa.Backend.Infrastructure/"]
COPY ["sources/core/Ggio.DddCore.Infrastructure/Ggio.DddCore.Infrastructure.csproj", "sources/core/Ggio.DddCore.Infrastructure/"]
RUN dotnet restore "sources/core/Ggio.BikeSherpa.Backend/Ggio.BikeSherpa.Backend.csproj"
COPY . .
WORKDIR "/src/sources/core/Ggio.BikeSherpa.Backend"
RUN dotnet build "./Ggio.BikeSherpa.Backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ggio.BikeSherpa.Backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ggio.BikeSherpa.Backend.dll"]
