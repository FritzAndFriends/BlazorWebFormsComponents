# Psylocke — History

## 2025-07-25: Updated migration-toolkit skills for Phase 1 capabilities

**Task:** Update SKILL.md files so end-users get guidance on new Phase 1 "Just Make It Compile" shims and script capabilities.

**Files modified:**
- `migration-toolkit/skills/bwfc-migration/SKILL.md` — Added "Phase 1 Compile-Compatibility Shims" section (ConfigurationManager shim, BundleConfig/RouteConfig stubs, IsPostBack guard unwrapping, .aspx URL cleanup). Updated Layer 1 capability list, Installation section (added `UseConfigurationManagerShim()`), Common Gotchas (IsPostBack), and Per-Page Migration Checklist.
- `migration-toolkit/skills/bwfc-migration/CODE-TRANSFORMS.md` — Expanded Lifecycle Methods section with L1 auto-unwrap details and before/after examples. Added "IsPostBack Guard Handling (L1 Automated)" subsection. Added ".aspx URL Cleanup (L1 Automated)" subsection after Navigation.
- `migration-toolkit/skills/migration-standards/SKILL.md` — Added "Compile-Compatibility Shims" section (table of all shims, ConfigurationManager setup, appsettings.json mapping). Updated Layer 1 script capability list with IsPostBack unwrapping, .aspx URL cleanup, and using retention. Updated Page Lifecycle Mapping table for IsPostBack.

**Approach:** Read each implementation (ConfigurationManager.cs, BundleConfig.cs, RouteConfig.cs, Remove-IsPostBackGuards function, GAP-20 .aspx URL cleanup) to document actual APIs accurately. Matched existing formatting style in each file.

📌 Team update (2026-04-12): Migration toolkit enhancement complete — 3 CLI transforms added (ConfigurationManager, RequestForm, ServerShim), 373/373 tests passing, WingtipToys gap analysis shows 31 pages can inherit WebFormsPageBase. Decided by Psylocke, Forge, Bishop.

## 2025-07-30: Comprehensive shim coverage update across all migration skills

**Task:** Update all migration-related Copilot skills to reference new shims and migration-assisting features built since Phase 1.

**Files modified:**
- `migration-toolkit/skills/bwfc-migration/SKILL.md` — Expanded shims table (8→11 entries), added 3 new sections (WebFormsForm Component, ClientScript Migration, PostBack Event Handling), updated gotchas (PostBack Compatibility, ScriptManagerShim), fixed `AddSessionShim()` → auto-registered, expanded WebFormsPageBase property list.
- `migration-toolkit/skills/bwfc-migration/CODE-TRANSFORMS.md` — Updated Navigation table: Response.Redirect works AS-IS via ResponseShim.
- `migration-toolkit/skills/migration-standards/SKILL.md` — Expanded page base class properties (5→17), shims table (3→11), lifecycle mapping (+8 new rows), updated Session section, removed incorrect "NOT provided" items.
- `migration-toolkit/skills/bwfc-data-migration/SKILL.md` — Fixed ConfigurationManager gotcha, added SessionShim as Phase 1 bridge with typed API.
- `migration-toolkit/CONTROL-COVERAGE.md` — Fixed SelectMethod contradiction: `SelectMethod → Items` changed to `SelectMethod → delegate or Items`, added warning, rewrote Data Control Migration Pattern section.
- `.github/copilot-instructions.md` — Replaced "No Postback" with "PostBack Compatibility", added Migration Shims section (10 shims).
- `migration-toolkit/copilot-instructions-template.md` — Expanded code-behind table (6→12 entries), updated data binding guidance, updated gotchas.
- `.squad/skills/migration-standards/SKILL.md` — Updated page properties table and lifecycle mapping to match new shim capabilities.

**Key findings:**
- SelectMethod contradiction: CONTROL-COVERAGE.md said `→ Items` but BWFC has native `SelectMethod` parameter on `DataBoundComponent<T>`. Fixed to prefer delegate conversion.
- `AddSessionShim()` was referenced in multiple skills but does not exist — all shims are auto-registered by `AddBlazorWebFormsComponents()`.
- `IsPostBack` is no longer "always false" — SSR mode checks HTTP method, Interactive mode tracks render count.
- PostBack support exists despite earlier team decision saying "Do NOT attempt __doPostBack emulation" (decision was subsequently overridden by implementation).
- 10 shims now available: FormShim, ClientScriptShim, ScriptManagerShim, RequestShim, ResponseShim, SessionShim, CacheShim, ServerShim, ViewStateDictionary, WebFormsPageBase.
