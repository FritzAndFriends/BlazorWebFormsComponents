# Migration Toolkit Package: Design Document

**Author:** Forge (Lead / Web Forms Reviewer)
**Date:** 2026-03-03
**Status:** Design — ready for Beast to author content
**Requested by:** Jeffrey T. Fritz

---

## Purpose

Package the institutional knowledge built during the WingtipToys migration proof-of-concept into a **portable, self-contained toolkit** that another engineering team (developer + GitHub Copilot) can use to migrate their own ASP.NET Web Forms application to Blazor using BlazorWebFormsComponents.

The toolkit is the bridge between "here's a component library" and "here's how to use it to migrate your app."

---

## Audience

**Primary:** A .NET developer who owns a Web Forms application and wants to migrate it to Blazor. They have GitHub Copilot available and are willing to use it. They may not know Blazor deeply but know Web Forms well.

**Secondary:** A GitHub Copilot instance that a developer points at their Web Forms codebase with the instruction "migrate this to Blazor using BWFC." The toolkit provides the rules, patterns, and workflow for Copilot to follow.

**Usage model:**
1. Developer clones/installs BWFC and reads the toolkit README
2. Developer runs the scanner against their Web Forms project
3. Developer runs the migration script (Layer 1)
4. Developer opens the result in their editor with Copilot, pointing Copilot at the skill file
5. Copilot applies Layer 2 structural transforms using the skill
6. Developer + Copilot work through Layer 3 architecture decisions using the agent and decision guides
7. Developer builds, tests, and iterates

---

## Recommended Location

**`/migration-toolkit/`** — top-level folder at the repository root.

### Rationale

| Option | Verdict | Why |
|--------|---------|-----|
| `/migration-toolkit/` | ✅ **Recommended** | Top-level visibility. Signals that migration is a first-class product, not an afterthought. A developer cloning the repo sees it immediately. Matches Jeff's reframing: "the final product is a migration acceleration system." |
| `/docs/migration/` | ❌ | Buries it inside MkDocs documentation for the component library. Migration users don't need component docs — they need a workflow. |
| `.github/copilot-migration/` | ❌ | Too hidden. The `.github/` folder is for repo configuration, not user-facing deliverables. |
| `/tools/migration/` | ❌ | Reasonable but less discoverable. "Toolkit" implies more than tools — it includes methodology, templates, and reference material. |

---

## Folder Structure

```
/migration-toolkit/
├── README.md                              # 1. Entry point & quickstart
├── METHODOLOGY.md                         # 2. Three-layer pipeline deep-dive
├── QUICKSTART.md                          # 3. Step-by-step: scan → migrate → verify
├── ARCHITECTURE-GUIDE.md                  # 4. Layer 3 decision guidance + templates
├── CONTROL-COVERAGE.md                    # 5. Full 52-component coverage table
├── CASE-STUDY.md                          # 6. WingtipToys story with metrics
├── FAQ.md                                 # 7. Common questions and gotchas
├── CHECKLIST.md                           # 8. Per-page migration checklist template
└── copilot-instructions-template.md       # 9. Drop-in template for migration projects
```

All scripts, skills, and agents referenced by the toolkit already exist in the repo. The toolkit documents reference them by relative path — it does NOT duplicate them.

---

## Content Inventory

### 1. `README.md` — Toolkit Overview & Entry Point

**Description:** The single document a developer reads first. Explains what the toolkit is, what it contains, prerequisites (BWFC NuGet package, .NET 8+, PowerShell 7+), and a 4-step quick overview: Scan → Transform → Guide → Verify. Links to every other document in the toolkit and to the existing scripts/skills/agent.

**Extract vs. Create:** Net-new. No existing document serves this role.

**Content outline:**
- What is this toolkit?
- Prerequisites
- Quick overview (4 steps with links)
- File map — what's in this folder and why
- Links to existing artifacts: `scripts/bwfc-scan.ps1`, `scripts/bwfc-migrate.ps1`, `.github/skills/webforms-migration/SKILL.md`, `.github/agents/migration.agent.md`

---

### 2. `METHODOLOGY.md` — Three-Layer Migration Pipeline

**Description:** Deep-dive into the three-layer pipeline architecture. Explains what each layer does, why it exists, what percentage of work it handles, and how the layers compose. Includes the pipeline diagram, input/output for each layer, and decision criteria for what falls into which layer.

**Extract vs. Create:** ~60% extracted from `planning-docs/WINGTIPTOYS-MIGRATION-EXECUTIVE-REPORT.md` (the pipeline section, time estimates, accuracy metrics). ~40% net-new (pipeline diagram, layer boundary definitions, when-to-use guidance for each layer).

**Extracted from executive report:**
- Layer breakdown table (Layer 1: ~40%, Layer 2: ~45%, Layer 3: ~15%)
- Layer 1 accuracy metrics (147+ tag removals, 165+ runat removals, 100% accuracy)
- Page readiness categories (12% markup-complete, 64% skill-handleable, 24% architecture-needed)
- Time estimates per layer

---

### 3. `QUICKSTART.md` — Step-by-Step Migration Guide

**Description:** A linear walkthrough that takes a developer from "I have a Web Forms app" to "I have a running Blazor app" in the shortest path. Numbered steps with commands to run. Not exhaustive — points to METHODOLOGY.md and ARCHITECTURE-GUIDE.md for depth. This is the "just tell me what to do" document.

**Extract vs. Create:** ~30% extracted from `.github/skills/webforms-migration/SKILL.md` (Steps 1-4: project creation, _Imports.razor, services registration, JS reference). ~70% net-new (scan step, Layer 1 execution, Layer 2 Copilot session setup, Layer 3 handoff, build/test verification loop).

**Content outline:**
1. Install BWFC: `dotnet add package Fritz.BlazorWebFormsComponents`
2. Scan your project: `.\scripts\bwfc-scan.ps1 -Path .\MyWebFormsApp -OutputFormat Markdown`
3. Review scan report — understand your readiness score
4. Run Layer 1: `.\scripts\bwfc-migrate.ps1 -Path .\MyWebFormsApp -Output .\MyBlazorApp`
5. Review migration summary — flag items needing attention
6. Open in editor, configure Copilot with the BWFC migration skill
7. Walk through Layer 2 transforms with Copilot
8. Address Layer 3 architecture decisions
9. Build, run, verify: `dotnet build && dotnet run`
10. Iterate on remaining issues

---

### 4. `ARCHITECTURE-GUIDE.md` — Layer 3 Decision Templates

**Description:** A catalog of the major architectural decisions that every Web Forms migration must make, with recommended patterns, trade-offs, and copy-paste templates. Each decision includes a "Web Forms way," "Blazor way," decision criteria, and implementation template. This is what makes Layer 3 repeatable instead of ad-hoc.

**Extract vs. Create:** ~40% extracted from `.github/agents/migration.agent.md` (decision frameworks for Session→DI, EditForm vs. plain forms, ViewState mapping, PostBack patterns). ~30% extracted from Forge's WingtipToys migration analysis in history.md (EF6→EF Core, Identity migration, PayPal/HTTP integrations, route mapping). ~30% net-new (decision record template, prioritization guidance, dependency mapping).

**Decision catalog to include:**
1. **Master Page → Layout** — structural mapping, `@Body`, `<HeadContent>`
2. **Session State → Scoped Services** — the hardest semantic transform
3. **ASP.NET Identity → Blazor Identity** — scaffold vs. migrate decision
4. **Entity Framework 6 → EF Core** — DbContext registration, `IDbContextFactory`
5. **Global.asax → Program.cs/Middleware** — lifecycle hook mapping
6. **Web.config → appsettings.json** — connection strings, custom config
7. **Data Source Controls → Service Injection** — SqlDataSource, ObjectDataSource elimination
8. **Route Table → @page Directives** — URL preservation strategy
9. **HTTP Handlers/Modules → Middleware** — IHttpHandler, IHttpModule conversion
10. **Third-Party Integrations** — payment APIs, external services (HttpClient pattern)

**Each decision includes:**
- What it is and why it matters
- Web Forms pattern → Blazor pattern mapping
- Recommended approach with rationale
- Code template (before/after)
- Gotchas and edge cases

---

### 5. `CONTROL-COVERAGE.md` — Component Coverage Reference

**Description:** A complete table of all 52 BWFC components organized by category, with migration complexity rating (trivial/easy/medium/complex), common gotchas per control, and a note about what's NOT supported. This is the "can I migrate this control?" quick-reference.

**Extract vs. Create:** ~70% extracted from `.github/skills/webforms-migration/SKILL.md` (the Control Translation Table sections — Simple Controls, Form Controls, Validation Controls, Data Controls, Navigation Controls, Login Controls). ~30% net-new (complexity ratings, gotchas column, unsupported-feature notes per control, the "not supported" controls section for DataSource controls).

**Table structure:**

| Control | Category | BWFC? | Complexity | Key Changes | Gotchas |
|---------|----------|-------|------------|-------------|---------|
| Button | Editor | ✅ | Trivial | Remove `asp:`, `runat` | `OnClick` is now `EventCallback` |
| GridView | Data | ✅ | Medium | `ItemType`→`TItem`, `SelectMethod`→`Items` | Add `Context="Item"` to templates |
| SqlDataSource | Data | ❌ | N/A | Replace with service injection | No BWFC equivalent — use DI |

---

### 6. `CASE-STUDY.md` — WingtipToys Migration Story

**Description:** The concrete, metrics-backed story of migrating WingtipToys. Not a copy of the executive report — a practitioner-focused retelling: what we did, in what order, what went wrong, what we'd do differently. Includes the before/after screenshots, time breakdowns, and lessons learned. This is the "proof that it works" document.

**Extract vs. Create:** ~50% extracted from `planning-docs/WINGTIPTOYS-MIGRATION-EXECUTIVE-REPORT.md` (metrics tables, screenshot references, actual timeline, estimated-vs-actual comparison, CSS fidelity analysis). ~20% extracted from Forge's history.md (component gap discoveries, the FormView RenderOuterTable fix, the BoundField DataFormatString bug, the 7 CSS fidelity differences). ~30% net-new (lessons-learned narrative, "what we'd do differently" section, recommendations for other teams).

**Key metrics to include:**
- 33 files, 230+ control instances, 29 control types
- 96.6% BWFC coverage (28/29 controls)
- Layer 1: 30 seconds, 100% accuracy
- Total: ~4.5 hours with AI agents, estimated 18-26 hours for solo developer
- 55-70% time reduction vs. manual rewrite
- 7 visual differences found and resolved (6 migration omissions, 1 library bug)

---

### 7. `FAQ.md` — Common Questions and Gotchas

**Description:** Answers to the questions that come up during every migration. Organized as a flat list of Q&A pairs. Drawn from the "Common Gotchas" section of the existing skill, plus lessons from WingtipToys.

**Extract vs. Create:** ~50% extracted from `.github/skills/webforms-migration/SKILL.md` (Common Gotchas section: No ViewState, No PostBack, No DataSource Controls, ID Rendering, Template Context Variable, runat on HTML elements, String Format, Visibility Pattern, Nested Master Pages). ~20% extracted from WingtipToys CSS fidelity analysis (wrong CSS framework, missing static files, GroupItemCount omission). ~30% net-new (questions about Copilot workflow, BWFC version compatibility, when to NOT use BWFC, partial migration strategies).

**Sample questions:**
- "My page uses SqlDataSource — what do I do?"
- "How do I handle Session state?"
- "Can I migrate one page at a time?"
- "What if a control I need isn't in BWFC?"
- "My CSS broke after migration — why?"
- "How do I handle postback-dependent logic?"
- "Should I use `@code` blocks or code-behind files?"

---

### 8. `CHECKLIST.md` — Per-Page Migration Checklist Template

**Description:** A copy-paste checklist that a developer uses for each page they migrate. Tracks the status of each migration step from "identify controls" through "verify visual fidelity." Designed to be used as a GitHub issue template or a markdown checklist in a tracking document.

**Extract vs. Create:** Fully net-new. No existing document provides a per-page checklist. Structure derived from the three-layer pipeline steps.

**Checklist structure:**
```markdown
## Page: [PageName.aspx] → [PageName.razor]

### Layer 1 — Automated
- [ ] File renamed (.aspx → .razor)
- [ ] Directives converted (@page added, <%@ Page %> removed)
- [ ] asp: prefixes removed
- [ ] runat="server" removed
- [ ] Expressions converted (<%: %> → @())
- [ ] URL references converted (~/ → /)
- [ ] Code-behind copied with TODO annotations

### Layer 2 — Copilot-Assisted
- [ ] SelectMethod → Items binding wired
- [ ] ItemType → TItem converted
- [ ] Data loading moved to OnInitializedAsync
- [ ] Event handlers converted to Blazor signatures
- [ ] Template Context variables added
- [ ] Form wrapper converted (EditForm or removed)
- [ ] Navigation calls converted (Response.Redirect → NavigateTo)

### Layer 3 — Architecture Decisions
- [ ] Data access pattern decided (service injection)
- [ ] State management approach decided
- [ ] Authentication/authorization wired
- [ ] Third-party integrations ported
- [ ] Route registered (@page directive correct)

### Verification
- [ ] Page builds without errors
- [ ] Page renders in browser
- [ ] Visual comparison against original
- [ ] Interactive features work (forms, buttons, navigation)
- [ ] No console errors
```

---

### 9. `copilot-instructions-template.md` — Drop-In Copilot Instructions

**Description:** A template file that developers copy into their own project's `.github/copilot-instructions.md` to give Copilot migration-specific context. This is the HIGHEST-VALUE deliverable for the "hand it to Copilot" use case. It's a condensed, actionable version of the migration skill, tailored for a specific project.

**Extract vs. Create:** ~60% extracted from `.github/skills/webforms-migration/SKILL.md` (core transformation rules, control translation table, expression conversion table, code-behind migration patterns). ~40% net-new (project-specific placeholder sections for the developer to fill in: their control inventory, their routing table, their service registration needs, their authentication approach).

**Template structure:**
```markdown
# Migration Instructions for [Project Name]

## Project Context
<!-- Fill in: your app's page count, key controls used, data access pattern -->

## BWFC Migration Rules
<!-- Condensed from the migration skill — the mechanical rules -->

## Your Application's Control Map
<!-- Fill in from bwfc-scan.ps1 output -->

## Architecture Decisions Made
<!-- Fill in as you make Layer 3 decisions -->

## Routing Table
<!-- Map old .aspx routes to new @page directives -->
```

---

## Existing Artifacts Referenced (Not Duplicated)

The toolkit documents reference these existing files by relative path. They are NOT copied into the toolkit folder:

| Artifact | Location | Role in Toolkit |
|----------|----------|-----------------|
| **Scanner script** | `scripts/bwfc-scan.ps1` | Layer 0: Assessment. Produces readiness report. |
| **Migration script** | `scripts/bwfc-migrate.ps1` | Layer 1: Mechanical transforms. Processes all .aspx/.ascx/.master files. |
| **Copilot migration skill** | `.github/skills/webforms-migration/SKILL.md` | Layer 2: Structural transforms. Teaches Copilot the BWFC control mappings and code-behind patterns. |
| **Migration agent** | `.github/agents/migration.agent.md` | Layer 3: Architecture decisions. Interactive assistant for semantic migration choices. |
| **WingtipToys (before)** | `samples/WingtipToys/` | Reference: Original Web Forms application (33 files). |
| **WingtipToys (after)** | `samples/AfterWingtipToys/` | Reference: Migrated Blazor application. |
| **Executive report** | `planning-docs/WINGTIPTOYS-MIGRATION-EXECUTIVE-REPORT.md` | Source material for CASE-STUDY.md (metrics, screenshots, timelines). |
| **HTML output matching guide** | `.github/skills/component-development/HTML_OUTPUT_MATCHING.md` | Reference for developers who need to verify HTML fidelity. |

---

## Extract vs. Create Summary

| Toolkit Document | % Extracted | Primary Sources | % Net-New |
|-----------------|-------------|-----------------|-----------|
| README.md | 0% | — | 100% |
| METHODOLOGY.md | 60% | Executive report | 40% |
| QUICKSTART.md | 30% | Migration skill (SKILL.md) | 70% |
| ARCHITECTURE-GUIDE.md | 70% | Migration agent + Forge's analysis | 30% |
| CONTROL-COVERAGE.md | 70% | Migration skill (SKILL.md) | 30% |
| CASE-STUDY.md | 70% | Executive report + Forge's history | 30% |
| FAQ.md | 50% | Migration skill (gotchas) + CSS audit | 50% |
| CHECKLIST.md | 0% | — | 100% |
| copilot-instructions-template.md | 60% | Migration skill (SKILL.md) | 40% |

**Total: ~5 documents are primarily extraction/adaptation, ~4 are primarily net-new creation.**

---

## Content Authoring Notes for Beast

### Tone & Voice
- **Practitioner-focused.** Write for someone who's done this before (Web Forms) and is learning something new (Blazor + BWFC). Don't condescend.
- **Concrete over abstract.** Every concept gets a code example. Every recommendation gets a "before → after."
- **Honest about limitations.** Call out what BWFC doesn't cover (DataSource controls, Wizard, AJAX Toolkit extenders). Don't oversell.

### Cross-References
- Every document should link to the other toolkit documents it relates to.
- Every document should link to the specific existing artifacts (scripts, skills, agent) it references.
- Use relative paths from the toolkit folder: `../scripts/bwfc-scan.ps1`, `../.github/skills/webforms-migration/SKILL.md`.

### No Duplication of Existing Content
- The toolkit REFERENCES the migration skill — it does not copy the full control translation table into multiple documents.
- CONTROL-COVERAGE.md is the ONE place the full table lives in the toolkit. Other docs link to it.
- QUICKSTART.md gives the commands; METHODOLOGY.md explains why those commands exist.

### Screenshots
- CASE-STUDY.md should reference the existing screenshots in `planning-docs/screenshots/` (the before/after comparisons from the executive report).
- Do NOT create new screenshots — the existing ones are from live running applications and are authoritative.

---

## Open Questions for Jeff

1. **NuGet packaging:** Should the toolkit eventually ship as part of the BWFC NuGet package (as content files), or remain repo-only?
2. **Versioning:** Should the toolkit be versioned independently of the BWFC library, or tied to library releases?
3. **Community contributions:** Should we accept PRs to the toolkit (e.g., case studies from other migrations)?
4. **MkDocs integration:** Should the toolkit be published to the documentation site, or remain as markdown in the repo?

---

## Implementation Priority

If Beast needs to batch this work, here's the priority order:

1. **README.md** — entry point; everything else depends on knowing this exists
2. **QUICKSTART.md** — the "just do it" path; highest developer value
3. **CONTROL-COVERAGE.md** — the "can I use this?" reference; answers the first question every developer asks
4. **METHODOLOGY.md** — explains the pipeline; needed before the deeper guides
5. **CHECKLIST.md** — per-page tracking; immediately useful during migration
6. **copilot-instructions-template.md** — the Copilot handoff; highest AI-assisted value
7. **ARCHITECTURE-GUIDE.md** — Layer 3 depth; complex but critical
8. **FAQ.md** — accumulated wisdom; grows over time
9. **CASE-STUDY.md** — proof point; important but not blocking
