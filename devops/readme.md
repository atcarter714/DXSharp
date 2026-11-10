# DXSharp on Azure Pipelines

Use [azure-pipelines.yml](azure-pipelines.yml) as the Azure DevOps Services pipeline definition. The earlier YAML and templates in this directory record the original CI experiments.

## Create and run the pipeline

1. Push the branch containing this YAML to the Azure Repos repository.
2. Open **Pipelines → New pipeline → Azure Repos Git** and select the repository. Choose **Existing Azure Pipelines YAML file**, then select the branch and `/devops/azure-pipelines.yml`.
3. Save and queue a run on that branch. The project needs Microsoft-hosted parallel-job capacity. The pipeline uses the `windows-2025` and `ubuntu-24.04` images.
4. After the first successful run, use the target branch's **Build validation** policy to require the pipeline on pull requests. The YAML CI trigger handles direct pushes. Set the pipeline's default branch to `main` once this definition lands there.

The Windows matrix calls [build.ps1](../build.ps1) in Debug and Release. Release also runs Windows NativeAOT and the [WARP sample checks](../build/VerifySamples.ps1). A Linux job exercises native COM in JIT and NativeAOT. The same qualification scripts run in GitHub Actions.

## Evidence and retention

| Artifact | Contents |
| --- | --- |
| `windows-Debug-packages`, `windows-Release-packages` | NuGet and symbol packages from successful jobs |
| `windows-Debug-evidence`, `windows-Release-evidence` | Commit and SDK metadata, build binlog, TRX files, and available sample logs and diagnostics |
| `windows-x64-native-interop`, `linux-x64-native-interop` | Standalone NativeAOT qualification binaries and native fixtures |
| `windows-x64-native-compute` | NativeAOT compute and DirectML sample with its runtime dependencies |

Use **Project settings → Pipelines → Settings** to set routine run, artifact, PR, and test retention. On a milestone run, use **Retain indefinitely** to pin the run and its artifacts. For a release, download or promote the validated packages to your release storage or NuGet feed so distribution has its own lifecycle. Feed promotion is a separate release step. [Microsoft retention documentation](https://learn.microsoft.com/en-us/azure/devops/pipelines/policies/retention)

## Access

Share the organization, project, and repository URLs, the branch to build, and the pipeline URL if a definition already exists. To let an operator configure it, give their authenticated identity project access, repository **Read**, and pipeline **View**, **Queue builds**, and **Create/Edit build pipeline** as needed. Grant repository **Contribute** only if that identity will push changes. An administrator can handle the branch policy and project retention separately.

An authenticated browser session or Azure CLI session in the working environment is sufficient. Use your Microsoft Entra sign-in for interactive CLI access. Keep credentials in that session or a secret store and share the resource URLs in chat. [Azure Repos pipeline setup](https://learn.microsoft.com/en-us/azure/devops/pipelines/repos/azure-repos-git) · [Azure Pipelines permissions](https://learn.microsoft.com/en-us/vsts/security/set-build-release-permissions)
