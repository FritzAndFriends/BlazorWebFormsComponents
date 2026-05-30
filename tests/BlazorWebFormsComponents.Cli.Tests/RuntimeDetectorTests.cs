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

        var detector = TestHelpers.CreateDefaultRuntimeDetector();

        var profile = detector.Detect(_tempDir);

        Assert.Contains(profile.CustomControlRegistrations.NamespaceTags, registration =>
            registration.TagPrefix == "webopt"
            && registration.Namespace == "Microsoft.AspNet.Web.Optimization.WebForms");
        Assert.Contains(profile.CustomControlRegistrations.RegisterDirectives, registration =>
            registration.TagPrefix == "ajaxToolkit"
            && registration.AssemblyName == "AjaxControlToolkit");
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
