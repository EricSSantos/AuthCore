FROM mcr.microsoft.com/dotnet/runtime:9.0-alpine AS base
WORKDIR /app
ENV DOTNET_EnableDiagnostics=0

RUN apk add --no-cache icu-libs && \
    adduser -D -H -s /sbin/nologin appuser

# Restore
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["MailCore.Worker/MailCore.Worker.csproj", "MailCore.Worker/"]
COPY ["MailCore.Service/MailCore.Service.csproj", "MailCore.Service/"]
COPY ["MailCore.sln", "./"]

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore "./MailCore.Worker/MailCore.Worker.csproj" --runtime linux-musl-x64

COPY . .

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish "./MailCore.Worker/MailCore.Worker.csproj" \
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
ENTRYPOINT ["dotnet", "MailCore.Worker.dll"]
