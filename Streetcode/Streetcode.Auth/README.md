# Streetcode.Auth Service

Authentication and authorization service for Streetcode project.

Uses:
- JWT authentication
- RabbitMQ
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

## 4. Configure jwt

```bash
dotnet user-secrets set "Jwt:Key" "StreetcodeSuperSecretJwtKeyForDevelopmentOnly1234567890" --project Streetcode.Auth
dotnet user-secrets set "Jwt:Issuer" "Streetcode" --project Streetcode.Auth
dotnet user-secrets set "Jwt:Audience" "StreetcodeUsers" --project Streetcode.Auth

dotnet user-secrets set "RabbitMq:UserName" "guest" --project Streetcode.Auth
dotnet user-secrets set "RabbitMq:Password" "guest" --project Streetcode.Aut
```

## 5. Run application

```bash
dotnet run --project Streetcode.Auth
```

---


# 🐳 Docker setup (.env)

## 1. Create .env file in repository root example

```env
DB_PASSWORD=Admin@1234
RABBITMQ_USERNAME=guest
RABBITMQ_PASSWORD=guest

JWT_KEY=StreetcodeSuperSecretJwtKeyForDevelopmentOnly1234567890
JWT_ISSUER=Streetcode
JWT_AUDIENCE=StreetcodeUsers
JWT_ACCESS_TOKEN_LIFETIME_IN_MINUTES=120
JWT_REFRESH_TOKEN_LIFETIME_IN_DAYS=7


EmailConfiguration:From = your@email.com
EmailConfiguration:SmtpServer = smtp.gmail.com
EmailConfiguration:Port = 465
EmailConfiguration:UserName = your@email.com
EmailConfiguration:Password = your-app-password

ADMIN_EMAIL=admin@gmail.com
ADMIN_PASSWORD=admin@1234
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