# Wingtip Run 28 CLI Root-Cause Analysis

## Goal

Capture why the fresh `samples\AfterWingtipToys` output was broken after the `run28` migration, what had to be rewritten during repair, and what the CLI should do differently so future runs need far less manual work.

## Core Finding

The generated output failed for three overlapping reasons:

1. The CLI preserved too much **raw Web Forms behavior**.
2. The CLI converted too little **page/runtime semantics**.
3. The CLI copied too much **legacy code that was not runnable in the generated Blazor SSR shape**.

This was not just a master-page problem. Master pages were one part of a larger issue where the output looked structurally familiar but still behaved like partially copied Web Forms.

## What the CLI Emitted

### Master pages

`MasterPageTransform` already does useful structural cleanup:

- strips outer document scaffolding
- converts `<asp:ContentPlaceHolder>` to `<ContentPlaceHolder>`
- wraps output in `<MasterPage>`
- extracts some `<head>` content

But that is still mostly a **markup wrapper transform**, not a **runnable shell transform**.

Result: generated files such as `Site.razor` still needed real migration decisions around shell composition, head assets, script/bundle references, login/cart chrome, forms, and master/content wiring.

### Content pages

`ContentWrapperTransform` converts:

- `<asp:Content ... ContentPlaceHolderID="X">`
- to `<Content ContentPlaceHolderID="X">`
- and wraps the page in the master component, e.g. `<Site>...</Site>`

That preserves the **shape** of content-page relationships, but not the **behavior** of the page.

Result: pages still needed semantic rewrites for:

- query-string binding
- cart actions
- login/register form behavior
- navigation/redirect flows
- validation behavior
- selector-compatible UI for Playwright

### Code-behind

Generated `.razor.cs` files were copied with TODO comments, but many still contained Web Forms assumptions such as:

- `Page.PreLoad`
- `ViewState`
- `HttpContext.Current`
- `Context.GetOwinContext()`
- EF `ProductContext` access
- `[QueryString]` / `[RouteData]`-style patterns

That left many files in an awkward middle state: present in the build, partially annotated, but not genuinely migrated.

## What Actually Needed Rewriting

### 1. Master-page shell files

These were the highest-value runtime fixes:

- `Site.razor`
- parts of `Site.razor.cs`

Why they broke:

- generated shell still expected Web Forms lifecycle and anti-XSRF patterns
- still assumed OWIN logout/auth wiring
- still assumed server-managed chrome composition instead of a runnable Blazor SSR shell

What the CLI should have emitted:

- a valid SSR shell component
- proper `ChildContent` contract
- normalized head/static asset handling
- Blazor-compatible login/cart/nav rendering

### 2. Acceptance-path pages

These pages needed semantic repair:

- `Default.razor`
- `About.razor`
- `Contact.razor`
- `ProductList.razor`
- `ProductDetails.razor`
- `ShoppingCart.razor`
- `Account\Register.razor`
- `Account\Login.razor`

Why they broke:

- content wrappers were converted, but page semantics were not
- product lookup/query handling was still Web Forms shaped
- add-to-cart flow still behaved like a routed action page instead of a usable SSR interaction
- login/register behavior still assumed validator/control patterns rather than working SSR forms

### 3. Build-blocker pages

These pages were not all core features, but they still blocked build or runtime:

- `Account\*.razor`
- `Checkout\*.razor`
- `Admin\AdminPage.razor`
- `AddToCart.razor`
- `ErrorPage.razor`
- `ViewSwitcher.razor`

Why they broke:

- invalid leftover Web Forms markup
- unsupported identity/updatepanel-era patterns
- copied code-behind that was still only partly transformed

### 4. Project surface

The generated project also needed non-page repair:

- `Program.cs`
- `WingtipToys.csproj`

Why they broke:

- the CLI copied a broad compile surface that did not belong in a runnable repaired SSR sample
- App_Start, OWIN, EF initializer/context, and legacy code-behind should not have been compiled as-is

## Why the Current CLI Stops Short

The existing transforms are still mostly **syntactic**:

- **MasterPageTransform**: make the master page look like BWFC markup
- **ContentWrapperTransform**: make content sections look like BWFC content components

Wingtip needed the next layer: **semantic transforms**.

## What the CLI Should Do Next

### 1. Make master-page migration a first-class runnable mode

The CLI should emit master pages as real BWFC shell components, not just cleaned fragments.

Needed behaviors:

- generate valid `ChildContent` contracts
- normalize `<head>` assets into app-level references
- remove or rewrite `ScriptManager`, bundle references, and server form assumptions
- migrate login/cart/user chrome into Blazor-compatible behavior

### 2. Stop compiling copied legacy code-behind by default

Copied `.razor.cs` should be one of two things:

1. fully migrated and compile-ready, or
2. clearly quarantined as manual migration material

The current half-migrated middle state causes most of the friction.

Recommended CLI behavior:

- include migrated code-behind only when transforms make it compile
- otherwise exclude it from compile or place it in a manual-migration area
- apply the same rule to App_Start, OWIN, EF initializer, and identity bootstrap files

### 3. Add semantic transforms for common Web Forms page patterns

Wingtip exposed several recurring patterns the CLI should learn directly:

1. **Query-driven details pages**  
   Convert `SelectMethod` + query-string patterns into Blazor query-bound parameters and service-backed lookups.

2. **Redirect/action pages**  
   Convert pages like `AddToCart.aspx` into a usable action flow, not a routed content page that still behaves like a page.

3. **Simple account pages**  
   Convert login/register flows into SSR-safe forms rather than preserving validator-heavy control markup that does not compile or run well.

4. **Master/content contracts**  
   When a page is wrapped in `<Site>...</Site>`, ensure the generated component contract is valid and the target master component actually expects the child content being emitted.

### 4. Add compile-surface filtering

The CLI should automatically recognize files that are unlikely to survive direct migration into the generated SSR app shape.

For Wingtip, likely filter or quarantine:

- `App_Start\*`
- `Startup.Auth.cs`
- `IdentityConfig.cs`
- EF initializer/context bootstrapping files
- copied `.razor.cs` files with unresolved Web Forms runtime assumptions

### 5. Turn the benchmark findings into CLI regression tests

The specific failures from run28 should become tests for:

- master-page `ChildContent` / content-section wiring
- runnable `Site.Master` conversion
- content-page wrapping that produces valid component contracts
- add-to-cart conversion
- account-page form conversion
- exclusion or quarantine of non-runnable legacy compile surface

## Bottom Line

The broken output was not just "master page conversion failed."

It was:

1. **master-page conversion preserved structure but not a runnable shell**
2. **content-page conversion preserved wrappers but not page semantics**
3. **legacy code-behind/app-start/identity/EF surface was copied into the build when it should have been transformed or quarantined**

The next CLI improvements should focus on **semantic migration output**, not just cleaner markup transforms.

## Recommended Follow-Up Work

1. Add a dedicated **runnable master-page shell transform** for `.master` files.
2. Add **pattern-based page transforms** for details, cart, and account flows.
3. Add **compile-surface filtering/quarantine rules** for copied legacy files.
4. Add **Wingtip benchmark regression tests** that assert those behaviors.
