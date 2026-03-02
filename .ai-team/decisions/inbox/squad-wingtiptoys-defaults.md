### 2026-03-02: WingtipToys migration architecture decisions (defaults)
**By:** Squad (Coordinator)
**What:** Default architecture decisions for WingtipToys Blazor migration, made while Jeff was unavailable. Jeff can override any of these.
**Why:** Needed to finalize the migration plan with concrete decisions rather than leaving open questions.

**Decisions:**
1. **Database: SQLite** — Simplest for demo, no SQL Server dependency, EF Core supports it natively. Keeps the sample self-contained.
2. **Account pages: Scaffold Identity UI (Razor Pages)** — Standard practice for ASP.NET Core. The 14 Account pages are Identity boilerplate, not BWFC showcase material. Blazor Server + Razor Pages coexist in the same app.
3. **PayPal: Keep NVP API integration** — Port from HttpWebRequest to HttpClient. The checkout flow is realistic and demonstrates external API integration in a migrated app. Keep sandbox mode.
4. **Render mode: Full InteractiveServer** — Simplest migration path. Every page is interactive via Blazor Server circuits. No SSR complexity.

All decisions are overridable by Jeff. These are reasonable defaults, not mandates.
