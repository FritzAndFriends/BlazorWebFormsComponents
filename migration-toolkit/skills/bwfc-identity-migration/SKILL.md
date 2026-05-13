---
name: bwfc-identity-migration
description: "**WORKFLOW SKILL** — Migrate ASP.NET Web Forms Identity and Membership authentication to Blazor Server Identity. Covers OWIN→Core middleware, login/register/logout minimal API endpoints, BWFC login controls, cookie auth under Interactive Server, and role-based authorization. WHEN: \"migrate identity\", \"login page migration\", \"OWIN to core\", \"cookie auth blazor\", \"LoginView migration\". INVOKES: dotnet CLI for identity scaffolding. FOR SINGLE OPERATIONS: use bwfc-migration for markup, bwfc-data-migration for EF."
---

# Web Forms Identity → Blazor Identity Migration

## Overview

| Web Forms Auth System | Era | Blazor Migration Path |
|----------------------|-----|----------------------|
| ASP.NET Identity (OWIN) | 2013+ | ASP.NET Core Identity (closest match) |
| ASP.NET Membership | 2005-2013 | Core Identity (schema migration required) |
| FormsAuthentication | 2002-2005 | Core Identity or cookie auth |

**Related:** `/bwfc-migration` (markup), `/bwfc-data-migration` (EF/architecture)

## Critical Rules

1. **Cookie auth requires HTTP endpoints** — login/register/logout MUST use `<form method="post">` + minimal API endpoints. Component event handlers (`@onclick`) silently fail for cookie operations in interactive mode.
2. **NEVER replace LoginView with AuthorizeView** — BWFC `LoginView` uses `AuthenticationStateProvider` natively with same template names (`AnonymousTemplate`, `LoggedInTemplate`).
3. **DisableAntiforgery required** — Blazor forms don't include antiforgery tokens. All auth endpoints must call `.DisableAntiforgery()`.
4. **Session-based auth data works** — `Session["key"]` via `WebFormsPageBase.Session` / `SessionShim`. Don't inject `IHttpContextAccessor`.

## Quick Reference

### BWFC Login Controls (drop-in replacements)

`<Login />`, `<LoginName />`, `<LoginStatus />`, `<LoginView>`, `<CreateUserWizard />`, `<ChangePassword />`, `<PasswordRecovery />`

### Auth Endpoint Pattern

```csharp
// Program.cs — login via HTTP POST (required for cookie auth)
app.MapPost("/Account/LoginHandler", async (HttpContext ctx, SignInManager<IdentityUser> sm) =>
{
    var form = await ctx.Request.ReadFormAsync();
    var result = await sm.PasswordSignInAsync(form["email"]!, form["password"]!, false, false);
    return result.Succeeded ? Results.Redirect("/") : Results.Redirect("/Account/Login?error=failed");
}).DisableAntiforgery();
```

## Companion Documents

| Document | Content |
|----------|---------|
| **[IDENTITY-PATTERNS.md](IDENTITY-PATTERNS.md)** | Cookie auth details, identity config steps, BWFC login controls, authorization patterns, endpoint templates, OWIN mapping, gotchas |

## L2 Break-Fix Playbook

| Error Signature | Recipe File |
|---|---|
| `CS0103` on OAuth fields (`providerDetails`, etc.) | `../bwfc-migration/recipes/oauth-page-stubs.md` |
| `CS0103` on fields in Account pages, no `.razor.cs` | `../bwfc-migration/recipes/missing-code-behind.md` |
