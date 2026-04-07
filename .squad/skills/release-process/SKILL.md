---
name: "release-process"
description: "Complete release workflow for BlazorWebFormsComponents: version bump, PR from dev→main, GitHub Release creation, and post-release CI/CD pipeline that publishes NuGet packages, deploys docs to GitHub Pages, builds Docker images, and deploys sample sites. Use when preparing a release, creating a release PR, tagging a version, or troubleshooting release CI/CD."
domain: "release-workflow"
confidence: "medium"
source: "manual"
---

## Context

BlazorWebFormsComponents uses a two-branch model: `dev` (active development) and `main` (releases only). All feature work merges into `dev` via squash merge. Releases are cut by merging `dev` into `main` via a regular merge commit, then tagging and creating a GitHub Release.

The upstream repository is `FritzAndFriends/BlazorWebFormsComponents`. The fork is `csharpfritz/BlazorWebFormsComponents`.

## Release Workflow — Step by Step

### Prerequisites

- All feature PRs for this release are squash-merged into `upstream/dev`
- Local `dev` branch is synced: `git fetch upstream && git reset --hard upstream/dev && git push origin dev`
- CI is green on dev (Build and Test + Integration Tests passing)

### Step 1 — Bump Version

Version is managed by Nerdbank.GitVersioning (`version.json` at repo root).

```json
{
  "version": "0.19.0",
  "publicReleaseRefSpec": [
    "^refs/heads/master$",
    "^refs/heads/main$",
    "^refs/heads/v\\d+(?:\\.\\d+)?$",
    "^refs/tags/v\\d+\\.\\d+(\\.\\d+)?$"
  ],
  "release": {
    "firstUnstableTag": "preview"
  }
}
```

- For a **new release**: update `"version"` in `version.json` if the version has changed since last release.
- Commit the version bump to `dev` and push before creating the release PR.
- The version in `version.json` is the NEXT version after this release (NBGV auto-increments).

### Step 2 — Create Release PR

Create a PR from `upstream/dev` → `upstream/main`:

```bash
# Ensure dev is current
git checkout dev
git fetch upstream
git reset --hard upstream/dev
git push origin dev

# Create the release PR via gh CLI
gh pr create \
  --repo FritzAndFriends/BlazorWebFormsComponents \
  --base main \
  --head dev \
  --title "Release v{VERSION}" \
  --body "## Release v{VERSION}

### What's Included
{summary of features since last release}

### Checklist
- [ ] All CI checks passing
- [ ] Integration tests passing
- [ ] Version number correct in version.json
- [ ] Release notes prepared"
```

**Important:** This PR uses **regular merge** (NOT squash merge). This preserves the full commit history on main and keeps dev and main in sync.

### Step 3 — Merge the Release PR

Merge using **regular merge commit** (not squash, not rebase):

```bash
gh pr merge {PR_NUMBER} \
  --repo FritzAndFriends/BlazorWebFormsComponents \
  --merge \
  --subject "Release v{VERSION}"
```

### Step 4 — Create Git Tag and GitHub Release

After the merge to main:

```bash
# Fetch the merge commit
git fetch upstream
git checkout main
git reset --hard upstream/main

# Tag the release
git tag -a v{VERSION} -m "Release v{VERSION}"
git push upstream v{VERSION}

# Create GitHub Release (triggers CI/CD pipeline)
gh release create v{VERSION} \
  --repo FritzAndFriends/BlazorWebFormsComponents \
  --title "v{VERSION}" \
  --notes "{release notes}" \
  --target main
```

### Step 5 — Verify CI/CD Pipeline

The GitHub Release (`published` event) triggers `.github/workflows/release.yml` which runs 5 parallel jobs:

| Job | What It Does | Artifacts |
|-----|-------------|-----------|
| `build-and-test` | Build + run unit tests | `test-results.trx` |
| `publish-nuget` | Pack + push to GitHub Packages + nuget.org | `.nupkg` attached to release |
| `deploy-docker` | Build Docker image → GHCR + trigger Azure webhook | Docker image tags: `latest`, `{version}`, `{sha}` |
| `deploy-docs` | Build MkDocs → deploy to `gh-pages` branch | GitHub Pages site |
| `build-demos` | Publish server-side demo → attach `.tar.gz` to release | Demo artifact |

Additionally, pushes to `main` trigger:
- `docs.yml` — MkDocs build + GitHub Pages deploy (path-filtered: `docs/**`, `mkdocs.yml`)
- `demo.yml` — Build demo sites (path-filtered: `src/**`, `samples/**`)

### Step 6 — Post-Release Sync

After release is complete:

```bash
# Sync local branches
git fetch upstream
git checkout dev
git reset --hard upstream/dev
git push origin dev

git checkout main
git reset --hard upstream/main
git push origin main
```

## CI/CD Architecture

### Workflows by Trigger

| Workflow | Trigger | Branch/Event |
|----------|---------|-------------|
| `build.yml` | push, PR | `main`, `dev`, `v*` |
| `integration-tests.yml` | push, PR | `main`, `dev`, `v*` |
| `release.yml` | release published | tags `v*` |
| `docs.yml` | push, PR | `main`, `v*` (docs paths only) |
| `demo.yml` | push, PR, workflow_run | `main`, `v*` (src/samples paths) |
| `nuget.yml` | workflow_dispatch | manual (emergency publish) |
| `deploy-server-side.yml` | workflow_dispatch | manual (emergency deploy) |

### Version Resolution

- **During development (dev branch):** NBGV computes version from `version.json` + git height → e.g., `0.19.0-preview.42`
- **During release (release.yml):** Version extracted from git tag (`v0.19.0` → `0.19.0`). NBGV is REMOVED from the build to prevent conflicts.
- **Manual publish (nuget.yml):** Version provided as workflow_dispatch input.

### NuGet Publishing

- **GitHub Packages:** Always published (uses `GITHUB_TOKEN`)
- **nuget.org:** Published only if `NUGET_API_KEY` secret is configured
- **Package ID:** `Fritz.BlazorWebFormsComponents`

### Docker / Azure Deployment

- **Registry:** `ghcr.io/fritzandfriends/blazorwebformscomponents/serversidesamples`
- **Tags:** `latest`, `{version}`, `{commit-sha}`
- **Azure:** Triggered via webhook URL in `AZURE_WEBAPP_WEBHOOK_URL` secret (if configured)

### Documentation Deployment

- **Tool:** MkDocs (via Docker: `docs/Dockerfile`)
- **Target:** GitHub Pages (`gh-pages` branch)
- **Deploys on:** push to `main` with docs path changes, OR release published

## Key Files

| File | Purpose |
|------|---------|
| `version.json` | NBGV version configuration |
| `Directory.Build.props` | Shared build properties (NBGV reference) |
| `.github/workflows/release.yml` | Main release pipeline (5 jobs) |
| `.github/workflows/nuget.yml` | Manual NuGet publish (emergency) |
| `.github/workflows/deploy-server-side.yml` | Manual Azure deploy (emergency) |
| `.github/workflows/build.yml` | CI: build + unit tests + analyzer tests |
| `.github/workflows/integration-tests.yml` | CI: Playwright integration tests |
| `.github/workflows/docs.yml` | CI/CD: MkDocs build + GitHub Pages |
| `.github/workflows/demo.yml` | CI: demo site build |
| `nuget.config` | NuGet source configuration |

## Common Issues

### "Version conflicts" during release build
The release workflow strips NBGV from `Directory.Build.props` and uses `-p:Version={tag}` directly. If you see version conflicts, check that the `sed` command in `release.yml` correctly removes the NBGV PackageReference.

### NuGet push fails
Check that `NUGET_API_KEY` is set in the repository secrets. GitHub Packages push uses `GITHUB_TOKEN` (auto-provided).

### Docker image not deploying to Azure
Check that `AZURE_WEBAPP_WEBHOOK_URL` secret is configured. The webhook call is non-fatal — it logs a warning if it fails.

### Docs not deploying
Docs only deploy on push to `main` (not PRs). Check path filters — only changes to `docs/**`, `mkdocs.yml`, or `.github/workflows/docs.yml` trigger the docs workflow.
