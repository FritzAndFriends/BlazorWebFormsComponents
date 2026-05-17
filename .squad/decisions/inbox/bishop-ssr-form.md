# Bishop SSR form contract transform

- **Date:** 2026-05-17T19:20:41-04:00
- **Owner:** Bishop

## Decision
The CLI should enforce the Blazor static SSR form contract in one late markup pass shared by both normal markup transforms and semantic-pattern output.

That pass must:
1. detect `<form method="post">`, `<EditForm>`, and `<WebFormsForm>` blocks,
2. add a deterministic filename-derived form name when one is missing (`@formname` for HTML/WebFormsForm, `FormName` for `EditForm`), and
3. inject `<AntiforgeryToken />` as the first child element when it is absent.

## Why
WingtipToys and ContosoUniversity were both paying the same Layer 2 tax on every rerun: forms looked correct in generated `.razor` files but were still missing the SSR postback contract required by Blazor. Centralizing the rule in the CLI removes that mechanical repair step, keeps semantic rewrites and standard transforms aligned, and makes generated form names stable enough to reason about in tests and benchmark diffs.
