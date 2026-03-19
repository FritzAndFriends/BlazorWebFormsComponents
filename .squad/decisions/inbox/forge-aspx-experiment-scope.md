# ASPX Middleware Experiment Scope — Forge Decision

**Date:** 2026-03-14  
**Decision Maker:** Forge (Lead / Web Forms Reviewer)  
**Requested by:** Jeffrey T. Fritz  
**Context:** Phase 1 of ASPX-to-Blazor SSR runtime middleware (see planning-docs/ASPX-MIDDLEWARE-FEASIBILITY.md)  

---

## Executive Summary

The ASPX middleware experiment is **scoped to Phase 1: Markup Rendering** — the minimal viable scope that proves the concept works. This session will:

1. Build a `BlazorWebFormsComponents.AspxMiddleware` project (new, experimental)
2. Implement a hybrid XML/regex ASPX parser that handles the three core control patterns
3. Create component tree rendering via RenderTreeBuilder + HtmlRenderer
4. Validate against 3 simple .aspx test files
5. Intentionally defer data binding, code-behind, and advanced features to Phase 2+

**Definition of Done:** A developer can point a middleware-enabled Blazor app at a folder of .aspx files and see them render correctly as HTML, using BWFC components under the hood, without manual migration.

---

## 1. Definition of Done (2-3 Concrete Scenarios)

### Scenario A: Simple Button + Label + Static HTML
**Input:** `simple.aspx`
```html
<%@ Page %>
<div class="container">
  <asp:Label Text="Enter your name:" />
  <asp:TextBox ID="txtName" />
  <asp:Button Text="Submit" CssClass="btn btn-primary" />
</div>
```

**Expected Outcome:**
- Middleware intercepts request to `/simple.aspx`
- Parser extracts markup AST
- ComponentTreeBuilder maps `asp:Label` → `Label` component, `asp:TextBox` → `TextBox`, `asp:Button` → `Button`
- Attributes (`Text`, `CssClass`, `ID`) coerce to component parameters
- Static `<div class="container">` passes through unchanged
- HtmlRenderer produces valid HTML string
- Response renders in browser with correct visual structure (label, input field, button)

**Verification:** HTTP GET `/simple.aspx` returns 200 with HTML; browser DOM contains expected elements.

---

### Scenario B: Nested Panel with Child Controls (Template Content)
**Input:** `nested.aspx`
```html
<%@ Page %>
<asp:Panel CssClass="panel panel-default">
  <asp:Label Text="Header" />
  <asp:Panel>
    <asp:Label Text="Inner content" />
  </asp:Panel>
</asp:Panel>
```

**Expected Outcome:**
- Parser correctly nests `<asp:Panel>` tags and identifies child controls
- ComponentTreeBuilder builds nested RenderFragment for Panel's `ChildContent` parameter
- Outer Panel renders with CssClass, inner Panel renders as child
- Final HTML shows nested `<div>` structure matching Panel's HTML template

**Verification:** Rendered HTML contains nested divs with correct nesting depth; inner label is visually positioned as child of inner panel.

---

### Scenario C: Master Page + Content Placeholder (Layout Mapping)
**Input:** `master.Master` + `content.aspx`
```html
<!-- master.Master -->
<%@ Master %>
<html>
<head><title>My App</title></head>
<body>
  <header>Navigation</header>
  <asp:ContentPlaceHolder ID="cph_Main" />
  <footer>Copyright</footer>
</body>
</html>

<!-- content.aspx -->
<%@ Page MasterPageFile="~/master.Master" %>
<%@ MasterType VirtualPath="~/master.Master" %>
<asp:Content ContentPlaceHolderID="cph_Main">
  <asp:Label Text="Hello from content page" />
</asp:Content>
```

**Expected Outcome:**
- Middleware loads master.Master and maps `<asp:ContentPlaceHolder>` to a Blazor layout component
- Middleware loads content.aspx and maps `<asp:Content>` to @Body-like child content
- Final HTML renders master layout with content page injected into placeholder
- Header, footer, and page content all render in correct order

**Verification:** HTTP GET `/content.aspx` returns 200; HTML contains "Navigation" (header), "Hello from content page" (content), "Copyright" (footer) in correct DOM positions.

---

## 2. Project Structure Decision

### Decision: New Project `BlazorWebFormsComponents.AspxMiddleware`

**Location:** `src/BlazorWebFormsComponents.AspxMiddleware/` (parallel to `src/BlazorWebFormsComponents/`)

**Rationale:**

1. **Experimental scope:** This is an experiment, not a guaranteed feature. Keeping it separate prevents accidental dependencies in the main library.
2. **Clear ownership:** Signals to the team that this is optional middleware, not core BWFC.
3. **Clean dependencies:** The main BWFC project provides *components*; the middleware project provides *orchestration*. They remain loosely coupled.
4. **Future extraction:** If this becomes a standalone tool, moving to a separate repo is frictionless. If it's abandoned, removing a single project is safer than unwinding entangled code.
5. **Test isolation:** Middleware tests live in `BlazorWebFormsComponents.AspxMiddleware.Tests/`, separate from BWFC unit tests.

**Integration point:** New extension method in `BlazorWebFormsComponents.ServiceCollectionExtensions`:
```csharp
public static IApplicationBuilder UseAspxPages(this IApplicationBuilder app, string aspxRootPath)
{
    // Wires up the AspxMiddleware from the new project
    return app.UseMiddleware<AspxPageMiddleware>(aspxRootPath);
}
```

This allows the middleware to be **opt-in** — developers who don't need ASPX parsing pay zero cost.

---

## 3. Parser Approach Decision

### Decision: XDocument (System.Xml.Linq) with Pre-Processing

**Why XDocument, not AngleSharp or Custom Tokenizer:**

| Approach | Pros | Cons | Verdict |
|----------|------|------|---------|
| **XDocument** | DOM parsing, familiar to .NET, handles malformed XML reasonably, pre-processing handles `<% %>` blocks | Slower than regex for simple extraction | **CHOSEN for Phase 1** |
| **AngleSharp** | HTML5-spec parser, handles broken HTML | Extra NuGet dependency, overkill for ASPX | Not chosen; defer to Phase 2 if needed |
| **Custom Tokenizer** | Full control, fast, regex-based | High implementation risk, edge cases, manual test burden | Risky for experiment; only if XDocument fails |

**Implementation Strategy:**

1. **Pre-processing step:** Strip `<% ... %>` code blocks and `<%# ... %>` data-binding expressions *before* XDocument parsing. Replace with placeholders or remove entirely (Phase 1 doesn't support expressions).
   ```csharp
   string aspxMarkup = File.ReadAllText(filePath);
   aspxMarkup = Regex.Replace(aspxMarkup, @"<%[^>]*%>", "");  // Strip code blocks
   aspxMarkup = Regex.Replace(aspxMarkup, @"<%#[^>]*%>", "");  // Strip data-binding expressions
   ```

2. **Parse with XDocument:**
   ```csharp
   var doc = XDocument.Parse(aspxMarkup);
   var root = doc.Root;
   ```

3. **Handle directives separately:** `<%@ Page %>`, `<%@ Master %>` are XML comments or processing instructions; parse them with regex before XDocument.
   ```csharp
   var pageMatch = Regex.Match(aspxMarkup, @"<%@\s*Page\s+([^%>]*)\s*%>");
   var masterPageFile = pageMatch.Groups[1].Value;  // Extract MasterPageFile attribute
   ```

4. **Recursive tree walk:** Once XDocument is built, walk `XElement` nodes and convert to component tree.

**Supported Markup in Phase 1:**
- ✅ `<asp:Button Text="Submit" />`
- ✅ `<asp:Label Text="Hello" />`
- ✅ `<asp:TextBox ID="txt" CssClass="form-control" />`
- ✅ `<asp:Panel><asp:Label Text="Child" /></asp:Panel>` (nested controls)
- ✅ `<div class="container">...</div>` (static HTML passthrough)
- ✅ `<%@ Page %>` directive (ignored for now, parsed for future metadata)
- ✅ `<%@ Master %>` and `<asp:ContentPlaceHolder />` (mapped to layout)
- ❌ `<% C# code %>` (stripped, logged as "not supported in Phase 1")
- ❌ `<%# data-binding expressions %>` (stripped, logged as "Phase 2")

---

## 4. Test ASPX Files (Minimal but Representative)

Create three test .aspx files in `samples/AspxMiddlewareTest/` (new folder):

### File 1: `simple-form.aspx`
```html
<%@ Page Title="Simple Form" %>
<form>
  <fieldset>
    <legend>Contact Form</legend>
    <div class="form-group">
      <asp:Label Text="Name:" For="txtName" />
      <asp:TextBox ID="txtName" CssClass="form-control" />
    </div>
    <div class="form-group">
      <asp:Label Text="Email:" For="txtEmail" />
      <asp:TextBox ID="txtEmail" TextMode="Email" CssClass="form-control" />
    </div>
    <asp:Button Text="Send" CssClass="btn btn-primary" />
  </fieldset>
</form>
```

**Tests:** 
- Parser produces AST with 2 TextBox, 2 Label, 1 Button
- ComponentTreeBuilder maps each to correct BWFC type
- Attributes (`CssClass`, `For`, `TextMode`) coerce correctly
- HTML output contains `<form>`, `<fieldset>`, `<input>` elements with classes

---

### File 2: `dashboard-with-panels.aspx`
```html
<%@ Page Title="Dashboard" %>
<div class="dashboard">
  <asp:Panel CssClass="card">
    <h3>Statistics</h3>
    <asp:Panel CssClass="card-body">
      <asp:Label Text="Total Users: 42" />
    </asp:Panel>
  </asp:Panel>
  <asp:Panel CssClass="card">
    <h3>Recent Activity</h3>
    <asp:Panel CssClass="card-body">
      <asp:Label Text="Last login: today" />
    </asp:Panel>
  </asp:Panel>
</div>
```

**Tests:**
- Parser correctly nests multiple Panel controls
- ChildContent parameter receives nested HTML
- CssClass applied to outer Panel
- Static `<h3>` elements pass through unchanged
- HTML output shows nested divs with correct structure

---

### File 3: `page-with-master.aspx` + `site.Master`
```html
<!-- site.Master -->
<%@ Master Title="Site Master" %>
<!DOCTYPE html>
<html>
<head>
  <title><asp:ContentPlaceHolder ID="head" /></title>
</head>
<body>
  <header>
    <asp:Label Text="My App" CssClass="brand" />
  </header>
  <main>
    <asp:ContentPlaceHolder ID="main" />
  </main>
  <footer>
    <asp:Label Text="© 2026" />
  </footer>
</body>
</html>

<!-- page-with-master.aspx -->
<%@ Page MasterPageFile="~/site.Master" Title="Home" %>
<%@ MasterType VirtualPath="~/site.Master" %>
<asp:Content ContentPlaceHolderID="main">
  <h1>Welcome</h1>
  <asp:Label Text="This is the home page" />
  <asp:Button Text="Learn More" />
</asp:Content>
```

**Tests:**
- Parser loads master.Master, maps ContentPlaceholder IDs
- Parser loads page-with-master.aspx, extracts Content blocks by ID
- ComponentTreeBuilder builds nested layout structure
- HTML output contains header, main content, footer in correct order
- Title attribute from Page directive maps to `<PageTitle>` or HEAD

---

## 5. Explicit Out-of-Scope Items (Phase 2+)

### Phase 1 Explicitly SKIPS:

1. **Code-Behind (.aspx.cs files)**
   - `Page_Load`, `Page_Init`, event handlers (btnSubmit_Click)
   - Will log a warning: "Phase 1 does not support code-behind; data and events will not function."
   - Middleware renders markup only; developers must wire interactivity via Blazor components or code-behind refactoring

2. **Data Binding**
   - `<asp:GridView SelectMethod="GetProducts" />`
   - `<%# Eval("Name") %>`, `<%# Bind("Price") %>`
   - Will be parsed but data=null; middleware logs "Phase 2 feature; wire data via BWFC's Items parameter"

3. **Server-Side Events**
   - `OnClick="btnSubmit_Click"` handlers
   - Middleware logs "Not supported; use Blazor @onclick handlers instead"
   - Button will render but no event wiring

4. **ViewState and PostBack**
   - No `IsPostBack` detection
   - No ViewState persistence
   - Each request renders the page fresh; form submissions are ignored (middleware is SSR-only)

5. **Web Forms Page Lifecycle**
   - No Page_Load, Page_PreRender, Page_Render
   - No Init → Load → PostBack → Render progression
   - Middleware performs single-pass SSR render

6. **Master Page Code-Behind**
   - Only markup from .Master files is processed
   - Events/properties on master page class ignored
   - Will warn if master has code-behind

7. **User Controls (.ascx)**
   - Phase 1 only handles .aspx files
   - `<uc:MyControl />` tags will render as empty or log "User controls deferred to Phase 2"

8. **Custom Server Controls**
   - Third-party controls not in the BWFC component registry
   - Middleware logs "Not found in component registry; skipping"

9. **JavaScript Integration**
   - No `ClientScript.RegisterClientScriptBlock()`
   - No `Page.ClientScript.RegisterStartupScript()`
   - Middleware cannot extract or execute server-side JS registration logic

10. **Web Forms HTTP Handlers (.ashx)**
    - Not applicable; middleware handles .aspx only
    - (Already separate `AshxHandlerMiddleware` exists in main project)

11. **Themes and Skins (.skin files)**
    - Phase 1 parses markup only, not theme metadata
    - Defer to Phase 2 (may integrate with BWFC ThemeProvider)

12. **Control State, Validation Rules**
    - No `RequiredFieldValidator`, `RangeValidator`, etc.
    - These will be parsed but rendered as-is (Phase 2 maps to Blazor validation)
    - Will warn: "Validation controls deferred to Phase 2; use Blazor's DataAnnotations instead"

---

## 6. Parser Error Handling Strategy

**Non-Fatal Warnings (log, continue rendering):**
- Unknown `<asp:*>` control name → render as empty placeholder with warning
- Unparseable attribute → use default value, log warning
- Missing master page file → render without layout, log warning
- `<% %>` code block → skip, log "Code-behind not supported in Phase 1"

**Fatal Errors (throw, fail-fast):**
- Malformed XML after preprocessing → throw `AspxParseException`
- Missing required attribute on component (e.g., Button without Text) → log warning but continue (set Text="")
- Circular layout references (master includes master) → detect cycle, throw

**Logging:**
- Use `Microsoft.Extensions.Logging.ILogger`
- Middleware takes optional `Action<AspxMiddlewareOptions>` to configure log level (Debug, Information, Warning)
- Default: Warning (only critical issues)

---

## 7. Component Registry (asp: Tag → BWFC Type Mapping)

Create a static registry in `BlazorWebFormsComponents.AspxMiddleware`:

```csharp
public static class ComponentRegistry
{
    public static readonly Dictionary<string, Type> ControlMap = new(StringComparer.OrdinalIgnoreCase)
    {
        // Basic Controls
        ["button"]       = typeof(Button),
        ["label"]        = typeof(Label),
        ["textbox"]      = typeof(TextBox),
        ["checkbox"]     = typeof(CheckBox),
        ["radiobutton"]  = typeof(RadioButton),
        ["hyperlink"]    = typeof(HyperLink),
        ["image"]        = typeof(Image),
        ["imagemap"]     = typeof(ImageMap),
        ["panel"]        = typeof(Panel),
        ["literal"]      = typeof(Literal),

        // Data Controls
        ["gridview"]     = typeof(GridView),
        ["formview"]     = typeof(FormView),
        ["detailsview"]  = typeof(DetailsView),
        ["repeater"]     = typeof(Repeater),
        ["datalist"]     = typeof(DataList),

        // Login Controls
        ["login"]        = typeof(Login),
        ["loginview"]    = typeof(LoginView),
        ["loginname"]    = typeof(LoginName),
        ["passwordrecovery"] = typeof(PasswordRecovery),
        ["changepassword"]   = typeof(ChangePassword),

        // Validation
        ["requiredfieldvalidator"]  = typeof(RequiredFieldValidator),
        ["rangevalidator"]          = typeof(RangeValidator),
        ["comparevalidator"]        = typeof(CompareValidator),
        ["customvalidator"]         = typeof(CustomValidator),
        ["validationsummary"]       = typeof(ValidationSummary),

        // Navigation
        ["sitemap"]      = typeof(SiteMapPath),
        ["menu"]         = typeof(Menu),
        ["treeview"]     = typeof(TreeView),

        // Other
        ["placeholder"]  = typeof(ContentPlaceHolder),
        ["content"]      = typeof(Content),
        ["multiview"]    = typeof(MultiView),
        ["view"]         = typeof(View),

        // Infrastructure (render but don't bind)
        ["scriptmanager"] = typeof(ScriptManager),

        // Deferred
        ["chart"]        = null,  // Phase 2+
        ["substitution"] = null,
        ["xml"]          = null,
    };

    public static bool TryGetComponentType(string aspTagName, out Type? componentType)
    {
        return ControlMap.TryGetValue(aspTagName, out componentType);
    }
}
```

---

## 8. Attribute Coercion Strategy

Create `AttributeCoercer` class to map string attributes → component parameters:

```csharp
public static class AttributeCoercer
{
    public static object? CoerceValue(Type targetType, string? value)
    {
        if (value == null) return GetDefaultValue(targetType);

        if (targetType == typeof(string)) return value;
        if (targetType == typeof(bool)) return bool.TryParse(value, out var b) ? b : false;
        if (targetType == typeof(int)) return int.TryParse(value, out var i) ? i : 0;
        if (targetType == typeof(Unit)) return Unit.Parse(value);  // BWFC Unit class
        if (targetType.IsEnum) return Enum.Parse(targetType, value, ignoreCase: true);
        if (targetType == typeof(Color)) return ColorTranslator.FromHtml(value);

        // Fallback: return as-is (RenderTreeBuilder will handle type mismatch)
        return value;
    }

    private static object? GetDefaultValue(Type type) =>
        type.IsValueType ? Activator.CreateInstance(type) : null;
}
```

Phase 1 coerces:
- `Text="Hello"` → string
- `CssClass="btn btn-primary"` → string
- `Enabled="false"` → bool
- `Width="100"`, `Width="100px"`, `Width="10em"` → Unit
- `BorderStyle="Solid"` → enum (BorderStyle)
- `ForeColor="#FF0000"` → Color

---

## 9. RenderTreeBuilder Strategy

High-level pseudocode for tree building:

```csharp
public class ComponentTreeBuilder
{
    private readonly HtmlRenderer _htmlRenderer;
    private readonly ComponentRegistry _registry;

    public async Task<string> BuildAndRenderAsync(AspxNode rootNode)
    {
        var renderFragment = BuildRenderFragment(rootNode);
        
        var html = await _htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            var output = await _htmlRenderer.RenderComponentAsync<DynamicPage>(
                ParameterView.FromDictionary(new Dictionary<string, object?>
                {
                    { "Content", renderFragment }
                }));
            return output.ToHtmlString();
        });

        return html;
    }

    private RenderFragment BuildRenderFragment(AspxNode node) =>
        builder =>
        {
            int sequence = 0;

            foreach (var child in node.Children)
            {
                if (child is AspxControl control)
                {
                    // asp:* tag
                    if (ComponentRegistry.TryGetComponentType(control.TagName, out var componentType) && componentType != null)
                    {
                        builder.OpenComponent(sequence++, componentType);

                        // Set attributes as parameters
                        foreach (var attr in control.Attributes)
                        {
                            var paramValue = AttributeCoercer.CoerceValue(attr.Type, attr.Value);
                            builder.AddAttribute(sequence++, attr.Name, paramValue);
                        }

                        // Recursively build ChildContent
                        builder.AddAttribute(sequence++, "ChildContent",
                            new RenderFragment(innerBuilder =>
                            {
                                int innerSeq = 0;
                                foreach (var child of control.Children)
                                {
                                    innerBuilder.AddContent(innerSeq++,
                                        BuildRenderFragment(child));
                                }
                            }));

                        builder.CloseComponent();
                    }
                    else
                    {
                        // Unknown control; render placeholder with warning
                        builder.AddMarkupContent(sequence++,
                            $"<!-- Unknown control: {control.TagName} -->");
                    }
                }
                else if (child is AspxHtmlElement html)
                {
                    // Static HTML passthrough
                    builder.AddMarkupContent(sequence++, html.OuterHtml);
                }
                else if (child is AspxText text)
                {
                    // Text node
                    builder.AddContent(sequence++, text.Value);
                }
            }
        };
}
```

---

## 10. Middleware Integration Point

Extend `ServiceCollectionExtensions.cs` in main project:

```csharp
public static IApplicationBuilder UseAspxPages(
    this IApplicationBuilder app,
    string aspxRootPath = "aspx-pages",
    Action<AspxMiddlewareOptions>? configure = null)
{
    var options = new AspxMiddlewareOptions { AspxRootPath = aspxRootPath };
    configure?.Invoke(options);

    app.UseMiddleware<AspxPageMiddleware>(options);
    return app;
}

public class AspxMiddlewareOptions
{
    public string AspxRootPath { get; set; } = "aspx-pages";
    public bool EnableCaching { get; set; } = true;
    public LogLevel LogLevel { get; set; } = LogLevel.Warning;
    public Func<string, bool>? FileFilter { get; set; }  // Custom file filter
}
```

Usage in Program.cs:
```csharp
app.UseAspxPages("LegacyPages", options =>
{
    options.EnableCaching = true;
    options.LogLevel = LogLevel.Information;
});
```

---

## 11. Success Criteria (Definition of Done — Detailed Checklist)

- [ ] New project `src/BlazorWebFormsComponents.AspxMiddleware/` created and added to solution
- [ ] AspxParser class: parses simple.aspx, nested.aspx, site.Master + page-with-master.aspx without errors
- [ ] ComponentRegistry populated with all 50+ BWFC components
- [ ] AttributeCoercer handles string, bool, int, Unit, enum, Color coercion
- [ ] RenderTreeBuilder walks AST and builds RenderFragment using RenderTreeBuilder API
- [ ] AspxPageMiddleware intercepts .aspx requests and returns HTML via HtmlRenderer
- [ ] Three test .aspx files in `samples/AspxMiddlewareTest/` render correctly in browser
- [ ] Unit tests for parser, registry, coercer in `BlazorWebFormsComponents.AspxMiddleware.Tests`
- [ ] Integration test: HTTP GET `/simple.aspx` returns 200 with HTML containing expected elements
- [ ] Integration test: HTTP GET `/nested.aspx` renders nested panels
- [ ] Integration test: HTTP GET `/page-with-master.aspx` renders layout with master + content
- [ ] Error handling: unknown controls logged as warnings, rendering continues
- [ ] Error handling: `<% %>` blocks logged as "Phase 1: not supported", rendering continues
- [ ] No regressions in main BWFC project (all 797 tests still pass)

---

## 12. Phase 2 Handoff (Not in Scope, But Noted)

If this experiment succeeds, Phase 2 will include:

1. **Expression Compiler:** Parse `<%# Eval("Name") %>`, `<%# Item.Price %>` and compile to lambdas
2. **Data Provider Registry:** `SelectMethod="GetProducts"` → resolve via DI
3. **Template Mapping:** `<ItemTemplate>` → RenderFragment<T> parameters
4. **Form PostBack:** Two-way binding, event handlers (via code-behind service)
5. **Master Page Code-Behind:** Event hookup for master page classes

Phase 2 is estimated at ~4 weeks additional work.

---

## Sign-Off

This scope is **approved for experimental Phase 1**. It is:
- ✅ Technically achievable in a 1-week sprint
- ✅ Representatively tests the three core patterns (simple controls, nesting, layouts)
- ✅ Intentionally scoped to avoid over-commitment
- ✅ Clear exit criteria (Definition of Done checklist)
- ✅ Defers complex features to Phase 2 without blocking initial validation

**Assigned to:** Cyclops (implementation), Rogue (unit tests), Colossus (integration tests)  
**Review gate:** Forge will review completed work against this scope before Phase 2 decision.
