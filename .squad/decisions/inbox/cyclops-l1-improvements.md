### L1 Script ~60% Automation — Enum/Bool/Unit/Redirect/Session/ViewState/DataSource Transforms

**By:** Cyclops (Component Dev)

**What:** Added 5 new transformation categories to `bwfc-migrate.ps1`, increasing L1 automation coverage from ~40% to ~60%. New transforms: boolean normalization, enum type-qualifying (18 attributes), unit px-stripping, Response.Redirect→NavigationManager.NavigateTo, Session/ViewState detection with migration guidance, DataSourceID/data source control replacement.

**Technical decisions:**
- **Enum values use `@EnumType.Value` syntax** (e.g., `GridLines="@GridLines.Both"`) so Razor evaluates the C# enum directly. This bypasses EnumParameter<T> string parsing and catches typos at compile time.
- **Only unambiguous attribute→enum mappings** are included. SelectionMode (ambiguous: CalendarSelectionMode vs ListSelectionMode) and Mode (ambiguous across many controls) are skipped to avoid incorrect transforms.
- **Response.Redirect preserves .aspx in URLs** — AspxRewriteMiddleware handles rewriting at runtime. Only `~/` prefix is stripped since it's a Web Forms-only construct.
- **ViewState and Session are detected, not auto-converted** — actual conversion to fields/services is L2 work. L1 inserts structured migration guidance with specific key names and suggested field declarations.
- **Data source control regex uses `(?s)` single-line mode** to handle multi-line tags containing `<%$ ... %>` expressions.

**Why this matters:** Reduces manual work for developers migrating Web Forms apps. The enum/bool/unit transforms eliminate common Razor compilation errors. The code-behind transforms (redirect, session, viewstate) provide actionable guidance that accelerates L2 conversion.
