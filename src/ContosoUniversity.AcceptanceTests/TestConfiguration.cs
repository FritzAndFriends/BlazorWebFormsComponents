namespace ContosoUniversity.AcceptanceTests;

/// <summary>
/// Provides the base URL for all acceptance tests.
/// Set the CONTOSO_BASE_URL environment variable to target a specific deployment.
/// Defaults to http://localhost:44380 if not set.
/// </summary>
public static class TestConfiguration
{
    public static string BaseUrl =>
        Environment.GetEnvironmentVariable("CONTOSO_BASE_URL")
        ?? "http://localhost:44380";
}
