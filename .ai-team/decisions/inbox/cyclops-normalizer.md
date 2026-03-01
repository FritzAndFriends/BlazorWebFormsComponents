### 2026-03-01: Normalizer pipeline order and compare case-insensitivity

**By:** Cyclops
**What:** The HTML normalizer pipeline in `scripts/normalize-html.mjs` runs transforms in a fixed order: regex rules → style normalization → empty style strip → boolean attrs → GUID IDs → attribute sort → artifact cleanup → whitespace. Compare mode uses case-insensitive file pairing (lowercase key maps) so that folder casing differences (e.g., HyperLink vs Hyperlink) don't produce false divergences. Boolean attributes are collapsed to bare form, GUIDs in IDs are replaced with `GUID` placeholder, and empty `style=""` attributes are stripped.
**Why:** These 4 enhancements (issue #387) eliminate the main sources of false-positive divergences in the HTML fidelity audit. The pipeline ordering matters because later steps depend on earlier cleanup (e.g., empty style stripping must happen after style normalization).
