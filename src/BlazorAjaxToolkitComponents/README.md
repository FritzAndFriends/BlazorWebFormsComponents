# BlazorAjaxToolkitComponents

Blazor components emulating the [ASP.NET Ajax Control Toolkit](https://github.com/DevExpress/AjaxControlToolkit) controls.

## Overview

This library extends [BlazorWebFormsComponents](../BlazorWebFormsComponents/) to provide Blazor equivalents of the popular Ajax Control Toolkit extenders and controls, enabling migration from Web Forms applications that depend on the toolkit.

## Getting Started

```xml
<PackageReference Include="BlazorAjaxToolkitComponents" Version="*" />
```

Add the namespace to your `_Imports.razor`:

```razor
@using BlazorAjaxToolkitComponents
```

## Architecture

- **BaseExtenderComponent** — Base class for extender controls that attach behavior to a target control via `TargetControlID`.
- Components use JS interop for client-side behaviors that mirror the original toolkit's JavaScript.

## Status

🚧 Project structure created. Component implementation in progress.
