# Local Backend Development

This guide describes how to run the backend services locally for frontend development.

## Prerequisites

- .NET SDK 10
- Docker and Docker Compose
- Node.js for the frontend

## Environment Files

Create local environment files from examples:

```bash
cp backend/Account/dev.env.example backend/Account/dev.env
cp backend/Chess/dev.env.example backend/Chess/dev.env
```

Do not commit `dev.env`. Only `dev.env.example` should be tracked.

## Start Databases

Start the Account database:

```bash
docker compose -f backend/Account/docker-compose.dev.yml up -d
```

Start the Chess database:

```bash
docker compose -f backend/Chess/docker-compose.dev.yml up -d
```

Check that containers are running:

```bash
docker ps
```

Local database ports:

```text
Account Postgres: localhost:5432
Chess Postgres:   localhost:5433
```

## Apply Migrations

Account applies migrations automatically when `Account.API` starts.

Apply Chess migrations manually from the repository root:

```bash
dotnet ef database update --project backend/Chess/Chess.Infrastructure/Chess.Infrastructure.csproj --startup-project backend/Chess/Chess.API/Chess.API.csproj
```

If you are already in `backend/Chess`, use:

```bash
dotnet ef database update --project Chess.Infrastructure/Chess.Infrastructure.csproj --startup-project Chess.API/Chess.API.csproj
```

## Run APIs

Run Account API:

```bash
dotnet run --project backend/Account/Account.API/Account.API.csproj --launch-profile http
```

Account API URL:

```text
http://localhost:8080
```

Run Chess API:

```bash
dotnet run --project backend/Chess/Chess.API/Chess.API.csproj --launch-profile http
```

Chess API URL:

```text
http://localhost:5122
```

## Frontend Integration

Frontend dev origin:

```text
http://localhost:5173
```

Backend base URLs:

```text
Account API: http://localhost:8080
Chess API:   http://localhost:5122
```

Send JWT tokens to Chess API with the `Authorization` header:

```http
Authorization: Bearer <token>
```

## Minimal Manual Flow

1. Start Account Postgres.
2. Start Chess Postgres.
3. Apply Chess migrations.
4. Run Account API.
5. Run Chess API.
6. Get a JWT from Account API.
7. Call Chess API with `Authorization: Bearer <token>`.
8. Start a game.
9. Make a move.

## Useful Checks

Validate Docker Compose configuration:

```bash
docker compose -f backend/Chess/docker-compose.dev.yml config
docker compose -f backend/Account/docker-compose.dev.yml config
```

Build backend APIs:

```bash
dotnet build backend/Account/Account.API/Account.API.csproj --no-restore --nologo -v:minimal --disable-build-servers -p:UseSharedCompilation=false -p:BuildInParallel=false
dotnet build backend/Chess/Chess.API/Chess.API.csproj --no-restore --nologo -v:minimal --disable-build-servers -p:UseSharedCompilation=false -p:BuildInParallel=false
```
