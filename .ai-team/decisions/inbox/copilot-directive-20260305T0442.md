### 2026-03-05: User directive  BWFC library usage is migration top priority
**By:** Jeff Fritz (via Copilot)
**What:** Migration skills must prioritize using BWFC components and utility features above all else. Every asp: control MUST become a BWFC component. Utility features (WebFormsPageBase, FontInfo, theming, etc.) must be preferred over raw Blazor equivalents. Standard Blazor server-side interactive features should be used for static files, CSS links, and JS references (UseStaticFiles, MapStaticAssets, HeadContent, etc.). This ensures the quickest and highest fidelity migration.
**Why:** User request  captured for team memory. Runs 6-8 all suffered from agents replacing BWFC components with raw HTML. Making BWFC usage the #1 priority prevents this regression.
