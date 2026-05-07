using System.Text;
using BlazorWebFormsComponents.Cli.Config;

namespace BlazorWebFormsComponents.Cli.Scaffolding;

public class ProgramCsEmitter
{
    public string Generate(string projectName, RuntimeProfile profile, DatabaseProviderInfo dbProvider)
    {
        var builder = new StringBuilder();

        builder.AppendLine("// TODO(bwfc-general): Review and adjust this generated Program.cs for your application needs.");
        builder.AppendLine("// Generated for .NET 10 Blazor static SSR. Keep interactive render modes opt-in and page-specific.");
        builder.AppendLine("using BlazorWebFormsComponents;");

        if (profile.NeedsEntityFramework)
            builder.AppendLine("using Microsoft.EntityFrameworkCore;");

        if (profile.NeedsIdentity)
            builder.AppendLine("using Microsoft.AspNetCore.Identity;");

        builder.AppendLine();
        builder.AppendLine("var builder = WebApplication.CreateBuilder(args);");
        builder.AppendLine();
        builder.AppendLine("builder.Services.AddRazorComponents();");

        if (profile.NeedsSession)
        {
            builder.AppendLine("builder.Services.AddHttpContextAccessor();");
            builder.AppendLine("builder.Services.AddDistributedMemoryCache();");
            builder.AppendLine("builder.Services.AddSession(options =>");
            builder.AppendLine("{");
            builder.AppendLine("    options.Cookie.HttpOnly = true;");
            builder.AppendLine("    options.Cookie.IsEssential = true;");
            builder.AppendLine("});");
        }

        if (profile.NeedsEntityFramework)
        {
            var connectionStringName = profile.ConnectionStringNames.FirstOrDefault() ?? "DefaultConnection";
            var dbContextTypeName = profile.ResolvedDbContextTypeName ?? "YourDbContext";

            builder.AppendLine();
            builder.AppendLine($"var connectionString = builder.Configuration.GetConnectionString(\"{connectionStringName}\")");
            builder.AppendLine($"    ?? throw new InvalidOperationException(\"Connection string '{connectionStringName}' was not found.\");");
            builder.AppendLine($"builder.Services.AddDbContext<{dbContextTypeName}>(options =>");
            builder.AppendLine($"    options.{dbProvider.ProviderMethod}(connectionString));");
        }

        if (profile.NeedsIdentity)
        {
            builder.AppendLine();
            builder.AppendLine("var identityBuilder = builder.Services.AddDefaultIdentity<IdentityUser>(options =>");
            builder.AppendLine("{");
            builder.AppendLine("    options.SignIn.RequireConfirmedAccount = false;");
            builder.AppendLine("});");

            if (profile.NeedsEntityFramework && !string.IsNullOrWhiteSpace(profile.ResolvedDbContextTypeName))
            {
                builder.AppendLine($"identityBuilder.AddEntityFrameworkStores<{profile.ResolvedDbContextTypeName}>();");
            }
            else
            {
                builder.AppendLine("// TODO(bwfc-identity): Identity pages were detected but no DbContext class was found.");
                builder.AppendLine("// Add identityBuilder.AddEntityFrameworkStores<YourDbContext>() after migrating your auth store.");
            }

            builder.AppendLine("builder.Services.AddAuthorization();");
            builder.AppendLine("builder.Services.AddCascadingAuthenticationState();");
        }

        builder.AppendLine();
        builder.AppendLine("builder.Services.AddBlazorWebFormsComponents();");
        builder.AppendLine();
        builder.AppendLine("var app = builder.Build();");
        builder.AppendLine();
        builder.AppendLine("if (!app.Environment.IsDevelopment())");
        builder.AppendLine("{");
        builder.AppendLine("    app.UseExceptionHandler(\"/Error\");");
        builder.AppendLine("    app.UseHsts();");
        builder.AppendLine("}");
        builder.AppendLine();
        builder.AppendLine("app.UseHttpsRedirection();");
        builder.AppendLine("app.UseStaticFiles();");

        if (profile.NeedsSession)
            builder.AppendLine("app.UseSession();");

        if (profile.NeedsIdentity)
        {
            builder.AppendLine("app.UseAuthentication();");
            builder.AppendLine("app.UseAuthorization();");
        }

        builder.AppendLine("app.UseAntiforgery();");

        if (profile.ApplicationStartPatterns.Count > 0)
        {
            builder.AppendLine();
            builder.AppendLine("// TODO(bwfc-general): Review legacy Application_Start registrations during runtime cutover:");
            foreach (var pattern in profile.ApplicationStartPatterns)
            {
                builder.AppendLine($"// - {pattern}");
            }
        }

        builder.AppendLine();
        builder.AppendLine($"app.MapRazorComponents<{projectName}.Components.App>();");
        builder.AppendLine();
        builder.AppendLine("app.Run();");

        return builder.ToString();
    }
}
