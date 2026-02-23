### Themes and Skins Migration Strategy

**By:** Forge
**Date:** 2026-02-12
**What:** Evaluated 5 approaches for migrating Web Forms Themes and Skins to Blazor. Recommended **CascadingValue ThemeProvider** (Approach 2) as the primary strategy. CSS Custom Properties are complementary but insufficient alone. Generated CSS Isolation rejected due to high tooling cost. DI-based approach rejected because it cannot scope themes to subtrees.

**Key points:**
- CascadingValue is the only approach that faithfully models both `Theme` (override) and `StyleSheetTheme` (default) semantics
- SkinID must be corrected from `bool` to `string` on `BaseWebFormsComponent` before any implementation
- Implementation is opt-in via `<ThemeProvider>` wrapper — zero breaking changes to existing consumers
- Theme resolution logic lives in `BaseWebFormsComponent.OnParametersSet`, benefiting all 50+ components automatically
- Strategy is exploratory — the README still says themes/skins are not converted; this documents how they *could* be

**Why:** Jeff requested exploration of theme/skin migration despite the project's current exclusion. Developers migrating themed Web Forms apps need guidance. The CascadingValue approach aligns with existing library patterns (TableItemStyle already cascades) and is incrementally adoptable.
