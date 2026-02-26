# Deployment Pipeline Fix

- **Date:** 2026-02-25
- **Requested by:** Jeffrey T. Fritz

## Who worked

- Forge (Lead)

## What was done

Fixed three deployment pipeline issues discovered after v0.14 release:

1. **Azure App Service not pulling new Docker image** — added webhook step to `deploy-server-side.yml`
2. **NuGet not publishing to nuget.org** — added nuget.org push step to `nuget.yml`
3. **Docker version not computed correctly** — compute version with nbgv before Docker build, pass as build-arg

## Outcomes

- PR #348 created on FritzAndFriends/BlazorWebFormsComponents
- Decision recorded: deployment pipeline patterns for Docker versioning, Azure webhook, and NuGet publishing
