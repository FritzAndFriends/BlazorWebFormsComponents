namespace BlazorWebFormsComponents.Cli.Services;

/// <summary>
/// Uses AI (OpenAI/GitHub Copilot) to enhance the conversion from Web Forms to Blazor
/// </summary>
public class AiAssistant
{
    private readonly bool _isAvailable;

    public AiAssistant()
    {
        // Check for environment variables but mark as not available for now
        // TODO: Implement AI enhancement using Microsoft.Extensions.AI and OpenAI SDK
        var openAiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

        _isAvailable = !string.IsNullOrEmpty(openAiKey) || !string.IsNullOrEmpty(githubToken);
        
        if (!_isAvailable)
        {
            Console.WriteLine("  Note: Set OPENAI_API_KEY or GITHUB_TOKEN environment variable to enable AI-assisted conversion.");
        }
        else
        {
            Console.WriteLine("  Note: AI enhancement is configured but not yet implemented in this version.");
        }
    }

    public Task<string> EnhanceConversionAsync(string convertedMarkup, string componentName)
    {
        // For now, just return the markup as-is
        // In future versions, this will use Microsoft.Extensions.AI with OpenAI or GitHub Models
        // to intelligently improve the conversion
        return Task.FromResult(convertedMarkup);
    }
}
