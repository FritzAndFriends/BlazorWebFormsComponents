using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for IdentityUsingTransform — adds BWFC Identity type aliases or namespace
/// using to code-behind files that reference BWFC Identity shim types.
/// </summary>
public class IdentityUsingTransformTests
{
    private readonly IdentityUsingTransform _transform = new();

    private static FileMetadata TestMetadata(string content) => new()
    {
        SourceFilePath = "Default.aspx.cs",
        OutputFilePath = "Default.razor.cs",
        FileType = FileType.Page,
        OriginalContent = content
    };

    [Fact]
    public void File_WithIdentityResult_GetsTypeAlias()
    {
        var input = @"using System;

namespace MyApp
{
    public class Login
    {
        public void SignIn()
        {
            IdentityResult result = null;
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("using IdentityResult = BlazorWebFormsComponents.Identity.IdentityResult;", result);
        // Should NOT add namespace using (IdentityResult is a conflicting type)
        Assert.DoesNotContain("using BlazorWebFormsComponents.Identity;", result);
    }

    [Fact]
    public void File_WithApplicationUserManager_GetsNamespaceUsing()
    {
        var input = @"using System;

namespace MyApp
{
    public class Login
    {
        private ApplicationUserManager userManager;
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        // ApplicationUserManager is BWFC-only, so namespace using is safe
        Assert.Contains("using BlazorWebFormsComponents.Identity;", result);
    }

    [Fact]
    public void File_WithApplicationSignInManager_GetsNamespaceUsing()
    {
        var input = @"using System;

namespace MyApp
{
    public class Login
    {
        private ApplicationSignInManager signInManager;
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("using BlazorWebFormsComponents.Identity;", result);
    }

    [Fact]
    public void File_WithSignInStatus_GetsNamespaceUsing()
    {
        var input = @"using System;

namespace MyApp
{
    public class Login
    {
        public void CheckStatus()
        {
            var status = SignInStatus.Success;
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("using BlazorWebFormsComponents.Identity;", result);
    }

    [Fact]
    public void File_WithIdentityDbContext_GetsNamespaceUsing()
    {
        var input = @"using System;

namespace MyApp
{
    public class MyContext : IdentityDbContext
    {
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        // IdentityDbContext is generic — can't alias open generics, so use namespace using
        Assert.Contains("using BlazorWebFormsComponents.Identity;", result);
        Assert.DoesNotContain("using IdentityDbContext =", result);
    }

    [Fact]
    public void File_WithDefaultAuthenticationTypes_GetsNamespaceUsing()
    {
        var input = @"using System;

namespace MyApp
{
    public class Login
    {
        public void SignIn()
        {
            var authType = DefaultAuthenticationTypes.ApplicationCookie;
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("using BlazorWebFormsComponents.Identity;", result);
    }

    [Fact]
    public void File_WithIdentityUser_GetsTypeAlias()
    {
        var input = @"using System;

namespace MyApp
{
    public class Login
    {
        private IdentityUser currentUser;
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("using IdentityUser = BlazorWebFormsComponents.Identity.IdentityUser;", result);
    }

    [Fact]
    public void File_WithUserLoginInfo_GetsTypeAlias()
    {
        var input = @"using System;

namespace MyApp
{
    public class Login
    {
        private UserLoginInfo loginInfo;
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("using UserLoginInfo = BlazorWebFormsComponents.Identity.UserLoginInfo;", result);
    }

    [Fact]
    public void File_AlreadyHasNamespaceUsing_DoesNotModify()
    {
        var input = @"using System;
using BlazorWebFormsComponents.Identity;

namespace MyApp
{
    public class Login
    {
        public void SignIn()
        {
            IdentityResult result = null;
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Equal(input, result);
    }

    [Fact]
    public void File_WithNoIdentityTypes_IsUnchanged()
    {
        var input = @"using System;
using System.Linq;

namespace MyApp
{
    public class Default
    {
        public void Load()
        {
            var items = new[] { 1, 2, 3 };
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.DoesNotContain("BlazorWebFormsComponents.Identity", result);
        Assert.Equal(input, result);
    }

    [Fact]
    public void File_WithFullyQualifiedMicrosoftIdentityResult_DoesNotGetAlias()
    {
        var input = @"using System;

namespace MyApp
{
    public class Login
    {
        public void SignIn()
        {
            Microsoft.AspNetCore.Identity.IdentityResult result = null;
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.DoesNotContain("using IdentityResult =", result);
    }

    [Fact]
    public void File_WithFullyQualifiedMicrosoftIdentityUser_DoesNotGetAlias()
    {
        var input = @"using System;

namespace MyApp
{
    public class Login
    {
        private Microsoft.AspNetCore.Identity.IdentityUser currentUser;
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.DoesNotContain("using IdentityUser =", result);
    }

    [Fact]
    public void File_WithFullyQualifiedMicrosoftUserLoginInfo_DoesNotGetAlias()
    {
        var input = @"using System;

namespace MyApp
{
    public class Login
    {
        private Microsoft.AspNetCore.Identity.UserLoginInfo loginInfo;
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.DoesNotContain("using UserLoginInfo =", result);
    }

    [Fact]
    public void File_WithMixedQualifiedAndUnqualified_GetsAliasForUnqualified()
    {
        var input = @"using System;

namespace MyApp
{
    public class Login
    {
        private IdentityUser currentUser;
        private Microsoft.AspNetCore.Identity.IdentityResult result;
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        // Has unqualified IdentityUser → needs alias
        Assert.Contains("using IdentityUser = BlazorWebFormsComponents.Identity.IdentityUser;", result);
        // IdentityResult is fully-qualified → no alias needed
        Assert.DoesNotContain("using IdentityResult =", result);
    }

    [Fact]
    public void File_WithMultipleBwfcOnlyTypes_GetsNamespaceUsingOnce()
    {
        var input = @"using System;

namespace MyApp
{
    public class Login
    {
        private ApplicationUserManager userManager;
        private ApplicationSignInManager signInManager;
        private SignInStatus status;
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("using BlazorWebFormsComponents.Identity;", result);

        // Only one namespace using
        var matches = System.Text.RegularExpressions.Regex.Matches(
            result, @"using\s+BlazorWebFormsComponents\.Identity;");
        Assert.Equal(1, matches.Count);
    }

    [Fact]
    public void File_WithBothConflictingAndBwfcOnly_GetsBothStyles()
    {
        var input = @"using System;

namespace MyApp
{
    public class Login
    {
        private ApplicationUserManager userManager;
        private IdentityResult result;
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        // Namespace using for ApplicationUserManager
        Assert.Contains("using BlazorWebFormsComponents.Identity;", result);
        // Type alias for IdentityResult
        Assert.Contains("using IdentityResult = BlazorWebFormsComponents.Identity.IdentityResult;", result);
    }

    [Fact]
    public void Aliases_AreInsertedAfterLastExistingUsing()
    {
        var input = @"using System;
using System.Linq;
using System.Collections.Generic;

namespace MyApp
{
    public class Login
    {
        private IdentityResult result;
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        var genericIndex = result.IndexOf("using System.Collections.Generic;");
        var aliasIndex = result.IndexOf("using IdentityResult =");

        Assert.True(aliasIndex > genericIndex, "Alias should be after last existing using");
    }

    [Fact]
    public void File_WithNoExistingUsings_GetsAliasAtTop()
    {
        var input = @"namespace MyApp
{
    public class Login
    {
        private ApplicationUserManager userManager;
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.StartsWith("using BlazorWebFormsComponents.Identity;", result);
    }

    [Fact]
    public void OrderIs103()
    {
        Assert.Equal(103, _transform.Order);
    }

    [Fact]
    public void NameIsIdentityUsing()
    {
        Assert.Equal("IdentityUsing", _transform.Name);
    }
}
