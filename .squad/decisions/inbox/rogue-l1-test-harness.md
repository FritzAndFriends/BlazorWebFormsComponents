### 2026-07-25: L1 Test Harness — Baseline Established

**By:** Rogue (QA Analyst)

**What:** Created `migration-toolkit/tests/` with 10 focused test cases and an automated test runner (`Run-L1Tests.ps1`) measuring L1 script quality. Baseline: **7/10 pass (70%), 94.3% line accuracy**.

**Three L1 bugs documented:**
1. `<%#: Eval("Name") %>` partially converted — delimiters survive (TC06)
2. Content wrapper removal eats first-line indentation (TC09)
3. `ItemType="object"` double-added to components with explicit `TItem` (TC10)

**Why this matters:** The team now has a repeatable, automated way to measure L1 script quality. When Cyclops fixes the script, re-running `Run-L1Tests.ps1` immediately shows whether quality improved. Adding new test cases is trivial (drop .aspx + .razor pair into inputs/ and expected/).

**How to use:** `cd migration-toolkit/tests && .\Run-L1Tests.ps1`
