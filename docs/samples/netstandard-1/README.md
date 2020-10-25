One of the first steps in our [recommended migration strategy](../../Migration/NET-Standard.md) focuses on migrating class libraries from .NET Framework (netfx) 3.5 and later to .NET Standard 2.0+

## Scenario

In this scenario, we have an existing class library called `Library` in use with the ASP<span></span>.NET Web Forms project called `WebProject`.  This library provides simple accounting formulas to the web application and was originally written with .NET Framework 4.5.

The changes prescribed will convert `Library` to .NET Standard 2.0 and update references in `WebProject` to allow it to continue using `Library`.  This change allows `Library` to be referenced by a Blazor or .NET Core project in the future.

### Why does this work?

The code included in this library is simple and references APIs used in the .NET Framework base class library that are all available in .NET Standard 2.0 and lower versions.

## Reference Code

You can find the starting code for this sample in the [start](start) folder and the resultant code in the [completed](completed) folder.

## Steps

