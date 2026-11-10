# Publishing DXSharp previews

The first public version is `0.4.0-net10-preview.1`. Publish `DXSharp.Core`, `DXSharp.DirectML`, and `DXSharp.Framework.Desktop` for Windows x64. The legacy projects continue to build and produce CI artifacts for migration. The public preview depends only on the modern packages and their pinned upstream native dependencies.

## One-time configuration

1. In GitHub, create the `nuget` environment and restrict deployments to `main`. Enable a required reviewer if supported by the repository plan. Set the environment secret `NUGET_USER` to the nuget.org profile name of the publishing account.
2. On nuget.org, create a [Trusted Publishing](https://learn.microsoft.com/nuget/nuget-org/trusted-publishing) policy owned by the account or organization that will own the packages. Set repository owner `atcarter714`, repository `DXSharp`, workflow filename `release.yml`, and environment `nuget`. Enable creation and updates for the three package IDs above. The first publish requires creation scope. Disable it once all three IDs have owners and versions on the feed.
3. In Azure DevOps, keep `devops/azure-pipelines.yml` configured as the build definition and use build validation on the integration branch. The Azure Release build retains an independently qualified candidate artifact. GitHub Actions holds the public publishing identity. [Azure build and retention](../devops/readme.md)

## Release

1. Land the changes on `main` with the intended `PackageVersion` in `Directory.Build.props`. Review the [qualification checklist](SDK-Release-Qualification.md) and attach hardware logs to the release review.
2. Run **DXSharp preview release** from `main` with the exact version and **publish** unchecked to inspect the artifacts. Use the same commit for the publishing run.
3. Run the workflow with **publish** checked. The workflow builds the candidate on its triggering commit, qualifies WARP and NativeAOT, and stores the three packages, symbols, and a SHA-256 manifest. The publish job revalidates that run's artifact and checks the tag and package versions are available before requesting a temporary NuGet credential. It pushes Core first, then the two dependents. A successful push creates a GitHub prerelease at the qualified commit and attaches the candidate.
4. Verify each package and version in nuget.org and verify the release tag and manifest against the qualifying run. Use a clean consumer with NuGet.org as the only package source to confirm public restore.

A NuGet package version is immutable. If publication fails partway through, keep the failed run's candidate and logs. Check package and symbol status with the owner, confirm the published package contents against that candidate, and upload only missing files from the same artifact. If content differs, choose a new version and requalify. If the final GitHub step alone failed, create the release at the manifest's commit and attach that same candidate. The workflow rejects pre-existing versions and tags to surface these cases for review.

## Azure DevOps access

For a human operator, grant project-scoped access to **Arkaen Engine SDK** and only the repository and pipeline permissions needed for the task. Read access is sufficient for reviewing repositories, runs, and logs. Grant the ability to queue builds or edit pipelines separately as needed. For automation inside Azure Pipelines, use the project's build service identity and scope job authorization to the current project. Keep NuGet publishing tied to the GitHub workflow's Trusted Publishing policy. [Azure pipeline job access tokens](https://learn.microsoft.com/azure/devops/pipelines/process/access-tokens)
