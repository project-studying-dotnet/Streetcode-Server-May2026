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
```