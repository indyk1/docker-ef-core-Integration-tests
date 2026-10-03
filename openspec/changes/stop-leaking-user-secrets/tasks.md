# Tasks

## 1. Safe response contract

- [ ] 1.1 Add `public sealed record UserResponse(string Id, string? UserName, string? Email);` in `src/Docker.Example/Models/UserResponse.cs` (design D1). Verify: `dotnet build src/Docker.Example -warnaserror` succeeds
- [ ] 1.2 Change `UsersController.Get()` to return `Task<UserResponse?>`, mapping the found user to `UserResponse` and returning `null` when no user is found (design D1). Verify: `dotnet build Docker.Example.sln -warnaserror` succeeds, and `curl /api/Users` against a locally running app returns only `id`, `userName` and `email`

## 2. Integration test coverage

- [ ] 2.1 Update `SomeStuff.SomethingElse` to parse the body as a `JsonDocument`, assert that its property names are exactly `id`, `userName` and `email`, and keep the existing value assertions (design D2; spec scenarios "Seeded user is returned" and "Only public fields are exposed"). Verify: `dotnet test` passes, and temporarily reverting the controller to return `IdentityUser` makes the test fail

## 3. Integration checks

- [ ] 3.1 Run `dotnet build Docker.Example.sln -warnaserror`, `dotnet test` and `openspec validate stop-leaking-user-secrets --strict` from a clean clone. Verify: all three succeed
