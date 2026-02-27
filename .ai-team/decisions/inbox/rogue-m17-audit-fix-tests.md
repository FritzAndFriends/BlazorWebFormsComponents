### M17 Audit Fix Test Coverage

**By:** Rogue
**What:** Added 9 bUnit tests covering all 5 M17 audit fixes (EnablePartialRendering default, Scripts collection, CssClass rendering, display:block;visibility:hidden, ScriptReference properties). Updated 2 existing tests to match new behavior. All 29 ScriptManager/UpdateProgress tests pass.
**Why:** Audit fixes change observable behavior — tests must be updated to assert the corrected defaults and new properties. ScriptReference defaults tested via plain C# instantiation (no render needed). UpdateProgress CssClass tested both with and without value to ensure no spurious `class=""` attribute.
