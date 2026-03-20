# Decision: BWFC013 + BWFC014 Analyzer IDs Reserved

**Author:** Cyclops  
**Date:** 2026-03-18  
**Status:** Implemented

## Context

BWFC013 and BWFC014 diagnostic IDs are now allocated in AnalyzerReleases.Unshipped.md:

- **BWFC013** — ResponseObjectUsageAnalyzer (Response.Write/WriteFile/Clear/Flush/End)
- **BWFC014** — RequestObjectUsageAnalyzer (Request.Form/Cookies/Headers/Files/QueryString/ServerVariables)

## Decision

Next available analyzer ID is **BWFC015**. Both new analyzers follow the established code fix pattern: replace statement with EmptyStatement + TODO comment trivia.

## Impact

AllAnalyzersIntegrationTests now expects 10+ analyzers and validates both new IDs in ExpectedIds. Any future analyzer must use BWFC015 or higher.
