# Streetcode.Auth Service

- GoogleAuth:
---
## 1. Configure GoogleAuth__ClientId

```bash
dotnet user-secrets set "GoogleAuth:ClientId" "123721973387-7i3rs06c8iui5lrb805f22o0k2s6gk1o.apps.googleusercontent.com"
```

## 2. Create .env file in repository root example

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

GoogleAuth__ClientId=123721973387-7i3rs06c8iui5lrb805f22o0k2s6gk1o.apps.googleusercontent.com
```


---