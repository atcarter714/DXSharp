# DX# To-Do List:

---

## Shader Handling

- [ ] Decide on suitable location for shader-handling code

- [ ] Implement shader-related functions

- [ ] Implement shader-related data structures
    * D3D12_FUNCTION_DESC
    * D3D12_LIBRARY_DESC
    * D3D12_PARAMETER_DESC
    * D3D12_SHADER_BUFFER_DESC
    * D3D12_SHADER_DESC
    * D3D12_SHADER_INPUT_BIND_DESC
    * D3D12_SHADER_TYPE_DESC
    * D3D12_SHADER_VARIABLE_DESC
    * D3D12_SIGNATURE_PARAMETER_DESC

- [ ] Implement shader-related enumerations
    * D3D12_SHADER_VISIBILITY

- [ ] Implement shader-related interfaces
    * ID3D12FunctionParameterReflection
    * ID3D12FunctionReflection
    * ID3D12LibraryReflection
    * ID3D12ShaderReflection
    * ID3D12ShaderReflectionConstantBuffer
    * ID3D12ShaderReflectionType
    * ID3D12ShaderReflectionVariable

_____________________________________________
---
## Direct3D 12 API Coverage & Headers

We have a few Windows SDK and DirectX-related headers that we need to
create coverage for ...

- Dxcapi.h (DirectX Shader Compiler, aka "DXC" or "DXIL")
- D3D12Shader.h (Direct3D 12 Shader Reflection)
- Mfd3d12.h (APIs for synchronized resource access to MF producers/consumers)
- D3d12video.h (Direct3D 12 Video APIs)
_____________________________________________
---

## Missing Types:
Code gen, proxy interface re-definition and/or wrapper classes of the 
following types are currently missing:

---
  ### **DXGI Interfaces:**
 - IDXGIDecodeSwapChain
 - IDXGIDisplayControl

### **D3D12 Interfaces:**
 - ID3D12DebugCommandList
 - ID3D12DebugCommandList1
 - ID3D12DebugCommandList2
 - ID3D12DebugCommandQueue
 - ID3D12DebugDevice
 - ID3D12DebugDevice1
 - ID3D12DebugDevice2
 - ID3D12DeviceRemovedExtendedData
 - ID3D12DeviceRemovedExtendedData1
 - ID3D12DeviceRemovedExtendedData2
 - ID3D12DeviceRemovedExtendedDataSettings
 - ID3D12InfoQueue
 - ID3D12InfoQueue1
 - ID3D12SDKConfiguration
 - ID3D12ShaderCacheSession
 - ID3D12SharingContract
 - ID3D12StateObject
 - ID3D12StateObjectProperties
 - ID3D12SwapChainAssistant
 - ID3D12Tools
 - ID3D12VersionedRootSignatureDeserializer
 - ID3D12VirtualizationGuestDevice


### **DXCore Module:**

- The whole thing ...
_____________________________________________

### Dev Strategy Notes (11/11/2023):
Focus on a **working** API layer with *full* coverage, first and foremost ...
it's "allowed" to be a little dirty and imperfect! It's OK to try out some weird ideas
just to see if it "might" work, as long as there's a strong case for doing or exploring it ...

Things are already looking pretty good, stuff works and we've crushed all the technical blockers
that could stop someone from doing this, but there's still a *lot* of work to do. I love the sort
of "style" we are developing here from the perspective of <i>using</i> the API. But what we're doing
here is writing some ugly stuff that that would make Unity devs feint.

The basic workflow to take on is: Find something that doesn't exist, and make it exist. Then, make the
stuff it needs exist, and what they need, until you run out of things that don't exist. Then, you're done.
If you can understand and write this sort of code, then just look at the documentation and look at the existing
code, and it's pretty simple to implement an interface, fix up documentation, etc ...

___
<br/>

### Recent Progress:
- #### Made major improvements to "app framework" code ...
  - `RenderForm` no longer flickers or shows artifacts when resizing or moving around, and seems to change monitors gracefully enough ...
  - Rewrote bits of `RenderForm` to enable high-DPI support ...
  - Created a debug layer management system ...

- #### Implemented a way to load native modules like 'dxcompiler.dll' and `d3d12core.dll` ...
  - `DxcCompiler3` and other Dxc types/interfaces can now be created and used!
  - The Agility SDK can be loaded in place of the "built-in" native D3D12 libraries.

- #### Created new *"Advanced Sample"* application (v0.0.1) ...

- #### Created *"DXSharp.Framework"* project
  - Can be used as a base for future projects and applications ...
  - Being used for development of a powerful application system and 3D programming API ...
  - Intended for use cases such as game engines, 3D editors, and other complex applications ...

- #### Created *"DXSharp.Dxc"* project
  - Exposes the DirectX Shader Compiler (DXC) API & tooling ...
  - Hooked up CsWin32, Win32Metadata, etc for interop layer ...
  - Created the `NativeMethods.txt` and `NativeMethods.json` files ...
  - Defined list of all Dxc functions, types, and enumerations and generated code ...

- #### Completed *"Basic Sample"* application (v0.1.0)
  - Demonstrates basic usage of the DXSharp API ...
  - Successfully initializes/shuts down the Direct3D 12 API/pipeline ...
  - Successfully compiles shaders at runtime and renders a triangle ...
  - Closely matches the "Hello Triangle" sample from Microsoft ...
