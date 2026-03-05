# Decision: Run 9 Benchmark Report Structure

**By:** Beast
**Date:** 2025-07-25

**What:** Run 9 benchmark report placed at `docs/Migration/Run9-WingtipToys-Benchmark.md` with copy at `samples/Run9WingtipToys/BENCHMARK-REPORT.md`. Added to mkdocs.yml nav under Migration section. Report follows Run 8 pattern with improvements: Layer 0 timing, control inventory with categories, "What Improved" section with code examples, and 3-run comparison table.

**Why:** Benchmark reports serve dual purposes — in-sample reference for developers running the migration and doc-site reference for evaluating toolkit maturity. The dual-location convention (docs/ + samples/) ensures both audiences are served. The expanded comparison table (Run 7/8/9) enables trend tracking across toolkit iterations.

**Impact:** Future benchmark reports should follow this structure. The BENCHMARK-DATA.md → BENCHMARK-REPORT.md pipeline (Bishop generates data, Beast writes report) should be the standard workflow.
