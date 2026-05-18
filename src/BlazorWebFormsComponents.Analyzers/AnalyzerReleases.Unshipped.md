### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
BWFC001 | Usage    | Warning  | MissingParameterAttributeAnalyzer
BWFC002 | Usage    | Info     | ViewStateUsageAnalyzer (was Warning — ViewState now works as migration shim)
BWFC003 | Usage    | Info     | IsPostBackUsageAnalyzer (was Warning — IsPostBack now works via BWFC shim)
BWFC004 | Usage    | Warning  | ResponseRedirectAnalyzer
BWFC005 | Usage    | Warning  | SessionUsageAnalyzer
BWFC010 | Usage    | Info     | RequiredAttributeAnalyzer
BWFC011 | Usage    | Info     | EventHandlerSignatureAnalyzer
BWFC012 | Usage    | Warning  | RunatServerAnalyzer
BWFC013 | Usage    | Warning  | ResponseObjectUsageAnalyzer
BWFC014 | Usage    | Warning  | RequestObjectUsageAnalyzer
BWFC020 | Migration | Info     | ViewStatePropertyPatternAnalyzer
BWFC021 | Migration | Warning  | FindControlUsageAnalyzer
BWFC022 | Migration | Warning  | PageClientScriptUsageAnalyzer (enhanced with method-specific guidance)
BWFC023 | Migration | Warning  | IPostBackEventHandlerUsageAnalyzer (enhanced with EventCallback migration guidance)
BWFC024 | Migration | Warning  | ScriptManagerUsageAnalyzer
BWFC025 | Usage    | Warning  | NonSerializableViewStateAnalyzer
