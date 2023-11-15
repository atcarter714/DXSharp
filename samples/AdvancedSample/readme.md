# DXSharp "Advanced Sample"

---

## Introduction:
___


### Important Notes:
In order to run this sample, it requires the following:

- Windows 10.1 or later
- DirectX 12 compatible GPU
- Windows 10 SDK (10.0.22621.0+) or later

  - **Note:** You can lower the version number in the 
  `AdvancedSample.csproj` file to match your Windows SDK version.
  - If you change the version number, you're responsible for the compatibility implications!
  - Be careful what Direct3D, DXGI or other features you use and the versions that support it.
  - Many interfaces and API functions are marked with `SupportedOSPlatform` attributes as guidance.
  - These attributes give compiler warnings about method call sights and version numbers, but can't be relied on.

### Project/Assembly Dependencies:
- #### **DXSharp**.dll
  - Native DLLs:
    - *D3D12Core.dll*
    - *d3d12SDKLayers.dll*

---

- #### **DXSharp.Dxc**.dll
    - Native DLLs:
      - *dxcompiler.dll*
      - *dxil.dll*

---

The sample includes a `PackageReference` for the `Microsoft.Direct3D.D3D12` package (aka, the DirectX "Agility SDK") and
for the `Microsoft.Direct3D.DXC` package (aka, the DirectX "Shader Compiler").

The new MSBuild script should now automatically copy the native NuGet dependencies for Agility SDK into the output directory. 
So, the previous instructions about manually copying the Agility SDK binaries side-along with the assembly
should no longer be necessary. If you get errors about missing native DLLs, please make an issue and report it. 
But manually copying the native Agility SDK *.dll files should fix it. You can also modify the Props and Targets
to do custom setups.

  * **NOTE:** DXSharp looks for `d3d12core.dll` and 
  `d3d12sdklayers.dll` in the same directory as the DXSharp assembly. Your project MSBuild script can grab all of these
  things and put them wherever you want them to live in your solution tree. 
  * You can set `$(UsingAgilitySDK)` to `true` or `false` in your project file to enable/disable the Agility SDK 
  * If you disable Agility SDK, the default Windows SDK libraries are loaded.
  * Native Dxc (DirectX Shader Compiler) binaries still need to accompany the `DXSharp.Dxc` assembly.

---

## Sample Overview:

The purpose of this sample is to provide a more "advanced" playground for DXSharp development, to act as a catalyst for
progress and provide a more "realistic" example and way to test and profile the DXSharp runtime. This project is 
recommended for those who are more familiar with the DirectX SDK and are looking for a more "advanced" example of how to
use DXSharp to build things.


____________________________________________________________________________________________________________________

