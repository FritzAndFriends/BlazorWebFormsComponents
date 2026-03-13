# ContosoUniversity Migration — Run {NN} Report

**Date:** {YYYY-MM-DD}
**Branch:** `{branch-name}`
**Source:** `samples/ContosoUniversity/ContosoUniversity/`
**Target:** `samples/AfterContosoUniversity/`
**Database:** SQL Server LocalDB `(localdb)\mssqllocaldb`

---

## Summary

| Metric | Result |
|--------|--------|
| **Build Status** | {✅ 0 errors / ❌ N errors} |
| **Acceptance Tests** | {passed}/{total} ({skipped} skipped) |
| **L1 Time** | {N.NN}s |
| **L2 Time** | ~{N} min ({N}s) |
| **Total Time** | ~{N} min |
| **Total Transforms** | {N} |
| **Source Files** | {N} (.aspx/.master) |
| **Output Files** | {N} ({N} .razor, {N} .cs, static assets) |
| **BLL Classes Created** | {N} |
| **Review Items** | {N} ({N} resolved) |
| **SQLite References** | {N} |
| **Legacy Artifacts** | {N} |

**Result: {ONE-LINE VERDICT}**

---

## Migration Timing

### Layer 1 — Automated Script (`bwfc-migrate.ps1`)
- **Duration:** {N.NN} seconds
- **Transforms:** {N} mechanical transforms
- **Files processed:** {N} Web Forms files ({N} .aspx + {N} .master)
- **Static files copied:** {N}
- **Model files copied:** {N}
- **Review items:** {N}

### Layer 2 — Copilot-Assisted Structural Transforms
- **Duration:** ~{N} minutes ({N} seconds)
- **Files created:** {N} (list key files)
- **Files modified:** {N} (list key files)
- **Build iterations:** {N} (describe progression)
- **Final build:** {N} errors, {N} warnings

---

## Acceptance Test Results

| Test Class | Passed | Failed | Notes |
|------------|--------|--------|-------|
| **NavigationTests** | {N}/{N} | | |
| **HomePageTests** | {N}/{N} | | |
| **AboutPageTests** | {N}/{N} | | |
| **StudentsPageTests** | {N}/{N} | | |
| **CoursesPageTests** | {N}/{N} | | |
| **InstructorsPageTests** | {N}/{N} | | |
| **Total** | **{N}/{N}** | **{N}** | |

### Failed Test Details
<!-- List each failed test with the assertion error and likely cause -->
<!-- If all tests passed, replace with: "All {N} acceptance tests passed." -->

---

## What Worked Well

### 1. {Title}
{Description of what went well, with specific details and metrics}

### 2. {Title}
{Description}

<!-- Add more subsections as needed -->

---

## What Didn't Work Well / Needs Attention

### 1. {Title}
{Description of the issue, its impact, and potential fix}

### 2. {Title}
{Description}

<!-- Add more subsections as needed -->
<!-- If everything worked, replace with: "No significant issues encountered." -->

---

## Page-by-Page Status

| Page | Status | Notes |
|------|--------|-------|
| **Home** | {✅/⚠️/❌} | |
| **About** | {✅/⚠️/❌} | |
| **Students** | {✅/⚠️/❌} | |
| **Courses** | {✅/⚠️/❌} | |
| **Instructors** | {✅/⚠️/❌} | |
| **MainLayout** | {✅/⚠️/❌} | |

---

## Architecture

```
AfterContosoUniversity/
├── Program.cs
├── ContosoUniversity.csproj
├── Components/
│   ├── App.razor
│   ├── Routes.razor
│   └── Layout/
│       └── MainLayout.razor
├── Models/
│   └── ContosoUniversityContext.cs
├── BLL/
│   └── {list BLL classes}
├── {list page .razor files}
├── _Imports.razor
└── wwwroot/
```

---

## Comparison with Previous Run (Run {NN-1})

| Metric | Run {NN-1} | Run {NN} |
|--------|------------|----------|
| **Acceptance Tests** | {N}/{N} | {N}/{N} |
| **Build Errors** | {N} | {N} |
| **L1 Time** | {N}s | {N}s |
| **L2 Time** | ~{N} min | ~{N} min |
| **SelectMethod** | {handling} | {handling} |

---

## Recommendations for Next Run

1. {Specific actionable recommendation}
2. {Specific actionable recommendation}
3. {Specific actionable recommendation}
