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
        Assert.Equal("Microsoft.AspNet.Web.Optimization.WebForms", result.PrefixToNamespaceMap["webopt"]);
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

    [Fact]
    public void Parse_ExtractsNamespaceTags_FromNamespacedWebConfig()
    {
        var webConfigPath = Path.Combine(_testRoot, "Web.config");
        File.WriteAllText(webConfigPath, """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration xmlns="urn:test:webconfig">
              <system.web>
                <pages>
                  <controls>
                    <add assembly="Contoso.Web" namespace="Contoso.Web.Controls" tagPrefix="contoso" />
                  </controls>
                </pages>
              </system.web>
            </configuration>
            """);

        var parser = new WebConfigAssemblyParser();

        var result = parser.Parse(webConfigPath);

        Assert.Null(result.Error);
        Assert.Contains(result.NamespaceTags, registration =>
            registration.TagPrefix == "contoso"
            && registration.Namespace == "Contoso.Web.Controls"
            && registration.AssemblyName == "Contoso.Web");
    }

    [Fact]
    public void ParseProject_DeduplicatesRegisterDirectives_CaseInsensitive()
    {
        File.WriteAllText(Path.Combine(_testRoot, "Web.config"), """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration />
            """);

        File.WriteAllText(Path.Combine(_testRoot, "Default.aspx"), """
            <%@ Page Language="C#" %>
            <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
            <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AJAXTOOLKIT" %>
            """);

        var parser = new WebConfigAssemblyParser();

        var result = parser.ParseProject(_testRoot);

        var registrations = result.RegisterDirectives
            .Where(registration =>
                string.Equals(registration.TagPrefix, "ajaxToolkit", StringComparison.OrdinalIgnoreCase)
                && string.Equals(registration.Namespace, "AjaxControlToolkit", StringComparison.OrdinalIgnoreCase))
            .ToList();

        Assert.Single(registrations);
    }

    [Fact]
    public void ParseProject_MergesAllWebConfigControlRegistrations()
    {
        File.WriteAllText(Path.Combine(_testRoot, "Web.config"), """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <system.web>
                <pages>
                  <controls>
                    <add tagPrefix="root" namespace="Contoso.Root.Controls" assembly="Contoso.Root" />
                  </controls>
                </pages>
              </system.web>
            </configuration>
            """);

        var adminDirectory = Path.Combine(_testRoot, "Admin");
        Directory.CreateDirectory(adminDirectory);
        File.WriteAllText(Path.Combine(adminDirectory, "Web.config"), """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <system.web>
                <pages>
                  <controls>
                    <add tagPrefix="admin" namespace="Contoso.Admin.Controls" assembly="Contoso.Admin" />
                  </controls>
                </pages>
              </system.web>
            </configuration>
            """);

        var parser = new WebConfigAssemblyParser();

        var result = parser.ParseProject(_testRoot);

        Assert.Contains(result.NamespaceTags, registration =>
            registration.TagPrefix == "root"
            && registration.Namespace == "Contoso.Root.Controls");
        Assert.Contains(result.NamespaceTags, registration =>
            registration.TagPrefix == "admin"
            && registration.Namespace == "Contoso.Admin.Controls");
        Assert.Equal("Contoso.Root.Controls", result.PrefixToNamespaceMap["root"]);
        Assert.Equal("Contoso.Admin.Controls", result.PrefixToNamespaceMap["admin"]);
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
