# Decision: ASPX Middleware Phase 1 Architecture

**Date:** 2026-03-19  
**Author:** Cyclops  
**Status:** Experimental  

## Context

Jeff requested an experiment: middleware to parse .aspx files at runtime and render them via BWFC Blazor components using HtmlRenderer SSR.

## Decision

Built `BlazorWebFormsComponents.AspxMiddleware` with this pipeline:

1. **Parser** — Regex pre-process (`asp:` → `asp_`, `<% %>` → XML comments) then XDocument.Parse
2. **Registry** — 47 BWFC components mapped (generic data controls use `<object>` type arg)
3. **TreeBuilder** — Walks AST, builds RenderFragment via RenderTreeBuilder with reflection-based attribute coercion
4. **Middleware** — Intercepts `.aspx` requests, runs pipeline, returns HTML via HtmlRenderer

## Rationale

- Pre-processing asp: prefix avoids XML namespace issues (simplest approach)
- `typeof(GridView<object>)` for generic controls enables Phase 1 markup rendering without data
- NoOpJsRuntime needed for SSR since BWFC components inject IJSRuntime

## Impact

New experimental project. Does not affect existing library or tests.
