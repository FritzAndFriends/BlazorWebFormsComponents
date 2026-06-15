# beast History

## 2026-06-12 — Phase 1 doc alignment fixes (Jeffrey T. Fritz request)

### Changes made
1. **docs/dashboard.md** — Updated tracked component count from **52 → 61** to match `dev-docs/tracked-components.json`.
2. **README.md** — Added `BaseCompareValidator` and `BaseValidator` to the Validation Controls component listing (both have doc files and mkdocs entries; they were simply absent from README).
3. **README.md** — Fixed broken `View` link: was pointing to `docs/EditorControls/MultiView.md`; corrected to `docs/EditorControls/View.md` (the file exists).

### Items left alone (naming-policy-sensitive or not clearly safe)
- `Xml` (Deferred): no doc file exists; adding a README link or mkdocs entry would create a broken reference.
- `HyperLink` / `ImageMap` tracked as "Editor" in JSON but placed under Navigation in mkdocs and README — category mismatch; left for a naming-policy decision.
- `NamingContainer`, `Content`, `ContentPlaceHolder`, `MasterPage`, `ScriptManager`, `UpdatePanel`: tracked but not in the README component listing — placement/section decisions deferred.

### Verification
- All three edited files confirmed via inspection before and after.
- mkdocs.yml already had entries for all existing doc pages — no nav additions were required.

