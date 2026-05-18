using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;
using Xunit;

namespace BlazorWebFormsComponents.Cli.Tests.Transforms.CodeBehind;

public class DisposeReadonlyFieldTransformTests
{
    private readonly DisposeReadonlyFieldTransform _transform = new();
    private static FileMetadata MakeMetadata() => new() { SourceFilePath = "Test.cs", OutputFilePath = "Test.cs", FileType = FileType.CodeFile, OriginalContent = "" };

    [Fact]
    public void Apply_ReadonlyFieldNulledInDispose_RemovesNullAssignment()
    {
        var input = """
            public class MyService
            {
                private readonly ProductContext _db;

                public void Dispose()
                {
                    if (_db != null)
                    {
                        _db.Dispose();
                        _db = null;
                    }
                }
            }
            """;
        var result = _transform.Apply(input, MakeMetadata());
        Assert.DoesNotContain("_db = null", result);
        Assert.Contains("_db.Dispose()", result);
    }

    [Fact]
    public void Apply_NonReadonlyField_LeavesUnchanged()
    {
        var input = """
            public class MyService
            {
                private ProductContext _db;

                public void Dispose()
                {
                    if (_db != null)
                    {
                        _db.Dispose();
                        _db = null;
                    }
                }
            }
            """;
        var result = _transform.Apply(input, MakeMetadata());
        Assert.Contains("_db = null", result);
    }

    [Fact]
    public void Apply_ReadonlyFieldWithNullBang_RemovesNullAssignment()
    {
        var input = """
            public class MyService
            {
                private readonly ProductContext _db;

                public void Dispose()
                {
                    _db = null!;
                }
            }
            """;
        var result = _transform.Apply(input, MakeMetadata());
        Assert.DoesNotContain("_db = null", result);
    }

    [Fact]
    public void Apply_MultipleReadonlyFields_RemovesAllNullAssignments()
    {
        var input = """
            public class MyService
            {
                private readonly ProductContext _db;
                private readonly IHttpContextAccessor _httpContextAccessor;

                public void Dispose()
                {
                    _db = null;
                    _httpContextAccessor = null;
                }
            }
            """;
        var result = _transform.Apply(input, MakeMetadata());
        Assert.DoesNotContain("_db = null", result);
        Assert.DoesNotContain("_httpContextAccessor = null", result);
    }

    [Fact]
    public void Apply_OnlyNullAssignmentsInDispose_RemovesEntireMethod()
    {
        var input = """
            public class MyService
            {
                private readonly ProductContext _db;

                public void DoWork() { }

                public void Dispose()
                {
                    _db = null;
                }
            }
            """;
        var result = _transform.Apply(input, MakeMetadata());
        Assert.DoesNotContain("Dispose", result);
        Assert.Contains("DoWork", result);
    }
}
