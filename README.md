# ReadOrNot

ReadOrNot is a production-style email image tracking application built with ASP.NET Core and a fully separated vanilla JavaScript frontend.

The system tracks image loads and open events, not guaranteed human reads. Users create unguessable tracking tokens, embed the resulting image URLs in email HTML, and review token-level reports that distinguish likely automated opens from probable human opens.

## Tech Stack

- Backend API: ASP.NET Core on .NET 10
- Frontend host: separate ASP.NET Core project serving static files
- Frontend UI: HTML, CSS, plain JavaScript modules, Fetch API
- Persistence: EF Core with provider selection via configuration
- Supported database providers: SQL Server and MySQL
- Authentication: ASP.NET Core Identity with JWT bearer tokens
- Tests: xUnit unit and integration tests

## Solution Layout

```text
ReadOrNot.slnx
src/
  ReadOrNot.Domain/               Domain entities and core rules
  ReadOrNot.Application/          DTOs, options, interfaces, exceptions
  ReadOrNot.Infrastructure/       EF Core, Identity, services, provider wiring
  ReadOrNot.Migrations.SqlServer/ Provider-specific SQL Server migrations
  ReadOrNot.Migrations.MySql/     Provider-specific MySQL migrations
  ReadOrNot.Api/                  REST API, controllers, middleware, tracking asset
  ReadOrNot.Web/                  Separate static frontend host
tests/
  ReadOrNot.Domain.Tests/         Unit tests
  ReadOrNot.Api.IntegrationTests/ API integration tests
```

## Architecture Overview

The codebase uses a clean layered split:

- `ReadOrNot.Domain` contains the token and open-event entities plus small domain behavior such as expiration checks.
- `ReadOrNot.Application` defines the contracts used by the API and infrastructure layers: DTOs, options, interfaces, and application exceptions.
- `ReadOrNot.Infrastructure` implements Identity, JWT issuance, EF Core persistence, reporting queries, public tracking logic, bot heuristics, and configurable IP privacy handling.
- `ReadOrNot.Api` exposes the versioned REST API, public tracking endpoint, rate limiting, global exception handling, health checks, and CORS.
- `ReadOrNot.Web` is fully separate from the API and serves a dependency-free SPA written in modular vanilla JavaScript.

## Key Features

- Account registration, login, logout, profile editing, password change, forgot-password, and reset-password flows
- Token CRUD with unguessable public identifiers and copyable tracking image URLs
- Public tracking endpoint at `/t/{publicIdentifier}`
- UTC open-event logging with IP strategy control, user agent, referer, accept-language, query-string snapshot, and bot-heuristic flags
- Token-level reporting with date-range filtering and bot include/exclude toggles
- Provider-specific migrations for SQL Server and MySQL
- Rate limiting on auth and tracking endpoints
- Health endpoint at `/health`
- Structured exception responses with validation details

## Local Development

### Prerequisites

- .NET SDK 10.0.201 or compatible feature roll-forward
- A running SQL Server or MySQL instance
- ASP.NET Core development certificate trusted locally if using HTTPS

### Configure the JWT signing key

The API intentionally rejects the placeholder signing key in `appsettings.json`. Set a development-only key before running the API:

```bash
cd src/ReadOrNot.Api
dotnet user-secrets init
dotnet user-secrets set "Jwt:SigningKey" "replace-with-a-long-random-development-key"
```

### Choose a database provider

The API reads the active provider from `Database:Provider` and `Database:ConnectionStringName`.

- SQL Server sample settings: [src/ReadOrNot.Api/appsettings.SqlServer.sample.json](/Users/brustolin/GitHub/ReadOrNot/src/ReadOrNot.Api/appsettings.SqlServer.sample.json)
- MySQL sample settings: [src/ReadOrNot.Api/appsettings.MySql.sample.json](/Users/brustolin/GitHub/ReadOrNot/src/ReadOrNot.Api/appsettings.MySql.sample.json)

Default local development URLs are:

- API HTTPS: `https://localhost:7040`
- Frontend HTTPS: `https://localhost:7067`

### Run the backend API

```bash
dotnet run --project src/ReadOrNot.Api
```

### Run the frontend host

```bash
dotnet run --project src/ReadOrNot.Web
```

The frontend fetches its API base URL from `ReadOrNot.Web` configuration at runtime through `/client-config`.

## Switching Between SQL Server and MySQL

1. Set `Database:Provider` to either `SqlServer` or `MySql`.
2. Set `Database:ConnectionStringName` to the matching connection string name.
3. Update the matching connection string.
4. Apply the matching migrations project to the target database.

Example `appsettings.json` fragments:

```json
{
  "Database": {
    "Provider": "SqlServer",
    "ConnectionStringName": "SqlServer"
  }
}
```

```json
{
  "Database": {
    "Provider": "MySql",
    "ConnectionStringName": "MySql",
    "MySqlServerVersion": "8.0.36"
  }
}
```

## Migrations

This repo keeps migrations per provider so SQL Server and MySQL can evolve independently.

### Create a new SQL Server migration

```bash
dotnet ef migrations add MigrationName \
  --project src/ReadOrNot.Migrations.SqlServer/ReadOrNot.Migrations.SqlServer.csproj \
  --startup-project src/ReadOrNot.Migrations.SqlServer/ReadOrNot.Migrations.SqlServer.csproj \
  --context ReadOrNot.Infrastructure.Persistence.ReadOrNotDbContext \
  --output-dir Migrations
```

### Create a new MySQL migration

```bash
dotnet ef migrations add MigrationName \
  --project src/ReadOrNot.Migrations.MySql/ReadOrNot.Migrations.MySql.csproj \
  --startup-project src/ReadOrNot.Migrations.MySql/ReadOrNot.Migrations.MySql.csproj \
  --context ReadOrNot.Infrastructure.Persistence.ReadOrNotDbContext \
  --output-dir Migrations
```

### Apply SQL Server migrations to a database

```bash
dotnet ef database update \
  --project src/ReadOrNot.Migrations.SqlServer/ReadOrNot.Migrations.SqlServer.csproj \
  --startup-project src/ReadOrNot.Migrations.SqlServer/ReadOrNot.Migrations.SqlServer.csproj \
  --context ReadOrNot.Infrastructure.Persistence.ReadOrNotDbContext \
  -- connection="Server=localhost,1433;Database=ReadOrNot;User Id=sa;Password=YOUR_PASSWORD_HERE;TrustServerCertificate=True"
```

### Apply MySQL migrations to a database

```bash
dotnet ef database update \
  --project src/ReadOrNot.Migrations.MySql/ReadOrNot.Migrations.MySql.csproj \
  --startup-project src/ReadOrNot.Migrations.MySql/ReadOrNot.Migrations.MySql.csproj \
  --context ReadOrNot.Infrastructure.Persistence.ReadOrNotDbContext \
  -- connection="Server=localhost;Port=3306;Database=readornot;User=root;Password=YOUR_PASSWORD_HERE" \
     serverVersion="8.0.36"
```

## Privacy and Tracking Notes

- ReadOrNot records image loads and opens, not guaranteed reads.
- Some email clients preload or proxy remote images. These events are kept, but flagged as likely bots or automated activity instead of being discarded.
- IP storage is configurable through `Privacy:IpStorageMode`:
  - `Full`
  - `Hashed`
  - `None`
- Old open events can be purged later through the configurable retention settings:
  - `Retention:PurgeEnabled`
  - `Retention:OpenEventsRetentionDays`
  - `Retention:CleanupIntervalHours`
- The privacy logic is isolated behind `IIpPrivacyService` so user-specific privacy settings can be added later without rewriting the tracking pipeline.

## Auth and Account Notes

- Authentication uses ASP.NET Core Identity for user management and password reset token generation.
- The API uses JWT bearer tokens for the separated frontend/backend setup.
- Forgot-password works without an email transport for now:
  - In Development, the API returns a reset-token preview and frontend reset URL.
  - In production, you would normally replace that with a real email sender.

## Testing

Run everything:

```bash
dotnet test ReadOrNot.slnx -c Debug
```

Included coverage areas:

- Token expiration and bot-detection unit tests
- Protected-route authorization checks
- Cross-user token ownership protection
- Public tracking endpoint behavior and event logging

## API Surface

Primary routes:

- `POST /api/v1/auth/register`
- `POST /api/v1/auth/login`
- `POST /api/v1/auth/logout`
- `POST /api/v1/auth/forgot-password`
- `POST /api/v1/auth/reset-password`
- `GET /api/v1/account`
- `PUT /api/v1/account`
- `POST /api/v1/account/change-password`
- `GET /api/v1/tokens`
- `POST /api/v1/tokens`
- `GET /api/v1/tokens/{id}`
- `PUT /api/v1/tokens/{id}`
- `POST /api/v1/tokens/{id}/enable`
- `POST /api/v1/tokens/{id}/disable`
- `DELETE /api/v1/tokens/{id}`
- `GET /api/v1/reports/tokens/{id}`
- `GET /t/{publicIdentifier}`
- `GET /health`

## Future Enhancements

- Custom domains for tracking URLs
- Webhooks on open events
- Team and shared workspaces
- CSV export
- API keys
- Email templates
- Geolocation enrichment
- Advanced bot detection
- Dashboards and richer charts
- Per-user privacy controls and retention policies
