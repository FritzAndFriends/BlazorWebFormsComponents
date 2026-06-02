# L2 Repair Playbook

This playbook encodes the expert sequence for repairing a freshly migrated Web Forms app. Following this order minimizes backtracking and resolves root causes before symptoms.

## Pre-Flight (30 seconds)

Before touching any files:

```powershell
dotnet build samples\AfterXxx\Xxx.csproj 2>&1 | Select-String "error CS" | Group-Object { ($_ -split ':')[0] } | Sort-Object Count -Descending
```

This tells you **which files** have the most errors. Fix files with the most errors first — they're usually the root cause.

## Triage Order

Fix errors in this exact order. Each step eliminates cascading errors downstream.

### Step 1: Program.cs / Startup (1 min)

| Symptom | Recipe | Fix |
|---------|--------|-----|
| Misplaced `using` statements | — | Move to top of file |
| Missing service registrations | `new-dbcontext-to-di.md` | Add `builder.Services.AddScoped<T>()` |
| Duplicate `using` | — | Remove duplicates |

### Step 2: Service / Logic Classes (2 min)

These cause the most cascading errors across pages.

| Symptom | Recipe | Fix |
|---------|--------|-----|
| Class injects itself | `circular-self-injection.md` | Remove self-referencing field + param |
| `_db` vs `_productContext` mismatch | `new-dbcontext-to-di.md` | Unify to the DI field name |
| Session garbling (`SetString(key, = null)`) | `session-transform-garbling.md` | Restore `GetString(key) == null` comparisons |
| Nested class same name as outer | `nested-class-collision.md` | Rename or remove inner class |
| Missing Include() for navigation | — | Add `.Include(x => x.Nav)` on EF queries |

### Step 3: Page Code-Behind Files (2 min)

| Symptom | Recipe | Fix |
|---------|--------|-----|
| Missing `@ref` backing fields | — | Add field declarations in `.razor.cs` |
| `(object sender, EventArgs e)` signature | `eventcallback-signature-mismatch.md` | Remove `sender` parameter |
| `Request.IsLocal` not found | `request-shim-gaps.md` | Replace with `true` or env check |
| Static method using instance field | — | Make method non-static or inject dependency |
| Missing type stubs (NVPCodec etc.) | `nested-class-collision.md` | Add minimal stub class |
| `IDatabaseInitializer<T>` | `database-seed-initializer.md` | Convert to `IHostedService` |

### Step 4: Markup Files (.razor) (1 min)

| Symptom | Recipe | Fix |
|---------|--------|-----|
| Wrong `SelectMethod` name | `selectmethod-string-binding.md` | Match to actual method name in code-behind |
| Unclosed `<b>`, `<i>` tags | — | Close the tag |
| Orphan `</p>`, `</div>` | — | Remove orphan closing tag |
| Self-closing `<td/>` | — | Change to `<td></td>` |
| `int` to `string` type mismatch in `Text=` | — | Add `.ToString()` |
| `@ref` to component that doesn't need it | — | Remove `@ref` or add backing field |

### Step 5: Rebuild and Verify (30 sec)

```powershell
dotnet build samples\AfterXxx\Xxx.csproj --nologo
```

If errors remain, re-run Step 1's grouping command and fix the next highest-error file.

## Startup Triage (after build succeeds)

```powershell
dotnet run --project samples\AfterXxx\Xxx.csproj --urls https://localhost:5001
```

### Quick smoke test:

```powershell
@('/', '/About', '/Contact', '/ProductList', '/Account/Login') | ForEach-Object {
    $status = curl -k -s -o NUL -w "%{http_code}" "https://localhost:5001$_"
    "$status $_"
}
```

### Common startup failures:

| Console Error | Root Cause | Fix |
|---------------|------------|-----|
| `Invalid object name` | Missing tables | `Database.EnsureCreated()` in Program.cs |
| `Cannot attach` | Bad connection string | Fix in `appsettings.json` |
| `Unable to resolve service` | Missing DI | Add registration in Program.cs |
| `No method named 'XxxMethod'` | Wrong SelectMethod | Fix name in `.razor` file |

## Expert Tips

1. **Fix logic classes before pages** — a single ShoppingCartActions fix often resolves 3-4 page errors
2. **Don't rewrite CLI output** — read what the CLI generated before changing it. Often the code is correct but a config issue is causing the failure
3. **Session comparison vs assignment** — if you see garbled Session code, check the original `.cs` file to understand the intent
4. **@ref is rarely needed** — if a component has `@ref`, check if the code-behind actually uses it. Often the @ref can just be removed
5. **Trust the shims** — `WebFormsPageBase` provides `Session`, `Request`, `Response`, `Server`, `Cache`, `ViewState`, `ClientScript`. Don't reinject these services
