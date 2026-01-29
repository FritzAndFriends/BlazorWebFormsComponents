# .NET Standard Class Library Migration Sample

This sample demonstrates how to update an existing .NET Framework class library to target .NET Standard, enabling code sharing between Web Forms and Blazor applications.

## Overview

This sample shows the step-by-step process of migrating a class library from .NET Framework 4.5 to .NET Standard 2.0. This is a key strategy for enabling code reuse during Web Forms to Blazor migrations.

## Why .NET Standard?

By targeting .NET Standard 2.0, your class library can be referenced by:

- ASP.NET Web Forms (.NET Framework 4.6.1+)
- ASP.NET Core / Blazor Server
- Blazor WebAssembly
- Xamarin mobile applications
- .NET MAUI applications

## Sample Structure

```
netstandard-1/
├── Before/
│   └── MyClassLibrary/           # Original .NET Framework 4.5 project
│       ├── MyClassLibrary.csproj
│       └── BusinessLogic.cs
│
└── After/
    └── MyClassLibrary/           # Migrated .NET Standard 2.0 project
        ├── MyClassLibrary.csproj
        └── BusinessLogic.cs
```

## Migration Steps

### Step 1: Analyze the Project

Before migrating, use the [.NET Portability Analyzer](https://docs.microsoft.com/en-us/dotnet/standard/analyzers/portability-analyzer) to identify any APIs that may not be available in .NET Standard.

### Step 2: Update the Project File

**Before (.NET Framework 4.5):**

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
    <OutputType>Library</OutputType>
    <!-- ... other properties ... -->
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="System" />
    <Reference Include="System.Core" />
    <!-- ... other references ... -->
  </ItemGroup>
  <ItemGroup>
    <Compile Include="BusinessLogic.cs" />
    <!-- ... other files ... -->
  </ItemGroup>
</Project>
```

**After (.NET Standard 2.0 - SDK style):**

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
  </PropertyGroup>

</Project>
```

### Step 3: Address Compatibility Issues

Common issues to address:

1. **System.Web dependencies** - These are not available in .NET Standard. Move web-specific code to the presentation layer.

2. **System.Configuration** - Use `Microsoft.Extensions.Configuration` instead.

3. **Entity Framework** - Use Entity Framework Core or ensure you're using a compatible EF version.

### Step 4: Reference from Both Projects

Once migrated, reference the .NET Standard library from both your Web Forms and Blazor applications:

```xml
<!-- In Web Forms .csproj -->
<ProjectReference Include="..\MyClassLibrary\MyClassLibrary.csproj" />

<!-- In Blazor .csproj -->
<ProjectReference Include="..\MyClassLibrary\MyClassLibrary.csproj" />
```

## Benefits

1. **Code Reuse** - Share business logic between old and new applications
2. **Gradual Migration** - Migrate the presentation layer while keeping business logic stable
3. **Testability** - Business logic is now isolated and easier to unit test
4. **Future Flexibility** - .NET Standard libraries work with any .NET implementation

## Learn More

- [.NET Standard Overview](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)
- [.NET Standard Version Table](https://docs.microsoft.com/en-us/dotnet/standard/net-standard#net-implementation-support)
- [.NET Portability Analyzer](https://docs.microsoft.com/en-us/dotnet/standard/analyzers/portability-analyzer)

## See Also

- [.NET Standard to the Rescue](../../Migration/NET-Standard.md) - Migration guide overview
- [Migration Strategies](../../Migration/Strategies.md) - Overall migration approaches
