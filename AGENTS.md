# Repository Guidelines

## Project Structure & Module Organization

This is a .NET 10 Aspire solution (`AspireShop.slnx`) for a distributed shop application:

- `AspireShop.AppHost/` defines the local Aspire topology, including Redis, PostgreSQL, migrations, the catalog API, and web frontend.
- `AspireShop.Api.Catalog/` contains the catalog API, product contracts/entities, EF Core persistence, and migrations.
- `AspireShop.Web/` contains the Blazor frontend, Razor components, static assets, and API clients.
- `AspireShop.ServiceDefaults/` contains shared hosting, health, resilience, and OpenTelemetry defaults.
- `AspireShop.Tests/` contains xUnit v3 Aspire integration tests.

Keep generated `bin/` and `obj/` output out of changes. Add new API features under the existing `Api`, `Domain`, and `Infrastructure` boundaries.

## Build, Test, and Development Commands

Run from the repository root:

```powershell
dotnet restore AspireShop.slnx
dotnet build AspireShop.slnx
dotnet test AspireShop.Tests/AspireShop.Tests.csproj
dotnet run --project AspireShop.AppHost/AspireShop.AppHost.csproj
```

Building the Web project incrementally installs frontend dependencies when `package.json` or `package-lock.json` changes, then regenerates `wwwroot/css/tailwind.css` when Tailwind inputs change; Node.js must be available on `PATH`. Use `npm --prefix AspireShop.Web run watch:css` during styling work. `dotnet run` starts the Aspire dashboard and dependent services; use the dashboard endpoints for local verification. The test project launches the distributed application and checks the `webfrontend` resource.

## Coding Style & Naming Conventions

Use four-space indentation, nullable reference types, implicit usings, and file-scoped namespaces where appropriate. Follow standard .NET naming: PascalCase for types, methods, properties, and Razor components; camelCase for locals and parameters; descriptive kebab-case Aspire resource names (for example, `catalog-api`). Keep async methods suffixed with `Async`. Run the formatter before submitting changes:

```powershell
dotnet format AspireShop.slnx
```

## Testing Guidelines

Use xUnit `[Fact]` tests with behavior-focused names such as `GetWebResourceRootReturnsOkStatusCode`. Prefer Aspire integration tests for service wiring, health, and HTTP behavior; keep unit tests focused on isolated domain or mapping logic. Run the full test project before opening a pull request.

## Commit & Pull Request Guidelines

No usable Git history is present in this checkout, so use concise imperative commit subjects (for example, `Add product catalog endpoint`). Pull requests should explain the user-visible or architectural change, list validation commands and results, link any issue, and include screenshots for frontend changes. Call out configuration, migration, or service-topology changes explicitly.

## Security & Configuration Tips

Do not commit secrets or local credentials. Use .NET user secrets or environment variables for development overrides. Review `appsettings*.json` and EF migrations carefully when changing database or service configuration.
