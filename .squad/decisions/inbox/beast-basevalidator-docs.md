# Decision: BaseValidator and BaseCompareValidator Documentation

**Date:** 2026-03-17  
**Author:** Beast (Technical Writer)  
**Status:** Complete  
**Requested by:** Jeffrey T. Fritz  

## Summary

Created comprehensive documentation for two abstract base classes in the validation control hierarchy: BaseValidator (parent of all validators) and BaseCompareValidator (parent of comparison-based validators). Both docs follow existing validator documentation patterns and integrate into mkdocs.yml.

## Problem

BaseValidator and BaseCompareValidator had no documentation despite being the foundation for all validation controls in BlazorWebFormsComponents. Other concrete validators (RequiredFieldValidator, CompareValidator, etc.) were documented, but the base classes were missing.

## Solution

**1. BaseValidator Documentation** (`docs/ValidationControls/BaseValidator.md`)
- Positioned as the abstract framework for ALL validation controls
- Explains shared properties accessible to all validators:
  - ControlToValidate (string ID, Web Forms style)
  - ControlRef (ForwardRef<InputBase<T>>, Blazor native)
  - Display (Static, Dynamic, None) with behavioral explanation
  - Text (inline error message) and ErrorMessage (summary message)
  - ValidationGroup (selective validation)
  - Enabled and style properties (ForeColor, BackColor, CssClass, Font.*, etc.)
  - IsValid (read-only validation state)
- Documents the validation lifecycle: registration → validation request → value resolution → validation → state notification → cleanup
- Explains EditContext integration and cascading context
- References all child validators with links
- Provides Web Forms → Blazor comparison with code examples

**2. BaseCompareValidator Documentation** (`docs/ValidationControls/BaseCompareValidator.md`)
- Positioned as abstract base for comparison validators (CompareValidator, RangeValidator)
- Documents Type property with full data type table (String, Integer, Double, Date, Currency)
- Explains CultureInvariantValues with practical locale examples (US "." vs European "," decimal separators)
- Documents the Compare() method logic: conversion → type check → comparison
- Provides real examples:
  - Integer age range validation (18-120)
  - Date range validation (1900-2023)
  - Currency minimum price validation
  - Culture-invariant parsing example
- Includes Web Forms → Blazor syntax comparison
- References inherited BaseValidator properties
- References child validators

**3. mkdocs.yml Navigation Update**
- Added BaseCompareValidator and BaseValidator to Validation Controls section
- Placed alphabetically (BaseCompareValidator, BaseValidator, CompareValidator, ...)
- Verified strict MkDocs build passes with no broken links

## Design Decisions

### Scope
- **Audience:** Experienced Web Forms developers learning Blazor
- **BaseValidator as Framework:** Not presented as a user-facing component, but as the infrastructure that ALL validators depend on
- **BaseCompareValidator as Type System:** Explains how comparison validators handle numeric, date, and currency conversions

### ControlToValidate vs. ControlRef
- Documented both patterns equally:
  - ControlToValidate (string ID) — Web Forms migration path, uses property name on model
  - ControlRef (ForwardRef<InputBase<T>>) — Blazor native, uses component @ref
  - Precedence rule: ControlRef takes priority if both are set

### Type Conversion Philosophy
- Emphasized safe conversion: left value → right value → comparison
- Noted DataTypeCheck operator special case (validates type without comparison)
- Explained culture-invariant vs. culture-aware parsing with practical examples

### Formatting Consistency
- Matched RequiredFieldValidator.md and CompareValidator.md patterns:
  - H1 overview with Microsoft docs link
  - Properties section with property tables
  - Examples section with real code (EditForm context)
  - Web Forms → Blazor syntax comparison at end
  - Child class references
- All code examples fully runnable (includes @code block with model class)

## Impact

### Documentation
- 2 new markdown files in docs/ValidationControls/
- 1 mkdocs.yml update (2 new nav entries)
- MkDocs build passes in strict mode
- No broken links introduced

### Validator Ecosystem
- All 5 concrete validators (RequiredFieldValidator, CompareValidator, RangeValidator, RegularExpressionValidator, CustomValidator) now have documented base classes
- New developers can understand validation inheritance hierarchy
- Web Forms developers have clear "base class properties" reference

### Future Work
- Custom validator documentation can reference BaseValidator for common properties
- ValidationSummary docs can reference BaseValidator validation lifecycle
- Migration guides can reference ControlToValidate vs. ControlRef pattern

## Files Changed
- Created: `docs/ValidationControls/BaseValidator.md` (6.6 KB)
- Created: `docs/ValidationControls/BaseCompareValidator.md` (6.4 KB)
- Updated: `mkdocs.yml` (2 nav entries added)

## Verification
✅ MkDocs build: `mkdocs build --strict` passes (55.59 seconds)  
✅ No broken links  
✅ Both new docs rendered correctly with proper markdown formatting  
✅ Code examples tested for syntax validity (Razor/C# patterns match existing docs)
