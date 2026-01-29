# Blazor WebForms Components - Integration Tests

This project contains Playwright-based integration tests for the AfterBlazorServerSide sample application.

## Overview

These tests verify that:
- Sample pages load successfully without errors
- No console errors occur during page rendering
- Components render correctly
- Navigation works as expected

## Test Categories

### Home Page Tests (`HomePageTests.cs`)
- Verifies the home page loads successfully
- Checks for console errors
- Validates navigation menu presence

### Component List Tests (`ComponentListPageTests.cs`)
- Verifies the component list page loads
- Checks for console errors
- Validates component links are present

### Control Sample Tests (`ControlSampleTests.cs`)
- Tests all control sample pages across different categories:
  - Editor Controls (Button, CheckBox, HyperLink, etc.)
  - Data Controls (DataList, GridView, Repeater, etc.)
  - Navigation Controls (TreeView)
  - Validation Controls
  - Login Controls
  - Other Controls (AdRotator)

## Prerequisites

Before running the tests, you need to install Playwright browsers:

```bash
pwsh samples/AfterBlazorServerSide.Tests/bin/Debug/net10.0/playwright.ps1 install
```

Or on Linux/macOS:

```bash
dotnet build samples/AfterBlazorServerSide.Tests
playwright install
```

## Running the Tests

### Run all integration tests:
```bash
dotnet test samples/AfterBlazorServerSide.Tests
```

### Run specific test class:
```bash
dotnet test samples/AfterBlazorServerSide.Tests --filter "FullyQualifiedName~HomePageTests"
```

### Run with verbose output:
```bash
dotnet test samples/AfterBlazorServerSide.Tests --verbosity normal
```

## How It Works

The tests use:
- **xUnit** as the test framework
- **Playwright** for browser automation
- **WebApplicationFactory** to host the sample application in-memory during tests

Each test:
1. Starts the Blazor server application
2. Launches a headless browser
3. Navigates to the test page
4. Verifies no errors occur
5. Cleans up resources

## CI/CD Integration

These tests run automatically in GitHub Actions as part of the `integration-tests.yml` workflow on:
- Pull requests
- Pushes to main/dev branches
- Manual workflow dispatch

## Troubleshooting

### Browser Not Found
If you get an error about missing browsers, run:
```bash
playwright install chromium
```

### Port Already in Use
The tests use a random port for each run. If you encounter port issues, ensure no other instances are running.

### Timeout Errors
Some tests have a 30-second timeout. If tests fail with timeout errors, it may indicate:
- Performance issues with the sample application
- Network connectivity problems
- Issues with specific components

## Adding New Tests

To add tests for new sample pages:

1. Add the page route to the appropriate `[Theory]` in `ControlSampleTests.cs`
2. Or create a new test class if testing a new feature area

Example:
```csharp
[Theory]
[InlineData("/ControlSamples/NewControl")]
public async Task NewControl_Loads_WithoutErrors(string path)
{
    await VerifyPageLoadsWithoutErrors(path);
}
```
