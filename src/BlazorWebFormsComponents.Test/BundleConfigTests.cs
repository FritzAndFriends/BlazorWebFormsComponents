using System;
using System.Web.Optimization;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Test;

/// <summary>
/// Tests for BundleTable/BundleCollection/ScriptBundle/StyleBundle stubs.
/// These are no-op shims that exist solely so migrated Global.asax code compiles.
/// Tests verify: (1) it compiles, (2) it doesn't throw, (3) fluent API chains work.
/// Tests are written from spec — implementation may not exist yet.
/// </summary>
public class BundleConfigTests
{
	#region BundleTable

	[Fact]
	public void BundleTable_Bundles_IsNotNull()
	{
		// Act
		var bundles = BundleTable.Bundles;

		// Assert
		bundles.ShouldNotBeNull();
	}

	[Fact]
	public void BundleTable_Bundles_ReturnsSameInstance()
	{
		// Act — multiple accesses should return the same collection
		var first = BundleTable.Bundles;
		var second = BundleTable.Bundles;

		// Assert
		first.ShouldBeSameAs(second);
	}

	#endregion

	#region BundleCollection

	[Fact]
	public void BundleCollection_Add_DoesNotThrow()
	{
		// Arrange
		var bundles = BundleTable.Bundles;
		var bundle = new ScriptBundle("~/bundles/jquery");

		// Act & Assert — should not throw
		Should.NotThrow(() => bundles.Add(bundle));
	}

	[Fact]
	public void BundleCollection_Add_MultipleDoesNotThrow()
	{
		// Arrange
		var bundles = BundleTable.Bundles;

		// Act & Assert — adding multiple bundles should not throw
		Should.NotThrow(() =>
		{
			bundles.Add(new ScriptBundle("~/bundles/jquery"));
			bundles.Add(new ScriptBundle("~/bundles/modernizr"));
			bundles.Add(new StyleBundle("~/Content/css"));
		});
	}

	#endregion

	#region ScriptBundle

	[Fact]
	public void ScriptBundle_CanBeCreatedWithVirtualPath()
	{
		// Act
		var bundle = new ScriptBundle("~/bundles/jquery");

		// Assert
		bundle.ShouldNotBeNull();
	}

	[Fact]
	public void ScriptBundle_Include_ReturnsSelf()
	{
		// Arrange
		var bundle = new ScriptBundle("~/bundles/jquery");

		// Act — fluent Include should return the same bundle for chaining
		var result = bundle.Include("~/Scripts/jquery-{version}.js");

		// Assert
		result.ShouldNotBeNull();
		result.ShouldBeSameAs(bundle);
	}

	[Fact]
	public void ScriptBundle_Include_MultipleFiles_ReturnsSelf()
	{
		// Arrange
		var bundle = new ScriptBundle("~/bundles/jqueryval");

		// Act — include with multiple paths (typical Web Forms pattern)
		var result = bundle.Include(
			"~/Scripts/jquery.validate.js",
			"~/Scripts/jquery.validate.unobtrusive.js");

		// Assert
		result.ShouldNotBeNull();
		result.ShouldBeSameAs(bundle);
	}

	[Fact]
	public void ScriptBundle_FluentChain_DoesNotThrow()
	{
		// Full fluent chain as typically seen in BundleConfig.RegisterBundles
		Should.NotThrow(() =>
		{
			BundleTable.Bundles.Add(
				new ScriptBundle("~/bundles/jquery")
					.Include("~/Scripts/jquery-{version}.js"));
		});
	}

	#endregion

	#region StyleBundle

	[Fact]
	public void StyleBundle_CanBeCreatedWithVirtualPath()
	{
		// Act
		var bundle = new StyleBundle("~/Content/css");

		// Assert
		bundle.ShouldNotBeNull();
	}

	[Fact]
	public void StyleBundle_Include_ReturnsSelf()
	{
		// Arrange
		var bundle = new StyleBundle("~/Content/css");

		// Act
		var result = bundle.Include("~/Content/bootstrap.css");

		// Assert
		result.ShouldNotBeNull();
		result.ShouldBeSameAs(bundle);
	}

	[Fact]
	public void StyleBundle_Include_MultipleFiles_ReturnsSelf()
	{
		// Arrange
		var bundle = new StyleBundle("~/Content/css");

		// Act
		var result = bundle.Include(
			"~/Content/bootstrap.css",
			"~/Content/site.css");

		// Assert
		result.ShouldNotBeNull();
		result.ShouldBeSameAs(bundle);
	}

	[Fact]
	public void StyleBundle_FluentChain_DoesNotThrow()
	{
		// Full fluent chain pattern
		Should.NotThrow(() =>
		{
			BundleTable.Bundles.Add(
				new StyleBundle("~/Content/css")
					.Include("~/Content/bootstrap.css",
						"~/Content/site.css"));
		});
	}

	#endregion

	#region Bundle Base

	[Fact]
	public void Bundle_CanBeCreatedWithVirtualPath()
	{
		// ScriptBundle and StyleBundle both inherit from Bundle
		var script = new ScriptBundle("~/bundles/app");
		var style = new StyleBundle("~/Content/themes");

		script.ShouldNotBeNull();
		style.ShouldNotBeNull();
	}

	#endregion
}
