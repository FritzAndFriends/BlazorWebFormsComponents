# Decision: ViewState Phase 1 Implementation Details

**Author:** Cyclops  
**Date:** 2026-03-24  
**Status:** Implemented on `feature/viewstate-postback-shim`

## Context

Implemented Phase 1 of Forge's ViewState/PostBack architecture proposal. Key implementation decisions made during build:

## Decisions

### 1. IDataProtectionProvider is nullable/optional
The `[Inject] private IDataProtectionProvider` on BaseWebFormsComponent is null-checked before use. If the consuming app hasn't registered Data Protection services, ViewState serialization for SSR is silently skipped. This preserves backward compatibility — existing apps that don't need SSR ViewState won't break.

### 2. ViewState deserialization order
Deserialization from form POST happens at the TOP of `OnInitializedAsync`, BEFORE `Parent?.Controls.Add(this)`, OnInit, OnLoad, and OnPreRender events. This ensures developer code in those lifecycle methods reads correct ViewState values. Matches Web Forms behavior where ViewState was restored before Page_Load.

### 3. CryptographicException handling
Tampered or expired ViewState payloads result in a caught `CryptographicException` that silently falls back to an empty ViewState. This is fail-safe — the component initializes with default values rather than crashing.

### 4. Hidden field naming convention
`__bwfc_viewstate_{ID}` uses the developer-set `ID` parameter. If no ID is set, the hidden field name is `__bwfc_viewstate_` (empty suffix). Phase 2 should consider a deterministic fallback when ID is null.

### 5. WebFormsPageBase OnInitialized override
Added `OnInitialized` override to set `_hasInitialized = true`. This is safe because WebFormsPageBase didn't previously override `OnInitialized` (it uses `OnInitializedAsync` via derived classes). The override calls `base.OnInitialized()` first.

## Impact
- **Rogue:** Unit tests needed for ViewStateDictionary (serialize/deserialize/type coercion) and IsPostBack (SSR/Interactive modes)
- **Forge:** Phase 2 (SSR hidden field round-trip integration) can proceed
- **Beast:** ViewState docs need update — [Obsolete] removed, new behavior documented
