using BlazorWebFormsComponents.Diagnostics;

if (args.Length < 2)
{
	Console.Error.WriteLine("Usage: GenerateHealthSnapshot <solutionRoot> <outputPath>");
	Console.Error.WriteLine("  solutionRoot: Path to the repository root");
	Console.Error.WriteLine("  outputPath:   Path to write health-snapshot.json");
	return 1;
}

var solutionRoot = args[0];
var outputPath = args[1];

Console.WriteLine($"Solution root: {solutionRoot}");
Console.WriteLine($"Output path:   {outputPath}");

var baselinesPath = Path.Combine(solutionRoot, "dev-docs", "reference-baselines.json");
var baselines = ReferenceBaselines.LoadFromFile(baselinesPath);
var service = new ComponentHealthService(baselines, solutionRoot);

HealthSnapshotGenerator.GenerateSnapshot(service, outputPath);

var reports = service.GetAllReports();
Console.WriteLine($"Generated snapshot with {reports.Count} component reports.");

return 0;
