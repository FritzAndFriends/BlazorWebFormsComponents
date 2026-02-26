# 2026-02-12 — Boy Scout Test Fixes

**Requested by:** Jeffrey T. Fritz
**Agent:** Colossus
**Branch:** sprint3/detailsview-passwordrecovery
**Commit:** a4d17f5
**PR:** #3

## What happened

Colossus applied the Boy Scout rule and fixed all 7 pre-existing integration test failures:

1. **ChangePassword / CreateUserWizard** — Added missing `@using BlazorWebFormsComponents.LoginControls` to sample pages. Updated test selectors to ID-based (`input[id$='_CurrentPassword']`) for .NET 10 InputText compatibility.
2. **Image / ImageMap** — Replaced all external `via.placeholder.com` URLs with local SVG placeholder images in `wwwroot/img/`.
3. **TreeView/Images** — Fixed broken path `/img/C#.png` → `/img/CSharp.png`.
4. **Calendar** — Added regex filter in `VerifyPageLoadsWithoutErrors` to exclude ASP.NET Core structured log messages (ISO 8601 timestamp prefix) from console error assertions.
5. **ImageMap duplicate InlineData** — Removed duplicate `[InlineData("/ControlSamples/ImageMap")]` from `EditorControl_Loads_WithoutErrors` (ImageMap is a Navigation Control per team decisions).

## Result

- 111/111 integration tests passing
- 797/797 bUnit tests passing
- All tests green

## Decisions made

- ChangePassword and CreateUserWizard sample pages require `@using BlazorWebFormsComponents.LoginControls`
- External placeholder URLs replaced with local SVGs — future samples must never use external image URLs
- ASP.NET Core structured log messages filtered in integration test console error checks
