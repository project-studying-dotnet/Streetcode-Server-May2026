## API Gateway

### Routes

| Route | Service |
|---------|---------|
| /api/webapi/* | Streetcode.WebApi |
| /api/email/* | Streetcode.EmailService |
| /api/email/health | EmailService health check |

### Run locally

dotnet run --project Streetcode.WebApi
dotnet run --project Streetcode.EmailService
dotnet run --project Streetcode.ApiGateway