# Publishing DXSharp previews

The first public version is `0.4.0-net10-preview.1`. Publish `DXSharp.Core`, `DXSharp.DirectML`, and `DXSharp.Framework.Desktop` for Windows x64. The legacy projects continue to build and produce CI artifacts for migration. The public preview depends only on the modern packages and their pinned upstream native dependencies.

## One-time configuration

1. In GitHub, create the `nuget` environment and restrict deployments to `main`. Enable a required reviewer if supported by the repository plan. Set the environment secret `NUGET_USER` to the nuget.org profile name of the publishing account.
2. On nuget.org, create a [Trusted Publishing](https://learn.microsoft.com/nuget/nuget-org/trusted-publishing) policy owned by the account or organization that will own the packages. Set repository owner `atcarter714`, repository `DXSharp`, workflow filename `release.yml`, and environment `nuget`. Enable creation and updates for the three package IDs above. The first publish requires creation scope. Disable it once all three IDs have owners and versions on the feed.
3. In Azure DevOps, keep `devops/azure-pipelines.yml` configured as the build definition and use build validation on the integration branch. The Azure Release build retains an independently qualified candidate artifact. GitHub Actions holds the public publishing identity. [Azure build and retention](../devops/readme.md)

## Release

1. Prepare the intended `PackageVersion` in `Directory.Build.props` and its matching `doc/releases/<version>.md` notes. Review the [qualification checklist](SDK-Release-Qualification.md) and attach hardware logs to the release review.
2. A merge to `main` that changes a versioned release note starts **DXSharp preview release** automatically. The workflow qualifies the exact triggering commit with Release builds, package consumption, WARP, and Windows and Linux NativeAOT.
3. After qualification, it publishes a GitHub prerelease at that commit and attaches the three packages, symbols, and a SHA-256 manifest. The separate NuGet job verifies the downloaded candidate, tag commit, and package version availability, then requests a temporary NuGet credential. It pushes Core first and then its dependents. The release notes distinguish the GitHub assets from NuGet publication and record the publishing run after successful upload.
4. Verify each package and version in nuget.org and verify the release tag and manifest against the qualifying run. Use a clean consumer with NuGet.org as the only package source to confirm public restore.

For a manual dry run before merging, dispatch the workflow with the exact committed version and **publish** unchecked. A manual publishing run is restricted to `main`. Keep the release tag bound to the manifest's commit.

If the NuGet identity or Trusted Publishing policy is missing, configure it and rerun only the failed NuGet job in the original run. The successful qualification and GitHub release jobs retain the candidate used by the retry. Starting a new full publishing run for an existing release tag is rejected.

A NuGet package version is immutable. If publication fails partway through, keep the failed run's candidate and logs. Check package and symbol status with the owner, confirm the published package contents against that candidate, and upload only missing files from the same artifact. If content differs, choose a new version and requalify. If only the final release-note update failed after a successful NuGet upload, update the existing release to record that upload. The workflow rejects pre-existing package versions to surface partial publication for review.

## Azure DevOps access

For a human operator, grant project-scoped access to **Arkaen Engine SDK** and only the repository and pipeline permissions needed for the task. Read access is sufficient for reviewing repositories, runs, and logs. Grant the ability to queue builds or edit pipelines separately as needed. For automation inside Azure Pipelines, use the project's build service identity and scope job authorization to the current project. Keep NuGet publishing tied to the GitHub workflow's Trusted Publishing policy. [Azure pipeline job access tokens](https://learn.microsoft.com/azure/devops/pipelines/process/access-tokens)
