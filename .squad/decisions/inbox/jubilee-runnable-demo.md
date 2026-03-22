# Decision: AfterDepartmentPortal Runnable Demo Setup

**Date:** 2026-03-23
**Author:** Jubilee (Sample Writer)
**Status:** Implemented

## Context
AfterDepartmentPortal built clean but couldn't render — missing CSS files and no home page.

## Decisions

1. **Bootstrap via CDN** — Used Bootstrap 5.3.3 and Bootstrap Icons from jsdelivr CDN instead of bundling local copies. Keeps the sample lightweight and avoids checking large vendor files into the repo.

2. **Home page at /home, not /** — Dashboard.razor already claimed `@page "/"`. Rather than disrupting the existing route, the new Home.razor welcome page lives at `/home`. The Dashboard *is* the landing page.

3. **SectionPanel CssClass fix** — Removed `new CssClass` property that shadowed the base class `[Parameter]`. Blazor parameters are case-insensitive and must be unique across the inheritance chain. Used `OnInitialized()` to set the default instead.

4. **Site.css copied from DepartmentPortal** — Preserves the same CSS classes used by the before/after migration pair, ensuring visual consistency.
