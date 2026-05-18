# Copilot Skills for AI-Assisted Migration

BlazorWebFormsComponents provides specialized **Copilot Skills** that enable AI-assisted migration from Web Forms to Blazor. These skills are frontmatter-based instruction files that give GitHub Copilot deep knowledge of BWFC migration patterns, shim infrastructure, and best practices.

---

## What Are Copilot Skills?

Copilot Skills are markdown files with YAML frontmatter that contain:

- **Specialized knowledge** about specific migration scenarios
- **Pattern recognition rules** for transforming Web Forms patterns to Blazor
- **Decision frameworks** for architecture choices
- **Anti-pattern detection** to avoid common mistakes

When you reference a skill in a Copilot session, it gains context-specific expertise for that migration area.

---

## Available Skills

BlazorWebFormsComponents provides three specialized migration skills:

### 1. Core Migration Skill
**File:** `migration-toolkit/skills/bwfc-migration/SKILL.md`

**Purpose:** Handles the core Web Forms  Blazor markup and code-behind transformations (Layer 2 work).

**What it covers:**
- Control tag transformations (`asp:Button`  `Button`)
- Expression conversions (`<%: %>`  `@()`)
- Data binding patterns (`SelectMethod`, `ItemType`, templates)
- Event handler signature updates
- Master Page  Layout conversions
- Shim usage patterns (Session, Response, Request, Cache)

**When to use:** During Layer 2 migration when converting `.razor` files after the automated Layer 1 transforms have run.

 **[Read the Core Migration documentation ](CoreMigration.md)**

---

### 2. Identity & Authentication Migration Skill
**File:** `migration-toolkit/skills/bwfc-identity-migration/SKILL.md`

**Purpose:** Guides migration of authentication and authorization from Web Forms to Blazor.

**What it covers:**
- ASP.NET Identity (OWIN)  ASP.NET Core Identity
- ASP.NET Membership  ASP.NET Core Identity
- FormsAuthentication migration
- Cookie authentication under Interactive Server mode
- Login control migration (Login, LoginView, ChangePassword)
- Role-based authorization patterns

**When to use:** When migrating login pages, account management, or any authentication-dependent features.

 **[Read the Identity Migration documentation ](IdentityMigration.md)**

---

### 3. Data & Architecture Migration Skill
**File:** `migration-toolkit/skills/bwfc-data-migration/SKILL.md`

**Purpose:** Handles Layer 3 architecture decisions and data access pattern migrations.

**What it covers:**
- Entity Framework 6  EF Core migrations
- DataSource controls  service injection patterns
- Session state to scoped services (when needed beyond SessionShim)
- Global.asax  Program.cs middleware
- Web.config  appsettings.json
- HTTP handlers/modules  middleware

**When to use:** During Layer 3 work when making architectural decisions about data access, state management, and application structure.

 **[Read the Data Migration documentation ](DataMigration.md)**

---

## How to Use These Skills

### Option 1: Copy Skills to Your Project (Recommended)

Copy the `skills/` directory from the migration-toolkit into your project:

```bash
# From your Blazor project root
mkdir -p .github/skills
cp -r path/to/bwfc-repo/migration-toolkit/skills/* .github/skills/
```

Then reference the skill in Copilot Chat:

```
@workspace Use the migration rules in .github/skills/bwfc-migration/SKILL.md 
to complete the migration of this file.
```

### Option 2: Reference Skills from the BWFC Repository

If you have the BWFC repository cloned locally or as a submodule, reference skills directly:

```
Use the migration patterns from migration-toolkit/skills/bwfc-migration/SKILL.md 
to transform this page.
```

### Option 3: Use the Copilot Instructions Template

For team-wide migration projects, use the `copilot-instructions-template.md` file to create project-specific Copilot instructions that reference all three skills:

```bash
cp migration-toolkit/copilot-instructions-template.md .github/copilot-instructions.md
# Edit the template to fill in project-specific details
```

---

## Skill Selection Guide

Use this table to choose the right skill for your current migration task:

| Migration Task | Use This Skill |
|----------------|----------------|
| Converting `.aspx`  `.razor` markup | **Core Migration** |
| Updating code-behind lifecycle methods | **Core Migration** |
| Master Page  Layout conversion | **Core Migration** |
| Data binding and templates | **Core Migration** |
| Login page migration | **Identity & Auth** |
| ASP.NET Identity migration | **Identity & Auth** |
| Cookie auth setup | **Identity & Auth** |
| EF6  EF Core migration | **Data & Architecture** |
| SqlDataSource replacement | **Data & Architecture** |
| Session state architecture | **Data & Architecture** |
| Global.asax  Program.cs | **Data & Architecture** |

---

## Best Practices

### 1. Use Skills in Order

Follow the three-layer migration pipeline:

1. **Layer 1**  Run automated transforms (CLI or PowerShell script)
2. **Layer 2**  Use **Core Migration** skill for markup/code-behind
3. **Layer 3**  Use **Data & Architecture** skill for architectural decisions
4. Use **Identity & Auth** skill as needed when you encounter authentication

### 2. Reference Skills Explicitly

When asking Copilot for help, explicitly reference the skill file:

```
Using the patterns from .github/skills/bwfc-migration/SKILL.md, 
convert this Master Page to a Blazor Layout component.
```

### 3. Review AI Suggestions

Always review Copilot's suggestions before accepting them. The skills provide high-quality guidance, but context-specific nuances may require human judgment.

### 4. Combine Skills When Needed

Some pages may need multiple skills. For example, a login page needs both **Core Migration** (for markup) and **Identity & Auth** (for authentication logic).

---

## Skill Maintenance

The skills are maintained in the BWFC repository and updated as:

- New BWFC components are released
- Shim infrastructure evolves
- Migration patterns are refined based on community feedback

Check the [BWFC repository](https://github.com/FritzAndFriends/BlazorWebFormsComponents/tree/dev/migration-toolkit/skills) for the latest versions.

---

## Related Resources

- [Quick Start Guide](../QuickStart.md)  Step-by-step migration walkthrough
- [Three-Layer Methodology](../Methodology.md)  Understanding the migration pipeline
- [Control Coverage Reference](../ControlCoverage.md)  Which controls are supported
- [Migration Checklist Template](../ChecklistTemplate.md)  Per-page progress tracking
