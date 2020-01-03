# Migration Strategies

Migration from ASP<span></span>.NET Web Forms to Blazor is not simple and this repository attempts to make it easier for developers to reuse as much of their Web Forms application as possible.  The two technologies are 'concept compatible', but run on different implementations of the .NET runtime (.NET Framework vs. .NET Core / .NET 5+)

We, the maintainers of this project, believe that with a little ingenuity the markup from a Web Forms application can be copied over with minimal changes and function similarly to its original purpose.  We believe that well formatted and maintained code in Web Forms should be easily migrated.  Applications that are a significant mix of C# and markup will have a more difficult time going through this process.

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

The concept of a MasterPage does not exist in Blazor.  Instead, your ASPX pages will be loaded inside of a host page.  You can compose a razor component that hosts other *converted ASPX pages* but your pages cannot dictate their parent container.  See the MasterPage strategy below for more details.

### Page Directive Changes

### No Namespaces, No Tag-Prefixes 

Namespaces and tag-prefixes are gone.  You can do a Find and Replace on asp: and remove those from your markup.

## Strategies

- A simple initial site migration
- Intertwined code
- Model-Binding
- .NET Standard to the rescue!
- Other considerations
  - MasterPage
    - Rearchitecting Web Application Layout
  - UserControls
    - The simple conversion
