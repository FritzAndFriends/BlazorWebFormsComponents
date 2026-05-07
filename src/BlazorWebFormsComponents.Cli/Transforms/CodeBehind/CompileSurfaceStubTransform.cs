using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Replaces high-risk page code-behind with build-safe stubs while preserving the transformed original in migration artifacts.
/// </summary>
public class CompileSurfaceStubTransform : ICodeBehindTransform
{
    private readonly PageQuarantineDetector _pageQuarantineDetector;

    public CompileSurfaceStubTransform()
        : this(new PageQuarantineDetector())
    {
    }

    public CompileSurfaceStubTransform(PageQuarantineDetector pageQuarantineDetector)
    {
        _pageQuarantineDetector = pageQuarantineDetector;
    }

    public string Name => "CompileSurfaceStub";
    public int Order => 850;

    public string Apply(string content, FileMetadata metadata)
    {
        if (metadata.FileType != FileType.Page)
        {
            return content;
        }

        var decision = _pageQuarantineDetector.AnalyzeEarly(metadata, content);
        if (!decision.ShouldQuarantine)
        {
            return content;
        }

        metadata.QuarantineDecision = decision;
        metadata.CompileSurfaceStubReason = decision.Reason;
        metadata.CompileSurfaceOriginalCodeBehind = content;
        metadata.MarkupContent = decision.StubMarkup;

        return decision.StubCodeBehind;
    }
}
