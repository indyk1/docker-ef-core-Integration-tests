# Spec Delta

## MODIFIED Requirements

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
