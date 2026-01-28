# JavaScript Setup

BlazorWebFormsComponents requires a small JavaScript file to handle certain Web Forms features like `OnClientClick` event handling and page title management. This document explains the different ways to include this JavaScript in your application.

## Options for Including JavaScript

There are three ways to include the required JavaScript, listed from most recommended to least:

### Option 1: Service Registration (Recommended)

The easiest and most modern approach is to register the BlazorWebFormsComponents service in your `Program.cs`. This automatically handles JavaScript loading with no manual script tags required.

```csharp
// Program.cs
builder.Services.AddBlazorWebFormsComponents();
```

That's it! The JavaScript module is automatically imported on first component render using ES module dynamic imports.

!!! tip "Best for new projects"
    This approach is recommended for new projects or when you have control over the application's service configuration.

### Option 2: Script Loader Component

Add the `<BlazorWebFormsScripts />` component once in your layout or `App.razor`:

```razor
@* In App.razor or your layout *@
<body>
    <Routes @rendermode="InteractiveServer" />

    <script src="_framework/blazor.web.js"></script>
    <BlazorWebFormsScripts />
</body>
```

This component automatically loads the JavaScript module when rendered.

### Option 3: HeadContent Component

Use the `<BlazorWebFormsHead />` component to inject the script tag into the page head:

```razor
@* In any component or layout *@
<BlazorWebFormsHead />
```

This uses Blazor's `<HeadContent>` to add the script tag to the document head.

### Option 4: Manual Script Tag (Legacy)

You can manually add the script tag to your layout. This is the traditional approach and still works:

```html
<!-- In App.razor or _Host.cshtml -->
<body>
    <!-- Your content -->

    <script src="_framework/blazor.web.js"></script>
    <script src="_content/Fritz.BlazorWebFormsComponents/js/Basepage.js"></script>
</body>
```

!!! warning "Placement matters"
    The script tag must be placed **after** the Blazor framework script (`blazor.web.js` or `blazor.server.js`).

## What the JavaScript Does

The JavaScript file provides:

1. **OnClientClick Support** - Handles the `OnClientClick` attribute on buttons and other controls, allowing client-side JavaScript to run before server events
2. **Page Title Management** - Provides functions to get and set the page title programmatically
3. **Post-Render Initialization** - Sets up event handlers for Web Forms compatibility features after Blazor renders

## Choosing the Right Option

| Scenario | Recommended Option |
|----------|-------------------|
| New projects | Option 1: Service Registration |
| Existing projects with layout access | Option 2 or 3: Component-based |
| Projects with strict CSP policies | Option 1: Service Registration (uses ES modules) |
| Legacy projects or minimal changes | Option 4: Manual Script Tag |

## Troubleshooting

### Script Not Loading

If you see JavaScript errors related to `bwfc.Page` being undefined:

1. Verify the script is included using one of the methods above
2. Check browser developer tools Network tab to ensure the script is loaded
3. Ensure the script is placed after the Blazor framework script

### OnClientClick Not Working

The `OnClientClick` functionality requires the JavaScript to be loaded. If buttons with `OnClientClick` aren't executing client-side code:

1. Confirm JavaScript is loaded (check browser console)
2. Ensure the component has rendered at least once (the initialization runs on first render)

## Migration Note

When migrating from Web Forms, you don't need to change how `OnClientClick` works in your markup. The BlazorWebFormsComponents library handles the translation automatically:

```razor
@* This works the same as in Web Forms *@
<Button Text="Submit" OnClientClick="return confirm('Are you sure?');" OnClick="HandleSubmit" />
```
