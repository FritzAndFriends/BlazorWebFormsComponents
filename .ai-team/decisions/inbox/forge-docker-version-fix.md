### 2026-02-25: Strip NBGV from Docker build to fix version stamping

**By:** Forge

**What:** Added `sed` command in `samples/AfterBlazorServerSide/Dockerfile` (after `COPY . .`, before `dotnet build`) to remove the `Nerdbank.GitVersioning` PackageReference from `Directory.Build.props` inside the Docker build context. This allows the SDK's default assembly attribute generation to use the externally-passed `-p:Version=$VERSION -p:InformationalVersion=$VERSION` properties without NBGV interference.

**Why:** NBGV's MSBuild targets unconditionally override version properties during build execution via task `<Output>` elements, which bypass MSBuild global property (`-p:`) precedence. Inside Docker (where `.git` is excluded by `.dockerignore`), NBGV falls back to `version.json` and stamps assemblies with a stale/inaccurate version instead of the precise version computed by `nbgv get-version` in the workflow. No combination of `-p:` properties can reliably prevent this because NBGV: (1) suppresses SDK attribute generation, (2) overwrites `AssemblyInformationalVersion` during execution, and (3) generates its own attribute source file. The only reliable fix is to remove NBGV entirely from the Docker build, which is safe because all projects only depend on NBGV transitively through `Directory.Build.props` — none reference NBGV properties directly.
