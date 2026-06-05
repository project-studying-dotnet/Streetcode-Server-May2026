## Local setup

Initialize user secrets:

```bash
dotnet user-secrets init --project Streetcode.EmailService
```

Configure SMTP:

```bash
dotnet user-secrets set "EmailConfiguration:From" "your@email.com" --project Streetcode.EmailService
dotnet user-secrets set "EmailConfiguration:SmtpServer" "smtp.gmail.com" --project Streetcode.EmailService
dotnet user-secrets set "EmailConfiguration:Port" "465" --project Streetcode.EmailService
dotnet user-secrets set "EmailConfiguration:UserName" "your@email.com" --project Streetcode.EmailService
dotnet user-secrets set "EmailConfiguration:Password" "your-app-password" --project Streetcode.EmailService
```

Configure RabbitMQ credentials:

```bash
dotnet user-secrets set "RabbitMq:UserName" "guest" --project Streetcode.EmailService
dotnet user-secrets set "RabbitMq:Password" "guest" --project Streetcode.EmailService
```

Verify configuration:

```bash
dotnet user-secrets list --project Streetcode.EmailService
```

Example output:

```text
EmailConfiguration:From = your@email.com
EmailConfiguration:SmtpServer = smtp.gmail.com
EmailConfiguration:Port = 465
EmailConfiguration:UserName = your@email.com
EmailConfiguration:Password = your-app-password
RabbitMq:UserName = guest
RabbitMq:Password = guest
```

The following RabbitMQ settings remain in `appsettings.json`:

```json
"RabbitMq": {
  "HostName": "localhost",
  "Port": 5672,
  "QueueName": "email-queue"
}
```

Start RabbitMQ locally if it is not already running:

```bash
docker compose --env-file ../.env up rabbitmq -d
```

Run EmailService:

```bash
dotnet run --project Streetcode.EmailService
```

Health check:

```text
http://localhost:5190/health
```

Swagger:

```text
http://localhost:5190/swagger
```

## Docker setup

Create `.env` file in the repository root directory based on `.env.example`.

Start all services from the `Streetcode` directory:

```bash
docker compose --env-file ../.env up --build -d
```

Stop all services:

```bash
docker compose --env-file ../.env down
```

Remove containers, network and database volume:

```bash
docker compose --env-file ../.env down --remove-orphans -v
```

Available services:

| Service              | URL                           |
| -------------------- | ----------------------------- |
| Web API Swagger      | http://localhost:5000/swagger |
| EmailService Swagger | http://localhost:5190/swagger |
| EmailService Health  | http://localhost:5190/health  |
| RabbitMQ Management  | http://localhost:15672        |

RabbitMQ default credentials:

```text
guest / guest
```

View logs:

```bash
docker compose --env-file ../.env logs webapi --tail 100
docker compose --env-file ../.env logs emailservice --tail 100
docker compose --env-file ../.env logs rabbitmq --tail 100
```
