---
name: wingtip-migration-test
description: "**WORKFLOW SKILL** - Execute the end-to-end WingtipToys migration benchmark: clear samples\\AfterWingtipToys, run the migration-toolkit against samples\\WingtipToys, repair the generated app until Playwright acceptance tests pass, and write a numbered run report with embedded screenshots under dev-docs\\migration-tests\\wingtiptoys. WHEN: \"run Wingtip migration\", \"test WingtipToys migration\", \"Wingtip benchmark\", \"migrate WingtipToys\", \"rerun Wingtip migration\". INVOKES: migration-toolkit\\scripts\\bwfc-migrate.ps1, migration-toolkit\\skills\\migration-standards, bwfc-migration, bwfc-data-migration, bwfc-identity-migration, dotnet CLI, Playwright tests."
---

# WingtipToys Migration Test

End-to-end migration benchmark for the canonical WingtipToys Web Forms sample. This workflow uses the repository's migration toolkit as the public entry point, preserves the migrated application shape in `samples\AfterWingtipToys\`, and considers the run successful only when the existing Playwright acceptance tests pass.

## Benchmark Integrity Rules

This workflow is a **benchmark**, so every run must start from scratch.

### Required behavior

1. Start with the raw Web Forms source in `samples\WingtipToys\`.
2. Clear `samples\AfterWingtipToys\` before each run.
3. Run `migration-toolkit\scripts\bwfc-migrate.ps1` to produce the output for **this run**.
4. Repair **only** the fresh output produced during the current run.

### Forbidden behavior

1. **Do not restore or copy previously migrated content** into `samples\AfterWingtipToys\`.
2. **Do not use git history as migration input or repair content**:
   - no `git restore`
   - no `git checkout`
   - no `git show` to pull old file contents into the run
   - no copying files from prior commits, branches, tags, or stashes
3. **Do not reuse prior benchmark outputs** from:
   - `samples\AfterWingtipToys\`
   - `dev-docs\migration-tests\wingtiptoys\run*`
   - session artifacts, temp folders, or prior migration snapshots
4. **Do not treat an earlier repaired sample as the answer.** The point of the run is to measure what the toolkit plus current repair work can achieve from scratch.

If a run uses prior migrated content or git-sourced repairs, the benchmark is invalid and must be restarted from a freshly cleared output folder.

## Paths

| Item | Path |
|------|------|
| Web Forms wrapper | `samples/WingtipToys/` |
| Effective Web Forms app | `samples/WingtipToys/WingtipToys/` |
| Blazor output | `samples/AfterWingtipToys/` |
| Toolkit entry point | `migration-toolkit/scripts/bwfc-migrate.ps1` |
| Toolkit skills | `migration-toolkit/skills/` |
| Acceptance tests | `src/WingtipToys.AcceptanceTests/` |
| Run reports | `dev-docs/migration-tests/wingtiptoys/` |
| Report template | `./REPORT-TEMPLATE.md` |

## Success Criteria

A Wingtip run is only a success when **all** of the following are true:

1. `samples\AfterWingtipToys\` was cleared before the run.
2. The migration was started through `migration-toolkit\scripts\bwfc-migrate.ps1`.
3. The generated app was repaired **in place** until it builds and runs.
4. `dotnet test src\WingtipToys.AcceptanceTests\` passes against the migrated app.
5. A new numbered report folder was written under `dev-docs\migration-tests\wingtiptoys\runNN\`.
6. The report includes total runtime, what worked well, what did not work well, and embedded screenshots proving the app is working.

## Prerequisites

- .NET 10 SDK
- Playwright browsers installed for `src\WingtipToys.AcceptanceTests\`
- Local HTTPS dev certificate trusted if the run uses the default `https://localhost:5001`
- Any seed data or local DB setup required by the current `samples\AfterWingtipToys` implementation

If Playwright browsers have not been installed yet for this machine:

```powershell
dotnet build src\WingtipToys.AcceptanceTests\WingtipToys.AcceptanceTests.csproj
pwsh src\WingtipToys.AcceptanceTests\bin\Debug\net10.0\playwright.ps1 install
```

## Workflow

### Phase 0: Preparation

1. **Determine the next run number**  
   Scan `dev-docs\migration-tests\wingtiptoys\run*` folders and use the next numeric value after the current maximum. Preserve zero padding: `run26`, `run27`, etc.

2. **Initialize the timing ledger**  
   Create a SQL table and record the run start time **before** any other work:

   ```sql
   CREATE TABLE IF NOT EXISTS run_timing (
     phase TEXT PRIMARY KEY,
     started_at TEXT,
     finished_at TEXT,
     notes TEXT
   );
   DELETE FROM run_timing;   -- clean slate for this run
   INSERT INTO run_timing (phase, started_at) VALUES
     ('total',        strftime('%Y-%m-%dT%H:%M:%S','now','localtime'));
   ```

   At the **start** of each subsequent phase, insert its row:
   ```sql
   INSERT INTO run_timing (phase, started_at) VALUES
     ('phase_name', strftime('%Y-%m-%dT%H:%M:%S','now','localtime'));
   ```

   At the **end** of each phase, update its row:
   ```sql
   UPDATE run_timing SET finished_at = strftime('%Y-%m-%dT%H:%M:%S','now','localtime')
     WHERE phase = 'phase_name';
   ```

   Phase names to track (must match these keys exactly):

   | Phase key | Corresponds to |
   |-----------|---------------|
   | `total` | Entire run (Phase 0 start → Phase 6 end) |
   | `preparation` | Phase 0: folder cleanup, report folder creation |
   | `l1_migration` | Phase 1: `bwfc-migrate.ps1` execution |
   | `build_repair` | Phase 2: compile-error fixes |
   | `startup_triage` | Phase 3: DI/config/DB root-cause fixes |
   | `acceptance_tests` | Phase 4: Playwright test runs + targeted repairs |
   | `screenshots` | Phase 5: screenshot capture |
   | `report` | Phase 6: report generation |

   > **Why SQL?** Timestamps persist across tool calls and can be queried at report time to auto-populate the timing table. No manual bookkeeping required.

3. **Clear the output folder contents**  
   Delete everything under `samples\AfterWingtipToys\` while keeping the folder itself.

4. **Create the report folder early**  
   Create `dev-docs\migration-tests\wingtiptoys\runNN\` and an `images\` subfolder so logs and screenshots have a known destination from the start.

5. **Mark Phase 0 complete**
   ```sql
   INSERT INTO run_timing (phase, started_at, finished_at) VALUES
     ('preparation',
      (SELECT started_at FROM run_timing WHERE phase = 'total'),
      strftime('%Y-%m-%dT%H:%M:%S','now','localtime'));
   ```

### Phase 1: Layer 1 - Migration Toolkit Run

**Start timing:**
```sql
INSERT INTO run_timing (phase, started_at) VALUES
  ('l1_migration', strftime('%Y-%m-%dT%H:%M:%S','now','localtime'));
```

Run the toolkit wrapper, not the CLI directly:

```powershell
pwsh -File migration-toolkit\scripts\bwfc-migrate.ps1 `
  -Path samples\WingtipToys `
  -Output samples\AfterWingtipToys `
  -Verbose
```

Record:

- Layer 1 duration
- Any CLI/toolkit summary output
- Whether the toolkit resolved the nested `samples\WingtipToys\WingtipToys\` app root automatically
- Whether `.razor` output, scaffold files, and static assets were produced in the expected places

**End timing:**
```sql
UPDATE run_timing SET finished_at = strftime('%Y-%m-%dT%H:%M:%S','now','localtime')
  WHERE phase = 'l1_migration';
```

### Phase 2: Build Repair (Compile Errors Only)

**Start timing:**
```sql
INSERT INTO run_timing (phase, started_at) VALUES
  ('build_repair', strftime('%Y-%m-%dT%H:%M:%S','now','localtime'));
```

Load the migration toolkit skills from `migration-toolkit\skills\` for reference during repair:

| Skill | Responsibility |
|-------|---------------|
| `migration-standards` | Canonical migration rules, page base class, render mode, SelectMethod, shims |
| `bwfc-migration` | Markup conversion, template cleanup, lifecycle conversion, master/layout migration |
| `bwfc-data-migration` | EF/data-layer modernization, service registration, data access fixes |
| `bwfc-identity-migration` | Authentication and account-page migration |

Repair the generated app **in place**. Do not replace it with a simplified rewrite.

**Benchmark constraints:**
- Every repair must be derived from the **current run's freshly generated output**, the raw Web Forms source, BWFC/toolkit rules, and normal debugging/build feedback.
- Do **not** import repaired files from previous runs, from git history, or from other saved artifacts.
- **NEVER replace generated BWFC data controls (`ListView`, `FormView`, `GridView`, `DataList`, `Repeater`) with manual HTML.**

Fix **only compile errors** in this phase. Do not rewrite page logic, add features, or restructure files — only make the project build.

```powershell
dotnet build samples\AfterWingtipToys\WingtipToys.csproj
```

**Repair rules:**
- Fix one error category at a time (missing usings, missing types, syntax errors)
- Add minimal stubs for missing types (empty class body, not full implementations)
- Do NOT rewrite code-behind files that already exist — the CLI generated them for a reason
- Do NOT replace BWFC data controls with manual HTML
- Rebuild after each batch of fixes to verify progress

Record:
- Initial error count
- Error categories
- Number of rebuild iterations
- Final build status

**End timing:**
```sql
UPDATE run_timing SET finished_at = strftime('%Y-%m-%dT%H:%M:%S','now','localtime')
  WHERE phase = 'build_repair';
```

### Phase 3: Startup Triage (Root Cause Before Symptoms)

**Start timing:**
```sql
INSERT INTO run_timing (phase, started_at) VALUES
  ('startup_triage', strftime('%Y-%m-%dT%H:%M:%S','now','localtime'));
```

> **CRITICAL: This phase prevents wasted L2 repair work.**
> Most page-level failures are caused by 1-2 startup issues (missing DB tables, bad connection string, missing DI registration). Fixing the root cause often makes 10+ "broken" pages work instantly. **Never skip this phase.**

#### Step 1: Start the app

```powershell
dotnet run --project samples\AfterWingtipToys\WingtipToys.csproj
```

Watch the console output. If the app crashes at startup, the error message tells you what to fix.

#### Step 2: Smoke test the home page

```powershell
curl -k -s -o NUL -w "%{http_code}" https://localhost:5001/
```

| Result | Action |
|--------|--------|
| **200** | App is running. Proceed to Phase 4. |
| **500** | Read console logs. Fix the root cause (DB init, config, DI). Do NOT touch page files yet. |
| **Connection refused** | App crashed at startup. Read console output. Fix and restart. |

#### Step 3: If 500 — diagnose before rewriting

Common root causes and their fixes:

| Console Error Pattern | Root Cause | Fix |
|----------------------|------------|-----|
| `Invalid object name 'TableName'` | Tables don't exist | Add `Database.EnsureCreated()` in Program.cs |
| `Cannot attach the file` / `AttachDbFilename` | Bad connection string | Change to `Initial Catalog=DbName` |
| `Unable to resolve service for type` | Missing DI registration | Add `builder.Services.AddDbContext<T>()` or similar |
| `No such table` (SQLite) | Tables don't exist | Add `Database.EnsureCreated()` |
| `Authentication scheme not registered` | Missing auth config | Add `AddAuthentication()` / `AddDefaultIdentity()` |

**After each root-cause fix:**
1. Restart the app
2. Re-run the smoke test
3. Only proceed to page-level fixes once the home page returns 200

#### Step 4: Verify multiple routes before page repair

Once `/` returns 200, spot-check 3-4 other key routes:

```powershell
curl -k -s -o NUL -w "%{http_code}" https://localhost:5001/ProductList
curl -k -s -o NUL -w "%{http_code}" https://localhost:5001/About
curl -k -s -o NUL -w "%{http_code}" https://localhost:5001/Account/Login
```

If these all return 200, most pages are working. Skip to acceptance tests (Phase 4). If specific pages return 500, check their console errors — these are genuine page-level issues to fix.

**End timing:**
```sql
UPDATE run_timing SET finished_at = strftime('%Y-%m-%dT%H:%M:%S','now','localtime')
  WHERE phase = 'startup_triage';
```

### Phase 4: Acceptance Tests and Targeted Repair

**Start timing:**
```sql
INSERT INTO run_timing (phase, started_at) VALUES
  ('acceptance_tests', strftime('%Y-%m-%dT%H:%M:%S','now','localtime'));
```

Run the Playwright suite against the running app:

```powershell
$env:WINGTIPTOYS_BASE_URL = "https://localhost:5001"
dotnet test src\WingtipToys.AcceptanceTests\WingtipToys.AcceptanceTests.csproj --verbosity normal
```

**For each failing test:**

1. **Read the failure message** — what did the test expect vs what it got?
2. **Inspect the specific page** — `curl` or browser check. Is it a 500? Missing content? Wrong layout?
3. **Check if the CLI already generated the file correctly** — `cat` the .razor and .razor.cs. If the code looks reasonable, the problem may be data/config, not the file itself.
4. **Make the minimum fix** — don't rewrite the whole file. Fix the specific issue.

**NEVER do this:**
- See 18 test failures and rewrite 18 files
- Replace a CLI-generated code-behind with a from-scratch rewrite
- Add features that weren't in the original Web Forms app

Record:

- Total / passed / failed / skipped counts
- Per-failure root cause (startup issue? missing content? broken template? missing route?)
- Which fixes were root-cause (config/startup) vs page-specific
- Final pass condition

If the suite does not pass after targeted repairs, the run is **not** successful. Write a failed run report that explains the blocker. Do not keep rewriting files hoping something sticks.

**End timing:**
```sql
UPDATE run_timing SET finished_at = strftime('%Y-%m-%dT%H:%M:%S','now','localtime')
  WHERE phase = 'acceptance_tests';
```

### Phase 5: Screenshot Capture

**Start timing:**
```sql
INSERT INTO run_timing (phase, started_at) VALUES
  ('screenshots', strftime('%Y-%m-%dT%H:%M:%S','now','localtime'));
```

Capture proof screenshots from the working migrated app and save them under `runNN\images\`.

Recommended minimum set:

1. `01-home.png`
2. `02-products.png`
3. `03-product-details.png`
4. `04-shopping-cart.png`
5. `05-login.png`
6. `06-about.png`

Use additional screenshots when they clarify a major success or known defect.

**End timing:**
```sql
UPDATE run_timing SET finished_at = strftime('%Y-%m-%dT%H:%M:%S','now','localtime')
  WHERE phase = 'screenshots';
```

### Phase 6: Report Generation

**Start timing:**
```sql
INSERT INTO run_timing (phase, started_at) VALUES
  ('report', strftime('%Y-%m-%dT%H:%M:%S','now','localtime'));
```

Create `dev-docs\migration-tests\wingtiptoys\runNN\report.md` from `REPORT-TEMPLATE.md`.

#### Populate timing from the ledger

Query the `run_timing` table to fill the report's Timing section:

```sql
SELECT
  phase,
  started_at,
  finished_at,
  CASE
    WHEN finished_at IS NOT NULL AND started_at IS NOT NULL
    THEN CAST((julianday(finished_at) - julianday(started_at)) * 1440 AS INTEGER) || ' min'
    ELSE 'not recorded'
  END AS duration
FROM run_timing
ORDER BY rowid;
```

Copy the query output directly into the Timing table in the report. If a phase was not timed (agent was working across multiple turns without explicit checkpoints), note it as "~estimate" and explain in the Notes column.

The report must include:

- Run metadata (date, branch, operator if known)
- Source/output/tool paths
- Total wall-clock runtime
- Per-phase timing when available
- Final build result
- Final acceptance-test result
- What worked well
- What did not work well
- Toolkit/CLI gaps exposed by the run
- Embedded screenshot gallery using relative image paths

Optional supporting artifacts:

- `summary.md`
- `raw-data.md`
- captured command output snippets

**End timing (report + total):**
```sql
UPDATE run_timing SET finished_at = strftime('%Y-%m-%dT%H:%M:%S','now','localtime')
  WHERE phase = 'report';
UPDATE run_timing SET finished_at = strftime('%Y-%m-%dT%H:%M:%S','now','localtime')
  WHERE phase = 'total';
```

After closing the timing ledger, re-run the duration query above and update the report's Timing table with the final values. The **Total** row in the report must show the actual elapsed time from run start to report completion.

## Critical Rules

| Rule | Detail |
|------|--------|
| **Always clear output first** | `samples/AfterWingtipToys/` must be emptied before each run so results are reproducible |
| **Use the toolkit wrapper** | Start Layer 1 with `migration-toolkit/scripts/bwfc-migrate.ps1`, not an ad hoc direct CLI call |
| **Work from scratch** | Every run must begin from the raw source plus fresh toolkit output only; no prior migrated content may be reused |
| **No git/history restores** | Never use `git restore`, `git checkout`, `git show`, or copied historical file contents to repair the benchmark run |
| **Repair in place** | Do not swap in a smaller clean app or rewrite the site from scratch |
| **Triage before rewriting** | Always diagnose startup crashes and root causes BEFORE touching page files. A single missing `EnsureCreated()` can cause 18 test failures — fix the root cause, not the symptoms |
| **Inspect CLI output first** | Before rewriting any file, READ what the CLI generated. If the code looks correct, the problem is likely config/startup, not the file |
| **Never replace BWFC controls** | Do not replace generated `ListView`, `FormView`, `GridView`, `DataList`, or `Repeater` with manual HTML |
| **Acceptance tests are the gate** | The run is only successful when `src/WingtipToys.AcceptanceTests/` passes |
| **Report every run** | Successful or failed runs both get a numbered report folder |
| **Embed screenshots** | The main report must show images inline with Markdown links |
| **Measure total runtime** | Start timing before output cleanup and stop after the report is written |
| **Document gaps honestly** | Every manual fix that was necessary is evidence for improving the toolkit |

## Suggested Output Structure

```text
dev-docs/
  migration-tests/
    wingtiptoys/
      runNN/
        report.md
        summary.md                # optional
        raw-data.md               # optional
        images/
          01-home.png
          02-products.png
          03-product-details.png
          04-shopping-cart.png
          05-login.png
          06-about.png
```

## Reference Documents

- `migration-toolkit/README.md`
- `migration-toolkit/METHODOLOGY.md`
- `migration-toolkit/skills/migration-standards/SKILL.md`
- `migration-toolkit/skills/bwfc-migration/SKILL.md`
- `migration-toolkit/skills/bwfc-data-migration/SKILL.md`
- `migration-toolkit/skills/bwfc-identity-migration/SKILL.md`
- `src/WingtipToys.AcceptanceTests/TestConfiguration.cs`
- `dev-docs/migration-tests/wingtiptoys/run25/report.md`
