# Bishop Run 39 Gap Analysis

## Priority summary

### P0 — Must fix to keep the benchmark green

1. **Acceptance-path runtime wiring**  
   - **Category:** Runtime wiring  
   - **Gap:** Fresh output still needed manual catalog, cart, and auth runtime setup before the benchmark path worked end-to-end.  
   - **Concrete fix:** Add a Wingtip benchmark scaffold mode in the CLI that emits the lightweight runtime used in successful runs: benchmark-safe catalog data service, cart persistence wiring, session-enabled request pipeline, and simple auth pages/services.  
   - **Estimated impact:** Saves 5-8 minutes per run and removes the biggest remaining manual repair bucket.

2. **Compile-surface debt outside the benchmark path**  
   - **Category:** Compile surface  
   - **Gap:** Fresh output still depended on prior compile-surface reduction strategy so non-benchmark pages would not block build/test validation.  
   - **Concrete fix:** Expand CLI quarantine/stub coverage for heavy Account/Admin/Checkout/payment pages and automatically exclude or park unsupported artifacts behind safe routable stubs plus migration-artifact preservation.  
   - **Estimated impact:** Saves 3-5 minutes per run and prevents build-blocking regressions.

### P1 — Significant time savings

3. **FormView SSR first-render behavior**  
   - **Category:** Library fix  
   - **Gap:** `FormView` did not establish `CurrentItem` until after first render, so SSR details pages arrived blank on first request.  
   - **Concrete fix:** Keep the current library fix and add focused regression coverage proving `CurrentItem` is available during parameter processing/first SSR render.  
   - **Estimated impact:** Saves 3-4 minutes of debugging and prevents benchmark-path false negatives.

4. **Session raw-string round-trip**  
   - **Category:** Library fix  
   - **Gap:** Page-side cart/session lookups could miss persisted raw string values because `SessionShim` assumed JSON payloads on readback.  
   - **Concrete fix:** Keep the current shim fallback for raw strings and add regression tests for mixed JSON/object and raw-string session storage across requests.  
   - **Estimated impact:** Saves 2-3 minutes and prevents cart/auth state mismatches.

### P2 — Quality improvements with smaller time wins

5. **Generate request-safe session access patterns**  
   - **Category:** CLI transform  
   - **Gap:** Benchmark pages still needed manual repair to read/write cart IDs directly against `Context.Session` on request-bound paths.  
   - **Concrete fix:** Add a transform or scaffold helper that rewrites obvious session-cookie/cart-ID patterns to request-safe APIs when migrating action/cart pages.  
   - **Estimated impact:** Saves 1-2 minutes and reduces fragile manual edits.

6. **Port/process safety during iterative validation**  
   - **Category:** Runtime wiring  
   - **Gap:** Rebuilds while the app was still live caused transient `MSB3027/MSB3021` output-lock failures.  
   - **Concrete fix:** Have the migration test workflow or helper script detect/stop the prior app PID before rebuild/run phases.  
   - **Estimated impact:** Saves about 1 minute and reduces noisy false failures.

### P3 — Nice to have

7. **Benchmark-path diagnostics in reports/tooling**  
   - **Category:** Runtime wiring  
   - **Gap:** Blank SSR details pages and session mismatches were diagnosable, but only after manual inspection.  
   - **Concrete fix:** Add a lightweight smoke check after migration/build for key pages (`/ProductList`, `/ProductDetails?id=...`, `/ShoppingCart`) to flag blank-body SSR or missing cart persistence before full Playwright runs.  
   - **Estimated impact:** Saves less than 1 minute on average, but improves operator feedback.

## Run 38 → Run 39 progress

### Fixed since Run 38

- **Acceptance-path data controls are no longer being replaced by manual HTML.** Run 39 preserved `ListView`, `FormView`, and `GridView` on the benchmark path.
- **Run 38 `FormView/query-details` gap is materially improved.** The details page no longer needs manual control replacement; the remaining issue was a library SSR timing bug, now fixed in `FormView`.
- **The cart path is closer to framework-correct behavior.** Run 39 isolated the remaining failure to session-string round-trip and repaired it in `SessionShim`.

### Still persisting from Run 38

- **Runtime scaffold gap persists.** Fresh output still needs manual catalog/cart/auth setup to satisfy the benchmark suite.
- **Compile-surface exclusion/quarantine gap persists.** Non-benchmark pages still need automated pruning/stubbing so they do not block build validation.
- **Checkout/payment and heavy account/admin surfaces remain outside automatic coverage.** They are still part of the compile-surface debt even if the benchmark path can be made green.

## Net assessment

Run 39 shows clear progress: the benchmark path now keeps BWFC data controls intact and the two regressions were true library bugs, not proof that the controls must be flattened. The next highest-value work is no longer control preservation; it is automating the benchmark runtime scaffold and hardening compile-surface quarantine so fresh runs reach green with minimal manual repair.
