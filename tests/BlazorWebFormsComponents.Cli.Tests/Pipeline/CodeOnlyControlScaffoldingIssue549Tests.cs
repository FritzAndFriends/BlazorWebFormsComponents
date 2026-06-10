namespace BlazorWebFormsComponents.Cli.Tests.Pipeline;

public class CodeOnlyControlScaffoldingIssue549Tests
{
    [Fact(Skip = "Blocked by #549: scaffolded code-only controls should project legacy public properties into [Parameter] members.")]
    public void FullMigration_CodeOnlyControl_PublicPropertiesAreProjectedAsParameters()
    {
    }

    [Fact(Skip = "Blocked by #549: scaffolded code-only controls should preserve simple render-time HTML semantics when no ASCX markup exists.")]
    public void FullMigration_CodeOnlyControl_GeneratesMinimalSemanticMarkupContract()
    {
    }
}
