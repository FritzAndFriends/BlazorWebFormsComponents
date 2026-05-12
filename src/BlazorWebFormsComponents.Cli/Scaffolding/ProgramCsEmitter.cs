using System.Text;
using BlazorWebFormsComponents.Cli.Config;

namespace BlazorWebFormsComponents.Cli.Scaffolding;

/// <summary>
/// Generates Program.cs for migrated Blazor static SSR apps.
/// 
/// Architecture: template-based generation with layered enhancement.
/// 1. Start from a correct .NET 10 Blazor static SSR baseline (what dotnet new blazor produces)
/// 2. Layer on detected features from the old Web Forms app (EF, Identity, Session, etc.)
/// 3. Result is a complete, working Program.cs — not a skeleton that needs L2 repair
/// </summary>
public class ProgramCsEmitter
{
    public string Generate(string projectName, RuntimeProfile profile, DatabaseProviderInfo dbProvider)
    {
        var sb = new StringBuilder();

        EmitUsings(sb, projectName, profile);
        EmitBuilderSection(sb);
        EmitServiceRegistrations(sb, projectName, profile, dbProvider);
        EmitAppBuildAndStartup(sb, projectName, profile);

        return sb.ToString();
    }

    // ── Section 1: Using Declarations ──────────────────────────────────────────

    private static void EmitUsings(StringBuilder sb, string projectName, RuntimeProfile profile)
    {
        // Always present in a BWFC migrated app
        sb.AppendLine("using BlazorWebFormsComponents;");

        if (profile.NeedsEntityFramework)
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");

        if (profile.NeedsIdentity)
        {
            sb.AppendLine("using Microsoft.AspNetCore.Identity;");
            sb.AppendLine($"using {projectName}.Models;");
        }
        else if (profile.NeedsEntityFramework && profile.AdditionalDbContextNames.Count > 0)
        {
            sb.AppendLine($"using {projectName}.Models;");
        }

        sb.AppendLine();
    }

    // ── Section 2: Builder Creation ────────────────────────────────────────────

    private static void EmitBuilderSection(StringBuilder sb)
    {
        // Standard Blazor template baseline — this is what dotnet new blazor generates
        sb.AppendLine("var builder = WebApplication.CreateBuilder(args);");
        sb.AppendLine();
    }

    // ── Section 3: Service Registrations (layered onto baseline) ───────────────

    private static void EmitServiceRegistrations(
        StringBuilder sb, string projectName, RuntimeProfile profile, DatabaseProviderInfo dbProvider)
    {
        // 3a. Razor components (always — Blazor static SSR baseline)
        // NEVER add .AddInteractiveServerComponents() — migrated apps target static SSR only.
        // Interactive render modes must be opted into deliberately per-page, not globally.
        sb.AppendLine("// Add services to the container.");
        sb.AppendLine("builder.Services.AddRazorComponents();");

        // 3b. HttpContext access (needed for Session, Identity, or BWFC shims)
        if (profile.NeedsSession || profile.NeedsIdentity)
        {
            sb.AppendLine("builder.Services.AddHttpContextAccessor();");
        }

        // 3c. Session (detected from Session[] usage or sessionState config)
        if (profile.NeedsSession)
        {
            sb.AppendLine("builder.Services.AddDistributedMemoryCache();");
            sb.AppendLine("builder.Services.AddSession(options =>");
            sb.AppendLine("{");
            sb.AppendLine("    options.Cookie.HttpOnly = true;");
            sb.AppendLine("    options.Cookie.IsEssential = true;");
            sb.AppendLine("});");
        }

        // 3d. Entity Framework DbContext(s)
        if (profile.NeedsEntityFramework)
        {
            EmitEntityFrameworkRegistration(sb, profile, dbProvider);
        }

        // 3e. ASP.NET Identity
        if (profile.NeedsIdentity)
        {
            EmitIdentityRegistration(sb, profile);
        }

        // 3f. BWFC services (always — the whole point of the migration)
        sb.AppendLine();
        sb.AppendLine("builder.Services.AddBlazorWebFormsComponents();");
        sb.AppendLine();
    }

    private static void EmitEntityFrameworkRegistration(
        StringBuilder sb, RuntimeProfile profile, DatabaseProviderInfo dbProvider)
    {
        var connectionStringName = profile.ConnectionStringNames.FirstOrDefault() ?? "DefaultConnection";
        var dbContextTypeName = profile.NeedsIdentity
            ? "ApplicationDbContext"
            : (profile.ResolvedDbContextTypeName ?? "YourDbContext");

        sb.AppendLine();
        sb.AppendLine($"var connectionString = builder.Configuration.GetConnectionString(\"{connectionStringName}\")");
        sb.AppendLine($"    ?? throw new InvalidOperationException(\"Connection string '{connectionStringName}' was not found.\");");
        sb.AppendLine($"builder.Services.AddDbContext<{dbContextTypeName}>(options =>");
        sb.AppendLine($"    options.{dbProvider.ProviderMethod}(connectionString));");

        foreach (var additionalContext in profile.AdditionalDbContextNames)
        {
            if (profile.NeedsIdentity && string.Equals(additionalContext, "ApplicationDbContext", StringComparison.Ordinal))
                continue;

            sb.AppendLine($"builder.Services.AddDbContextFactory<{additionalContext}>(options =>");
            sb.AppendLine($"    options.{dbProvider.ProviderMethod}(connectionString));");
        }
    }

    private static void EmitIdentityRegistration(StringBuilder sb, RuntimeProfile profile)
    {
        sb.AppendLine();
        sb.AppendLine("var identityBuilder = builder.Services.AddDefaultIdentity<ApplicationUser>(options =>");
        sb.AppendLine("{");
        sb.AppendLine("    options.SignIn.RequireConfirmedAccount = false;");
        sb.AppendLine("})");
        sb.AppendLine("    .AddRoles<IdentityRole>();");

        if (profile.NeedsEntityFramework)
        {
            sb.AppendLine("identityBuilder.AddEntityFrameworkStores<ApplicationDbContext>();");
        }
        else
        {
            sb.AppendLine("// TODO(bwfc-identity): No EF DbContext was detected. Add a DbContext and wire identity stores.");
            sb.AppendLine("// identityBuilder.AddEntityFrameworkStores<ApplicationDbContext>();");
        }

        sb.AppendLine("builder.Services.ConfigureApplicationCookie(options =>");
        sb.AppendLine("{");

        // Use detected auth paths from Web.config, or fall back to standard defaults
        var loginPath = profile.AuthLoginPath ?? "/Account/Login";
        var logoutPath = "/Account/Logout";
        sb.AppendLine($"    options.LoginPath = \"{loginPath}\";");
        sb.AppendLine($"    options.LogoutPath = \"{logoutPath}\";");

        sb.AppendLine("});");
        sb.AppendLine("builder.Services.AddAuthorization();");
        sb.AppendLine("builder.Services.AddCascadingAuthenticationState();");
    }

    // ── Section 4: App Build, Middleware, and Run ──────────────────────────────

    private static void EmitAppBuildAndStartup(
        StringBuilder sb, string projectName, RuntimeProfile profile)
    {
        sb.AppendLine("var app = builder.Build();");

        // 4a. EnsureCreated for all registered DbContexts
        if (profile.NeedsEntityFramework)
        {
            EmitEnsureCreated(sb, profile);
        }

        // 4b. Environment-specific middleware (standard Blazor baseline)
        sb.AppendLine();
        sb.AppendLine("if (!app.Environment.IsDevelopment())");
        sb.AppendLine("{");
        sb.AppendLine("    app.UseExceptionHandler(\"/Error\");");
        sb.AppendLine("    app.UseHsts();");
        sb.AppendLine("}");
        sb.AppendLine();

        // 4c. Standard middleware pipeline (order matters — follows ASP.NET Core conventions)
        sb.AppendLine("app.UseHttpsRedirection();");
        sb.AppendLine("app.MapStaticAssets();");
        sb.AppendLine("app.UseBlazorWebFormsComponents();");

        if (profile.NeedsSession)
            sb.AppendLine("app.UseSession();");

        if (profile.NeedsIdentity)
        {
            sb.AppendLine("app.UseAuthentication();");
            sb.AppendLine("app.UseAuthorization();");
        }

        sb.AppendLine("app.UseAntiforgery();");

        if (profile.NeedsIdentity)
        {
            // Auth endpoints (Login/Register/Logout) are generated by RedirectHandlerAnnotator
            // when Account pages are detected in the source project.
        }

        // 4d. Application_Start patterns as TODO comments
        if (profile.ApplicationStartPatterns.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("// TODO(bwfc-general): Review legacy Application_Start registrations:");
            foreach (var pattern in profile.ApplicationStartPatterns)
            {
                sb.AppendLine($"// - {pattern}");
            }
        }

        // 4e. Custom error page registration (from Web.config <customErrors>)
        if (!string.IsNullOrEmpty(profile.CustomErrorRedirect))
        {
            sb.AppendLine();
            sb.AppendLine($"// Custom error page detected from Web.config <customErrors defaultRedirect=\"{profile.CustomErrorRedirect}\">");
            sb.AppendLine("// The UseExceptionHandler(\"/Error\") above handles this. Create an /Error page if needed.");
        }

        // 4f. Map Razor components (static SSR only — no interactive render mode)
        // NEVER add .AddInteractiveServerRenderMode() — migrated apps target static SSR only.
        sb.AppendLine();
        sb.AppendLine($"app.MapRazorComponents<{projectName}.Components.App>();");

        // 4g. Identity seed data
        if (profile.NeedsIdentity && (profile.DetectedRoleNames.Count > 0 || profile.DetectedSeedUsers.Count > 0))
        {
            EmitIdentitySeedData(sb, profile);
        }

        sb.AppendLine();
        sb.AppendLine("app.Run();");
    }

    private static void EmitEnsureCreated(StringBuilder sb, RuntimeProfile profile)
    {
        var primaryContext = profile.NeedsIdentity
            ? "ApplicationDbContext"
            : (profile.ResolvedDbContextTypeName ?? "YourDbContext");

        var additionalContexts = profile.AdditionalDbContextNames
            .Where(ctx => !string.Equals(ctx, primaryContext, StringComparison.Ordinal))
            .ToList();

        sb.AppendLine();
        sb.AppendLine("// Ensure database tables exist for all registered DbContexts");
        sb.AppendLine("using (var scope = app.Services.CreateScope())");
        sb.AppendLine("{");
        sb.AppendLine($"    scope.ServiceProvider.GetRequiredService<{primaryContext}>().Database.EnsureCreated();");

        foreach (var ctx in additionalContexts)
        {
            sb.AppendLine($"    var {char.ToLowerInvariant(ctx[0])}{ctx[1..]}Factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<{ctx}>>();");
            sb.AppendLine($"    using (var {char.ToLowerInvariant(ctx[0])}{ctx[1..]} = {char.ToLowerInvariant(ctx[0])}{ctx[1..]}Factory.CreateDbContext())");
            sb.AppendLine("    {");
            sb.AppendLine($"        {char.ToLowerInvariant(ctx[0])}{ctx[1..]}.Database.EnsureCreated();");
            sb.AppendLine("    }");
        }

        sb.AppendLine("}");
    }

    private static void EmitIdentitySeedData(StringBuilder sb, RuntimeProfile profile)
    {
        sb.AppendLine();
        sb.AppendLine("// Seed identity roles and users (detected from Web Forms source)");
        sb.AppendLine("using (var scope = app.Services.CreateScope())");
        sb.AppendLine("{");
        sb.AppendLine("    var services = scope.ServiceProvider;");

        if (profile.DetectedRoleNames.Count > 0)
        {
            sb.AppendLine("    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();");
            foreach (var role in profile.DetectedRoleNames)
            {
                sb.AppendLine($"    if (!await roleManager.RoleExistsAsync(\"{role}\"))");
                sb.AppendLine($"        await roleManager.CreateAsync(new IdentityRole(\"{role}\"));");
            }
        }

        if (profile.DetectedSeedUsers.Count > 0)
        {
            sb.AppendLine("    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();");
            foreach (var (email, password, roleName) in profile.DetectedSeedUsers)
            {
                sb.AppendLine($"    if (await userManager.FindByEmailAsync(\"{email}\") == null)");
                sb.AppendLine("    {");
                sb.AppendLine($"        var seedUser = new ApplicationUser {{ UserName = \"{email}\", Email = \"{email}\" }};");
                sb.AppendLine($"        await userManager.CreateAsync(seedUser, \"{password}\");");
                if (!string.IsNullOrEmpty(roleName))
                    sb.AppendLine($"        await userManager.AddToRoleAsync(seedUser, \"{roleName}\");");
                sb.AppendLine("    }");
            }
        }

        sb.AppendLine("}");
    }
}
