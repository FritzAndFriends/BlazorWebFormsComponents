### ModelErrorMessage integration test coverage added

**By:** Colossus
**What:** Added 1 smoke test `[InlineData]` and 3 interactive tests for the ModelErrorMessage sample page (`/ControlSamples/ModelErrorMessage`). Smoke test added to `ValidationControl_Loads_WithoutErrors` Theory group. Interactive tests cover: submit-shows-errors, valid-submit-no-errors, and clear-button-removes-errors.
**Why:** Every sample page gets an integration test — no exceptions. The ModelErrorMessage component is a validation control that conditionally renders `<span class="text-danger">` elements, so tests verify both the error-present and error-absent states via DOM element counting. The Clear button test exercises the EditContext reset path, which is unique to this sample page.
