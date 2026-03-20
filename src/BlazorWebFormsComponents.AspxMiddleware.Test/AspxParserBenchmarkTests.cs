using System.Diagnostics;
using System.Text;
using BlazorWebFormsComponents.AspxMiddleware;
using Shouldly;
using Xunit.Abstractions;

namespace BlazorWebFormsComponents.AspxMiddleware.Test;

/// <summary>
/// Performance benchmarks for the AngleSharp-based AspxParser.
/// Measures parse time, throughput, and memory allocation across input sizes.
/// Run with: dotnet test --filter "FullyQualifiedName~Benchmark" -v n
/// </summary>
public class AspxParserBenchmarkTests
{
    private const int Iterations = 1000;
    private const int WarmupIterations = 50;
    private readonly ITestOutputHelper _output;

    public AspxParserBenchmarkTests(ITestOutputHelper output)
    {
        _output = output;
    }

    // ── Size-based benchmarks ──────────────────────────────────────────

    [Fact]
    public void Benchmark_SmallInput_10Lines()
    {
        var input = GenerateSmallInput();
        var lineCount = CountLines(input);
        _output.WriteLine($"Input size: {input.Length} chars, {lineCount} lines");

        RunBenchmark("Small (~10 lines)", input);
    }

    [Fact]
    public void Benchmark_MediumInput_50Lines()
    {
        var input = GenerateMediumInput();
        var lineCount = CountLines(input);
        _output.WriteLine($"Input size: {input.Length} chars, {lineCount} lines");

        RunBenchmark("Medium (~50 lines)", input);
    }

    [Fact]
    public void Benchmark_LargeInput_200Lines()
    {
        var input = GenerateLargeInput();
        var lineCount = CountLines(input);
        _output.WriteLine($"Input size: {input.Length} chars, {lineCount} lines");

        RunBenchmark("Large (~200 lines)", input);
    }

    [Fact]
    public void Benchmark_ExtraLargeInput_500Lines()
    {
        var input = GenerateExtraLargeInput();
        var lineCount = CountLines(input);
        _output.WriteLine($"Input size: {input.Length} chars, {lineCount} lines");

        RunBenchmark("XL (~500 lines)", input);
    }

    // ── AngleSharp-specific scenario benchmarks ────────────────────────

    [Fact]
    public void Benchmark_UnclosedTags()
    {
        var input = GenerateUnclosedTagsInput();
        _output.WriteLine($"Input size: {input.Length} chars, {CountLines(input)} lines");

        var result = AspxParser.Parse(input);
        _output.WriteLine($"Parsed nodes: {CountAllNodes(result)}");

        RunBenchmark("Unclosed <br>/<hr> tags", input);
    }

    [Fact]
    public void Benchmark_AmpersandEntities()
    {
        var input = GenerateEntityInput();
        _output.WriteLine($"Input size: {input.Length} chars, {CountLines(input)} lines");

        var result = AspxParser.Parse(input);
        _output.WriteLine($"Parsed nodes: {CountAllNodes(result)}");

        RunBenchmark("& entities in attributes", input);
    }

    [Fact]
    public void Benchmark_SingleQuoteAttributes()
    {
        var input = GenerateSingleQuoteInput();
        _output.WriteLine($"Input size: {input.Length} chars, {CountLines(input)} lines");

        var result = AspxParser.Parse(input);
        _output.WriteLine($"Parsed nodes: {CountAllNodes(result)}");

        RunBenchmark("Single-quote attributes", input);
    }

    [Fact]
    public void Benchmark_ScriptBlocks()
    {
        var input = GenerateScriptBlockInput();
        _output.WriteLine($"Input size: {input.Length} chars, {CountLines(input)} lines");

        var result = AspxParser.Parse(input);
        _output.WriteLine($"Parsed nodes: {CountAllNodes(result)}");

        RunBenchmark("<script> with operators", input);
    }

    [Fact]
    public void Benchmark_Summary()
    {
        // Run all sizes and print a combined table
        var scenarios = new (string Name, string Input)[]
        {
            ("Small (~10 lines)", GenerateSmallInput()),
            ("Medium (~50 lines)", GenerateMediumInput()),
            ("Large (~200 lines)", GenerateLargeInput()),
            ("XL (~500 lines)", GenerateExtraLargeInput()),
            ("Stress 500KB+", Generate500KBStressInput()),
            ("Unclosed tags", GenerateUnclosedTagsInput()),
            ("& entities", GenerateEntityInput()),
            ("Single-quote attrs", GenerateSingleQuoteInput()),
            ("<script> blocks", GenerateScriptBlockInput()),
        };

        _output.WriteLine("");
        _output.WriteLine("=== AngleSharp Parser Benchmark Summary ===");
        _output.WriteLine($"{"Scenario",-25} {"Chars",8} {"Lines",6} {"Avg (ms)",10} {"Parses/sec",12} {"Alloc (KB)",11} {"Iters",6}");
        _output.WriteLine(new string('-', 80));

        foreach (var (name, input) in scenarios)
        {
            var stats = MeasureBenchmark(input);
            _output.WriteLine(
                $"{name,-25} {input.Length,8} {CountLines(input),6} {stats.AvgMs,10:F3} {stats.ParsesPerSec,12:N0} {stats.AllocKB,11:N1} {Iterations,6}");
        }

        _output.WriteLine(new string('-', 80));
        _output.WriteLine($"Warmup: {WarmupIterations} iterations (excluded). Measured: {Iterations} iterations.");
        _output.WriteLine($"GC Memory = Gen0 allocated bytes across all iterations / iteration count.");
    }

    // ── Core measurement ───────────────────────────────────────────────

    private void RunBenchmark(string label, string input)
    {
        var stats = MeasureBenchmark(input);

        _output.WriteLine("");
        _output.WriteLine($"=== AngleSharp Parser Benchmark: {label} ===");
        _output.WriteLine($"  Avg parse time:  {stats.AvgMs:F3} ms");
        _output.WriteLine($"  Min parse time:  {stats.MinMs:F3} ms");
        _output.WriteLine($"  Max parse time:  {stats.MaxMs:F3} ms");
        _output.WriteLine($"  Std deviation:   {stats.StdDevMs:F3} ms");
        _output.WriteLine($"  Throughput:      {stats.ParsesPerSec:N0} parses/sec");
        _output.WriteLine($"  GC alloc (avg):  {stats.AllocKB:N1} KB/parse");
        _output.WriteLine($"  Iterations:      {Iterations} (+ {WarmupIterations} warmup)");
    }

    private static BenchmarkStats MeasureBenchmark(string input)
    {
        // Warmup — let JIT compile and settle
        for (var i = 0; i < WarmupIterations; i++)
        {
            AspxParser.Parse(input);
        }

        // Force GC before measurement
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var timings = new double[Iterations];
        var sw = new Stopwatch();

        var memBefore = GC.GetTotalAllocatedBytes(precise: true);

        for (var i = 0; i < Iterations; i++)
        {
            sw.Restart();
            AspxParser.Parse(input);
            sw.Stop();
            timings[i] = sw.Elapsed.TotalMilliseconds;
        }

        var memAfter = GC.GetTotalAllocatedBytes(precise: true);
        var totalAllocBytes = memAfter - memBefore;
        var allocKBPerParse = totalAllocBytes / 1024.0 / Iterations;

        var avg = timings.Average();
        var min = timings.Min();
        var max = timings.Max();
        var variance = timings.Select(t => (t - avg) * (t - avg)).Average();
        var stdDev = Math.Sqrt(variance);
        var parsesPerSec = avg > 0 ? 1000.0 / avg : double.PositiveInfinity;

        return new BenchmarkStats
        {
            AvgMs = avg,
            MinMs = min,
            MaxMs = max,
            StdDevMs = stdDev,
            ParsesPerSec = parsesPerSec,
            AllocKB = allocKBPerParse
        };
    }

    private record struct BenchmarkStats
    {
        public double AvgMs;
        public double MinMs;
        public double MaxMs;
        public double StdDevMs;
        public double ParsesPerSec;
        public double AllocKB;
    }

    // ── Input generators ───────────────────────────────────────────────

    /// <summary>Small: 1 directive + 2 controls (~10 lines)</summary>
    private static string GenerateSmallInput()
    {
        return """
            <%@ Page Title="Small Test" Language="C#" %>
            <html>
            <body>
                <h1>Welcome</h1>
                <asp:Label ID="lblName" Text="Hello World" CssClass="label-primary" />
                <asp:Button ID="btnSubmit" Text="Submit" CssClass="btn btn-primary" />
            </body>
            </html>
            """;
    }

    /// <summary>Medium: 1 directive + expressions + 15 controls in panels (~50 lines)</summary>
    private static string GenerateMediumInput()
    {
        var sb = new StringBuilder();
        sb.AppendLine("""<%@ Page Title="Medium Test" Language="C#" MasterPageFile="~/Site.Master" %>""");
        sb.AppendLine("<html><body>");
        sb.AppendLine("<div class=\"container\">");

        // 3 panels with 5 controls each = 15 controls
        for (var p = 1; p <= 3; p++)
        {
            sb.AppendLine($"  <asp:Panel ID=\"panel{p}\" CssClass=\"panel panel-default\">");
            sb.AppendLine($"    <h2>Section {p}</h2>");
            sb.AppendLine($"    <asp:Label ID=\"lbl{p}_1\" Text=\"Label {p}.1\" CssClass=\"field-label\" />");
            sb.AppendLine($"    <asp:TextBox ID=\"txt{p}_1\" CssClass=\"form-control\" />");
            sb.AppendLine($"    <asp:RequiredFieldValidator ID=\"rfv{p}\" ControlToValidate=\"txt{p}_1\" ErrorMessage=\"Required\" />");
            sb.AppendLine($"    <asp:Label ID=\"lbl{p}_2\" Text=\"Label {p}.2\" />");
            sb.AppendLine($"    <asp:DropDownList ID=\"ddl{p}\" CssClass=\"form-select\" />");
            sb.AppendLine($"  </asp:Panel>");
        }

        sb.AppendLine("<%= DateTime.Now.ToString(\"yyyy-MM-dd\") %>");
        sb.AppendLine("<%# Item.Name %>");
        sb.AppendLine("</div>");
        sb.AppendLine("</body></html>");

        return sb.ToString();
    }

    /// <summary>Large: directive + master page ref + expressions + 30+ controls with nesting (~200 lines)</summary>
    private static string GenerateLargeInput()
    {
        var sb = new StringBuilder();
        sb.AppendLine("""<%@ Page Title="Large Test" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="App.Default" %>""");
        sb.AppendLine("""<%@ Import Namespace="System.Data" %>""");
        sb.AppendLine("<html><head><title>Large Page</title></head><body>");
        sb.AppendLine("<form id=\"form1\" runat=\"server\">");

        // Header section
        sb.AppendLine("<div class=\"header\">");
        sb.AppendLine("  <asp:LoginView ID=\"loginView1\">");
        sb.AppendLine("    <asp:Label ID=\"lblWelcome\" Text=\"Welcome\" CssClass=\"welcome\" />");
        sb.AppendLine("  </asp:LoginView>");
        sb.AppendLine("  <asp:Menu ID=\"navMenu\" CssClass=\"nav-menu\" Orientation=\"Horizontal\" />");
        sb.AppendLine("</div>");

        // Main content — 5 form sections with 6 controls each = 30 controls
        for (var s = 1; s <= 5; s++)
        {
            sb.AppendLine($"<div class=\"section-{s}\">");
            sb.AppendLine($"  <asp:Panel ID=\"pnlSection{s}\" CssClass=\"panel\">");
            sb.AppendLine($"    <h3>Section {s}</h3>");
            sb.AppendLine($"    <asp:Label ID=\"lblField{s}_1\" Text=\"Field {s}.1:\" AssociatedControlID=\"txt{s}_1\" />");
            sb.AppendLine($"    <asp:TextBox ID=\"txt{s}_1\" CssClass=\"form-control\" MaxLength=\"100\" />");
            sb.AppendLine($"    <asp:RequiredFieldValidator ID=\"rfv{s}_1\" ControlToValidate=\"txt{s}_1\" ErrorMessage=\"Required\" Display=\"Dynamic\" />");
            sb.AppendLine($"    <asp:Label ID=\"lblField{s}_2\" Text=\"Field {s}.2:\" />");
            sb.AppendLine($"    <asp:DropDownList ID=\"ddl{s}\" CssClass=\"form-select\" AutoPostBack=\"true\" />");
            sb.AppendLine($"    <asp:CheckBox ID=\"chk{s}\" Text=\"Enable {s}\" CssClass=\"form-check\" />");
            sb.AppendLine($"  </asp:Panel>");
            sb.AppendLine($"</div>");
        }

        // GridView section
        sb.AppendLine("<div class=\"grid-section\">");
        sb.AppendLine("  <asp:GridView ID=\"gvProducts\" CssClass=\"table table-striped\" AutoGenerateColumns=\"false\">");
        sb.AppendLine("    <asp:BoundField DataField=\"Name\" HeaderText=\"Product Name\" />");
        sb.AppendLine("    <asp:BoundField DataField=\"Price\" HeaderText=\"Price\" DataFormatString=\"{0:C}\" />");
        sb.AppendLine("    <asp:BoundField DataField=\"Category\" HeaderText=\"Category\" />");
        sb.AppendLine("  </asp:GridView>");
        sb.AppendLine("</div>");

        // Expressions
        sb.AppendLine("<div class=\"footer\">");
        sb.AppendLine("  <p>Generated: <%= DateTime.Now %></p>");
        sb.AppendLine("  <p>Version: <%= System.Reflection.Assembly.GetExecutingAssembly().GetName().Version %></p>");
        sb.AppendLine("  <%# Eval(\"TotalCount\") %>");
        sb.AppendLine("</div>");

        // Additional miscellaneous controls
        sb.AppendLine("<asp:HiddenField ID=\"hfState\" Value=\"active\" />");
        sb.AppendLine("<asp:Literal ID=\"litScript\" Mode=\"PassThrough\" />");
        sb.AppendLine("<asp:PlaceHolder ID=\"phDynamic\" />");

        sb.AppendLine("</form>");
        sb.AppendLine("</body></html>");

        return sb.ToString();
    }

    /// <summary>XL: 100+ controls stress test (~500 lines)</summary>
    private static string GenerateExtraLargeInput()
    {
        var sb = new StringBuilder();
        sb.AppendLine("""<%@ Page Title="XL Stress Test" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="App.Default" %>""");
        sb.AppendLine("""<%@ Import Namespace="System.Data" %>""");
        sb.AppendLine("""<%@ Import Namespace="System.Linq" %>""");
        sb.AppendLine("""<%@ Register TagPrefix="uc" TagName="Header" Src="~/Controls/Header.ascx" %>""");
        sb.AppendLine("<html><head><title>XL Stress Test</title></head><body>");
        sb.AppendLine("<form id=\"form1\" runat=\"server\">");

        // 10 repeating form sections, each with 10+ controls = 100+ controls
        for (var s = 1; s <= 10; s++)
        {
            sb.AppendLine($"<div class=\"section\" id=\"section-{s}\">");
            sb.AppendLine($"  <asp:Panel ID=\"pnl{s}\" CssClass=\"panel panel-default\" Visible=\"true\">");
            sb.AppendLine($"    <asp:Panel ID=\"pnl{s}_header\" CssClass=\"panel-heading\">");
            sb.AppendLine($"      <asp:Label ID=\"lbl{s}_title\" Text=\"Section {s}\" Font-Bold=\"true\" Font-Size=\"14pt\" />");
            sb.AppendLine($"    </asp:Panel>");
            sb.AppendLine($"    <asp:Panel ID=\"pnl{s}_body\" CssClass=\"panel-body\">");

            // Text input + validator
            sb.AppendLine($"      <asp:Label ID=\"lbl{s}_name\" Text=\"Name:\" AssociatedControlID=\"txt{s}_name\" />");
            sb.AppendLine($"      <asp:TextBox ID=\"txt{s}_name\" CssClass=\"form-control\" MaxLength=\"200\" Placeholder=\"Enter name\" />");
            sb.AppendLine($"      <asp:RequiredFieldValidator ID=\"rfv{s}_name\" ControlToValidate=\"txt{s}_name\" ErrorMessage=\"Name is required\" Display=\"Dynamic\" CssClass=\"text-danger\" />");

            // Email input + regex validator
            sb.AppendLine($"      <asp:Label ID=\"lbl{s}_email\" Text=\"Email:\" AssociatedControlID=\"txt{s}_email\" />");
            sb.AppendLine($"      <asp:TextBox ID=\"txt{s}_email\" CssClass=\"form-control\" TextMode=\"Email\" />");
            sb.AppendLine($"      <asp:RegularExpressionValidator ID=\"rev{s}_email\" ControlToValidate=\"txt{s}_email\" ValidationExpression=\".*@.*\\..*\" ErrorMessage=\"Invalid email\" />");

            // Dropdown
            sb.AppendLine($"      <asp:DropDownList ID=\"ddl{s}_category\" CssClass=\"form-select\" AutoPostBack=\"true\">");
            sb.AppendLine($"      </asp:DropDownList>");

            // Checkbox + RadioButton
            sb.AppendLine($"      <asp:CheckBox ID=\"chk{s}_active\" Text=\"Active\" Checked=\"true\" />");
            sb.AppendLine($"      <asp:RadioButton ID=\"rb{s}_opt1\" Text=\"Option A\" GroupName=\"group{s}\" />");
            sb.AppendLine($"      <asp:RadioButton ID=\"rb{s}_opt2\" Text=\"Option B\" GroupName=\"group{s}\" />");

            sb.AppendLine($"    </asp:Panel>");

            // Footer with buttons
            sb.AppendLine($"    <asp:Panel ID=\"pnl{s}_footer\" CssClass=\"panel-footer\">");
            sb.AppendLine($"      <asp:Button ID=\"btn{s}_save\" Text=\"Save\" CssClass=\"btn btn-primary\" />");
            sb.AppendLine($"      <asp:Button ID=\"btn{s}_cancel\" Text=\"Cancel\" CssClass=\"btn btn-secondary\" CausesValidation=\"false\" />");
            sb.AppendLine($"      <asp:HyperLink ID=\"lnk{s}_details\" Text=\"Details\" NavigateUrl=\"~/Details.aspx?id={s}\" CssClass=\"btn btn-link\" />");
            sb.AppendLine($"    </asp:Panel>");

            sb.AppendLine($"  </asp:Panel>");
            sb.AppendLine($"</div>");
        }

        // Expressions scattered throughout
        for (var e = 1; e <= 5; e++)
        {
            sb.AppendLine($"<p>Expression {e}: <%= DateTime.Now.AddDays({e}) %></p>");
        }

        // Data-bind expressions
        sb.AppendLine("<%# Container.DataItemIndex %>");
        sb.AppendLine("<%# Eval(\"ProductName\") %>");
        sb.AppendLine("<%# Eval(\"UnitPrice\", \"{0:C}\") %>");

        // Server comments
        sb.AppendLine("<%-- TODO: Remove this section before release --%>");
        sb.AppendLine("<%-- HACK: Workaround for issue #123 --%>");

        sb.AppendLine("</form></body></html>");

        return sb.ToString();
    }

    // ── AngleSharp-specific scenario inputs ─────────────────────────────

    /// <summary>Input with unclosed HTML5 void elements</summary>
    private static string GenerateUnclosedTagsInput()
    {
        var sb = new StringBuilder();
        sb.AppendLine("""<%@ Page Title="Unclosed Tags" Language="C#" %>""");
        sb.AppendLine("<html><body>");

        for (var i = 1; i <= 10; i++)
        {
            sb.AppendLine($"<asp:Label ID=\"lbl{i}\" Text=\"Label {i}\" />");
            sb.AppendLine("<br>");
            sb.AppendLine("<hr>");
            sb.AppendLine($"<input type=\"text\" name=\"field{i}\">");
            sb.AppendLine($"<img src=\"image{i}.png\">");
        }

        sb.AppendLine("</body></html>");
        return sb.ToString();
    }

    /// <summary>Input with &amp; entities in URLs and text</summary>
    private static string GenerateEntityInput()
    {
        var sb = new StringBuilder();
        sb.AppendLine("""<%@ Page Title="Entity Test" Language="C#" %>""");
        sb.AppendLine("<html><body>");

        for (var i = 1; i <= 10; i++)
        {
            sb.AppendLine($"""<asp:HyperLink ID="lnk{i}" NavigateUrl="page.aspx?id={i}&amp;view=details&amp;tab=summary" Text="Link {i}" />""");
            sb.AppendLine($"<a href=\"page.aspx?a={i}&amp;b=2&amp;c=3\">Regular link {i}</a>");
        }

        sb.AppendLine("</body></html>");
        return sb.ToString();
    }

    /// <summary>Input using single-quote attributes in directives and controls</summary>
    private static string GenerateSingleQuoteInput()
    {
        var sb = new StringBuilder();
        sb.AppendLine("""<%@ Page Title='Single Quote Page' Language='C#' MasterPageFile='~/Site.Master' %>""");
        sb.AppendLine("<html><body>");

        for (var i = 1; i <= 10; i++)
        {
            // Mix single and double quotes
            if (i % 2 == 0)
            {
                sb.AppendLine($"""<asp:Label ID="lbl{i}" Text='Label {i}' CssClass='field-label' />""");
            }
            else
            {
                sb.AppendLine($"""<asp:Label ID='lbl{i}' Text="Label {i}" CssClass="field-label" />""");
            }
            sb.AppendLine($"""<asp:TextBox ID='txt{i}' CssClass="form-control" MaxLength='100' />""");
        }

        sb.AppendLine("</body></html>");
        return sb.ToString();
    }

    /// <summary>Input with script blocks containing operators that break XML parsers</summary>
    private static string GenerateScriptBlockInput()
    {
        var sb = new StringBuilder();
        sb.AppendLine("""<%@ Page Title="Script Test" Language="C#" %>""");
        sb.AppendLine("<html><head>");

        sb.AppendLine("""
            <script type="text/javascript">
                function validate() {
                    var x = 5;
                    if (x < 10 && x > 0) {
                        return true;
                    }
                    var arr = [1, 2, 3];
                    for (var i = 0; i < arr.length; i++) {
                        console.log(arr[i]);
                    }
                }
            </script>
            """);

        sb.AppendLine("</head><body>");

        for (var i = 1; i <= 10; i++)
        {
            sb.AppendLine($"""<asp:Button ID="btn{i}" Text="Button {i}" OnClientClick="return validate();" />""");
        }

        sb.AppendLine("""
            <script>
                var a = 1;
                var b = 2;
                if (a < b && b > 0) {
                    document.getElementById('result').innerHTML = a + ' < ' + b;
                }
            </script>
            """);

        sb.AppendLine("</body></html>");
        return sb.ToString();
    }

    // ── Stress benchmark: 500KB+ ──────────────────────────────────────

    private const int StressIterations = 50;
    private const int StressWarmupIterations = 5;

    [Fact]
    public void Benchmark_StressTest_500KB()
    {
        var input = Generate500KBStressInput();
        var lineCount = CountLines(input);
        var sizeKB = input.Length / 1024.0;

        _output.WriteLine($"Input size: {input.Length} chars ({sizeKB:N1} KB), {lineCount} lines");
        sizeKB.ShouldBeGreaterThan(500, "Stress input must exceed 500 KB");

        // Verify parsing completes without error
        var sw = Stopwatch.StartNew();
        var result = AspxParser.Parse(input);
        sw.Stop();

        var controlCount = CountAspControlNodes(result.Nodes);
        var exprCount = result.Nodes.OfType<ExpressionNode>().Count();

        _output.WriteLine($"Parse time: {sw.ElapsedMilliseconds} ms");
        _output.WriteLine($"ASP controls: {controlCount}");
        _output.WriteLine($"Expressions: {exprCount}");
        _output.WriteLine($"Directives: {result.Directives.Count}");
        _output.WriteLine($"Top-level nodes: {result.Nodes.Count}");
        _output.WriteLine($"Throughput: {sizeKB / sw.Elapsed.TotalSeconds:N0} KB/sec");

        // Sanity: parser found the controls we generated
        controlCount.ShouldBeGreaterThan(500, "Should find 500+ ASP controls");
        result.Directives.ShouldNotBeEmpty();

        // Run reduced-iteration benchmark (500KB × 1000 iters is too slow for CI)
        RunStressBenchmark("Stress 500KB+", input);
    }

    private void RunStressBenchmark(string label, string input)
    {
        var stats = MeasureStressBenchmark(input);

        _output.WriteLine("");
        _output.WriteLine($"=== AngleSharp Parser Benchmark: {label} ===");
        _output.WriteLine($"  Avg parse time:  {stats.AvgMs:F3} ms");
        _output.WriteLine($"  Min parse time:  {stats.MinMs:F3} ms");
        _output.WriteLine($"  Max parse time:  {stats.MaxMs:F3} ms");
        _output.WriteLine($"  Std deviation:   {stats.StdDevMs:F3} ms");
        _output.WriteLine($"  Throughput:      {stats.ParsesPerSec:N0} parses/sec");
        _output.WriteLine($"  GC alloc (avg):  {stats.AllocKB:N1} KB/parse");
        _output.WriteLine($"  Iterations:      {StressIterations} (+ {StressWarmupIterations} warmup)");
    }

    private static BenchmarkStats MeasureStressBenchmark(string input)
    {
        for (var i = 0; i < StressWarmupIterations; i++)
        {
            AspxParser.Parse(input);
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var timings = new double[StressIterations];
        var sw = new Stopwatch();

        var memBefore = GC.GetTotalAllocatedBytes(precise: true);

        for (var i = 0; i < StressIterations; i++)
        {
            sw.Restart();
            AspxParser.Parse(input);
            sw.Stop();
            timings[i] = sw.Elapsed.TotalMilliseconds;
        }

        var memAfter = GC.GetTotalAllocatedBytes(precise: true);
        var totalAllocBytes = memAfter - memBefore;
        var allocKBPerParse = totalAllocBytes / 1024.0 / StressIterations;

        var avg = timings.Average();
        var min = timings.Min();
        var max = timings.Max();
        var variance = timings.Select(t => (t - avg) * (t - avg)).Average();
        var stdDev = Math.Sqrt(variance);
        var parsesPerSec = avg > 0 ? 1000.0 / avg : double.PositiveInfinity;

        return new BenchmarkStats
        {
            AvgMs = avg,
            MinMs = min,
            MaxMs = max,
            StdDevMs = stdDev,
            ParsesPerSec = parsesPerSec,
            AllocKB = allocKBPerParse
        };
    }

    /// <summary>
    /// Generates a realistic ~500KB+ .aspx file with 500+ controls,
    /// deep nesting (5-10 levels), mixed expressions, directives,
    /// server comments, and realistic attribute density.
    /// </summary>
    private static string Generate500KBStressInput()
    {
        var sb = new StringBuilder();

        // ── Directives (realistic page header)
        sb.AppendLine("""<%@ Page Title="Enterprise Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="App.Dashboard" %>""");
        sb.AppendLine("""<%@ Import Namespace="System.Data" %>""");
        sb.AppendLine("""<%@ Import Namespace="System.Linq" %>""");
        sb.AppendLine("""<%@ Import Namespace="System.Collections.Generic" %>""");
        sb.AppendLine("""<%@ Import Namespace="System.Web.UI.WebControls" %>""");
        sb.AppendLine("""<%@ Register TagPrefix="uc" TagName="Header" Src="~/Controls/Header.ascx" %>""");
        sb.AppendLine("""<%@ Register TagPrefix="uc" TagName="Footer" Src="~/Controls/Footer.ascx" %>""");
        sb.AppendLine("""<%@ Register TagPrefix="uc" TagName="Sidebar" Src="~/Controls/Sidebar.ascx" %>""");
        sb.AppendLine("<html><head><title>Enterprise Dashboard</title></head><body>");
        sb.AppendLine("<form id=\"form1\" runat=\"server\">");

        // ── Server comments
        sb.AppendLine("<%-- Enterprise Dashboard: generated stress test for AngleSharp benchmarking --%>");

        // ── 50 major sections x ~12 controls each = 600+ controls
        for (var section = 1; section <= 50; section++)
        {
            sb.AppendLine($"<%-- Section {section}: Module {section} configuration panel --%>");
            sb.AppendLine($"<div class=\"container-fluid\" id=\"section-{section}\" data-module=\"module-{section}\" data-version=\"2.1.{section}\">");

            // Level 1: outer panel
            sb.AppendLine($"  <asp:Panel ID=\"pnlOuter{section}\" CssClass=\"panel panel-default shadow-sm mb-4\" Visible=\"true\" EnableViewState=\"true\" GroupingText=\"Module {section}\">");

            // Level 2: header panel
            sb.AppendLine($"    <asp:Panel ID=\"pnlHeader{section}\" CssClass=\"panel-heading bg-primary text-white d-flex justify-content-between align-items-center\">");
            sb.AppendLine($"      <asp:Label ID=\"lblTitle{section}\" Text=\"Section {section} - Dashboard Module Configuration\" Font-Bold=\"true\" Font-Size=\"16pt\" CssClass=\"panel-title mb-0\" />");
            sb.AppendLine($"      <asp:HyperLink ID=\"lnkHelp{section}\" NavigateUrl=\"~/Help.aspx?section={section}&amp;mode=detail&amp;tab=overview\" Text=\"Help &amp; Documentation\" CssClass=\"btn btn-sm btn-outline-light float-right\" ToolTip=\"Open help for section {section}\" />");
            sb.AppendLine($"    </asp:Panel>");

            // Level 2: body panel
            sb.AppendLine($"    <asp:Panel ID=\"pnlBody{section}\" CssClass=\"panel-body p-4\">");

            // Level 3: form rows (5 rows per section)
            for (var row = 1; row <= 5; row++)
            {
                sb.AppendLine($"      <div class=\"form-group row mb-3\" data-row=\"{row}\" data-section=\"{section}\">");

                // Level 4: inner panel
                sb.AppendLine($"        <asp:Panel ID=\"pnlRow{section}_{row}\" CssClass=\"col-md-12 border rounded p-3 bg-light\">");

                // Level 5: label + input + validators
                sb.AppendLine($"          <div class=\"input-group mb-2\">");
                sb.AppendLine($"            <asp:Label ID=\"lbl{section}_{row}\" Text=\"Field {section}.{row} — Configuration Value:\" AssociatedControlID=\"txt{section}_{row}\" CssClass=\"form-label fw-bold text-secondary\" />");
                sb.AppendLine($"            <asp:TextBox ID=\"txt{section}_{row}\" CssClass=\"form-control border-primary\" MaxLength=\"200\" Placeholder=\"Enter configuration value for module {section}, field {row}\" ToolTip=\"Input field {section}.{row} — accepts alphanumeric characters\" TextMode=\"SingleLine\" />");
                sb.AppendLine($"            <asp:RequiredFieldValidator ID=\"rfv{section}_{row}\" ControlToValidate=\"txt{section}_{row}\" ErrorMessage=\"Configuration field {section}.{row} is required — please provide a value\" Display=\"Dynamic\" CssClass=\"text-danger small mt-1\" ValidationGroup=\"grp{section}\" SetFocusOnError=\"true\" />");

                if (row % 2 == 0)
                {
                    sb.AppendLine($"            <asp:RegularExpressionValidator ID=\"rev{section}_{row}\" ControlToValidate=\"txt{section}_{row}\" ValidationExpression=\"^[a-zA-Z0-9_\\-\\.]+$\" ErrorMessage=\"Only alphanumeric characters, underscores, hyphens, and periods are allowed\" Display=\"Dynamic\" CssClass=\"text-danger small\" ValidationGroup=\"grp{section}\" />");
                }

                if (row % 3 == 0)
                {
                    sb.AppendLine($"            <asp:CompareValidator ID=\"cv{section}_{row}\" ControlToValidate=\"txt{section}_{row}\" Operator=\"NotEqual\" ValueToCompare=\"\" ErrorMessage=\"Value cannot be empty\" Display=\"Dynamic\" CssClass=\"text-danger small\" />");
                }

                sb.AppendLine($"          </div>");

                // Level 5: dropdown + checkbox + radio
                sb.AppendLine($"          <div class=\"d-flex flex-wrap gap-3 align-items-center mt-2\">");
                sb.AppendLine($"            <asp:DropDownList ID=\"ddl{section}_{row}\" CssClass=\"form-select\" AutoPostBack=\"true\" EnableViewState=\"true\" ToolTip=\"Select category for field {section}.{row}\">");
                sb.AppendLine($"            </asp:DropDownList>");
                sb.AppendLine($"            <asp:CheckBox ID=\"chk{section}_{row}\" Text=\"Enable advanced option {section}.{row}\" CssClass=\"form-check\" AutoPostBack=\"false\" />");
                sb.AppendLine($"            <asp:RadioButton ID=\"rb{section}_{row}_a\" Text=\"Mode A\" GroupName=\"grp{section}_{row}\" CssClass=\"form-check-inline\" />");
                sb.AppendLine($"            <asp:RadioButton ID=\"rb{section}_{row}_b\" Text=\"Mode B\" GroupName=\"grp{section}_{row}\" CssClass=\"form-check-inline\" />");
                sb.AppendLine($"          </div>");

                sb.AppendLine($"        </asp:Panel>");
                sb.AppendLine($"      </div>");
            }

            // Expressions within the section
            sb.AppendLine($"      <div class=\"section-metadata text-muted small\">");
            sb.AppendLine($"        <p>Last updated: <%= DateTime.Now.AddHours(-{section}).ToString(\"yyyy-MM-dd HH:mm:ss\") %></p>");
            sb.AppendLine($"        <p>Status: <%# Eval(\"Section{section}Status\") %></p>");
            sb.AppendLine($"        <p>Count: <%# Eval(\"Section{section}ItemCount\", \"{{0:N0}}\") %></p>");
            sb.AppendLine($"      </div>");

            sb.AppendLine($"    </asp:Panel>");

            // Level 2: footer panel with buttons
            sb.AppendLine($"    <asp:Panel ID=\"pnlFooter{section}\" CssClass=\"panel-footer bg-light border-top p-3 d-flex gap-2\">");
            sb.AppendLine($"      <asp:Button ID=\"btnSave{section}\" Text=\"Save Module {section} Configuration\" CssClass=\"btn btn-primary\" ValidationGroup=\"grp{section}\" ToolTip=\"Save all changes for section {section}\" />");
            sb.AppendLine($"      <asp:Button ID=\"btnCancel{section}\" Text=\"Cancel Changes\" CssClass=\"btn btn-secondary\" CausesValidation=\"false\" ToolTip=\"Discard all unsaved changes\" />");
            sb.AppendLine($"      <asp:LinkButton ID=\"lnkDelete{section}\" Text=\"Delete Module\" CssClass=\"btn btn-outline-danger\" OnClientClick=\"return confirm('Are you sure you want to permanently delete module {section}?');\" />");
            sb.AppendLine($"      <asp:HiddenField ID=\"hf{section}\" Value=\"section-{section}-state-active\" />");
            sb.AppendLine($"      <asp:HiddenField ID=\"hfTimestamp{section}\" Value=\"\" />");
            sb.AppendLine($"    </asp:Panel>");

            sb.AppendLine($"  </asp:Panel>");
            sb.AppendLine($"</div>");
        }

        // ── GridView section (deeply nested)
        sb.AppendLine("<div class=\"table-responsive mt-4\">");
        sb.AppendLine("  <asp:GridView ID=\"gvMaster\" CssClass=\"table table-striped table-hover table-bordered\" AutoGenerateColumns=\"false\" AllowPaging=\"true\" PageSize=\"25\" AllowSorting=\"true\">");
        for (var col = 1; col <= 15; col++)
        {
            sb.AppendLine($"    <asp:BoundField DataField=\"Column{col}\" HeaderText=\"Column {col} Header\" SortExpression=\"Column{col}\" ItemStyle-CssClass=\"text-nowrap\" />");
        }
        sb.AppendLine("  </asp:GridView>");
        sb.AppendLine("</div>");

        // ── More databind expressions
        sb.AppendLine("<div class=\"expression-block\">");
        for (var e = 1; e <= 20; e++)
        {
            sb.AppendLine($"  <p class=\"mb-1\">Property {e}: <%# Eval(\"Property{e}\") %></p>");
            sb.AppendLine($"  <span class=\"badge bg-info\"><%= Model.GetValue({e}).ToString(\"N2\") %></span>");
        }
        sb.AppendLine("</div>");

        // ── Server comments
        for (var c = 1; c <= 10; c++)
        {
            sb.AppendLine($"<%-- TODO: Review section {c} before release (ticket #{1000 + c}) — assigned to team lead for final approval --%>");
        }

        sb.AppendLine("</form></body></html>");

        return sb.ToString();
    }

    // ── Helpers ─────────────────────────────────────────────────────────

    private static int CountLines(string text) =>
        text.Split('\n').Length;

    private static int CountAllNodes(AspxParseResult result) =>
        result.Nodes.Count + result.Directives.Count;

    private static int CountAspControlNodes(IEnumerable<AspxNode> nodes)
    {
        var count = 0;
        foreach (var node in nodes)
        {
            if (node is AspControlNode control)
            {
                count++;
                count += CountAspControlNodes(control.Children);
            }
        }
        return count;
    }
}
