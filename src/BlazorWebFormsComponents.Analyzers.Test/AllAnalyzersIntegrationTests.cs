using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Reflection;

namespace BlazorWebFormsComponents.Analyzers.Test;

/// <summary>
/// Cross-analyzer integration tests. Verifies that all BWFC analyzers
/// are registered, have unique IDs, and fire correctly in combination.
/// </summary>
public class AllAnalyzersIntegrationTests
{
    /// <summary>
    /// All analyzer types in the assembly, discovered via reflection.
    /// </summary>
    private static readonly Type[] AllAnalyzerTypes = typeof(MissingParameterAttributeAnalyzer)
        .Assembly
        .GetTypes()
        .Where(t => !t.IsAbstract && typeof(DiagnosticAnalyzer).IsAssignableFrom(t))
        .OrderBy(t => t.Name)
        .ToArray();

    /// <summary>
    /// Expected diagnostic IDs. BWFC011 is pending from Cyclops — add it here when delivered.
    /// </summary>
    private static readonly string[] ExpectedIds = new[]
    {
        "BWFC001", // MissingParameterAttributeAnalyzer
        "BWFC002", // ViewStateUsageAnalyzer
        "BWFC003", // IsPostBackUsageAnalyzer
        "BWFC004", // ResponseRedirectAnalyzer
        "BWFC005", // SessionUsageAnalyzer
        "BWFC010", // RequiredAttributeAnalyzer
        "BWFC011", // EventHandlerSignatureAnalyzer
        "BWFC012", // RunatServerAnalyzer
    };

    #region ID Registration and Uniqueness

    [Fact]
    public void AllAnalyzerTypes_AreDiscoverable()
    {
        // Sanity check: we should find at least the 8 analyzers we know exist
        Assert.True(AllAnalyzerTypes.Length >= 8,
            $"Expected at least 8 analyzer types but found {AllAnalyzerTypes.Length}: " +
            string.Join(", ", AllAnalyzerTypes.Select(t => t.Name)));
    }

    [Fact]
    public void AllDiagnosticIds_AreUnique()
    {
        var ids = new List<(string Id, string AnalyzerName)>();

        foreach (var analyzerType in AllAnalyzerTypes)
        {
            var analyzer = CreateAnalyzer(analyzerType);
            foreach (var descriptor in analyzer.SupportedDiagnostics)
            {
                ids.Add((descriptor.Id, analyzerType.Name));
            }
        }

        var duplicates = ids
            .GroupBy(x => x.Id)
            .Where(g => g.Count() > 1)
            .Select(g => $"{g.Key} is claimed by: {string.Join(", ", g.Select(x => x.AnalyzerName))}")
            .ToList();

        Assert.True(duplicates.Count == 0,
            "Duplicate diagnostic IDs found:\n" + string.Join("\n", duplicates));
    }

    [Fact]
    public void AllExpectedIds_AreRegistered()
    {
        var registeredIds = new HashSet<string>();

        foreach (var analyzerType in AllAnalyzerTypes)
        {
            var analyzer = CreateAnalyzer(analyzerType);
            foreach (var descriptor in analyzer.SupportedDiagnostics)
            {
                registeredIds.Add(descriptor.Id);
            }
        }

        var missingIds = ExpectedIds.Where(id => !registeredIds.Contains(id)).ToList();

        Assert.True(missingIds.Count == 0,
            "Expected diagnostic IDs not found in any analyzer: " + string.Join(", ", missingIds) +
            "\nRegistered IDs: " + string.Join(", ", registeredIds.OrderBy(x => x)));
    }

    [Fact]
    public void AllAnalyzers_HaveUsageCategory()
    {
        foreach (var analyzerType in AllAnalyzerTypes)
        {
            var analyzer = CreateAnalyzer(analyzerType);
            foreach (var descriptor in analyzer.SupportedDiagnostics)
            {
                Assert.Equal("Usage", descriptor.Category);
            }
        }
    }

    [Fact]
    public void AllAnalyzers_AreEnabledByDefault()
    {
        foreach (var analyzerType in AllAnalyzerTypes)
        {
            var analyzer = CreateAnalyzer(analyzerType);
            foreach (var descriptor in analyzer.SupportedDiagnostics)
            {
                Assert.True(descriptor.IsEnabledByDefault,
                    $"{descriptor.Id} ({analyzerType.Name}) is not enabled by default");
            }
        }
    }

    #endregion

    #region Multi-Analyzer Integration

    /// <summary>
    /// A WebControl subclass with multiple migration issues should trigger
    /// BWFC001 (missing [Parameter]), BWFC002 (ViewState), and BWFC003 (IsPostBack) simultaneously.
    /// </summary>
    [Fact]
    public async Task ComplexWebControl_TriggersMultipleDiagnostics()
    {
        var source = @"
using System.Collections.Generic;

namespace BlazorWebFormsComponents.CustomControls
{
    public class WebControl
    {
        public string ID { get; set; }
        public Dictionary<string, object> ViewState { get; } = new Dictionary<string, object>();
        public bool IsPostBack { get; set; }
        public WebControl Page => this;
    }
}

namespace MyApp
{
    using BlazorWebFormsComponents.CustomControls;

    public class LegacyGrid : WebControl
    {
        // BWFC001: public property without [Parameter]
        public string DataSourceID { get; set; }

        // BWFC001: another missing [Parameter]
        public bool AllowPaging { get; set; }

        public void Page_Load()
        {
            // BWFC003: IsPostBack check
            if (!IsPostBack)
            {
                // BWFC002: ViewState usage
                ViewState[""initialized""] = true;
            }
        }
    }
}";

        var analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(
            new MissingParameterAttributeAnalyzer(),
            new ViewStateUsageAnalyzer(),
            new IsPostBackUsageAnalyzer());

        var diagnostics = await GetDiagnosticsAsync(source, analyzers);

        var diagnosticIds = diagnostics.Select(d => d.Id).ToList();

        Assert.Contains("BWFC001", diagnosticIds);
        Assert.Contains("BWFC002", diagnosticIds);
        Assert.Contains("BWFC003", diagnosticIds);

        // BWFC001 should fire twice (DataSourceID + AllowPaging)
        var bwfc001Count = diagnosticIds.Count(id => id == "BWFC001");
        Assert.Equal(2, bwfc001Count);

        // BWFC002 should fire once (ViewState["initialized"])
        var bwfc002Count = diagnosticIds.Count(id => id == "BWFC002");
        Assert.Equal(1, bwfc002Count);

        // BWFC003 should fire once (IsPostBack)
        var bwfc003Count = diagnosticIds.Count(id => id == "BWFC003");
        Assert.Equal(1, bwfc003Count);
    }

    /// <summary>
    /// A single file with issues spanning 5+ analyzers: missing [Parameter],
    /// ViewState, IsPostBack, Session, runat="server", and event handler signature.
    /// </summary>
    [Fact]
    public async Task KitchenSink_FiveAnalyzersFire()
    {
        var source = @"
using System;
using System.Collections.Generic;

namespace BlazorWebFormsComponents.CustomControls
{
    public class WebControl
    {
        public string ID { get; set; }
        public Dictionary<string, object> ViewState { get; } = new Dictionary<string, object>();
        public bool IsPostBack { get; set; }
        public WebControl Page => this;
        public Dictionary<string, object> Session { get; } = new Dictionary<string, object>();
    }
}

namespace MyApp
{
    using BlazorWebFormsComponents.CustomControls;

    public class MegaPage : WebControl
    {
        // BWFC001: missing [Parameter]
        public string Title { get; set; }

        public void Page_Load()
        {
            // BWFC003: IsPostBack
            if (!IsPostBack)
            {
                // BWFC002: ViewState
                ViewState[""loaded""] = true;
            }

            // BWFC005: Session usage
            Session[""user""] = ""admin"";

            // BWFC012: runat=""server"" in string
            var legacy = ""<asp:Label runat=\""server\"" Text=\""Hello\"" />"";
        }

        // BWFC011: Web Forms event handler signature
        protected void Button_Click(object sender, EventArgs e)
        {
        }
    }
}";

        var analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(
            new MissingParameterAttributeAnalyzer(),
            new ViewStateUsageAnalyzer(),
            new IsPostBackUsageAnalyzer(),
            new SessionUsageAnalyzer(),
            new RunatServerAnalyzer(),
            new EventHandlerSignatureAnalyzer());

        var diagnostics = await GetDiagnosticsAsync(source, analyzers);

        var diagnosticIds = diagnostics.Select(d => d.Id).Distinct().OrderBy(id => id).ToList();

        Assert.Contains("BWFC001", diagnosticIds);
        Assert.Contains("BWFC002", diagnosticIds);
        Assert.Contains("BWFC003", diagnosticIds);
        Assert.Contains("BWFC005", diagnosticIds);
        Assert.Contains("BWFC011", diagnosticIds);
        Assert.Contains("BWFC012", diagnosticIds);
    }

    /// <summary>
    /// Clean code that follows Blazor patterns should produce NO diagnostics
    /// from any analyzer in the suite.
    /// </summary>
    [Fact]
    public async Task CleanBlazorComponent_NoDiagnostics()
    {
        var source = @"
namespace BlazorWebFormsComponents.CustomControls
{
    public class WebControl
    {
        public string ID { get; set; }
    }
}

namespace Microsoft.AspNetCore.Components
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class ParameterAttribute : System.Attribute { }
}

namespace MyApp
{
    using BlazorWebFormsComponents.CustomControls;
    using Microsoft.AspNetCore.Components;

    public class CleanComponent : WebControl
    {
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public int PageSize { get; set; }

        public void OnInitialized()
        {
            var message = ""Hello, Blazor!"";
        }
    }
}";

        var analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(
            new MissingParameterAttributeAnalyzer(),
            new ViewStateUsageAnalyzer(),
            new IsPostBackUsageAnalyzer(),
            new ResponseRedirectAnalyzer(),
            new SessionUsageAnalyzer(),
            new RunatServerAnalyzer());

        var diagnostics = await GetDiagnosticsAsync(source, analyzers);

        Assert.Empty(diagnostics);
    }

    #endregion

    #region Helpers

    private static DiagnosticAnalyzer CreateAnalyzer(Type analyzerType) =>
        (DiagnosticAnalyzer)(Activator.CreateInstance(analyzerType)
            ?? throw new InvalidOperationException($"Could not create instance of {analyzerType.Name}"));

    private static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(
        string source,
        ImmutableArray<DiagnosticAnalyzer> analyzers)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Collections").Location),
        };

        var compilation = CSharpCompilation.Create(
            "IntegrationTest",
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var compilationWithAnalyzers = compilation.WithAnalyzers(analyzers);
        var allDiagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();

        // Filter to only our BWFC diagnostics
        return allDiagnostics
            .Where(d => d.Id.StartsWith("BWFC"))
            .ToImmutableArray();
    }

    #endregion
}
