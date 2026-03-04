### 2026-03-04: User directive
**By:** Jeffrey T. Fritz (via Copilot)
**What:** Entity Framework Core migrations should always use the latest .NET 10 version of the package, currently 10.0.3. Use `Microsoft.EntityFrameworkCore` version 10.0.3 (and related packages like `.SqlServer`, `.Tools`, `.Design` at the same version).
**Why:** User request — captured for team memory. Ensures migrated projects use current stable EF Core matching the net10.0 TFM.
