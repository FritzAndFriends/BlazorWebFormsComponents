# Work Routing

How to decide who handles what.

## Routing Table

| Work Type | Route To | Examples |
|-----------|----------|----------|
| Component development | Cyclops | Build new Blazor components, implement Web Forms control equivalents, fix component bugs |
| Component completeness review | Forge | Review components against Web Forms originals, verify attribute parity, check HTML output fidelity |
| Architecture & scope | Forge | What to build next, trade-offs, decisions, Web Forms behavior research |
| Documentation | Beast | MkDocs docs, migration guides, component API docs, utility feature docs |
| Sample apps & demos | Jubilee | Sample pages, usage examples, demo scenarios, AfterBlazorServerSide samples |
| Testing & QA | Rogue | bUnit tests, edge cases, validation, quality |
| Integration testing | Colossus | Playwright integration tests, sample page verification, end-to-end testing |
| Code review | Forge | Review PRs, check quality, verify Web Forms compatibility |
| Session logging | Scribe | Automatic — never needs routing |

## Rules

1. **Eager by default** — spawn all agents who could usefully start work, including anticipatory downstream work.
2. **Scribe always runs** after substantial work, always as `mode: "background"`. Never blocks.
3. **Quick facts → coordinator answers directly.** Don't spawn an agent for "what port does the server run on?"
4. **When two agents could handle it**, pick the one whose domain is the primary concern.
5. **"Team, ..." → fan-out.** Spawn all relevant agents in parallel as `mode: "background"`.
6. **Anticipate downstream work.** If a component is being built, spawn Rogue for tests, Beast for docs, and Jubilee for samples simultaneously.
7. **Forge reviews all component work.** Before any component PR merges, Forge must review for Web Forms completeness.
