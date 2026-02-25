# Component Fixes: LoginView and PasswordRecovery → BaseStyledComponent

**Date:** 2026-02-25
**Author:** Cyclops
**Issues:** #352, #354

## Decision

LoginView and PasswordRecovery now inherit from `BaseStyledComponent` instead of `BaseWebFormsComponent`, matching Login, ChangePassword, and CreateUserWizard.

## Details

- **PasswordRecovery** renders `class="@CssClass"`, `style="border-collapse:collapse;@Style"`, and `title="@ToolTip"` on all three step `<table>` elements — same pattern as Login and ChangePassword.
- **LoginView** has no wrapper HTML element (it's a template-switching component). The base class change makes CssClass/Style/ToolTip properties available as parameters, but they aren't rendered to any element. This is consistent with Web Forms LoginView behavior where the outer element is optional.

## Impact

All login controls now consistently inherit from BaseStyledComponent. No breaking changes — all 1200+ tests pass.
