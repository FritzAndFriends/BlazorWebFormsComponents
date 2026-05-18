namespace BlazorWebFormsComponents.Cli.Scaffolding;

/// <summary>
/// Detects usage of System.Data.SqlClient in source files and sets NeedsSqlClient flag
/// so the scaffolder adds the System.Data.SqlClient NuGet package.
/// </summary>
public class SqlClientRuntimeSignalDetector : IRuntimeSignalDetector
{
    public void Apply(string sourcePath, RuntimeProfile profile)
    {
        foreach (var file in RuntimeDetectionFiles.EnumerateFiles(sourcePath, ".cs", ".aspx", ".ascx"))
        {
            var content = File.ReadAllText(file);
            if (content.Contains("System.Data.SqlClient") || content.Contains("using System.Data.SqlClient"))
            {
                profile.NeedsSqlClient = true;
                return;
            }
        }
    }
}
