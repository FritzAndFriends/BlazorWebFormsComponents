using System;
using Bunit;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using Xunit;

using CheckBoxComponent = BlazorWebFormsComponents.CheckBox;
using TextBoxComponent = BlazorWebFormsComponents.TextBox;
using RadioButtonComponent = BlazorWebFormsComponents.RadioButton;

namespace BlazorWebFormsComponents.Test;

/// <summary>
/// Tests for AutoPostBack behavior — verifies that SSR mode with AutoPostBack=true
/// emits onchange="this.form.submit()" and that Interactive mode does not.
/// </summary>
public class AutoPostBackTests : IDisposable
{
	private readonly BunitContext _ctx;

	public AutoPostBackTests()
	{
		_ctx = new BunitContext();
		_ctx.JSInterop.SetupVoid("bwfc.Page.OnAfterRender");
		_ctx.Services.AddSingleton<LinkGenerator>(new Mock<LinkGenerator>().Object);
		_ctx.Services.AddSingleton<IDataProtectionProvider>(new EphemeralDataProtectionProvider());
		_ctx.Services.AddLogging();
	}

	public void Dispose() => _ctx.Dispose();

	private void RegisterHttpContextWithMethod(string method)
	{
		var httpContext = new DefaultHttpContext();
		httpContext.Request.Method = method;
		var mock = new Mock<IHttpContextAccessor>();
		mock.Setup(x => x.HttpContext).Returns(httpContext);
		_ctx.Services.AddSingleton<IHttpContextAccessor>(mock.Object);
	}

	private void RegisterNoHttpContext()
	{
		var mock = new Mock<IHttpContextAccessor>();
		HttpContext? noContext = null;
		mock.Setup(x => x.HttpContext).Returns(noContext);
		_ctx.Services.AddSingleton<IHttpContextAccessor>(mock.Object);
	}

	#region DropDownList

	[Fact]
	public void DropDownList_SSR_AutoPostBackTrue_EmitsOnchangeScript()
	{
		RegisterHttpContextWithMethod("GET");
		var items = new ListItemCollection
		{
			new ListItem("One", "1"),
			new ListItem("Two", "2")
		};

		var cut = _ctx.Render<DropDownList<object>>(parameters => parameters
			.Add(p => p.StaticItems, items)
			.Add(p => p.AutoPostBack, true));

		cut.Find("select").GetAttribute("onchange").ShouldBe("this.form.submit()");
	}

	[Fact]
	public void DropDownList_SSR_AutoPostBackFalse_NoOnchangeScript()
	{
		RegisterHttpContextWithMethod("GET");
		var items = new ListItemCollection
		{
			new ListItem("One", "1"),
			new ListItem("Two", "2")
		};

		var cut = _ctx.Render<DropDownList<object>>(parameters => parameters
			.Add(p => p.StaticItems, items)
			.Add(p => p.AutoPostBack, false));

		cut.Find("select").GetAttribute("onchange").ShouldBeNull();
	}

	[Fact]
	public void DropDownList_Interactive_AutoPostBackTrue_NoOnchangeScript()
	{
		RegisterNoHttpContext();
		var items = new ListItemCollection
		{
			new ListItem("One", "1"),
			new ListItem("Two", "2")
		};

		var cut = _ctx.Render<DropDownList<object>>(parameters => parameters
			.Add(p => p.StaticItems, items)
			.Add(p => p.AutoPostBack, true));

		cut.Find("select").GetAttribute("onchange").ShouldBeNull();
	}

	#endregion

	#region CheckBox

	[Fact]
	public void CheckBox_SSR_AutoPostBackTrue_EmitsOnchangeScript()
	{
		RegisterHttpContextWithMethod("GET");

		var cut = _ctx.Render<CheckBoxComponent>(parameters => parameters
			.Add(p => p.Text, "Accept")
			.Add(p => p.AutoPostBack, true));

		cut.Find("input[type='checkbox']").GetAttribute("onchange").ShouldBe("this.form.submit()");
	}

	[Fact]
	public void CheckBox_SSR_AutoPostBackFalse_NoOnchangeScript()
	{
		RegisterHttpContextWithMethod("GET");

		var cut = _ctx.Render<CheckBoxComponent>(parameters => parameters
			.Add(p => p.Text, "Accept")
			.Add(p => p.AutoPostBack, false));

		cut.Find("input[type='checkbox']").GetAttribute("onchange").ShouldBeNull();
	}

	#endregion

	#region TextBox

	[Fact]
	public void TextBox_SSR_AutoPostBackTrue_EmitsOnchangeScript()
	{
		RegisterHttpContextWithMethod("GET");

		var cut = _ctx.Render<TextBoxComponent>(parameters => parameters
			.Add(p => p.Text, "hello")
			.Add(p => p.AutoPostBack, true));

		cut.Find("input").GetAttribute("onchange").ShouldBe("this.form.submit()");
	}

	#endregion

	#region RadioButton

	[Fact]
	public void RadioButton_SSR_AutoPostBackTrue_EmitsOnchangeScript()
	{
		RegisterHttpContextWithMethod("GET");

		var cut = _ctx.Render<RadioButtonComponent>(parameters => parameters
			.Add(p => p.Text, "Option A")
			.Add(p => p.AutoPostBack, true));

		cut.Find("input[type='radio']").GetAttribute("onchange").ShouldBe("this.form.submit()");
	}

	[Fact]
	public void RadioButton_Interactive_AutoPostBackTrue_NoOnchangeScript()
	{
		RegisterNoHttpContext();

		var cut = _ctx.Render<RadioButtonComponent>(parameters => parameters
			.Add(p => p.Text, "Option A")
			.Add(p => p.AutoPostBack, true));

		cut.Find("input[type='radio']").GetAttribute("onchange").ShouldBeNull();
	}

	#endregion

	#region ListBox

	[Fact]
	public void ListBox_SSR_AutoPostBackTrue_EmitsOnchangeScript()
	{
		RegisterHttpContextWithMethod("GET");
		var items = new ListItemCollection
		{
			new ListItem("One", "1"),
			new ListItem("Two", "2")
		};

		var cut = _ctx.Render<ListBox<object>>(parameters => parameters
			.Add(p => p.StaticItems, items)
			.Add(p => p.AutoPostBack, true));

		cut.Find("select").GetAttribute("onchange").ShouldBe("this.form.submit()");
	}

	#endregion

	#region CheckBoxList

	[Fact]
	public void CheckBoxList_SSR_AutoPostBackTrue_EmitsOnchangeScript()
	{
		RegisterHttpContextWithMethod("GET");
		var items = new ListItemCollection
		{
			new ListItem("One", "1"),
			new ListItem("Two", "2")
		};

		var cut = _ctx.Render<CheckBoxList<object>>(parameters => parameters
			.Add(p => p.StaticItems, items)
			.Add(p => p.AutoPostBack, true));

		var checkboxes = cut.FindAll("input[type='checkbox']");
		checkboxes.Count.ShouldBeGreaterThan(0);
		foreach (var cb in checkboxes)
		{
			cb.GetAttribute("onchange").ShouldBe("this.form.submit()");
		}
	}

	#endregion

	#region RadioButtonList

	[Fact]
	public void RadioButtonList_SSR_AutoPostBackTrue_EmitsOnchangeScript()
	{
		RegisterHttpContextWithMethod("GET");
		var items = new ListItemCollection
		{
			new ListItem("One", "1"),
			new ListItem("Two", "2")
		};

		var cut = _ctx.Render<RadioButtonList<object>>(parameters => parameters
			.Add(p => p.StaticItems, items)
			.Add(p => p.AutoPostBack, true));

		var radios = cut.FindAll("input[type='radio']");
		radios.Count.ShouldBeGreaterThan(0);
		foreach (var radio in radios)
		{
			radio.GetAttribute("onchange").ShouldBe("this.form.submit()");
		}
	}

	#endregion
}
