# ContosoUniversity Acceptance Tests

Playwright-based end-to-end acceptance tests for the **original** ContosoUniversity ASP.NET Web Forms application. These tests verify that every page loads, renders data correctly, and supports its expected interactions (CRUD, search, sort, filter).

## Prerequisites

- .NET 10.0 SDK
- ContosoUniversity Web Forms app running (IIS Express or IIS)
- SQL Server LocalDB (for the ContosoUniversity database)
- Playwright browsers (installed automatically on first run)

## Configuration

Set the base URL of the ContosoUniversity site via environment variable:

```powershell
# PowerShell
$env:CONTOSO_BASE_URL = "http://localhost:44380"
```

```bash
# Bash
export CONTOSO_BASE_URL="http://localhost:44380"
```

Defaults to `http://localhost:44380` if not set.

## How to Start the ContosoUniversity App

1. Open `samples/ContosoUniversity/ContosoUniversity.sln` in Visual Studio
2. Press F5 to run with IIS Express (or configure your preferred web server)
3. Note the port number — update `CONTOSO_BASE_URL` if it differs from 44380

## Running Tests

```powershell
# Build and install Playwright browsers
dotnet build src/ContosoUniversity.AcceptanceTests
pwsh src/ContosoUniversity.AcceptanceTests/bin/Debug/net10.0/playwright.ps1 install chromium

# Set the base URL (adjust port if needed)
$env:CONTOSO_BASE_URL = "http://localhost:44380"

# Run all tests
dotnet test src/ContosoUniversity.AcceptanceTests
```

## Test Coverage (40 test cases)

| Test Class | Count | Scenarios |
|------------|-------|-----------|
| **NavigationTests** | 11 | Master page renders nav links, each nav link navigates to correct page (×5), all pages return HTTP 200 (×5) |
| **HomePageTests** | 4 | Page loads, welcome text present, site branding renders, footer present |
| **AboutPageTests** | 5 | Page loads, has page title, GridView renders as table, has expected columns (Date + Count), has data rows |
| **StudentsPageTests** | 9 | Page loads, GridView shows data, GridView has columns, search by name, DetailsView shows details, add new student, edit student, delete student, clear button resets form |
| **CoursesPageTests** | 6 | Page loads, department dropdown has options, selecting department filters courses, GridView has course columns, search by name shows DetailsView, pagination works |
| **InstructorsPageTests** | 5 | Page loads, GridView shows instructors, GridView has expected columns, column click sorts grid, sort toggles direction |

## Notes

- **UpdatePanel**: Students, Courses, and Instructors pages use AJAX UpdatePanels. Tests use `WaitForLoadStateAsync(LoadState.NetworkIdle)` to handle partial page updates.
- **Web Forms IDs**: ASP.NET Web Forms generates IDs with naming container prefixes (e.g., `ContentPlaceHolder1_grv`). Tests use `[id*='...']` partial attribute selectors for resilience.
- **Proactive tests**: These tests were written based on the known page inventory. Minor selector adjustments may be needed once running against the live app.
