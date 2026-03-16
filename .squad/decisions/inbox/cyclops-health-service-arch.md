# Decision: ComponentHealthService Architecture

**By:** Cyclops (Component Dev)
**Date:** 2026-03-16
**Status:** Implemented

## Context

PRD §7 calls for a `ComponentHealthService` that lives inside the main BWFC library so it can perform runtime reflection over its own assembly. This is the core engine that the dashboard page (built later) will consume.

## Decisions Made

### 1. Service lives in `BlazorWebFormsComponents.Diagnostics` namespace
Placed in `src/BlazorWebFormsComponents/Diagnostics/` to keep diagnostic code separate from component code while remaining in the same assembly for reflection access.

### 2. Singleton registration via `AddComponentHealthDashboard(solutionRoot)`
The extension method takes a `solutionRoot` path because file detection (tests, docs, samples) needs filesystem access. The service eagerly loads baselines and discovers types at registration time, so subsequent `GetAllReports()` calls are fast.

### 3. Hardcoded fallback tracked components list
Since `dev-docs/tracked-components.json` doesn't exist yet, the service includes a hardcoded 56-component list matching PRD §3.3. When the JSON file is created, it will take priority automatically.

### 4. CountPropertiesAndEvents is internal static
Made the counting method `internal static` so it can be unit-tested directly without needing DI or filesystem access. Same for `StripGenericArity`.

### 5. Graceful degradation for missing baselines
Per §4.4, missing baselines produce null parity scores. The weighted average redistributes weights across available dimensions. This means the service works immediately even without `reference-baselines.json`.

### 6. ComponentCatalog detection via string search
Rather than taking a compile-time dependency on the sample app, the service reads `ComponentCatalog.cs` as text and searches for the component name in quotes. This keeps the library decoupled from the sample app.

## What's Next
- **Forge:** Curate `dev-docs/reference-baselines.json` with MSDN-verified counts
- **Forge:** Create `dev-docs/tracked-components.json` (optional — hardcoded list works)
- **Jubilee:** Build the dashboard Razor page in the sample app consuming this service
- **Rogue:** Write tests for the counting logic (internal static methods are testable)
