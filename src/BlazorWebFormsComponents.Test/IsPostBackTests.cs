using System;
using System.Threading.Tasks;
using BlazorWebFormsComponents;
using Bunit;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

using BWFCBase = BlazorWebFormsComponents.BaseWebFormsComponent;
using BWFCPage = BlazorWebFormsComponents.WebFormsPageBase;
using BWFCPageService = BlazorWebFormsComponents.PageService;

namespace BlazorWebFormsComponents.Test;

/// <summary>
/// Contract tests for IsPostBack behavior on BaseWebFormsComponent and WebFormsPageBase.
/// Validates mode-adaptive semantics per the ViewState-PostBack-Shim-Proposal:
///   SSR GET → false, SSR POST → true,
///   Interactive first render → false, Interactive after init → true.
/// </summary>
public class IsPostBackTests : IDisposable
{
	private readonly BunitContext _ctx;

	public IsPostBackTests()
	{
		_ctx = new BunitContext();
		_ctx.JSInterop.SetupVoid("bwfc.Page.OnAfterRender");
		_ctx.Services.AddSingleton<LinkGenerator>(new Mock<LinkGenerator>().Object);
		_ctx.Services.AddSingleton<IDataProtectionProvider>(new EphemeralDataProtectionProvider());
		_ctx.Services.AddLogging();
	}

	public void Dispose() => _ctx.Dispose();

	#region Test Components

	/// <summary>
	/// Concrete test component inheriting BaseWebFormsComponent.
	/// Captures IsPostBack during OnInitializedAsync (before base sets _hasInitialized).
	/// </summary>
	private class TestComponent : BWFCBase
	{
		public bool IsPostBackValue => IsPostBack;
		public bool IsPostBackDuringInit { get; private set; }

		protected override async Task OnInitializedAsync()
		{
			IsPostBackDuringInit = IsPostBack;
			await base.OnInitializedAsync();
		}

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder.OpenElement(0, "div");
			builder.AddContent(1, "TestComponent");
			builder.CloseElement();
		}
	}

	/// <summary>
	/// Concrete test page inheriting WebFormsPageBase.
	/// Captures IsPostBack during OnInitialized (before base sets _hasInitialized).
	/// </summary>
	private class TestPage : BWFCPage
	{
		public bool IsPostBackValue => IsPostBack;
		public bool IsPostBackDuringInit { get; private set; }

		protected override void OnInitialized()
		{
			IsPostBackDuringInit = IsPostBack;
			base.OnInitialized();
		}

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder.OpenElement(0, "div");
			builder.AddContent(1, "TestPage");
			builder.CloseElement();
		}
	}

	#endregion

	#region Helpers

	private void RegisterNoHttpContext()
	{
		var mock = new Mock<IHttpContextAccessor>();
		mock.Setup(x => x.HttpContext).Returns((HttpContext?)null);
		_ctx.Services.AddSingleton<IHttpContextAccessor>(mock.Object);
	}

	private void RegisterHttpContextWithMethod(string method)
	{
		var httpContext = new DefaultHttpContext();
		httpContext.Request.Method = method;
		var mock = new Mock<IHttpContextAccessor>();
		mock.Setup(x => x.HttpContext).Returns(httpContext);
		_ctx.Services.AddSingleton<IHttpContextAccessor>(mock.Object);
	}

	#endregion

	#region BaseWebFormsComponent — ServerInteractive Mode (No HttpContext)

	[Fact]
	public void BaseComponent_InteractiveMode_IsPostBackFalseDuringOnInitialized()
	{
		// No HttpContext = InteractiveServer mode
		RegisterNoHttpContext();

		var cut = _ctx.Render<TestComponent>();

		// During OnInitializedAsync (before base sets _hasInitialized), IsPostBack is false
		cut.Instance.IsPostBackDuringInit.ShouldBeFalse();
	}

	[Fact]
	public void BaseComponent_InteractiveMode_IsPostBackTrueAfterInitialized()
	{
		// No HttpContext = InteractiveServer mode
		RegisterNoHttpContext();

		var cut = _ctx.Render<TestComponent>();

		// After OnInitializedAsync completes, _hasInitialized = true → IsPostBack = true
		cut.Instance.IsPostBackValue.ShouldBeTrue();
	}

	#endregion

	#region BaseWebFormsComponent — SSR Mode (HttpContext Available)

	[Fact]
	public void BaseComponent_SSR_GetRequest_IsPostBackFalse()
	{
		RegisterHttpContextWithMethod("GET");

		var cut = _ctx.Render<TestComponent>();

		cut.Instance.IsPostBackValue.ShouldBeFalse();
	}

	[Fact]
	public void BaseComponent_SSR_PostRequest_IsPostBackTrue()
	{
		RegisterHttpContextWithMethod("POST");

		var cut = _ctx.Render<TestComponent>();

		cut.Instance.IsPostBackValue.ShouldBeTrue();
	}

	#endregion

	#region WebFormsPageBase — ServerInteractive Mode

	[Fact]
	public void Page_InteractiveMode_IsPostBackFalseDuringOnInitialized()
	{
		RegisterNoHttpContext();
		_ctx.Services.AddScoped<IPageService, BlazorWebFormsComponents.PageService>();

		var cut = _ctx.Render<TestPage>();

		// During OnInitialized (before base sets _hasInitialized), IsPostBack is false
		cut.Instance.IsPostBackDuringInit.ShouldBeFalse();
	}

	[Fact]
	public void Page_InteractiveMode_IsPostBackTrueAfterInitialized()
	{
		RegisterNoHttpContext();
		_ctx.Services.AddScoped<IPageService, BlazorWebFormsComponents.PageService>();

		var cut = _ctx.Render<TestPage>();

		// After OnInitialized completes, _hasInitialized = true → IsPostBack = true
		cut.Instance.IsPostBackValue.ShouldBeTrue();
	}

	#endregion

	#region WebFormsPageBase — SSR Mode

	[Fact]
	public void Page_SSR_GetRequest_IsPostBackFalse()
	{
		RegisterHttpContextWithMethod("GET");
		_ctx.Services.AddScoped<IPageService, BlazorWebFormsComponents.PageService>();

		var cut = _ctx.Render<TestPage>();

		cut.Instance.IsPostBackValue.ShouldBeFalse();
	}

	[Fact]
	public void Page_SSR_PostRequest_IsPostBackTrue()
	{
		RegisterHttpContextWithMethod("POST");
		_ctx.Services.AddScoped<IPageService, BlazorWebFormsComponents.PageService>();

		var cut = _ctx.Render<TestPage>();

		cut.Instance.IsPostBackValue.ShouldBeTrue();
	}

	#endregion

	#region Guard Pattern — !IsPostBack Block

	[Fact]
	public void Page_SSR_Get_GuardBlockExecutes()
	{
		// if (!IsPostBack) { BindData(); } — should execute on GET
		RegisterHttpContextWithMethod("GET");
		_ctx.Services.AddScoped<IPageService, BlazorWebFormsComponents.PageService>();

		var cut = _ctx.Render<TestPage>();
		var blockShouldExecute = !cut.Instance.IsPostBackValue;

		blockShouldExecute.ShouldBeTrue();
	}

	[Fact]
	public void Page_SSR_Post_GuardBlockSkips()
	{
		// if (!IsPostBack) { BindData(); } — should skip on POST
		RegisterHttpContextWithMethod("POST");
		_ctx.Services.AddScoped<IPageService, BlazorWebFormsComponents.PageService>();

		var cut = _ctx.Render<TestPage>();
		var blockShouldExecute = !cut.Instance.IsPostBackValue;

		blockShouldExecute.ShouldBeFalse();
	}

	[Fact]
	public void BaseComponent_SSR_Get_GuardBlockExecutes()
	{
		RegisterHttpContextWithMethod("GET");

		var cut = _ctx.Render<TestComponent>();
		var blockShouldExecute = !cut.Instance.IsPostBackValue;

		blockShouldExecute.ShouldBeTrue();
	}

	[Fact]
	public void BaseComponent_SSR_Post_GuardBlockSkips()
	{
		RegisterHttpContextWithMethod("POST");

		var cut = _ctx.Render<TestComponent>();
		var blockShouldExecute = !cut.Instance.IsPostBackValue;

		blockShouldExecute.ShouldBeFalse();
	}

	#endregion
}
