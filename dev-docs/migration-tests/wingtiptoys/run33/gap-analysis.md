# Run 33 Gap Analysis

This note investigates toolkit gaps 3, 5, and 6 from `dev-docs/migration-tests/wingtiptoys/run33/report.md`.

## Gap 3 — Raw `<% %>` blocks survived in `Account/Manage.razor`

### Verdict
Yes, we have an existing transform in this area, but it only handles Web Forms **expression tags** (`<%:`, `<%=`, `<%#`, `<%-- --%>`). It does **not** handle Web Forms **statement/code-render blocks** like `<% if (...) { %>`, `<% } else { %>`, and `<% } %>`.

### Root cause
`ExpressionTransform` is the transform Jeff was probably thinking of. It converts comments and expression-style tags, but there is no regex for statement/control-flow blocks.

That leaves two failure modes:

1. **Active statement blocks are never matched** because `ExpressionTransform` only covers `<%: ... %>` and `<%= ... %>`.
2. **Commented-out statement blocks survive inside Razor comments** because the comment conversion preserves the inner raw `<% ... %>` text.

That exact second case is still visible in the current migrated file.

### Evidence
- Run 33 report flags the issue at `dev-docs/migration-tests/wingtiptoys/run33/report.md:122`.
- The manual repair is recorded at `dev-docs/migration-tests/wingtiptoys/run33/report.md:48`.
- Source page contains both commented and active Web Forms statement blocks:
  - `samples/WingtipToys/WingtipToys/Account/Manage.aspx:39-52`
  - `samples/WingtipToys/WingtipToys/Account/Manage.aspx:61-74`
- Current migrated file still contains raw statement tags inside a Razor comment:
  - `samples/AfterWingtipToys/Account/Manage.razor:38-52`
  - raw tags remain at `samples/AfterWingtipToys/Account/Manage.razor:39`, `:44`, and `:52`
- Existing transform coverage stops at expressions/comments:
  - `src/BlazorWebFormsComponents.Cli/Transforms/Markup/ExpressionTransform.cs:15-16` converts `<%-- ... --%>` comments
  - `src/BlazorWebFormsComponents.Cli/Transforms/Markup/ExpressionTransform.cs:63-70` converts `<%: ... %>` and `<%= ... %>`
  - there is **no** handling for `<% if`, `<% } else { %>`, or `<% } %>` anywhere in `src/BlazorWebFormsComponents.Cli/Transforms/`

### Why it did not fire during Run 33
The transform did run, but the markup never matched any of its regexes. `<% if (...) { %>` is a statement block, not an expression block. For the phone-number section, the outer `<%-- ... --%>` comment matched first and got rewritten to `@* ... *@`, but the inner `<% if ... %>` / `<% } %>` text was carried through unchanged.

### Proposed fix
Extend `ExpressionTransform` (or add a dedicated `ServerCodeBlockTransform` immediately after it) to handle statement blocks and to sanitize raw server tags that appear inside converted comments.

Specific implementation shape:

1. Add regexes for common Web Forms control-flow forms:
   - `<% if (expr) { %>` -> `@if (expr)\n{`
   - `<% } else { %>` -> `}\nelse\n{`
   - `<% } %>` -> `}`
   - optionally also `foreach`, `for`, `while`, `switch`
2. Change comment conversion to use a replacement callback instead of a plain replacement string, and inside the callback either:
   - recursively convert nested `<% ... %>` blocks before wrapping in `@* *@`, or
   - strip/neutralize raw `<%` delimiters inside comment bodies so Razor never sees them.
3. Add regression coverage using `Manage.aspx`-style input with both:
   - active `if/else` blocks
   - commented-out `if/else` blocks nested inside `<%-- ... --%>`

That fix would make Run 33’s `Manage.aspx` pattern convert cleanly instead of requiring manual cleanup.

---

## Gap 5 — `ItemStyle` needed wrapping in `<ChildComponents>` under `TemplateField`

### Verdict
No existing transform currently handles this shape. We do have existing **ChildComponents-related** logic, but it is for **master/content page shells**, not for nested control children such as `TemplateField` styles.

### Root cause
The markup pipeline converts `asp:` tags and normalizes attributes, but it never rewrites the structure of a `TemplateField` to move style children into `<ChildComponents>`.

So the pipeline can get from this:

```aspx
<asp:TemplateField>
    <ItemTemplate>...</ItemTemplate>
    <ItemStyle HorizontalAlign="Left" />
</asp:TemplateField>
```

…to this shape:

```razor
<TemplateField ...>
    <ItemTemplate>...</ItemTemplate>
    <ItemStyle HorizontalAlign="@HorizontalAlign.Left" />
</TemplateField>
```

…but there is no transform that performs the final BWFC-specific wrapper step.

### Evidence
- Run 33 report flags the issue at `dev-docs/migration-tests/wingtiptoys/run33/report.md:126`.
- The manual repair is recorded at `dev-docs/migration-tests/wingtiptoys/run33/report.md:50`.
- Source markup has bare `ItemStyle` directly inside `TemplateField`:
  - `samples/WingtipToys/WingtipToys/Checkout/CheckoutReview.aspx:16-34`
  - specific style line: `samples/WingtipToys/WingtipToys/Checkout/CheckoutReview.aspx:33`
- Current migrated file shows the manually-corrected target structure:
  - `samples/AfterWingtipToys/Checkout/CheckoutReview.razor:19-39`
  - wrapper present at `samples/AfterWingtipToys/Checkout/CheckoutReview.razor:36-38`
- Existing transforms only do tag/attribute normalization:
  - `src/BlazorWebFormsComponents.Cli/Transforms/Markup/AspPrefixTransform.cs:15-43` strips `asp:` prefixes
  - `src/BlazorWebFormsComponents.Cli/Transforms/Markup/AttributeStripTransform.cs:32-60` adds generic `ItemType="object"` fallback, including `TemplateField`
  - `src/BlazorWebFormsComponents.Cli/Transforms/Markup/AttributeNormalizeTransform.cs:30-79` normalizes enum attributes like `HorizontalAlign`
- Existing ChildComponents semantic handling is for page wrappers only:
  - `src/BlazorWebFormsComponents.Cli/SemanticPatterns/MasterContentContractsSemanticPattern.cs:24-30` only matches page wrappers with named `<Content>` blocks
  - `src/BlazorWebFormsComponents.Cli/SemanticPatterns/SemanticPatternMarkupHelpers.cs:53-76` wraps page-level named regions into `<ChildComponents>`
- BWFC components expose `ChildComponents` as the container for extra nested children:
  - `src/BlazorWebFormsComponents/BaseWebFormsComponent.cs:333-335`

### Why it did not fire during Run 33
There was nothing to fire. The current pipeline has transforms for tag conversion and page-level ChildComponents grouping, but no transform that recognizes `TemplateField`/style-child patterns.

### Proposed fix
Add a new markup transform dedicated to data-field style children, for example:

- file: `src/BlazorWebFormsComponents.Cli/Transforms/Markup/TemplateFieldChildComponentsTransform.cs`
- order: after `AspPrefixTransform` (610) and before `AttributeNormalizeTransform` (810); `620` is a good fit

Behavior:

1. Match `<TemplateField ...> ... </TemplateField>` blocks.
2. Inside each block, find direct child style tags such as:
   - `<ItemStyle ... />`
   - `<HeaderStyle ... />`
   - `<FooterStyle ... />`
   - optionally `<ControlStyle ... />`
3. If those tags are not already inside `<ChildComponents>`, remove them from the direct child list and wrap them as:

```razor
<ChildComponents>
    <ItemStyle ... />
    <HeaderStyle ... />
</ChildComponents>
```

4. Preserve `ItemTemplate`, `EditItemTemplate`, and other render fragments in place.
5. Add regression tests using the exact `CheckoutReview.aspx` pattern.

That would turn the current manual fix into deterministic L1 output.

---

## Gap 6 — BWFC `Button` renders as `<input type="submit">`

### Verdict
This is **not** a missed transform. It is current BWFC component behavior, and it is intentional in the code. In fact, it aligns with classic Web Forms button rendering when `UseSubmitBehavior` is `true` (the default).

### Root cause
`Button` is implemented to render an HTML `<input>` element, not a `<button>` element.

`UseSubmitBehavior` only changes the **type** (`submit` vs `button`); it does not change the **tag name**.

So the selector conflict in Run 33 was caused by the component’s chosen HTML contract, not by the migration pipeline failing to transform something.

### Evidence
- Run 33 report flags the issue at `dev-docs/migration-tests/wingtiptoys/run33/report.md:128` and describes the failing selector at `:85`.
- Current component markup renders an `<input>` tag:
  - `src/BlazorWebFormsComponents/Button.razor:3-15`
  - tag line is `src/BlazorWebFormsComponents/Button.razor:5`
- Current component logic only toggles the `type` attribute:
  - `src/BlazorWebFormsComponents/Button.razor.cs:8`
  - `src/BlazorWebFormsComponents/Button.razor.cs:15-16`
- Current tests lock in `<input type="submit">` rendering:
  - `src/BlazorWebFormsComponents.Test/Button/Format.razor:5-11`
  - `src/BlazorWebFormsComponents.Test/Button/Tooltip.razor:5-21`
- Microsoft’s Web Forms `Button.UseSubmitBehavior` docs say the default is `true`; default Web Forms rendering uses the browser submit mechanism, i.e. `<input type="submit">`.

### Why it did not fire during Run 33
There is no transform to fire here. The CLI correctly emitted `<Button ... />`, and the BWFC component then rendered `<input type="submit">` by design.

### Proposed fix
Do **not** change the default rendering blindly. That would break current tests and move BWFC away from Web Forms-compatible output.

A safer fix is to add an **opt-in render-tag choice** on the component, while keeping today’s behavior as the default.

Recommended implementation:

1. Add a new parameter on `Button`, e.g.:
   - `public ButtonRenderTag RenderTag { get; set; } = ButtonRenderTag.Input;`
   - enum values: `Input`, `Button`
2. Update `Button.razor` so:
   - `RenderTag.Input` keeps current behavior (`<input type="submit|button" value="..." />`)
   - `RenderTag.Button` renders `<button type="submit|button">@Text</button>` with the same CSS/events/disabled/title/accesskey handling
3. Keep `UseSubmitBehavior` semantics unchanged; it should still control `submit` vs `button`.
4. Add focused tests for both render tags.
5. If we want the migration toolkit to avoid this issue automatically, add a **targeted migration transform** (or semantic pattern) that emits `RenderTag="Button"` only for known problematic layouts such as action buttons inside shopping-cart tables.

That gives Jeff a compatibility-preserving default while providing an explicit escape hatch for accessibility/test-selector scenarios.

## Bottom line
- **Gap 3:** existing expression transform exists, but it is missing statement-block handling and comment sanitization.
- **Gap 5:** no transform exists yet for wrapping field style children in `<ChildComponents>`.
- **Gap 6:** not a transform gap; current BWFC `Button` behavior is intentional and Web Forms-aligned. The right fix is an opt-in render-tag choice, not a silent default flip.
