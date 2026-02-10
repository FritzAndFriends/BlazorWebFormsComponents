# Decision: FileUpload must use Blazor InputFile component internally

**Decided by:** Cyclops (implementing Forge's review finding)
**Date:** 2026-02-10
**Context:** Sprint 1, Item 2 (P0) — Fix FileUpload Component Data Flow

## Problem

The `<input type="file" @onchange="handler">` pattern in Blazor receives `ChangeEventArgs`, which contains only the file name as a string value. It does NOT provide `InputFileChangeEventArgs` or `IBrowserFile` objects. This means:
- `_currentFile` is never populated
- `HasFile` always returns `false`
- `FileBytes`, `FileContent`, `PostedFile`, `SaveAs()` are all fundamentally broken

## Decision

FileUpload MUST use Blazor's `<InputFile>` component internally instead of a raw `<input type="file">`. `InputFile` provides proper `InputFileChangeEventArgs` with `IBrowserFile` objects that enable all file operations.

## Implications

- Any future component needing browser file access must use `InputFile`, not raw `<input type="file">`
- `@using Microsoft.AspNetCore.Components.Forms` is required in the `.razor` file
- `InputFile` renders as `<input type="file">` in the DOM, so existing tests targeting that selector still pass
