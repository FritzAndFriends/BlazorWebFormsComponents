---
name: analyzer-development
description: "Create Roslyn-based analyzers and code-fix providers for detecting Web Forms migration patterns. Covers DiagnosticAnalyzer architecture, diagnostic ID conventions (BWFC001–BWFC099), CodeFixProvider authoring, testing with CSharpAnalyzerTest/CSharpCodeFixTest, and the analyzer release tracking files. Use when adding a new migration rule, creating a code-fix suggestion, or debugging analyzer behavior."
---

# Analyzer Development

This skill covers creating Roslyn analyzers and code-fix providers in the `BlazorWebFormsComponents.Analyzers` project.

## Architecture

The analyzer project ships as a NuGet analyzer package alongside the main BWFC library. Analyzers detect Web Forms patterns in user code and suggest BWFC replacements.

### Project Structure

```
src/BlazorWebFormsComponents.Analyzers/
├── *Analyzer.cs              — DiagnosticAnalyzer implementations
├── *CodeFixProvider.cs       — Optional companion code-fix providers
├── SyntaxExtensions.cs       — Shared Roslyn helper methods
├── AnalyzerReleases.Shipped.md    — Released diagnostic IDs
└── AnalyzerReleases.Unshipped.md  — In-progress diagnostic IDs

src/BlazorWebFormsComponents.Analyzers.Test/
├── *AnalyzerTests.cs         — Analyzer unit tests
├── *CodeFixTests.cs          — Code-fix unit tests
└── AllAnalyzersIntegrationTests.cs — Cross-cutting integration tests
```

### Current Analyzers

| Analyzer | Diagnostic | Detects |
|----------|------------|---------|
| `EventHandlerSignatureAnalyzer` | BWFC001+ | Web Forms event handler signatures |
| `FindControlUsageAnalyzer` | BWFC0xx | `FindControl()` calls |
| `IsPostBackUsageAnalyzer` | BWFC0xx | `IsPostBack` property usage |
| `ViewStateUsageAnalyzer` | BWFC0xx | `ViewState["key"]` access |
| `ViewStatePropertyPatternAnalyzer` | BWFC0xx | ViewState-backed property patterns |
| `SessionUsageAnalyzer` | BWFC0xx | `Session["key"]` access |
| `ResponseObjectUsageAnalyzer` | BWFC0xx | `Response.Redirect`, `Response.Write` |
| `ResponseRedirectAnalyzer` | BWFC0xx | `Response.Redirect` specifically |
| `RequestObjectUsageAnalyzer` | BWFC0xx | `Request.QueryString`, `Request.Form` |
| `PageClientScriptUsageAnalyzer` | BWFC0xx | `ClientScript.Register*` |
| `ScriptManagerUsageAnalyzer` | BWFC0xx | `ScriptManager` usage |
| `RunatServerAnalyzer` | BWFC0xx | `runat="server"` in markup |
| `NonSerializableViewStateAnalyzer` | BWFC0xx | Non-serializable types in ViewState |
| `MissingParameterAttributeAnalyzer` | BWFC0xx | Missing `[Parameter]` on public props |
| `RequiredAttributeAnalyzer` | BWFC0xx | Required HTML attributes |
| `IPostBackEventHandlerUsageAnalyzer` | BWFC0xx | `IPostBackEventHandler` implementation |

## Creating a New Analyzer

### Step 1: Define the Diagnostic Descriptor

```csharp
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace BlazorWebFormsComponents.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MyPatternAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "BWFC0XX";  // Pick next available ID

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: "Web Forms pattern detected",
        messageFormat: "'{0}' is a Web Forms pattern — use '{1}' instead",
        category: "Migration",
        defaultSeverity: DiagnosticSeverity.Info,  // Info, Warning, or Error
        isEnabledByDefault: true,
        description: "Detects a Web Forms pattern that should be replaced with a BWFC equivalent.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // Register for the syntax node types you want to analyze
        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        // Check if this matches the Web Forms pattern
        if (invocation.Expression is MemberAccessExpressionSyntax memberAccess
            && memberAccess.Name.Identifier.Text == "TargetMethod")
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                invocation.GetLocation(),
                "TargetMethod",     // {0} in messageFormat
                "BwfcAlternative"); // {1} in messageFormat

            context.ReportDiagnostic(diagnostic);
        }
    }
}
```

### Step 2: Create a Code-Fix Provider (Optional)

```csharp
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
using System.Composition;

namespace BlazorWebFormsComponents.Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MyPatternCodeFixProvider))]
[Shared]
public class MyPatternCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(MyPatternAnalyzer.DiagnosticId);

    public override FixAllProvider GetFixAllProvider() =>
        WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
        var diagnostic = context.Diagnostics[0];
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var node = root?.FindNode(diagnosticSpan);
        if (node == null) return;

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Replace with BWFC equivalent",
                createChangedDocument: c => FixAsync(context.Document, node, c),
                equivalenceKey: nameof(MyPatternCodeFixProvider)),
            diagnostic);
    }

    private static async Task<Document> FixAsync(
        Document document, SyntaxNode node, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken);
        // Create the replacement node
        var newNode = SyntaxFactory.ParseExpression("BwfcAlternative()");
        var newRoot = root!.ReplaceNode(node, newNode);
        return document.WithSyntaxRoot(newRoot);
    }
}
```

### Step 3: Update Release Tracking

Add the new diagnostic ID to `AnalyzerReleases.Unshipped.md`:

```markdown
### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|------
BWFC0XX | Migration | Info | Detects MyPattern usage
```

### Step 4: Write Tests

```csharp
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyAnalyzer = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    BlazorWebFormsComponents.Analyzers.MyPatternAnalyzer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace BlazorWebFormsComponents.Analyzers.Test;

public class MyPatternAnalyzerTests
{
    // Stub types the analyzer needs to compile against
    private const string ComponentStubs = @"
namespace System.Web.UI
{
    public class Page
    {
        public void TargetMethod() { }
    }
}";

    [Fact]
    public async Task DetectsTargetMethod_ReportsDiagnostic()
    {
        var test = new VerifyAnalyzer
        {
            TestCode = ComponentStubs + @"
class TestPage : System.Web.UI.Page
{
    void Test()
    {
        {|#0:TargetMethod()|};
    }
}",
            ExpectedDiagnostics =
            {
                new DiagnosticResult(MyPatternAnalyzer.DiagnosticId)
                    .WithLocation(0)
                    .WithArguments("TargetMethod", "BwfcAlternative")
            }
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task NoTargetMethod_NoDiagnostic()
    {
        var test = new VerifyAnalyzer
        {
            TestCode = @"
class MyClass
{
    void Test()
    {
        var x = 1 + 2;
    }
}"
        };

        await test.RunAsync();
    }
}
```

## Testing Commands

```bash
# Run analyzer tests
dotnet test src/BlazorWebFormsComponents.Analyzers.Test --nologo

# Run a specific test
dotnet test src/BlazorWebFormsComponents.Analyzers.Test --filter "MyPatternAnalyzerTests"
```

## Diagnostic ID Conventions

- **Range:** `BWFC001` through `BWFC099`
- **Category:** `Migration` (for Web Forms → Blazor patterns)
- **Severity levels:**
  - `Info` — Pattern detected, BWFC equivalent available (most common)
  - `Warning` — Pattern that will cause runtime issues if not addressed
  - `Error` — Pattern that prevents compilation in Blazor

## NuGet Packaging

The analyzer DLL is packed into the `analyzers/dotnet/cs` path automatically via the `.csproj`:

```xml
<None Include="$(OutputPath)\$(AssemblyName).dll"
      Pack="true" PackagePath="analyzers/dotnet/cs" Visible="false" />
```

Consumers get analyzers automatically when they reference the BWFC NuGet package.

## Checklist

- [ ] Analyzer class has `[DiagnosticAnalyzer(LanguageNames.CSharp)]` attribute
- [ ] Diagnostic ID follows `BWFC0XX` convention (pick next available)
- [ ] `Initialize` calls `ConfigureGeneratedCodeAnalysis` and `EnableConcurrentExecution`
- [ ] Code-fix provider (if any) has `[ExportCodeFixProvider]` and `[Shared]` attributes
- [ ] Diagnostic ID added to `AnalyzerReleases.Unshipped.md`
- [ ] Positive tests (pattern detected → diagnostic reported)
- [ ] Negative tests (no pattern → no diagnostic)
- [ ] Code-fix tests (if applicable) verify the fix produces correct code
- [ ] `dotnet test src/BlazorWebFormsComponents.Analyzers.Test` passes
