### LoginView uses `<div>` wrapper for BaseStyledComponent styles

**By:** Cyclops
**What:** LoginView renders a `<div class="@CssClass" style="@Style" title="@ToolTip">` wrapper around its content. Unlike Login/ChangePassword/CreateUserWizard which use `<table>` wrappers, LoginView uses `<div>` because it's a template-switching control with no table layout.
**Why:** LoginView already inherited BaseStyledComponent but had no outer HTML element rendering CssClass, Style, or ToolTip. The `<div>` wrapper provides a DOM attachment point for these properties without introducing unnecessary table markup.
