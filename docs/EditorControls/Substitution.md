# Substitution

The **Substitution** component renders dynamic content by invoking a callback function. In Web Forms, the Substitution control was used for post-cache substitution — injecting dynamic content into output-cached pages. In Blazor, output caching does not apply, so Substitution simply renders the return value of its callback directly.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.substitution?view=netframework-4.8

## Features Supported in Blazor

- **SubstitutionCallback** — A `Func<HttpContext, string>` delegate that returns the content to render
- **MethodName** — Accepted for migration reference (string); use `SubstitutionCallback` for the actual delegate

### Blazor Notes

- In Web Forms, the callback was a static method referenced by name (`MethodName`). In Blazor, you pass a delegate directly via the `SubstitutionCallback` parameter
- The callback receives an `HttpContext` parameter for compatibility, allowing access to request/user information
- The return value of the callback is rendered as raw string content
- Post-cache substitution does not apply in Blazor — the component simply invokes the callback on each render

## Web Forms Features NOT Supported

- **Post-cache substitution** — Blazor does not use output caching in the same way; all rendering is dynamic by default
- **Static method resolution by name** — Use the `SubstitutionCallback` delegate parameter instead of relying on `MethodName` string resolution

## Web Forms Declarative Syntax

```html
<%@ OutputCache Duration="60" VaryByParam="none" %>

<asp:Substitution
    EnableTheming="True|False"
    EnableViewState="True|False"
    ID="string"
    MethodName="string"
    OnDataBinding="DataBinding event handler"
    OnDisposed="Disposed event handler"
    OnInit="Init event handler"
    OnLoad="Load event handler"
    OnPreRender="PreRender event handler"
    OnUnload="Unload event handler"
    runat="server"
    Visible="True|False"
/>
```

## Blazor Razor Syntax

### Basic Usage

```razor
<Substitution SubstitutionCallback="GetCurrentTime" />

@code {
    string GetCurrentTime(HttpContext context)
    {
        return DateTime.Now.ToString("HH:mm:ss");
    }
}
```

### With MethodName (Migration Reference)

```razor
@* MethodName kept for documentation; SubstitutionCallback does the work *@
<Substitution MethodName="GetGreeting"
              SubstitutionCallback="GetGreeting" />

@code {
    string GetGreeting(HttpContext context)
    {
        var name = context.User.Identity?.Name ?? "Guest";
        return $"Hello, {name}!";
    }
}
```

## HTML Output

Substitution renders the **return value of the callback as inline text** with no wrapper element.

**Blazor:**
```razor
<Substitution SubstitutionCallback="GetTime" />
```

**Rendered HTML (example):**
```html
14:30:45
```

## Migration Notes

When migrating from Web Forms to Blazor:

1. **Convert static methods to instance methods** — Web Forms required `MethodName` to point to a `public static string Method(HttpContext context)`. In Blazor, pass a delegate via `SubstitutionCallback` — it can be an instance method
2. **Remove `asp:` prefix and `runat="server"`** — Standard Blazor migration
3. **Remove OutputCache directive** — Post-cache substitution is not needed; Blazor renders dynamically by default
4. **Replace `MethodName` with `SubstitutionCallback`** — Pass the actual delegate instead of a string name

!!! note "Key Difference"
    In Web Forms, Substitution existed to solve a very specific problem: injecting dynamic content into an otherwise cached page. In Blazor, **every render is dynamic** — there is no output cache to punch holes in. Substitution in Blazor simply calls your function and renders the result, which is equivalent to calling a method in Razor directly (`@GetCurrentTime()`). The component exists for markup migration compatibility.

### Before (Web Forms)

```html
<%@ OutputCache Duration="60" VaryByParam="none" %>

<p>This content is cached for 60 seconds.</p>

<asp:Substitution ID="TimeStamp" runat="server"
                  MethodName="GetCurrentTime" />

<p>This content is also cached.</p>
```

```csharp
// Must be a public static method
public static string GetCurrentTime(HttpContext context)
{
    return DateTime.Now.ToString("HH:mm:ss");
}
```

### After (Blazor)

```razor
<p>This content renders on every request.</p>

<Substitution SubstitutionCallback="GetCurrentTime" />

<p>This content also renders on every request.</p>

@code {
    string GetCurrentTime(HttpContext context)
    {
        return DateTime.Now.ToString("HH:mm:ss");
    }
}
```

!!! tip "Consider Native Blazor"
    For new Blazor development, you can replace `<Substitution>` with a simple Razor expression: `@DateTime.Now.ToString("HH:mm:ss")`. Use the Substitution component only when you want to preserve the original Web Forms markup structure during migration.

## Examples

### User-Specific Greeting

```razor
<Substitution SubstitutionCallback="GetUserGreeting" />

@code {
    string GetUserGreeting(HttpContext context)
    {
        var userName = context.User.Identity?.Name ?? "Guest";
        return $"Welcome back, {userName}!";
    }
}
```

### Dynamic Footer Content

```razor
<footer>
    <Substitution SubstitutionCallback="GetFooter" />
</footer>

@code {
    string GetFooter(HttpContext context)
    {
        return $"© {DateTime.Now.Year} My Application | Server: {Environment.MachineName}";
    }
}
```

## See Also

- [Literal](Literal.md) - Renders text content with optional encoding
- [Label](Label.md) - Renders text in a `<span>` element
- [Deferred Controls](../Migration/DeferredControls.md) - Previously deferred controls reference
- [Migration — Getting Started](../Migration/readme.md)
