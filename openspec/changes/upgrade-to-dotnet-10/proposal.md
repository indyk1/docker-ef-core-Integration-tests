# Proposal

## Why

The solution targets .NET 6, which went out of support in November 2024, and its NuGet packages are pinned to 2022 releases (EF Core 6.0.x, Testcontainers 2.1.0, xUnit 2.4.1). That means no security patches, and the Testcontainers 2.x API used by the integration tests is now obsolete. Moving to .NET 10, the current LTS release, puts the project back on a supported runtime with a support window that runs until November 2028.

## What Changes

- Change `TargetFramework` from `net6.0` to `net10.0` in both projects (`Docker.Example`, `Docker.Example.Tests.Integration`).
- Add a `global.json` that pins the .NET 10 SDK so local and CI builds use the same toolchain.
- Upgrade the ASP.NET Core and EF Core packages to 10.0.x: Identity.EntityFrameworkCore, Identity.UI, Diagnostics.EntityFrameworkCore, EntityFrameworkCore.SqlServer, EntityFrameworkCore.Sqlite, EntityFrameworkCore.Tools, Mvc.Testing and Web.CodeGeneration.Design.
- Upgrade the test tooling: Microsoft.NET.Test.Sdk 18.x, coverlet.collector 10.x and NSubstitute 6.x.
- **BREAKING (tests only)**: Move from xUnit v2 (`xunit` 2.4.1) to xUnit v3 (`xunit.v3`). The `IAsyncLifetime` method signatures change to return `ValueTask`, and `ITestOutputHelper` moves to a different namespace.
- **BREAKING (tests only)**: Upgrade Testcontainers from 2.1.0 to 4.x and switch to the `Testcontainers.MsSql` module. `TestcontainersBuilder<TestcontainersContainer>` is replaced by `MsSqlBuilder`. The container gets a random host port, and the tests read its connection string from the container instead of using a hard-coded value.
- Replace FluentAssertions with AwesomeAssertions. FluentAssertions 8.x requires a paid commercial licence. AwesomeAssertions is the Apache-2.0 fork with a compatible API.
- Replace the placeholder `Assert.True(true)` with a real assertion on the user returned by `GET api/Users`, so the test actually checks the upgraded stack.
- Fix the SQL Server connection string for the stricter TLS defaults introduced in Microsoft.Data.SqlClient 4+ (`Encrypt=True` by default), so the app can still connect to the local and containerised SQL Server.
- Regenerate or add EF Core migrations if the EF Core 10 / Identity 10 model differs from the 6.0 snapshot, so `Database.MigrateAsync()` does not fail with a pending-model-changes error.

## Capabilities

### New Capabilities

- `user-lookup-api`: The HTTP API that returns the seeded Identity user. Recorded as a baseline so the upgrade can be shown to keep its behaviour unchanged.
- `integration-test-harness`: How the integration tests provision an isolated SQL Server, point the app at it, and check real behaviour on the supported runtime.

### Modified Capabilities

<!-- None: openspec/specs/ is empty; this is the project's first change. -->

## Impact

- **Code**: Both `.csproj` files, `src/Docker.Example/Program.cs` (the explicit `public partial class Program` is redundant in .NET 10), `src/Docker.Example/appsettings.json`, `src/Docker.Example/Migrations/*`, `tests/.../SomethingFactory.cs` and `tests/.../Tests/SomeStuff.cs`.
- **Tooling**: Contributors and CI need the .NET 10 SDK, and `dotnet-ef` must be on 10.x to create migrations. Running the tests still requires Docker.
- **Runtime behaviour**: The public behaviour of the web app and `api/Users` is intended to stay the same. The Identity database schema only changes if EF Core 10 reports pending model changes.
- **Licensing**: Removing FluentAssertions avoids the Xceed commercial licence that versions 8.0 and later require.
