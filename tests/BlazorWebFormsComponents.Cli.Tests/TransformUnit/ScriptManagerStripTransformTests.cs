using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

public class ScriptManagerStripTransformTests
{
    private readonly ScriptManagerStripTransform _transform = new();

    private static FileMetadata MasterMetadata => new()
    {
        SourceFilePath = "Site.master",
        OutputFilePath = "Site.razor",
        FileType = FileType.Master,
        OriginalContent = string.Empty
    };

    private static FileMetadata PageMetadata => new()
    {
        SourceFilePath = "Default.aspx",
        OutputFilePath = "Default.razor",
        FileType = FileType.Page,
        OriginalContent = string.Empty
    };

    [Fact]
    public void StripsFullScriptManagerBlockAndLeavesComment()
    {
        var input = """
            <MasterPage>
            <Head>
            <asp:ScriptManager runat="server">
                <Scripts>
                    <asp:ScriptReference Name="jquery" />
                </Scripts>
            </asp:ScriptManager>
            </Head>
            </MasterPage>
            """;

        var result = _transform.Apply(input, MasterMetadata);

        Assert.Contains("@* Framework scripts are managed by Blazor — no ScriptManager needed. *@", result);
        Assert.DoesNotContain("<asp:ScriptManager", result);
        Assert.DoesNotContain("<asp:ScriptReference", result);
    }

    [Fact]
    public void StripsBundleReferenceTag()
    {
        var input = """
            <MasterPage>
            <Head>
            <webopt:bundlereference runat="server" path="~/Content/css" />
            </Head>
            </MasterPage>
            """;

        var result = _transform.Apply(input, MasterMetadata);

        Assert.DoesNotContain("webopt:bundlereference", result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void StripsScriptsRenderPlaceholderBlock()
    {
        var input = """
            <MasterPage>
            <Head>
            <asp:PlaceHolder runat="server">
                <%: Scripts.Render("~/bundles/modernizr") %>
            </asp:PlaceHolder>
            </Head>
            </MasterPage>
            """;

        var result = _transform.Apply(input, MasterMetadata);

        Assert.DoesNotContain("Scripts.Render", result);
        Assert.DoesNotContain("<asp:PlaceHolder", result);
    }

    [Fact]
    public void SkipsNonMasterFiles()
    {
        var input = "<asp:ScriptManager runat=\"server\"></asp:ScriptManager>";

        var result = _transform.Apply(input, PageMetadata);

        Assert.Equal(input, result);
    }

    [Fact]
    public void PreservesContentAfterScriptManager()
    {
        var input = """
            <MasterPage>
            <Head>
            <asp:ScriptManager runat="server"></asp:ScriptManager>
            </Head>
            <ChildContent>
                <div class="shell">Main content</div>
            </ChildContent>
            </MasterPage>
            """;

        var result = _transform.Apply(input, MasterMetadata);

        Assert.Contains("<div class=\"shell\">Main content</div>", result);
        Assert.DoesNotContain("\n\n\n", result);
    }
}
