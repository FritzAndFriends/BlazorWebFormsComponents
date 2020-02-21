# Migration Strategies

Migration from ASP<span></span>.NET Web Forms to Blazor is not simple and this repository attempts to make it easier for developers to reuse as much of their Web Forms application as possible.  The two technologies are 'concept compatible', but run on different implementations of the .NET runtime (.NET Framework vs. .NET Core / .NET 5+)

We, the maintainers of this project, believe that with a little ingenuity the markup from a Web Forms application can be copied over with minimal changes and function similarly to its original purpose.  We believe that well formatted and maintained code in Web Forms should be easily migrated.  Applications that are a significant mix of C# and markup will have a more difficult time going through this process.

## Readiness Planning

Migrating an application to Blazor is not a trivial process and it would be great to have some indication ahead of time how much work is needed and what steps you need to take to prepare to migrate.  Check our migration [readiness document](../migration_readiness.md) to help determine how much work will be needed for your application to begin the process.

## Known Required Changes

There are several changes that are going to need to be made to the markup in your ASPX and ASCX files in order to get them working.  Some of these are obvious changes, and some are considerations necessary for the razor templating and Blazor rendering engine.

### Visual Basic converted to C#

Blazor uses razor templates which only support the C# language.  All Visual Basic in the markup will need to be converted to C#.

### Bee-sting notation <% %> needs to be converted to @() notation

ASP<span></span>.NET famously wrapped all .NET code in <% %>.  Starting with ASP<span></span>.NET MVC and razor templates, all C# code is wrapped in @() syntax.

### _Imports files needed

In order to match some of the syntax previously available in Web Forms markup, a series of `using static` statements are necessary to allow the appropriate shims to work with your markup.  You can find our sample of the `_Imports.razor` file here:  **INSERT LINK**

### User Controls (ASCX files) need to be converted to components

Components are the building blocks for Blazor, just as controls were in Web Forms.  If you have user controls in your application, you will need to convert those to components.  See the UserControl strategy below for more details.

### MasterPages are no more

The concept of a MasterPage does not exist in Blazor.  Instead, your ASPX pages will be loaded inside of a host page.  You can compose a razor component that hosts other *converted ASPX pages* but your pages cannot dictate their parent container.  See the [MasterPage strategy](MasterPages.md) below for more details.

### Page Directive Changes

### No <%#: DataBinding expressions 

Databinding expressions in Web Forms let you evaluate the content of the elements and format them appropriately for presentation.  For editor controls, it also allows you to setup a 2-way binding so that you can receive values entered into the same variable bound to the control.

In Blazor, for repeater-style components, just format the variable using context, Item, and simple formatting like this:

```csharp
@Item.ShipDate.ToString("D")
```

For editor components, simply `@bind` the variable to the component.  This will give you two-way data-binding and the ability to handle changes in the editor component AS it changes.

```html
<input type="text" name="foo" @bind="bar" />
```

### No Namespaces, No Tag-Prefixes 

Namespaces and tag-prefixes are gone.  You can do a Find and Replace on `asp:` and remove those from your markup.

### Redirect Color to WebColor

This change should **NOT** require any coding modifications.  In Web Forms, you could refer to `System.Drawing.Color` objects when setting `BackColor`, `BorderColor`, and `ForeColor` to name a few properties.  You could _ALSO_ freely use HTML hex-color notation freely in these fields.  
The `System.Drawing.Color` object does not have a converter that allows you to convert between these two formats, so we wrapped the object and made `BlazorWebFormsComponents.WebColor` that performs the same task and allows the interchange of `System.Drawing.Color` object with HTML hex notation.

## Strategies

- A simple initial site migration
- Intertwined code
- [DataBinder](Databinder.md)
- Model-Binding
- [.NET Standard to the rescue!](NET-Standard.md)
- Other considerations
  - [MasterPage](MasterPages.md)
    - Rearchitecting Web Application Layout
  - UserControls
    - The simple conversion
  - [Custom Controls](Custom-Controls.md)