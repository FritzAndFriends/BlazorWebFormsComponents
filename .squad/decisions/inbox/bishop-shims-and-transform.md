# Decision: Run 34 shim compatibility and attribute data-binding transform

**Date:** 2026-05-06
**Author:** Bishop (Migration Tooling Dev)
**Status:** Proposed

## Context

Run 34 exposed four deterministic migration gaps that should be fixed in tooling rather than left for Layer 2 manual cleanup:

1. BWFC components accepted `Width="500"` but not CSS-style string literals such as `Width="500px"` or `Width="50%"` in migrated Razor.
2. Migrated code frequently used `Request.QueryString.Get("key")`, but `RequestShim.QueryString` surfaced `IQueryCollection`, which only supported indexer access.
3. Migrated code also referenced `System.Web.HttpUtility`, which modern .NET apps do not expose by default.
4. The CLI handled `<%# ... %>` expressions in content, but not when those expressions appeared inside attribute values.

## Decision

### 1. Width and Height parameters on `BaseStyledComponent`

Use **string-backed component parameters** for `Width` and `Height`, while preserving the internal `IStyle`/`IHasLayoutStyle` contract as `Unit` via explicit interface implementation.

- Public component parameters accept plain strings such as `"500"`, `"500px"`, and `"50%"`.
- Existing quoted legacy syntax such as `Width="Unit.Pixel(200)"` is preserved by parsing `Unit.Pixel(...)`, `Unit.Point(...)`, and `Unit.Percentage(...)` string forms.
- Internal style generation, theme skin application, and style-copy logic still operate on `Unit` values.

### 2. Query string `.Get()` compatibility

Add `BlazorWebFormsComponents.QueryStringExtensions.Get(string)` as a global BWFC extension for `IQueryCollection`.

- This keeps migrated `Request.QueryString.Get("key")` code compiling unchanged.
- Returning `queryString[key]?.ToString()` matches Web Forms expectations closely enough for migration compatibility.

### 3. `System.Web.HttpUtility` compatibility shim

Add a static `System.Web.HttpUtility` type to the BWFC library backed by `System.Net.WebUtility`.

- `UrlEncode` / `UrlDecode` delegate to `WebUtility.UrlEncode` / `WebUtility.UrlDecode`.
- `HtmlEncode` / `HtmlDecode` delegate to `WebUtility.HtmlEncode` / `WebUtility.HtmlDecode`.
- This provides the Web Forms API surface without requiring migrated apps to rewrite these calls immediately.

### 4. Attribute data-binding transform

Add `DataBindingAttributeTransform` with **Order 615**, registered immediately after `AspPrefixTransform` in both the CLI runtime and test pipeline.

- It rewrites attribute values containing `<%# ... %>` or `<%= ... %>` into Razor `@(...)` expressions.
- It preserves surrounding attributes and switches quote styles when needed so embedded C# string literals remain valid Razor.
- This closes the gap for controls like `<HyperLink NavigateUrl='<%# Item.GetUrl() %>' />`.

## Rationale

These four fixes are all deterministic compile-surface issues. Leaving them to Layer 2 wastes manual time and creates noisy false negatives in benchmark runs, while the tooling can resolve them mechanically and safely.

Using string-backed `Width`/`Height` parameters is the least disruptive way to unlock plain migrated Razor markup while preserving the typed `Unit` behavior internally where BWFC styling already depends on it. The shim additions follow the existing project direction of zero-rewrite compile-compatibility for common Web Forms APIs.

## Validation

- `dotnet build src\BlazorWebFormsComponents\BlazorWebFormsComponents.csproj --nologo`
- `dotnet test src\BlazorWebFormsComponents.Test --nologo`
- `dotnet test tests\BlazorWebFormsComponents.Cli.Tests --nologo`

All commands succeeded after the changes.
