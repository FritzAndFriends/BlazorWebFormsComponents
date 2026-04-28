using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.SemanticPatterns;

namespace BlazorWebFormsComponents.Cli.Tests;

public class SemanticPatternConcreteTests
{
    [Fact]
    public void DefaultSemanticPatterns_ExposeFirstFourCatalogEntriesInCanonicalOrder()
    {
        var patterns = TestHelpers.CreateDefaultSemanticPatterns();

        Assert.Equal(
            [
                "pattern-query-details",
                "pattern-master-content-contracts",
                "pattern-action-pages",
                "pattern-account-pages"
            ],
            patterns.Select(static pattern => pattern.Id).ToArray());
    }

    [Fact]
    public void AccountPagesPattern_NormalizesValidatorHeavyLoginMarkup()
    {
        var result = ApplyPattern(
            new AccountPagesSemanticPattern(),
            """
            @page "/Account/Login"
            <PageTitle>Login</PageTitle>
            <Site>
                <Content ContentPlaceHolderID="MainContent">
                    <h1>Login</h1>
                    <div class="form-horizontal">
                        <Label AssociatedControlID="Email" CssClass="col-md-2 control-label">Email</Label>
                        <TextBox ID="Email" CssClass="form-control" TextMode="Email" />
                        <RequiredFieldValidator ControlToValidate="Email" ErrorMessage="Email is required." />
                        <Label AssociatedControlID="Password" CssClass="col-md-2 control-label">Password</Label>
                        <TextBox ID="Password" CssClass="form-control" TextMode="Password" />
                        <RequiredFieldValidator ControlToValidate="Password" ErrorMessage="Password is required." />
                        <CheckBox ID="RememberMe" />
                        <Label AssociatedControlID="RememberMe">Remember me?</Label>
                        <Button Text="Log in" CssClass="btn btn-default" />
                        <HyperLink>Register as a new user</HyperLink>
                    </div>
                </Content>
            </Site>
            """,
            "D:\\input\\Account\\Login.aspx",
            FileType.Page);

        Assert.Contains("TODO(bwfc-identity)", result.Markup);
        Assert.Contains("<form method=\"get\" action=\"/Account/PerformLogin\" class=\"form-horizontal\">", result.Markup);
        Assert.Contains("type=\"email\"", result.Markup);
        Assert.Contains("type=\"password\"", result.Markup);
        Assert.Contains("type=\"checkbox\"", result.Markup);
        Assert.Contains("Register as a new user", result.Markup);
        Assert.Contains("SupplyParameterFromQuery(Name = \"returnUrl\")", result.Markup);
        Assert.DoesNotContain("<RequiredFieldValidator", result.Markup);
    }

    [Fact]
    public void MasterContentContractsPattern_AddsChildComponentsToMasterShell()
    {
        var result = ApplyPattern(
            new MasterContentContractsSemanticPattern(),
            """
            <MasterPage>
            <ChildContent>
                <div class="container">
                    <ContentPlaceHolder ID="HeadContent" />
                    <ContentPlaceHolder ID="MainContent" />
                    @ChildContent
                </div>
            </ChildContent>
            </MasterPage>

            @code {
                [Parameter]
                public RenderFragment? ChildContent { get; set; }
            }
            """,
            "D:\\input\\Site.master",
            FileType.Master);

        Assert.Contains("@ChildComponents", result.Markup);
        Assert.Contains("public RenderFragment? ChildComponents { get; set; }", result.Markup);
        Assert.Contains("public RenderFragment? ChildContent { get; set; }", result.Markup);
    }

    [Fact]
    public void MasterContentContractsPattern_GroupsNamedSectionsUnderChildComponents()
    {
        var result = ApplyPattern(
            new MasterContentContractsSemanticPattern(),
            """
            @page "/Default"
            <Site>
                <Content ContentPlaceHolderID="HeadContent">
                    <title>Home</title>
                </Content>
                <Content ContentPlaceHolderID="MainContent">
                    <h1>Hello</h1>
                </Content>
            </Site>
            """,
            "D:\\input\\Default.aspx",
            FileType.Page);

        Assert.Contains("<ChildComponents>", result.Markup);
        Assert.Contains("<Content ContentPlaceHolderID=\"HeadContent\">", result.Markup);
        Assert.Contains("<Content ContentPlaceHolderID=\"MainContent\">", result.Markup);
        Assert.DoesNotContain("<ChildContent>", result.Markup);
    }

    [Fact]
    public void Catalog_AppliesMasterAndAccountPatternsTogether()
    {
        var catalog = new SemanticPatternCatalog(
        [
            new MasterContentContractsSemanticPattern(),
            new AccountPagesSemanticPattern()
        ]);

        var migrationContext = new MigrationContext
        {
            SourcePath = "input",
            OutputPath = "output",
            Options = new MigrationOptions()
        };

        var sourceFile = new SourceFile
        {
            MarkupPath = "D:\\input\\Account\\Register.aspx",
            OutputPath = "D:\\output\\Account\\Register.razor",
            FileType = FileType.Page
        };

        var metadata = new FileMetadata
        {
            SourceFilePath = sourceFile.MarkupPath,
            OutputFilePath = sourceFile.OutputPath,
            FileType = sourceFile.FileType,
            OriginalContent = "original"
        };

        var report = new MigrationReport();
        var result = catalog.Apply(
            migrationContext,
            sourceFile,
            metadata,
            """
            @page "/Account/Register"
            <PageTitle>Register</PageTitle>
            <Site>
                <Content ContentPlaceHolderID="MainContent">
                    <ValidationSummary CssClass="text-danger" />
                    <Label AssociatedControlID="Email">Email</Label>
                    <TextBox ID="Email" CssClass="form-control" TextMode="Email" />
                    <RequiredFieldValidator ControlToValidate="Email" ErrorMessage="Required" />
                    <Label AssociatedControlID="Password">Password</Label>
                    <TextBox ID="Password" CssClass="form-control" TextMode="Password" />
                    <RequiredFieldValidator ControlToValidate="Password" ErrorMessage="Required" />
                    <Label AssociatedControlID="ConfirmPassword">Confirm password</Label>
                    <TextBox ID="ConfirmPassword" CssClass="form-control" TextMode="Password" />
                    <CompareValidator ControlToValidate="ConfirmPassword" ControlToCompare="Password" ErrorMessage="Mismatch" />
                    <Button Text="Register" CssClass="btn btn-default" />
                </Content>
            </Site>
            """,
            codeBehind: null,
            report);

        Assert.Equal(
            ["pattern-master-content-contracts", "pattern-account-pages"],
            result.AppliedPatterns.Select(p => p.PatternId).ToArray());
        Assert.Contains("<ChildComponents>", result.Markup);
        Assert.Contains("TODO(bwfc-identity)", result.Markup);
        Assert.Contains("method=\"get\"", result.Markup);
        Assert.Contains("action=\"/Account/PerformRegister\"", result.Markup);
        Assert.DoesNotContain("<ValidationSummary", result.Markup);
        Assert.Equal(2, report.SemanticPatternsApplied);
    }

    private static SemanticPatternResult ApplyPattern(
        ISemanticPattern pattern,
        string markup,
        string markupPath,
        FileType fileType)
    {
        var context = new SemanticPatternContext
        {
            MigrationContext = new MigrationContext
            {
                SourcePath = "input",
                OutputPath = "output",
                Options = new MigrationOptions()
            },
            SourceFile = new SourceFile
            {
                MarkupPath = markupPath,
                OutputPath = Path.ChangeExtension(markupPath, ".razor"),
                FileType = fileType
            },
            Metadata = new FileMetadata
            {
                SourceFilePath = markupPath,
                OutputFilePath = Path.ChangeExtension(markupPath, ".razor"),
                FileType = fileType,
                OriginalContent = markup
            },
            Report = new MigrationReport(),
            Markup = markup,
            CodeBehind = null
        };

        var match = pattern.Match(context);
        Assert.True(match.IsMatch);
        return pattern.Apply(context);
    }
}
