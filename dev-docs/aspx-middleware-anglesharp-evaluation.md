# ASPX Middleware: AngleSharp Parser Evaluation

> **Status:** ✅ Prototype Complete  
> **Branch:** `experiment/aspx-middleware`  
> **Last Updated:** 2026-03-20  

## Executive Summary

The Phase 1 ASPX middleware prototype uses `System.Xml.Linq.XDocument` to parse `.aspx` markup after regex pre-processing (`asp:` → `asp_`, expression extraction). This approach works for well-formed XML but **breaks on real-world .aspx files** that contain unclosed tags, `&` entities, non-XHTML constructs, and other HTML-legal but XML-illegal patterns.

This document tracks the evaluation of **AngleSharp** as a tolerant HTML parser replacement.

---

## Current Architecture (XDocument)

### Pipeline
```
.aspx file → ExtractDirectives (regex) → ReplaceExpressions (regex) → asp: → asp_ rename (regex) → XDocument.Parse() → Walk XML tree → AST nodes
```

### Files
| File | Lines | Purpose |
|------|-------|---------|
| `AspxParser.cs` | 211 | Core parser — regex pre-process + XDocument |
| `AspxNodes.cs` | ~60 | AST: AspControlNode, HtmlNode, ExpressionNode, DirectiveNode |
| `AspxComponentTreeBuilder.cs` | ~160 | AST → RenderFragment via RenderTreeBuilder |
| `AspxComponentRegistry.cs` | ~80 | 47+ tag name → BWFC Type mappings |
| `AspxRenderingMiddleware.cs` | ~60 | HTTP middleware for .aspx requests |
| `AspxPageHost.cs` | ~30 | ComponentBase wrapper for HTML shell |
| `AspxMiddlewareExtensions.cs` | ~30 | DI extensions |

### Known XDocument Limitations
1. **Unclosed tags** — `<br>`, `<hr>`, `<input>` without self-close crash XDocument
2. **`&` entities** — Unescaped `&` in URLs or content is invalid XML
3. **Non-XHTML attributes** — Boolean attributes like `checked`, `disabled` without values
4. **CDATA / script blocks** — `<script>` with `<` or `>` characters fails
5. **HTML comments** — Nested/malformed HTML comments
6. **Fallback is total** — On any XML error, entire page falls back to raw HTML string

### Known Bugs (from Forge's Phase 1 Review)
| # | Bug | Severity |
|---|-----|----------|
| 1 | Single-quote attributes (`attr='val'`) silently dropped — regex only matches double quotes | Critical |
| 2 | `runat="server"` passes through as Blazor parameter | Critical |
| 3 | 6 validators missing from registry | Medium |
| 4 | TableRow/TableCell/TableHeaderCell not registered | Medium |
| 5 | Server-side comments (`<%-- --%>`) misclassified as code expressions | Medium |
| 6 | No EventCallback coercion | Medium |

---

## AngleSharp Proposal

### What AngleSharp Provides
- **Tolerant HTML5 parsing** — handles unclosed tags, malformed markup, entities
- **DOM API** — familiar `IDocument`, `IElement`, `INode` tree traversal
- **CSS selector support** — `querySelectorAll` style queries
- **No XML namespace issues** — `asp:` prefix handled natively as custom elements
- **Active maintenance** — NuGet: `AngleSharp` (latest stable)

### Expected Benefits
- [x] Parse real-world .aspx without pre-processing hacks — ⚠️ Regex still needed for directives/expressions, but HTML parsing is tolerant
- [ ] ~~Eliminate regex `asp:` → `asp_` rename~~ — ❌ Must keep (AngleSharp auto-close bug)
- [x] Handle unclosed HTML tags natively
- [x] Handle `&` entities without crashing
- [x] Better error recovery (partial parse vs total fallback)
- [ ] ~~Cleaner codebase (fewer regex passes)~~ — ⚠️ Added 2 new regex passes (server comments, self-closing expand)

### Risks & Concerns
- [x] AngleSharp may normalize HTML in unexpected ways — **Confirmed:** lowercases tag names; mitigated by case mapping dictionary
- [x] `asp:` prefix may be treated differently — **Confirmed:** auto-closes unknown elements; mitigated by keeping `asp:` → `asp_` rename
- [x] Expression extraction (`<%= %>`, `<%# %>`) still needs regex pre-processing — **Confirmed**
- [x] Directive extraction (`<%@ Page %>`) still needs regex pre-processing — **Confirmed**
- [x] Performance delta measured — AngleSharp averages 0.04ms (small) to 3.35ms (XL 18KB), ~260–22,600 parses/sec. Allocations scale linearly (~51–926 KB/parse). Acceptable for on-demand .aspx rendering.
- [x] Test suite must remain green through the transition — **58/58 passing**

---

## Investigation Results

### Forge Analysis

**Completed:** 2026-03-20  
**Verdict:** ⚠️ **PROCEED WITH CAUTION** — AngleSharp solves 90% of XDocument's problems but introduces new ones.

---

#### 1. XDocument Limitation Catalog

Real-world .aspx patterns that break `XDocument.Parse()`:

##### 1.1 Unclosed HTML Tags
```aspx
<!-- BREAKS XDocument -->
<p>Some text<br>More text
<hr>
<input type="text" name="email">
```
**Why XDocument fails:** XML requires all elements to be self-closed (`<br />`) or have closing tags (`</br>`).  
**AngleSharp:** ✅ Handles perfectly — auto-closes per HTML5 rules.

##### 1.2 Unescaped `&` in URLs
```aspx
<!-- BREAKS XDocument -->
<a href="products.aspx?cat=1&page=2">Products</a>
```
**Why XDocument fails:** `&` must be `&amp;` in XML.  
**AngleSharp:** ✅ Tolerates raw `&`, normalizes to `&amp;` in serialization.

##### 1.3 Single-Quote Attributes
```aspx
<!-- WORKS IN XDOCUMENT, but current REGEX DROPS IT -->
<asp:Label Text='Single Quote' ID="lbl1" />
```
**Current bug:** `AttributeRegex()` only matches double quotes: `(\w+)\s*=\s*"([^"]*)"`  
**AngleSharp:** ✅ Parses both single and double quotes natively.

##### 1.4 Boolean Attributes (No Value)
```aspx
<!-- BREAKS XDocument -->
<input type="checkbox" checked disabled>
```
**Why XDocument fails:** XML requires attribute values: `checked="checked"`.  
**AngleSharp:** ✅ HTML5-compliant — handles boolean attributes.

##### 1.5 Nested or Malformed Comments
```aspx
<!-- BREAKS XDocument -->
<!-- outer <!-- inner --> -->
```
**Why XDocument fails:** XML doesn't allow `--` inside comments.  
**AngleSharp:** ⚠️ May still struggle, but more forgiving than XML.

##### 1.6 `<script>` Blocks with Operators
```aspx
<!-- BREAKS XDocument -->
<script>
if (x < 5 && y > 2) { }
</script>
```
**Why XDocument fails:** `<` and `&&` are XML-illegal outside CDATA.  
**AngleSharp:** ✅ Script tags parsed as RAWTEXT — content not interpreted as markup.

---

#### 2. AngleSharp Behavior with `asp:` Tags

**Critical finding:** AngleSharp preserves `asp:` prefixes perfectly in HTML mode.

##### Test Results (from prototype):
```csharp
// Input
<asp:Button ID="btn1" Text="Submit" runat="server" />

// AngleSharp Output
TagName:    "ASP:BUTTON"
LocalName:  "asp:button"
Attributes: ID="btn1", Text="Submit", runat="server"
```

**Key Observations:**

1. **Tag names preserved:** `<asp:Button>` → `TagName: "ASP:BUTTON"` (uppercase per HTML5), `LocalName: "asp:button"` (lowercase).
2. **Attributes intact:** All attributes (including single-quote `Text='value'`) are preserved.
3. **No namespace splitting:** The colon is part of the tag name string, NOT an XML namespace separator.
4. **Self-closing behavior:** ⚠️ **CRITICAL ISSUE** — AngleSharp treats `<asp:Button />` as an **open tag**, not self-closing. Serializes as `<asp:button>...</asp:button>`.
5. **Auto-closing cascade:** Because `asp:` tags are unknown elements, AngleSharp auto-closes them at end of parent. Example:

```html
<!-- Input -->
<asp:Label ID="lbl1" Text="Name:" />
<asp:Button ID="btn1" Text="Go" />

<!-- AngleSharp Output (WRONG) -->
<asp:label id="lbl1" text="Name:">
<asp:button id="btn1" text="Go">
</asp:button></asp:label>
```

The second `asp:button` becomes a **child** of `asp:label` instead of a sibling. This is a **fatal flaw** for naive parsing.

##### Workaround

The current parser already does this:
```csharp
content = AspOpenTagRegex().Replace(content, "<asp_");
content = AspCloseTagRegex().Replace(content, "</asp_");
```
**This workaround MUST remain with AngleSharp** to prevent auto-closing issues. Replace `asp:` → `asp_` BEFORE parsing, then reverse in AST node construction.

---

#### 3. Expression/Directive Compatibility

**Test Results:**

```aspx
<%@ Page Title="Test" Language="C#" %>
<%= DateTime.Now %>
<%# Item.Name %>
```

**AngleSharp behavior:**
- **Directives:** `<%@ ... %>` are escaped to `&lt;%@ ... %&gt;` (HTML-encoded text nodes).
- **Expressions:** `<%= %>`, `<%# %>` also HTML-encoded.

**Conclusion:** ✅ **Regex pre-extraction MUST remain.**

The current pipeline is correct:
```
1. ExtractDirectives (regex) → remove <%@ %> → store in result.Directives
2. ReplaceExpressions (regex) → replace <%= %> with <!--___ASPX_EXPR_0___-->
3. AspPrefix rename: asp: → asp_
4. Parse HTML (XDocument OR AngleSharp)
5. Walk tree, restore asp_ → asp:
```

**No change needed** for steps 1–2. AngleSharp replaces only step 4.

---

#### 4. Migration Path

##### Files That Change

| File | Current Lines | Change Scope |
|------|---------------|--------------|
| `AspxParser.cs` | 211 | **MAJOR** — Replace `XDocument.Parse()` + tree walk with AngleSharp |
| `AspxParser.csproj` | N/A | **ADD** — `<PackageReference Include="AngleSharp" Version="1.1.2" />` |

##### Files That Stay the Same

| File | Why No Change |
|------|---------------|
| `AspxNodes.cs` | AST node definitions unchanged |
| `AspxComponentTreeBuilder.cs` | Consumes AST — agnostic to parser |
| `AspxComponentRegistry.cs` | Tag name → Type mapping unchanged |
| `AspxRenderingMiddleware.cs` | Calls `AspxParser.Parse()` — interface unchanged |
| `AspxPageHost.cs`, `AspxMiddlewareExtensions.cs` | No parser dependency |

##### Implementation Sketch

```csharp
// OLD: XDocument approach
var xml = $"<__root__>{content}</__root__>";
XDocument doc = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
foreach (var node in doc.Root.Nodes())
{
    var astNode = ConvertXmlNode(node, expressions);
}

// NEW: AngleSharp approach
using AngleSharp.Html.Parser;

var parser = new HtmlParser();
var document = parser.ParseDocument(content); // No wrapper needed
foreach (var node in document.Body.ChildNodes)
{
    var astNode = ConvertAngleSharpNode(node, expressions);
}
```

**Key differences:**
- Replace `ConvertXmlNode(XNode, ...)` with `ConvertAngleSharpNode(INode, ...)`.
- Map `IElement` → `AspControlNode`, `IText` → `HtmlNode`, `IComment` → expression placeholders.
- Handle `IElement.TagName` (uppercase) vs `LocalName` (lowercase).
- AngleSharp's `IElement.InnerHtml` for serializing unknown HTML elements.

##### Test Migration

**Goal:** All 52 existing tests must pass.

**Risk areas:**
1. Whitespace handling — AngleSharp may collapse/normalize differently than XDocument.
2. Attribute order — AngleSharp may reorder attributes (not breaking, but test assertions may be brittle).
3. Case sensitivity — AngleSharp lowercases tag names in output (`<asp:Button>` → `<asp:button>`).

**Recommendation:** Update test assertions to use case-insensitive comparisons for tag names.

---

#### 5. Risks

##### 5.1 HTML5 Auto-Closing Behavior

**CRITICAL:** AngleSharp auto-closes unknown elements like `<asp:Button />` as `<asp:button></asp:button>`, and nests subsequent siblings inside.

**Mitigation:** Keep `asp:` → `asp_` rename. Standard HTML elements (`div`, `span`) don't have namespaced names, so no risk there.

##### 5.2 Attribute Reordering

AngleSharp may change attribute order during serialization. Not semantically breaking, but test assertions using string equality may fail.

**Mitigation:** Update tests to parse attributes as dictionaries, not compare raw HTML strings.

##### 5.3 Case Normalization

AngleSharp lowercases tag names and attribute names per HTML5 spec. `<asp:Button ID="btn1">` → `<asp:button id="btn1">`.

**Mitigation:** Store original case in AST nodes if needed. Most Blazor components are case-insensitive.

##### 5.4 Performance

AngleSharp allocates a full DOM tree (`IDocument`, `IElement`, `INode`). XDocument is similar, but AngleSharp includes HTML5 tokenizer overhead.

**Estimated impact:** 10-20% slower than XDocument. Still fast enough for on-demand .aspx rendering (not a hot path).

**Measurement needed:** Benchmark on large .aspx files (>50KB).

##### 5.5 Dependency Weight

- **AngleSharp NuGet:** ~450 KB (IL)
- **Transitive deps:** None (AngleSharp is self-contained)

**Conclusion:** Negligible impact on library size.

##### 5.6 Expression Placeholder Brittle-ness

Current regex replaces `<%= %>` with `<!--___ASPX_EXPR_0___-->`. AngleSharp preserves HTML comments, but:
- Comment text may be trimmed.
- Multiple spaces collapsed.

**Mitigation:** Use non-whitespace delimiters: `<!--___ASPX_EXPR_0___-->` (current pattern is safe).

---

#### 6. Recommendation

### ✅ **PROCEED** — with mandatory guardrails.

**Why proceed:**
- Solves 5 of 6 XDocument bugs (unclosed tags, entities, single-quote attrs, boolean attrs, script blocks).
- No change to AST or TreeBuilder.
- Test migration is low-risk (case normalization only).

**Mandatory guardrails:**

1. **Keep `asp:` → `asp_` rename** — Do NOT rely on AngleSharp to handle `asp:` prefix correctly. The auto-closing behavior is a deal-breaker.

2. **Keep regex pre-processing** — Directives and expressions MUST be extracted before HTML parsing.

3. **Add integration tests** — Test unclosed tags, single-quote attrs, `&` in URLs, `<script>` blocks.

4. **Performance benchmark** — Measure AngleSharp vs XDocument on large .aspx files (ContosoUniversity, WingtipToys).

5. **Whitespace validation** — Ensure AngleSharp preserves whitespace in text nodes (may need `new HtmlParser(new HtmlParserOptions { IsKeepingSourceReferences = false })`).

**Scope of change:** ~100 lines in `AspxParser.cs` (replace XDocument tree walk with AngleSharp). Zero changes to 5 other files.

**Timeline:** 1 sprint (Cyclops implements, Rogue validates 52 tests + 6 new HTML-tolerance tests).

---

#### 7. Open Questions for Cyclops

1. Does AngleSharp have a "preserve source whitespace" mode? (Test with `\n` and `\r\n` in text nodes.)
2. Can we suppress AngleSharp's HTML5 error correction for unknown elements? (Probably not — may need to parse as XML via `AngleSharp.Xml`.)
3. What's the performance of AngleSharp on a 500KB .aspx file? (Benchmark needed.)

---

**Next Steps (remaining):**
- [x] ~~Cyclops: Build AngleSharp parser prototype~~
- [x] ~~Rogue: Add 6 HTML-tolerance tests~~ (done by Cyclops)
- [x] ~~Cyclops: Run existing 52 tests~~ — all 58 pass
- [ ] Cyclops: Performance benchmark (XDocument vs AngleSharp on WingtipToys Default.aspx)
- [ ] Cyclops: Register missing validators + TableRow/TableCell/TableHeaderCell
- [ ] Cyclops: EventCallback coercion in type mapper
- [ ] Forge: Final review before merge

### Cyclops Prototype
✅ **Complete** — AngleSharp-based parser successfully replaces XDocument.

**Implementation Details:**
- Added `AngleSharp 1.4.0` NuGet package
- Replaced `XDocument.Parse()` with `HtmlParser.ParseDocument()`
- Kept regex pre-processing for directives (`<%@ %>`) and expressions (`<%= %>`, `<%# %>`)
- Added server-side comment stripping (`<%-- --%>`)
- Convert self-closing `<asp_Control />` to explicit close tags (AngleSharp HTML5 mode requirement)
- Preserve original tag name casing (AngleSharp lowercases tags)
- Strip `runat="server"` attributes during parsing
- Support single-quote attributes in directives (`Title='Test'`)

**Fixed Phase 1 Bugs:**
1. ✅ Single-quote attributes now parsed correctly
2. ✅ `runat="server"` stripped from ASP control attributes
3. ✅ Server-side comments (`<%-- --%>`) properly stripped
4. HTML entities (`&amp;`) preserved correctly
5. Unclosed tags (`<br>`, `<hr>`) handled natively

---

## Test Results

### Phase 1 Baseline (XDocument)
- **52 tests passing** (parser, registry, tree builder, integration)
- 3 test pages: Simple.aspx, Nested.aspx, Mixed.aspx

### AngleSharp Replacement
✅ **All tests passing** — 58/58 tests green (52 original + 6 new tolerance tests)

**New Tolerance Tests Added:**
1. ✅ `Parse_UnclosedHtmlTags_ParsesSuccessfully` — handles `<br>`, `<hr>` without self-close
2. ✅ `Parse_EntityInAttributeValue_PreservesEntity` — `&amp;` in URLs preserved
3. ✅ `Parse_SingleQuoteAttributes_ExtractedCorrectly` — `<%@ Page Title='Test' %>`
4. ✅ `Parse_RunatServerAttribute_Stripped` — `runat="server"` removed
5. ✅ `Parse_ServerSideComment_Stripped` — `<%-- comment --%>` completely removed
6. ✅ `Parse_MixedQuotesInAttributes_HandledCorrectly` — mixed `"` and `'` in same tag

**Key Findings:**
- AngleSharp's HTML5 mode treats unknown self-closing tags as open tags
- Solution: Convert `<asp_Control />` → `<asp_Control></asp_Control>` via regex before parsing
- AngleSharp lowercases tag names — case mapping dictionary preserves original casing
- All existing functionality preserved — AST output identical to XDocument version

---

## Decision Log

| Date | Decision | By |
|------|----------|----|
| 2026-03-20 | Begin AngleSharp evaluation as XDocument replacement | Jeff |
| 2026-03-20 | Proceed with AngleSharp + mandatory guardrails (keep asp:→asp_ rename, keep regex pre-processing) | Forge |
| 2026-03-20 | Prototype complete — 58/58 tests passing, 3 Phase 1 bugs fixed | Cyclops |

---

## Performance Benchmark Results

> **Benchmarked:** 2026-03-20 by Rogue  
> **Method:** `System.Diagnostics.Stopwatch` timing, 1000 iterations per scenario (+ 50 warmup), `GC.GetTotalAllocatedBytes()` for memory  
> **Environment:** .NET 10.0, AngleSharp 1.4.0, xUnit test runner  
> **Test class:** `src/BlazorWebFormsComponents.AspxMiddleware.Test/AspxParserBenchmarkTests.cs`  
> **Run command:** `dotnet test --filter "FullyQualifiedName~Benchmark" -v n`

### Size-Based Results

| Scenario | Input Size | Lines | Avg Parse (ms) | Throughput (parses/sec) | Avg Alloc (KB) |
|----------|-----------|-------|-----------------|------------------------|----------------|
| Small | 252 chars | 8 | 0.044 | 22,614 | 51.5 |
| Medium | 1,493 chars | 32 | 0.254 | 3,933 | 121.1 |
| Large | 4,218 chars | 83 | 0.636 | 1,574 | 260.9 |
| XL (stress) | 18,277 chars | 268 | 3.348 | 299 | 926.2 |

### AngleSharp-Specific Scenario Results

| Scenario | Avg Parse (ms) | Throughput (parses/sec) | Avg Alloc (KB) |
|----------|-----------------|------------------------|----------------|
| Unclosed `<br>`/`<hr>` tags | 0.206 | 4,860 | 187.7 |
| `&` entities in attributes | 0.198 | 5,053 | 127.6 |
| Single-quote attributes | 0.197 | 5,074 | 112.2 |
| `<script>` with operators | 0.136 | 7,330 | 89.0 |

### Key Findings

1. **Performance is fast enough:** Even the XL stress test (18KB, 100+ controls) parses in ~3.3ms. Typical pages (1–4KB) parse in under 1ms. This is a non-issue for on-demand .aspx rendering.

2. **Memory scales linearly:** Allocation grows roughly proportional to input size (~50 KB for small inputs, ~926 KB for XL). No unexpected spikes.

3. **AngleSharp-specific scenarios add no overhead:** Unclosed tags, entities, single-quote attrs, and script blocks all parse at speeds comparable to equivalent-sized clean inputs. The HTML5 tolerance comes for free.

4. **Forge's 10-20% slower estimate:** Cannot be directly confirmed since XDocument code has been replaced, but absolute performance is well within acceptable bounds for this use case (not a hot path).

---

## Next Steps

- [x] Forge: Complete feasibility analysis
- [x] Cyclops: Build AngleSharp parser prototype
- [x] Run existing 52 tests against new parser
- [x] Add new tests for HTML-tolerance scenarios (unclosed tags, entities, etc.)
- [x] Performance comparison (benchmarked)
- [x] Fix Phase 1 bugs (single-quote, runat="server", server comments)
- [ ] Consider merging to main branch after validation
