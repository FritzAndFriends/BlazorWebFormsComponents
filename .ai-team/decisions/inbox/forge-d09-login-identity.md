# Decision: Login Controls + Blazor Identity Integration Strategy (D-09)

**Date:** 2026-02-27
**By:** Forge
**Task:** D-09

## Context

The project has 7 login-related Web Forms controls implemented as Blazor components (Login, LoginName, LoginStatus, LoginView, ChangePassword, CreateUserWizard, PasswordRecovery). All are "visual shells" — they render correct Web Forms HTML structure but don't connect to ASP.NET Core Identity. This analysis determines how to bridge that gap.

## Key Findings

1. **LoginName and LoginView already work** — they correctly read `AuthenticationStateProvider` for display purposes. Both need a fix to re-render on auth state changes (currently read once in `OnInitializedAsync`).

2. **The HttpContext problem is the critical constraint.** `SignInManager` operations (sign-in, sign-out) require `HttpContext` for cookie manipulation, which is unavailable in Blazor Server interactive mode and Blazor WebAssembly. Direct `SignInManager` calls from components will fail. Redirect-based flows (navigate to a server endpoint) are the standard solution.

3. **UserManager operations work directly.** `ChangePasswordAsync()`, `CreateAsync()`, `GeneratePasswordResetTokenAsync()` do NOT require `HttpContext` and can be called from Blazor components in any hosting model.

4. **All 7 controls use an event-only pattern** where the developer handles all logic in callbacks. This works but provides no built-in functionality.

## Decisions

### 1. Create a separate `BlazorWebFormsComponents.Identity` NuGet package

The core package must NOT depend on `Microsoft.AspNetCore.Identity`. The Identity package provides pre-built handler implementations and server endpoints.

### 2. Use handler delegate pattern in core package

Each control gets optional `Func<>` handler delegate parameters (e.g., `AuthenticateHandler`, `ChangePasswordHandler`). When set, the component calls the handler instead of relying on event-only patterns. When not set, existing behavior is preserved. **Zero breaking changes.**

### 3. Priority order: LoginName → LoginView → LoginStatus → Login → ChangePassword → CreateUserWizard → PasswordRecovery

The first three are Small complexity (1-2 days each). Login and ChangePassword are Medium (3-5 days). CreateUserWizard and PasswordRecovery are Large (5-10 days).

### 4. Auth state re-render fix is cross-cutting

LoginName, LoginView, LoginStatus, and Login all need to subscribe to `AuthenticationStateChanged` or accept `[CascadingParameter] Task<AuthenticationState>`. This should be done as a single foundational change before individual control work.

### 5. Redirect-based flows for cookie operations

Login and LoginStatus should get `LoginActionUrl` and `LogoutActionUrl` parameters for redirect-based sign-in/sign-out. The Identity package provides the corresponding server endpoints.

## Impact

- **Core package:** Additive API surface only. No breaking changes. ~4-6 weeks.
- **Identity package:** New package. ~3-4 weeks after core changes.
- **Full analysis:** `planning-docs/LOGIN-IDENTITY-ANALYSIS.md`
