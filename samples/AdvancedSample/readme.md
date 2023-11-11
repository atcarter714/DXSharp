# DXSharp "Advanced Sample"

---

## Introduction:
___


### Important Notes:
In order to run this sample, it requires the following:

- Windows 10 or later
- DirectX 12 compatible GPU
- Windows 10 SDK (10.0.19041.0) or later

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

If you use a custom package cache location, you will need to manually locate these NuGet packages and copy the 
native DLLs to the output directory. The native DLLs are located in the `\build\native\bin\` folder of the package root.
You may either copy them to the output directory, or add the folder to your `PATH` environment variable or DLL search 
path. You can also include them as a link in your project and set the `Copy to Output Directory` property to 
`Copy if newer`. In any case, you simply need to ensure that the native DLLs are available to the application at 
startup/runtime for them to be auto-loaded by the DXSharp runtime. If you do not do this, you will have to manually load
the native DLLs yourself into the process memory.

---

## Sample Overview:

The purpose of this sample is to provide a more "advanced" playground for DXSharp development, to act as a catalyst for
progress and provide a more "realistic" example and way to test and profile the DXSharp runtime. This project is 
recommended for those who are more familiar with the DirectX SDK and are looking for a more "advanced" example of how to
use DXSharp to build things.


____________________________________________________________________________________________________________________

