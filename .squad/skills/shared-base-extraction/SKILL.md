# Skill: Extracting a Shared Base Class from Sibling Components

**confidence:** low
**source:** earned

## When to Use
When multiple components in the same inheritance tier share identical properties, methods, or logic that should live in a common base class. Signs: 3+ components with copy-pasted `[Parameter]` declarations and helper methods.

## Steps

### 1. Identify the duplicated surface
List every property, method, and field that appears identically (or near-identically) across the sibling components. Only consolidate members that are truly shared by ALL siblings.

### 2. Create the intermediate base class
Place it in the same directory/namespace as its parent. Inherit from the existing shared parent:
```csharp
public class BaseListControl<TItem> : DataBoundComponent<TItem>
```

### 3. Move shared members to the base
- `[Parameter]` properties: move as-is
- Helper methods: make `protected` if subclasses or `.razor` templates call them, `private` otherwise
- New feature parameters go here too — that's often the motivation for the extraction

### 4. Update each subclass
- Change `: OldParent` to `: NewBase` in `.razor.cs`
- Change `@inherits OldParent` to `@inherits NewBase` in `.razor`
- Remove the now-inherited members from each subclass
- Keep subclass-specific members untouched

### 5. Build and verify
A successful build confirms no accidental member hiding or missing references. Watch for CS0263 (partial class base mismatch) — both `.razor` and `.razor.cs` must agree on the base class.

## Key Facts
- In Blazor, `@inherits` in the `.razor` file and `: BaseClass` in the `.razor.cs` must specify the same type for partial classes.
- `protected` methods in the base are accessible from `.razor` template code (`@code` blocks and inline expressions).
- When creating new `ListItem` copies to apply formatting, preserve all properties (Text, Value, Selected, Enabled) to avoid data loss.
- Apply rendering transforms (like format strings) in the shared `GetItems()` at render time, not at bind time, so they affect both static and data-bound items.

## Checklist
- [ ] All sibling components compile with the new base
- [ ] No duplicate `[Parameter]` declarations remain in subclasses
- [ ] Subclass-specific members are preserved
- [ ] `@inherits` directives updated in all `.razor` files
