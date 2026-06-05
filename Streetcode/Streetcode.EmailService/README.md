## Local setup

Initialize user secrets:

```bash
dotnet user-secrets init
```

Configure SMTP:

```bash
dotnet user-secrets set "EmailConfiguration:From" "your@email.com"
dotnet user-secrets set "EmailConfiguration:SmtpServer" "smtp.gmail.com"
dotnet user-secrets set "EmailConfiguration:Port" "465"
dotnet user-secrets set "EmailConfiguration:UserName" "your@email.com"
dotnet user-secrets set "EmailConfiguration:Password" "your-app-password"
```

Run service:

```bash
dotnet run --project Streetcode.EmailService
<<<<<<< Updated upstream
```
=======
```

## RabbitMQ Configuration

RabbitMQ credentials should not be stored in source control.

Configure them locally using User Secrets:

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
RabbitMq:UserName = guest
RabbitMq:Password = guest
```

The following settings remain in `appsettings.json`:

```json
"RabbitMq": {
  "HostName": "localhost",
  "Port": 5672,
  "QueueName": "email-queue"
}
```
# Run EmailService in Docker

1. Create .env file based on .env.example

2. Start services

docker compose -f docker-compose.emailservice.yml --env-file .env up --build

3. Open RabbitMQ UI

http://localhost:15672

Default credentials:
guest / guest

4. Open EmailService

Health:
http://localhost:5190/health

Swagger:
http://localhost:5190/swagger
>>>>>>> Stashed changes
