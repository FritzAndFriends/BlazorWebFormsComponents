### 2026-03-06: User directive — STOP rewriting asp:LoginView as AuthorizeView
**By:** Jeffrey T. Fritz (via Copilot)
**What:** STOP rewriting asp:LoginView as a Blazor AuthorizeView. The BWFC LoginView component exposes templates with the same name as the templates in asp:LoginView (AnonymousTemplate, LoggedInTemplate) and routes them to AuthorizeView internally. The migration script and skills must preserve the BWFC LoginView — not bypass it with native AuthorizeView.
**Why:** User request — the BWFC LoginView component is the correct migration target. Converting to AuthorizeView breaks the component and defeats the purpose of the library.
