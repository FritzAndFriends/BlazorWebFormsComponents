# Decision: Remove @rendermode InteractiveServer from CrudOperations.razor

**Author:** Jubilee (Sample Writer)
**Date:** 2025-07-15
**Status:** Applied

## Context

PR #343 introduced `CrudOperations.razor` in `samples/AfterBlazorServerSide/Components/Pages/ControlSamples/ListView/` with `@rendermode InteractiveServer` on line 2. The `AfterBlazorClientSide` project includes all server-side sample pages via wildcard in its csproj, so this directive caused a build failure — `InteractiveServer` is not available in the WebAssembly SDK.

## Decision

Removed the `@rendermode InteractiveServer` directive. No other sample page in the `ControlSamples` directory uses this directive; they all work without it. This is the minimal change that restores consistency and fixes the CI build.

## Verification

- `dotnet build samples/AfterBlazorClientSide/ --configuration Release` — ✅ passes
- `dotnet build samples/AfterBlazorServerSide/ --configuration Release` — ✅ passes
- `dotnet test src/BlazorWebFormsComponents.Test/ --no-restore` — ✅ passes
