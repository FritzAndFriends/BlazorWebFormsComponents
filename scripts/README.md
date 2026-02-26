# Scripts

This directory contains scripts for release management and development tooling for BlazorWebFormsComponents.

---

## BeforeWebForms IIS Express Setup

### Setup-IISExpress.ps1

Automates setup and launch of the BeforeWebForms (.NET Framework 4.8) sample app under IIS Express for the HTML audit. The project uses dynamic compilation (CodeFile= directives) instead of MSBuild.

**Prerequisites:**
- .NET Framework 4.8
- IIS Express (comes with Visual Studio or standalone install)
- `git` in PATH

**Usage:**
```powershell
# Full setup + launch on default port 55501
.\scripts\Setup-IISExpress.ps1

# Custom port, no browser launch
.\scripts\Setup-IISExpress.ps1 -Port 8080 -NoBrowser

# Revert temporary file changes (restores git state)
.\scripts\Setup-IISExpress.ps1 -Revert
```

**What it does:**
1. Converts `CodeBehind=` to `CodeFile=` in all .aspx/.ascx/.master files (enables dynamic compilation without MSBuild)
2. Adds `partial` keyword to `Global.asax.cs` if missing
3. Restores NuGet packages via `nuget.exe` (downloads nuget.exe if not found)
4. Copies required DLLs from NuGet packages to `samples/BeforeWebForms/bin/`
5. Launches IIS Express serving the sample app

**Important:** The CodeBehind→CodeFile changes are temporary and should **not** be committed. Use `-Revert` to undo them.

---

## Release Scripts

Scripts for version publishing and release management using Nerdbank.GitVersioning.

## Prerequisites

- `git` - Git version control
- `nbgv` - Nerdbank.GitVersioning CLI tool
- `gh` (optional) - GitHub CLI for creating releases directly

Install nbgv globally:
```bash
dotnet tool install -g nbgv
```

Install GitHub CLI (optional):
```bash
# See: https://github.com/cli/cli#installation
```

## Scripts

### 1. prepare-release.sh

Prepares a new release by updating the version number and generating release notes using Nerdbank.GitVersioning.

**Usage:**
```bash
./scripts/prepare-release.sh <version>
```

**Example:**
```bash
./scripts/prepare-release.sh 0.14
```

**What it does:**
- Uses `nbgv set-version` to update `version.json` with the new version number
- Generates release notes from commits in the dev branch since the last merge to main
- Saves release notes to `RELEASE_NOTES.md`
- Provides instructions for the next steps

**Best practices:**
- Run this script on the `dev` branch after all features for the release are merged
- Review the generated `RELEASE_NOTES.md` and edit if needed
- Commit the changes before merging to main

### 2. generate-release-notes.sh

Generates release notes from git commits since the last merge to main.

**Usage:**
```bash
./scripts/generate-release-notes.sh
```

This script is automatically called by `prepare-release.sh`, but can also be run standalone to preview release notes.

**What it does:**
- Finds commits since the last merge to main
- Categorizes commits by type (features, bug fixes, documentation, etc.)
- Lists all contributors
- Outputs formatted release notes to stdout

### 3. publish-release.sh

Creates a git tag and publishes the release using Nerdbank.GitVersioning.

**Usage:**
```bash
./scripts/publish-release.sh
```

**Example:**
```bash
./scripts/publish-release.sh
```

**What it does:**
- Validates that you're on the `main` branch
- Reads the version from `version.json`
- Uses `nbgv tag` to create a git tag based on the current version
- Pushes the tag to GitHub
- Provides instructions for creating a GitHub release

**Important:**
- This script must be run on the `main` branch after merging dev
- No version argument is needed - the version is determined from `version.json`
- The tag push will automatically trigger the NuGet package build and publish via GitHub Actions

## Release Workflow

Here's the complete workflow for releasing a new version using Nerdbank.GitVersioning:

### Step 1: Prepare the Release on Dev Branch

1. Checkout the `dev` branch:
   ```bash
   git checkout dev
   git pull origin dev
   ```

2. Run the prepare script:
   ```bash
   ./scripts/prepare-release.sh 0.14
   ```

3. Review the changes:
   ```bash
   cat version.json
   cat RELEASE_NOTES.md
   ```

4. Edit `RELEASE_NOTES.md` if needed to add context or clean up commit messages

5. Commit the changes:
   ```bash
   git add version.json RELEASE_NOTES.md
   git commit -m "Prepare release v0.14"
   git push origin dev
   ```

### Step 2: Merge Dev to Main

1. Create a pull request from `dev` to `main` on GitHub
2. Wait for CI checks to pass
3. Merge the pull request

### Step 3: Publish the Release

1. Checkout and update `main`:
   ```bash
   git checkout main
   git pull origin main
   ```

2. Run the publish script (no version argument needed):
   ```bash
   ./scripts/publish-release.sh
   ```

3. Create the GitHub release:
   
   **Option A: Using GitHub CLI**
   ```bash
   gh release create v0.14 --title "Release v0.14" --notes-file RELEASE_NOTES.md
   ```
   
   **Option B: Manually on GitHub**
   - Go to https://github.com/FritzAndFriends/BlazorWebFormsComponents/releases/new?tag=v0.14
   - Copy the contents of `RELEASE_NOTES.md` into the description
   - Click "Publish release"

### Step 4: Verify the Release

1. Check that the GitHub Actions workflow completed successfully
2. Verify the NuGet package was published to GitHub Packages
3. Check the Docker image was published (if applicable)
4. Verify the live demo site was updated on Azure

## Commit Message Conventions

To make release notes more useful, consider using conventional commit prefixes:

- `feat:` or `feature:` - New features
- `fix:` or `bug:` - Bug fixes
- `docs:` or `documentation:` - Documentation changes
- `test:` or `tests:` - Test changes
- `refactor:` - Code refactoring
- `chore:` or `build:` or `ci:` - Build/CI maintenance

Example:
```bash
git commit -m "feat: Add DataGrid pagination support"
git commit -m "fix: Resolve TreeView node selection issue"
git commit -m "docs: Update GridView documentation with examples"
```

## Troubleshooting

### "nbgv: command not found"

Install Nerdbank.GitVersioning CLI:
```bash
dotnet tool install -g nbgv
```

### Tag already exists

If you need to re-create a tag:
```bash
git tag -d v0.14          # Delete locally
git push origin :v0.14    # Delete remotely
```

### "No git repo found" error from nbgv

This can happen in shallow clones. Try deepening the clone:
```bash
git fetch --unshallow
```
