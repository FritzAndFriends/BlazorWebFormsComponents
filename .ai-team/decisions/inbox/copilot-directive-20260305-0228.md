### 2026-03-05: User directive  Always preserve asp: controls as BWFC components
**By:** Jeffrey T. Fritz (via Copilot)
**What:** Migration must ALWAYS preserve the default asp: controls by using BWFC components. Never flatten GridView, ListView, BoundField, TemplateField, TextBox, CheckBox, Button, Label, or any other asp: control to raw HTML. The migration output must use the BWFC component equivalents.
**Why:** User request  this is the core value proposition of the library. Flattening to raw HTML defeats the migration story.