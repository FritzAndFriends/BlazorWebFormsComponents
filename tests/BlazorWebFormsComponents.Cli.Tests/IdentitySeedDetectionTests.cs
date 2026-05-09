using BlazorWebFormsComponents.Cli.Config;
using BlazorWebFormsComponents.Cli.Scaffolding;

namespace BlazorWebFormsComponents.Cli.Tests;

public class IdentitySeedDetectionTests
{
    [Fact]
    public void DetectsRoleNames_FromRoleManagerCreate()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"bwfc-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);
        try
        {
            File.WriteAllText(Path.Combine(tempDir, "RoleActions.cs"), """
                var roleMgr = new RoleManager<IdentityRole>(roleStore);
                if (!roleMgr.RoleExists("canEdit"))
                {
                    IdRoleResult = roleMgr.Create(new IdentityRole { Name = "canEdit" });
                }
                """);

            var detector = new IdentityRuntimeSignalDetector();
            var profile = new RuntimeProfile();
            detector.Apply(tempDir, profile);

            Assert.True(profile.NeedsIdentity);
            Assert.Contains("canEdit", profile.DetectedRoleNames);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void DetectsSeedUsers_FromUserManagerCreate()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"bwfc-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);
        try
        {
            File.WriteAllText(Path.Combine(tempDir, "RoleActions.cs"), """
                var roleMgr = new RoleManager<IdentityRole>(roleStore);
                roleMgr.Create(new IdentityRole { Name = "canEdit" });
                var userMgr = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var appUser = new ApplicationUser
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com"
                };
                IdUserResult = userMgr.Create(appUser, "Pa$$word1");
                userMgr.AddToRole(userId, "canEdit");
                """);

            var detector = new IdentityRuntimeSignalDetector();
            var profile = new RuntimeProfile();
            detector.Apply(tempDir, profile);

            Assert.Single(profile.DetectedSeedUsers);
            Assert.Equal("admin@example.com", profile.DetectedSeedUsers[0].Email);
            Assert.Equal("Pa$$word1", profile.DetectedSeedUsers[0].Password);
            Assert.Equal("canEdit", profile.DetectedSeedUsers[0].RoleName);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void ProgramCsEmitter_IncludesIdentitySeeding()
    {
        var emitter = new ProgramCsEmitter();
        var profile = new RuntimeProfile
        {
            NeedsIdentity = true,
            NeedsEntityFramework = true,
            DetectedRoleNames = ["canEdit"],
            DetectedSeedUsers = [("admin@test.com", "Pa$$word1", "canEdit")]
        };
        var dbProvider = new DatabaseProviderInfo
        {
            ProviderMethod = "UseSqlite",
            PackageName = "Microsoft.EntityFrameworkCore.Sqlite",
            DetectedFrom = "test",
            ConnectionString = "Data Source=test.db"
        };

        var result = emitter.Generate("TestApp", profile, dbProvider);

        Assert.Contains("RoleManager<IdentityRole>", result);
        Assert.Contains("RoleExistsAsync(\"canEdit\")", result);
        Assert.Contains("FindByEmailAsync(\"admin@test.com\")", result);
        Assert.Contains("CreateAsync(seedUser, \"Pa$$word1\")", result);
        Assert.Contains("AddToRoleAsync(seedUser, \"canEdit\")", result);
        Assert.Contains(".AddRoles<IdentityRole>()", result);
    }
}
