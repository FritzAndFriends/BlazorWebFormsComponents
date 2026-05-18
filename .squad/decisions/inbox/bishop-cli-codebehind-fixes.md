# Bishop decision inbox — CLI code-behind fixes

- **Date:** 2026-05-17T10:01:20-04:00
- **Owner:** Bishop

## Decision
1. `LegacyHelperStubTransform` must skip page, master, and user-control code-behinds by `FileMetadata.FileType` first, not by `SourceFilePath` suffix checks alone.
2. Keep extension-based fallback checks on both source and output paths, including `.razor.cs`, as belt-and-suspenders protection for renamed code-behind files.
3. When helper stubs are generated, preserve protected method signatures instead of dropping them from the extracted API surface.
4. `DataBindTransform` must consume optional `this.` prefixes for both `DataSource` assignments and `DataBind()` removal, and the `DataBind()` regex must own the whole line so it cannot leave a dangling `this.` token behind.

## Why
`FileMetadata.SourceFilePath` points at the markup file for page migrations, so suffix-only detection silently treated real page code-behinds as standalone helpers whenever they referenced `System.Configuration` or other legacy namespaces. That replaced Contoso page logic with empty stubs and stripped event-handler bodies out of generated `.razor.cs` files.

The companion `DataBindTransform` bug was smaller but still a true pipeline failure: removing only `grv.DataBind();` from `this.grv.DataBind();` leaves invalid C#. These are Layer 1 correctness bugs, so the pipeline has to absorb them before L2 ever sees the output.

## Validation
- Added focused regression tests for `LegacyHelperStubTransform` skip behavior and protected helper methods.
- Added focused regression tests for `DataBindTransform` handling of `this.`-prefixed `DataSource` and `DataBind()` statements.
- Full CLI test suite passed: 818/818.
- Fresh isolated Contoso migration confirmed `Instructors.razor.cs` no longer has a dangling `this.` in `OnAfterRenderAsync`, and `Students.razor.cs` / `Courses.razor.cs` preserved real page code instead of API stubs.
