# Page

[Features implemented](#features-implemented-on-the-page) | [Syntax required in your application](#syntax-required-in-your-application)

In Web Forms, every page inherits from the `System.Web.UI.Page` class.  To emulate the features of the Page object and to provide some connective code between components, we recommend the `aspx` pages that you migrate from Web Forms also inherit from the the `BlazorWebFormsComponents.Page` object.

```razor
@inherits BlazorWebFormsComponents.Page
@page "/About"
```

## Features implemented on the Page

  - `Title` property

##### [Back to top](#page)

## Syntax required in your application

To support the interactions with the browser and the entire HTML page, you need to add the following script reference to the footer of the host page (`Host.cshtml` is the default in the Blazor template)

```html
<script src="_content/Fritz.BlazorWebFormsComponents/js/Basepage.js"></script>
```

##### [Back to top](#page)
