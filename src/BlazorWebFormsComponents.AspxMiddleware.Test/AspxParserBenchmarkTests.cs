using System.Diagnostics;
using System.Text;
using BlazorWebFormsComponents.AspxMiddleware;
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

    // ── Helpers ─────────────────────────────────────────────────────────

    private static int CountLines(string text) =>
        text.Split('\n').Length;

    private static int CountAllNodes(AspxParseResult result) =>
        result.Nodes.Count + result.Directives.Count;
}
