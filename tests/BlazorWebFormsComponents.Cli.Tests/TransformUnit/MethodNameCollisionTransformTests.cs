using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for MethodNameCollisionTransform — detects and resolves method name
/// collisions with class names (CS0542 error).
/// </summary>
public class MethodNameCollisionTransformTests
{
    private readonly MethodNameCollisionTransform _transform = new();

    private static FileMetadata TestMetadata(string content, string? markupContent = null) => new()
    {
        SourceFilePath = "Forgot.aspx.cs",
        OutputFilePath = "Forgot.razor.cs",
        FileType = FileType.Page,
        OriginalContent = content,
        MarkupContent = markupContent
    };

    [Fact]
    public void MethodNamedSameAsClass_GetsRenamed()
    {
        var input = @"
namespace MyApp
{
    public partial class Forgot
    {
        protected void Forgot()
        {
            // Reset password logic
        }
    }
}";

        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("protected void OnForgot()", result);
        Assert.DoesNotContain("protected void Forgot()", result);
    }

    [Fact]
    public void MarkupContent_InMetadata_GetsUpdated()
    {
        var codeBehind = @"
namespace MyApp
{
    public partial class Forgot
    {
        protected void Forgot()
        {
            // Logic
        }
    }
}";

        var markup = @"<Button Text=""Submit"" OnClick=""@Forgot"" />";
        var metadata = TestMetadata(codeBehind, markup);

        _transform.Apply(codeBehind, metadata);

        Assert.Contains(@"OnClick=""@OnForgot""", metadata.MarkupContent);
        Assert.DoesNotContain(@"OnClick=""@Forgot""", metadata.MarkupContent);
    }

    [Fact]
    public void MethodsWithDifferentNames_AreNotAffected()
    {
        var input = @"
namespace MyApp
{
    public partial class Forgot
    {
        protected void Submit()
        {
            // Submit logic
        }
    }
}";

        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Equal(input, result); // Unchanged
        Assert.Contains("protected void Submit()", result);
    }

    [Fact]
    public void NoClassName_ContentUnchanged()
    {
        var input = @"
namespace MyApp
{
    // No partial class declaration
    public class SomeOtherClass
    {
        protected void Forgot()
        {
        }
    }
}";

        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Equal(input, result); // Unchanged
    }

    [Fact]
    public void AsyncMethod_Collision_AlsoHandled()
    {
        var input = @"
namespace MyApp
{
    public partial class Forgot
    {
        protected async Task Forgot()
        {
            await Task.Delay(100);
        }
    }
}";

        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("protected async Task OnForgot()", result);
        Assert.DoesNotContain("protected async Task Forgot()", result);
    }

    [Fact]
    public void Constructor_IsNotRenamed()
    {
        var input = @"
namespace MyApp
{
    public partial class Forgot
    {
        public Forgot()
        {
            // Constructor
        }

        protected void Forgot()
        {
            // Method - this should be renamed
        }
    }
}";

        var result = _transform.Apply(input, TestMetadata(input));

        // Constructor should remain unchanged
        Assert.Contains("public Forgot()", result);
        
        // Method should be renamed
        Assert.Contains("protected void OnForgot()", result);
        Assert.DoesNotContain("protected void Forgot()", result);
    }

    [Fact]
    public void TaskWithGenericReturnType_HandlesCollision()
    {
        var input = @"
namespace MyApp
{
    public partial class Forgot
    {
        protected async Task<bool> Forgot()
        {
            return await Task.FromResult(true);
        }
    }
}";

        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("protected async Task<bool> OnForgot()", result);
        Assert.DoesNotContain("protected async Task<bool> Forgot()", result);
    }

    [Fact]
    public void OrderIs215()
    {
        Assert.Equal(215, _transform.Order);
    }

    [Fact]
    public void MethodCalls_WithinCodeBehind_AreUpdated()
    {
        var input = @"
namespace MyApp
{
    public partial class Forgot
    {
        protected void Forgot()
        {
            // Reset password logic
        }

        private void OnPageLoad()
        {
            this.Forgot();
        }
    }
}";

        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("this.OnForgot()", result);
        Assert.DoesNotContain("this.Forgot()", result);
    }
}
