using System;

namespace BlazorWebFormsComponents.EntityFramework;

/// <summary>
/// Compatibility shim for EF6's <c>DropCreateDatabaseIfModelChanges&lt;TContext&gt;</c>.
/// <para>
/// In EF6, this was a database initializer that dropped and recreated the database when
/// the model changed. EF Core has no equivalent — use <c>DbContext.Database.EnsureCreated()</c>
/// or EF Core migrations instead.
/// </para>
/// <para>
/// This stub exists so that code-behinds referencing the type compile after Layer 1.
/// Layer 2 should remove the usage entirely and configure EF Core properly in Program.cs.
/// </para>
/// </summary>
[Obsolete("EF Core has no database initializers. Use DbContext.Database.EnsureCreated() or EF Core Migrations.")]
public class DropCreateDatabaseIfModelChanges<TContext> where TContext : class
{
    public virtual void InitializeDatabase(TContext context) { }

    protected virtual void Seed(TContext context) { }
}
