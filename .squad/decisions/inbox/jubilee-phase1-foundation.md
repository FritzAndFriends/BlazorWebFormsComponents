# Decision: DepartmentPortal Phase 1 Foundation Conventions

**Author:** Jubilee (Sample Writer)
**Date:** 2026-03-20
**Status:** Implemented

## Context
Phase 1 of the ASCX Sample Milestone — creating the DepartmentPortal Web Forms project foundation.

## Decisions Made

1. **No .designer.cs files** — Typed field declarations (`protected Label foo;`) go directly in code-behind partial classes. Simpler and avoids generated file churn.

2. **CodeFile directive** (not CodeBehind) in .aspx/.master directives — matches BeforeWebForms convention.

3. **packages.config format** with NuGet restore to repo-root `packages/` directory. CodeDom .props import path: `..\..\packages\Microsoft.CodeDom.Providers.DotNetCompilerPlatform.2.0.1\build\net46\...`

4. **Static in-memory data** via `PortalDataProvider` — no EF, no database. 5 departments, 20 employees, 10 announcements, 15 courses, 20 resources.

5. **Bootstrap 3 via CDN** in Site.Master `<head>` — no NuGet package for Bootstrap/jQuery.

6. **App_Code/** for base classes — included as `<Compile>` items in .csproj (Web Application Project style, not Web Site).

## Impact
All future DepartmentPortal phases should follow these patterns. ASCX controls (Phase 2+) should inherit from `BaseUserControl`. Authenticated pages inherit from `BasePage`.
