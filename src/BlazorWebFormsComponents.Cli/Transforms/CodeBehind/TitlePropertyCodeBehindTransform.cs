using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Adds a Title property to the code-behind when the markup references @(Title)
/// and no Title property already exists. Uses the title extracted from the
/// original <%@ Page Title="..." %> directive.
/// </summary>
public class TitlePropertyCodeBehindTransform : ICodeBehindTransform
{
	public string Name => "TitleProperty";
	public int Order => 850;

	private static readonly Regex TitleReferenceRegex = new(
		@"@\(?\s*Title\s*\)?",
		RegexOptions.Compiled);

	private static readonly Regex TitlePropertyRegex = new(
		@"\b(string|protected\s+string|public\s+string|private\s+string)\s+Title\s*[{=;]",
		RegexOptions.Compiled);

	public string Apply(string content, FileMetadata metadata)
	{
		var markup = metadata.MarkupContent ?? string.Empty;

		// Only act if markup uses @(Title) or @Title and code-behind doesn't already have a Title property
		if (!TitleReferenceRegex.IsMatch(markup))
			return content;

		if (TitlePropertyRegex.IsMatch(content))
			return content;

		var titleValue = metadata.PageTitle ?? "Page";
		var titleField = $"\tprotected string Title {{ get; set; }} = \"{EscapeCSharpString(titleValue)}\";";

		// Insert the Title property right after the class opening brace
		var classBodyIndex = FindClassBodyInsertionPoint(content);
		if (classBodyIndex < 0)
			return content;

		return content[..classBodyIndex] + "\n" + titleField + "\n" + content[classBodyIndex..];
	}

	private static int FindClassBodyInsertionPoint(string content)
	{
		// Find the first { after "partial class" or "class"
		var classMatch = Regex.Match(content, @"\bclass\s+\w+[^{]*\{");
		if (!classMatch.Success)
			return -1;

		return classMatch.Index + classMatch.Length;
	}

	private static string EscapeCSharpString(string value) =>
		value.Replace("\\", "\\\\").Replace("\"", "\\\"");
}
