### 2026-03-22T16-22-17Z: User directive
**By:** Jeffrey T. Fritz (via Copilot)
**What:** Devise a strategy to extract CSS and JS content from NuGet packages that use the old ASP.NET Web Forms BundleConfig pattern, and place them in wwwroot/ during migration. This should handle the gap where Web Forms apps reference static assets via NuGet packages (e.g., jQuery, Bootstrap) that get unpacked into Content/Scripts folders, and those references disappear when migrating to Blazor.
**Why:** User request  captured for team memory. This is a real migration gap discovered during the DepartmentPortal migration: the CSS was identical but nobody thought to copy it because the delivery mechanism (NuGet + BundleConfig) is completely different in Blazor (wwwroot + CDN/libman).
