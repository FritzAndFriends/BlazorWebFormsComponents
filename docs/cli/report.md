# Migration Report Format

After running the CLI tool, a `migration-report.json` file is generated with detailed information about the transformation. This document describes the report schema and how to interpret it.

## Report Structure

The report is a JSON document with the following top-level fields:

```json
{
  "migrationDate": "2026-04-02T15:30:45Z",
  "sourceProject": "/home/user/MyApp.Web",
  "outputProject": "/home/user/MyApp.Blazor",
  "projectName": "MyApp.Blazor",
  "projectFramework": "net8.0",
  "toolVersion": "1.0.0",
  "summary": {
    "totalFiles": 45,
    "filesProcessed": 42,
    "filesFailed": 0,
    "filesSkipped": 3,
    "totalTransforms": 1247,
    "totalManualItems": 156,
    "criticalIssues": 5,
    "warnings": 18,
    "infos": 133
  },
  "files": [ /* array of file results */ ],
  "manualItems": [ /* array of manual work items */ ],
  "categories": [ /* grouped manual items by TODO category */ ]
}
```

---

## Summary Section

| Field | Type | Description |
|-------|------|-------------|
| `migrationDate` | string (ISO 8601) | When the migration was run |
| `sourceProject` | string | Path to original Web Forms project |
| `outputProject` | string | Path to new Blazor project |
| `projectName` | string | Name of the generated Blazor project |
| `projectFramework` | string | Target framework (e.g., `net8.0`) |
| `toolVersion` | string | CLI tool version |
| **summary.totalFiles** | integer | Total `.aspx`, `.ascx`, `.master` files found |
| **summary.filesProcessed** | integer | Files successfully converted |
| **summary.filesFailed** | integer | Files with conversion errors |
| **summary.filesSkipped** | integer | Files not processed (e.g., excluded) |
| **summary.totalTransforms** | integer | Total transforms applied across all files |
| **summary.totalManualItems** | integer | Number of TODO comments inserted |
| **summary.criticalIssues** | integer | Count of severity="Error" items |
| **summary.warnings** | integer | Count of severity="Warning" items |
| **summary.infos** | integer | Count of severity="Info" items |

---

## Files Array

Each file in the `files` array represents a processed source file:

```json
{
  "sourceFile": "Pages/Product.aspx",
  "outputFile": "Pages/Product.razor",
  "fileType": "Page",
  "status": "Success",
  "linesAdded": 12,
  "linesRemoved": 8,
  "transformsApplied": [
    {
      "name": "PageDirective",
      "count": 1,
      "description": "Converted <%@ Page %> to @page"
    },
    {
      "name": "ExpressionTransform",
      "count": 5,
      "description": "Converted <%: %> expressions to @()"
    }
  ],
  "manualItems": [
    {
      "line": 45,
      "category": "bwfc-datasource",
      "severity": "Warning",
      "message": "DataSourceID='ProductSource' → wire Items binding and data service"
    }
  ]
}
```

### File Fields

| Field | Type | Description |
|-------|------|-------------|
| `sourceFile` | string | Relative path in Web Forms project |
| `outputFile` | string | Relative path in Blazor project |
| `fileType` | string | "Page", "Control", "Master" |
| `status` | string | "Success", "Warning", "Error", "Skipped" |
| `linesAdded` | integer | New lines in output (includes TODO comments) |
| `linesRemoved` | integer | Lines removed during conversion |
| `transformsApplied` | array | List of transforms run on this file |
| `manualItems` | array | TODO comments with line numbers |

---

## ManualItem Schema

Each `manualItem` represents a TODO comment that requires manual follow-up:

```json
{
  "file": "Pages/Product.aspx",
  "line": 45,
  "column": 0,
  "category": "bwfc-datasource",
  "severity": "Warning",
  "message": "DataSourceID attribute detected — implement IProductDataService and wire Items binding",
  "suggestion": "// TODO(bwfc-datasource): Replace DataSourceID with Items=\"@ProductList\" and create data service",
  "relatedTransforms": ["DataSourceIdTransform", "SelectMethodTransform"]
}
```

### ManualItem Fields

| Field | Type | Description |
|-------|------|-------------|
| `file` | string | Relative file path |
| `line` | integer | Line number in output file |
| `column` | integer | Column number (0-based) |
| `category` | string | TODO category slug (e.g., `bwfc-lifecycle`) |
| `severity` | string | "Info", "Warning", "Error" |
| `message` | string | Human-readable description |
| `suggestion` | string | Code suggestion or TODO template |
| `relatedTransforms` | array | Transforms that created this item |

### Severity Levels

| Severity | Meaning | Action |
|----------|---------|--------|
| **Error** | Blocking issue — project likely won't compile | Fix immediately |
| **Warning** | Code will compile but behavior may differ | Review before building |
| **Info** | Guidance comment — migrate at your pace | Reference during L2 automation |

---

## Categories Summary

The `categories` section groups manual items by TODO category:

```json
{
  "categories": [
    {
      "category": "bwfc-lifecycle",
      "count": 8,
      "severity": "Warning",
      "files": [
        "Pages/Product.aspx",
        "Pages/Checkout.aspx"
      ],
      "description": "Page lifecycle methods need conversion to Blazor component lifecycle"
    },
    {
      "category": "bwfc-datasource",
      "count": 12,
      "severity": "Info",
      "files": [
        "Pages/Product.aspx",
        "Pages/Search.aspx",
        "Controls/ProductCard.ascx"
      ],
      "description": "Data binding patterns need to be replaced with component data services"
    }
  ]
}
```

### Category Summary Fields

| Field | Type | Description |
|-------|------|-------------|
| `category` | string | TODO category slug |
| `count` | integer | Number of items in this category |
| `severity` | string | Highest severity in this category |
| `files` | array | Files affected (unique list) |
| `description` | string | What needs to be done |

---

## Example Report Output

### Minimal Success Report

```json
{
  "migrationDate": "2026-04-02T14:30:00Z",
  "sourceProject": "./MyApp.Web",
  "outputProject": "./MyApp.Blazor",
  "projectName": "MyApp.Blazor",
  "toolVersion": "1.0.0",
  "summary": {
    "totalFiles": 5,
    "filesProcessed": 5,
    "filesFailed": 0,
    "filesSkipped": 0,
    "totalTransforms": 87,
    "totalManualItems": 12,
    "criticalIssues": 0,
    "warnings": 3,
    "infos": 9
  },
  "files": [
    {
      "sourceFile": "Default.aspx",
      "outputFile": "Pages/Index.razor",
      "fileType": "Page",
      "status": "Success",
      "linesAdded": 3,
      "linesRemoved": 2,
      "transformsApplied": [
        {"name": "PageDirective", "count": 1},
        {"name": "ExpressionTransform", "count": 6},
        {"name": "AspPrefixTransform", "count": 4}
      ],
      "manualItems": [
        {
          "line": 12,
          "category": "bwfc-datasource",
          "severity": "Info",
          "message": "Review data binding pattern"
        }
      ]
    }
  ],
  "categories": [
    {
      "category": "bwfc-datasource",
      "count": 4,
      "severity": "Info",
      "files": ["Default.aspx"]
    }
  ]
}
```

### Report with Errors

```json
{
  "summary": {
    "totalFiles": 10,
    "filesProcessed": 8,
    "filesFailed": 2,
    "filesSkipped": 0,
    "totalManualItems": 45,
    "criticalIssues": 3,
    "warnings": 15,
    "infos": 27
  },
  "files": [
    {
      "sourceFile": "Controls/CustomControl.ascx",
      "outputFile": "Components/CustomControl.razor",
      "fileType": "Control",
      "status": "Error",
      "statusMessage": "Parser error on line 34: unmatched closing tag",
      "manualItems": [
        {
          "line": 34,
          "severity": "Error",
          "category": "bwfc-general",
          "message": "Malformed markup — review original file for syntax errors"
        }
      ]
    }
  ]
}
```

---

## How to Use the Report

### 1. Review Critical Issues First

```bash
# Filter for errors (blocks compilation)
jq '.manualItems[] | select(.severity == "Error")' migration-report.json

# Count by category
jq 'group_by(.category) | map({category: .[0].category, count: length})' \
  <(jq '.manualItems[] | select(.severity == "Error")' migration-report.json)
```

### 2. Check Summary Statistics

```bash
# Print summary
jq '.summary' migration-report.json
```

### 3. Find Work by Category

```bash
# All lifecycle TODOs
jq '.manualItems[] | select(.category == "bwfc-lifecycle")' migration-report.json

# All warnings grouped by category
jq 'group_by(.category) | map({category: .[0].category, count: length})' \
  <(jq '.manualItems[] | select(.severity == "Warning")' migration-report.json)
```

### 4. Build a Worklist

```bash
# Export TODOs for a specific file
jq '.manualItems[] | select(.file == "Product.aspx")' migration-report.json | \
  jq -s 'sort_by(.line)' | \
  jq '.[] | "\(.line): [\(.severity)] \(.category) - \(.message)"'
```

### 5. Track Automation Progress

```bash
# Before L2 automation
jq '.summary' migration-report-before.json

# After L2 automation (lifecycle conversion)
jq '.summary' migration-report-after-lifecycle.json

# Calculate improvement
```

---

## Report Interpretation Guide

### What Errors Mean

| Error | Cause | Resolution |
|-------|-------|-----------|
| `Parser error: unmatched closing tag` | Malformed markup in source | Review original `.aspx/.ascx` file for syntax errors |
| `File not found` | Referenced include or code-behind missing | Check project file references |
| `Unsupported directive` | Web Forms-specific directive without Blazor equivalent | Manual conversion needed |

### What Warnings Mean

| Warning | Meaning | Action |
|---------|---------|--------|
| `DataSourceID detected` | Needs data service wiring | Implement L2 datasource automation |
| `ViewState usage found` | State pattern conversion needed | Create component fields or parameters |
| `Complex IsPostBack guard` | Hard to unwrap automatically | Review unwrapped code, may need manual adjustment |

### What Infos Mean

| Info | Meaning | Action |
|------|---------|--------|
| `TODO comment inserted` | Guidance for developer | Review alongside code during L2 automation |
| `Pattern detected` | Recognized Web Forms pattern | L2 skill can automate this |

---

## Example Workflow: Using the Report to Plan L2 Automation

```bash
#!/bin/bash

# 1. Run initial migration
webforms-to-blazor migrate --input ./MyApp.Web --output ./MyApp.Blazor

# 2. Check what needs manual work
echo "=== CRITICAL ISSUES ==="
jq '.summary.criticalIssues' MyApp.Blazor/migration-report.json

# 3. Fix errors first
if [ $(jq '.summary.criticalIssues' MyApp.Blazor/migration-report.json) -gt 0 ]; then
  echo "Fixing critical issues..."
  jq '.manualItems[] | select(.severity == "Error")' MyApp.Blazor/migration-report.json
fi

# 4. Plan L2 passes by category count
echo "=== WORK BY CATEGORY ==="
jq '.categories | sort_by(-(.count)) | .[] | "\(.count) x \(.category)"' MyApp.Blazor/migration-report.json

# 5. Run L2 for high-impact categories
# (Use Copilot L2 skills for each category)
copilot /webforms-migration --focus bwfc-lifecycle
copilot /webforms-migration --focus bwfc-datasource
copilot /webforms-migration --focus bwfc-validation

# 6. Generate updated report
webforms-to-blazor migrate --input ./MyApp.Web --output ./MyApp.Blazor --dry-run > migration-report-updated.json
```

---

## Next Steps

- **[TODO Categories](todo-conventions.md)** — Understand what each TODO category means
- **[Transform Reference](transforms.md)** — Learn what each transform does
- **[Back to CLI Overview](index.md)** — Return to main CLI documentation
