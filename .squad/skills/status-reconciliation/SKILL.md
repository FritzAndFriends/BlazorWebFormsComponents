# SKILL: Status Reconciliation

**Confidence:** low
**Source:** earned

## When to Use
When planning a sprint or auditing project status, and the status tracking document may be stale relative to the actual codebase state.

## Pattern

1. **Read the status document** to understand what it claims.
2. **Verify claims against the filesystem** — check if files marked "Not Started" actually exist on the current branch. Use `glob` to search for `.razor`, `.cs`, or other source files.
3. **Verify claims against git history** — use `git log --oneline -- <path>` to confirm when files were added and which PRs merged them.
4. **Check downstream artifacts** — docs, sample pages, tests, nav entries. A component isn't truly "Complete" without these per team policy.
5. **Reconcile counts** — summary tables often drift from detailed breakdowns. Count the actual ✅ entries and compare to the summary. Fix both to match reality.
6. **Watch for grouping mismatches** — components like MultiView/View may be listed as separate rows but counted as one logical component in totals.

## Common Traps

- **Merge-to-dev without status update:** PRs merge but nobody updates the tracking doc. Always check git log for recent merges.
- **Summary vs detail drift:** Summary table says "20 complete" but counting ✅ rows gives 23. One gets updated, the other doesn't.
- **Section headers vs tables:** A section header says "18/27" but the summary table says "20/27" and the actual count is "23/27". Three numbers, three different values.
- **Stale estimation tables:** Effort estimation sections list items as remaining work that are already complete. These sections are informational but misleading if not updated.

## Output
A corrected status document with consistent counts across summary, detail, and estimation sections.
