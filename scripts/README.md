# Release Scripts

This directory contains scripts to help with version publishing and release management for BlazorWebFormsComponents.

## Prerequisites

- `git` - Git version control
- `jq` - JSON processor for manipulating version.json
- `gh` (optional) - GitHub CLI for creating releases directly

Install jq on Ubuntu/Debian:
```bash
sudo apt-get install jq
```

Install GitHub CLI (optional):
```bash
# See: https://github.com/cli/cli#installation
```

## Scripts

### 1. prepare-release.sh

Prepares a new release by updating the version number and generating release notes.

**Usage:**
```bash
./scripts/prepare-release.sh <version>
```

**Example:**
```bash
./scripts/prepare-release.sh 0.14
```

**What it does:**
- Updates `version.json` with the new version number
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

Creates a git tag and publishes the release.

**Usage:**
```bash
./scripts/publish-release.sh <version>
```

**Example:**
```bash
./scripts/publish-release.sh 0.14
```

**What it does:**
- Validates that you're on the `main` branch
- Checks that `version.json` matches the version you're releasing
- Creates a git tag (e.g., `v0.14`)
- Pushes the tag to GitHub
- Provides instructions for creating a GitHub release

**Important:**
- This script must be run on the `main` branch after merging dev
- The tag push will automatically trigger the NuGet package build and publish via GitHub Actions

## Release Workflow

Here's the complete workflow for releasing a new version:

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

2. Run the publish script:
   ```bash
   ./scripts/publish-release.sh 0.14
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

### "jq: command not found"

Install jq:
```bash
# Ubuntu/Debian
sudo apt-get install jq

# macOS
brew install jq
```

### Tag already exists

If you need to re-create a tag:
```bash
git tag -d v0.14          # Delete locally
git push origin :v0.14    # Delete remotely
```

### Version mismatch error

Make sure `version.json` has been updated to the version you're trying to release. Run `prepare-release.sh` first if you haven't already.
