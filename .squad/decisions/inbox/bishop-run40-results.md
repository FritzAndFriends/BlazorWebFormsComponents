# Bishop Run 40 Results

## Priority summary

### P0 — Highest-value automation gaps still blocking fast fresh Wingtip runs

1. **Benchmark runtime scaffold still needs a first-class mode**  
   - **Category:** Runtime wiring  
   - **Gap:** `RuntimeDetector` / `ProgramCsEmitter` now emit a credible modern scaffold, but the fresh Wingtip output still required manual replacement of the generated DB/identity runtime with a lightweight benchmark-safe catalog/cart/auth runtime before the acceptance suite could pass.  
   - **Concrete fix:** Add a Wingtip benchmark scaffold profile in the CLI that emits the proven minimal runtime pattern: seeded catalog service, singleton cart store keyed by a stable session cart key, session middleware, and simple register/login/logout endpoints.  
   - **Estimated impact:** Saves 5-8 minutes per run and removes the largest remaining manual repair bucket.

2. **Compile-surface quarantine is still too shallow**  
   - **Category:** Compile surface  
   - **Gap:** Fresh output still shipped a long tail of Account/Admin/Checkout/mobile/payment artifacts that were quicker to stub manually than to migrate.  
   - **Concrete fix:** Expand CLI compile-surface stubbing/quarantine so infrastructure-heavy routable pages emit build-safe stubs automatically while preserving transformed artifacts for manual follow-up.  
   - **Estimated impact:** Saves 4-6 minutes per run and reduces first-build noise dramatically.

### P1 — Important correctness improvements

3. **BWFC templated control emission still needs hardening**  
   - **Category:** Markup transforms  
   - **Gap:** Fresh `ProductList` and `ProductDetails` output still arrived with malformed `ListView` / `FormView` structure and invalid template content.  
   - **Concrete fix:** Add focused transform coverage for templated data-control child structure, template wrapping, and item-context references on real Wingtip fixtures.  
   - **Estimated impact:** Saves 2-4 minutes and preserves benchmark-path controls with less manual rewrite work.

4. **Generated cart/session patterns should prefer stable session keys over raw `Session.Id`**  
   - **Category:** Runtime wiring  
   - **Gap:** Run 40's first acceptance pass reached `24/25` because navbar cart count updated while the cart page still rendered empty until the runtime switched to an explicit session-backed `cart-key`.  
   - **Concrete fix:** Bake a `GetOrCreateCartKey()` helper pattern into the benchmark scaffold or cart-page transforms so request and page reads share the same persisted key.  
   - **Estimated impact:** Saves 2-3 minutes and prevents a recurring cart-flow false negative.

### P2 — Smaller but worthwhile workflow improvements

5. **Build/rebuild should be port-safe by default**  
   - **Category:** Validation workflow  
   - **Gap:** One validation iteration failed with `MSB3027/MSB3021` because `WingtipToys.exe` still held the output binary.  
   - **Concrete fix:** Teach the benchmark helper flow to detect and stop the listening PID before rebuild/run steps.  
   - **Estimated impact:** Saves about 1 minute and reduces noisy false failures.

6. **Screenshot capture should stay serialized in the workflow**  
   - **Category:** Evidence capture  
   - **Gap:** Parallel navigation on one Playwright page produced misleading screenshot files even though the app itself was healthy.  
   - **Concrete fix:** Keep the runbook and any helper automation strictly serial for screenshot capture: navigate, wait, capture, verify, then move on.  
   - **Estimated impact:** Small time win, but avoids invalid benchmark evidence.

## Run 39 → Run 40 progress

### Improved in Run 40

- Fresh Run 40 again preserved the required BWFC controls (`ListView`, `FormView`, `GridView`) on the acceptance path.
- The generated modern scaffold was good enough to validate `RuntimeDetector` / `ProgramCsEmitter` as a viable starting point rather than a dead-end scaffold.
- Total benchmark time dropped from `00:38:34.12` to `00:21:55.10` while still finishing at `25/25` acceptance tests.

### Still persisting from earlier runs

- Fresh output still needs manual benchmark runtime wiring to get catalog/cart/auth behavior green.
- Compile-surface debt outside the benchmark path is still too expensive in raw fresh output.
- Templated data-control emission is still not deterministic enough on real Wingtip pages.

## Net assessment

Run 40 confirms the migration wrapper and modern scaffold generation are moving in the right direction, but the benchmark is still won or lost on three things: emitting a benchmark-safe runtime automatically, reducing compile-surface debt before first build, and making templated BWFC page output trustworthy on real migrated pages. Those remain the highest-leverage investments for future Wingtip runs.
