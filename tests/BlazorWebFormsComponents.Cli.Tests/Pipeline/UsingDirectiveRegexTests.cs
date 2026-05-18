using System.Text.RegularExpressions;
using Xunit;

namespace BlazorWebFormsComponents.Cli.Tests.Pipeline;

/// <summary>
/// Verifies the regex used to find "last using-directive" in Program.cs
/// doesn't accidentally match using-block statements like "using (var scope = ...)".
/// </summary>
public class UsingDirectiveRegexTests
{
	private const string UsingDirectivePattern = @"^using\s+[^;(\n]+;\s*$";

	[Fact]
	public void Matches_SimpleUsingDirective()
	{
		var content = "using System;\nusing System.Collections.Generic;";
		var match = Regex.Match(content, UsingDirectivePattern,
			RegexOptions.Multiline | RegexOptions.RightToLeft);
		Assert.True(match.Success);
		Assert.Equal("using System.Collections.Generic;", match.Value.Trim());
	}

	[Fact]
	public void DoesNotMatch_UsingBlockStatement()
	{
		var content = @"using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
}";
		var match = Regex.Match(content, UsingDirectivePattern,
			RegexOptions.Multiline | RegexOptions.RightToLeft);
		Assert.False(match.Success);
	}

	[Fact]
	public void Matches_LastDirective_NotBlockStatement_InProgramCs()
	{
		var content = @"using System;
using WingtipToys.Models;

var builder = WebApplication.CreateBuilder(args);
// ...
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
}
app.Run();";
		var match = Regex.Match(content, UsingDirectivePattern,
			RegexOptions.Multiline | RegexOptions.RightToLeft);
		Assert.True(match.Success);
		Assert.Equal("using WingtipToys.Models;", match.Value.Trim());
	}

	[Fact]
	public void InsertedUsing_GoesAfterDirectives_NotInsideBlock()
	{
		var content = @"using System;
using WingtipToys.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager>();
}

app.Run();";

		var usingLine = "using WingtipToys.Logic;";
		if (!content.Contains(usingLine, StringComparison.Ordinal))
		{
			var lastUsing = Regex.Match(content, UsingDirectivePattern,
				RegexOptions.Multiline | RegexOptions.RightToLeft);
			Assert.True(lastUsing.Success);
			var insertAt = lastUsing.Index + lastUsing.Length;
			content = content[..insertAt] + "\n" + usingLine + content[insertAt..];
		}

		// The new using should appear right after the other top-level usings
		var lines = content.Split('\n');
		var logicLineIndex = Array.FindIndex(lines, l => l.Trim() == "using WingtipToys.Logic;");
		var modelsLineIndex = Array.FindIndex(lines, l => l.Trim() == "using WingtipToys.Models;");
		var builderLineIndex = Array.FindIndex(lines, l => l.Contains("var builder"));

		Assert.True(logicLineIndex > modelsLineIndex,
			"using WingtipToys.Logic should appear after using WingtipToys.Models");
		Assert.True(logicLineIndex < builderLineIndex,
			"using WingtipToys.Logic should appear before var builder (in the top-level usings section)");
	}
}
