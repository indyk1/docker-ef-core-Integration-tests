# Proposal

## Why

`GET /api/Users` serialises the whole ASP.NET Core Identity `IdentityUser` entity, so the response includes `passwordHash`, `securityStamp`, `concurrencyStamp` and the lockout, two-factor and phone fields. The endpoint has no authentication, so anyone who can reach the app can read the seeded user's password hash, which can be cracked offline, and the security stamp. The current `user-lookup-api` spec only says which fields must be present. It doesn't limit what else may appear, so nothing guards against this.

## What Changes

- **BREAKING**: `GET /api/Users` returns only `id`, `userName` and `email`. All other `IdentityUser` fields are removed from the response, including `passwordHash`, `securityStamp`, `concurrencyStamp`, `normalizedUserName`, `normalizedEmail`, `emailConfirmed`, `phoneNumber`, `phoneNumberConfirmed`, `twoFactorEnabled`, `lockoutEnd`, `lockoutEnabled` and `accessFailedCount`. In this repo the only caller is the integration test.
- The integration test reads the response into the new response shape and fails if any field other than the three allowed ones appears.
- Unchanged: the route, the HTTP method, the 200 status when the user exists, and the current empty response (204 No Content) when the user doesn't exist.

## Capabilities

### New Capabilities

<!-- None -->

### Modified Capabilities

- `user-lookup-api`: The "Return the seeded user" requirement changes from listing the fields that must be present to listing the only fields allowed, and gains a scenario that checks credential fields are absent.

## Impact

- **Code**: `src/Docker.Example/Controllers/UsersController.cs` (return type), a new response record in the web project, and `tests/Docker.Example.Tests.Integration/Tests/SomeStuff.cs`.
- **API**: The `GET /api/Users` JSON body becomes smaller. Any client reading the removed fields would break, but no such client exists in this repo.
- **Dependencies, data, configuration**: No changes.
