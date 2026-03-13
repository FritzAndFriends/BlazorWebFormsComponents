---
name: contoso-migration-test
description: "**WORKFLOW SKILL** — Execute end-to-end ContosoUniversity migration benchmark: clear output folder, run L1 script + L2 Copilot transforms, build, run Playwright acceptance tests, and generate a numbered run report. WHEN: \"run contoso migration test\", \"test contoso university migration\", \"contoso migration benchmark\", \"run CU migration\". INVOKES: bwfc-migrate.ps1, bwfc-migration skill, bwfc-data-migration skill, bwfc-identity-migration skill, migration-standards skill, dotnet CLI, Playwright tests."
---

# ContosoUniversity Migration Test

End-to-end migration benchmark that converts the ContosoUniversity Web Forms sample to Blazor Server and validates with 40 Playwright acceptance tests.

## Paths

| Item | Path |
|------|------|
| Web Forms source | `samples/ContosoUniversity/ContosoUniversity/` |
| Blazor output | `samples/AfterContosoUniversity/` |
| L1 script | `migration-toolkit/scripts/bwfc-migrate.ps1` |
| Migration skills | `migration-toolkit/skills/` (4 skills) |
| Acceptance tests | `src/ContosoUniversity.AcceptanceTests/` |
| Run reports | `dev-docs/migration-tests/contosouniversity/` |

## Prerequisites

- .NET 10 SDK
- SQL Server LocalDB with `ContosoUniversity` database (attach `samples/ContosoUniversity/ContosoUniversity.mdf` if needed)
- Playwright browsers installed (run `pwsh bin/Debug/net10.0/playwright.ps1 install` from test project after first build)

## Workflow

### Phase 0: Preparation

1. **Determine run number** — Count existing `runNN` folders in `dev-docs/migration-tests/contosouniversity/` and use the next sequential number
2. **Record start time** — Capture wall-clock time for overall timing
3. **Clear output folder** — Delete all contents of `samples/AfterContosoUniversity/` but keep the directory itself

### Phase 1: Layer 1 — Automated Script

**Expected duration:** < 2 seconds

1. Start L1 timer
2. Run the migration script:
   ```powershell
   .\migration-toolkit\scripts\bwfc-migrate.ps1 `
     -Path samples\ContosoUniversity\ContosoUniversity `
     -Output samples\AfterContosoUniversity `
     -Verbose
   ```
3. Stop L1 timer — record duration and transform count from script output
4. Verify: `.razor` files created, no `.aspx` files in output

### Phase 2: Layer 2 — Copilot-Assisted Transforms

**Expected duration:** 20–30 minutes

Load and apply all four migration skills from `migration-toolkit/skills/`:

| Skill | Responsibility |
|-------|---------------|
| `migration-standards` | Target architecture (.NET 10, Server Interactive), page base class, render mode |
| `bwfc-migration` | Control translation, data binding expressions, Master→Layout, code-behind lifecycle |
| `bwfc-data-migration` | EF6→EF Core, Session→Scoped services, Global.asax→Program.cs, Web.config→appsettings |
| `bwfc-identity-migration` | Identity/auth migration (if applicable to source) |

**Key transforms to execute:**
- Convert code-behind lifecycle (`Page_Load` → `OnInitializedAsync`)
- Wire `SelectMethod` as `SelectHandler<ItemType>` delegates (**NOT** `Items=`)
- Create EF Core `DbContext` with SQL Server LocalDB connection
- Create `Program.cs` with DI registration for all BLL services
- Convert `Site.Master` → `MainLayout.razor`
- Migrate BLL classes to use EF Core + DI (`IDbContextFactory`)
- Wire all CRUD operations (Create, Read, Update, Delete) to UI

### Phase 3: Build Validation

1. Run `dotnet build samples\AfterContosoUniversity\`
2. Fix compilation errors iteratively until 0 errors achieved
3. Record final error and warning counts

### Phase 4: Acceptance Tests

1. Start the Blazor app in the background:
   ```powershell
   $env:ASPNETCORE_URLS = "http://localhost:44380"
   dotnet run --project samples\AfterContosoUniversity\
   ```
2. Wait for HTTP 200 from `http://localhost:44380`
3. Run acceptance tests:
   ```powershell
   $env:CONTOSO_BASE_URL = "http://localhost:44380"
   dotnet test src\ContosoUniversity.AcceptanceTests\ --verbosity normal
   ```
4. Record: total tests, passed, failed, skipped
5. Stop the Blazor app

### Phase 5: Report Generation

1. Create `dev-docs/migration-tests/contosouniversity/runNN/`
2. Generate `REPORT.md` using [REPORT-TEMPLATE.md](REPORT-TEMPLATE.md)
3. All sections required — even failed runs get a full report

## Critical Rules

| Rule | Detail |
|------|--------|
| **Database** | SQL Server LocalDB — **never SQLite** |
| **Connection** | `Server=(localdb)\mssqllocaldb;Database=ContosoUniversity` |
| **SelectMethod** | Preserve as `SelectHandler<ItemType>` delegate — never convert to `Items=` |
| **L1→L2 handoff** | No manual fixes between layers — L2 starts from raw L1 output |
| **Report every run** | Even failed/partial runs get a report documenting what went wrong |
| **ItemType** | Use `ItemType` (not `TItem`) for all data-bound component type parameters |

## Reference Documents

- [REPORT-TEMPLATE.md](REPORT-TEMPLATE.md) — Standard report format with all required sections
