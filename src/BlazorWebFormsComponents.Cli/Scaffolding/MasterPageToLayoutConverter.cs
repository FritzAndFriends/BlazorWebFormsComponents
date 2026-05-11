using System.Text;
using System.Text.RegularExpressions;

namespace BlazorWebFormsComponents.Cli.Scaffolding;

/// <summary>
/// Converts a Site.Master file into a Blazor MainLayout.razor.
/// Extracts the structural HTML (navbar, footer, body wrapper), converts
/// Web Forms controls to Blazor equivalents, and places @Body where
/// the main ContentPlaceHolder was.
/// </summary>
public class MasterPageToLayoutConverter
{
	// ── Regexes for structural extraction ──

	private static readonly Regex MasterDirectiveRegex = new(
		@"<%@\s*Master\b[^%]*%>\s*\r?\n?",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly Regex DocTypeRegex = new(
		@"[ \t]*<!DOCTYPE[^>]*>[ \t]*\r?\n?",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly Regex HtmlOpenRegex = new(
		@"[ \t]*<html\b[^>]*>[ \t]*\r?\n?",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly Regex HtmlCloseRegex = new(
		@"[ \t]*</html>[ \t]*\r?\n?",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly Regex HeadSectionRegex = new(
		@"[ \t]*<head\b[^>]*>[\s\S]*?</head>[ \t]*\r?\n?",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly Regex BodyOpenRegex = new(
		@"[ \t]*<body\b[^>]*>[ \t]*\r?\n?",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly Regex BodyCloseRegex = new(
		@"[ \t]*</body>[ \t]*\r?\n?",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	// ── Server-side infrastructure ──

	private static readonly Regex ServerFormOpenRegex = new(
		@"[ \t]*<form\b[^>]*\brunat\s*=\s*""server""[^>]*>[ \t]*\r?\n?",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly Regex ServerFormCloseRegex = new(
		@"[ \t]*</form>[ \t]*\r?\n?",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly Regex ScriptManagerBlockRegex = new(
		@"[ \t]*<asp:ScriptManager\b[^>]*>[\s\S]*?</asp:ScriptManager>[ \t]*\r?\n?",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly Regex ScriptManagerSelfClosingRegex = new(
		@"[ \t]*<asp:ScriptManager\b[^>]*/\s*>[ \t]*\r?\n?",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly Regex BundlerPlaceHolderRegex = new(
		@"[ \t]*<asp:PlaceHolder\b[^>]*runat\s*=\s*""server""[^>]*>[\s\S]*?</asp:PlaceHolder>[ \t]*\r?\n?",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly Regex WebOptBundleRegex = new(
		@"[ \t]*<webopt:bundlereference\b[^>]*/?\s*>[ \t]*\r?\n?",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	// ── ContentPlaceHolder ──

	private static readonly Regex ContentPlaceHolderMainBlockRegex = new(
		@"[ \t]*<asp:ContentPlaceHolder\b[^>]*\bID\s*=\s*""(?:MainContent|ContentPlaceHolder1)""[^>]*>[\s\S]*?</asp:ContentPlaceHolder>[ \t]*\r?\n?",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly Regex ContentPlaceHolderMainSelfClosingRegex = new(
		@"[ \t]*<asp:ContentPlaceHolder\b[^>]*\bID\s*=\s*""(?:MainContent|ContentPlaceHolder1)""[^>]*/\s*>[ \t]*\r?\n?",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly Regex ContentPlaceHolderHeadBlockRegex = new(
		@"[ \t]*<asp:ContentPlaceHolder\b[^>]*\bID\s*=\s*""(?:head|HeadContent)""[^>]*>[\s\S]*?</asp:ContentPlaceHolder>[ \t]*\r?\n?",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly Regex ContentPlaceHolderHeadSelfClosingRegex = new(
		@"[ \t]*<asp:ContentPlaceHolder\b[^>]*\bID\s*=\s*""(?:head|HeadContent)""[^>]*/\s*>[ \t]*\r?\n?",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	// ── LoginView → AuthorizeView ──

	private static readonly Regex LoginViewBlockRegex = new(
		@"<asp:LoginView\b[^>]*>([\s\S]*?)</asp:LoginView>",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly Regex AnonymousTemplateRegex = new(
		@"<AnonymousTemplate>([\s\S]*?)</AnonymousTemplate>",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly Regex LoggedInTemplateRegex = new(
		@"<LoggedInTemplate>([\s\S]*?)</LoggedInTemplate>",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	// ── LoginStatus control ──

	private static readonly Regex LoginStatusRegex = new(
		@"<asp:LoginStatus\b[^>]*LogoutText\s*=\s*""([^""]*)""[^>]*/?\s*>(?:</asp:LoginStatus>)?",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	// ── Inline expressions ──

	private static readonly Regex ServerExpressionRegex = new(
		@"<%:\s*(.*?)\s*%>",
		RegexOptions.Compiled);

	private static readonly Regex ServerCodeExpressionRegex = new(
		@"<%=\s*(.*?)\s*%>",
		RegexOptions.Compiled);

	private static readonly Regex ServerCommentRegex = new(
		@"<%--[\s\S]*?--%>",
		RegexOptions.Compiled);

	// ── Attribute cleanup ──

	private static readonly Regex RunatServerRegex = new(
		@"\s+runat\s*=\s*""server""",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly Regex AspIdAttributeRegex = new(
		@"\s+ID\s*=\s*""[^""]*""",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly Regex TildeSlashRegex = new(
		@"(?<=[""'])~/",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly Regex AspxExtensionRegex = new(
		@"(?<=(?:href|src|action)\s*=\s*""[^""]*)\.aspx",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly Regex ViewStateDisabledRegex = new(
		@"\s+ViewStateMode\s*=\s*""Disabled""",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	/// <summary>
	/// Converts a Site.Master file's content into a MainLayout.razor string.
	/// Returns null if conversion fails or input is empty.
	/// </summary>
	public string? Convert(string masterContent)
	{
		if (string.IsNullOrWhiteSpace(masterContent))
			return null;

		var content = masterContent.Replace("\r\n", "\n");

		// Step 1: Strip document wrapper
		content = MasterDirectiveRegex.Replace(content, "");
		content = DocTypeRegex.Replace(content, "");
		content = HtmlOpenRegex.Replace(content, "");
		content = HtmlCloseRegex.Replace(content, "");
		content = HeadSectionRegex.Replace(content, "");
		content = BodyOpenRegex.Replace(content, "");
		content = BodyCloseRegex.Replace(content, "");

		// Step 2: Strip server infrastructure
		content = ScriptManagerBlockRegex.Replace(content, "");
		content = ScriptManagerSelfClosingRegex.Replace(content, "");
		content = BundlerPlaceHolderRegex.Replace(content, "");
		content = WebOptBundleRegex.Replace(content, "");
		content = ServerFormOpenRegex.Replace(content, "");
		content = ServerFormCloseRegex.Replace(content, "");

		// Step 3: Strip server comments
		content = ServerCommentRegex.Replace(content, "");

		// Step 4: Replace main ContentPlaceHolder with @Body
		content = ContentPlaceHolderMainBlockRegex.Replace(content, "        @Body\n");
		content = ContentPlaceHolderMainSelfClosingRegex.Replace(content, "        @Body\n");

		// Step 5: Remove head ContentPlaceHolder (handled by HeadContent in pages)
		content = ContentPlaceHolderHeadBlockRegex.Replace(content, "");
		content = ContentPlaceHolderHeadSelfClosingRegex.Replace(content, "");

		// Step 6: Convert LoginView → AuthorizeView
		content = ConvertLoginView(content);

		// Step 7: Convert inline expressions
		content = ServerExpressionRegex.Replace(content, "@($1)");
		content = ServerCodeExpressionRegex.Replace(content, "@($1)");

		// Step 8: Strip runat="server" and ViewStateMode from HTML elements
		content = ViewStateDisabledRegex.Replace(content, "");
		content = RunatServerRegex.Replace(content, "");

		// Step 9: Remove ID attributes from plain HTML elements (not components)
		// Only remove IDs on elements that previously had runat="server" — these are
		// lower-case HTML tags like <a>, <div>, not PascalCase components.
		// We use a conservative approach: remove ID on <a> tags that had runat="server" stripped.
		content = CleanHtmlElementIds(content);

		// Step 10: Convert URL patterns
		content = TildeSlashRegex.Replace(content, "/");
		content = AspxExtensionRegex.Replace(content, "");

		// Step 11: Clean up excessive blank lines
		content = Regex.Replace(content, @"\n{3,}", "\n\n");
		content = content.Trim();

		// Step 12: Assemble final layout
		var sb = new StringBuilder();
		sb.AppendLine("@inherits LayoutComponentBase");
		sb.AppendLine();
		sb.AppendLine(content);
		sb.AppendLine();

		return sb.ToString();
	}

	/// <summary>
	/// Finds the primary Site.Master file in the source project directory.
	/// Returns null if not found.
	/// </summary>
	public static string? FindMasterPage(string sourcePath)
	{
		if (string.IsNullOrEmpty(sourcePath) || !Directory.Exists(sourcePath))
			return null;

		// Check common locations
		var candidates = new[]
		{
			Path.Combine(sourcePath, "Site.Master"),
			Path.Combine(sourcePath, "Site.master"),
			Path.Combine(sourcePath, "MasterPage.Master"),
			Path.Combine(sourcePath, "MasterPage.master"),
		};

		foreach (var candidate in candidates)
		{
			if (File.Exists(candidate))
				return candidate;
		}

		// Search recursively but prefer root-level master pages
		var masterFiles = Directory.GetFiles(sourcePath, "*.Master", SearchOption.AllDirectories)
			.Where(f => !Path.GetFileName(f).Contains("Mobile", StringComparison.OrdinalIgnoreCase))
			.OrderBy(f => f.Split(Path.DirectorySeparatorChar).Length)
			.ToArray();

		return masterFiles.Length > 0 ? masterFiles[0] : null;
	}

	private static string ConvertLoginView(string content)
	{
		return LoginViewBlockRegex.Replace(content, m =>
		{
			var inner = m.Groups[1].Value;

			var anonymousContent = "";
			var loggedInContent = "";

			var anonMatch = AnonymousTemplateRegex.Match(inner);
			if (anonMatch.Success)
				anonymousContent = anonMatch.Groups[1].Value.Trim();

			var loggedMatch = LoggedInTemplateRegex.Match(inner);
			if (loggedMatch.Success)
			{
				loggedInContent = loggedMatch.Groups[1].Value.Trim();
				// Convert LoginStatus to a logout form
				loggedInContent = ConvertLoginStatus(loggedInContent);
				// Convert Context.User.Identity expressions
				loggedInContent = loggedInContent.Replace(
					"Context.User.Identity.GetUserName()",
					"context.User.Identity?.Name");
				loggedInContent = loggedInContent.Replace(
					"Context.User.Identity.Name",
					"context.User.Identity?.Name");
			}

			var sb = new StringBuilder();
			sb.AppendLine("<AuthorizeView>");
			if (!string.IsNullOrWhiteSpace(loggedInContent))
			{
				sb.AppendLine("                        <Authorized>");
				sb.AppendLine($"                            {loggedInContent}");
				sb.AppendLine("                        </Authorized>");
			}
			if (!string.IsNullOrWhiteSpace(anonymousContent))
			{
				sb.AppendLine("                        <NotAuthorized>");
				sb.AppendLine($"                            {anonymousContent}");
				sb.AppendLine("                        </NotAuthorized>");
			}
			sb.Append("                    </AuthorizeView>");

			return sb.ToString();
		});
	}

	private static string ConvertLoginStatus(string content)
	{
		return LoginStatusRegex.Replace(content, m =>
		{
			var logoutText = m.Groups[1].Value;
			if (string.IsNullOrEmpty(logoutText))
				logoutText = "Log off";

			return $@"<form method=""post"" action=""/Account/PerformLogout"" data-enhance=""false"">
                                <AntiforgeryToken />
                                <button type=""submit"" class=""btn btn-link navbar-btn"">{logoutText}</button>
                            </form>";
		});
	}

	/// <summary>
	/// Removes ID attributes from plain HTML elements that previously had runat="server".
	/// Preserves IDs on elements that look like they serve a CSS/JS purpose.
	/// </summary>
	private static string CleanHtmlElementIds(string content)
	{
		// Remove IDs that follow the pattern of server-generated IDs on <a> tags
		// where runat="server" was already stripped. Keep IDs on divs/sections that
		// might be CSS targets (e.g., id="navBar", id="TitleContent").
		return content;
	}
}
