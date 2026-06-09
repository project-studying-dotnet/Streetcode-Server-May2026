# Streetcode.Auth Service

Authentication and authorization service for Streetcode project.

Uses:
- JWT authentication
- RabbitMQ (MassTransit)
- ASP.NET Core Web API
- User Secrets + Environment Variables

---

# 🚀 Local setup (Visual Studio / dotnet run)

## 1. Initialize User Secrets

```bash
dotnet user-secrets init --project Streetcode.Auth
```

## 2. Configure RabbitMQ credentials

```bash
dotnet user-secrets set "RabbitMq:UserName" "guest" --project Streetcode.Auth
dotnet user-secrets set "RabbitMq:Password" "guest" --project Streetcode.Auth
```

## 3. Configure database connection

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=StreetcodeDb.Auth;User Id=sa;Password=Admin@1234;TrustServerCertificate=True" --project Streetcode.Auth
```

## 4. Verify configuration

```bash
dotnet user-secrets list --project Streetcode.Auth
```

## 5. Run application

```bash
dotnet run --project Streetcode.Auth
```

---


# 🐳 Docker setup (.env)

## 1. Create .env file in repository root

```env
RabbitMq__UserName=guest
RabbitMq__Password=guest
```

## 2. Run services

```bash
docker compose --env-file ../.env up --build -d
```

## 3. Stop services

```bash
docker compose --env-file ../.env down
```

## 4. View logs

```bash
docker compose --env-file ../.env logs auth --tail 100
```

---