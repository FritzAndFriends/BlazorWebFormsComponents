using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Replaces <c>new HttpException(...)</c> with <c>new InvalidOperationException(...)</c>
/// since <c>System.Web.HttpException</c> does not exist in .NET Core.
/// Also strips the HTTP status code parameter that HttpException accepts.
/// </summary>
public class HttpExceptionTransform : ICodeBehindTransform
{
    public string Name => "HttpException";
    public int Order => 108;

    // new HttpException(statusCode, message, innerException)
    private static readonly Regex ThreeArgRegex = new(
        @"new\s+HttpException\s*\(\s*\d+\s*,\s*([^,]+)\s*,\s*([^)]+)\s*\)",
        RegexOptions.Compiled);

    // new HttpException(statusCode, message)
    private static readonly Regex TwoArgRegex = new(
        @"new\s+HttpException\s*\(\s*\d+\s*,\s*([^)]+)\s*\)",
        RegexOptions.Compiled);

    // new HttpException(message)
    private static readonly Regex OneArgRegex = new(
        @"new\s+HttpException\s*\(\s*([^)]+)\s*\)",
        RegexOptions.Compiled);

    // throw new HttpException(...)
    private static readonly Regex ThrowRegex = new(
        @"throw\s+new\s+HttpException\b",
        RegexOptions.Compiled);

    // catch (HttpException ...)
    private static readonly Regex CatchRegex = new(
        @"catch\s*\(\s*HttpException\b",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        if (!content.Contains("HttpException"))
            return content;

        // Replace 3-arg: new HttpException(code, msg, inner) → new InvalidOperationException(msg, inner)
        content = ThreeArgRegex.Replace(content, "new InvalidOperationException($1, $2)");

        // Replace 2-arg: new HttpException(code, msg) → new InvalidOperationException(msg)
        content = TwoArgRegex.Replace(content, "new InvalidOperationException($1)");

        // Replace 1-arg: new HttpException(msg) → new InvalidOperationException(msg)
        content = OneArgRegex.Replace(content, "new InvalidOperationException($1)");

        // Replace catch (HttpException → catch (InvalidOperationException
        content = CatchRegex.Replace(content, "catch (InvalidOperationException");

        return content;
    }
}
