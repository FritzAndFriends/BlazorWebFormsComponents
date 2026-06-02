---
name: cli-transform-authoring
description: "Create and register new CLI migration transforms for the webforms-to-blazor pipeline. Covers IMarkupTransform and ICodeBehindTransform interfaces, transform ordering, dual-registration in Program.cs and TestHelpers.cs, testing patterns, and when to use a transform vs. a semantic pattern. Use when adding a new markup or code-behind transform, debugging transform output, or understanding transform execution order."
---

# CLI Transform Authoring

This skill covers creating new transforms for the `webforms-to-blazor` CLI migration pipeline.

## Architecture Overview

The migration CLI uses two transform interfaces that run in sequence during file conversion:

1. **Markup transforms** (`IMarkupTransform`) — Convert `.aspx`/`.ascx`/`.master` markup to `.razor` syntax
2. **Code-behind transforms** (`ICodeBehindTransform`) — Convert `.aspx.cs` code-behind to `.razor.cs`

Both interfaces share the same contract:

```csharp
public interface IMarkupTransform  // (or ICodeBehindTransform)
{
    string Name { get; }
    int Order { get; }
    string Apply(string content, FileMetadata metadata);
}
```

- **`Name`** — Human-readable identifier (used in logs and diagnostics)
- **`Order`** — Execution sequence (ascending). Lower numbers run first.
- **`Apply`** — Takes current content + file metadata, returns transformed content

### Pipeline Execution Order

`MigrationPipeline` runs transforms in this order:

1. **Markup transforms** (sorted by `Order` ascending) — all run on `.razor` content
2. **Code-behind transforms** (sorted by `Order` ascending) — all run on `.razor.cs` content
3. **Semantic patterns** (`SemanticPatternCatalog`) — page-level rewrites after all transforms

### FileMetadata

Every transform receives `FileMetadata` which carries context about the file being processed:

- `OriginalPath` — Source `.aspx` path
- `OutputPath` — Target `.razor` path
- `PageDirectives` — Parsed directive info (CodeBehind, Inherits, MasterPageFile)
- `MarkupContent` — Can be set by code-behind transforms to modify the markup file
- `CodeBehindContent` — The code-behind source
- `IsUserControl` / `IsMasterPage` — File type flags

## Creating a New Transform

### Step 1: Create the Transform Class

```
src/BlazorWebFormsComponents.Cli/Transforms/Markup/MyNewTransform.cs
    — or —
src/BlazorWebFormsComponents.Cli/Transforms/CodeBehind/MyNewTransform.cs
```

```csharp
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;  // or .CodeBehind

public class MyNewTransform : IMarkupTransform  // or ICodeBehindTransform
{
    public string Name => "MyNewTransform";
    public int Order => 500;  // Choose based on dependencies

    public string Apply(string content, FileMetadata metadata)
    {
        // Return content unchanged if this transform doesn't apply
        if (!content.Contains("pattern-to-match"))
            return content;

        // Apply transformation
        return content.Replace("old-pattern", "new-pattern");
    }
}
```

### Step 2: Register in BOTH Locations

**⚠️ CRITICAL: Every transform must be registered in TWO places.**

**`src/BlazorWebFormsComponents.Cli/Program.cs`** (runtime DI):

```csharp
services.AddSingleton<IMarkupTransform, MyNewTransform>();
// — or —
services.AddSingleton<ICodeBehindTransform, MyNewTransform>();
```

**`tests/BlazorWebFormsComponents.Cli.Tests/TestHelpers.cs`** (test pipeline):

```csharp
var markupTransforms = new List<IMarkupTransform>
{
    // ... existing transforms ...
    new MyNewTransform(),
};
// — or for code-behind —
var codeBehindTransforms = new List<ICodeBehindTransform>
{
    // ... existing transforms ...
    new MyNewTransform(),
};
```

### Step 3: Write Tests

Create a test file at `tests/BlazorWebFormsComponents.Cli.Tests/MyNewTransformTests.cs`:

```csharp
using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;
using Xunit;

namespace BlazorWebFormsComponents.Cli.Tests;

public class MyNewTransformTests
{
    private readonly MyNewTransform _transform = new();

    [Fact]
    public void Apply_WithMatchingPattern_TransformsCorrectly()
    {
        var input = "<asp:CustomControl runat=\"server\" />";
        var metadata = new FileMetadata { OriginalPath = "Test.aspx" };

        var result = _transform.Apply(input, metadata);

        Assert.Contains("expected-output", result);
        Assert.DoesNotContain("asp:", result);
    }

    [Fact]
    public void Apply_WithoutMatchingPattern_ReturnsUnchanged()
    {
        var input = "<div>No Web Forms here</div>";
        var metadata = new FileMetadata { OriginalPath = "Test.aspx" };

        var result = _transform.Apply(input, metadata);

        Assert.Equal(input, result);
    }
}
```

### Step 4: Run Tests

```bash
dotnet test tests\BlazorWebFormsComponents.Cli.Tests --nologo
```

## Transform Ordering Guidelines

| Order Range | Category | Examples |
|-------------|----------|----------|
| 0–99 | Directive stripping | `PageDirectiveTransform`, `RegisterDirectiveTransform` |
| 100–199 | Structural transforms | `MasterPageTransform`, `ContentWrapperTransform` |
| 200–299 | Prefix/attribute removal | `AspPrefixTransform`, `AttributeStripTransform` |
| 300–399 | Expression/binding rewrites | `ExpressionTransform`, `DataBindingAttributeTransform` |
| 400–499 | Template/column processing | `TemplateFieldChildComponentsTransform`, `GridViewColumnItemTypeTransform` |
| 500–599 | Event/form wiring | `EventWiringTransform`, `FormWrapperTransform` |
| 600–699 | URL/reference cleanup | `UrlReferenceTransform`, `ScriptManagerStripTransform` |
| 700+ | Post-processing | `FormAntiforgeryPostProcessor`, `ValidatorGenericTypeTransform` |

**Key ordering dependencies:**
- `AspPrefixTransform` must run before `AttributeStripTransform` (strip `asp:` before removing `runat`)
- `TemplateContextTransform` must run before `TemplateFieldChildComponentsTransform`
- `SelectMethodTransform` must run before `EventWiringTransform`

## Transform vs. Semantic Pattern

| Use a Transform when... | Use a Semantic Pattern when... |
|--------------------------|-------------------------------|
| Converting a specific Web Forms tag/attribute | Rewriting page-level structure |
| Change is mechanical (regex/string) | Change requires understanding page intent |
| Applies to ALL files universally | Applies only to specific page types (query, detail, action) |
| No context about surrounding markup needed | Needs to understand the full page layout |
| Example: stripping `runat="server"` | Example: converting a search form + GridView into a query page |

## Current Transform Inventory

- **24 markup transforms** in `src/BlazorWebFormsComponents.Cli/Transforms/Markup/`
- **5 directive transforms** in `src/BlazorWebFormsComponents.Cli/Transforms/Directives/`
- **27 code-behind transforms** in `src/BlazorWebFormsComponents.Cli/Transforms/CodeBehind/`

## Checklist

- [ ] Transform class implements `IMarkupTransform` or `ICodeBehindTransform`
- [ ] `Order` is set appropriately (check ordering dependencies)
- [ ] Registered in `Program.cs` DI container
- [ ] Registered in `TestHelpers.cs` test pipeline
- [ ] Unit tests cover: matching input, non-matching input, edge cases
- [ ] `dotnet test tests\BlazorWebFormsComponents.Cli.Tests` passes
- [ ] Benchmark regression check: run migration against current focus app
