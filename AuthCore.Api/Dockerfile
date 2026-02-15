# Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração e como o Visual Studio usa este Dockerfile para criar suas imagens para uma depuração mais rápida.

# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
ENV DOTNET_EnableDiagnostics=0

RUN useradd --create-home --shell /bin/bash appuser

# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["AuthCore.Api/AuthCore.Api.csproj", "AuthCore.Api/"]
COPY ["AuthCore.Application/AuthCore.Application.csproj", "AuthCore.Application/"]
COPY ["AuthCore.Domain/AuthCore.Domain.csproj", "AuthCore.Domain/"]
COPY ["AuthCore.Infrastructure/AuthCore.Infrastructure.csproj", "AuthCore.Infrastructure/"]

RUN dotnet restore "./AuthCore.Api/AuthCore.Api.csproj"

COPY . .
WORKDIR "/src/AuthCore.Api"
# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AuthCore.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false /p:ContinuousIntegrationBuild=true --no-restore

# Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER appuser
ENTRYPOINT ["dotnet", "AuthCore.Api.dll"]
