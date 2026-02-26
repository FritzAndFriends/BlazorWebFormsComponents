### 2026-02-26: User directive — SharedSampleObjects for sample parity
**By:** Jeffrey T. Fritz (via Copilot)
**What:** Use the `samples/SharedSampleObjects/` library to deliver the same data to both Blazor and WebForms samples. This is the mechanism for achieving sample data parity — both sides must consume identical model data from SharedSampleObjects.
**Why:** User request — the #1 blocker for HTML match rates is different sample data between WebForms and Blazor. SharedSampleObjects already exists and is shared by both projects.

### 2026-02-26: User directive — Unified legacy wrapper component
**By:** Jeffrey T. Fritz (via Copilot)
**What:** Merge NamingContainer and the Skins/Themes wrapper into a single unified wrapper component that provides all legacy Web Forms support (naming container, skins, themes). This component should be placed in the router to wrap all content. One component instead of multiple separate wrappers.
**Why:** User request — simplifies the migration story. Developers add one wrapper and get NamingContainer + Skins/Themes support together.

### 2026-02-26: User directive — Defer Login+Identity to future milestone
**By:** Jeffrey T. Fritz (via Copilot)
**What:** Login controls + Blazor Identity integration (D-09) is deferred to a future milestone. Do not schedule implementation work for Login, LoginName, LoginStatus, LoginView, ChangePassword, CreateUserWizard, or PasswordRecovery Identity integration now. The analysis at `planning-docs/LOGIN-IDENTITY-ANALYSIS.md` is preserved for when this work is scheduled.
**Why:** User request — wants to delay this work and focus on other priorities first.
