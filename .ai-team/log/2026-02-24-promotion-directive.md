# Session: 2026-02-24 — Promotion Directive

**Requested by:** Jeffrey T. Fritz

## What happened

Jeffrey T. Fritz captured the upstream promotion flow directive establishing the canonical git workflow for the team:

1. Work on a feature branch locally
2. Push feature branch to personal fork (csharpfritz/BlazorWebFormsComponents)
3. PR from personal fork feature branch → upstream/dev (FritzAndFriends/BlazorWebFormsComponents:dev)
4. After merge, pull upstream/dev locally → push to personal repo dev branch
5. Release = PR from upstream/dev → upstream/main, with version tag

## Decisions

- Upstream promotion flow directive added to decisions.md

## Outcome

- Decision recorded and propagated to all agents
