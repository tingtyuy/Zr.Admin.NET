# ZR.Admin.NET — Agent Guide

## Stack
- **Backend:** .NET 8 + ASP.NET Core Web API (minimal API entrypoint in `ZR.Admin.WebApi/Program.cs`)
- **ORM:** SqlSugar (not EF Core)
- **Mapping:** Mapster (not AutoMapper)
- **Auth:** JWT Bearer + custom `JwtAuthMiddleware`
- **Real-time:** SignalR (`MessageHub` at `/msgHub`)
- **Scheduler:** Quartz.NET v3
- **Logging:** NLog
- **Cache:** Redis via CSRedisCore (optional, config-driven)
- **Rate limit:** AspNetCoreRateLimit

## Project layout (12 projects)
| Project | Target | Role |
|---------|--------|------|
| `ZR.Model` | net8.0 | POCOs/DTOs — foundation layer, no project deps |
| `Infrastructure/` | net8.0 | Cross-cutting (JWT, Redis, ImageSharp, rate limiting) |
| `ZR.Repository` | net8.0 | Data access (SqlSugar.IOC) |
| `ZR.Common` | net8.0 | Common utilities (OSS, email, Excel, Serilog) |
| `ZR.ServiceCore` | net8.0 | Core system services |
| `ZR.Service` | net8.0 | Business services (Playwright) |
| `ZR.Tasks` | net8.0 | Quartz job definitions |
| `ZR.CodeGenerator` | net8.0 | Code-gen templates |
| `ZR.Mall` | net8.0 | Mall/e-commerce module |
| **`ZR.Admin.WebApi`** | net8.0 | **Main web host / entry point** |
| `ZR.ConsoleApp` | net10.0 | Console host (RulesEngine) |
| `ZR.WinFormsApp` | net10.0-windows | WinForms UI |
| `ZR.NUnit` | net10.0 | **Tests — NUnit v4** (tests ZR.Common only) |

Dependency flow: `Model → Infrastructure → Repository → Common → ServiceCore → Service → (Tasks|CodeGen|Mall) → WebApi`

## Commands
```powershell
# Run dev server (hot reload)
startup.bat
# equivalent: dotnet watch --project ZR.Admin.WebApi run

# Build
dotnet build --configuration Release

# Run all tests (NUnit v4)
dotnet test

# Package (creates .nupkg)
dotnet pack --configuration Release --output ./dist
```

## Frontend (legacy Vue 2 in `ZR.Vue/`)
```bash
cd ZR.Vue
npm install --registry=https://registry.npm.taobao.org
npm run dev                    # dev server → http://localhost:8887
npm run build:prod             # production build
```
Vue 3 version lives in a **separate repo** (ZR.Admin.Vue3). The `ZR.Vue/` directory is legacy.

## Conventions & gotchas
- **No Directory.Build.props** — each `.csproj` is self-contained
- **No NuGet.config / global.json** — uses default SDK + NuGet sources
- DB multi-tenancy is configured via SqlSugar.IOC
- `appsettings.Development.json` is gitignored — create it from `appsettings.json` template
- Docker: `ZR.Admin.WebApi` has Dockerfile support (Linux target)
- IDs use Snowflake algorithm
- CI triggers on `main` branch pushes + release creation; runs on `ubuntu-latest` with .NET 8.x

## Testing
- Framework: **NUnit v4** + NUnit3TestAdapter + coverlet
- Test project targets **net10.0** (can consume net8.0 libraries)
- Run single test: `dotnet test --filter "FullyQualifiedName~YourTestName"`
- Only `ZR.Common` is under test currently
