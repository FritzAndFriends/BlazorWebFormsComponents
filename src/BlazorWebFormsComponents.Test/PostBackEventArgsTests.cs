using System;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Test;

/// <summary>
/// Unit tests for <see cref="PostBackEventArgs"/>.
/// Covers construction, property values, immutability, and inheritance.
/// </summary>
public class PostBackEventArgsTests
{
	#region Construction

	[Fact]
	public void Constructor_SetsEventTarget()
	{
		var args = new PostBackEventArgs("Button1", "click");

		args.EventTarget.ShouldBe("Button1");
	}

	[Fact]
	public void Constructor_SetsEventArgument()
	{
		var args = new PostBackEventArgs("Button1", "submit");

		args.EventArgument.ShouldBe("submit");
	}

	[Fact]
	public void Constructor_NullTarget_Allowed()
	{
		var args = new PostBackEventArgs(null!, "arg");

		args.EventTarget.ShouldBeNull();
	}

	[Fact]
	public void Constructor_NullArgument_Allowed()
	{
		var args = new PostBackEventArgs("target", null!);

		args.EventArgument.ShouldBeNull();
	}

	[Fact]
	public void Constructor_BothNull_Allowed()
	{
		var args = new PostBackEventArgs(null!, null!);

		args.EventTarget.ShouldBeNull();
		args.EventArgument.ShouldBeNull();
	}

	[Fact]
	public void Constructor_EmptyStrings_Allowed()
	{
		var args = new PostBackEventArgs("", "");

		args.EventTarget.ShouldBe("");
		args.EventArgument.ShouldBe("");
	}

	#endregion

	#region Immutability

	[Fact]
	public void EventTarget_IsReadOnly()
	{
		var prop = typeof(PostBackEventArgs).GetProperty("EventTarget");

		prop.ShouldNotBeNull();
		prop!.CanWrite.ShouldBeFalse("EventTarget should be read-only");
	}

	[Fact]
	public void EventArgument_IsReadOnly()
	{
		var prop = typeof(PostBackEventArgs).GetProperty("EventArgument");

		prop.ShouldNotBeNull();
		prop!.CanWrite.ShouldBeFalse("EventArgument should be read-only");
	}

	#endregion

	#region Inheritance

	[Fact]
	public void InheritsFromEventArgs()
	{
		var args = new PostBackEventArgs("t", "a");

		args.ShouldBeAssignableTo<System.EventArgs>();
	}

	#endregion

	#region Round-Trip Values

	[Fact]
	public void PreservesSpecialCharacters()
	{
		var args = new PostBackEventArgs("btn'1", "arg\\with'special");

		args.EventTarget.ShouldBe("btn'1");
		args.EventArgument.ShouldBe("arg\\with'special");
	}

	[Fact]
	public void PreservesUnicodeCharacters()
	{
		var args = new PostBackEventArgs("按钮", "提交数据");

		args.EventTarget.ShouldBe("按钮");
		args.EventArgument.ShouldBe("提交数据");
	}

	[Fact]
	public void PreservesLongStrings()
	{
		var longTarget = new string('x', 10000);
		var longArg = new string('y', 10000);

		var args = new PostBackEventArgs(longTarget, longArg);

		args.EventTarget.ShouldBe(longTarget);
		args.EventArgument.ShouldBe(longArg);
	}

	#endregion
}
