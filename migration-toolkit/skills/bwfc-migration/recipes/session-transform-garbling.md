# Recipe: Session Transform Garbling

## Error Signature

```
CS1002: ; expected
CS1525: Invalid expression term '='
```

Lines containing patterns like:
```csharp
Session.SetString(CartSessionKey, = null)
Session.GetString(key).ToString() = null
```

## Detection

```powershell
Select-String -Path **/*.cs -Pattern "Session\.(Get|Set)String\(.*=\s*null"
```

Or look for garbled Session lines where `== null` comparison was mangled into an assignment.

## Root Cause

The CLI's Session transform converts `HttpContext.Current.Session[key]` to `Session.GetString(key)` / `Session.SetString(key, value)`. However, when the original code uses **comparison patterns** like:

```csharp
if (HttpContext.Current.Session[CartSessionKey] == null)
```

The transform incorrectly treats the `== null` as part of the value assignment, producing:

```csharp
Session.SetString(CartSessionKey, = null)  // garbled
```

The transform's regex matches `Session[key] = value` (assignment) but doesn't distinguish `Session[key] == null` (comparison).

## Fix Pattern

### Step 1: Identify garbled lines

Search for `Session.SetString(` or `Session.GetString(` lines that contain `= null` or look syntactically broken.

### Step 2: Restore the original intent

| Original Web Forms | Garbled CLI Output | Correct Fix |
|---|---|---|
| `if (Session[key] == null)` | `Session.SetString(key, = null)` | `if (Session.GetString(key) == null)` |
| `if (Session[key] != null)` | `Session.SetString(key, != null)` | `if (Session.GetString(key) != null)` |
| `Session[key] = value` | Usually correct | `Session.SetString(key, value.ToString())` |
| `var x = (string)Session[key]` | Usually correct | `var x = Session.GetString(key)` |

### Step 3: Fix the file

```csharp
// BEFORE (garbled):
if (Session.SetString(CartSessionKey, = null))
{
    Session.SetString(CartSessionKey, Guid.NewGuid().ToString());
}

// AFTER (correct):
if (Session.GetString(CartSessionKey) == null)
{
    Session.SetString(CartSessionKey, Guid.NewGuid().ToString());
}
```

## Prevention

CLI fix needed: The Session transform regex should detect comparison operators (`==`, `!=`) and emit `Session.GetString(key) == null` instead of `Session.SetString(key, = null)`.

## Verification

```powershell
dotnet build  # garbled lines produce CS1002/CS1525
```

After fix, the Session lines should compile and produce correct runtime behavior (null checks work, values persist in session).
