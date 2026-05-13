---
name: bwfc-data-migration
description: "**WORKFLOW SKILL** — Migrate Web Forms data access and architecture to Blazor Server. Covers EF6→EF Core with IDbContextFactory, Session→SessionShim, Global.asax→Program.cs, Web.config→appsettings.json, DataSource controls→service injection. WHEN: \"migrate EF6\", \"session state to services\", \"Global.asax to Program.cs\", \"data access migration\", \"SelectMethod to delegate\". INVOKES: dotnet CLI for EF migrations. FOR SINGLE OPERATIONS: use bwfc-migration for markup, bwfc-identity-migration for auth."
---

# Web Forms Data Access & Architecture Migration

## Overview

Covers data access and architecture migration — the **Layer 2/3 decisions** requiring project-specific judgment.

**Related:** `/bwfc-migration` (markup), `/bwfc-identity-migration` (auth)

## When to Use This Skill

- Convert `SelectMethod` string → `SelectHandler` delegate, replace `DataSource` controls with service injection
- Migrate Entity Framework 6 → EF Core
- Convert `Session`/`ViewState`/`Application` state to Blazor patterns
- Migrate `Global.asax` → `Program.cs`, `Web.config` → `appsettings.json`
- Replace HTTP Handlers/Modules with middleware

## Critical Rules

1. **Use SessionShim** — `Session["key"]` works AS-IS via `WebFormsPageBase`. No `IHttpContextAccessor`, no cookies, no custom services.
2. **Use IDbContextFactory** — never `AddDbContext` for Blazor Server. Circuits are long-lived.
3. **Preserve database provider** — if Web Forms used SQL Server, use `UseSqlServer()`. NEVER default to SQLite.
4. **Materialize queries** — `SelectHandler` delegates must `.ToList()` inside the `using` block.

## Anti-Patterns (DO NOT)

| ❌ Wrong | ✅ Correct |
|----------|-----------|
| `app.MapPost("/api/cart/add", ...)` for page actions | Call service directly in component |
| `[Inject] IHttpContextAccessor` for session | `Session["key"]` via WebFormsPageBase |
| `Response.Cookies.Append()` for state | `Session["key"]` via SessionShim |
| `HttpContext.Current.Session["key"]` | `Session["key"]` (property on WebFormsPageBase) |
| `new XxxContext()` | `IDbContextFactory<XxxContext>` via DI |

## Companion Documents

| Document | Content |
|----------|---------|
| **[DATA-PATTERNS.md](DATA-PATTERNS.md)** | SessionShim details, EF6→EF Core, DataSource→services, state migration, gotchas |
| **[ARCHITECTURE-TRANSFORMS.md](ARCHITECTURE-TRANSFORMS.md)** | Global.asax→Program.cs, Web.config→appsettings, routes, HTTP handlers, middleware |

## L2 Break-Fix Playbook

| Error Signature | Recipe File |
|---|---|
| `CS1503: SelectMethod ... 'string' to 'SelectHandler'` | `../bwfc-migration/recipes/selectmethod-string-binding.md` |
| `CS7036: no argument ... 'options' of 'XxxContext'` | `../bwfc-migration/recipes/new-dbcontext-to-di.md` |
| `CS0246: 'IDatabaseInitializer'` | `../bwfc-migration/recipes/database-seed-initializer.md` |
