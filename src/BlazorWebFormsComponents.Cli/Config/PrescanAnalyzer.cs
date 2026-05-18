using System.Text.Json;
using System.Text.RegularExpressions;

namespace BlazorWebFormsComponents.Cli.Config;

/// <summary>
/// Scans source files for common Web Forms migration patterns and emits a JSON summary.
/// Ported from the PowerShell -Prescan workflow in bwfc-migrate.ps1.
/// </summary>
public class PrescanAnalyzer
{
    private static readonly IReadOnlyList<PrescanRule> s_rules =
    [
        new("BWFC001", "Missing [Parameter]", @"public\s+\w+\s+\w+\s*\{\s*get\s*;\s*set\s*;\s*\}", "Public properties that may need [Parameter] attribute"),
        new("BWFC002", "ViewState Usage", @"ViewState\s*\[", "ViewState dictionary access"),
        new("BWFC003", "IsPostBack", @"(Page\.)?IsPostBack", "IsPostBack checks"),
        new("BWFC004", "Response.Redirect", @"Response\.Redirect\s*\(", "Response.Redirect calls"),
        new("BWFC005", "Session Usage", @"Session\s*\[|HttpContext\.Current\.Session", "Session state access"),
        new("BWFC011", "Event Handler Signatures", @"\(\s*object\s+\w+\s*,\s*EventArgs", "Web Forms event handler signatures"),
        new("BWFC012", "runat=\"server\"", @"runat\s*=\s*""server""", "runat=\"server\" artifacts in strings/comments"),
        new("BWFC013", "Response Object", @"Response\.(Write|WriteFile|Clear|Flush|End)\s*\(", "Response object method calls"),
        new("BWFC014", "Request Object", @"Request\.(Form|Cookies|Headers|Files|QueryString|ServerVariables)\s*[\[\.]", "Request object property access"),
        new("BWFC015", "Server Utility", @"Server\.(MapPath|HtmlEncode|HtmlDecode|UrlEncode|UrlDecode)\s*\(", "Server utility calls — use ServerShim on WebFormsPageBase"),
        new("BWFC016", "ConfigurationManager", @"ConfigurationManager\.(AppSettings|ConnectionStrings)\s*\[", "ConfigurationManager access — BWFC provides shim"),
        new("BWFC017", "ClientScript", @"(Page\.)?ClientScript\.(RegisterStartupScript|RegisterClientScriptBlock|RegisterClientScriptInclude|GetPostBackEventReference)\s*\(", "ClientScript calls — use ClientScriptShim"),
        new("BWFC018", "Cache Access", @"\bCache\s*\[", "Cache dictionary access — use CacheShim on WebFormsPageBase")
    ];

    public PrescanResult Analyze(string sourcePath)
    {
        var result = new PrescanResult
        {
            ScanDate = DateTimeOffset.UtcNow,
            SourcePath = sourcePath
        };

        if (!Directory.Exists(sourcePath))
            return result;

        var csFiles = Directory.EnumerateFiles(sourcePath, "*.cs", SearchOption.AllDirectories).ToList();
        result.TotalFiles = csFiles.Count;

        foreach (var file in csFiles)
        {
            string content;
            try
            {
                content = File.ReadAllText(file);
            }
            catch
            {
                continue;
            }

            var fileMatches = new List<PrescanFileMatch>();
            foreach (var rule in s_rules)
            {
                var matches = rule.Regex.Matches(content);
                if (matches.Count == 0)
                    continue;

                var lines = matches.Select(m => GetLineNumber(content, m.Index)).ToList();
                fileMatches.Add(new PrescanFileMatch(rule.Id, rule.Name, matches.Count, lines));

                if (!result.Summary.TryGetValue(rule.Id, out var summary))
                {
                    summary = new PrescanSummary(rule.Name, rule.Description, 0, 0);
                }

                result.Summary[rule.Id] = summary with
                {
                    TotalHits = summary.TotalHits + matches.Count,
                    FileCount = summary.FileCount + 1
                };

                result.TotalMatches += matches.Count;
            }

            if (fileMatches.Count == 0)
                continue;

            var relativePath = Path.GetRelativePath(sourcePath, file);
            result.Files.Add(new PrescanFileResult(relativePath, fileMatches));
        }

        result.FilesWithMatches = result.Files.Count;
        return result;
    }

    public static string ToJson(PrescanResult result)
    {
        return JsonSerializer.Serialize(result, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    private static int GetLineNumber(string content, int index)
    {
        var line = 1;
        for (var i = 0; i < index; i++)
        {
            if (content[i] == '\n')
                line++;
        }

        return line;
    }

    private sealed record PrescanRule(string Id, string Name, string Pattern, string Description)
    {
        public Regex Regex { get; } = new(Pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }
}

public class PrescanResult
{
    public DateTimeOffset ScanDate { get; set; }
    public string SourcePath { get; set; } = string.Empty;
    public Dictionary<string, PrescanSummary> Summary { get; } = [];
    public List<PrescanFileResult> Files { get; } = [];
    public int TotalFiles { get; set; }
    public int FilesWithMatches { get; set; }
    public int TotalMatches { get; set; }
}

public sealed record PrescanSummary(string Name, string Description, int TotalHits, int FileCount);
public sealed record PrescanFileResult(string Path, IReadOnlyList<PrescanFileMatch> Matches);
public sealed record PrescanFileMatch(string Rule, string Name, int Count, IReadOnlyList<int> Lines);
