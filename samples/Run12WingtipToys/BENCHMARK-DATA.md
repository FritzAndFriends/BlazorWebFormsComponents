# Run 12 WingtipToys Migration — Benchmark Data

## Pipeline Timing

| Layer | Duration | Notes |
|-------|----------|-------|
| Layer 0 (Scan) | skipped | bwfc-scan.ps1 not run this cycle |
| Layer 1 (Transform) | ~3.5s | 365 transforms applied, 46 manual review items |
| Layer 2 (Copilot-Assisted) | ~15min | Auth services, EF Core, admin CRUD, code-behinds |
| Build Verification | ~3.0s | **2 build attempts** → 0 errors, 0 warnings |

## Transform Summary

- **Files processed:** 32
- **Transforms applied:** 365
- **Static files copied:** 79
- **Manual review items:** 46

### Layer 1 New Features (Cycle 3)
- **Mock auth service generation** — `Add-MockAuthService` creates `MockAuthService.cs` and `MockAuthenticationStateProvider.cs` when Account/Login.aspx detected
- **Program.cs auth injection** — Conditional auth service registration in generated Program.cs
- **LogoutAction enum conversion** — `LogoutAction="Redirect"` → `LogoutAction="@LogoutAction.Redirect"`
- **BorderStyle enum conversion** — `BorderStyle="None"` → `BorderStyle="@BorderStyle.None"`
- **Visible attribute handling** — `Visible="true"` stripped (default), `Visible="false"` preserved
- **Hex color escaping** — `BorderColor="#efeeef"` → `BorderColor="@("#efeeef")"` for Razor safety

## Output Inventory

| Metric | Count |
|--------|-------|
| .razor files | 32 |
| .razor.cs code-behinds | 32 |
| wwwroot static files | 79 |
| Routable pages | 28 |
| BWFC control instances | **184** |
| Unique BWFC control types | **27** |

### BWFC Control Inventory

| Control | Instances |
|---------|-----------|
| Label | 44 |
| TextBox | 22 |
| RequiredFieldValidator | 21 |
| Button | 17 |
| PlaceHolder | 14 |
| HyperLink | 9 |
| ValidationSummary | 7 |
| Literal | 7 |
| BoundField | 7 |
| CompareValidator | 4 |
| ListView | 4 |
| TemplateField | 4 |
| DropDownList | 3 |
| LinkButton | 3 |
| CheckBox | 3 |
| GridView | 2 |
| OpenAuthProviders | 2 |
| HiddenField | 2 |
| Panel | 1 |
| LoginStatus | 1 |
| RegularExpressionValidator | 1 |
| FileUpload | 1 |
| DetailsView | 1 |
| ImageButton | 1 |
| AuthorizeView | 1 |
| Image | 1 |
| FormView | 1 |

## Build Attempts

| Attempt | Errors | Fix Applied |
|---------|--------|-------------|
| 1 | 95 | Raw Layer 1 output — missing Data/Models/Services, Web Forms code-behinds |
| 2 | 0 | ✅ Layer 2: EF Core, models, CartStateService, auth services, all code-behinds |

## Auth Services (NEW in Run 12)

| Service | File | Purpose |
|---------|------|---------|
| MockAuthService | Services/MockAuthService.cs | In-memory user store with email/password auth |
| MockAuthenticationStateProvider | Services/MockAuthenticationStateProvider.cs | Blazor auth state with login/logout |
| CartStateService | Services/CartStateService.cs | In-memory shopping cart state |

### Mock Auth Credentials
- Default: `admin@wingtiptoys.com` / `Pass@word1`
- Registration creates new users in-memory

## Page Status

### Functional Pages (data-bound, interactive)
| Page | Status | Notes |
|------|--------|-------|
| Default (/) | ✅ Functional | Uses @Title from WebFormsPageBase |
| ProductList | ✅ Functional | OnParametersSetAsync, category filter via ?id= |
| ProductDetails | ✅ Functional | OnParametersSetAsync, FormView with Items binding |
| AddToCart | ✅ Functional | Adds to CartStateService, redirects to ShoppingCart |
| ShoppingCart | ✅ Functional | ImageButton preserved, cart CRUD |
| CheckoutReview | ✅ Functional | DetailsView with shipping info preserved |
| CheckoutComplete | ✅ Functional | Transaction ID, continue shopping |
| AdminPage | ✅ Functional | Full CRUD: Add/remove products with EF Core |
| MainLayout | ✅ Functional | Category nav, cart count, AuthorizeView + LoginStatus |
| **Login** | ✅ **Functional** | **NEW: MockAuth integration, @bind-Text for TextBox** |
| **Register** | ✅ **Functional** | **NEW: MockAuth registration, auto-login on success** |

### Minimal Pages (BWFC markup preserved, stub handlers)
| Page | Status | Notes |
|------|--------|-------|
| About | ✅ Minimal | Static content, @Title |
| Contact | ✅ Minimal | Static content, @Title |
| ManageLogins | ✅ Minimal | ListView + Button + PlaceHolder preserved |
| Manage | ✅ Minimal | HyperLink navigation |
| ManagePassword | ✅ Minimal | Full BWFC validator markup |
| Forgot | ✅ Minimal | BWFC form markup |
| Confirm | ✅ Minimal | HyperLink + PlaceHolder |
| ResetPassword | ✅ Minimal | BWFC form with validators |

### Stub Pages (routable, minimal content)
| Page | Status | Notes |
|------|--------|-------|
| Lockout | Stub | Static lockout message |
| ResetPasswordConfirmation | Stub | HyperLink to login |
| RegisterExternalLogin | Stub | BWFC form preserved |
| AddPhoneNumber | Stub | BWFC form preserved |
| VerifyPhoneNumber | Stub | BWFC form preserved |
| TwoFactorAuthenticationSignIn | Stub | BWFC form with DropDownList |
| CheckoutStart | Stub | Placeholder |
| CheckoutCancel | Stub | Static message |
| CheckoutError | Stub | Error display |
| ErrorPage | Stub | Label + Panel |
| OpenAuthProviders | Stub | ListView with EmptyDataTemplate |
| ViewSwitcher | Stub | Not applicable in Blazor |

## Run-over-Run Comparison

| Metric | Run 9 | Run 10 | Run 11 | Run 12 |
|--------|-------|--------|--------|--------|
| Layer 1 transforms | 667 | 673 | 354* | **365** |
| Build attempts | 7 | 3 | 4 | **2** |
| .razor files | 35 | 35 | 32 | **32** |
| Routable pages | 28 | 28 | 28 | **28** |
| BWFC instances | 173 | 172 | 178 | **184** |
| Unique BWFC types | 23 | 26 | 26 | **27** |
| P0 issues | 3 | 3 | 0 | **0** |
| Preservation rate | 98.9% | 92.7% | ~100% | **~100%** |
| Auth functional | ❌ | ❌ | ❌ | **✅** |

*Run 11 used `-SkipProjectScaffold` flag; transform count reflects markup transforms only.

### Key Improvements in Run 12
1. **Functional authentication** — Login/Register pages use MockAuthService with @bind-Text BWFC binding
2. **Auto-generated auth services** — Layer 1 script now creates MockAuthService + MockAuthenticationStateProvider
3. **Enum conversion coverage** — LogoutAction, BorderStyle added to Layer 1 pipeline
4. **Hex color escaping** — `#hex` values safely escaped for Razor preprocessor
5. **Visible attribute handling** — `Visible="true"` stripped, `Visible="false"` preserved
6. **Lowest build attempts** — 2 attempts to 0 errors (vs 4 in Run 11)
7. **Highest BWFC count** — 184 instances (vs 178 in Run 11), 27 types (vs 26)
8. **LoginStatus preserved** — BWFC LoginStatus with LogoutAction enum on MainLayout
