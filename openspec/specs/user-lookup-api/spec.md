# user-lookup-api Specification

## Purpose
Exposes the application's seeded Identity user over HTTP so that callers, and the integration tests, can confirm the app is running end to end against its database.

## Requirements

### Requirement: Seed a known user on startup
The application SHALL apply all pending database migrations at startup and then create a user with user name and email `test.test@test.com`.

#### Scenario: Fresh database
- **WHEN** the application starts against an empty SQL Server database
- **THEN** the Identity schema is created by migrations
- **AND** a user with email `test.test@test.com` exists in the users table

#### Scenario: No pending model changes
- **WHEN** the application applies migrations at startup on the supported runtime
- **THEN** startup completes without a pending-model-changes error

### Requirement: Return the seeded user
`GET /api/Users` SHALL respond with HTTP 200 and a JSON body describing the seeded user that contains exactly the properties `id`, `userName` and `email`, and no others. In particular, the body MUST NOT expose credential or security data such as password hashes or security stamps.

#### Scenario: Seeded user is returned
- **WHEN** a client sends `GET /api/Users` after startup has completed
- **THEN** the response status is 200
- **AND** the JSON body's `email` and `userName` are both `test.test@test.com`
- **AND** the JSON body's `id` is a non-empty string

#### Scenario: Only public fields are exposed
- **WHEN** a client sends `GET /api/Users` after startup has completed
- **THEN** the JSON body's property names are exactly `id`, `userName` and `email`
- **AND** the body contains no `passwordHash`, `securityStamp` or `concurrencyStamp` property
