# **📁 🚀 DXSharp DevOps Directory Readme ::**
___
<br/>
This directory holds important scripts and configuration files that play 
a pivotal role in maintaining the robustness and efficiency of our CI/CD pipeline. 
This `README.md` serves as a guide to using and understanding these files.

**Warning**:
These are all new and under experimental development, and thus are a bit dirty and dubious.
Proceed with caution. Bring up an issue if you encounter a bug or have a suggestion.

---
## Directory & File Structure
The following is a snapshot of the hierarchy of files within the `.\devops` directory:
___
📁 `.\devops`

└──  📁 `\pipelines`

└──  📁 `\inc`

        └──  📄 global-varz.yml

└──  📁 `\scripts`

        └──  📄 read-pipelines-init.ps1

└──  📄 `DevOps.Pipelines.targets`

└──  📄 `dxs-core-pipeline.yml`

└──  📄 `azure-pipelines.yml`

└──  📄 `pipeline.init`

___

## 🔑 Key Files

### 📄 global-varz.yml
This YAML file holds variables that hold global meanings in the context of pipelines. 
They cover a multitude of configurable variables, spanning from pool names to target 
.NET frameworks. These variables come handy in several stages of our pipeline, paving 
the way for consistency and reusability across different pipeline configurations. Default 
values are set for certain properties like `dotNetTargets` and `winSDKVersion` to ensure 
the pipeline doesn't fail in case the Powershell script to read `pipeline.init` 
encounters an error.

### 📄 azure-pipelines.yml
This is an autonomous, simplified pipeline that manages clean build and test runs. 
It doesn't rely on external imports or inclusions.

### 📄 dxs-core-pipeline.yml
An advanced version of our pipeline configuration. This handles complex processes following 
the multi-stage job model. This will further be discussed in the upcoming sections of this guide.

### 📄 pipeline.init
This INI file serves as the single source of truth for setting or updating global parameters like 
the Windows SDK and .NET versions. It plays an integral role in maintaining harmony in version 
management across the DXSharp solution.

### 📄 read-pipelines-init.ps1
This Powershell script reads the `pipeline.init` file and sets the global variables accordingly.
This includes:
- `targetFrameworkStrings`
- `winSdkString`

The script return value is a semi-colon delimited string in the format of:

        `net7.0-windows10.0.22621.0;net8.0-windows10.0.22621.0`


This corresponds to the MSBuild property declarations in DXSharp projects' MSBuild files:
```xml
<PropertyGroup>
    <DotNetTargets>net7.0-windows10.0.22621.0;net8.0-windows10.0.22621.0</DotNetTargets>
</PropertyGroup>
```
___

## 🌟 About `DXSharp.Tools.Build` Project
The `DXSharp.Tools.Build` project links all files in the `.\devops` directory, making all scripts 
for various pipeline stages available for direct editing in the IDE. Another home may be found for
these later, but for now the philosophy was to keep all the tools development consolidated into the
tools project. This project links with and captures files in the `.\devops` folder by way of importing 
`DevOps.Pipelines.targets` file. 

---

The ease of accessibility and 
visibility this setup has provided has significantly streamlined our development workflows.

## 📜 How to Use DevOps.Pipelines.targets
The `DevOps.Pipelines.targets` file is a MSBuild script file that imports all the scripts and
configuration files in the `.\devops` directory. This file is imported in the `DXSharp.Tools.Build`
project, and can be imported in any other DXSharp project as well. This file is imported in the
`DXSharp.Tools.Build` project as follows:

This file can be imported in a DXSharp .csproj or another MSBuild script file as follows:
```xml
<Import Project="$(SolutionDir)devops\pipelines\DevOps.Pipelines.targets"
    Condition=" '$(DevOpsTargetImported)' != 'true' " />
```

This will link to those files and make them visible for editing, with special metadata attached.
For example, Powershell scripts (`PwshScript` items) are defined as:
```xml
<PwshScript>
    <ExecuteCommand>pwsh -ExecutionPolicy Bypass -File "%(Identity)"</ExecuteCommand>
    <Link>devops\pipelines\%(RecursiveDir)%(Filename)%(Extension)</Link>
    <FullScriptPath>%(Identity)</FullScriptPath>
    <ScriptName>%(Filename)</ScriptName>
    
    <Visible>true</Visible>
</PwshScript>
```

Note the custom metadata for `ExecuteCommand`, which can be used directly in an MSBuild `Exec` task.
This helps keep scripts a bit cleaner and more readable, as long as you familiarize yourself with the
setup and structure.
```xml
<Exec Command="@( PwshScript -> '%(ExecuteCommand)' )" />
```

---
