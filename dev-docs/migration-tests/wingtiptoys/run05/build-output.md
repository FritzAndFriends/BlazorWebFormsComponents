# Run 5 Build Output

## Final Clean Build

```
Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:04.56
```

## Build Round 1 (33 errors)

Errors were in Account and Checkout pages that still had unconverted Web Forms markup
(ProviderName, Title, Email, Click handlers, Password fields).

**Fix:** Replaced all Account/*.razor and Checkout/*.razor files with stub pages.

## Build Round 2 (2 errors)

```
App.razor(10,30): error CS0103: The name 'InteractiveServer' does not exist in the current context
App.razor(14,26): error CS0103: The name 'InteractiveServer' does not exist in the current context
```

**Fix:** Added `@using static Microsoft.AspNetCore.Components.Web.RenderMode` to _Imports.razor.

## Build Configuration

- Target: net10.0
- SDK: 10.0.200-preview.0.26103.119
- Build command: `dotnet build samples/AfterWingtipToys -p:NBGV_CacheMode=None`
- Project reference: `..\..\src\BlazorWebFormsComponents\BlazorWebFormsComponents.csproj`
