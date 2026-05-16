using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using BlazorWebFormsComponents.Cli.Io;

namespace BlazorWebFormsComponents.Cli.Config;

/// <summary>
/// Parses Web.config and generates appsettings.json.
/// Ported from Convert-WebConfigToAppSettings in bwfc-migrate.ps1 (line ~892).
/// </summary>
public class WebConfigTransformer
{
    private static readonly HashSet<string> BuiltInConnectionNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "LocalSqlServer",
        "LocalMySqlServer"
    };

    /// <summary>
    /// Transforms Web.config appSettings and connectionStrings into appsettings.json content.
    /// Returns null if no Web.config found or no settings to extract.
    /// </summary>
    public WebConfigResult? Transform(string sourcePath)
    {
        // Find Web.config (case-insensitive search)
        var webConfigPath = FindWebConfig(sourcePath);
        if (webConfigPath == null)
            return null;

        XDocument doc;
        try
        {
            doc = XDocument.Load(webConfigPath);
        }
        catch (Exception ex)
        {
            return new WebConfigResult
            {
                JsonContent = null,
                AppSettingsCount = 0,
                ConnectionStringsCount = 0,
                Error = $"Could not parse Web.config: {ex.Message}"
            };
        }

        var appSettings = new Dictionary<string, string>();
        var connectionStrings = new Dictionary<string, string>();

        // Parse <appSettings>
        var appSettingsNodes = doc.Descendants("appSettings").Elements("add");
        foreach (var node in appSettingsNodes)
        {
            var key = node.Attribute("key")?.Value;
            var value = node.Attribute("value")?.Value ?? "";
            if (!string.IsNullOrEmpty(key))
            {
                appSettings[key] = value;
            }
        }

        // Parse <connectionStrings>
        var connStrNodes = doc.Descendants("connectionStrings").Elements("add");
        foreach (var node in connStrNodes)
        {
            var name = node.Attribute("name")?.Value;
            var connStr = node.Attribute("connectionString")?.Value;
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(connStr))
            {
                if (!BuiltInConnectionNames.Contains(name))
                {
                    connectionStrings[name] = NormalizeConnectionString(connStr);
                }
            }
        }

        if (appSettings.Count == 0 && connectionStrings.Count == 0)
            return null;

        // Build JSON structure
        var jsonObj = new Dictionary<string, object>();

        if (connectionStrings.Count > 0)
        {
            jsonObj["ConnectionStrings"] = connectionStrings;
        }

        foreach (var entry in appSettings)
        {
            jsonObj[entry.Key] = entry.Value;
        }

        // Add standard Blazor sections
        if (!jsonObj.ContainsKey("Logging"))
        {
            jsonObj["Logging"] = new Dictionary<string, object>
            {
                ["LogLevel"] = new Dictionary<string, string>
                {
                    ["Default"] = "Information",
                    ["Microsoft.AspNetCore"] = "Warning"
                }
            };
        }

        if (!jsonObj.ContainsKey("AllowedHosts"))
        {
            jsonObj["AllowedHosts"] = "*";
        }

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var jsonContent = JsonSerializer.Serialize(jsonObj, options);

        return new WebConfigResult
        {
            JsonContent = jsonContent,
            AppSettingsCount = appSettings.Count,
            ConnectionStringsCount = connectionStrings.Count,
            AppSettingsKeys = [.. appSettings.Keys],
            ConnectionStringNames = [.. connectionStrings.Keys]
        };
    }

    private static string NormalizeConnectionString(string connStr)
    {
        // EF6 metadata connection strings wrap the real SQL connection string inside
        // a provider connection string parameter. Unwrap it first.
        connStr = UnwrapEf6ConnectionString(connStr);

        if (!connStr.Contains("AttachDbFilename=|DataDirectory|", StringComparison.OrdinalIgnoreCase))
            return connStr;

        var segments = connStr.Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(segment => segment.Trim())
            .ToList();

        string? databaseName = null;
        var normalizedSegments = new List<string>();

        foreach (var segment in segments)
        {
            var separatorIndex = segment.IndexOf('=');
            if (separatorIndex < 0)
            {
                normalizedSegments.Add(segment);
                continue;
            }

            var key = segment[..separatorIndex].Trim();
            var value = segment[(separatorIndex + 1)..].Trim();

            if (key.Equals("AttachDbFilename", StringComparison.OrdinalIgnoreCase) &&
                value.StartsWith("|DataDirectory|", StringComparison.OrdinalIgnoreCase))
            {
                var fileName = Path.GetFileNameWithoutExtension(value.Replace("|DataDirectory|\\", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Replace("|DataDirectory|/", string.Empty, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrWhiteSpace(fileName))
                    databaseName ??= NormalizeDatabaseName(fileName);

                continue;
            }

            normalizedSegments.Add(segment);
        }

        if (string.IsNullOrWhiteSpace(databaseName))
            return connStr;

        if (!normalizedSegments.Any(segment => segment.StartsWith("Initial Catalog=", StringComparison.OrdinalIgnoreCase)))
        {
            var insertIndex = normalizedSegments.FindIndex(segment => segment.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase));
            var catalogSegment = $"Initial Catalog={databaseName}";
            if (insertIndex >= 0)
                normalizedSegments.Insert(insertIndex + 1, catalogSegment);
            else
                normalizedSegments.Add(catalogSegment);
        }

        return string.Join(';', normalizedSegments);
    }

    private static string NormalizeDatabaseName(string fileName)
    {
        return fileName.ToLowerInvariant() switch
        {
            "wingtiptoys" => "WingtipToys",
            "contosouniversity" => "ContosoUniversity",
            "departmentportal" => "DepartmentPortal",
            _ => string.Concat(fileName
                .Split(['-', '_', ' '], StringSplitOptions.RemoveEmptyEntries)
                .Select(segment => char.ToUpperInvariant(segment[0]) + segment[1..].ToLowerInvariant()))
        };
    }

    // Matches EF6 metadata connection strings:
    //   metadata=res://*/...;provider=System.Data.SqlClient;provider connection string="actual conn string"
    private static readonly Regex Ef6MetadataRegex = new(
        @"^\s*metadata\s*=\s*res://",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex Ef6ProviderConnectionStringRegex = new(
        @"provider\s+connection\s+string\s*=\s*""(?<inner>[^""]*)""",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Unwraps EF6 metadata connection strings to extract the inner SQL connection string.
    /// EF6 format: metadata=res://*/...;provider=System.Data.SqlClient;provider connection string="Data Source=...;Initial Catalog=..."
    /// Returns the inner provider connection string, or the original if not EF6 format.
    /// </summary>
    private static string UnwrapEf6ConnectionString(string connStr)
    {
        if (!Ef6MetadataRegex.IsMatch(connStr))
            return connStr;

        var match = Ef6ProviderConnectionStringRegex.Match(connStr);
        if (match.Success)
        {
            var inner = match.Groups["inner"].Value;
            // The inner string may use &quot; for quotes (from XML) — unescape
            return inner.Replace("&quot;", "\"");
        }

        return connStr;
    }

    private static string? FindWebConfig(string sourcePath)
    {
        var path1 = Path.Combine(sourcePath, "Web.config");
        if (File.Exists(path1)) return path1;

        var path2 = Path.Combine(sourcePath, "web.config");
        if (File.Exists(path2)) return path2;

        return null;
    }
}

public class WebConfigResult
{
    public string? JsonContent { get; init; }
    public int AppSettingsCount { get; init; }
    public int ConnectionStringsCount { get; init; }
    public List<string> AppSettingsKeys { get; init; } = [];
    public List<string> ConnectionStringNames { get; init; } = [];
    public string? Error { get; init; }
}
