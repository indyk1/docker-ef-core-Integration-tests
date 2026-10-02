# integration-test-harness Specification

## Purpose
Runs the web application in-process against a real, throwaway SQL Server instance so its behaviour can be checked end to end without depending on a shared or locally installed database.

## Requirements

### Requirement: Supported runtime
The application and its integration tests SHALL build and run on .NET 10, using only package versions that are supported on that runtime.

#### Scenario: Clean build
- **WHEN** a contributor with the .NET 10 SDK runs `dotnet build` on the solution
- **THEN** the build succeeds with no errors and no warnings about obsolete APIs or vulnerable packages

### Requirement: Isolated database per test run
The test harness SHALL start a dedicated SQL Server container before any test runs and stop it after the tests finish.

#### Scenario: Container lifecycle
- **WHEN** the integration tests are run on a machine with Docker available
- **THEN** a SQL Server container is started before the first test
- **AND** the container is stopped and removed after the last test finishes

### Requirement: No fixed host port
The test harness SHALL NOT need a fixed host port for SQL Server. The application under test SHALL connect using the address and credentials reported by the container.

#### Scenario: Local SQL Server already running
- **WHEN** another process is already listening on host port 1433
- **THEN** the integration tests still start their container and pass

#### Scenario: Application targets the container
- **WHEN** the application under test opens a database connection during a test run
- **THEN** it connects to the test container, not to the server configured in `appsettings.json`

### Requirement: Tests verify real behaviour
Each integration test SHALL fail when the behaviour it covers is broken. Unconditional assertions are not allowed.

#### Scenario: Users endpoint regression
- **WHEN** `GET /api/Users` does not return the seeded user
- **THEN** the integration test covering that endpoint fails
