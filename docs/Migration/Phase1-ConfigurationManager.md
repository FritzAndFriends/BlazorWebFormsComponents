# ConfigurationManager Migration

The `ConfigurationManager` shim allows migrated Web Forms code to access application settings and connection strings without modification. Your existing `ConfigurationManager.AppSettings["key"]` and `ConfigurationManager.ConnectionStrings["name"]` calls work unchanged in Blazor.

## Overview

**What it does:**
- Provides a static `ConfigurationManager` class in the `BlazorWebFormsComponents` namespace
- Enables `AppSettings["key"]` access backed by ASP.NET Core `IConfiguration`
- Enables `ConnectionStrings["name"]` access via `GetConnectionString()`
- Allows migrated Business Logic Layer (BLL) and Data Access Layer (DAL) code to compile and run without code changes

**Why it matters:**
When migrating a Web Forms application, your BLL/DAL code often uses `ConfigurationManager` to read database connection strings and application settings. The shim eliminates the need to refactor that code during the initial migration phase, letting you focus on UI layer migration first.

## Before and After

=== "Web Forms (Original)"
    ```csharp
    // App_Start/ProductRepository.cs - BLL code
    using System.Configuration;
    
    public class ProductRepository
    {
        public ProductRepository()
        {
            // Access connection string from web.config
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            _dbConnection = new SqlConnection(connStr);
        }
        
        public void SaveTimeout()
        {
            // Access app setting from web.config
            int timeout = int.Parse(ConfigurationManager.AppSettings["DBTimeout"] ?? "30");
            _dbConnection.ConnectionTimeout = timeout;
        }
    }
    
    // web.config
    <configuration>
      <connectionStrings>
        <add name="DefaultConnection" connectionString="Server=.;Database=MyApp;Integrated Security=true;" />
      </connectionStrings>
      <appSettings>
        <add key="DBTimeout" value="30" />
        <add key="ApiKey" value="secret123" />
      </appSettings>
    </configuration>
    ```

=== "Blazor with BWFC (Using the Shim)"
    ```csharp
    // Shared/ProductRepository.cs - Same code, no changes!
    using BlazorWebFormsComponents;
    
    public class ProductRepository
    {
        public ProductRepository()
        {
            // Same code as Web Forms — ConfigurationManager just works
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            _dbConnection = new SqlConnection(connStr);
        }
        
        public void SaveTimeout()
        {
            // Same code as Web Forms
            int timeout = int.Parse(ConfigurationManager.AppSettings["DBTimeout"] ?? "30");
            _dbConnection.ConnectionTimeout = timeout;
        }
    }
    
    // appsettings.json (created by L1 migration script)
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=.;Database=MyApp;Integrated Security=true;"
      },
      "AppSettings": {
        "DBTimeout": "30",
        "ApiKey": "secret123"
      }
    }
    ```

## Setup: Initialize in Program.cs

To enable the shim, call the initialization helper in `Program.cs`:

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Register BWFC services and initialize ConfigurationManager shim
builder.Services.AddBlazorWebFormsComponents();

var app = builder.Build();

// ... configure middleware ...

app.Run();
```

The `AddBlazorWebFormsComponents()` extension automatically initializes the shim with the application's `IConfiguration`. No additional setup needed.

### Manual Initialization (Optional)

If you prefer manual control, you can initialize directly:

```csharp
// Program.cs
builder.Services.AddBlazorWebFormsComponents();

var app = builder.Build();

// Manually initialize if needed (e.g., for unit tests)
BlazorWebFormsComponents.ConfigurationManager.Initialize(app.Services.GetRequiredService<IConfiguration>());

app.Run();
```

## How It Works

The shim reads from ASP.NET Core's `IConfiguration` in this order:

### AppSettings Access

```csharp
ConfigurationManager.AppSettings["DBTimeout"]
```

Tries these keys in order:
1. `AppSettings:DBTimeout` (from appsettings.json)
2. `DBTimeout` (flat key)
3. Returns `null` if not found

This dual-lookup pattern supports both structured config (`AppSettings` section) and flat keys.

### ConnectionStrings Access

```csharp
ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString
```

Returns a `ConnectionStringSettings` object with:
- `Name` — the connection string name
- `ConnectionString` — the actual connection string
- `ProviderName` — optional ADO.NET provider (default: empty)

Reads from the `ConnectionStrings` section in `appsettings.json`, accessed via `IConfiguration.GetConnectionString()`.

## web.config Mapping

The L1 migration script (`bwfc-migrate.ps1`) automatically converts `web.config` settings to `appsettings.json`:

=== "web.config"
    ```xml
    <configuration>
      <connectionStrings>
        <add name="DefaultConnection" 
             connectionString="Server=myserver;Database=mydb;" />
        <add name="LegacyDb" 
             connectionString="Server=oldserver;Database=olddb;" />
      </connectionStrings>
      <appSettings>
        <add key="DBTimeout" value="30" />
        <add key="MaxRetries" value="3" />
        <add key="FeatureFlag_NewUI" value="true" />
      </appSettings>
    </configuration>
    ```

=== "appsettings.json (Migrated)"
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=myserver;Database=mydb;",
        "LegacyDb": "Server=oldserver;Database=olddb;"
      },
      "AppSettings": {
        "DBTimeout": "30",
        "MaxRetries": "3",
        "FeatureFlag_NewUI": "true"
      }
    }
    ```

The shim then reads these values exactly as your Web Forms code expects.

## Configuration Precedence

For environment-specific overrides, use the standard ASP.NET Core configuration hierarchy:

```
appsettings.json
  ↓ (overridden by)
appsettings.Development.json
  ↓ (overridden by)
appsettings.Production.json
  ↓ (overridden by)
Environment variables
  ↓ (overridden by)
User Secrets (Development only)
```

Example:

```json
// appsettings.json (default)
{
  "AppSettings": { "DBTimeout": "30" }
}

// appsettings.Production.json (production override)
{
  "AppSettings": { "DBTimeout": "60" }
}
```

When running in Production, `ConfigurationManager.AppSettings["DBTimeout"]` returns `"60"`.

## Limitations and Known Issues

The shim does **not** support:

| Feature | Web Forms | BWFC Shim | Alternative |
|---------|-----------|-----------|-------------|
| Custom config sections | `<custom>...</custom>` | ❌ Not supported | Use structured JSON sections in appsettings.json |
| File references | `configSource="settings.xml"` | ❌ Not supported | Use separate JSON files + ASP.NET Core config providers |
| Encrypted settings | `encryptionProvider` attribute | ❌ Not supported | Use Azure Key Vault, AWS Secrets Manager, or .NET User Secrets |
| Nested app settings | `AppSettings.AppSettings["section"]["key"]` | ❌ Not supported | Use structured JSON: `"AppSettings": { "Section": { "Key": "value" } }` |

### Workaround: Custom Config Sections

If you have custom config sections, migrate them to structured JSON:

```csharp
// Web Forms: Custom section
<customSettings>
  <database host="localhost" port="5432" />
</customSettings>
var dbHost = config.GetSection("customSettings").GetValue<string>("database:host");

// BWFC: Structured JSON
{
  "CustomSettings": {
    "Database": {
      "Host": "localhost",
      "Port": 5432
    }
  }
}
var dbHost = config["CustomSettings:Database:Host"];
```

## Summary

The `ConfigurationManager` shim:
- ✅ Lets BLL/DAL code use `ConfigurationManager` unchanged
- ✅ Reads from ASP.NET Core `IConfiguration` (appsettings.json)
- ✅ Initializes automatically via `AddBlazorWebFormsComponents()`
- ✅ Supports both flat and nested configuration structures
- ❌ Does not support custom config sections or encryption

Use it for **Phase 1 ("Just Make It Compile")** migrations. Later, consider migrating to dependency injection for configuration access:

```csharp
// Phase 2+: Dependency Injection (Better Practice)
public class ProductRepository
{
    private readonly IConfiguration _config;
    
    public ProductRepository(IConfiguration config)
    {
        _config = config;
    }
    
    public void Initialize()
    {
        string connStr = _config.GetConnectionString("DefaultConnection");
        // ...
    }
}
```

See [Service Registration](../UtilityFeatures/ServiceRegistration.md) for more on dependency injection patterns in Blazor.
