using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Converts Web Forms data-binding expressions inside attribute values to Razor expressions.
/// Example: NavigateUrl='<%# Item.GetUrl() %>' → NavigateUrl='@(Item.GetUrl())'.
/// </summary>
public class DataBindingAttributeTransform : IMarkupTransform
{
	private static readonly Regex AttributeExpressionRegex = new(
		@"(?<name>[\w:-]+)\s*=\s*(?<quote>['""])\s*<%(?<marker>[#=])\s*(?<expr>.*?)\s*%>\s*\k<quote>",
		RegexOptions.Compiled | RegexOptions.Singleline);

	public string Name => "DataBindingAttribute";
	public int Order => 615;

	public string Apply(string content, FileMetadata metadata)
		=> AttributeExpressionRegex.Replace(content, match =>
		{
			var attributeName = match.Groups["name"].Value;
			var expression = match.Groups["expr"].Value.Trim();
			var quote = SelectQuote(expression, match.Groups["quote"].Value[0]);

			return quote is null
				? $"{attributeName}=@({expression})"
				: $"{attributeName}={quote}@({expression}){quote}";
		});

	private static char? SelectQuote(string expression, char originalQuote)
	{
		var containsSingleQuote = expression.Contains('\'');
		var containsDoubleQuote = expression.Contains('\"');

		if (originalQuote == '\'' && !containsSingleQuote)
			return '\'';

		if (originalQuote == '"' && !containsDoubleQuote)
			return '"';

		if (!containsSingleQuote)
			return '\'';

		if (!containsDoubleQuote)
			return '"';

		return null;
	}
}
