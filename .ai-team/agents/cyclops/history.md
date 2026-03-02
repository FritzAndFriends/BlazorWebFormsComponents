# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents  Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!--  Summarized 2026-02-27 by Scribe  covers M1M16 -->

### Core Context (2026-02-10 through 2026-02-27)

**M1M3 components:** Calendar (enum fix, async events), ImageMap (BaseStyledComponent, Guid IDs), FileUpload (InputFile integration, path sanitization), PasswordRecovery (3-step wizard), DetailsView (DataBoundComponent<T>, auto-field reflection, 10 events), Chart (BaseStyledComponent, CascadingValue "ParentChart", JS interop via ChartJsInterop, ChartConfigBuilder pure static).

**M6 base class fixes:** DataBoundComponent chain  BaseStyledComponent (14 data controls). BaseListControl<TItem> for 5 list controls (DataTextFormatString, AppendDataBoundItems). CausesValidation on CheckBox/RadioButton/TextBox. Label AssociatedControlID switches spanlabel. Login/ChangePassword/CreateUserWizard  BaseStyledComponent. Validator ControlToValidate dual-path: ForwardRef + string ID via reflection.

**M6 Menu overhaul:**  BaseStyledComponent. Selection tracking (SelectedItem/SelectedValue, MenuItemClick, MenuItemDataBound). MenuEventArgs, MaximumDynamicDisplayLevels, Orientation enum + CSS horizontal class. MenuLevelStyle lists. StaticMenuStyle sub-component + IMenuStyleContainer interface. RenderFragment parameters for all menu styles. RenderingMode=Table added (M14) with inline Razor for AngleSharp compatibility.

**M7 style sub-components:** GridView (8), DetailsView (10), FormView (7), DataGrid (7)  all CascadingParameter + UiTableItemStyle. Style priority: Edit > Selected > Alternating > Row. TreeView: TreeNodeStyle + 6 sub-components, selection, ExpandAll/CollapseAll, FindNode, ExpandDepth, NodeIndent. GridView: selection, 10 display props. FormView/DetailsView events + PagerTemplate + Caption. DataGrid paging/sorting. ListView 10 CRUD events + EditItemTemplate/InsertItemTemplate. Panel BackImageUrl. Login Orientation + TextLayout. Shared PagerSettings (12 props, IPagerSettingsContainer) for GridView/FormView/DetailsView.

**M8 bug fixes:** Menu JS null guard + Calendar conditional scope + Menu auto-ID (`menu_{GetHashCode():x}`).

**M9 migration-fidelity:** ToolTip  BaseStyledComponent (removed from 8, added title="@ToolTip" to 32 components). ValidationSummary comma-split fix (IndexOf + Substring). SkinID boolstring. TreeView NodeImage fallback restructured (ShowExpandCollapse + ExpandCollapseImage helper).

**M10 Theming:** ControlSkin (nullable props, StyleSheetTheme semantics). ThemeConfiguration (case-insensitive keys, empty-string default skin, GetSkin returns null). ThemeProvider as CascadingValue wrapper. SkinID="" default, EnableTheming=true, [Obsolete] removed. CascadingParameter in BaseStyledComponent, ApplySkin in OnParametersSet. LoginView/PasswordRecovery  BaseStyledComponent.

**M15 HTML fidelity fixes:** Button `<input>` rendering. BulletedList `<span>` removal + `<ol>` CSS-only (no HTML type attr, GetStartAttribute returns int?). LinkButton class + aspNetDisabled. Image longdesc conditional. Calendar structural (tbody, width:14%, day titles, abbr headers, align center, border-collapse, navigation sub-table). FileUpload clean ID. CheckBox span verified. GridView UseAccessibleHeader default falsetrue. 27 test files updated for Button `<input>`. 10 new tests. All 1283 pass.

**M16:** LoginView wrapper `<div>` for styles (#352). ClientIDMode enum (Inherit/AutoID/Static/Predictable) on BaseWebFormsComponent. ComponentIdGenerator refactored: GetEffectiveClientIDMode(), BuildAutoID(), BuildPredictableID(). UseCtl00Prefix only in AutoID mode. NamingContainer auto-sets AutoID when UseCtl00Prefix=true.

**Key patterns:** Orientation enum collides with parameter name  use `Enums.Orientation.Vertical`. `_ = callback.InvokeAsync()` for render-time events. `Path.GetFileName()` for file save security. CI secret-gating: env var indirection. Null-returning helpers for conditional HTML attributes. aspNetDisabled class for disabled controls. Always test default parameter values explicitly.


<!--  Summarized 2026-03-01 by Scribe  covers M17-M20 Wave 1 -->

### M17-M20 Wave 1 Context (2026-02-27 through 2026-03-01)

**M17 AJAX audit fixes:** ScriptManager EnablePartialRendering default false>true. Scripts collection added (List<ScriptReference>). UpdateProgress conditional CssClass + display:block;visibility:hidden for non-dynamic mode. ScriptReference gained ScriptMode/NotifyScriptLoaded/ResourceUICultures. Lesson: C# bool defaults false, but Web Forms often defaults true.

**M18 bug verification:** #380 BulletedList, #382 CheckBox span, #383 FileUpload GUID  all verified already fixed in M15. FileUpload blazor:elementReference is inherent InputFile artifact. Lesson: verify current state before assuming bugs still exist.

**M18 deterministic IDs & Menu fonts:** CheckBox bare input gained id (#386). MenuItemStyle needed SetFontsFromAttributes(OtherAttributes) in OnInitialized (#360)  Font-Bold maps to Font.Bold sub-property. Lesson: CaptureUnmatchedValues + Font- attrs need explicit SetFontsFromAttributes handling.

**Issue #379:** LinkButton CssClass verified already correct from M15. GetCssClassOrNull() returns null for empty, appends aspNetDisabled when disabled. Edge case: IsNullOrEmpty not IsNullOrWhiteSpace.

**Issue #387 normalizer:** 4 enhancements  case-insensitive file pairing, boolean attr collapse (6 attrs), empty style stripping, GUID ID placeholders. Pipeline order: regex > style norm > empty style strip > boolean attrs > GUID IDs > attr sort > artifact cleanup > whitespace. Key files: scripts/normalize-html.mjs, scripts/normalize-rules.json.

**Issues #364/#365 theming:** SkinBuilder uses expression trees for Set<TValue>()  supports direct (s.BackColor) and nested (s.Font.Bold) via recursive GetOrCreateValue. ThemeConfiguration gained ForControl(name, configure) fluent methods. ThemeProvider cascades via unnamed CascadingValue  nesting naturally overrides. WebColor.FromHtml() added. 14 unit + 4 bUnit tests. Lesson: expression trees require recursive auto-init of intermediate nulls. CascadingValue by type sufficient for unique types.

Team update (2026-02-27): Branching workflow  feature PRs from personal fork to upstream dev, only dev>main on upstream  decided by Jeffrey T. Fritz
Team update (2026-02-27): Issues must be closed via PR references using 'Closes #N'  decided by Jeffrey T. Fritz
Team update (2026-02-27): AJAX Controls nav category; migration stub doc pattern for no-ops  decided by Beast
Team update (2026-02-27): M17 sample pages created  decided by Jubilee
Team update (2026-02-27): Forge approved M17 with 4 non-blocking follow-ups  decided by Forge
Team update (2026-02-27): Timer duplicate [Parameter] bug fixed; 47 M17 tests  decided by Rogue
Team update (2026-02-27): No-op stub property coverage 41-50% acceptable  decided by Forge
Team update (2026-02-27): UpdatePanel Triggers deliberately omitted  decided by Forge
Team update (2026-02-28): GetCssClassOrNull() uses IsNullOrEmpty not IsNullOrWhiteSpace  low priority  noted by Rogue
 Team update (2026-03-01): Skins & Themes has dual docs  SkinsAndThemes.md (practical guide, update first) and ThemesAndSkins.md (architecture). Update SkinsAndThemes.md first for API changes  decided by Beast
 Team update (2026-03-01): D-11 through D-14 formally registered  D-11 GUID IDs needs fix, D-12 boolean attrs intentional, D-13/D-14 Calendar fixes recommended  decided by Forge

### Issue #366 — Base Class Theme Integration (2026-03-01)

- **Theme wiring moved to BaseWebFormsComponent:** Added `[CascadingParameter] ThemeConfiguration CascadedTheme` to BaseWebFormsComponent. Added `OnParametersSet` override that resolves the ControlSkin via `GetType().Name + SkinID` and calls virtual `ApplyThemeSkin(ControlSkin)`. Base implementation is no-op; BaseStyledComponent overrides to apply IStyle properties.
- **BaseStyledComponent simplified:** Removed `[CascadingParameter] Theme` and `OnParametersSet` override. Renamed `ApplySkin` to `ApplyThemeSkin` (protected override). StyleSheetTheme semantics preserved: theme sets defaults, explicit values win.
- **Property naming:** Named the cascading parameter `CascadedTheme` (not `Theme`) because `_Imports.razor` has `@inherits BaseWebFormsComponent` making ALL `.razor` files inherit from it. WebFormsPage and ThemeProvider both have their own `[Parameter] Theme` — same name would cause Blazor's "declares more than one parameter" error.
- **ThemeProvider fix:** Added `@inherits ComponentBase` to ThemeProvider.razor so it doesn't inherit BaseWebFormsComponent via _Imports.razor. ThemeProvider is infrastructure, not a Web Forms control.
- **WebFormsPage fix:** Changed `<CascadingValue Value="Theme">` to `Value="@(Theme ?? CascadedTheme)"` so the cascade works whether the user passes Theme explicitly or inherits it from a parent ThemeProvider.
- **Lesson:** `_Imports.razor @inherits BaseWebFormsComponent` affects ALL .razor files in the project, including infrastructure components like ThemeProvider. When adding properties to BaseWebFormsComponent, check for name conflicts with every .razor component's @code block.
- **Lesson:** C# `virtual`/`override` on properties with different attributes ([CascadingParameter] vs [Parameter]) does NOT work for Blazor — reflection returns the base class's attribute, not the override's. Use different property names instead.

### FontInfo Name/Names Auto-Sync Fix (2026-03-01)

- **FontInfo auto-sync:** Converted `Name` and `Names` from auto-properties to backing-field properties with bidirectional sync. Setting `Name` also sets `Names` (single value). Setting `Names` also sets `Name` (first comma-separated entry, trimmed). Setting either to null/empty clears both. Matches ASP.NET Web Forms `FontInfo` behavior.
- **ApplyThemeSkin guard:** Updated `BaseStyledComponent.ApplyThemeSkin` to check both `Font.Name` AND `Font.Names` are empty before applying theme font. Prevents theme from overriding an explicitly set `Names` value.
- **Root cause:** `ApplyThemeSkin` set `Font.Name` but `HasStyleExtensions.ToStyle()` reads `Font.Names` for `font-family`. Without auto-sync, theme fonts were silently lost.
- **Lesson:** When Web Forms has paired/synced properties (Name↔Names, Value↔SelectedValue, etc.), our Blazor equivalents must replicate the sync behavior or the rendering pipeline breaks at property boundaries.

### CI/CD Unified Release Process (2026-03-02)

- **Unified release.yml:** Created `.github/workflows/release.yml` triggered on `release: published`. Single workflow coordinates all release artifacts: NuGet publish, Docker build+push to GHCR, docs deploy to GitHub Pages, demo builds with release attachment. All jobs extract version from `github.event.release.tag_name` stripping the `v` prefix, ensuring every artifact gets the same version.
- **Version extraction pattern:** Use `${{ github.event.release.tag_name }}` (not `${{ github.ref_name }}`) for release events. Strip `v` prefix via bash `${VERSION#v}` and write to `$GITHUB_ENV` for use across steps.
- **NuGet version override:** Pass both `-p:PackageVersion=$VERSION` and `-p:Version=$VERSION` to `dotnet pack` and `dotnet build` respectively, overriding NBGV-computed versions with the exact release tag version.
- **Secret-gating pattern:** Use `env: NUGET_API_KEY: ${{ secrets.NUGET_API_KEY }}` at step level, then `if: env.NUGET_API_KEY != ''` — this is the GitHub Actions idiom for conditional steps based on secret availability.
- **gh CLI in workflows:** Set `GH_TOKEN: ${{ secrets.GITHUB_TOKEN }}` as env var for `gh release upload` commands.
- **Docker image tags lowercase:** Registry/image path must be lowercase: `ghcr.io/fritzandfriends/blazorwebformscomponents/serversidesamples`.
- **deploy-server-side.yml refactored:** Removed `push: branches: [main]` trigger and path filters. Now `workflow_dispatch` only — emergency manual escape hatch.
- **nuget.yml refactored:** Removed `push: tags: [v*]` trigger. Now `workflow_dispatch` with version input — emergency manual NuGet republish.
- **docs.yml fix:** Replaced deprecated `echo ::set-output name=release::${RELEASE}` with `echo "release=${RELEASE}" >> "$GITHUB_OUTPUT"`. Kept push-to-main deploy for doc-only changes between releases.
- **demo.yml versioned artifacts:** Added NBGV version computation step. Artifact names now include version: `demo-server-side-${{ steps.nbgv.outputs.version }}`.
- **version.json:** Changed from `"version": "0.17"` (2-segment) to `"version": "0.17.0"` (3-segment SemVer). NBGV now computes clean 3-segment versions matching our tag format.
- **NBGV key lesson:** NBGV ignores git tags entirely — it reads `version.json` for major.minor and computes patch from git height. Tags are informational; `version.json` must be kept in sync. For releases, override NBGV output with explicit `-p:Version=` from the tag.
- **Workflow dependency order:** release.yml uses `needs:` to enforce: build-and-test → [publish-nuget, deploy-docker, deploy-docs, build-demos] (fan-out after gate job).
- **Lesson:** GitHub Actions `$GITHUB_OUTPUT` replaced `::set-output` (deprecated Oct 2022). Always use `echo "key=value" >> "$GITHUB_OUTPUT"` for step outputs.
