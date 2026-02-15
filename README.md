# AuthCore

A ideia é transformar este projeto em um boilerplate para iniciar APIs em .NET 9 ou um serviço isolado de autenticação. Suporta autenticação via JWT e gerenciamento de sessões.

## Requisitos

* .NET SDK 9
* PostgreSQL
* Redis
* RabbitMQ

## Como rodar

1. Build

   ```bash
   dotnet build
   ```
3. Criar migration

   ```bash
   dotnet ef migrations add InitialCreate --output-dir Persistence/Migrations --project AuthCore.Infrastructure --startup-project AuthCore.Api --context EFCoreDbContext
   ```
4. Aplicar migrations

   ```bash
   dotnet ef database update --project AuthCore.Infrastructure --startup-project AuthCore.Api --context EFCoreDbContext
   ```
5. Rodar API

   ```bash
   dotnet run --project AuthCore.Api
   ```

## Configurações

### Local

Edite:

* `appsettings.json`
* `appsettings.Development.json`

### Docker

Crie um arquivo `.env` na raiz do projeto:

```bash
# Postgres
DATABASE__POSTGRES__CONNECTIONSTRING=Host=<DB_HOST>;Port=<DB_PORT>;Database=<DB_NAME>;Username=<DB_USER>;Password=<DB_PASSWORD>

# Redis
DATABASE__REDIS__CONNECTIONSTRING=Host=<REDIS_HOST:PORT>;Password=<REDIS_PASSWORD>

# CORS
CORS__DEVELOPMENTORIGINS__0=<DEV_ORIGIN>
CORS__PRODUCTIONORIGINS__0=<PROD_ORIGIN>

# JWT
SECURITY__JWT__ISSUER=<JWT_ISSUER>
SECURITY__JWT__AUDIENCE=<JWT_AUDIENCE>
SECURITY__JWT__EXPIRESINMINUTES=30

# Keys
SECURITY__KEYS__SYMMETRIC__PRIVATEKEY=<SYMMETRIC_PRIVATE_KEY>
SECURITY__KEYS__ASYMMETRIC__PRIVATEKEYPATH=<ASYMMETRIC_PRIVATE_KEY_PATH>
SECURITY__KEYS__ASYMMETRIC__PUBLICKEYPATH=<ASYMMETRIC_PUBLIC_KEY_PATH>

# RabbitMQ
RABBITMQ__HOST=<RABBITMQ_HOST>
RABBITMQ__PORT=<RABBITMQ_PORT>
RABBITMQ__USER=<RABBITMQ_USER>
RABBITMQ__PASSWORD=<RABBITMQ_PASSWORD>
RABBITMQ__EMAILQUEUE=<RABBITMQ_EMAILQUEUE>
RABBITMQ__DEADLETTERQUEUE=<RABBITMQ_DEADLETTERQUEUE>
```

### Modelagem

<p align="center">
  <img width="720" height="720" alt="User" src="https://github.com/user-attachments/assets/e8f49f51-5feb-4cd4-948b-388d50295802" />
</p>
