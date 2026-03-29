# Bishop - Migration Tooling Dev History

## Role
Bishop is the Migration Tooling Dev on the BlazorWebFormsComponents project, responsible for building migration tools and utilities that help developers move from ASP.NET Web Forms to Blazor.

## Project Context
BlazorWebFormsComponents is a library providing Blazor components that emulate ASP.NET Web Forms controls, enabling migration with minimal markup changes. The project aims to preserve the same component names, attributes, and HTML output as the original Web Forms controls.

## Learnings

### WI-8: .skin File Parser Implementation (2025-01-26)

**Task**: Build a runtime parser that reads ASP.NET Web Forms .skin files and converts them into ThemeConfiguration objects.

**Implementation Details**:
- Created `SkinFileParser.cs` with three public methods:
  - `ParseSkinFile(string, ThemeConfiguration)` - parses .skin content from string
  - `ParseSkinFileFromPath(string, ThemeConfiguration)` - parses single .skin file from disk
  - `ParseThemeFolder(string, ThemeConfiguration)` - parses all .skin files in a directory
  
- Parsing approach:
  - Strip ASP.NET comments (`<%-- ... --%>`)
  - Wrap content in root element and replace `<asp:` with `<asp_` for XML compatibility
  - Parse as XML using XDocument
  - Walk element tree to build ControlSkin and TableItemStyle objects

- Type conversions:
  - WebColor: use `WebColor.FromHtml(value)` for color attributes
  - Unit: use `new Unit(value)` for size/width attributes
  - FontUnit: use `FontUnit.Parse(value)` for font sizes
  - BorderStyle: use `Enum.TryParse<BorderStyle>()` for enum values
  - Font attributes: special handling for `Font-Bold`, `Font-Italic`, `Font-Size`, etc.

- Sub-styles: Nested elements like `<HeaderStyle>`, `<RowStyle>` become entries in `ControlSkin.SubStyles` dictionary as `TableItemStyle` objects

- Error handling: Defensive parsing with try-catch blocks and console warnings, never throws on parse errors

**Key Technical Decisions**:
1. Used XML parsing after preprocessing rather than custom parser - leverages proven XML infrastructure
2. Case-insensitive attribute and control name matching for robustness
3. Silently ignore unknown attributes to handle variations in .skin files
4. Console.WriteLine for warnings rather than throwing exceptions - allows partial parsing success

**Build Status**: ✅ Successfully builds with no errors

**Verification**: ✅ Tested with sample .skin content:
- Successfully parsed default Button skin with colors and font properties
- Successfully parsed named Button skin (SkinID="DangerButton")
- Successfully parsed GridView with nested HeaderStyle and RowStyle sub-components
- All color conversions, font attributes, and sub-styles worked correctly

### Web Forms Theme Migration SKILL.md (2025-01-27)

**Task**: Write a SKILL.md that teaches Copilot and Squad agents how to migrate Web Forms themes to Blazor using BWFC auto-discovery.

**Delivered**:
- Created `.squad/skills/theme-migration/SKILL.md` as authoritative reference for theme migration pattern
- Documented Web Forms theme structure (App_Themes/ folder with .skin, .css, images)
- Explained auto-discovery flow: copy → `AddBlazorWebFormsComponents()` → `SkinFileParser` → `ThemeProvider` injection
- Covered key concepts:
  - Theme folder identification and copy operation (preserve structure)
  - Default theme selection (first folder alphabetically)
  - CSS auto-discovery and injection via ThemeProvider
  - Named skins (SkinID parameter requirement in Blazor)
  - ThemeMode (StyleSheetTheme default vs. Theme override mode)
  - Multiple themes support and custom ThemesPath configuration
- Provided 3 detailed examples (simple, multiple themes, named skins)
- Documented edge cases (no themes, CSS-only themes, custom paths)
- Included anti-patterns with do's and don'ts (manual registration, missing ThemeProvider, image handling)

**Why This Matters**: This SKILL.md replaces the need for agent-specific migration scripts. Any agent (Copilot, Cyclops, Rogue, or future Squad members) doing Web Forms migration now has a standardized reference explaining the theme pattern. No more tribal knowledge — the pattern is documented, discoverable, and reusable across projects.

**Key Principle**: The SKILL teaches the "what" and "why" but delegates implementation details (SkinFileParser internals, ThemeProvider rendering) to library code. This keeps the SKILL maintainable as the implementation evolves.

### Migration Automation Audit (2026-07-25)

**Task**: Audit bwfc-migrate.ps1 (2,714 lines), all BWFC shims, and migration test results to identify manual gaps and propose automation solutions.

**Key Findings**:
- L1 script handles ~60% of migration mechanically (22 categories of transforms)
- BWFC library provides 20+ shims (WebFormsPageBase, ResponseShim, RequestShim, ViewStateDictionary, Handler framework, Middleware, Identity stubs, EF6 stubs)
- Identified 23 automation gaps with proposed solutions
- 9 quick wins shippable in single PRs (ConfigurationManager shim, Web.config→appsettings.json, IsPostBack unwrap, App_Themes auto-copy, BundleConfig stubs, etc.)
- Top 3 "sneaky wins": ConfigurationManager.AppSettings shim, Session["key"] shim, Web.config→appsettings.json extraction
- Missing shims: ConfigurationManager, HttpContext.Current, FormsAuthentication, Session (on WebFormsPageBase), BundleConfig/RouteConfig, Server.MapPath (outside handlers)
- Missing script transforms: Page_Load→OnInitializedAsync, event handler signatures, Bind() expressions, Global.asax extraction, Startup.cs/OWIN parsing
- WingtipToys Run 7: 14/14 tests pass, 366 transforms, 55 L2 files touched
- ContosoUniversity Run 22: 39/40 tests pass, EDMX parser handles all models

**Deliverable**: `dev-docs/migration-automation-opportunities.md` — structured report with gap inventory, proposed solutions, complexity estimates, and implementation order across 3 phases.
