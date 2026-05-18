---
name: "semantic-pattern-guardrails"
description: "Guardrails for implementing Web Forms semantic page-pattern rewrites in the migration CLI"
domain: "migration-architecture"
confidence: "high"
source: "manual"
---

## Context

Use this when adding semantic catalog entries to the Web Forms migration CLI. These rules apply to cross-file page-shape rewrites where the mechanical transforms have already converted raw syntax and the remaining work is contract normalization.

## Patterns

### Master/content normalization belongs in the semantic pass

- Let the markup transforms do the local tag conversion first.
- Then normalize generated master/page pairs to the BWFC-tested contract:
  - master shell markup → `ChildContent`
  - migrated `<Content ContentPlaceHolderID="...">` blocks → `ChildComponents`
  - preserve exact placeholder IDs, including head slots

### Query/detail pages must stay read-only

Safe matches:
- `SelectMethod`/code-behind only filters by `[QueryString]` / `[RouteData]`
- no writes, redirects, form access, auth calls, or postback branches
- one clear data-display page shape

Stop and emit TODO/manual review if:
- the page mutates state
- the page mixes route/query entry points in a way you cannot preserve exactly
- the code-behind depends on postback/event sequencing

### Action pages must still act

Safe matches:
- empty or scaffold-only page
- action happens on load from deterministic query inputs
- one redirect target
- no auth cookies, no POST body dependency, no external provider workflow

Anti-pattern:
- converting an action page into a visible informational page that does nothing

### Account pages are high-risk and mostly manual

Treat these as manual/TODO boundaries unless you have explicit HTTP endpoint/handler output:
- login/logout
- register
- password reset token issuance/consumption
- external auth challenge/callback
- 2FA provider selection and verification
- add/remove external logins
- phone / security settings that resignal identity state

Why:
- Web Forms account pages depend on live HTTP response semantics and OWIN/Identity middleware
- a Blazor component event handler cannot safely fake that contract

## Minimum valid output contract

**Master shell**

```razor
<MasterPage>
    <Head>...</Head>
    <ChildContent>
        ...chrome...
        <ContentPlaceHolder ID="MainContent" />
    </ChildContent>
    <ChildComponents>
        @ChildComponents
    </ChildComponents>
</MasterPage>
```

**Child page**

```razor
<Site>
    <ChildComponents>
        <Content ContentPlaceHolderID="MainContent">
            ...
        </Content>
    </ChildComponents>
</Site>
```

## Anti-Patterns

- Rewriting multi-placeholder masters to `@Body`
- Dropping `ContentPlaceHolderID` names
- Pretending auth pages are fully migrated when only the shell was understood
- Converting redirect-only action pages into dead-end content pages
- Collapsing query/route precedence to a single hard-coded `id` path
