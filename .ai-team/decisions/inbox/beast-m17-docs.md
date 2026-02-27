# Beast — M17 AJAX Controls Documentation Decisions

**Date:** 2026-02-26
**By:** Beast

## 1. AJAX Controls get their own nav category

**What:** Created a new "AJAX Controls" section in mkdocs.yml and README.md, separate from "Editor Controls", containing Timer, ScriptManager, ScriptManagerProxy, UpdatePanel, UpdateProgress, and Substitution.

**Why:** These 6 controls are conceptually related (all part of the Web Forms AJAX/partial-rendering infrastructure) and grouping them together helps developers migrating AJAX-heavy pages find all relevant docs in one place. The doc files themselves live in `docs/EditorControls/` for filesystem consistency, but the nav groups them separately.

## 2. Migration stub doc pattern established

**What:** ScriptManager and ScriptManagerProxy docs use a `!!! warning "Migration Stub Only"` admonition at the top, document all accepted-but-ignored properties, and include explicit "include during migration → remove when stable" guidance.

**Why:** Future no-op migration compatibility components should follow this pattern so developers immediately understand the component renders nothing and is temporary scaffolding.

## 3. Substitution moved from deferred to implemented

**What:** Updated `docs/Migration/DeferredControls.md` to mark Substitution as ✅ Complete (was ❌ Deferred). Created full documentation at `docs/EditorControls/Substitution.md`.

**Why:** Substitution is now implemented as a component that renders callback output directly. The DeferredControls page and summary table needed updating to reflect this change.

## 4. UpdateProgress migration pattern: explicit state over automatic association

**What:** UpdateProgress docs recommend wrapping in `@if (isLoading)` with explicit boolean state management rather than relying on automatic UpdatePanel association (which doesn't exist in Blazor).

**Why:** This is the fundamental architectural difference developers need to understand. The before/after examples make this pattern concrete and copy-pasteable.
