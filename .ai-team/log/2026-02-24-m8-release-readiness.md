# Session: M8 Release Readiness

**Date:** 2026-02-24
**Requested by:** Jeffrey T. Fritz
**Milestone:** 8 — Release Readiness

## Context

Milestone 8 kicked off after upstream/dev merge. Focus: bug fixes, formal deferrals, doc polish, and shared sub-component groundwork.

## Work Done

### Cyclops — 3 bug fixes
- **Menu JS interop crash:** Added null guard + try/catch in `Menu.js` to prevent `TypeError` when element not found
- **Calendar attribute rendering:** Fixed conditional `scope` attribute to render properly
- **Menu auto-ID generation:** Implemented hash-based ID (`menu_{GetHashCode():x}`) in `OnParametersSet` when no explicit ID provided

### Beast — deferrals + doc polish
- Formally deferred Substitution and Xml controls (status.md + README updated)
- Chart documentation: removed "Phase 1" hedging language
- Fixed mkdocs.yml duplicate entry
- Corrected README links

## Status

- **Tests:** 1,206 passing, 0 failures
- **Build:** 0 errors
- **In progress:** PagerSettings shared sub-component

## Decisions

- Menu auto-ID pattern established (Cyclops)
- Substitution/Xml formally deferred (Beast)
- M8 scope excludes version bump and release (user directive)
