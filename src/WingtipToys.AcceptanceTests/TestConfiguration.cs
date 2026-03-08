namespace WingtipToys.AcceptanceTests;

/// <summary>
/// Provides the base URL for all acceptance tests.
/// Set the WINGTIPTOYS_BASE_URL environment variable to target a specific deployment.
/// Defaults to https://localhost:5001 if not set.
/// </summary>
public static class TestConfiguration
{
    public static string BaseUrl =>
        Environment.GetEnvironmentVariable("WINGTIPTOYS_BASE_URL")
        ?? "https://localhost:5001";
}
