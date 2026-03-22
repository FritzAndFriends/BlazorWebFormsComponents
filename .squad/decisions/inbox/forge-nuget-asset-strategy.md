# Decision: NuGet Static Asset Migration Strategy

**Date:** 2026-03-08  
**By:** Forge (Lead / Web Forms Reviewer)  
**Status:** Proposed (awaiting Jeffrey T. Fritz approval & team review)

---

## Decision

Implement **Option C (NuGet Extraction Tool) + optional WebOptimizer** as the default migration strategy for BWFC's `bwfc migrate-assets` command.

### What This Means

1. **Primary Strategy:** PowerShell script that reads `packages.config`, extracts `Content/` and `Scripts/` folders from NuGet packages, places them in `wwwroot/lib/`, and generates Blazor-compatible asset references.

2. **Intelligent CDN Mapping:** For known OSS packages (jQuery, Bootstrap, DataTables, Modernizr, SignalR, etc.), suggest CDN URLs instead of extraction to reduce wwwroot footprint.

3. **Hybrid Default:** Extract custom/private packages, suggest CDN for OSS packages, output decision summary.

4. **Optional Bundling:** Teams can integrate WebOptimizer or esbuild post-migration for minification + cache-busting (not required).

---

## Why This Approach

| Option | Pros | Cons | Recommendation |
|--------|------|------|---|
| **A: CDN** | Simple, no wwwroot bloat | Internet-dependent, no custom packages | ❌ Not suitable for all apps |
| **B: LibMan** | VS integrated, mixed sources | Limited to public libs, learning curve | ⚠️ Good alt for known packages |
| **C: Extraction Tool** | Works for all packages, automated, auditable | wwwroot grows, requires script maintenance | ✅ **Recommended** |
| **D: npm equivalents** | Modern ecosystem, powerful | Requires Node.js toolchain, complex setup | ⚠️ Good for modern teams |

**Selected:** **Option C (Hybrid approach)** — Combines extraction for custom packages + CDN suggestions for known OSS, maximizing automation while respecting team preferences.

---

## Implementation Details

### New Command

```bash
bwfc migrate-assets --source C:\MyWebFormsApp [--strategy hybrid|extract|cdn]
```

### Script Location

`migration-toolkit/scripts/Migrate-NugetStaticAssets.ps1`

### Execution Flow

1. Parse `packages.config` → extract package IDs + versions
2. Scan `packages/` folder for `Content/` and `Scripts/` directories
3. For each detected package:
   - If known OSS (in CDN map) AND strategy allows: suggest CDN, skip extraction
   - Else: extract to `wwwroot/lib/{PackageName}/`
4. Generate `asset-manifest.json` (extraction summary)
5. Generate `AssetReferences.html` (copy-paste snippet)
6. Output console summary (packages extracted, CDN suggested, custom preserved)

### Known CDN Mappings (Initial)

- jQuery → https://code.jquery.com/
- Bootstrap → https://stackpath.bootstrapcdn.com/bootstrap/
- Modernizr → https://cdnjs.cloudflare.com/
- DataTables → https://cdn.datatables.net/
- [+10 more]

### Output

**Console:**
```
✓ NuGet Static Asset Migration
========================================
Detected 12 packages:
  ✓ jQuery.3.6.0 → CDN (...)
  ⓘ MyApp.Reports.1.0.0 → Extracted to wwwroot/lib/MyApp.Reports/

Generated Asset References:
<link href="https://code.jquery.com/jquery-3.6.0.min.js" rel="stylesheet" />
<link href="/_framework/lib/MyApp.Reports/reports.css" rel="stylesheet" />
...
```

**Files:**
- `wwwroot/lib/{PackageName}/` (extracted assets)
- `asset-manifest.json` (metadata)
- `AssetReferences.html` (snippet for App.razor)

---

## Rationale

1. **Handles all scenarios:** Works for OSS packages, custom packages, and mixed setups.
2. **Automation-ready:** Integrates into `bwfc migrate-assets` for one-command migration.
3. **Low barrier:** No Node.js, webpack, or advanced tooling required.
4. **Preserves fidelity:** Exact same assets as original Web Forms app.
5. **Auditable:** Generated manifest makes decisions transparent.
6. **Scalable:** Works for small DepartmentPortal (1 custom CSS) to enterprise apps (50+ packages).

---

## DepartmentPortal Validation

**Original (Web Forms):**
- `packages.config`: Only build tool (no static assets)
- `Content/Site.css`: Custom app stylesheet
- No external NuGet libraries

**Migration Result:**
- `wwwroot/css/site.css` (copied)
- `asset-manifest.json`: No external packages detected
- `AssetReferences.html`: Single link tag for custom CSS

**Outcome:** ✅ Minimal case validates extraction logic for custom assets.

---

## Timeline

- **Week 1–2:** Implement `Migrate-NugetStaticAssets.ps1` + CDN mappings
- **Week 2–3:** Integrate into `bwfc-migrate.ps1` and `bwfc` CLI
- **Week 3–4:** Documentation + performance/security guides
- **Week 4–5:** Hardening + edge case handling
- **Week 5–6:** GA release

---

## Related Artifacts

- **Strategy Document:** `dev-docs/proposals/nuget-static-asset-migration.md`
- **GitHub Issue Draft:** Issue specifications documented locally (awaiting GitHub creation by Jeffrey T. Fritz)
- **Implementation File:** `migration-toolkit/scripts/Migrate-NugetStaticAssets.ps1` (to be created)

---

## Open Questions

1. Should we support `.nupkg` file inspection (fallback if `packages/` folder unavailable)? → **Yes, add as Phase 2**
2. What's the max wwwroot size threshold before suggesting LibMan/CDN? → **No hard limit; let users decide**
3. Should we integrate WebOptimizer by default, or keep it optional? → **Optional; document as Phase 2 enhancement**
4. How to handle version mismatches (e.g., app expects Bootstrap 4.6, CDN has 5.0)? → **Exact version matching required; fail fast**

---

## Approval Chain

- [ ] Jeffrey T. Fritz (Project Owner)
- [ ] Team (Cyclops, Beast, Jubilee, Rogue)
- [ ] Implementation: Assign to Cyclops (PowerShell + toolkit integration)

---

**Document Owner:** Forge  
**Created:** 2026-03-08  
**Status:** Proposed
