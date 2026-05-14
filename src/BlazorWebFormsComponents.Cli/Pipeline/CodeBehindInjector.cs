using System.Text;
using System.Text.RegularExpressions;

namespace BlazorWebFormsComponents.Cli.Pipeline;

/// <summary>
/// Shared utility for injecting members into code-behind partial class bodies.
/// Used by transforms and semantic patterns that need to add parameters, fields,
/// or methods to the .razor.cs file instead of emitting @code blocks.
/// </summary>
public static class CodeBehindInjector
{
	// Matches [Parameter] or [Parameter, ...] property declarations
	private static readonly Regex ExistingParameterRegex = new(
		@"\[Parameter[^\]]*\]\s*(?:public|protected|private|internal)\s+\w[\w<>,\s\?\[\]]*\s+(?<name>\w+)\s*\{",
		RegexOptions.Compiled);

	/// <summary>
	/// Inserts one or more member declarations (fields, properties, methods) into
	/// the last class body in the code-behind content, just before the final closing brace.
	/// Automatically deduplicates [Parameter] properties using case-insensitive comparison.
	/// </summary>
	/// <param name="codeBehind">The full code-behind content.</param>
	/// <param name="members">Lines of C# code to inject (without outer braces).</param>
	/// <returns>The updated code-behind content, or the original if injection fails.</returns>
	public static string InjectMembers(string codeBehind, string members)
	{
		if (string.IsNullOrWhiteSpace(codeBehind) || string.IsNullOrWhiteSpace(members))
			return codeBehind;

		// Find the last closing brace of the class body (inside the namespace)
		var lastBrace = FindClassClosingBrace(codeBehind);
		if (lastBrace < 0)
			return codeBehind;

		// Deduplicate: remove [Parameter] lines from members if an equivalent
		// property (case-insensitive) already exists in the code-behind
		members = DeduplicateParameters(codeBehind, members);
		if (string.IsNullOrWhiteSpace(members))
			return codeBehind;

		// Ensure the injected members have proper spacing
		var injection = "\n" + members.TrimEnd() + "\n";
		return codeBehind.Insert(lastBrace, injection);
	}

	/// <summary>
	/// Removes [Parameter] property declarations from <paramref name="members"/>
	/// if a property with the same name (case-insensitive) already exists in <paramref name="existingCode"/>.
	/// Non-parameter members (fields, methods) are always kept.
	/// </summary>
	internal static string DeduplicateParameters(string existingCode, string members)
	{
		// Collect existing parameter names (case-insensitive)
		var existingNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (Match m in ExistingParameterRegex.Matches(existingCode))
		{
			existingNames.Add(m.Groups["name"].Value);
		}

		if (existingNames.Count == 0)
			return members;

		// Process members line-by-line, skipping duplicate [Parameter] declarations
		var lines = members.Split('\n');
		var result = new StringBuilder();

		for (var i = 0; i < lines.Length; i++)
		{
			var trimmed = lines[i].TrimStart();

			// Detect [Parameter...] attribute line — check if the following property is a dupe
			if (trimmed.StartsWith("[Parameter", StringComparison.Ordinal))
			{
				// Look ahead for the property declaration (may be on same or next line)
				var combined = trimmed;
				var lookAhead = i + 1;
				while (lookAhead < lines.Length && !combined.Contains('{'))
				{
					combined += " " + lines[lookAhead].TrimStart();
					lookAhead++;
				}

				var propMatch = Regex.Match(combined, @"(?:public|protected|private|internal)\s+\w[\w<>,\s\?\[\]]*\s+(?<name>\w+)\s*\{");
				if (propMatch.Success && existingNames.Contains(propMatch.Groups["name"].Value))
				{
					// Skip this parameter attribute + property declaration lines
					// Advance past all lookahead lines that were part of this declaration
					var linesToSkip = lookAhead - i - 1;
					i += linesToSkip;
					continue;
				}
			}

			result.AppendLine(lines[i]);
		}

		return result.ToString().TrimEnd('\r', '\n');
	}

	/// <summary>
	/// Generates a minimal scaffold code-behind for a .razor file that has no
	/// corresponding .aspx.cs source.
	/// </summary>
	public static string GenerateScaffold(string projectNamespace, string className, string? subNamespace = null)
	{
		var sb = new StringBuilder();
		sb.AppendLine("using Microsoft.AspNetCore.Components;");
		sb.AppendLine();

		var fullNamespace = string.IsNullOrEmpty(subNamespace)
			? projectNamespace
			: $"{projectNamespace}.{subNamespace}";

		sb.AppendLine($"namespace {fullNamespace};");
		sb.AppendLine();
		sb.AppendLine($"public partial class {className}");
		sb.AppendLine("{");
		sb.Append('}');
		return sb.ToString();
	}

	/// <summary>
	/// Derives the Blazor class name from a .razor output path using the same
	/// logic as ClassNameAlignTransform.
	/// </summary>
	public static string DeriveClassName(string outputFilePath)
	{
		var razorFileName = Path.GetFileNameWithoutExtension(outputFilePath);
		if (razorFileName.EndsWith(".razor", StringComparison.OrdinalIgnoreCase))
			razorFileName = Path.GetFileNameWithoutExtension(razorFileName);

		var name = razorFileName.Replace('.', '_').Replace('-', '_');
		if (name.Length > 0 && char.IsDigit(name[0]))
			name = "_" + name;

		// PascalCase the first letter and after underscores
		if (name.Length > 0)
			name = char.ToUpperInvariant(name[0]) + name[1..];
		name = Regex.Replace(name, @"(?<=_)([a-z])", m => m.Value.ToUpperInvariant());

		return name;
	}

	/// <summary>
	/// Derives the sub-namespace from the relative path of the output file within the project.
	/// For example, "Account/Login.razor" → "Account".
	/// </summary>
	public static string? DeriveSubNamespace(string outputFilePath, string outputRootPath)
	{
		var relativePath = Path.GetRelativePath(outputRootPath, outputFilePath);
		var directory = Path.GetDirectoryName(relativePath);
		if (string.IsNullOrEmpty(directory) || directory == ".")
			return null;

		// Convert path separators to dots for namespace
		return directory
			.Replace(Path.DirectorySeparatorChar, '.')
			.Replace(Path.AltDirectorySeparatorChar, '.');
	}

	/// <summary>
	/// Finds the closing brace of the last class body — i.e., the second-to-last '}'.
	/// The last '}' closes the namespace (if file-scoped namespace, there's only one).
	/// </summary>
	private static int FindClassClosingBrace(string content)
	{
		// For file-scoped namespace (ends with ;), the last } is the class closing brace
		if (Regex.IsMatch(content, @"^namespace\s+[\w.]+\s*;", RegexOptions.Multiline))
		{
			return content.LastIndexOf('}');
		}

		// For block-scoped namespace, the second-to-last } is the class
		var lastBrace = content.LastIndexOf('}');
		if (lastBrace < 0) return -1;
		var secondLast = content.LastIndexOf('}', lastBrace - 1);
		return secondLast >= 0 ? secondLast : lastBrace;
	}
}
