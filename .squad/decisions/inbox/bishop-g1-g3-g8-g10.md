# Bishop decisions — G1 / G3 / G8 / G10

## Context
Run 35 exposed four recurring Layer 1 gaps in migrated Wingtip output: invalid display-expression Razor, EF6 DbContext constructor emission, unsupported `Server.*` compatibility calls, and `HttpUtility` ambiguity when migrated apps still carry the legacy package.

## Decisions

1. **Normalize Web Forms display expressions before the main expression pass.**
   - Added `DisplayExpressionTransform` at Order 490.
   - It converts `<%#: expr %>`, `<%=: expr %>`, and broken `@(: expr)` into `@(...)` early so later markup transforms operate on valid Razor.

2. **Modernize EF6 DbContext constructors in Layer 1 instead of leaving them for manual repair.**
   - Added `EfContextConstructorTransform` at Order 106.
   - It rewrites `: base("name")` constructors on `DbContext` / `IdentityDbContext` types to EF Core `DbContextOptions<TContext>` constructors and ensures `using Microsoft.EntityFrameworkCore;` exists.

3. **Treat `Server.Transfer`, `Server.GetLastError`, and `Server.ClearError` as supported shim surface.**
   - Extended `ServerShim` with compatibility implementations/stubs.
   - Updated CLI guidance so these calls are no longer flagged as “NO SHIM” manual rewrites.

4. **Rewrite `HttpUtility` inline instead of depending on the compatibility shim.**
   - Added `HttpUtilityRewriteTransform` at Order 104.
   - The CLI now rewrites supported `HttpUtility` calls directly to `WebUtility`, adds `using System.Net`, and avoids relying on `System.Web.HttpUtility` in the generated app.

5. **Apply G3/G10 fixes to copied source files, not only page code-behind.**
   - Updated `SourceFileCopier` to include the new transforms so `Models`, `Logic`, and similar copied files receive the same modernization pass.

6. **Guard generated projects against the legacy HttpUtility package.**
   - Added an explicit `System.Web.HttpUtility` package-strip guard in `ProjectScaffolder` and a regression assertion in scaffolding tests.

## Validation
- `dotnet build src\BlazorWebFormsComponents\BlazorWebFormsComponents.csproj --nologo`
- `dotnet test src\BlazorWebFormsComponents.Test --nologo`
- `dotnet test tests\BlazorWebFormsComponents.Cli.Tests --nologo`
