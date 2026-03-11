# GitHub Issue: Create BlazorAjaxToolkitComponents companion library

**Labels:** enhancement, help wanted

---

## Summary

Create a new companion library **BlazorAjaxToolkitComponents** (or similar name) that provides Blazor component equivalents for the [AJAX Control Toolkit](https://github.com/DevExpress/AjaxControlToolkit) controls, following the same design philosophy as BlazorWebFormsComponents.

## Background

The AJAX Control Toolkit was a popular extension library for ASP.NET Web Forms that provided rich UI controls and behaviors. Many Web Forms applications being migrated to Blazor use these controls extensively. Currently, our migration scripts strip `<ajaxToolkit:*>` tags with TODO comments, leaving developers to manually find replacements.

## Proposed Approach

Following the BWFC pattern:
1. **Same control names** — `<ajaxToolkit:Accordion>` becomes `<Accordion>`
2. **Same attributes** — Property names match the original toolkit
3. **Similar HTML output** — Preserve CSS compatibility where possible
4. **NuGet package** — `Fritz.BlazorAjaxToolkitComponents` or similar

## Priority Controls

Based on real-world migration frequency:

### High Priority
| AJAX Toolkit Control | Notes |
|---------------------|-------|
| `Accordion` / `AccordionPane` | Collapsible panel groups |
| `TabContainer` / `TabPanel` | Tabbed content areas |
| `ModalPopupExtender` | Modal dialogs |
| `CollapsiblePanelExtender` | Single collapsible section |
| `CalendarExtender` | Date picker (may leverage existing BWFC Calendar) |
| `AutoCompleteExtender` | Typeahead/autocomplete textbox |
| `FilteredTextBoxExtender` | Input masking/filtering |

### Medium Priority
| AJAX Toolkit Control | Notes |
|---------------------|-------|
| `ConfirmButtonExtender` | Confirmation dialogs on button click |
| `MaskedEditExtender` | Input masks |
| `NumericUpDownExtender` | Numeric spinner |
| `SliderExtender` | Range slider |
| `ToggleButtonExtender` | Toggle state buttons |
| `PopupControlExtender` | Popup panels |
| `HoverMenuExtender` | Hover-triggered menus |

### Lower Priority
- `AnimationExtender`
- `DragPanelExtender`
- `DropShadowExtender`
- `RoundedCornersExtender` (CSS handles this now)
- `ReorderList`
- `Rating`

## Design Considerations

### Extender Pattern
Many AJAX Toolkit controls are "extenders" that attach behavior to existing controls (e.g., `<ajaxToolkit:CalendarExtender TargetControlID="txtDate" />`). Options:
1. **Wrapper components** — `<CalendarTextBox>` combines TextBox + calendar
2. **Behavior components** — Use Blazor's `@ref` and JS interop to attach behaviors
3. **CSS/JS only** — Some extenders can be pure CSS/JS without Blazor components

### JavaScript Interop
Unlike BWFC (mostly pure Blazor), many AJAX Toolkit features require JavaScript (animations, popups, drag-drop). The library should:
- Minimize JS dependencies
- Use modern CSS where possible (CSS Grid, Flexbox, CSS animations)
- Provide clean JS interop for complex behaviors

## Integration with BWFC

- Separate NuGet package to keep BWFC lightweight
- Share common infrastructure (WebColor, enums, base classes) if appropriate
- Coordinated versioning and documentation

## Migration Script Integration

Once the library exists, update `bwfc-migrate.ps1` to:
1. Recognize `<ajaxToolkit:*>` controls
2. Convert to `<BlazorAjaxToolkit:*>` equivalents (or strip prefix like BWFC)
3. Add appropriate `@using` statements

## Success Criteria

- [ ] NuGet package published
- [ ] Top 7 high-priority controls implemented
- [ ] Migration script recognizes and converts common controls
- [ ] Documentation with migration examples
- [ ] At least one sample app demonstrating migration

## Related

- BWFC migration script: `migration-toolkit/scripts/bwfc-migrate.ps1`
- Current behavior: `ajaxToolkit:` tags are stripped with `@* TODO: AjaxToolkit ... *@` comments
- Original toolkit: https://github.com/DevExpress/AjaxControlToolkit
