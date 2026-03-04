# Decision: Run 4 Migration Results and Script Enhancement Recommendations

**Date:** 2026-03-04
**By:** Forge (Lead / Web Forms Reviewer)
**Status:** Recorded

## Context

Completed Run 4 of the WingtipToys migration benchmark using the enhanced `bwfc-migrate.ps1` script. All 11 features pass, build is clean (0 errors, 0 warnings), 289 transforms applied.

## Key Results

1. **ConvertFrom-MasterPage works well.** Auto-generates MainLayout.razor from Site.Master. Highest-impact enhancement — eliminates the most complex manual step.
2. **Format-string regexes work correctly.** Eval format-string and simple String.Format patterns converted mechanically.
3. **289 transforms** (up from 277 in Run 3), **7 scaffold files** (up from 4).
4. **Manual items still at 18** — new ContentPlaceHolder/LoginView/SelectMethod items offset eliminated format-string items.

## Recommendations

1. **Add CascadingAuthenticationState to New-AppRazorScaffold.** The Routes.razor scaffold should wrap the Router in `<CascadingAuthenticationState>` by default. Every Blazor app using AuthorizeView needs this, and it's a common build error.

2. **Consider adding a `--with-auth` flag to bwfc-migrate.ps1.** When present, generate Identity-aware Routes.razor and add authentication services to Program.cs scaffold.

3. **Master page conversion quality is high enough for production use.** The auto-generated MainLayout.razor requires only Layer 2 fixes (LoginView→AuthorizeView, SelectMethod→injected service), not a full rewrite.

## Impact

Run 4 validates that the enhanced script is ready for inclusion in the migration toolkit. The 3 new features (master page conversion, App/Routes scaffold, format-string regexes) collectively reduce manual Layer 2 work by approximately 30-40 minutes per migration.
