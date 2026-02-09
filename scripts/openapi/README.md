# OpenAPI Client Generation

Use NSwag (local dotnet tool) to generate typed C# clients + DTOs from the Playground API spec.

## Prereqs
- .NET SDK (repo already uses net10.0)
- Local tool manifest at `.config/dotnet-tools.json` (created) with `nswag.consolecore`

## Usage

### Windows (PowerShell)
```powershell
./scripts/openapi/generate-api-clients.ps1 -SpecUrl "https://localhost:7030/openapi/v1.json"
```

### macOS/Linux (Bash)
```bash
./scripts/openapi/generate-api-clients.sh "https://localhost:7030/openapi/v1.json"
```

### Alternative (PowerShell Core - cross-platform)
If you have PowerShell Core installed on macOS/Linux:
```bash
pwsh ./scripts/openapi/generate-api-clients.ps1 -SpecUrl "https://localhost:7030/openapi/v1.json"
```

This restores the local tool, ensures the output directory exists, and runs NSwag with the spec URL you provide.

## Output
- Clients + DTOs: `src/Playground/Playground.Blazor/ApiClient/Generated.cs` (single file; multiple client types grouped by first path segment after the base path, e.g., `/api/v1/identity/*` -> `IdentityClient`).
- Namespace: `FSH.Playground.Blazor.ApiClient`
- Client grouping: `MultipleClientsFromPathSegments`; ensure Minimal API routes keep module-specific first segments.
- Bearer auth: configure `HttpClient` (via DI) with the bearer token; generated clients use injected `HttpClient`. Base URLs are not baked into the generated code (`useBaseUrl: false`), so `HttpClient.BaseAddress` must be set by the app (see `Program.cs`).

## Drift Check (manual)

### Windows (PowerShell)
```powershell
./scripts/openapi/check-openapi-drift.ps1 -SpecUrl "https://localhost:7030/openapi/v1.json"
```

### macOS/Linux (Bash)
```bash
./scripts/openapi/check-openapi-drift.sh "https://localhost:7030/openapi/v1.json"
```

This regenerates the clients and fails if `ApiClient/Generated.cs` changes. Useful in PRs to ensure the spec and generated clients stay in sync even before CI enforcement.

> Note: The spec endpoint must be reachable when running the generation scripts. If the API is not running locally, point `-SpecUrl` to an accessible environment or start the Playground API first.

## Tips
- If the API changes, rerun the script with the updated spec URL (e.g., staging/prod).
- Commit regenerated clients alongside related API changes to keep UI consumers in sync.
