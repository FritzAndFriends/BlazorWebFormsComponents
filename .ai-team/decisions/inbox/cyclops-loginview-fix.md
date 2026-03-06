# Decision: LoginView migration must preserve BWFC template names

**Date:** 2026-03-06
**Author:** Cyclops
**Status:** Implemented

## Context

The `ConvertFrom-LoginView` function in `bwfc-migrate.ps1` was converting `<asp:LoginView>` to `<AuthorizeView>` and renaming `AnonymousTemplate`/`LoggedInTemplate` to `NotAuthorized`/`Authorized`. This bypassed the BWFC `LoginView` component entirely.

## Decision

The migration script must:
1. Convert `<asp:LoginView>` → `<LoginView>` (BWFC component), NOT `<AuthorizeView>`
2. Leave `<AnonymousTemplate>` and `<LoggedInTemplate>` as-is — these are already the correct BWFC parameter names
3. Reference the BWFC `RoleGroup` component for `<RoleGroups>` manual items

## Rationale

The BWFC `LoginView` component (`src/BlazorWebFormsComponents/LoginControls/LoginView.razor.cs`) already has:
- `[Parameter] public RenderFragment AnonymousTemplate { get; set; }`
- `[Parameter] public RenderFragment LoggedInTemplate { get; set; }`
- Injects `AuthenticationStateProvider` and handles auth state internally
- Supports `RoleGroup` child components

Converting to `AuthorizeView` defeats the purpose of the BWFC library and forces developers to rewrite their template structure.

## Files Changed

- `migration-toolkit/scripts/bwfc-migrate.ps1` — `ConvertFrom-LoginView` function rewritten
- `samples/AfterWingtipToys/Components/Layout/MainLayout.razor` — uses `<LoginView>` with correct templates
- `samples/AfterWingtipToys/_Imports.razor` — added `@using BlazorWebFormsComponents.LoginControls`
