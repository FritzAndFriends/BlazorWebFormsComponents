# Xml — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.xml?view=netframework-4.8
**Blazor Component:** N/A
**Implementation Status:** 🔴 Not Started

## Overview

The Web Forms `Xml` control displays an XML document without formatting or using Extensible Stylesheet Language Transformations (XSLT). It renders the XML document content (optionally transformed by an XSL stylesheet) directly into the page output. This control inherits from `Control`, not `WebControl`.

## Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| Document | `XmlDocument` | 🔴 Missing | Obsolete in Web Forms (use `XPathNavigator` instead) |
| DocumentContent | `string` | 🔴 Missing | XML content as a string |
| DocumentSource | `string` | 🔴 Missing | Path/URL to XML file |
| Transform | `XslTransform` | 🔴 Missing | Obsolete in Web Forms (use `XslCompiledTransform` instead) |
| TransformArgumentList | `XsltArgumentList` | 🔴 Missing | Arguments for the XSLT transformation |
| TransformSource | `string` | 🔴 Missing | Path/URL to XSLT file |
| XPathNavigator | `XPathNavigator` | 🔴 Missing | XML data as an `XPathNavigator` cursor |
| EnableTheming | `bool` | N/A | Throws `NotSupportedException` in Web Forms |
| SkinID | `string` | N/A | Throws `NotSupportedException` in Web Forms |
| ID | `string` | 🔴 Missing | Control.ID |
| Visible | `bool` | 🔴 Missing | Control.Visible |
| EnableViewState | `bool` | N/A | Server-side only |
| ViewState | `StateBag` | N/A | Server-side only |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| Init | `EventHandler` | 🔴 Missing | Control lifecycle |
| Load | `EventHandler` | 🔴 Missing | Control lifecycle |
| PreRender | `EventHandler` | 🔴 Missing | Control lifecycle |
| Unload | `EventHandler` | 🔴 Missing | Control lifecycle |
| Disposed | `EventHandler` | 🔴 Missing | Control lifecycle |
| DataBinding | `EventHandler` | 🔴 Missing | Control lifecycle |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| DataBind() | `void DataBind()` | N/A | No-op |
| Focus() | `void Focus()` | N/A | Throws `NotSupportedException` in Web Forms |
| FindControl() | `Control FindControl(string)` | 🔴 Missing | Would come from base class |
| GetXmlDocument() | `XmlDocument GetXmlDocument()` | 🔴 Missing | Internal method |

## HTML Output Comparison

**Web Forms** renders the XML content (or XSLT-transformed output) directly as HTML. No wrapper element.

**Blazor** — No implementation exists.

## Implementation Notes

The `Xml` control's primary use cases are:
1. **Display raw XML** — renders XML content directly (browser shows it formatted)
2. **XSLT transformation** — applies an XSL stylesheet to transform XML to HTML

A Blazor implementation would need:

1. `DocumentContent` or `DocumentSource` parameter for XML input
2. `TransformSource` parameter for XSLT stylesheet
3. Server-side XSLT transformation using `System.Xml.Xsl.XslCompiledTransform`
4. Render transformed output as `MarkupString`
5. Base `Control` properties (ID, Visible)
6. Lifecycle events

**Dependencies:** Requires `System.Xml` and `System.Xml.Xsl` — available in .NET but adds XML processing dependency.

**Recommendation:** This control may be permanently deferred. XSLT transformation is a legacy pattern rarely used in modern web apps. Migration guidance should recommend converting XML data to C# objects and using Blazor components/templates for rendering.

## Features That Would Need Implementing

- `DocumentContent` / `DocumentSource` parameters for XML input
- `TransformSource` parameter for XSLT stylesheet
- `Transform` / `TransformArgumentList` for compiled transform with arguments
- `XPathNavigator` property for XPath-based data access
- XSLT transformation engine integration
- Rendered HTML output of transformed content
- Base `Control` properties (ID, Visible)
- Lifecycle events (Init, Load, PreRender, Unload, Disposed)

## Summary

- **Matching:** 0 properties, 0 events
- **Needs Work:** 0
- **Missing:** 9 properties, 6 events (entire control)
- **N/A (server-only):** 4 items
