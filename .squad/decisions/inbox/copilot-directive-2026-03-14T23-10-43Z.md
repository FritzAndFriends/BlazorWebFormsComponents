### 2026-03-14T23-10-43Z: User directive  Deprecation docs revision requirement
**By:** Jeff (via Copilot)
**What:** The deprecation guidance docs (#438) must be revised before merging. Each section that covers a removed/deprecated Web Forms pattern MUST show how BWFC addresses that pattern, making it simple for developers to continue using the familiar API. The current docs just explain what's gone  they need to show the BWFC bridge.
**Why:** User request  the whole point of BWFC is that you DON'T have to abandon the Web Forms API patterns. Docs that just say "this is deprecated, use native Blazor" miss the library's value proposition.
