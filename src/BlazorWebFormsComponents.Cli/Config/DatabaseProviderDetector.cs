using System.Xml.Linq;

namespace BlazorWebFormsComponents.Cli.Config;

/// <summary>
/// Detects EF database provider from Web.config connectionStrings.
/// Ported from Find-DatabaseProvider in bwfc-migrate.ps1.
/// </summary>
public class DatabaseProviderDetector
{
    private static readonly Dictionary<string, (string PackageName, string ProviderMethod)> ProviderMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["System.Data.SqlClient"] = ("Microsoft.EntityFrameworkCore.SqlServer", "UseSqlServer"),
        ["System.Data.SQLite"] = ("Microsoft.EntityFrameworkCore.Sqlite", "UseSqlite"),
        ["Npgsql"] = ("Npgsql.EntityFrameworkCore.PostgreSQL", "UseNpgsql"),
        ["MySql.Data.MySqlClient"] = ("Pomelo.EntityFrameworkCore.MySql", "UseMySql")
    };

    public DatabaseProviderInfo Detect(string sourcePath)
    {
        var defaultResult = new DatabaseProviderInfo
        {
            PackageName = "Microsoft.EntityFrameworkCore.SqlServer",
            ProviderMethod = "UseSqlServer",
            DetectedFrom = "Default — no Web.config connectionStrings found",
            ConnectionString = ""
        };

        if (string.IsNullOrEmpty(sourcePath))
            return defaultResult;

        // Look for Web.config in source path and parent directory
        var candidates = new[]
        {
            Path.Combine(sourcePath, "Web.config"),
            Path.Combine(Path.GetDirectoryName(sourcePath) ?? sourcePath, "Web.config")
        };

        string? webConfigPath = null;
        foreach (var candidate in candidates)
        {
            if (File.Exists(candidate))
            {
                webConfigPath = candidate;
                break;
            }
        }

        if (webConfigPath == null)
            return defaultResult;

        XDocument doc;
        try
        {
            doc = XDocument.Load(webConfigPath);
        }
        catch
        {
            return defaultResult;
        }

        var connStringsElement = doc.Root?.Element("connectionStrings");
        if (connStringsElement == null)
            return defaultResult;

        var adds = connStringsElement.Elements("add").ToList();
        if (adds.Count == 0)
            return defaultResult;

        // Pass 1: Non-EntityClient entries with explicit providerName
        foreach (var entry in adds)
        {
            var providerName = entry.Attribute("providerName")?.Value;
            if (string.IsNullOrEmpty(providerName) || providerName == "System.Data.EntityClient")
                continue;

            if (ProviderMap.TryGetValue(providerName, out var mapped))
            {
                return new DatabaseProviderInfo
                {
                    PackageName = mapped.PackageName,
                    ProviderMethod = mapped.ProviderMethod,
                    DetectedFrom = $"Web.config providerName={providerName}",
                    ConnectionString = entry.Attribute("connectionString")?.Value ?? ""
                };
            }
        }

        // Pass 2: Entries without providerName — detect from connection string content
        foreach (var entry in adds)
        {
            var connString = entry.Attribute("connectionString")?.Value;
            if (string.IsNullOrEmpty(connString) || connString.StartsWith("metadata=", StringComparison.OrdinalIgnoreCase))
                continue;
            if (entry.Attribute("providerName") != null)
                continue;

            if (System.Text.RegularExpressions.Regex.IsMatch(connString, @"(?i)\(LocalDB\)|Server="))
            {
                return new DatabaseProviderInfo
                {
                    PackageName = "Microsoft.EntityFrameworkCore.SqlServer",
                    ProviderMethod = "UseSqlServer",
                    DetectedFrom = "Web.config connection string pattern (SQL Server)",
                    ConnectionString = connString
                };
            }

            if (System.Text.RegularExpressions.Regex.IsMatch(connString, @"(?i)Data Source=.*\.db"))
            {
                return new DatabaseProviderInfo
                {
                    PackageName = "Microsoft.EntityFrameworkCore.Sqlite",
                    ProviderMethod = "UseSqlite",
                    DetectedFrom = "Web.config connection string pattern (SQLite)",
                    ConnectionString = connString
                };
            }
        }

        // Pass 3: EntityClient entries — extract inner provider (EF6 pattern)
        foreach (var entry in adds)
        {
            if (entry.Attribute("providerName")?.Value != "System.Data.EntityClient")
                continue;

            var connString = entry.Attribute("connectionString")?.Value ?? "";
            var match = System.Text.RegularExpressions.Regex.Match(connString, @"provider=([^;""]+)");
            if (match.Success)
            {
                var innerProvider = match.Groups[1].Value.Trim();
                if (ProviderMap.TryGetValue(innerProvider, out var mapped))
                {
                    return new DatabaseProviderInfo
                    {
                        PackageName = mapped.PackageName,
                        ProviderMethod = mapped.ProviderMethod,
                        DetectedFrom = $"Web.config EntityClient provider={innerProvider}",
                        ConnectionString = ""
                    };
                }
            }
        }

        return defaultResult;
    }
}

public class DatabaseProviderInfo
{
    public required string PackageName { get; init; }
    public required string ProviderMethod { get; init; }
    public required string DetectedFrom { get; init; }
    public required string ConnectionString { get; init; }
}
