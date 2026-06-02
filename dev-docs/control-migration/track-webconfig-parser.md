# Track: Web.config Tag/Namespace Parser

**Priority:** P0 (Foundation)  
**Owner:** Bishop (CLI Development)  
**Status:** Planned  
**Validation:** ContosoUniversity prescan

## Problem Statement

The CLI migration pipeline currently ignores `Web.config` registration of custom controls. This means:

- Custom control namespaces registered via `<add assembly="..." namespace="...">` are not recognized
- `.ascx` pages using those custom controls cannot be validated or pre-scanned
- Layer 1 output includes `.ascx` files unchanged, breaking Layer 1 build

**Example from WingtipToys:**
```xml
<!-- Web.config -->
<configuration>
  <system.web>
    <compilation>
      <assemblies>
        <add assembly="WingtipToys, Version=1.0.0.0, Culture=neutral" />
      </assemblies>
    </compilation>
  </system.web>
  <system.webServer>
    <compilation>
      <assemblies>
        <add assembly="WingtipToys, Version=1.0.0.0, Culture=neutral" />
      </assemblies>
    </compilation>
  </system.webServer>
</configuration>
```

This tells ASP.NET that the `WingtipToys` assembly contains Web Forms controls that can be used in ASPX/ASCX without explicit `@Register` directives.

## Scope

Parse `Web.config` and extract:

1. **Assembly registrations** (`<assemblies>` section) → namespace + assembly name
2. **Namespace tag registrations** (`<controls>` section, if present) → tag prefix + namespace
3. **@Register directives in ASPX/ASCX** → tag prefix + control class + assembly

## Deliverables

### 1. Create `WebConfigAssemblyParser` class

**Location:** `src/BlazorWebFormsComponents.Cli/Analysis/`

```csharp
public class WebConfigAssemblyParser
{
    /// <summary>
    /// Parse Web.config and extract custom control registrations.
    /// </summary>
    public ControlRegistrationInfo Parse(string webConfigPath);
}

public class ControlRegistrationInfo
{
    public IReadOnlyList<AssemblyRegistration> Assemblies { get; }
    public IReadOnlyList<NamespaceTagRegistration> NamespaceTags { get; }
}

public record AssemblyRegistration(string AssemblyName, string Namespace);
public record NamespaceTagRegistration(string TagPrefix, string Namespace);
```

### 2. Integrate into `RuntimeDetector`

Extend `RuntimeDetector` to call the parser during prescan:

```csharp
public class RuntimeDetector
{
    // Existing fields...

    public ControlRegistrationInfo CustomControlRegistrations { get; private set; }

    public void Analyze(ProjectMetadata project)
    {
        // Existing analysis...

        var webConfigPath = Path.Combine(project.ProjectFolder, "Web.config");
        if (File.Exists(webConfigPath))
        {
            var parser = new WebConfigAssemblyParser();
            this.CustomControlRegistrations = parser.Parse(webConfigPath);
        }
    }
}
```

### 3. Add to prescan output

The prescan (`webforms-to-blazor prescan`) should report custom controls found:

```
[Custom Controls Found]
  - WingtipToys assembly: 3 classes detected
  - MyCompany.Controls namespace: 5 controls (registered in Web.config)
  - ProductsControl.ascx (found in /UserControls/)
```

### 4. Create unit tests

**Location:** `tests/BlazorWebFormsComponents.Cli.Tests/Analysis/WebConfigAssemblyParserTests.cs`

- ✓ Parse `<add assembly="...">` elements (both `<system.web>` and `<system.webServer>`)
- ✓ Handle missing Web.config gracefully
- ✓ Handle malformed XML without crash
- ✓ Extract correct assembly/namespace pairs
- ✓ Parse `<add assembly="X" />` and `<add assembly="X"><add assembly="Y" /></add>` nesting

### 5. Integration test

Add a prescan test using ContosoUniversity or WingtipToys:

```bash
dotnet run --project src/BlazorWebFormsComponents.Cli -- \
  prescan -i samples/WingtipToys
```

Output should list detected custom controls.

## Validation Criteria

✅ Parser correctly extracts assembly and namespace from Web.config  
✅ Prescan output includes "Custom Controls Found" section  
✅ No crash on missing/malformed Web.config  
✅ Unit tests pass: `dotnet test tests/BlazorWebFormsComponents.Cli.Tests --filter "WebConfigAssemblyParser"`  
✅ Prescan runs successfully on WingtipToys and ContosoUniversity without errors

## Dependencies

None — this is P0 foundation work.

## Unblocks

- ASCX descriptor analyzer (next step)
- Custom control scaffolder (later)
- Prescan reporting

## Not in Scope

- Actually loading and inspecting custom control assemblies (P2)
- Generating BWFC shims for custom controls (P2)
- Converting custom controls to Blazor components (P1–P2)
