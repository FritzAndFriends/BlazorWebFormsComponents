# Decision: Run 37 gap fixes G1 / G2 / G4 for the CLI pipeline

**Date:** 2026-05-06  
**Author:** Bishop (Migration Tooling Dev)  
**Status:** Implemented & Tested

---

## Context

WingtipToys Run 37 still exposed three deterministic CLI gaps:

- **G1** — `DisplayExpressionTransform` intentionally skipped `String.Format(...)`, leaving raw `<%#:` blocks in generated Razor.
- **G2** — master-page output still carried Web Forms-only script infrastructure (`ScriptManager`, `webopt:bundlereference`, `Scripts.Render(...)`) that has no Blazor equivalent.
- **G4** — large Account/Admin/infrastructure-heavy routable pages still blocked generated-app builds because their code-behind referenced ASP.NET Identity, OWIN, OpenAuth, or external payment APIs.

---

## Decision 1 — G1 display-expression coverage

`DisplayExpressionTransform` now excludes only `Bind(...)` and `Eval(...)` in its negative lookahead.

### Why
`Bind` and `Eval` still need specialized downstream handling, but `String.Format(...)` is just a normal complex expression. Letting it flow through the display-expression transform yields valid Razor `@(String.Format(...))` and prevents raw Web Forms syntax from leaking into `.razor` output.

### Coverage
Focused tests now verify both simple and nested `String.Format(...)` cases, while ensuring `Bind(...)` and `Eval(...)` remain untouched.

---

## Decision 2 — G2 ScriptManager strip pass (Order 255)

Added `ScriptManagerStripTransform` immediately after `MasterPageTransform`.

### What it removes
- `<asp:ScriptManager ...>...</asp:ScriptManager>` blocks
- `<webopt:bundlereference ... />`
- `<asp:PlaceHolder runat="server">` blocks whose only content is `Scripts.Render(...)`

### What it leaves behind
The first stripped ScriptManager block is replaced with:

```razor
@* Framework scripts are managed by Blazor — no ScriptManager needed. *@
```

Blank-line cleanup then collapses any `\n\n\n+` sequences back to a single empty line.

### Why
This infrastructure belongs to the old Web Forms request/update pipeline. Leaving it in generated master pages creates invalid output and distracts Layer 2 work with shell-level noise that should have been removed deterministically.

---

## Decision 3 — G4 compile-surface page stubs (Order 850)

Added `CompileSurfaceStubTransform` plus planner/pipeline support for dual output.

### Detection heuristic
A page is stubbed when it is **not** `Login.aspx` or `Register.aspx` and either:
- lives under `Account\` or `Admin\`, or
- still references ASP.NET Identity, OWIN, `OpenAuthProviders`, or payment-service namespaces such as PayPal/Stripe.

### Output contract
For detected pages:
- markup becomes a visible stub page with the original route and an "under migration" message
- compile-surface code-behind becomes a minimal `ComponentBase` partial class
- the transformed pre-stub code-behind is preserved under `migration-artifacts\codebehind\...`
- the pipeline records a `bwfc-compile-surface` manual item so downstream tooling knows this page was intentionally parked behind a safe stub

### Why dual output matters
A pure quarantine keeps the build clean but removes the route entirely. A pure stub keeps the route but loses the migration context. Emitting both gives us a build-safe app **and** a preserved artifact that Layer 2 can revisit later.

---

## Registration / docs impact

Updated both production and test registrations:
- `src\BlazorWebFormsComponents.Cli\Program.cs`
- `tests\BlazorWebFormsComponents.Cli.Tests\TestHelpers.cs`

Updated CLI docs:
- `docs\cli\index.md`
- `docs\cli\transforms.md`

---

## Validation

Ran the full CLI suite after each fix:

1. Baseline — `573` passing
2. After G1 — `577` passing
3. After G2 — `582` passing
4. After G4/final docs/tests — `588` passing

Command used each time:

```powershell
dotnet test tests\BlazorWebFormsComponents.Cli.Tests --no-restore --nologo
```
