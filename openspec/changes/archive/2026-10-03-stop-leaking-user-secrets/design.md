# Design

## Context

`UsersController.Get()` returns `Task<IdentityUser?>` straight from `UserManager.FindByEmailAsync`, so System.Text.Json serialises every public property of the entity. When the user isn't found, the action returns `null`, which ASP.NET Core sends as 204 No Content. The integration test (`SomeStuff.SomethingElse`) deserialises the body into `IdentityUser`, which would hide any extra fields because unknown or missing properties are not errors.

## Goals / Non-Goals

**Goals:**
- Make it impossible for a new `IdentityUser` property to appear in this response by accident (an allow-list, not a block-list).
- Have the integration test check the exact set of properties, not just their values.

**Non-Goals:**
- Adding authentication or authorization to the endpoint.
- Changing the not-found behaviour (it stays 204), the route, or the placeholder endpoints in `UsersController`.
- Making the seeding idempotent or moving the seed passwords out of the code (separate concerns, not chosen for this change).

## Decisions

### D1: Return a dedicated response record
Add `public sealed record UserResponse(string Id, string? UserName, string? Email);` under `src/Docker.Example/Models/` and map the user to it in the controller. The action returns `Task<UserResponse?>`, so a `null` user still gives 204. A record is an allow-list: new Identity fields stay hidden unless someone deliberately adds them here.
*Alternatives:* `[JsonIgnore]` on a derived user type, or a custom `JsonConverter` for `IdentityUser`. Both are block-lists, so a new sensitive field would leak by default, and both are more code. Returning an anonymous object works, but the endpoint then has no named contract and OpenAPI tooling can't describe it.

### D2: Assert the property names in the test, not just the values
Read the body as a `JsonDocument` and check that its property names are exactly `{ "id", "userName", "email" }`, then check the values. Deserialising into `UserResponse` alone would pass even if extra fields came back, which is the regression this change guards against.

### D3: Keep camelCase property names
Use the default `System.Text.Json` web settings (camelCase), so `id`, `userName` and `email` keep the exact names the existing spec scenario uses. No serializer configuration changes.

## Risks / Trade-offs

- [An external client relies on the removed fields] → Accepted. The only consumer in the repo is the integration test, and exposing a password hash is not behaviour worth keeping. The change is marked **BREAKING** in the proposal.
- [Duplicate mapping if more user endpoints are added later] → Accepted for now. With only one call site, an extension method or a mapper library would be premature.
