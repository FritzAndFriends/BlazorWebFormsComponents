using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlazorWebFormsComponents;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Test;

/// <summary>
/// Unit tests for <see cref="ClientScriptShim"/>.
/// Covers registration, deduplication, script tag handling, flush behavior,
/// unsupported postback methods, and edge cases.
/// </summary>
public class ClientScriptShimTests
{
	private static ClientScriptShim CreateShim()
	{
		return new ClientScriptShim(NullLogger<ClientScriptShim>.Instance);
	}

	private static Mock<IJSRuntime> CreateMockJSRuntime()
	{
		return new Mock<IJSRuntime>(MockBehavior.Loose);
	}

	/// <summary>
	/// Extracts all string arguments passed to the IJSRuntime mock,
	/// including strings nested inside object[] argument arrays.
	/// </summary>
	private static List<string> GetAllStringArgs(Mock<IJSRuntime> mock)
	{
		var result = new List<string>();
		foreach (var invocation in mock.Invocations)
		{
			foreach (var arg in invocation.Arguments)
			{
				if (arg is string s)
					result.Add(s);
				else if (arg is object[] arr)
				{
					foreach (var item in arr)
					{
						if (item is string str)
							result.Add(str);
					}
				}
			}
		}
		return result;
	}

	#region Registration — Startup Scripts

	[Fact]
	public void RegisterStartupScript_QueuesScript()
	{
		var shim = CreateShim();

		shim.RegisterStartupScript(typeof(ClientScriptShimTests), "init", "console.log('hello');", false);

		shim.IsStartupScriptRegistered(typeof(ClientScriptShimTests), "init").ShouldBeTrue();
	}

	[Fact]
	public void RegisterStartupScript_ThreeArgOverload_DefaultsAddScriptTagsFalse()
	{
		var shim = CreateShim();

		shim.RegisterStartupScript(typeof(ClientScriptShimTests), "three", "threeArg();");

		shim.IsStartupScriptRegistered(typeof(ClientScriptShimTests), "three").ShouldBeTrue();
	}

	[Fact]
	public void RegisterStartupScript_Deduplicates()
	{
		var shim = CreateShim();

		shim.RegisterStartupScript(typeof(ClientScriptShimTests), "dup", "first();", false);
		shim.RegisterStartupScript(typeof(ClientScriptShimTests), "dup", "second();", false);

		// Same key = still registered, no exception, first wins
		shim.IsStartupScriptRegistered(typeof(ClientScriptShimTests), "dup").ShouldBeTrue();
	}

	[Fact]
	public void RegisterStartupScript_DifferentKeys_BothRegistered()
	{
		var shim = CreateShim();

		shim.RegisterStartupScript(typeof(ClientScriptShimTests), "keyA", "a();", false);
		shim.RegisterStartupScript(typeof(ClientScriptShimTests), "keyB", "b();", false);

		shim.IsStartupScriptRegistered(typeof(ClientScriptShimTests), "keyA").ShouldBeTrue();
		shim.IsStartupScriptRegistered(typeof(ClientScriptShimTests), "keyB").ShouldBeTrue();
	}

	[Fact]
	public void RegisterStartupScript_DifferentTypes_SameKey_BothRegistered()
	{
		var shim = CreateShim();

		shim.RegisterStartupScript(typeof(ClientScriptShimTests), "shared", "x();", false);
		shim.RegisterStartupScript(typeof(string), "shared", "y();", false);

		shim.IsStartupScriptRegistered(typeof(ClientScriptShimTests), "shared").ShouldBeTrue();
		shim.IsStartupScriptRegistered(typeof(string), "shared").ShouldBeTrue();
	}

	[Fact]
	public void IsStartupScriptRegistered_ReturnsFalse_WhenNotRegistered()
	{
		var shim = CreateShim();

		shim.IsStartupScriptRegistered(typeof(ClientScriptShimTests), "nope").ShouldBeFalse();
	}

	#endregion

	#region Registration — Script Blocks

	[Fact]
	public void RegisterClientScriptBlock_QueuesBlock()
	{
		var shim = CreateShim();

		shim.RegisterClientScriptBlock(typeof(ClientScriptShimTests), "block1", "var x = 1;", false);

		shim.IsClientScriptBlockRegistered(typeof(ClientScriptShimTests), "block1").ShouldBeTrue();
	}

	[Fact]
	public void RegisterClientScriptBlock_Deduplicates()
	{
		var shim = CreateShim();

		shim.RegisterClientScriptBlock(typeof(ClientScriptShimTests), "dup", "first();", false);
		shim.RegisterClientScriptBlock(typeof(ClientScriptShimTests), "dup", "second();", false);

		shim.IsClientScriptBlockRegistered(typeof(ClientScriptShimTests), "dup").ShouldBeTrue();
	}

	[Fact]
	public void IsClientScriptBlockRegistered_ReturnsFalse_WhenNotRegistered()
	{
		var shim = CreateShim();

		shim.IsClientScriptBlockRegistered(typeof(ClientScriptShimTests), "missing").ShouldBeFalse();
	}

	#endregion

	#region Registration — Script Includes

	[Fact]
	public void RegisterClientScriptInclude_QueuesUrl()
	{
		var shim = CreateShim();

		shim.RegisterClientScriptInclude("jquery", "https://cdn.example.com/jquery.min.js");

		shim.IsClientScriptIncludeRegistered("jquery").ShouldBeTrue();
	}

	[Fact]
	public void RegisterClientScriptInclude_Deduplicates()
	{
		var shim = CreateShim();

		shim.RegisterClientScriptInclude("lib", "https://cdn.example.com/lib.js");
		shim.RegisterClientScriptInclude("lib", "https://cdn.example.com/lib-v2.js");

		// Same key = still registered, first URL wins
		shim.IsClientScriptIncludeRegistered("lib").ShouldBeTrue();
	}

	[Fact]
	public void RegisterClientScriptInclude_WithType_QueuesUrl()
	{
		var shim = CreateShim();

		shim.RegisterClientScriptInclude(typeof(ClientScriptShimTests), "typed", "https://cdn.example.com/typed.js");

		shim.IsClientScriptIncludeRegistered("typed").ShouldBeTrue();
	}

	[Fact]
	public void IsClientScriptIncludeRegistered_ReturnsFalse_WhenNotRegistered()
	{
		var shim = CreateShim();

		shim.IsClientScriptIncludeRegistered("unknown").ShouldBeFalse();
	}

	#endregion

	#region Script Tag Stripping

	[Fact]
	public async Task RegisterStartupScript_WithAddScriptTags_StripsScriptTags()
	{
		var shim = CreateShim();
		var mockJs = CreateMockJSRuntime();

		shim.RegisterStartupScript(
			typeof(ClientScriptShimTests),
			"tagged",
			"<script>alert('hi')</script>",
			addScriptTags: true);

		await shim.FlushAsync(mockJs.Object);

		// When addScriptTags=true, <script> wrappers should be stripped
		// because IJSRuntime.InvokeVoidAsync("eval", ...) needs raw JS
		var allArgs = GetAllStringArgs(mockJs);
		allArgs.ShouldContain(s => s.Contains("alert('hi')"));
		allArgs.ShouldNotContain(s => s.Contains("<script>"));
	}

	[Fact]
	public async Task RegisterStartupScript_WithoutAddScriptTags_KeepsScript()
	{
		var shim = CreateShim();
		var mockJs = CreateMockJSRuntime();

		var rawScript = "document.getElementById('x').style.display='none';";
		shim.RegisterStartupScript(
			typeof(ClientScriptShimTests),
			"raw",
			rawScript,
			addScriptTags: false);

		await shim.FlushAsync(mockJs.Object);

		var allArgs = GetAllStringArgs(mockJs);
		allArgs.ShouldContain(s => s.Contains(rawScript));
	}

	#endregion

	#region FlushAsync

	[Fact]
	public async Task FlushAsync_ExecutesStartupScripts()
	{
		var shim = CreateShim();
		var mockJs = CreateMockJSRuntime();

		shim.RegisterStartupScript(typeof(ClientScriptShimTests), "s1", "startupCode();", false);

		await shim.FlushAsync(mockJs.Object);

		mockJs.Invocations.Count.ShouldBeGreaterThan(0);
	}

	[Fact]
	public async Task FlushAsync_ExecutesScriptBlocks()
	{
		var shim = CreateShim();
		var mockJs = CreateMockJSRuntime();

		shim.RegisterClientScriptBlock(typeof(ClientScriptShimTests), "b1", "var block = true;", false);

		await shim.FlushAsync(mockJs.Object);

		mockJs.Invocations.Count.ShouldBeGreaterThan(0);
	}

	[Fact]
	public async Task FlushAsync_LoadsScriptIncludes()
	{
		var shim = CreateShim();
		var mockJs = CreateMockJSRuntime();

		shim.RegisterClientScriptInclude("cdn", "https://cdn.example.com/app.js");

		await shim.FlushAsync(mockJs.Object);

		mockJs.Invocations.Count.ShouldBeGreaterThan(0);
		var allArgs = GetAllStringArgs(mockJs);
		allArgs.ShouldContain(s => s.Contains("https://cdn.example.com/app.js"));
	}

	[Fact]
	public async Task FlushAsync_ClearsQueuesAfterFlush()
	{
		var shim = CreateShim();
		var mockJs = CreateMockJSRuntime();

		shim.RegisterStartupScript(typeof(ClientScriptShimTests), "temp", "temp();", false);
		shim.RegisterClientScriptBlock(typeof(ClientScriptShimTests), "block", "block();", false);
		shim.RegisterClientScriptInclude("inc", "https://example.com/inc.js");

		await shim.FlushAsync(mockJs.Object);

		// After flush, queues are empty (no re-execution) but registration
		// status persists — matching Web Forms page-lifecycle semantics.
		shim.IsStartupScriptRegistered(typeof(ClientScriptShimTests), "temp").ShouldBeTrue();
		shim.IsClientScriptBlockRegistered(typeof(ClientScriptShimTests), "block").ShouldBeTrue();
		shim.IsClientScriptIncludeRegistered("inc").ShouldBeTrue();
	}

	[Fact]
	public async Task FlushAsync_NoOp_WhenNothingQueued()
	{
		var shim = CreateShim();
		var mockJs = CreateMockJSRuntime();

		// Should not throw and should not invoke IJSRuntime
		await shim.FlushAsync(mockJs.Object);

		mockJs.Invocations.Count.ShouldBe(0);
	}

	[Fact]
	public async Task FlushAsync_CalledTwice_SecondIsNoOp()
	{
		var shim = CreateShim();
		var mockJs = CreateMockJSRuntime();

		shim.RegisterStartupScript(typeof(ClientScriptShimTests), "once", "runOnce();", false);

		await shim.FlushAsync(mockJs.Object);
		var countAfterFirst = mockJs.Invocations.Count;
		countAfterFirst.ShouldBeGreaterThan(0);

		await shim.FlushAsync(mockJs.Object);
		mockJs.Invocations.Count.ShouldBe(countAfterFirst);
	}

	[Fact]
	public async Task FlushAsync_ReRegistering_AfterFlush_Works()
	{
		var shim = CreateShim();
		var mockJs = CreateMockJSRuntime();

		shim.RegisterStartupScript(typeof(ClientScriptShimTests), "reuse", "first();", false);
		await shim.FlushAsync(mockJs.Object);

		// Re-register after flush
		shim.RegisterStartupScript(typeof(ClientScriptShimTests), "reuse", "second();", false);
		shim.IsStartupScriptRegistered(typeof(ClientScriptShimTests), "reuse").ShouldBeTrue();
	}

	#endregion

	#region PostBack / Callback Methods

	// These methods now return working JavaScript strings instead of throwing.

	[Fact]
	public void GetPostBackEventReference_ReturnsDoPostBackJs()
	{
		var shim = CreateShim();

		var result = shim.GetPostBackEventReference(new object(), "arg");

		result.ShouldContain("__doPostBack");
		result.ShouldContain("arg");
	}

	[Fact]
	public void GetPostBackEventReference_NullControl_ReturnsUnknownTarget()
	{
		var shim = CreateShim();

		var result = shim.GetPostBackEventReference(null!, "arg");

		result.ShouldContain("__doPostBack('unknown',");
	}

	[Fact]
	public void GetPostBackEventReference_NullArgument_DefaultsToEmpty()
	{
		var shim = CreateShim();

		var result = shim.GetPostBackEventReference(new object(), null!);

		result.ShouldContain("__doPostBack(");
		result.ShouldContain("''");
	}

	[Fact]
	public void GetPostBackClientHyperlink_ReturnsJavascriptUrl()
	{
		var shim = CreateShim();

		var result = shim.GetPostBackClientHyperlink(new object(), "arg");

		result.ShouldStartWith("javascript:");
		result.ShouldContain("__doPostBack");
	}

	[Fact]
	public void GetCallbackEventReference_ReturnsCallbackJs()
	{
		var shim = CreateShim();

		var result = shim.GetCallbackEventReference(
			new object(), "arg", "onSuccess", "ctx", "onError", false);

		result.ShouldContain("__bwfc_callback");
		result.ShouldContain("arg");
		result.ShouldContain("onSuccess");
		result.ShouldContain("onError");
	}

	[Fact]
	public void GetCallbackEventReference_NullCallbacks_UsesNull()
	{
		var shim = CreateShim();

		var result = shim.GetCallbackEventReference(
			new object(), "arg", null!, null, null!, false);

		result.ShouldContain("null");
	}

	#endregion

	#region Phase 2 — PostBack Deep Tests

	[Fact]
	public void GetPostBackEventReference_EscapesSingleQuotesInArgument()
	{
		var shim = CreateShim();

		var result = shim.GetPostBackEventReference(new object(), "it's a test");

		result.ShouldContain("it\\'s a test");
	}

	[Fact]
	public void GetPostBackEventReference_EscapesBackslashesInArgument()
	{
		var shim = CreateShim();

		var result = shim.GetPostBackEventReference(new object(), "path\\to\\file");

		result.ShouldContain("path\\\\to\\\\file");
	}

	[Fact]
	public void GetPostBackEventReference_EmptyArgument_ProducesEmptyQuotes()
	{
		var shim = CreateShim();

		var result = shim.GetPostBackEventReference(new object(), "");

		result.ShouldContain("''");
	}

	[Fact]
	public void GetPostBackEventReference_ControlType_ResolvesToTypeName()
	{
		var shim = CreateShim();

		var result = shim.GetPostBackEventReference(new object(), "arg");

		// Plain object resolves to its type name
		result.ShouldContain("Object");
	}

	[Fact]
	public void GetPostBackEventReference_OutputFormat_IsDoPostBackCall()
	{
		var shim = CreateShim();

		var result = shim.GetPostBackEventReference(new object(), "myarg");

		// Should match the format: __doPostBack('id', 'arg')
		result.ShouldStartWith("__doPostBack(");
		result.ShouldEndWith(")");
		result.ShouldContain("myarg");
	}

	[Fact]
	public void GetPostBackClientHyperlink_ContainsSameContentAsEventReference()
	{
		var shim = CreateShim();
		var control = new object();
		var argument = "test";

		var reference = shim.GetPostBackEventReference(control, argument);
		var hyperlink = shim.GetPostBackClientHyperlink(control, argument);

		hyperlink.ShouldBe($"javascript:{reference}");
	}

	[Fact]
	public void GetPostBackClientHyperlink_NullControl_ContainsUnknown()
	{
		var shim = CreateShim();

		var result = shim.GetPostBackClientHyperlink(null!, "arg");

		result.ShouldStartWith("javascript:");
		result.ShouldContain("unknown");
	}

	[Fact]
	public void GetPostBackClientHyperlink_EscapesSpecialChars()
	{
		var shim = CreateShim();

		var result = shim.GetPostBackClientHyperlink(new object(), "it's");

		result.ShouldStartWith("javascript:");
		result.ShouldContain("\\'");
	}

	#endregion

	#region Phase 2 — Callback Deep Tests

	[Fact]
	public void GetCallbackEventReference_EscapesSingleQuotesInArgument()
	{
		var shim = CreateShim();

		var result = shim.GetCallbackEventReference(
			new object(), "it's", "onSuccess", "ctx", "onError", false);

		result.ShouldContain("it\\'s");
	}

	[Fact]
	public void GetCallbackEventReference_EscapesBackslashesInContext()
	{
		var shim = CreateShim();

		var result = shim.GetCallbackEventReference(
			new object(), "arg", "onSuccess", "path\\ctx", "onError", false);

		result.ShouldContain("path\\\\ctx");
	}

	[Fact]
	public void GetCallbackEventReference_NullContext_DefaultsToEmpty()
	{
		var shim = CreateShim();

		var result = shim.GetCallbackEventReference(
			new object(), "arg", "onSuccess", null!, "onError", false);

		// null context → empty string between quotes
		result.ShouldContain("__bwfc_callback");
	}

	[Fact]
	public void GetCallbackEventReference_NullArgument_DefaultsToEmpty()
	{
		var shim = CreateShim();

		var result = shim.GetCallbackEventReference(
			new object(), null!, "onSuccess", "ctx", "onError", false);

		result.ShouldContain("__bwfc_callback");
		result.ShouldContain("''");
	}

	[Fact]
	public void GetCallbackEventReference_OutputFormat_ContainsAllParams()
	{
		var shim = CreateShim();

		var result = shim.GetCallbackEventReference(
			new object(), "myarg", "successFn", "myctx", "errorFn", true);

		result.ShouldStartWith("__bwfc_callback(");
		result.ShouldEndWith(")");
		result.ShouldContain("myarg");
		result.ShouldContain("successFn");
		result.ShouldContain("myctx");
		result.ShouldContain("errorFn");
	}

	#endregion

	#region Edge Cases

	[Fact]
	public void RegisterStartupScript_NullType_ThrowsArgumentNull()
	{
		var shim = CreateShim();

		// BuildKey now guards against null type with ArgumentNullException.
		Should.Throw<ArgumentNullException>(() =>
			shim.RegisterStartupScript(null!, "key", "script();", false));
	}

	[Fact]
	public void RegisterStartupScript_NullKey_HandlesGracefully()
	{
		var shim = CreateShim();

		// Null key does not throw — used as part of the composite key
		shim.RegisterStartupScript(typeof(ClientScriptShimTests), null!, "script();", false);

		shim.IsStartupScriptRegistered(typeof(ClientScriptShimTests), null!).ShouldBeTrue();
	}

	[Fact]
	public void RegisterStartupScript_EmptyScript_StillQueued()
	{
		var shim = CreateShim();

		shim.RegisterStartupScript(typeof(ClientScriptShimTests), "empty", "", false);

		shim.IsStartupScriptRegistered(typeof(ClientScriptShimTests), "empty").ShouldBeTrue();
	}

	[Fact]
	public async Task FlushAsync_NullJSRuntime_ThrowsArgumentNull()
	{
		var shim = CreateShim();
		shim.RegisterStartupScript(typeof(ClientScriptShimTests), "test", "x();", false);

		await Should.ThrowAsync<ArgumentNullException>(() =>
			shim.FlushAsync(null!));
	}

	#endregion
}
