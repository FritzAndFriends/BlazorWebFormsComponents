# Recipe: MainLayout Body CSS Class

## Error Signature

No compile error — this is a **visual regression**. Pages render but CSS styles from `Site.css` don't apply correctly. Layout looks broken, missing margins/padding, content not centered.

## Detection

Compare the original `Site.Master` wrapper structure with the generated `MainLayout.razor`. Look for missing CSS classes on wrapper `<div>` elements.

```powershell
# Check original master page for wrapper structure
Select-String -Path **/Site.Master -Pattern "class="
# Check generated layout
Select-String -Path **/MainLayout.razor -Pattern "class="
```

## Root Cause

Web Forms Master Pages typically wrap `<asp:ContentPlaceHolder>` in styled `<div>` elements:
```html
<!-- Site.Master -->
<div class="container body-content">
    <asp:ContentPlaceHolder ID="MainContent" runat="server" />
</div>
```

The CLI converts `<asp:ContentPlaceHolder>` to `@Body` but may not preserve the wrapping `<div>` with its CSS classes.

## Fix Pattern

1. **Read the original Master Page** and identify wrapper elements around `ContentPlaceHolder`
2. **Replicate the wrapper structure** in `MainLayout.razor` around `@Body`

```razor
@* Components/Layout/MainLayout.razor *@
@inherits LayoutComponentBase

<div class="container body-content">
    @Body
    <hr />
    <footer>
        <p>&copy; @DateTime.Now.Year - My Application</p>
    </footer>
</div>
```

## Common Patterns

### Bootstrap Container
```razor
<div class="container">
    @Body
</div>
```

### Two-Column Layout
```razor
<div class="row">
    <div class="col-md-3">
        <NavMenu />
    </div>
    <div class="col-md-9">
        @Body
    </div>
</div>
```

### Nested Wrappers
Some Master Pages have multiple levels — preserve all of them:
```razor
<div id="wrapper">
    <div class="container body-content">
        <section id="main">
            @Body
        </section>
    </div>
</div>
```

## Static Assets

Ensure `Site.css` and other stylesheets are referenced in `App.razor`:
```razor
<link rel="stylesheet" href="Content/Site.css" />
```

The CSS file should be in `wwwroot/Content/Site.css` (same relative path as the Web Forms project).

## Verification

After fixing the layout wrapper, visually compare the page against the original Web Forms site. Key things to check:
- Content centering and max-width
- Navigation bar positioning
- Footer placement
- Sidebar layout (if applicable)
