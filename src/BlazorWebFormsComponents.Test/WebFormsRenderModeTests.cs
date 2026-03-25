using System;
using BlazorWebFormsComponents;
using Bunit;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using Xunit;

using BWFCBase = BlazorWebFormsComponents.BaseWebFormsComponent;

namespace BlazorWebFormsComponents.Test;

/// <summary>
/// Contract tests for WebFormsRenderMode enum and CurrentRenderMode property.
/// Validates auto-detection based on HttpContext availability.
/// </summary>
public class WebFormsRenderModeTests : IDisposable
{
	private readonly BunitContext _ctx;

	public WebFormsRenderModeTests()
	{
		_ctx = new BunitContext();
		_ctx.JSInterop.SetupVoid("bwfc.Page.OnAfterRender");
		_ctx.Services.AddSingleton<LinkGenerator>(new Mock<LinkGenerator>().Object);
		_ctx.Services.AddSingleton<IDataProtectionProvider>(new EphemeralDataProtectionProvider());
		_ctx.Services.AddLogging();
	}

	public void Dispose() => _ctx.Dispose();

	#region Test Component

	/// <summary>
	/// Concrete test component that exposes protected CurrentRenderMode for assertion.
	/// </summary>
	private class TestComponent : BWFCBase
	{
		public WebFormsRenderMode ExposedRenderMode => CurrentRenderMode;
		public bool ExposedIsHttpContextAvailable => IsHttpContextAvailable;

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder.OpenElement(0, "div");
			builder.AddContent(1, "TestComponent");
			builder.CloseElement();
		}
	}

	#endregion

	#region WebFormsRenderMode Enum

	[Fact]
	public void WebFormsRenderMode_HasStaticSSR()
	{
		Enum.IsDefined(typeof(WebFormsRenderMode), WebFormsRenderMode.StaticSSR).ShouldBeTrue();
	}

	[Fact]
	public void WebFormsRenderMode_HasInteractiveServer()
	{
		Enum.IsDefined(typeof(WebFormsRenderMode), WebFormsRenderMode.InteractiveServer).ShouldBeTrue();
	}

	[Fact]
	public void WebFormsRenderMode_HasExactlyTwoValues()
	{
		var values = Enum.GetValues(typeof(WebFormsRenderMode));
		values.Length.ShouldBe(2);
	}

	#endregion

	#region CurrentRenderMode — SSR (HttpContext Available)

	[Fact]
	public void CurrentRenderMode_WithHttpContext_ReturnsStaticSSR()
	{
		var httpContext = new DefaultHttpContext();
		var mock = new Mock<IHttpContextAccessor>();
		mock.Setup(x => x.HttpContext).Returns(httpContext);
		_ctx.Services.AddSingleton<IHttpContextAccessor>(mock.Object);

		var cut = _ctx.Render<TestComponent>();

		cut.Instance.ExposedRenderMode.ShouldBe(WebFormsRenderMode.StaticSSR);
	}

	[Fact]
	public void IsHttpContextAvailable_WithHttpContext_ReturnsTrue()
	{
		var httpContext = new DefaultHttpContext();
		var mock = new Mock<IHttpContextAccessor>();
		mock.Setup(x => x.HttpContext).Returns(httpContext);
		_ctx.Services.AddSingleton<IHttpContextAccessor>(mock.Object);

		var cut = _ctx.Render<TestComponent>();

		cut.Instance.ExposedIsHttpContextAvailable.ShouldBeTrue();
	}

	#endregion

	#region CurrentRenderMode — InteractiveServer (No HttpContext)

	[Fact]
	public void CurrentRenderMode_WithoutHttpContext_ReturnsInteractiveServer()
	{
		HttpContext? noContext = null;
		var mock = new Mock<IHttpContextAccessor>();
		mock.Setup(x => x.HttpContext).Returns(noContext);
		_ctx.Services.AddSingleton<IHttpContextAccessor>(mock.Object);

		var cut = _ctx.Render<TestComponent>();

		cut.Instance.ExposedRenderMode.ShouldBe(WebFormsRenderMode.InteractiveServer);
	}

	[Fact]
	public void IsHttpContextAvailable_WithoutHttpContext_ReturnsFalse()
	{
		HttpContext? noContext = null;
		var mock = new Mock<IHttpContextAccessor>();
		mock.Setup(x => x.HttpContext).Returns(noContext);
		_ctx.Services.AddSingleton<IHttpContextAccessor>(mock.Object);

		var cut = _ctx.Render<TestComponent>();

		cut.Instance.ExposedIsHttpContextAvailable.ShouldBeFalse();
	}

	#endregion
}
