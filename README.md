# Docker EF Core Integration Tests

An ASP.NET Core MVC app using ASP.NET Core Identity on SQL Server via EF Core, with
integration tests that run the app in-process against a throwaway SQL Server container
started by [Testcontainers](https://dotnet.testcontainers.org/).

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (pinned by `global.json`)
- [Docker](https://docs.docker.com/get-docker/), running, for the integration tests
- Optional: `dotnet-ef` 10.x for working with migrations (`dotnet tool install --global dotnet-ef`)

## Running the app

The app expects a SQL Server on `localhost:1433` (see `ConnectionStrings:DefaultConnection`
in `src/Docker.Example/appsettings.json`). Start one with Docker:

```bash
docker run -d --name sql -e ACCEPT_EULA=Y -e MSSQL_SA_PASSWORD='password123!' \
  -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
dotnet run --project src/Docker.Example
```

On startup the app applies migrations and seeds the user `test.test@test.com`, which
`GET /api/Users` returns.

## Running the tests

```bash
dotnet test
```

Each run starts its own SQL Server container on a random port and removes it afterwards,
so no local SQL Server is needed and one already running on port 1433 does not interfere.
The first run pulls the SQL Server image (about 1.5 GB).
