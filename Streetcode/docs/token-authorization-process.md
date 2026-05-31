# ADR-001: JWT-Based Authorization and Token Handling

## Status

Accepted

## Context

The project requires an authorization mechanism for authenticated API access and role-based endpoint protection.

The backend is built with:

* ASP.NET Core
* ASP.NET Identity
* MediatR
* JWT Bearer Authentication

The system currently supports:

* user login;
* JWT token generation;
* JWT validation;
* role-based authorization;
* Swagger authentication support.

The implementation is based on:

* `ITokenService`
* `TokenService`
* `LoginUserHandler`
* `AddAuthenticationServices`

## Decision

The project uses JWT Bearer authentication as the primary authorization mechanism.

### Authentication Flow

1. User submits login credentials.
2. Credentials are validated using ASP.NET Identity.
3. JWT access token is generated.
4. Client stores token and sends it in:

   ```http
   Authorization: Bearer {token}
   ```
5. Backend validates token for protected endpoints.

### Token Contents

JWT tokens include:

* `NameIdentifier`
* `Name`
* `Role`

### Token Validation

The system validates:

* issuer;
* audience;
* token lifetime;
* signing key.

Validation is configured through `TokenValidationParameters`.

### Token Signing

Tokens are signed using:

* `HmacSha256`
* symmetric secret key from configuration.

### Authorization

Protected endpoints use:

```csharp
[Authorize]
```

Role-based access uses:

```csharp
[Authorize(Roles = "...")]
```

### Swagger Integration

Swagger supports Bearer authentication for testing protected endpoints.

## Current Refresh Token Behavior

The current implementation contains:

```csharp
RefreshToken(string token)
```

However, this is not a full refresh-token implementation.

Current behavior:

* validates expired token signature;
* extracts claims;
* generates a new access token.

The system does not currently:

* persist refresh tokens;
* revoke refresh tokens;
* rotate refresh tokens;
* validate user state during refresh.

Therefore, the current implementation should be treated as a token re-issue mechanism rather than a production-ready refresh-token flow.

## Consequences

### Positive

* Simple JWT-based authentication flow.
* Stateless authorization.
* Easy frontend integration.
* Built-in ASP.NET middleware support.
* Swagger integration for testing.
* Role-based authorization support.
* Minimal infrastructure complexity.

### Negative

* No refresh token persistence.
* No token revocation support.
* Potential security risks if access token is compromised.
* No session invalidation mechanism.
* No refresh token rotation.
* Limited auditability for active sessions.

## Security Considerations

### Current Strengths

* Generic login failure responses.
* JWT signature validation enabled.
* Token expiration validation enabled.
* Issuer and audience validation enabled.
* Role claim support.

### Current Risks

* Refresh flow is incomplete.
* Long-lived JWTs may increase security risk.
* LocalStorage token storage may expose tokens to XSS attacks.
* No revocation support for compromised tokens.

## Recommendations

### Short-Term

* Keep current JWT access-token flow.
* Use short-lived access tokens.
* Ensure all protected endpoints use `[Authorize]`.
* Document frontend token handling strategy.

### Long-Term

Implement full refresh-token support:

* refresh token database storage;
* refresh token rotation;
* token revocation;
* device/session tracking;
* refresh token expiration;
* user-state validation during refresh.

Recommended future architecture:

* short-lived access token;
* long-lived refresh token;
* secure refresh endpoint;
* revocable sessions.

## Alternatives Considered

### Cookie-Based Authentication

Rejected because:

* API is designed for token-based frontend communication;
* JWT better supports SPA/mobile integrations.

### Server-Side Session Storage

Rejected because:

* increases infrastructure complexity;
* reduces scalability;
* introduces server-side session management requirements.

## References

Main related files:

* `ITokenService.cs`
* `TokenService.cs`
* `LoginUserHandler.cs`
* `ServiceCollectionExtensions.cs`

Configuration:

* `JwtSettings`
* `appsettings.json`
