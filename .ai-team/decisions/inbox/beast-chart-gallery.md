### Chart Type Gallery documentation convention

**By:** Beast
**What:** Added a "Chart Type Gallery" section to `docs/DataControls/Chart.md` showing screenshots of all 8 Phase 1 chart types. Each entry includes: an `### H3` heading, an MkDocs image (`![alt](../images/chart/chart-{type}.png)`), the `SeriesChartType` enum value, and a 1-2 sentence description of when to use that chart type. Pie and Doughnut entries include `!!! warning "Palette Limitation"` admonitions documenting the Phase 1 single-color-per-series issue.
**Why:** Visual documentation helps migrating developers choose the correct `SeriesChartType` value and understand what each chart type looks like in the Blazor implementation. The palette limitation warnings set expectations and prevent bug reports for known Phase 1 behavior. Image path convention: `docs/images/{component}/` with `chart-{type}.png` naming.
