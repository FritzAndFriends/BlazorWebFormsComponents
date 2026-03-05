### 2026-03-05: BWFC control preservation is mandatory in all migration output

**By:** Cyclops

**What:** All migration output (Run samples AND AfterWingtipToys reference) MUST use BWFC components (GridView, BoundField, TemplateField, TextBox, CheckBox, Button, Label) wherever the original Web Forms used asp: controls. Plain HTML replacements are NOT acceptable — they defeat the library's purpose.

**Why:** Jeff was furious that ShoppingCart used a plain HTML table instead of GridView. The AfterWingtipToys reference had the same bug, causing it to propagate to Run 8. Fixed in both.
