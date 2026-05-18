# Skill: Code-Behind Transform — Class Body Location Pattern

## Summary

When a code-behind transform must locate the class body opening brace (`{`) to inject fields or members, it must use `[^{]*\{` **not** `\s*\{` between the class name and the brace.

## Problem This Solves

Code-behind transforms run in Order sequence. Earlier transforms (e.g., `BaseClassStripTransform` at Order 200) modify the class declaration before later transforms see it. By Order 220+, a typical Page class looks like:

```csharp
public partial class ShoppingCart : WebFormsPageBase
{
```

A regex like `partial\s+class\s+\w+\s*\{` fails to match this because `: WebFormsPageBase\n    ` is not `\s*`. The transform silently returns the content unchanged — no injection happens, no error is raised.

## Correct Pattern

```csharp
// WRONG — fails when base class or interface list is present
private static readonly Regex ClassOpenRegex = new(
    @"partial\s+class\s+\w+\s*\{",
    RegexOptions.Compiled);

// CORRECT — [^{]* skips base class, interfaces, newlines, anything before {
private static readonly Regex ClassOpenRegex = new(
    @"partial\s+class\s+\w+[^{]*\{",
    RegexOptions.Compiled);
```

`[^{]*` matches `: WebFormsPageBase`, `: Base, IInterface`, spaces, newlines — everything up to (but not including) the first `{`.

## When To Apply

- Any transform that injects members/fields/properties into a class body.
- Any transform that locates the class body to insert using statements or attributes near it.

## Test Coverage Required

When authoring such a transform, always include these three test shapes:

1. **Plain class** — `public partial class Foo {` (baseline)
2. **Class with base** — `public partial class Foo : WebFormsPageBase {`  
3. **Brace on next line with base** — `public partial class Foo : WebFormsPageBase\n{`

## Real-World Example

`ComponentRefCodeBehindTransform` (Order 220) injects `@ref` backing fields. It was broken from inception because its `ClassOpenRegex` used `\s*\{`. After `BaseClassStripTransform` (Order 200) added `: WebFormsPageBase`, the regex never matched — causing 15+ CS0103 errors per page in WingtipToys Run 80.

Fix commit: `ClassOpenRegex` changed from `\s*\{` to `[^{]*\{` (2026-05-15).
