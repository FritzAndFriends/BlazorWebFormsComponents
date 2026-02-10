# FileUpload

The **FileUpload** component allows users to select files from their local file system for upload to the server. It emulates the ASP.NET Web Forms FileUpload control with similar properties and behavior.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.fileupload?view=netframework-4.8

## Features Supported in Blazor

- `HasFile` - indicates whether a file has been selected
- `FileName` - gets the name of the selected file
- `FileBytes` - gets the file contents as a byte array
- `FileContent` - gets a Stream to read the file data
- `PostedFile` - gets a wrapper object compatible with HttpPostedFile
- `AllowMultiple` - allows selection of multiple files
- `Accept` - specifies file type restrictions (e.g., "image/*", ".pdf,.doc")
- `MaxFileSize` - sets maximum allowed file size in bytes (default: 500KB)
- `SaveAs(path)` - saves the uploaded file to a specified server path
- `GetMultipleFiles()` - retrieves all selected files when AllowMultiple is true
- `SaveAllFiles(directory)` - saves all selected files to a directory
- `Enabled` - enables or disables the control
- `Visible` - controls visibility
- `ToolTip` - tooltip text on hover
- All style properties (`BackColor`, `ForeColor`, `BorderColor`, `BorderStyle`, `BorderWidth`, `CssClass`, `Width`, `Height`, `Font`)

### Blazor Notes

- The control renders as a standard HTML `<input type="file">` element
- File processing must be handled through component properties or methods
- The `OnFileSelected` event fires when files are selected
- Maximum file size should be configured based on your server's capabilities
- For Blazor WebAssembly, file data is read in the browser before being sent to the server

## Web Forms Features NOT Supported

- Direct postback behavior - use event handlers instead
- Automatic form submission - implement form handling in Blazor
- Server-side file system access in WebAssembly - must send to API endpoint

## Web Forms Declarative Syntax

```html
<asp:FileUpload
    ID="FileUpload1"
    AllowMultiple="True|False"
    Enabled="True|False"
    BackColor="color name|#dddddd"
    BorderColor="color name|#dddddd"
    BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|Groove|Ridge|Inset|Outset"
    BorderWidth="size"
    CssClass="string"
    Height="size"
    Width="size"
    ToolTip="string"
    Visible="True|False"
    runat="server" />
```

## Blazor Syntax

```razor
<FileUpload @ref="myFileUpload" 
            Accept="image/*"
            AllowMultiple="false"
            MaxFileSize="1048576" 
            Width="Unit.Pixel(300)" />

<Button Text="Upload" OnClick="HandleUpload" />

@code {
    FileUpload myFileUpload;

    async Task HandleUpload()
    {
        if (myFileUpload.HasFile)
        {
            string fileName = myFileUpload.FileName;
            await myFileUpload.SaveAs($"uploads/{fileName}");
        }
    }
}
```

## Common Usage Patterns

### Single File Upload

```razor
<FileUpload @ref="fileControl" Accept=".pdf" />
<Button Text="Upload PDF" OnClick="UploadPdf" />

@code {
    FileUpload fileControl;

    async Task UploadPdf()
    {
        if (fileControl.HasFile)
        {
            await fileControl.SaveAs($"documents/{fileControl.FileName}");
        }
    }
}
```

### Multiple File Upload

```razor
<FileUpload @ref="fileControl" AllowMultiple="true" />
<Button Text="Upload All" OnClick="UploadAll" />

@code {
    FileUpload fileControl;

    async Task UploadAll()
    {
        if (fileControl.HasFile)
        {
            var savedPaths = await fileControl.SaveAllFiles("uploads");
            // Process savedPaths list
        }
    }
}
```

### Image Upload with Preview

```razor
<FileUpload @ref="imageUpload" Accept="image/*" MaxFileSize="5242880" />

@code {
    FileUpload imageUpload;

    void HandleImageSelected()
    {
        if (imageUpload.HasFile)
        {
            byte[] imageData = imageUpload.FileBytes;
            // Process image data
        }
    }
}
```

## Security Considerations

- Always validate file types on the server side, not just through the `Accept` attribute
- Set appropriate `MaxFileSize` limits to prevent denial-of-service attacks
- Sanitize file names before saving to prevent directory traversal attacks
- Scan uploaded files for malware before processing
- Store uploaded files outside of the web root when possible
- Implement authentication and authorization for file upload endpoints

## Migration from Web Forms

When migrating from Web Forms FileUpload:

1. Replace `<asp:FileUpload>` with `<FileUpload>` (remove `asp:` prefix and `runat="server"`)
2. Replace `FileUpload1.HasFile` with direct property access (no change needed)
3. Replace `FileUpload1.SaveAs(Server.MapPath("~/path"))` with `await FileUpload1.SaveAs(path)`
4. Handle file uploads asynchronously using `async/await` pattern
5. Use `@ref` instead of `ID` to reference the component in code

### Before (Web Forms)

```aspx
<asp:FileUpload ID="FileUpload1" runat="server" />
<asp:Button ID="UploadButton" Text="Upload" OnClick="UploadButton_Click" runat="server" />

// Code-behind
protected void UploadButton_Click(object sender, EventArgs e)
{
    if (FileUpload1.HasFile)
    {
        FileUpload1.SaveAs(Server.MapPath("~/uploads/" + FileUpload1.FileName));
    }
}
```

### After (Blazor)

```razor
<FileUpload @ref="FileUpload1" />
<Button Text="Upload" OnClick="UploadButton_Click" />

@code {
    FileUpload FileUpload1;

    async Task UploadButton_Click()
    {
        if (FileUpload1.HasFile)
        {
            await FileUpload1.SaveAs($"uploads/{FileUpload1.FileName}");
        }
    }
}
```
