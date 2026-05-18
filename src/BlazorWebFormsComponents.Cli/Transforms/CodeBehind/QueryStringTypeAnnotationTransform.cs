using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Replaces <c>var</c> with <c>string?</c> on assignments from
/// <c>Request.QueryString["key"]</c> or <c>Request["key"]</c>.
///
/// In Web Forms, <c>Request.QueryString["key"]</c> returns <see langword="string"/>.
/// In BWFC's <c>RequestShim</c>, the same indexer returns
/// <c>Microsoft.Extensions.Primitives.StringValues</c>. Using <c>var</c> captures
/// the <c>StringValues</c> type; if that variable is later used inside an EF Core
/// LINQ closure, EF cannot parameterize <c>StringValues</c> and throws
/// <c>InvalidCastException: Object must implement IConvertible</c>.
///
/// This transform forces an explicit <c>string?</c> type annotation so the
/// implicit conversion from <c>StringValues</c> to <c>string</c> happens at
/// assignment time.
/// </summary>
public class QueryStringTypeAnnotationTransform : ICodeBehindTransform
{
	public string Name => "QueryStringTypeAnnotation";
	public int Order => 325;

	// var x = Request.QueryString["..."]
	private static readonly Regex VarQueryStringRegex = new(
		@"\bvar\b(\s+\w+\s*=\s*Request\.QueryString\s*\[)",
		RegexOptions.Compiled);

	// var x = Request["..."]
	private static readonly Regex VarRequestIndexerRegex = new(
		@"\bvar\b(\s+\w+\s*=\s*Request\s*\[)",
		RegexOptions.Compiled);

	public string Apply(string content, FileMetadata metadata)
	{
		content = VarQueryStringRegex.Replace(content, "string?$1");
		content = VarRequestIndexerRegex.Replace(content, "string?$1");
		return content;
	}
}
