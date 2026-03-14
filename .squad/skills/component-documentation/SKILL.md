# SKILL: Component Documentation

## When to Use
When writing documentation for a new BlazorWebFormsComponents component. Apply this pattern to create consistent, migration-friendly documentation.

## Document Structure

Every component doc follows this exact section order:

1. **Title** (`# ComponentName`) — Bold intro sentence describing what it does, why it exists in the library
2. **MS Docs Link** — `Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.{name}?view=netframework-4.8`
3. **Features Supported in Blazor** — Bulleted list of all parameters, events, and properties
4. **Web Forms Features NOT Supported** — What was deliberately omitted and why
5. **Web Forms Declarative Syntax** — Full `<asp:Component>` syntax block showing all attributes
6. **Blazor Razor Syntax** — Multiple subsections with `### Heading` for each usage pattern, each with a `razor` code block
7. **HTML Output** — Side-by-side showing Blazor input → rendered HTML
8. **Migration Notes** — Numbered list of migration steps, then Before/After code blocks
9. **Examples** or **Common Scenarios** — Additional real-world patterns
10. **See Also** — Related components and MS docs links

## Conventions

- Use `razor` for Blazor code blocks, `html` for Web Forms/HTML, `csharp` for C# code-behind
- Use admonitions (`!!! note`, `!!! warning`, `!!! tip`) for gotchas, security notes, and best practices
- Always show the `@code { }` block in Blazor examples when event handlers or binding is involved
- Reference tables use `| Property | Type | Description |` format
- Nav entries in `mkdocs.yml` are alphabetical within category
- Editor Controls = form inputs and display; Navigation Controls = controls that navigate; Utility Features = non-visual services

## Category Assignment

| Category | Examples | Criteria |
|----------|----------|----------|
| Editor Controls | Button, TextBox, CheckBox, Image, FileUpload, Calendar | Form inputs, display controls |
| Data Controls | GridView, Repeater, ListView | Data-bound controls |
| Validation Controls | RequiredFieldValidator, CompareValidator | Input validation |
| Navigation Controls | HyperLink, Menu, TreeView, ImageMap | Controls that navigate users |
| Login Controls | Login, LoginName, LoginStatus | Authentication UI |
| Utility Features | ViewState, PageService, Databinder | Non-visual services and helpers |
