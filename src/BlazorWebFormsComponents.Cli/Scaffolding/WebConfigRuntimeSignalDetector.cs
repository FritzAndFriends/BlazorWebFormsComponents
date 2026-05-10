using System.Xml.Linq;

namespace BlazorWebFormsComponents.Cli.Scaffolding;

/// <summary>
/// Detects runtime configuration from Web.config sections that other detectors don't cover:
/// authentication mode, forms loginUrl, customErrors, and sessionState.
/// </summary>
public class WebConfigRuntimeSignalDetector : IRuntimeSignalDetector
{
    public void Apply(string sourcePath, RuntimeProfile profile)
    {
        var webConfigPath = FindWebConfig(sourcePath);
        if (webConfigPath == null)
            return;

        XDocument doc;
        try
        {
            doc = XDocument.Load(webConfigPath);
        }
        catch
        {
            return;
        }

        var systemWeb = doc.Root?.Element("system.web");
        if (systemWeb == null)
            return;

        DetectAuthentication(systemWeb, profile);
        DetectCustomErrors(systemWeb, profile);
        DetectSessionState(systemWeb, profile);
    }

    private static void DetectAuthentication(XElement systemWeb, RuntimeProfile profile)
    {
        var auth = systemWeb.Element("authentication");
        if (auth == null)
            return;

        var mode = auth.Attribute("mode")?.Value;
        if (!string.IsNullOrEmpty(mode))
        {
            profile.AuthenticationMode = mode;

            if (string.Equals(mode, "Forms", StringComparison.OrdinalIgnoreCase))
            {
                profile.NeedsIdentity = true;

                var forms = auth.Element("forms");
                if (forms != null)
                {
                    var loginUrl = forms.Attribute("loginUrl")?.Value;
                    if (!string.IsNullOrEmpty(loginUrl))
                    {
                        // Normalize ~/path to /path
                        profile.AuthLoginPath = loginUrl.TrimStart('~');
                    }
                }
            }
        }
    }

    private static void DetectCustomErrors(XElement systemWeb, RuntimeProfile profile)
    {
        var customErrors = systemWeb.Element("customErrors");
        if (customErrors == null)
            return;

        var defaultRedirect = customErrors.Attribute("defaultRedirect")?.Value;
        if (!string.IsNullOrEmpty(defaultRedirect))
        {
            profile.CustomErrorRedirect = defaultRedirect.TrimStart('~');
        }
    }

    private static void DetectSessionState(XElement systemWeb, RuntimeProfile profile)
    {
        var sessionState = systemWeb.Element("sessionState");
        if (sessionState == null)
            return;

        // If sessionState is explicitly configured (not Off), the app needs session
        var mode = sessionState.Attribute("mode")?.Value;
        if (!string.IsNullOrEmpty(mode) &&
            !string.Equals(mode, "Off", StringComparison.OrdinalIgnoreCase))
        {
            profile.NeedsSession = true;
        }
    }

    private static string? FindWebConfig(string sourcePath)
    {
        foreach (var file in RuntimeDetectionFiles.EnumerateFiles(sourcePath, ".config"))
        {
            if (Path.GetFileName(file).Equals("Web.config", StringComparison.OrdinalIgnoreCase))
                return file;
        }
        return null;
    }
}
