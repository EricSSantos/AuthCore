FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
WORKDIR /app
EXPOSE 8080
ENV DOTNET_EnableDiagnostics=0 \
    ASPNETCORE_HTTP_PORTS=8080

RUN apk add --no-cache icu-libs wget && \
    adduser -D -H -s /sbin/nologin appuser

# Restore
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["AuthCore/AuthCore.Api/AuthCore.Api.csproj", "AuthCore/AuthCore.Api/"]
COPY ["AuthCore/AuthCore.Application/AuthCore.Application.csproj", "AuthCore/AuthCore.Application/"]
COPY ["AuthCore/AuthCore.Domain/AuthCore.Domain.csproj", "AuthCore/AuthCore.Domain/"]
COPY ["AuthCore/AuthCore.Infrastructure/AuthCore.Infrastructure.csproj", "AuthCore/AuthCore.Infrastructure/"]
COPY ["AuthCore/AuthCore.sln", "AuthCore/"]
COPY ["AuthCore/Directory.Build.props", "AuthCore/"]

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore "./AuthCore/AuthCore.Api/AuthCore.Api.csproj" --runtime linux-musl-x64

COPY . .

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish "./AuthCore/AuthCore.Api/AuthCore.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    --runtime linux-musl-x64 \
    --self-contained false \
    /p:UseAppHost=false \
    /p:ContinuousIntegrationBuild=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER appuser
HEALTHCHECK --interval=30s --timeout=10s --start-period=20s --retries=3 \
    CMD wget -qO- http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "AuthCore.Api.dll"]
