using BlazorWebFormsComponents.Cli.Scaffolding;

namespace BlazorWebFormsComponents.Cli.Tests;

public class MasterPageToLayoutConverterTests
{
	private readonly MasterPageToLayoutConverter _converter = new();

	private static string GetRepoRoot()
	{
		var dir = new DirectoryInfo(AppContext.BaseDirectory);
		while (dir != null)
		{
			if (File.Exists(Path.Combine(dir.FullName, "BlazorMeetsWebForms.sln")))
				return dir.FullName;
			dir = dir.Parent;
		}
		throw new DirectoryNotFoundException("Could not locate repository root.");
	}

	private static string GetSampleMasterPath(params string[] pathParts)
	{
		var parts = new[] { GetRepoRoot(), "samples" }.Concat(pathParts).ToArray();
		return Path.Combine(parts);
	}

	[Fact]
	public void Convert_WingtipToysMaster_ProducesLayoutWithNavbar()
	{
		var masterContent = File.ReadAllText(
			GetSampleMasterPath("WingtipToys", "WingtipToys", "Site.Master"));

		var result = _converter.Convert(masterContent);

		Assert.NotNull(result);
		Assert.Contains("@inherits LayoutComponentBase", result);
		Assert.Contains("@Body", result);
		Assert.Contains("navbar", result);
		Assert.Contains("Wingtip Toys", result);
	}

	[Fact]
	public void Convert_WingtipToysMaster_ConvertsLoginViewToAuthorizeView()
	{
		var masterContent = File.ReadAllText(
			GetSampleMasterPath("WingtipToys", "WingtipToys", "Site.Master"));

		var result = _converter.Convert(masterContent);

		Assert.NotNull(result);
		Assert.Contains("<AuthorizeView>", result);
		Assert.Contains("</AuthorizeView>", result);
		Assert.Contains("<Authorized>", result);
		Assert.Contains("<NotAuthorized>", result);
		Assert.DoesNotContain("asp:LoginView", result);
		Assert.DoesNotContain("asp:LoginStatus", result);
	}

	[Fact]
	public void Convert_WingtipToysMaster_StripsServerInfrastructure()
	{
		var masterContent = File.ReadAllText(
			GetSampleMasterPath("WingtipToys", "WingtipToys", "Site.Master"));

		var result = _converter.Convert(masterContent);

		Assert.NotNull(result);
		Assert.DoesNotContain("runat=\"server\"", result);
		Assert.DoesNotContain("asp:ScriptManager", result);
		Assert.DoesNotContain("<!DOCTYPE", result);
		Assert.DoesNotContain("</html>", result);
		Assert.DoesNotContain("</head>", result);
		Assert.DoesNotContain("</body>", result);
		Assert.DoesNotContain("<%@", result);
	}

	[Fact]
	public void Convert_WingtipToysMaster_ConvertsTildeSlashToSlash()
	{
		var masterContent = File.ReadAllText(
			GetSampleMasterPath("WingtipToys", "WingtipToys", "Site.Master"));

		var result = _converter.Convert(masterContent);

		Assert.NotNull(result);
		Assert.DoesNotContain("~/", result);
	}

	[Fact]
	public void Convert_WingtipToysMaster_PreservesNavLinks()
	{
		var masterContent = File.ReadAllText(
			GetSampleMasterPath("WingtipToys", "WingtipToys", "Site.Master"));

		var result = _converter.Convert(masterContent);

		Assert.NotNull(result);
		Assert.Contains("href=\"/\"", result);
		Assert.Contains("href=\"/About\"", result);
		Assert.Contains("href=\"/Contact\"", result);
		Assert.Contains("href=\"/ProductList\"", result);
		Assert.Contains("href=\"/ShoppingCart\"", result);
	}

	[Fact]
	public void Convert_WingtipToysMaster_PreservesFooter()
	{
		var masterContent = File.ReadAllText(
			GetSampleMasterPath("WingtipToys", "WingtipToys", "Site.Master"));

		var result = _converter.Convert(masterContent);

		Assert.NotNull(result);
		Assert.Contains("<footer>", result);
		Assert.Contains("Wingtip Toys", result);
	}

	[Fact]
	public void Convert_ContosoUniversityMaster_ProducesLayoutWithNavLinks()
	{
		var masterContent = File.ReadAllText(
			GetSampleMasterPath("ContosoUniversity", "ContosoUniversity", "Site.Master"));

		var result = _converter.Convert(masterContent);

		Assert.NotNull(result);
		Assert.Contains("@inherits LayoutComponentBase", result);
		Assert.Contains("@Body", result);
		Assert.Contains("Contoso University", result);
	}

	[Fact]
	public void Convert_DepartmentPortalMaster_ProducesLayoutWithBootstrapNavbar()
	{
		var masterContent = File.ReadAllText(
			GetSampleMasterPath("DepartmentPortal", "Site.Master"));

		var result = _converter.Convert(masterContent);

		Assert.NotNull(result);
		Assert.Contains("@inherits LayoutComponentBase", result);
		Assert.Contains("@Body", result);
		Assert.Contains("navbar", result);
		Assert.Contains("Department Portal", result);
	}

	[Fact]
	public void Convert_MinimalMasterPage_ProducesLayout()
	{
		var master = """
			<%@ Master Language="C#" %>
			<!DOCTYPE html>
			<html>
			<head runat="server"><title>Test</title></head>
			<body>
			<form runat="server">
			<div class="container">
			    <asp:ContentPlaceHolder ID="MainContent" runat="server" />
			</div>
			</form>
			</body>
			</html>
			""";

		var result = _converter.Convert(master);

		Assert.NotNull(result);
		Assert.Contains("@inherits LayoutComponentBase", result);
		Assert.Contains("@Body", result);
		Assert.Contains("container", result);
		Assert.DoesNotContain("<!DOCTYPE", result);
		Assert.DoesNotContain("runat", result);
	}

	[Fact]
	public void Convert_NullOrEmpty_ReturnsNull()
	{
		Assert.Null(_converter.Convert(""));
		Assert.Null(_converter.Convert("   "));
		Assert.Null(_converter.Convert(null!));
	}

	[Fact]
	public void Convert_StripsAspxExtensions()
	{
		var master = """
			<%@ Master Language="C#" %>
			<html><body><form runat="server">
			<a href="Products.aspx">Products</a>
			<asp:ContentPlaceHolder ID="MainContent" runat="server" />
			</form></body></html>
			""";

		var result = _converter.Convert(master);

		Assert.NotNull(result);
		Assert.Contains("Products", result);
		Assert.DoesNotContain(".aspx", result);
	}

	[Fact]
	public void FindMasterPage_FindsWingtipToysMaster()
	{
		var sourcePath = GetSampleMasterPath("WingtipToys", "WingtipToys");

		var result = MasterPageToLayoutConverter.FindMasterPage(sourcePath);

		Assert.NotNull(result);
		Assert.Equal("Site.Master", Path.GetFileName(result));
	}
}
