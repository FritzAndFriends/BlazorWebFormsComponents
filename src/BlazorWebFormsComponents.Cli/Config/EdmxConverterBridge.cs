using BlazorWebFormsComponents.Cli.Pipeline;
using NativeEdmxToEfCoreConverter = BlazorWebFormsComponents.Cli.Services.EdmxToEfCoreConverter;

namespace BlazorWebFormsComponents.Cli.Config;

[Obsolete("Use BlazorWebFormsComponents.Cli.Services.EdmxToEfCoreConverter instead.")]
public class EdmxConverterBridge
{
    private readonly NativeEdmxToEfCoreConverter _inner;

    public EdmxConverterBridge(NativeEdmxToEfCoreConverter inner)
    {
        _inner = inner;
    }

    public Task<HashSet<string>> ConvertAsync(string sourcePath, string outputPath, string projectName, bool dryRun, MigrationReport report)
    {
        return _inner.ConvertAsync(sourcePath, outputPath, projectName, dryRun, report);
    }
}
