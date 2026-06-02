using BlazorWebFormsComponents.Cli.Analysis;

namespace BlazorWebFormsComponents.Cli.Tests.Analysis;

public sealed class WebConfigAssemblyParserTests : IDisposable
{
    private readonly string _testRoot = Path.Combine(AppContext.BaseDirectory, "TestArtifacts", $"webconfig-parser-{Guid.NewGuid():N}");

    public WebConfigAssemblyParserTests()
    {
        Directory.CreateDirectory(_testRoot);
    }

    [Fact]
    public void Parse_ExtractsAssembliesAndNamespaceTags()
    {
        var webConfigPath = Path.Combine(_testRoot, "Web.config");
        File.WriteAllText(webConfigPath, """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <system.web>
                <compilation>
                  <assemblies>
                    <add assembly="WingtipToys, Version=1.0.0.0, Culture=neutral" />
                    <add assembly="Contoso.Controls, Version=2.0.0.0">
                      <add assembly="Nested.Controls, Version=3.0.0.0" namespace="Contoso.Controls.Nested" />
                    </add>
                  </assemblies>
                </compilation>
                <pages>
                  <controls>
                    <add assembly="Microsoft.AspNet.Web.Optimization.WebForms" namespace="Microsoft.AspNet.Web.Optimization.WebForms" tagPrefix="webopt" />
                  </controls>
                </pages>
              </system.web>
            </configuration>
            """);

        var parser = new WebConfigAssemblyParser();

        var result = parser.Parse(webConfigPath);

        Assert.Null(result.Error);
        Assert.Contains(result.Assemblies, registration => registration.AssemblyName == "WingtipToys" && registration.Namespace is null);
        Assert.Contains(result.Assemblies, registration => registration.AssemblyName == "Nested.Controls" && registration.Namespace == "Contoso.Controls.Nested");
        Assert.Contains(result.NamespaceTags, registration =>
            registration.TagPrefix == "webopt"
            && registration.Namespace == "Microsoft.AspNet.Web.Optimization.WebForms"
            && registration.AssemblyName == "Microsoft.AspNet.Web.Optimization.WebForms");
    }

    [Fact]
    public void Parse_ReturnsEmptyResult_WhenWebConfigIsMissing()
    {
        var parser = new WebConfigAssemblyParser();

        var result = parser.Parse(Path.Combine(_testRoot, "missing.config"));

        Assert.Null(result.Error);
        Assert.Empty(result.Assemblies);
        Assert.Empty(result.NamespaceTags);
        Assert.Empty(result.RegisterDirectives);
    }

    [Fact]
    public void Parse_ReturnsError_WhenWebConfigIsMalformed()
    {
        var webConfigPath = Path.Combine(_testRoot, "Web.config");
        File.WriteAllText(webConfigPath, "<configuration><system.web>");

        var parser = new WebConfigAssemblyParser();

        var result = parser.Parse(webConfigPath);

        Assert.NotNull(result.Error);
        Assert.Empty(result.Assemblies);
        Assert.Empty(result.NamespaceTags);
    }

    [Fact]
    public void ParseProject_ExtractsRegisterDirectives()
    {
        File.WriteAllText(Path.Combine(_testRoot, "Web.config"), """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration />
            """);

        File.WriteAllText(Path.Combine(_testRoot, "Students.aspx"), """
            <%@ Page Language="C#" %>
            <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
            """);

        var accountDirectory = Path.Combine(_testRoot, "Account");
        Directory.CreateDirectory(accountDirectory);
        File.WriteAllText(Path.Combine(accountDirectory, "Login.aspx"), """
            <%@ Page Language="C#" %>
            <%@ Register Src="~/Account/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>
            """);

        var parser = new WebConfigAssemblyParser();

        var result = parser.ParseProject(_testRoot);

        Assert.Contains(result.RegisterDirectives, registration =>
            registration.TagPrefix == "ajaxToolkit"
            && registration.Namespace == "AjaxControlToolkit"
            && registration.AssemblyName == "AjaxControlToolkit"
            && registration.SourceFilePath == "Students.aspx");
        Assert.Contains(result.RegisterDirectives, registration =>
            registration.TagPrefix == "uc"
            && registration.TagName == "OpenAuthProviders"
            && registration.SourceVirtualPath == "~/Account/OpenAuthProviders.ascx"
            && registration.SourceFilePath == Path.Combine("Account", "Login.aspx"));
    }

    public void Dispose()
    {
        if (Directory.Exists(_testRoot))
        {
            try
            {
                Directory.Delete(_testRoot, recursive: true);
            }
            catch
            {
            }
        }
    }
}
