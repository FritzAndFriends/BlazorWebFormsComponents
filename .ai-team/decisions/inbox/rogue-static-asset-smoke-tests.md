# Decision: Static Asset Smoke Tests Added to Acceptance Suite

**Author:** Rogue (QA Analyst)
**Date:** 2026-03-06
**Status:** Implemented
**Triggered by:** Run 9 RCA — Fix 3 (visual failure: no CSS, images 404)

## Context

Run 9 passed all 14 functional acceptance tests but was a VISUAL FAILURE. The navbar rendered as a plain bullet list (Bootstrap CSS not loaded) and all product images returned 404. Functional tests don't catch this class of failure.

## Decision

Added `StaticAssetTests.cs` to `src/WingtipToys.AcceptanceTests/` with 11 tests covering:

1. **CSS delivery** — Intercept network responses during page load, assert at least one CSS file loads and all return HTTP 200.
2. **Image integrity** — On ProductList, iterate all `<img>` elements and verify `naturalWidth > 0` (catches 404s). Also intercept image network responses for HTTP status.
3. **Bootstrap styling** — Assert `.navbar` class exists and element height ≥ 30px (catches "bullet list navbar" regression).
4. **Visual sanity screenshots** — Homepage, ProductList, ProductDetails — verify key elements have non-zero dimensions, take full-page screenshots and assert byte size > threshold (catches blank/unstyled pages).
5. **Catch-all static asset check** — Intercept all static asset responses (CSS/JS/images/fonts/ico) and fail on any 4xx/5xx.

## Consequences

- Future migration runs that break CSS paths or image paths will fail acceptance tests before anyone has to eyeball the site.
- Screenshot byte-size checks are a coarse proxy — they won't catch subtle styling differences but will catch "completely unstyled" or "mostly blank" pages.
- No pixel-perfect comparison implemented (would require baseline images and maintenance overhead). If Jeff wants that later, we'd add Playwright's `toHaveScreenshot()` with stored baselines.

## Who Should Know

- **Forge/Beast:** Migration scripts need to preserve static asset paths or these tests will fail.
- **Jeff:** These tests directly address the Run 9 visual failure RCA. They verify CSS loads, images load, and Bootstrap styling is applied — not pixel-perfect comparison, but "the site isn't obviously broken" verification.
