namespace BlazorWebFormsComponents.Cli.Tests;

public sealed class RuntimeDetectorTests : IDisposable
{
    private readonly string _tempDir = Path.Combine(AppContext.BaseDirectory, "TestArtifacts", $"runtime-detector-{Guid.NewGuid():N}");

    public RuntimeDetectorTests()
    {
        Directory.CreateDirectory(_tempDir);
    }

    [Fact]
    public void Detect_IncludesCustomControlRegistrations()
    {
        File.WriteAllText(Path.Combine(_tempDir, "Web.config"), """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <system.web>
                <pages>
                  <controls>
                    <add assembly="Microsoft.AspNet.Web.Optimization.WebForms" namespace="Microsoft.AspNet.Web.Optimization.WebForms" tagPrefix="webopt" />
                  </controls>
                </pages>
              </system.web>
            </configuration>
            """);

        File.WriteAllText(Path.Combine(_tempDir, "Students.aspx"), """
            <%@ Page Language="C#" %>
            <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
            """);
        File.WriteAllText(Path.Combine(_tempDir, "FancyPager.cs"), """
            using System.Web.UI.WebControls;

            namespace AjaxControlToolkit;

            public class FancyPager : WebControl
            {
            }
            """);

        var detector = TestHelpers.CreateDefaultRuntimeDetector();

        var profile = detector.Detect(_tempDir);

        Assert.Contains(profile.CustomControlRegistrations.NamespaceTags, registration =>
            registration.TagPrefix == "webopt"
            && registration.Namespace == "Microsoft.AspNet.Web.Optimization.WebForms");
        Assert.Contains(profile.CustomControlRegistrations.RegisterDirectives, registration =>
            registration.TagPrefix == "ajaxToolkit"
            && registration.AssemblyName == "AjaxControlToolkit");
        Assert.Equal("Microsoft.AspNet.Web.Optimization.WebForms", profile.CustomControlPrefixToNamespaceMap["webopt"]);
        Assert.Equal("AjaxControlToolkit", profile.CustomControlPrefixToNamespaceMap["ajaxToolkit"]);
        Assert.Contains(profile.CodeOnlyServerControls, descriptor =>
            descriptor.ClassName == "FancyPager"
            && descriptor.Namespace == "AjaxControlToolkit"
            && descriptor.TagPrefixes.Contains("ajaxToolkit", StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public void Detect_IncludesNamespacePrefixPlumbing_FromRootAndNestedWebConfig()
    {
        File.WriteAllText(Path.Combine(_tempDir, "Web.config"), """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <system.web>
                <pages>
                  <controls>
                    <add assembly="Contoso.Root" namespace="Contoso.Root.Controls" tagPrefix="root" />
                  </controls>
                </pages>
              </system.web>
            </configuration>
            """);

        var adminDirectory = Path.Combine(_tempDir, "Admin");
        Directory.CreateDirectory(adminDirectory);
        File.WriteAllText(Path.Combine(adminDirectory, "Web.config"), """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <system.web>
                <pages>
                  <controls>
                    <add assembly="Contoso.Admin" namespace="Contoso.Admin.Controls" tagPrefix="admin" />
                  </controls>
                </pages>
              </system.web>
            </configuration>
            """);

        var detector = TestHelpers.CreateDefaultRuntimeDetector();

        var profile = detector.Detect(_tempDir);

        Assert.Equal("Contoso.Root.Controls", profile.CustomControlPrefixToNamespaceMap["root"]);
        Assert.Equal("Contoso.Admin.Controls", profile.CustomControlPrefixToNamespaceMap["admin"]);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            try
            {
                Directory.Delete(_tempDir, recursive: true);
            }
            catch
            {
            }
        }
    }
}
