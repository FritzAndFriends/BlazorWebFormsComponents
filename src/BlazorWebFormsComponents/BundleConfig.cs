namespace System.Web.Optimization;

/// <summary>
/// No-op shim for ASP.NET Web Forms <c>BundleCollection</c>.
/// Allows <c>App_Start/BundleConfig.cs</c> files to compile without modification.
/// Bundling is not needed in Blazor — use standard CSS/JS isolation or a build tool.
/// </summary>
public class BundleCollection
{
	public void Add(Bundle bundle) { }
}

/// <summary>
/// No-op shim for ASP.NET Web Forms <c>Bundle</c>.
/// </summary>
public class Bundle
{
	public Bundle(string virtualPath) { }
	public Bundle Include(params string[] virtualPaths) => this;
}

/// <summary>
/// No-op shim for ASP.NET Web Forms <c>ScriptBundle</c>.
/// </summary>
public class ScriptBundle : Bundle
{
	public ScriptBundle(string virtualPath) : base(virtualPath) { }
}

/// <summary>
/// No-op shim for ASP.NET Web Forms <c>StyleBundle</c>.
/// </summary>
public class StyleBundle : Bundle
{
	public StyleBundle(string virtualPath) : base(virtualPath) { }
}

/// <summary>
/// No-op shim for ASP.NET Web Forms <c>BundleTable</c>.
/// <c>BundleTable.Bundles</c> returns a no-op <see cref="BundleCollection"/>.
/// </summary>
public static class BundleTable
{
	public static BundleCollection Bundles { get; } = new BundleCollection();
}
