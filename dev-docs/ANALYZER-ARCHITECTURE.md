# Analyzer Architecture & Contributor Guide

This guide explains how to develop new Roslyn analyzers and code fix providers for the **BlazorWebFormsComponents.Analyzers** package. It covers the anatomy of an analyzer, testing strategy, common pitfalls, and the PR checklist for contributing new rules.

---

## Project Layout

The analyzer project lives in two NuGet-friendly locations:

```
src/BlazorWebFormsComponents.Analyzers/
├── *.cs                           # Analyzer and CodeFixProvider pairs
├── AnalyzerReleases.Shipped.md    # Released rule IDs (archive)
└── AnalyzerReleases.Unshipped.md  # New rule IDs (active sprint)

src/BlazorWebFormsComponents.Analyzers.Test/
├── *AnalyzerTests.cs              # Tests for DiagnosticAnalyzer
├── *CodeFixTests.cs               # Tests for CodeFixProvider
├── AllAnalyzersIntegrationTests.cs # Meta-test verifying all rules have tests
└── ...
```

### File Naming Convention

For each rule, create a pair of files:

1. **Analyzer:** `{Feature}Analyzer.cs` (e.g., `ResponseRedirectAnalyzer.cs`)
   - Inherits `DiagnosticAnalyzer`
   - Defines `DiagnosticDescriptor` with rule ID, title, and category
   - Implements `Initialize()` to register syntax node callbacks

2. **Code Fix Provider:** `{Feature}CodeFixProvider.cs` (e.g., `ResponseRedirectCodeFixProvider.cs`)
   - Inherits `CodeFixProvider`
   - Implements `RegisterCodeFixesAsync()` to transform the syntax tree

### Rule ID Assignment

Rule IDs follow the pattern `BWFC{NNN}` where `{NNN}` is a 3-digit number. Before implementing, assign a new ID:

1. Check `AnalyzerReleases.Shipped.md` to see what's already released
2. Check `AnalyzerReleases.Unshipped.md` to see what's in active development
3. Pick the next unused number
4. Add a placeholder entry to `AnalyzerReleases.Unshipped.md` when you start

Current assignments:
- `BWFC001` – `BWFC012`: Shipped in v1.0.0
- `BWFC013`, `BWFC014`, etc.: Under active development

---

## Anatomy of a DiagnosticAnalyzer

Here's the minimum structure:

```csharp
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BlazorWebFormsComponents.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MyFeatureAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "BWFC999";

        private static readonly LocalizableString Title = "Title of rule";
        private static readonly LocalizableString MessageFormat = "Message: {0}";
        private static readonly LocalizableString Description = "Detailed description...";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            
            // Register one or more syntax node actions
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.MethodDeclaration);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var node = (MethodDeclarationSyntax)context.Node;

            // Detection logic here
            if (ShouldReport(node))
            {
                var diagnostic = Diagnostic.Create(Rule, node.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool ShouldReport(MethodDeclarationSyntax node)
        {
            // Your pattern detection logic
            return false;
        }
    }
}
```

### Key Decisions

**SyntaxKind callbacks:** Choose based on what you're looking for:
- `InvocationExpression` — method/function calls (e.g., `Response.Redirect()`)
- `PropertyDeclaration` — property definitions (e.g., missing `[Parameter]`)
- `MethodDeclaration` — method definitions (e.g., old event handler signatures)
- `IdentifierName` — references to identifiers (e.g., `ViewState["key"]`)
- `StringLiteral` — string literals (e.g., `runat="server"`)

**Configuration flags:**
- `ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None)` — Analyze generated code (usually desired)
- `EnableConcurrentExecution()` — Allow Roslyn to parallelize analysis

**Severity levels:** (in ascending order)
- `Hidden` — Doesn't appear in the Error List (rarely used)
- `Info` — Blue squiggle (e.g., BWFC010, BWFC011)
- `Warning` — Yellow squiggle (e.g., BWFC001–BWFC005)
- `Error` — Red squiggle (rare; use only for build-breaking violations)

---

## Anatomy of a CodeFixProvider

Here's the minimum structure:

```csharp
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MyFeatureCodeFixProvider)), Shared]
    public class MyFeatureCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(MyFeatureAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() =>
            WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var node = root.FindNode(diagnosticSpan);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "My fix title",
                    createChangedDocument: c => ApplyFixAsync(context.Document, node, c),
                    equivalenceKey: "my-fix-key"),
                diagnostic);
        }

        private async Task<Document> ApplyFixAsync(Document document, SyntaxNode node, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            // Transform the syntax tree
            var newNode = Transform(node);

            var newRoot = root.ReplaceNode(node, newNode);
            return document.WithSyntaxRoot(newRoot);
        }

        private SyntaxNode Transform(SyntaxNode node)
        {
            // Your transformation logic here
            return node;
        }
    }
}
```

### Key Patterns

**Batch fixing:** Always use `WellKnownFixAllProviders.BatchFixer` so the user can apply all fixes in a document with a single action (Ctrl+. → "Fix all BWFC999 in document").

**Cancellation tokens:** Always pass `ConfigureAwait(false)` on async calls to avoid deadlock in UI thread contexts.

**Node transformation:** Use `SyntaxFactory` to create new nodes:

```csharp
// Add an attribute to a property
var newAttribute = SyntaxFactory.Attribute(
    SyntaxFactory.IdentifierName("Parameter"));

var newAttributeList = SyntaxFactory.AttributeList(
    SyntaxFactory.SingletonSeparatedList(newAttribute));

var newProperty = property.AddAttributeLists(newAttributeList);
```

---

## Testing Strategy

### CSharpAnalyzerTest Pattern

Use the xUnit + `Microsoft.CodeAnalysis.CSharp.Testing` framework:

```csharp
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace BlazorWebFormsComponents.Analyzers.Test;

using AnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    MyFeatureAnalyzer,
    DefaultVerifier>;

public class MyFeatureAnalyzerTests
{
    [Fact]
    public async Task DetectsPattern_ReportsDiagnostic()
    {
        var source = @"
public class MyClass
{
    public void MyMethod()
    {
        // Pattern that triggers BWFC999
    }
}
";

        var expected = new DiagnosticResult(MyFeatureAnalyzer.DiagnosticId, DiagnosticSeverity.Warning)
            .WithArguments("expected message args");

        var test = new AnalyzerTest
        {
            TestCode = source,
            ExpectedDiagnostics = { expected },
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task NonMatchingPattern_NoDiagnostic()
    {
        var source = @"
public class MyClass
{
    public void MyMethod()
    {
        // Pattern that does NOT trigger BWFC999
    }
}
";

        var test = new AnalyzerTest { TestCode = source };
        await test.RunAsync();
    }
}
```

### CSharpCodeFixTest Pattern

Test both the analyzer and the code fix together:

```csharp
using System.Threading.Tasks;

namespace BlazorWebFormsComponents.Analyzers.Test;

using CodeFixTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    MyFeatureAnalyzer,
    MyFeatureCodeFixProvider,
    DefaultVerifier>;

public class MyFeatureCodeFixTests
{
    [Fact]
    public async Task AppliesFix_Correctly()
    {
        var source = @"
public class MyClass
{
    public void MyMethod()
    {
        // Code with pattern BWFC999
    }
}
";

        var fixedSource = @"
public class MyClass
{
    public void MyMethod()
    {
        // Code after fix is applied
    }
}
";

        var test = new CodeFixTest
        {
            TestCode = source,
            FixedCode = fixedSource,
        };

        await test.RunAsync();
    }
}
```

### Stub Types for Testing

When your analyzer detects patterns that depend on external types (e.g., `ViewState`, `Response`, `HttpContext`), provide minimal stub implementations in the test source:

```csharp
private const string HttpStubs = @"
public class HttpResponse
{
    public void Redirect(string url) { }
}

public class HttpContext
{
    public static HttpResponse Response { get; set; }
}
";

[Fact]
public async Task ResponseRedirect_WithHttpContext_ReportsDiagnostic()
{
    var source = HttpStubs + @"
public class MyPage
{
    public void Submit()
    {
        HttpContext.Response.Redirect(""~/home"");
    }
}
";

    var test = new AnalyzerTest
    {
        TestCode = source,
        ExpectedDiagnostics = { new DiagnosticResult(...) },
    };

    await test.RunAsync();
}
```

### Diagnostic Location Markers: {|#N:code|}

For complex scenarios, use diagnostic location markers to verify the exact span:

```csharp
var source = @"
public class MyPage
{
    public void MyMethod()
    {
        {|#0:Response.Redirect(""url"")|};
    }
}
";

var expected = new DiagnosticResult(MyFeatureAnalyzer.DiagnosticId, DiagnosticSeverity.Warning)
    .WithLocationAndMessage(new CodeAnalysis.Text.TextSpan(marker[0].Span));

var test = new AnalyzerTest { TestCode = source, ExpectedDiagnostics = { expected } };
```

---

## Common Pitfalls

### 1. Trivia Handling (Whitespace & Comments)

Trivia — leading/trailing whitespace and comments — is critical when replacing or removing code.

❌ **WRONG:**
```csharp
// Loses indentation and EndOfLine trivia
var newNode = SyntaxFactory.EmptyStatement();
var newRoot = root.ReplaceNode(statement, newNode);
```

✅ **CORRECT:**
```csharp
// Preserve indentation
var leading = statement.GetLeadingTrivia()
    .Add(SyntaxFactory.Comment("// TODO comment"))
    .Add(SyntaxFactory.EndOfLine("\r\n"))
    .AddRange(indentationTrivia);

var newNode = SyntaxFactory.EmptyStatement()
    .WithLeadingTrivia(leading)
    .WithTrailingTrivia(statement.GetTrailingTrivia());

var newRoot = root.ReplaceNode(statement, newNode);
```

### 2. Null Checks on Ancestor Traversal

When walking up the syntax tree, ancestors may not exist. Always guard:

❌ **WRONG:**
```csharp
var method = invocation.FirstAncestorOrSelf<MethodDeclarationSyntax>();
var methodName = method.Identifier.Text; // Null reference exception
```

✅ **CORRECT:**
```csharp
var method = invocation.FirstAncestorOrSelf<MethodDeclarationSyntax>();
var methodName = method?.Identifier.Text ?? "<unknown>";
```

### 3. SyntaxKind Selection

Choose the right hook for your pattern. Common mistakes:

| ❌ WRONG | ✅ CORRECT | Reason |
|---------|----------|--------|
| Hook `InvocationExpression` for `ViewState["key"]` | Hook `ElementAccessExpression` | ViewState access is not a method call |
| Hook `IdentifierName` for `Response` object | Hook `MemberAccessExpression` | You want `Response.Redirect`, not every identifier |
| Hook `StringLiteral` for `runat="server"` but don't check value | Hook `StringLiteral` AND inspect `LiteralExpression.Token.Value` | Avoid false positives on unrelated strings |

### 4. Using `.ToString()` vs Syntax API

Avoid string comparisons when the Roslyn API provides structured access:

❌ **WRONG:**
```csharp
if (memberAccess.Expression.ToString() == "Response")
{
    // Fragile: "Response" by itself isn't enough, could be a local variable
}
```

✅ **CORRECT:**
```csharp
if (memberAccess.Expression is IdentifierNameSyntax identifier &&
    identifier.Identifier.Text == "Response")
{
    // More precise: specifically check for an identifier named "Response"
}
```

### 5. Message Format Arguments

Keep message format strings simple and pass only what the user needs:

❌ **WRONG:**
```csharp
new DiagnosticResult(MyAnalyzer.DiagnosticId)
    .WithArguments("Response.Redirect()", "NavigationManager.NavigateTo()", 
                   "~", "/", methodName, className, fileName)
```

✅ **CORRECT:**
```csharp
new DiagnosticResult(MyAnalyzer.DiagnosticId)
    .WithArguments(methodName)
```

The format string is: `"'{0}' uses Response.Redirect — use NavigationManager.NavigateTo() in Blazor instead"`
The argument is just the method name.

---

## PR Checklist for New Rules

Before submitting a pull request with a new analyzer rule, verify:

### Analyzer Implementation
- [ ] **Rule ID assigned** — Picked next unused BWFC{NNN}, added to `AnalyzerReleases.Unshipped.md`
- [ ] **DiagnosticDescriptor complete** — Title, MessageFormat, Description, Category, Severity all populated
- [ ] **SyntaxKind callbacks registered** — Correct callback type for the pattern (not too broad)
- [ ] **No string-based name checks** — Using Roslyn syntax API, not `.ToString()` comparisons
- [ ] **Null guards on traversal** — `.FirstAncestorOrSelf<T>()` results checked before dereferencing
- [ ] **Documentation in code** — XML comments on class and key methods
- [ ] **Build passes** — `dotnet build` with 0 errors, 0 warnings

### Code Fix Provider
- [ ] **Inherits CodeFixProvider** with `[ExportCodeFixProvider]` attribute
- [ ] **FixableDiagnosticIds returns rule ID** — Matches the analyzer's `DiagnosticId`
- [ ] **GetFixAllProvider returns BatchFixer** — Users can apply all fixes in document
- [ ] **RegisterCodeFixesAsync implemented** — Registers at least one code action
- [ ] **Trivia preserved** — Leading/trailing whitespace and comments handled correctly
- [ ] **Cancellation tokens used** — All async calls include `cancellationToken`
- [ ] **ConfigureAwait(false)** — All `await` calls to avoid deadlock

### Testing
- [ ] **Positive cases** — Tests where the rule SHOULD fire (with expected diagnostic)
- [ ] **Negative cases** — Tests where the rule should NOT fire (clean source)
- [ ] **Edge cases** — Inheritance, generic types, null conditions, etc.
- [ ] **Code fix tests** — FixedCode matches before/after transformation
- [ ] **Stub types provided** — External type stubs in test source (e.g., `ViewState`, `Response`)
- [ ] **All tests pass** — `dotnet test` with 0 failures
- [ ] **AllAnalyzersIntegrationTests.cs updated** — New rule ID added to the meta-test

### Documentation
- [ ] **docs/Migration/Analyzers.md updated** — New rule section with:
  - What it detects
  - Why it matters
  - Before/After code example
  - Code fix description
  - Any related utility features linked
- [ ] **Example is clear** — Before/After pair shows actual Web Forms pattern and Blazor migration
- [ ] **Severity level justified** — Rationale for Warning vs Info in prose
- [ ] **Related analyzer rules cross-referenced** — If BWFC003 and BWFC004 are related, they mention each other

### Integration
- [ ] **AnalyzerReleases.Unshipped.md updated** — Rule ID, category, severity, and description
- [ ] **No breaking changes** — Shipped rule IDs never removed or renumbered
- [ ] **Severity settings documented** — If different from default, note in Analyzers.md

---

## Reference Implementation: ResponseRedirectAnalyzer

For a complete working example, see:
- **Analyzer:** `src/BlazorWebFormsComponents.Analyzers/ResponseRedirectAnalyzer.cs`
- **Code Fix:** `src/BlazorWebFormsComponents.Analyzers/ResponseRedirectCodeFixProvider.cs`
- **Tests:** `src/BlazorWebFormsComponents.Analyzers.Test/ResponseRedirectAnalyzerTests.cs`

This rule detects `Response.Redirect()` calls and replaces them with a TODO comment pointing to `NavigationManager.NavigateTo()`.

---

## Build & Test Commands

```bash
# Build the analyzer package
dotnet build src/BlazorWebFormsComponents.Analyzers

# Run all analyzer tests
dotnet test src/BlazorWebFormsComponents.Analyzers.Test

# Run a specific test
dotnet test src/BlazorWebFormsComponents.Analyzers.Test -k "ResponseRedirect"

# Pack the analyzer NuGet package
dotnet pack src/BlazorWebFormsComponents.Analyzers -c Release
```

---

## Questions?

Refer to:
- [Roslyn Analyzer Documentation](https://github.com/dotnet/roslyn-analyzers/wiki)
- [CodeAnalysis.Testing Framework](https://github.com/dotnet/roslyn-sdk/tree/main/src/Microsoft.CodeAnalysis.Testing)
- Existing rules in the repo (BWFC001–BWFC012)

