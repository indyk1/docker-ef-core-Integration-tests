# Tasks

## 1. Toolchain

- [ ] 1.1 Install the .NET 10 SDK and `dotnet-ef` 10.x (`dotnet tool update --global dotnet-ef`). Verify: `dotnet --list-sdks` shows a 10.0.x SDK and `dotnet ef --version` reports 10.x
- [ ] 1.2 Add `global.json` at the repo root with `"sdk": { "version": "10.0.100", "rollForward": "latestFeature" }` (design D2). Verify: `dotnet --version` run from the repo root prints 10.0.x

## 2. Web app on .NET 10

- [ ] 2.1 In `src/Docker.Example/Docker.Example.csproj`, set `<TargetFramework>net10.0</TargetFramework>`, remove the `Microsoft.EntityFrameworkCore.Sqlite` reference (design D10), and upgrade every remaining `PackageReference` to the target versions in the design.md table (Identity, Diagnostics, EF Core SqlServer/Tools at 10.0.x; Web.CodeGeneration.Design at 10.0.x). Verify: `dotnet restore src/Docker.Example` succeeds and `dotnet list src/Docker.Example package --outdated` shows nothing outdated
- [ ] 2.2 Remove `public partial class Program { }` from `src/Docker.Example/Program.cs` (design D8). Verify: `dotnet build src/Docker.Example` succeeds with no ASP0027 warning
- [ ] 2.3 Add `TrustServerCertificate=True` to `ConnectionStrings:DefaultConnection` in `src/Docker.Example/appsettings.json` (design D7). Verify: `dotnet run --project src/Docker.Example` against a local SQL Server container gets through startup with no TLS/certificate error
- [ ] 2.4 Run `dotnet ef migrations has-pending-model-changes --project src/Docker.Example`. If it reports changes, run `dotnet ef migrations add UpgradeToNet10 --project src/Docker.Example` and check that the migration only touches Identity tables in ways that keep existing data (design D9). Verify: re-running `has-pending-model-changes` reports no changes
- [ ] 2.5 Fix any remaining compiler or obsolescence warnings that the upgrade introduced in `src/Docker.Example`. Verify: `dotnet build src/Docker.Example -warnaserror` succeeds

## 3. Integration test harness on .NET 10

- [ ] 3.1 In `tests/Docker.Example.Tests.Integration/Docker.Example.Tests.Integration.csproj`, set `net10.0` and `<OutputType>Exe</OutputType>`. Replace `xunit` with `xunit.v3`, `Testcontainers` with `Testcontainers.MsSql`, and `FluentAssertions` with `AwesomeAssertions`. Remove the `NSubstitute` reference (design D10). Upgrade Mvc.Testing, Microsoft.NET.Test.Sdk, xunit.runner.visualstudio and coverlet.collector to the target versions (design D3, D5, D6). Verify: `dotnet restore` succeeds and `dotnet list package --outdated` shows nothing outdated
- [ ] 3.2 Rewrite `SomethingFactory` to use `MsSqlBuilder().WithImage("mcr.microsoft.com/mssql/server:2022-latest").Build()` without a fixed port binding. Implement xUnit v3 `IAsyncLifetime` (`ValueTask InitializeAsync`) and override `DisposeAsync` so it stops the container and then calls `base.DisposeAsync()`. Delete the `#pragma` (design D3, D5). Verify: the test project compiles with no CS0108 or CS0114 warnings
- [ ] 3.3 Point the app at the container by calling `builder.UseSetting("ConnectionStrings:DefaultConnection", _dbContainer.GetConnectionString())` in `ConfigureWebHost`, and delete the `DbContextOptions` descriptor removal and the hard-coded connection string. Use the D4 fallback if the override is not picked up (design D4). Verify: covered by the spec scenario "Application targets the container"; with no local SQL Server running, the test from 3.4 passes
- [ ] 3.4 In `Tests/SomeStuff.cs`, change `using Xunit.Abstractions` to the xUnit v3 `ITestOutputHelper`, and replace `Assert.True(true)` with AwesomeAssertions checks that the response is 200 and that `email`/`userName` equal `test.test@test.com` with a non-empty `id` (spec: user-lookup-api "Seeded user is returned"; integration-test-harness "Tests verify real behaviour"). Verify: `dotnet test` passes, and temporarily changing the expected email makes it fail
- [ ] 3.5 Add a `README.md` at the repo root covering the prerequisites (.NET 10 SDK, Docker) and how to run the app and the tests (`dotnet test`). Verify: following the README on a clean clone gives a passing test run

## 4. Integration checks

- [ ] 4.1 From a clean clone, run `dotnet build Docker.Example.sln -warnaserror` and `dotnet list package --vulnerable --include-transitive`. Verify: the build succeeds and no vulnerable packages are reported (spec: "Clean build")
- [ ] 4.2 Start an unrelated SQL Server container on host port 1433, then run `dotnet test`. Verify: the tests pass, and `docker ps -a` afterwards shows no leftover test container (spec: "Local SQL Server already running", "Container lifecycle")
- [ ] 4.3 Run `openspec validate upgrade-to-dotnet-10 --strict`. Verify: it reports the change as valid
