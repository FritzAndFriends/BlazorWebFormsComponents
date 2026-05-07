namespace BlazorWebFormsComponents.Cli.Scaffolding;

public interface IRuntimeSignalDetector
{
    void Apply(string sourcePath, RuntimeProfile profile);
}
