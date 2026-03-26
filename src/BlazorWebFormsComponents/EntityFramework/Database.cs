using System;

namespace BlazorWebFormsComponents.EntityFramework;

/// <summary>
/// Compatibility shim for EF6's <c>Database.SetInitializer&lt;TContext&gt;()</c> pattern.
/// <para>
/// In EF6, this static class configured the database initialization strategy.
/// EF Core doesn't have this concept — configure database creation in Program.cs
/// using <c>DbContext.Database.EnsureCreated()</c> or EF Core Migrations.
/// </para>
/// </summary>
[Obsolete("EF Core does not use Database.SetInitializer. Configure in Program.cs instead.")]
public static class Database
{
    public static void SetInitializer<TContext>(object? initializer) where TContext : class
    {
        // No-op — EF Core handles database creation differently
    }
}
