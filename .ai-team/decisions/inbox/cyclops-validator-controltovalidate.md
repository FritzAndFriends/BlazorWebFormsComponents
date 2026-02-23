# Decision: Validator ControlToValidate String ID Support (WI-36)

**Date:** 2026-02-24
**By:** Cyclops
**Status:** Implemented

## What

Renamed the existing `ForwardRef<InputBase<Type>> ControlToValidate` parameter to `ControlRef` on `BaseValidator<Type>`, and added a new `[Parameter] public string ControlToValidate` parameter that accepts a string ID matching the Web Forms migration pattern `ControlToValidate="TextBox1"`.

## Why

In ASP.NET Web Forms, every validator uses `ControlToValidate="TextBoxID"` with a string control ID. The previous Blazor implementation required `ForwardRef<InputBase<Type>>` which doesn't match the "paste your markup and it works" migration story. This was identified as a migration-blocking API mismatch affecting all 5 input validators.

## How It Works

- **ControlToValidate (string):** Maps to a property/field name on the `EditContext.Model`. The validator uses `CurrentEditContext.Field(name)` for the field identifier and resolves the value via reflection on the model object. No JS interop needed.
- **ControlRef (ForwardRef):** The Blazor-native alternative. Uses the existing `ValueExpression.Body` → `MemberExpression` path and reads `CurrentValueAsString` from `InputBase<Type>` via reflection.
- **Precedence:** When both are set, `ControlRef` takes precedence.
- **Error handling:** Throws `InvalidOperationException` if neither is set.

## Impact

- `BaseValidator.razor.cs`: Core dual-path logic added (`GetFieldName()`, `GetCurrentValueAsString(fieldName)`)
- 38 test `.razor` files: `ControlToValidate=` → `ControlRef=`
- 8 sample `.razor` files: `ControlToValidate=` → `ControlRef=`
- `Login.razor`: `ControlToValidate=` → `ControlRef=`
- All 5 validators (RequiredFieldValidator, CompareValidator, RangeValidator, RegularExpressionValidator, CustomValidator) inherit dual-path support automatically through `BaseValidator<Type>`.

## Breaking Change

This is a parameter rename: existing code using `ControlToValidate="@someForwardRef"` must change to `ControlRef="@someForwardRef"`. This is intentional — the string `ControlToValidate` parameter is the Web Forms-compatible API, while `ControlRef` is the Blazor-native alternative.
