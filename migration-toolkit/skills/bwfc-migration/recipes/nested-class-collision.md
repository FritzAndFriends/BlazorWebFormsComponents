# Recipe: Nested Class Name Collision

## Error Signature

```
CS0542: 'ClassName': member names cannot be the same as their enclosing type
```

## Detection

```powershell
Select-String -Path **/*.cs -Pattern "class (\w+)\s*\{" | Group-Object { 
    [System.IO.Path]::GetFileName($_.Path) 
} | Where-Object { $_.Count -gt 1 }
```

Or look for files where the same class name appears twice — once as the outer class and once as a nested inner class.

## Root Cause

Some Web Forms code files contain nested helper classes. The CLI's class renaming or code-behind emission can produce a situation where a nested class has the same name as its enclosing type:

```csharp
public class NVPAPICaller
{
    // ... outer class members ...
    
    public class NVPAPICaller  // ← same name as outer!
    {
        // ... inner class ...
    }
}
```

This commonly happens with PayPal integration classes, WCF service proxies, or other auto-generated code that was originally in separate files but got merged during migration.

## Fix Pattern

### Step 1: Identify the nested class

Find the inner class declaration that duplicates the outer name.

### Step 2: Rename or extract

**Option A: Rename the inner class** (preferred for stubs)

```csharp
// BEFORE:
public class NVPAPICaller
{
    public class NVPAPICaller { }  // collision
}

// AFTER:
public class NVPAPICaller
{
    public class NVPAPICallerInner { }  // renamed
}
```

**Option B: Remove the duplicate** if it's an empty or redundant stub

```csharp
// If the inner class is empty or duplicates the outer, just remove it
public class NVPAPICaller
{
    // inner class removed — outer class has all the needed members
}
```

**Option C: Extract to its own file** if the inner class has real functionality

Move the nested class to a separate `.cs` file in the same namespace.

### Step 3: Add missing stub types

The nested class may reference types that don't exist in the migrated project. Add minimal stubs:

```csharp
// If NVPCodec or similar types are referenced but not defined:
public class NVPCodec : Dictionary<string, string>
{
    public string Encode() => string.Join("&", 
        this.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
    
    public void Decode(string nvpString)
    {
        foreach (var pair in nvpString.Split('&'))
        {
            var kv = pair.Split('=');
            if (kv.Length == 2) this[kv[0]] = Uri.UnescapeDataString(kv[1]);
        }
    }
}
```

## Prevention

CLI fix needed: Detect when a nested class declaration has the same name as its enclosing type and either rename the nested class or skip emitting the duplicate.

## Verification

```powershell
dotnet build  # CS0542 resolves
```
