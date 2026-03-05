# Decision: Layer 1 Enum Attribute Conversion (P2 Candidate)

**By:** Bishop
**Date:** 2025-07-25
**Context:** Run 10 WingtipToys migration benchmark

## What

Layer 1 (`bwfc-migrate.ps1`) should convert string-valued enum attributes to their Blazor enum equivalents. Specifically:

1. `TextMode="Email"` → `TextMode="TextBoxMode.Email"` (and Password, Phone, Number, etc.)
2. `Display="Dynamic"` → `Display="@ValidatorDisplay.Dynamic"` (on validators)
3. `GridLines="Vertical"` → `GridLines="@GridLines.Vertical"` (on GridView/DetailsView)
4. `AutoGenerateColumns="False"` → `AutoGenerateColumns="@false"` (boolean attributes)
5. `ShowFooter="True"` → `ShowFooter="@true"` (boolean attributes)

## Why

In Run 10, 3 of the 47 initial build errors (attempt 1 → 2) were caused by unquoted enum/boolean values. These are mechanical transforms that Layer 1 could handle, reducing Layer 2 effort further. The pattern is consistent: any BWFC component property that takes an enum or boolean should have its string value converted to the proper C# expression.

## Impact

- Would reduce build attempts from 3 to potentially 1-2
- Estimated 10-15 fewer manual fixes per migration run
- Low risk — patterns are well-defined and testable
