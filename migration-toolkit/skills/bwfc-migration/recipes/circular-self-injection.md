# Recipe: Circular Self-Injection

## Error Signature

```
System.InvalidOperationException: A circular dependency was detected for the service of type 'XxxActions'
```

Or at build time, the constructor signature looks wrong:

```csharp
public class ShoppingCartActions
{
    private readonly ShoppingCartActions _shoppingCartActions; // injects itself!
    
    public ShoppingCartActions(ProductContext productContext, ShoppingCartActions shoppingCartActions)
    {
        _shoppingCartActions = shoppingCartActions;
    }
}
```

## Detection

```powershell
# Find classes that inject themselves
Select-String -Path **/*.cs -Pattern "private readonly (\w+) _\w+;" | ForEach-Object {
    $type = $_.Matches.Groups[1].Value
    $file = $_.Path
    if (Select-String -Path $file -Pattern "class $type" -Quiet) {
        $_
    }
}
```

Or simply scan constructor parameters for the class's own type name.

## Root Cause

The CLI's DI transform detects `new XxxActions()` instantiations inside a class and converts them to constructor-injected dependencies. When `ShoppingCartActions` has a static method `GetCart()` that does `new ShoppingCartActions()`, the transform injects `ShoppingCartActions` into itself — creating a circular dependency.

This happens because the transform doesn't check whether the type being injected is the **same class** as the one being modified.

## Fix Pattern

### Step 1: Remove the self-referencing field and constructor parameter

```csharp
// BEFORE:
public class ShoppingCartActions
{
    private readonly ProductContext _productContext;
    private readonly ShoppingCartActions _shoppingCartActions; // ← remove
    
    public ShoppingCartActions(ProductContext productContext, ShoppingCartActions shoppingCartActions)
    {
        _productContext = productContext;
        _shoppingCartActions = shoppingCartActions; // ← remove
    }
}

// AFTER:
public class ShoppingCartActions
{
    private readonly ProductContext _productContext;
    
    public ShoppingCartActions(ProductContext productContext)
    {
        _productContext = productContext;
    }
}
```

### Step 2: Fix `GetCart()` or similar factory methods

The original pattern is usually a static factory that creates an instance:

```csharp
// Original Web Forms:
public static ShoppingCartActions GetCart(HttpContext context)
{
    var cart = new ShoppingCartActions();
    cart.ShoppingCartId = GetCartId(context);
    return cart;
}
```

Convert to use the injected instance (since DI provides the instance):

```csharp
// Migrated: instance method, uses the DI-provided instance
public ShoppingCartActions GetCart()
{
    ShoppingCartId = GetCartId();
    return this;
}
```

Or if the factory method is called externally, use the DI-registered service directly.

## Prevention

CLI fix needed: `DbContextInstantiationTransform` should skip injection when the resolved service type matches the containing class name.

## Verification

```powershell
dotnet build  # no circular DI at runtime
dotnet run     # app starts without circular dependency exception
```
