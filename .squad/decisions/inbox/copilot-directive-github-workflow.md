### 2026-04-07T12:26:20Z: GitHub workflow and release process
**By:** Jeffrey T. Fritz (via Copilot)
**What:**
1. Feature/fix PRs go against UPSTREAM (FritzAndFriends/BlazorWebFormsComponents) targeting the `dev` branch using squash merge.
2. After merge, pull upstream/dev to local and push to origin. Delete the feature branch.
3. Feature branches use `feature/{description}` naming convention.
4. Release process: PR from upstream/dev → upstream/main using regular merge (NOT squash). Add a tag, create a GitHub Release. This triggers CI/CD to deploy sample sites, docs, and push NuGet packages.
5. Post-release: sync both dev and main locally from upstream and push to origin.
**Why:** User request — captured for team memory. These are the canonical Git and release workflows for all Squad agents.
