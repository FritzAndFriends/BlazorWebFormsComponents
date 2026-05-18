using System;
using System.Reflection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Test;

/// <summary>
/// Unit tests for <see cref="ScriptManagerShim"/>.
/// Covers the static GetCurrent factory, constructor validation,
/// and delegation of RegisterXxx methods to ClientScriptShim.
/// </summary>
public class ScriptManagerShimTests
{
	private static ClientScriptShim CreateClientScriptShim()
	{
		return new ClientScriptShim(NullLogger<ClientScriptShim>.Instance);
	}

	#region Constructor

	[Fact]
	public void Constructor_WithNullClientScript_ThrowsArgumentNull()
	{
		Should.Throw<ArgumentNullException>(() => new ScriptManagerShim(null!));
	}

	[Fact]
	public void Constructor_WithValidClientScript_DoesNotThrow()
	{
		var shim = CreateClientScriptShim();

		var sm = new ScriptManagerShim(shim);

		sm.ShouldNotBeNull();
	}

	#endregion

	#region GetCurrent — Error Cases

	[Fact]
	public void GetCurrent_WithPlainObject_ThrowsInvalidOperation()
	{
		Should.Throw<InvalidOperationException>(
			() => ScriptManagerShim.GetCurrent("not a component"));
	}

	[Fact]
	public void GetCurrent_WithNull_ThrowsInvalidOperation()
	{
		Should.Throw<InvalidOperationException>(
			() => ScriptManagerShim.GetCurrent(null!));
	}

	[Fact]
	public void GetCurrent_WithIntegerArg_ThrowsInvalidOperation()
	{
		Should.Throw<InvalidOperationException>(
			() => ScriptManagerShim.GetCurrent(42));
	}

	[Fact]
	public void GetCurrent_ErrorMessage_MentionsBaseWebFormsComponent()
	{
		var ex = Should.Throw<InvalidOperationException>(
			() => ScriptManagerShim.GetCurrent(new object()));

		ex.Message.ShouldContain("BaseWebFormsComponent");
	}

	#endregion

	#region GetCurrent — Success via Reflection

	[Fact]
	public void GetCurrent_WithComponentHavingClientScript_ReturnsShim()
	{
		var clientScript = CreateClientScriptShim();
		var mockComponent = new Mock<BlazorWebFormsComponents.BaseWebFormsComponent>() { CallBase = true };

		// Use reflection to inject the ClientScriptShim into the private field
		var clientScriptField = typeof(BlazorWebFormsComponents.BaseWebFormsComponent)
			.GetField("_clientScript", BindingFlags.NonPublic | BindingFlags.Instance);
		var resolvedField = typeof(BlazorWebFormsComponents.BaseWebFormsComponent)
			.GetField("_clientScriptResolved", BindingFlags.NonPublic | BindingFlags.Instance);

		clientScriptField!.SetValue(mockComponent.Object, clientScript);
		resolvedField!.SetValue(mockComponent.Object, true);

		var result = ScriptManagerShim.GetCurrent(mockComponent.Object);

		result.ShouldNotBeNull();
	}

	[Fact]
	public void GetCurrent_WithComponentWithoutClientScript_Throws()
	{
		// Component with ClientScript == null (no service provider set)
		var mockComponent = new Mock<BlazorWebFormsComponents.BaseWebFormsComponent>() { CallBase = true };

		Should.Throw<InvalidOperationException>(
			() => ScriptManagerShim.GetCurrent(mockComponent.Object));
	}

	#endregion

	#region Delegation — RegisterStartupScript

	[Fact]
	public void RegisterStartupScript_DelegatesToClientScript()
	{
		var clientScript = CreateClientScriptShim();
		var sm = new ScriptManagerShim(clientScript);

		sm.RegisterStartupScript(typeof(ScriptManagerShimTests), "key1", "alert('hi');", false);

		clientScript.IsStartupScriptRegistered(typeof(ScriptManagerShimTests), "key1").ShouldBeTrue();
	}

	[Fact]
	public void RegisterStartupScript_WithControl_DelegatesToClientScript()
	{
		var clientScript = CreateClientScriptShim();
		var sm = new ScriptManagerShim(clientScript);

		sm.RegisterStartupScript(new object(), typeof(ScriptManagerShimTests), "key2", "alert('x');", false);

		clientScript.IsStartupScriptRegistered(typeof(ScriptManagerShimTests), "key2").ShouldBeTrue();
	}

	[Fact]
	public void RegisterStartupScript_WithAddScriptTags_DelegatesToClientScript()
	{
		var clientScript = CreateClientScriptShim();
		var sm = new ScriptManagerShim(clientScript);

		sm.RegisterStartupScript(typeof(ScriptManagerShimTests), "tagged", "<script>x();</script>", true);

		clientScript.IsStartupScriptRegistered(typeof(ScriptManagerShimTests), "tagged").ShouldBeTrue();
	}

	#endregion

	#region Delegation — RegisterClientScriptBlock

	[Fact]
	public void RegisterClientScriptBlock_DelegatesToClientScript()
	{
		var clientScript = CreateClientScriptShim();
		var sm = new ScriptManagerShim(clientScript);

		sm.RegisterClientScriptBlock(typeof(ScriptManagerShimTests), "block1", "var x = 1;", false);

		clientScript.IsClientScriptBlockRegistered(typeof(ScriptManagerShimTests), "block1").ShouldBeTrue();
	}

	#endregion

	#region Delegation — RegisterClientScriptInclude

	[Fact]
	public void RegisterClientScriptInclude_DelegatesToClientScript()
	{
		var clientScript = CreateClientScriptShim();
		var sm = new ScriptManagerShim(clientScript);

		sm.RegisterClientScriptInclude(typeof(ScriptManagerShimTests), "inc1", "https://cdn.example.com/lib.js");

		clientScript.IsClientScriptIncludeRegistered("inc1").ShouldBeTrue();
	}

	#endregion
}
