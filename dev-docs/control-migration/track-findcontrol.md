# Track: FindControl-to-@ref Transform

**Priority:** P1 (Transforms)  
**Owner:** Bishop (CLI Development)  
**Status:** Planned  
**Validation:** Code-behind rewrite tests + Layer 1 build

## Problem Statement

Web Forms code-behind uses `FindControl()` to query controls by ID at runtime:

```csharp
// Old Web Forms code-behind
protected void BindData()
{
    var gvProducts = (GridView)FindControl("gvProducts");
    gvProducts.DataSource = GetProducts();
    gvProducts.DataBind();
}
```

In Blazor, this pattern must be replaced with component references (`@ref`):

```csharp
// New Blazor code-behind
@code {
    private GridView gvProducts;  // Component reference
    
    protected async Task BindData()
    {
        if (gvProducts != null)
        {
            gvProducts.Items = await GetProducts();
            await gvProducts.Refresh();
        }
    }
}
```

The transform must:
1. Detect `FindControl` calls in code-behind
2. Extract the control ID
3. Rewrite to component reference syntax
4. Update the corresponding markup to use `@ref`

## Scope

Transform code-behind `FindControl` patterns:

1. **Simple FindControl calls** → `@ref` parameter + field declaration
2. **Casted FindControl** (`(GridView)FindControl(...)`) → typed reference
3. **Chain calls** (`FindControl(...).PropertyName = value`) → reference access
4. **Multiple FindControl calls** for same ID → reuse reference
5. **Parent page FindControl** → skip (requires page-level work)

## Deliverables

### 1. Create `FindControlTransform` (Code-Behind)

**Location:** `src/BlazorWebFormsComponents.Cli/Transforms/CodeBehind/`

```csharp
public class FindControlTransform : ICodeBehindTransform
{
    public string Name => "FindControl-to-@ref";
    
    public string Transform(string codeBehind, FileMetadata metadata)
    {
        // 1. Find all FindControl("id") calls
        // 2. Extract ID and type (from cast if present)
        // 3. Replace with reference variable
        // 4. Record references for markup transform
        
        return transformedCodeBehind;
    }
}
```

### 2. Create `FindControlMarkupTransform`

Coordinate markup side:

```csharp
public class FindControlMarkupTransform : IMarkupTransform
{
    public string Name => "FindControl-Markup-Binding";
    
    public string Transform(string markup, FileMetadata metadata)
    {
        // For each control ID tracked by code-behind transform,
        // add @ref="controlName" to matching control element
        
        return transformedMarkup;
    }
}
```

### 3. Pattern detection

Detect these patterns:

```csharp
// Pattern 1: Untyped cast
var ctrl = (GridView)FindControl("gvProducts");

// Pattern 2: Direct call
FindControl("gvProducts").DataSource = items;

// Pattern 3: Method parameter
ProcessControl(FindControl("gvProducts"));

// Pattern 4: Null check
if (FindControl("gvProducts") != null) { ... }
```

### 4. Rewrite strategy

For each pattern:

```csharp
// BEFORE:
var gvProducts = (GridView)FindControl("gvProducts");
gvProducts.DataSource = GetData();

// AFTER:
// (In markup: <GridView @ref="gvProducts" ... />)
if (gvProducts != null)
{
    gvProducts.Items = await GetData();  // DataSource → Items
    await gvProducts.Refresh();           // DataBind() → Refresh()
}
```

### 5. Integrate into pipeline

Register both transforms:

```csharp
// Program.cs
services.AddSingleton<ICodeBehindTransform, FindControlTransform>();
services.AddSingleton<IMarkupTransform, FindControlMarkupTransform>();
```

Register in test pipeline:

```csharp
// TestHelpers.cs
var codeBehindTransforms = new List<ICodeBehindTransform>
{
    new FindControlTransform(),
};

var markupTransforms = new List<IMarkupTransform>
{
    new FindControlMarkupTransform(codeBehindTransforms),
};
```

### 6. Create unit tests

**Location:** `tests/BlazorWebFormsComponents.Cli.Tests/Transforms/CodeBehind/FindControlTransformTests.cs`

Test cases:

- ✓ Detect untyped FindControl calls
- ✓ Detect casted FindControl (extract type)
- ✓ Rewrite to reference declaration
- ✓ Replace FindControl(...) with reference variable
- ✓ Handle multiple calls to same control (reuse reference)
- ✓ Skip FindControl in comments
- ✓ Handle nested FindControl calls
- ✓ Add null checks when needed
- ✓ Convert DataBind() to Refresh() on reference

### 7. Integration test

Test with real ASCX/page code-behind:

```bash
dotnet run --project src/BlazorWebFormsComponents.Cli -- \
  migrate -i samples/WingtipToys -o /tmp/test-output
```

Verify:
- References are declared in code-behind
- Markup includes `@ref="..."`  attributes
- FindControl calls are replaced

## Validation Criteria

✅ FindControl calls are detected and replaced with @ref references  
✅ Control types are preserved (from casts)  
✅ References are declared in code-behind  
✅ Markup includes @ref attributes  
✅ Null checks are added for safe access  
✅ Multiple calls to same control reuse reference  
✅ Unit tests pass: `dotnet test tests/BlazorWebFormsComponents.Cli.Tests --filter "FindControl"`  
✅ Integration test passes on WingtipToys

## Dependencies

- Requires `ContentTemplateUnwrapperTransform` (P1) — template controls also use FindControl
- Requires `AscxDescriptorAnalyzer` (P0) — to understand control types

## Unblocks

- ASCX binding/lifecycle transforms (P2) — binding patterns often interleave with FindControl
- Code-behind correctness in Layer 1 — critical for L2 build

## Not in Scope

- FindControl on parent page (requires `Page.FindControl` shim work)
- FindControl in inline code blocks (`<% %>`)
- FindControl with complex expressions or loops
- Dynamic control creation (`new GridView()`)

## Edge Cases

| Case | Handling | Status |
|------|----------|--------|
| FindControl with variable ID | Skip (dynamic; document as unsupported) | Warn |
| Nested FindControl (FindControl in method) | Include; replace with reference | OK |
| FindControl in lambda/LINQ | Skip (complex; warn) | Warn |
| FindControl result immediately disposed | Add null check anyway | OK |
| Multiple types cast to same ID | Use most specific type, warn if conflict | Warn |
