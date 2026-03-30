using System;
using System.Web.Routing;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Test;

/// <summary>
/// Tests for RouteTable/RouteCollection stubs.
/// These are no-op shims so migrated RouteConfig.cs code compiles.
/// Tests verify: (1) it compiles, (2) it doesn't throw, (3) method signatures match Web Forms API.
/// Tests are written from spec — implementation may not exist yet.
/// </summary>
public class RouteConfigTests
{
	#region RouteTable

	[Fact]
	public void RouteTable_Routes_IsNotNull()
	{
		// Act
		var routes = RouteTable.Routes;

		// Assert
		routes.ShouldNotBeNull();
	}

	[Fact]
	public void RouteTable_Routes_ReturnsSameInstance()
	{
		// Act — multiple accesses should return the same collection
		var first = RouteTable.Routes;
		var second = RouteTable.Routes;

		// Assert
		first.ShouldBeSameAs(second);
	}

	#endregion

	#region RouteCollection.MapPageRoute

	[Fact]
	public void RouteCollection_MapPageRoute_DoesNotThrow()
	{
		// Arrange
		var routes = RouteTable.Routes;

		// Act & Assert — 3-arg overload: routeName, routeUrl, physicalFile
		Should.NotThrow(() =>
			routes.MapPageRoute("ProductRoute", "Product/{productId}", "~/Product.aspx"));
	}

	[Fact]
	public void RouteCollection_MapPageRoute_MultipleRoutes_DoesNotThrow()
	{
		// Arrange
		var routes = RouteTable.Routes;

		// Act & Assert — typical RouteConfig.RegisterRoutes pattern
		Should.NotThrow(() =>
		{
			routes.MapPageRoute("Home", "", "~/Default.aspx");
			routes.MapPageRoute("About", "about", "~/About.aspx");
			routes.MapPageRoute("Contact", "contact", "~/Contact.aspx");
			routes.MapPageRoute("ProductDetail", "product/{id}", "~/ProductDetail.aspx");
		});
	}

	#endregion

	#region RouteCollection.Ignore

	[Fact]
	public void RouteCollection_Ignore_DoesNotThrow()
	{
		// Arrange
		var routes = RouteTable.Routes;

		// Act & Assert — typical ignore pattern for resource files
		Should.NotThrow(() => routes.Ignore("{resource}.axd/{*pathInfo}"));
	}

	[Fact]
	public void RouteCollection_Ignore_MultiplePatterns_DoesNotThrow()
	{
		// Arrange
		var routes = RouteTable.Routes;

		// Act & Assert
		Should.NotThrow(() =>
		{
			routes.Ignore("{resource}.axd/{*pathInfo}");
			routes.Ignore("favicon.ico");
			routes.Ignore("Content/{*pathInfo}");
		});
	}

	#endregion

	#region Typical Registration Pattern

	[Fact]
	public void TypicalRouteConfig_RegisterRoutes_DoesNotThrow()
	{
		// This mimics the exact pattern from a Web Forms RouteConfig.cs file
		Should.NotThrow(() =>
		{
			var routes = RouteTable.Routes;
			routes.Ignore("{resource}.axd/{*pathInfo}");
			routes.MapPageRoute("Default", "", "~/Default.aspx");
			routes.MapPageRoute("Products", "products", "~/Products.aspx");
			routes.MapPageRoute("ProductDetail", "product/{id}", "~/ProductDetail.aspx");
		});
	}

	#endregion
}
