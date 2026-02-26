# Session: 2026-02-25 — CI Workflow Fixes

**Requested by:** Jeffrey T. Fritz
**Agent:** Forge

## What Happened

- Diagnosed 2 CI workflow files (`nuget.yml` and `deploy-server-side.yml`) failing due to invalid `secrets.*` usage in step-level `if:` conditions
- Fixed both workflows to use the env var indirection pattern: declare secret in `env:`, check via `env.VAR_NAME` in `if:`
- Cleaned up fork dev branch history — reset `origin/dev` to `upstream/dev` and cherry-picked only the 2 fix commits
- PR #372 updated with clean 2-commit diff

## Decisions

- `deploy-server-side.yml` now uses same env var pattern as `nuget.yml` for secret-gated steps
- Dev branch cleanup: `--force-with-lease` push to remove ~37 stale pre-squash commits from PR #372

## Outcome

PR #372 shows exactly 2 commits. CI workflows valid.
