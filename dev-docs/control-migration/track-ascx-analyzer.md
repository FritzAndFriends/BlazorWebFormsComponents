# Track: ASCX Property/Event Descriptor Analyzer

**Priority:** P0 (Foundation)  
**Owner:** Bishop (CLI Development)  
**Status:** Planned  
**Validation:** ContosoUniversity prescan + Layer 1 build

## Problem Statement

ASCX files (user controls) define properties and events that parent pages use. Currently, the CLI has no way to analyze ASCX source and map those properties/events to Blazor `[Parameter]` and `EventCallback` equivalents.

**Example from WingtipToys:**
```razor
@* ProductsControl.ascx (Web Forms) *@
<%@ Control Language="C#" CodeBehind="ProductsControl.ascx.cs" Inherits="WingtipToys.ProductsControl" %>
<asp:GridView ID="gvProducts" runat="server" />
```

```csharp
// ProductsControl.ascx.cs
public partial class ProductsControl : UserControl
{
    public int? CategoryID { get; set; }  // <-- Property from parent page
    public event EventHandler OnProductSelected;  // <-- Event raised to parent
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) LoadProducts(CategoryID);
    }
    
    public void LoadProducts(int categoryId)
    {
        gvProducts.DataSource = GetProductsByCategory(categoryId);
        gvProducts.DataBind();
    }
}
```

Parent page usage:
```razor
<%@ Page ... %>
<%@ Register Src="~/Controls/ProductsControl.ascx" TagName="ProductsControl" TagPrefix="uc1" %>
<uc1:ProductsControl ID="productsCtrl" CategoryID="<%= Session["CategoryID"] %>" runat="server" />
```

When migrated to Blazor, this becomes:
```razor
<ProductsControl @ref="productsCtrl" CategoryID="@categoryId" OnProductSelected="HandleProductSelected" />
```

The analyzer must extract the property/event signature from `.ascx.cs` so the transformer knows what to emit.

## Scope

Parse ASCX code-behind (`.ascx.cs`) and extract:

1. **Public properties** (with getter/setter) → `[Parameter]` candidates
2. **Public events** → `EventCallback` candidates
3. **Public methods** → method call targets for parent
4. **DataBind/Load lifecycle** → execution order for binding transforms
5. **Parent control references** (e.g., `FindControl` calls) → `@ref` transformation targets

## Deliverables

### 1. Create `AscxDescriptor` class

**Location:** `src/BlazorWebFormsComponents.Cli/Analysis/`

```csharp
public class AscxDescriptor
{
    public string ControlName { get; set; }  // e.g., "ProductsControl"
    public string BaseClassName { get; set; }  // e.g., "ProductsControl" (from code-behind)
    public string CodeBehindPath { get; set; }
    
    public IReadOnlyList<PropertyDescriptor> Properties { get; set; }
    public IReadOnlyList<EventDescriptor> Events { get; set; }
    public IReadOnlyList<MethodDescriptor> Methods { get; set; }
    
    public bool HasDataBindCall { get; set; }
    public bool HasPageLoadOverride { get; set; }
}

public record PropertyDescriptor(
    string Name,
    string Type,
    bool HasGetter,
    bool HasSetter,
    string DefaultValue = null
);

public record EventDescriptor(
    string Name,
    string DelegateType
);

public record MethodDescriptor(
    string Name,
    string ReturnType,
    IReadOnlyList<string> ParameterNames
);
```

### 2. Create `AscxDescriptorAnalyzer` class

**Location:** `src/BlazorWebFormsComponents.Cli/Analysis/`

Use Roslyn to parse `.ascx.cs` files:

```csharp
public class AscxDescriptorAnalyzer
{
    /// <summary>
    /// Analyze an ASCX code-behind file and extract properties, events, and methods.
    /// </summary>
    public AscxDescriptor Analyze(string ascxCodeBehindPath);
}
```

### 3. Integrate into `RuntimeDetector`

Store descriptors in a catalog during prescan:

```csharp
public class RuntimeDetector
{
    public IDictionary<string, AscxDescriptor> AscxDescriptors { get; private set; }
    
    public void Analyze(ProjectMetadata project)
    {
        // ...existing code...
        
        var analyzer = new AscxDescriptorAnalyzer();
        foreach (var ascxCodeBehindPath in FindAscxCodeBehindFiles(project))
        {
            var descriptor = analyzer.Analyze(ascxCodeBehindPath);
            this.AscxDescriptors[descriptor.ControlName] = descriptor;
        }
    }
}
```

### 4. Create unit tests

**Location:** `tests/BlazorWebFormsComponents.Cli.Tests/Analysis/AscxDescriptorAnalyzerTests.cs`

Test cases:

- ✓ Parse public properties (auto-props, backing fields)
- ✓ Parse public events (EventHandler, custom delegates)
- ✓ Parse public methods (with parameters)
- ✓ Detect DataBind() calls
- ✓ Detect Page_Load override
- ✓ Handle missing code-behind gracefully
- ✓ Handle malformed C# without crash (partial parse)
- ✓ Extract method parameters and return types

### 5. Integration test

Add prescan test with real ASCX controls:

```bash
dotnet run --project src/BlazorWebFormsComponents.Cli -- \
  prescan -i samples/WingtipToys \
  --report-control-descriptors  # New flag
```

Output should list ASCX controls and their properties/events.

## Validation Criteria

✅ Analyzer extracts public properties from ASCX code-behind  
✅ Analyzer extracts public events from ASCX code-behind  
✅ Analyzer extracts public methods with parameter lists  
✅ Analyzer detects DataBind() calls and Page_Load overrides  
✅ Prescan report includes control descriptor summary  
✅ No crash on missing or partial code-behind  
✅ Unit tests pass: `dotnet test tests/BlazorWebFormsComponents.Cli.Tests --filter "AscxDescriptorAnalyzer"`

## Dependencies

- Requires `WebConfigAssemblyParser` (P0) to validate that custom assemblies can be loaded
- Requires Roslyn NuGet package (already in use for analyzers)

## Unblocks

- ContentTemplate unwrapper (uses descriptor to map template parameter types)
- FindControl transform (maps control references to @ref parameters)
- ASCX binding/lifecycle transforms (uses descriptor to route binding rewrites)

## Not in Scope

- Generating BWFC component from descriptor (that's the scaffolder, P3)
- Actually loading custom control assemblies (P2)
- Analyzing ASCX markup for embedded controls (that's ContentTemplate unwrapper, P1)
- Handling generic controls or complex inheritance chains (P2 enhancement)

## Edge Cases

| Case | Handling | Status |
|------|----------|--------|
| Control with no public properties | Valid; report empty list | OK |
| Property with only getter | Include but mark read-only for parameter mapping | OK |
| Event with custom delegate type | Extract delegate signature | OK |
| Nested/inherited properties | Use Roslyn symbol analysis to recurse base classes | P2 |
| ASCX with no code-behind (.ascx only) | Valid; report no descriptor | OK |
| Code-behind with syntax errors | Partial parse only, report errors in log | OK |
