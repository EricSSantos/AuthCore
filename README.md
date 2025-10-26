# AuthCore

![.NET](https://img.shields.io/badge/.NET-9.0-blue?logo=dotnet\&logoColor=white)
[![Docker](https://img.shields.io/badge/Docker-Supported-2496ED?logo=docker)](https://www.docker.com/)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue?logo=postgresql\&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-7-red?logo=redis\&logoColor=white)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-3.13+-orange?logo=rabbitmq\&logoColor=white)

**Boilerplate para inicialização de APIs em .NET 9**, com suporte a autenticação via **JWT**, gerenciamento de **sessões**.

### 🧩 Pré-requisitos

* [.NET SDK 9.0](https://dotnet.microsoft.com/download)
* [PostgreSQ](https://www.postgresql.org/download/)
* [Redis](https://redis.io/download/)
* [RabbitMQ](https://www.rabbitmq.com/download.html)
* [MailWorker](https://github.com/EricSSantos/MailWorker)
* [Git](https://git-scm.com/)

---

<details>
<summary>Configurações </summary>

#### Execução local

O projeto utiliza um arquivo appsettings.json para centralizar as configurações locais de ambiente:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Api": {
    "Versions": {
      "v1": {
        "Name": "API v1",
        "Description": "Initial API version"
      }
    }
  },
  "Database": {
    "Postgres": {
      "ConnectionString": "Host=<DB_HOST>;Port=<DB_PORT>;Database=<DB_NAME>;Username=<DB_USER>;Password=<DB_PASSWORD>"
    },
    "Redis": {
      "ConnectionString": "Host=<REDIS_HOST:PORT>;Password=<REDIS_PASSWORD>",
      "InstanceName": "<REDIS_INSTANCE_NAME>"
    }
  },
  "Cors": {
    "DevelopmentOrigins": [
      "<DEV_ORIGIN>"
    ],
    "ProductionOrigins": [
      "<PRODUCTION_ORIGIN>"
    ]
  },
  "Security": {
    "Jwt": {
      "Issuer": "<JWT_ISSUER>",
      "Audience": "<JWT_AUDIENCE>",
      "ExpiresInMinutes": 30
    },
    "Session": {
      "ExpiresInDays": 7,
      "MaxLifetimeInDays": 30
    },
    "Keys": {
      "Symmetric": {
        "PrivateKey": "<SYMMETRIC_PRIVATE_KEY>"
      },
      "Asymmetric": {
        "PrivateKeyPath": "<ASYMMETRIC_PRIVATE_KEY_PATH>",
        "PublicKeyPath": "<ASYMMETRIC_PUBLIC_KEY_PATH>"
      }
    }
  },
  "RabbitMQ": {
    "Host": "<RABBITMQ_HOST>",
    "Port": "<RABBITMQ_PORT>",
    "User": "<RABBITMQ_USER>",
    "Password": "<RABBITMQ_PASSWORD>",
    "EmailQueue": "<RABBITMQ_EMAILQUEUE>",
    "DeadLetterQueue": "<RABBITMQ_DEADLETTERQUEUE>"
  }
}
```

#### Execução via Docker

O container utiliza as mesmas chaves do appsettings.json,
seguindo a convenção SECTION__SUBSECTION__KEY.
Basta criar um arquivo .env com os valores correspondentes:

```bash
# Database
DATABASE__POSTGRES__CONNECTIONSTRING=Host=<DB_HOST>;Port=<DB_PORT>;Database=<DB_NAME>;Username=<DB_USER>;Password=<DB_PASSWORD>
DATABASE__REDIS__CONNECTIONSTRING=Host=<REDIS_HOST:PORT>;Password=<REDIS_PASSWORD>
DATABASE__REDIS__INSTANCENAME=<REDIS_INSTANCE_NAME>

# CORS
CORS__DEVELOPMENTORIGINS__0=<DEV_ORIGIN>
CORS__PRODUCTIONORIGINS__0=<PRODUCTION_ORIGIN>

# Security
SECURITY__JWT__ISSUER=<JWT_ISSUER>
SECURITY__JWT__AUDIENCE=<JWT_AUDIENCE>
SECURITY__JWT__EXPIRESINMINUTES=30
SECURITY__SESSION__EXPIRESINDAYS=7
SECURITY__SESSION__MAXLIFETIMEINDAYS=30
SECURITY__KEYS__SYMMETRIC__PRIVATEKEY=<SYMMETRIC_PRIVATE_KEY>
SECURITY__KEYS__ASYMMETRIC__PRIVATEKEYPATH=/keys/private.pem
SECURITY__KEYS__ASYMMETRIC__PUBLICKEYPATH=/keys/public.pem

# RabbitMQ
RABBITMQ__HOST=<RABBITMQ_HOST>
RABBITMQ__PORT=<RABBITMQ_PORT>
RABBITMQ__USER=<RABBITMQ_USER>
RABBITMQ__PASSWORD=<RABBITMQ_PASSWORD>
RABBITMQ__EMAILQUEUE=<RABBITMQ_EMAILQUEUE>
RABBITMQ__DEADLETTERQUEUE=<RABBITMQ_DEADLETTERQUEUE>
```

E execute:

```bash
docker compose up -d
```

#### Chaves EC (Elliptic Curve)

O projeto utiliza criptografia **ECDSA (Elliptic Curve Digital Signature Algorithm)** para assinar e validar tokens JWT.

```bash
# Gera chave privada (EC)
openssl ecparam -name prime256v1 -genkey -noout -out private_key.pem

# Gera chave pública a partir da privada
openssl ec -in private_key.pem -pubout -out public_key.pem
```

Defina os caminhos das chaves no appsettings.json ou .env:

```json
"PrivateKeyPath": "<ASYMMETRIC_PRIVATE_KEY_PATH>",
"PublicKeyPath": "<ASYMMETRIC_PUBLIC_KEY_PATH>"
```

> A chave privada é usada para **assinar** o token.
> A chave pública é usada para **validar** a assinatura.

</details>

<details>
<summary>Execução manual (.NET CLI)</summary>

1. **Restaure as dependências:**

   ```bash
   dotnet restore
   ```

2. **Compile a solução:**

   ```bash
   dotnet build
   ```

3. **Aplique as migrations (EF Core):**

   ```bash
   dotnet ef database update --project src/AuthCore.Infrastructure
   ```

   > Esse comando cria o banco de dados e aplica o schema definido pelas entidades do domínio.

4. **Execute a API:**

   ```bash
   dotnet run --project src/AuthCore.Api
   ```

5. **Acesse o Swagger:**

   ```
   https://localhost:7241/
   ```

</details>

<details>
<summary>Endpoints</summary>

#### Auth

| Método   | Endpoint                     | Descrição                              |
| -------- | ---------------------------- | -------------------------------------- |
| **POST** | `/api/v1/auth/sign-in`       | Faz login e inicia uma nova sessão.    |
| **POST** | `/api/v1/auth/sign-out`      | Faz logout e encerra a sessão atual.   |
| **POST** | `/api/v1/auth/refresh-token` | Atualiza o token de acesso do usuário. |

#### Sessions

| Método     | Endpoint           | Descrição                                                |
| ---------- | ------------------ | -------------------------------------------------------- |
| **GET**    | `/api/v1/sessions` | Lista outras sessões ativas do usuário.                  |
| **DELETE** | `/api/v1/sessions` | Encerra todas as outras sessões e mantém apenas a atual. |

#### Users

| Método    | Endpoint                           | Descrição                                               |
| --------- | ---------------------------------- | ------------------------------------------------------- |
| **POST**  | `/api/v1/users`                    | Registra um novo usuário.                               |
| **GET**   | `/api/v1/users/me`                 | Retorna os dados do usuário atual.                      |
| **PATCH** | `/api/v1/users/me/change-password` | Altera a senha do usuário atual.                        |
| **POST**  | `/api/v1/users/forgot-password`    | Envia um código de recuperação para o e-mail informado. |
| **PATCH** | `/api/v1/users/reset-password`     | Redefine a senha usando o código recebido.              |

</details>
