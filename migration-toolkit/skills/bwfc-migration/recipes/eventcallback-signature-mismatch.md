# Recipe: EventCallback Signature Mismatch

## Error Signature

```
CS1503: Argument 1: cannot convert from 'method group' to 'EventCallback'
CS0123: No overload for 'Handler' matches delegate 'EventCallback<EventArgs>'
```

The handler method in the code-behind has the Web Forms `(object sender, EventArgs e)` signature, but the Blazor component's `OnClick` or similar event expects `EventCallback<EventArgs>` which only passes the `EventArgs` parameter.

## Detection

```powershell
Select-String -Path **/*.razor.cs -Pattern "void \w+_Click\(object\??\s+(sender|s),\s*EventArgs"
Select-String -Path **/*.razor.cs -Pattern "void \w+\(object\??\s+sender,\s*\w*EventArgs"
```

## Root Cause

Web Forms event handlers always have the signature `void Handler(object sender, EventArgs e)`. Blazor's `EventCallback<T>` only passes the event args — there is no `sender` parameter.

The CLI copies the code-behind handler signature as-is, but the markup generates `OnClick="@Handler"` which expects `EventCallback<EventArgs>` (single `EventArgs` parameter).

## Fix Pattern

### Option A: Remove the `sender` parameter (preferred)

```csharp
// BEFORE (Web Forms signature):
protected void LogIn_Click(object sender, EventArgs e)
{
    // handler body
}

// AFTER (Blazor signature):
protected void LogIn_Click(EventArgs e)
{
    // handler body — remove any references to `sender`
}
```

### Option B: Wrap in a lambda in markup

If many handlers use `sender` (rare in practice), wrap in markup:

```razor
@* BEFORE: *@
<Button OnClick="@LogIn_Click" />

@* AFTER: *@
<Button OnClick="@((e) => LogIn_Click(this, e))" />
```

### Which to choose?

- **Option A** if the handler doesn't reference `sender` (vast majority of cases)
- **Option B** if the handler explicitly uses `sender` (cast to get the control)

In practice, Web Forms handlers almost never use `sender` — they reference controls by their field names directly. Option A is correct 95%+ of the time.

## Prevention

CLI fix needed: A code-behind transform should detect `(object sender, EventArgs e)` signatures in Page-type files and remove the `sender` parameter, since Blazor events don't provide it.

## Verification

```powershell
dotnet build  # CS1503/CS0123 errors resolve
```

Test that the handler fires correctly by clicking the button in the running app.
