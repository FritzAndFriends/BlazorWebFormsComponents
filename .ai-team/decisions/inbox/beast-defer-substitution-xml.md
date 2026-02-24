### Substitution and Xml formally deferred

**By:** Beast
**What:** Substitution and Xml controls are now formally marked as "⏸️ Deferred" (not "Not Started") in status.md. DeferredControls.md already had migration guidance; status.md and README.md now reflect the permanent deferral. Chart is marked as fully complete with no "Phase 1" qualifier.
**Why:** These two controls are architecturally incompatible with Blazor (Substitution relies on output caching; Xml relies on XSLT transforms). Marking them as deferred rather than "Not Started" accurately communicates that they will not be implemented, and clears the remaining work count to 0.
