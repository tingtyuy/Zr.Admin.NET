# AGENTS.md

## Project overview

Zr.Admin.NET is a .NET 8 RBAC admin framework (backend) with a Vue 2 + Element UI frontend. Layered architecture: WebApi → Service/ServiceCore → Repository → Model. Uses SqlSugar ORM, JWT auth, SignalR, and Quartz.NET task scheduling.

## Quick commands

**Start backend (port 8888):**
```bash
dotnet watch --project ZR.Admin.WebApi run
```

**Start frontend (port 8887):**
```bash
cd ZR.Vue && npm run dev
```
Note: Frontend requires `SET NODE_OPTIONS=--openssl-legacy-provider` (set in package.json dev script automatically).

**Build entire solution:**
```bash
dotnet build ZRAdmin.sln
```

**One-key start (Windows):** `start.bat` — starts both backend and frontend.

## Architecture

**Layer structure:**
- `ZR.Admin.WebApi/` — Entry point. Controllers, middleware, extensions. References Service, Tasks, CodeGenerator, Mall.
- `ZR.ServiceCore/` — System services (auth, users, roles, menus, files, AI tasks, etc.)
- `ZR.Service/` — Business services (user's custom modules)
- `ZR.Repository/` — Repository layer (stored procedures, data access)
- `ZR.Model/` — Entities, DTOs, enums. Multi-DB via `[Tenant("0")]` (main) and `[Tenant("1")]` (mall).
- `ZR.Common/` — Utilities, caching, dynamic API registration
- `Infrastructure/` — Cross-cutting: attributes, controllers, extensions, middleware
- `ZR.CodeGenerator/` — Code generation templates and logic
- `ZR.Mall/` — Separate mall module (order, brand, category, etc.)
- `ZR.Vue/` — Vue 2 frontend (dev server on port 8887)
- `ZR.Tasks/` — Quartz.NET scheduled tasks

**Key conventions:**
- Service registration: `[AppService(ServiceType = typeof(IxxxService), ServiceLifetime = LifeTime.Transient)]` — auto-registered via `AddDynamicApi()`.
- Database: SQLite by default (`ZrAdmin.db`, `ZrAdmin_Mall.db`). `InitDb=true` in appsettings.json auto-creates tables on startup (CodeFirst).
- SnowFlake IDs: Add `[JsonConverter(typeof(ValueToStringConverter))]` on `Id` properties for JS compatibility (C# `long` exceeds `Number.MAX_SAFE_INTEGER`).
- `[AllowAnonymous]` bypasses JWT auth on controller actions.
- `HttpContext.GetUId()` returns user ID (long); `HttpContext.GetName()` returns username string.
- `AppSettings.GetConfig(key)` returns string — parse manually.
- Controllers organized into subdirectories: `Controllers/Ai/`, `Controllers/System/`, `Controllers/Email/`, etc.

## Frontend routing (ZR.Vue)

Two-layer routing: `constantRoutes` (public, always in sidebar) + dynamic routes from backend `sys_menu` table via `getRouters()` → `filterAsyncRouter()` → `loadView()`.

Backend `component` field in `sys_menu`: `"Layout"` → Layout component, `"ParentView"` → ParentView, else → lazy-loads `@/views/${view}`.

Parent routes in `constantRoutes` MUST have `meta.title` for sidebar submenu display. `hidden: true` hides from sidebar while keeping routable.

## Config files

- `ZR.Admin.WebApi/appsettings.json` — Main config (DB, JWT, Redis, uploads, mail)
- `ZR.Admin.WebApi/codeGen.json` — Code generation config
- `ZR.Admin.WebApi/iprate.json` — IP rate limiting config
- `ZR.Vue/.env.development` / `.env.production` — Frontend env config

## Testing

No test framework configured in the solution. The `ZR.NUnit` directory exists but contains no csproj (it's listed in the .gitignore).

## Common gotchas

- `.bat` scripts must use English-only text — Chinese chars cause garbled output on Windows.
- `Upload.uploadUrl` in appsettings.json must use the externally-accessible IP/hostname (not localhost) for file URLs to work.
- SqlSugar `Queryable().Delete()` doesn't work — use `Context.Deleteable<T>().Where(...).ExecuteCommand()`.
- `UseTran(Func<T>)` supports return values but lambda must not be async.
- Batch update: `Update(List<T>)` doesn't exist on BaseService — use `Context.Updateable(list).ExecuteCommand()`.
- `login.vue` hardcodes `captchaOnOff: true` — backend SQLite config alone won't disable captcha.
