# FileUpload

The **FileUpload** component provides file upload functionality that emulates the ASP.NET Web Forms FileUpload control. It renders an HTML file input element and exposes properties and methods familiar to Web Forms developers, such as `HasFile`, `FileName`, `PostedFile`, and `SaveAs`.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.fileupload?view=netframework-4.8

## Features Supported in Blazor

- `HasFile` — indicates whether a file has been selected
- `FileName` — the name of the selected file
- `FileBytes` — the file content as a byte array (synchronous)
- `FileContent` — a `Stream` pointing to the uploaded file
- `PostedFile` — a `PostedFileWrapper` providing `ContentLength`, `ContentType`, `FileName`, `InputStream`, and `SaveAs` (compatible with Web Forms `HttpPostedFile` patterns)
- `AllowMultiple` — enables multi-file selection (default: `false`)
- `Accept` — restricts file types via the HTML `accept` attribute (e.g., `".jpg,.png"` or `"image/*"`)
- `MaxFileSize` — maximum file size in bytes (default: `512000` / ~500 KiB)
- `ToolTip` — tooltip text displayed on hover
- `OnFileSelected` — event raised when a file is selected
- `SaveAs(filename)` — saves the uploaded file to a specified server path
- `GetFileBytesAsync()` — async method to get file content as a byte array
- `GetMultipleFiles()` — returns all selected files when `AllowMultiple` is enabled
- `SaveAllFiles(directory)` — saves all uploaded files to a directory with sanitized filenames
- `Enabled` — enables or disables the file input
- `Visible` — controls visibility
- All base style properties (`CssClass`, `Style`, `BackColor`, `ForeColor`, `BorderColor`, `BorderStyle`, `BorderWidth`, `Width`, `Height`, `Font`)

### Blazor Notes

- The control uses Blazor's `InputFile` component internally, which renders as a standard HTML `<input type="file">` element
- File processing must be handled through component properties or methods
- The `OnFileSelected` event fires when files are selected
- Maximum file size should be configured based on your server's capabilities
- For Blazor WebAssembly, file data is read in the browser before being sent to the server

## Web Forms Features NOT Supported

- **PostedFile.SaveAs with HttpContext** — Blazor's `SaveAs` works directly with `IBrowserFile` streams; there is no `HttpContext`-based file handling
- **Server.MapPath** — Use absolute paths or `IWebHostEnvironment.WebRootPath` in Blazor
- **Request.Files collection** — Use the component's `GetMultipleFiles()` method instead
- **Lifecycle events** (`OnDataBinding`, `OnInit`, etc.) — Use Blazor lifecycle methods instead
- Direct postback behavior - use event handlers instead
- Automatic form submission - implement form handling in Blazor
- Server-side file system access in WebAssembly - must send to API endpoint

## Web Forms Declarative Syntax

```html
<asp:FileUpload
<asp:FileUpload
    AccessKey="string"
    AllowMultiple="True|False"
    BackColor="color name|#dddddd"
    BorderColor="color name|#dddddd"
    BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|Groove|Ridge|Inset|Outset"
    BorderWidth="size"
    CssClass="string"
    Enabled="True|False"
    Height="size"
    ID="string"
    ToolTip="string"
    Visible="True|False"
    Width="size"
    runat="server" />
```

## Blazor Razor Syntax

### Basic File Upload

```razor
<FileUpload OnFileSelected="HandleFileSelected" />

@code {
    void HandleFileSelected(InputFileChangeEventArgs args)
    {
        // File has been selected
    }
}
```

### File Upload with Type Restriction

```razor
<FileUpload Accept=".jpg,.png,.gif"
            OnFileSelected="HandleImageSelected" />
```

### Multiple File Upload

```razor
<FileUpload AllowMultiple="true"
            OnFileSelected="HandleFilesSelected" />
```

### File Upload with Increased Size Limit

```razor
<FileUpload MaxFileSize="10485760"
            OnFileSelected="HandleLargeFile" />

@code {
    async Task HandleLargeFile(InputFileChangeEventArgs args)
    {
        // MaxFileSize is set to 10 MB
    }
}
```

### Saving an Uploaded File

```razor
@inject IWebHostEnvironment Environment

<FileUpload @ref="uploader" OnFileSelected="HandleFile" />
<Button Text="Upload" OnClick="SaveFile" />

@code {
    private FileUpload uploader;

    void HandleFile(InputFileChangeEventArgs args)
    {
        // File selected, ready to save
    }

    async Task SaveFile()
    {
        if (uploader.HasFile)
        {
            var path = Path.Combine(Environment.WebRootPath, "uploads", uploader.FileName);
            await uploader.SaveAs(path);
        }
    }
}
```

### Using PostedFile (Web Forms Pattern)

```razor
<FileUpload @ref="uploader" OnFileSelected="HandleFile" />

@code {
    private FileUpload uploader;

    void HandleFile(InputFileChangeEventArgs args)
    {
        if (uploader.HasFile)
        {
            var postedFile = uploader.PostedFile;
            var fileName = postedFile.FileName;
            var contentType = postedFile.ContentType;
            var size = postedFile.ContentLength;
        }
    }
}
```

### Multiple File Save

```razor
<FileUpload @ref="uploader"
            AllowMultiple="true"
            OnFileSelected="HandleFiles" />
<Button Text="Save All" OnClick="SaveAllFiles" />

@code {
    private FileUpload uploader;

    void HandleFiles(InputFileChangeEventArgs args) { }

    async Task SaveAllFiles()
    {
        if (uploader.HasFile)
        {
            var savedPaths = await uploader.SaveAllFiles("C:\\uploads");
            // savedPaths contains the full path of each saved file
        }
    }
}
```

## HTML Output

**Blazor Input:**
```razor
<FileUpload CssClass="file-input" Accept=".pdf,.doc" AllowMultiple="true" />
```

**Rendered HTML:**
```html
<input type="file" multiple="true" accept=".pdf,.doc" class="file-input" />
```

## PostedFileWrapper Reference

The `PostedFile` property returns a `PostedFileWrapper` object that mirrors the Web Forms `HttpPostedFile` API:

| Property/Method | Type | Description |
|-----------------|------|-------------|
| `ContentLength` | `long` | Size of the uploaded file in bytes |
| `ContentType` | `string` | MIME content type of the file |
| `FileName` | `string` | Name of the uploaded file |
| `InputStream` | `Stream` | Stream pointing to the file content |
| `SaveAs(filename)` | `Task` | Saves the file to the specified path |

## Security Considerations

- Always validate file types on the server side, not just through the `Accept` attribute
- Set appropriate `MaxFileSize` limits to prevent denial-of-service attacks
- Sanitize file names before saving to prevent directory traversal attacks
- Scan uploaded files for malware before processing
- Store uploaded files outside of the web root when possible
- Implement authentication and authorization for file upload endpoints

## Migration Notes

When migrating from Web Forms to Blazor:

1. **Remove `asp:` prefix** — Change `<asp:FileUpload>` to `<FileUpload>`
2. **Remove `runat="server"`** — Not needed in Blazor
3. **Remove `ID` attribute** — Use `@ref` to get a component reference
4. **Replace `PostBack` button pattern** — In Web Forms, a separate Button triggered the upload via PostBack. In Blazor, use the `OnFileSelected` event or a Button with `@ref` to call `SaveAs`
5. **Replace `Server.MapPath`** — Use `IWebHostEnvironment.WebRootPath` or `ContentRootPath` for server paths
6. **Use async methods** — Prefer `GetFileBytesAsync()` over `FileBytes` for better performance

### Before (Web Forms)

```html
<asp:FileUpload ID="fileUpload1" runat="server" />
<asp:Button ID="btnUpload" Text="Upload" OnClick="btnUpload_Click" runat="server" />
<asp:Label ID="lblStatus" runat="server" />
```

```csharp
protected void btnUpload_Click(object sender, EventArgs e)
{
    if (fileUpload1.HasFile)
    {
        string fileName = fileUpload1.FileName;
        string savePath = Server.MapPath("~/uploads/") + fileName;
        fileUpload1.SaveAs(savePath);
        lblStatus.Text = "File uploaded: " + fileName;
    }
}
```

### After (Blazor)

```razor
@inject IWebHostEnvironment Environment

<FileUpload @ref="fileUpload" OnFileSelected="HandleFile" />
<Button Text="Upload" OnClick="Upload" />
<Label Text="@statusMessage" />

@code {
    private FileUpload fileUpload;
    private string statusMessage = "";

    void HandleFile(InputFileChangeEventArgs args) { }

    async Task Upload()
    {
        if (fileUpload.HasFile)
        {
            var fileName = fileUpload.FileName;
            var savePath = Path.Combine(Environment.WebRootPath, "uploads", fileName);
            await fileUpload.SaveAs(savePath);
            statusMessage = "File uploaded: " + fileName;
        }
    }
}
```

!!! warning "Security Consideration"
    Always validate uploaded files on the server side. The `Accept` attribute only provides client-side filtering and can be bypassed. Validate file extensions, content types, and file sizes before saving. The `SaveAllFiles` method sanitizes filenames using `Path.GetFileName` to prevent directory traversal attacks, but additional validation is recommended.

!!! note "File Size Limits"
    The default `MaxFileSize` is 512,000 bytes (~500 KiB). For larger files, increase this value. Be mindful that large files consume memory, especially when using `FileBytes` or `GetFileBytesAsync()`. For very large files, work with the `FileContent` stream directly.

## See Also

- [Microsoft Docs: FileUpload Control](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.fileupload?view=netframework-4.8)
- [Blazor File Uploads](https://docs.microsoft.com/en-us/aspnet/core/blazor/file-uploads)
