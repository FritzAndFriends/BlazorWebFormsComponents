>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# FileUpload — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.fileupload?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.FileUpload`
**Implementation Status:** ✅ Implemented

## Properties

### Control-Specific Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| HasFile | bool | ✅ Match | Returns true when a file is selected |
| HasFiles | bool | ✅ Match | Returns true when multiple files selected |
| FileName | string | ✅ Match | Name of the selected file |
| FileBytes | byte[] | ✅ Match | File content as byte array (also has async `GetFileBytesAsync`) |
| FileContent | Stream | ✅ Match | File content as stream |
| PostedFile | HttpPostedFile | ✅ Match | Via `PostedFileWrapper` class with ContentLength, ContentType, FileName, InputStream |
| AllowMultiple | bool | ✅ Match | Enables multi-file selection |
| ToolTip | string | ✅ Match | Rendered as `title` attribute |
| Accept | string | ✅ Match | Blazor-specific: file type filter (not in Web Forms) |
| MaxFileSize | long | ✅ Match | Blazor-specific: max file size limit (not in Web Forms) |

### WebControl Inherited Properties (from BaseStyledComponent)

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| AccessKey | string | 🔴 Missing | Not in BaseStyledComponent |
| BackColor | Color | ✅ Match | From BaseStyledComponent |
| BorderColor | Color | ✅ Match | From BaseStyledComponent |
| BorderStyle | BorderStyle | ✅ Match | From BaseStyledComponent |
| BorderWidth | Unit | ✅ Match | From BaseStyledComponent |
| CssClass | string | ✅ Match | From BaseStyledComponent |
| Enabled | bool | ✅ Match | From BaseWebFormsComponent; renders `disabled` attribute |
| Font | FontInfo | ✅ Match | From BaseStyledComponent |
| ForeColor | Color | ✅ Match | From BaseStyledComponent |
| Height | Unit | ✅ Match | From BaseStyledComponent |
| Width | Unit | ✅ Match | From BaseStyledComponent |
| TabIndex | short | ✅ Match | From BaseWebFormsComponent |
| Style | CssStyleCollection | ✅ Match | Computed from BaseStyledComponent |

### Control Inherited Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | string | ✅ Match | From BaseWebFormsComponent; rendered as `id` |
| ClientID | string | ✅ Match | From BaseWebFormsComponent |
| Visible | bool | ✅ Match | From BaseWebFormsComponent |
| EnableViewState | bool | N/A | Server-only |
| ViewState | StateBag | N/A | Server-only |
| EnableTheming | bool | N/A | Server-only |
| SkinID | string | N/A | Server-only |
| Page | Page | N/A | Server-only |
| NamingContainer | Control | N/A | Server-only |
| UniqueID | string | N/A | Server-only |
| ClientIDMode | ClientIDMode | N/A | Server-only |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| FileSelected | — | ✅ Match | `EventCallback<InputFileChangeEventArgs> OnFileSelected` (Blazor equivalent) |
| Init | EventHandler | ✅ Match | Via base class |
| Load | EventHandler | ✅ Match | Via base class |
| PreRender | EventHandler | ✅ Match | Via base class |
| Unload | EventHandler | ✅ Match | Via base class |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| SaveAs(string) | void | ✅ Match | Async version; includes path sanitization |
| SaveAllFiles(string) | — | ✅ Match | Blazor-specific: saves all multi-upload files |
| GetMultipleFiles() | — | ✅ Match | Blazor-specific: returns IBrowserFile collection |
| Focus() | void | N/A | Server-only |

## HTML Output Comparison

Web Forms renders `<input type="file" ... />`. The Blazor component uses Blazor's `<InputFile>` component which also renders as `<input type="file" ... />` in the DOM — identical HTML output.

Supports `multiple`, `accept`, `disabled`, `id`, `style`, `class`, and `title` attributes.

Key architecture note: Uses Blazor's `InputFile` internally (not raw `<input type="file">`) for proper `IBrowserFile` data access. See team decision about this ship-blocking fix.

## Summary

- **Matching:** 22 properties, 5 events
- **Needs Work:** 0 properties, 0 events
- **Missing:** 1 property (AccessKey), 0 events
- **N/A (server-only):** 7 items
