using BlazorWebFormsComponents.Cli.Analysis;
using BlazorWebFormsComponents.Cli.Io;
using BlazorWebFormsComponents.Cli.Scaffolding;

namespace BlazorWebFormsComponents.Cli.Tests.Pipeline;

public class CodeOnlyControlScaffoldingIssue549Tests : IDisposable
{
    private readonly string _tempDir = Path.Combine(
        AppContext.BaseDirectory, "TestArtifacts", $"issue549-{Guid.NewGuid():N}");

    public CodeOnlyControlScaffoldingIssue549Tests()
    {
        Directory.CreateDirectory(_tempDir);
    }

    [Fact]
    public async Task FullMigration_CodeOnlyControl_PublicPropertiesAreProjectedAsParameters()
    {
        // Arrange: a code-only WebControl with public settable properties
        var controlDescriptor = new CodeOnlyServerControlDescriptor
        {
            ClassName = "StarRating",
            Namespace = "DepartmentPortal.Controls",
            BaseType = "WebControl",
            SourceFilePath = Path.Combine("Code", "Controls", "StarRating.cs"),
            TagPrefixes = ["local"],
            Properties =
            [
                new CodeOnlyPropertyDescriptor("MaxStars", "int"),
                new CodeOnlyPropertyDescriptor("CurrentRating", "double"),
                new CodeOnlyPropertyDescriptor("AllowHalf", "bool")
            ],
            Events = []
        };

        var scaffolder = new CodeOnlyControlScaffolder();
        var writer = new OutputWriter();

        // Act
        await scaffolder.EmitAsync(_tempDir, "AfterDepartmentPortal", [controlDescriptor], writer);

        var codeBehindPath = Path.Combine(_tempDir, "Generated", "CodeOnlyControls", "StarRating.razor.cs");
        Assert.True(File.Exists(codeBehindPath));
        var codeBehind = await File.ReadAllTextAsync(codeBehindPath);

        // Assert: properties appear as [Parameter] members
        Assert.Contains("[Parameter] public int MaxStars { get; set; }", codeBehind);
        Assert.Contains("[Parameter] public double CurrentRating { get; set; }", codeBehind);
        Assert.Contains("[Parameter] public bool AllowHalf { get; set; }", codeBehind);
    }

    [Fact]
    public async Task FullMigration_CodeOnlyControl_GeneratesMinimalSemanticMarkupContract()
    {
        // Arrange: code-only WebControl (mapped to BaseStyledComponent)
        var controlDescriptor = new CodeOnlyServerControlDescriptor
        {
            ClassName = "EmployeeCard",
            Namespace = "DepartmentPortal.Controls",
            BaseType = "CompositeControl",
            SourceFilePath = Path.Combine("Code", "Controls", "EmployeeCard.cs"),
            TagPrefixes = ["local"],
            Properties =
            [
                new CodeOnlyPropertyDescriptor("EmployeeId", "int")
            ],
            Events =
            [
                new CodeOnlyEventDescriptor("EmployeeSelected", "EventHandler")
            ]
        };

        var scaffolder = new CodeOnlyControlScaffolder();
        var writer = new OutputWriter();

        // Act
        await scaffolder.EmitAsync(_tempDir, "AfterDepartmentPortal", [controlDescriptor], writer);

        var razorPath = Path.Combine(_tempDir, "Generated", "CodeOnlyControls", "EmployeeCard.razor");
        var codeBehindPath = Path.Combine(_tempDir, "Generated", "CodeOnlyControls", "EmployeeCard.razor.cs");

        Assert.True(File.Exists(razorPath), $"Expected .razor file at {razorPath}");
        Assert.True(File.Exists(codeBehindPath), $"Expected .razor.cs file at {codeBehindPath}");

        var markup = await File.ReadAllTextAsync(razorPath);
        var codeBehind = await File.ReadAllTextAsync(codeBehindPath);

        // Markup must reference ChildContent to enable child slot
        Assert.Contains("@ChildContent", markup);

        // Base class: CompositeControl → BaseWebFormsComponent
        Assert.Contains(": BaseWebFormsComponent", codeBehind);

        // Event preserved as EventCallback
        Assert.Contains("[Parameter] public EventCallback EmployeeSelected { get; set; }", codeBehind);

        // Property preserved
        Assert.Contains("[Parameter] public int EmployeeId { get; set; }", codeBehind);
    }

    [Fact]
    public void BaseClassMapping_WebControl_MapsToBaseStyledComponent()
    {
        Assert.Equal("BaseStyledComponent", CodeOnlyControlScaffolder.MapBwfcBaseClass("WebControl"));
    }

    [Fact]
    public void BaseClassMapping_CompositeControl_MapsToBaseWebFormsComponent()
    {
        Assert.Equal("BaseWebFormsComponent", CodeOnlyControlScaffolder.MapBwfcBaseClass("CompositeControl"));
    }

    [Fact]
    public void BaseClassMapping_Control_MapsToBaseWebFormsComponent()
    {
        Assert.Equal("BaseWebFormsComponent", CodeOnlyControlScaffolder.MapBwfcBaseClass("Control"));
    }

    [Fact]
    public void EventCallbackMapping_PlainEventHandler_MapsToEventCallback()
    {
        Assert.Equal("EventCallback", CodeOnlyControlScaffolder.MapEventCallback("EventHandler"));
    }

    [Fact]
    public void EventCallbackMapping_GenericEventHandler_MapsToEventCallbackT()
    {
        Assert.Equal("EventCallback<CommandEventArgs>",
            CodeOnlyControlScaffolder.MapEventCallback("EventHandler<CommandEventArgs>"));
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            try { Directory.Delete(_tempDir, recursive: true); }
            catch { /* best effort */ }
        }
    }
}
