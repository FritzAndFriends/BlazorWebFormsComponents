# Decision: Reframe Database Guidance — Detect and Match Original Provider

**Date:** 2025-07-24
**Author:** Beast (Technical Writer)
**Requested by:** Jeffrey T. Fritz
**Status:** Implemented

## Context

Skill files were updated to say "NEVER default to SQLite" after agents repeatedly substituted SQLite for the original SQL Server provider. While the prohibition was correct, Jeff observed that the framing was reactive — it told agents what NOT to do rather than what TO do.

## Decision

Reframe all database provider guidance across the three migration skill files to lead with the **positive action**: detect the original database provider from `Web.config` `<connectionStrings>` and install the matching EF Core package. The "NEVER substitute" language is retained as a guardrail, but it is no longer the lead message.

## Changes

1. **migration-standards/SKILL.md** — Main bullet changed from "NEVER substitute database providers" to "Detect and match the original database provider" with provider mapping examples and L1 `[DatabaseProvider]` review item reference.
2. **bwfc-data-migration/SKILL.md** — Added "Step 1: Detect the provider" blockquote above existing CRITICAL/NEVER warnings, referencing L1's `Find-DatabaseProvider` function.
3. **bwfc-migration/SKILL.md** — Added "Database provider" bullet to L2 checklist directing agents to verify L1-detected provider.

## Rationale

- Agents prioritize affirmative instructions over prohibitions
- "Detect and match" gives agents a clear workflow to follow
- Connects L2 guidance to L1 script's auto-detection output (`[DatabaseProvider]` review item)
- Existing NEVER/MUST guardrails preserved as backstops
