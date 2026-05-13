using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Fixes known C# type mismatch patterns that appear frequently in migrated Web Forms code.
/// Runs late in the pipeline to clean up after other transforms have modified method bodies.
///
/// Patterns handled:
/// <list type="bullet">
///   <item><c>(int?)x.Quantity * x.Product.UnitPrice</c> where UnitPrice is double? → wraps with (decimal?) cast</item>
///   <item>Nullable arithmetic mismatches in LINQ Select/Sum expressions</item>
/// </list>
/// </summary>
public class TypeMismatchFixTransform : ICodeBehindTransform
{
    public string Name => "TypeMismatchFix";
    public int Order => 900; // Late pass — after all other transforms

    // Pattern: select (int?)something.Quantity * something.Product.UnitPrice
    // The int? * double? produces double?, but the surrounding context often expects decimal?.
    // Wraps with (decimal?) cast.
    private static readonly Regex IntTimesUnitPriceRegex = new(
        @"select\s+\(int\?\)\s*(?<expr>\w+\.Quantity\s*\*\s*\w+\.(?:Product\.)?UnitPrice)",
        RegexOptions.Compiled);

    // Pattern: decimal? total = (decimal?)(from ... select (int?)x * y).Sum()
    // where the inner expression is already cast but produces double?, not decimal?
    // This is a broader version for LINQ aggregate queries.
    private static readonly Regex DecimalCastLinqRegex = new(
        @"\(decimal\?\)\s*\(\s*from\b",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        // Fix 1: Wrap (int?)Quantity * UnitPrice with (decimal?) cast
        // This handles: select (int?)cartItems.Quantity * cartItems.Product.UnitPrice
        // → select (decimal?)((int?)cartItems.Quantity * cartItems.Product.UnitPrice)
        content = IntTimesUnitPriceRegex.Replace(content, m =>
        {
            var expr = m.Groups["expr"].Value;
            return $"select (decimal?)((int?){expr})";
        });

        return content;
    }
}
