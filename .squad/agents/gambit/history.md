# Gambit — History

## Project Context

- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Owner:** Jeffrey T. Fritz
- **Stack:** C#, Blazor, .NET 10, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **My focus:** JavaScript interop for the BlazorAjaxToolkitComponents companion library (M24)
- **Key challenge:** 12 of 14 Ajax Toolkit controls need JS for positioning, animations, focus trapping, or keystroke handling

## Core Context

- BWFC is mostly pure Blazor — Ajax Toolkit introduces the first significant JS interop requirement
- The extender pattern (TargetControlID) is the defining Ajax Toolkit paradigm — need to design Blazor equivalent
- Existing BWFC infrastructure: WebColor, EnumParameter<T>, Unit, BaseWebFormsComponent, BaseStyledComponent
- JS modules should be collocated with components using Blazor's isolation pattern
- Project uses .NET 10 (net10.0 TFM)

## Learnings

(New agent — learnings will be added as work progresses)
