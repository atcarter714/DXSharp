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

_____________________________________________

### Dev Strategy Notes:

The primary objective: get API coverage! We want to make sure we have all the 
types, functions, and enumerations that are available in the C++ API. We can worry 
about the nit-picking implementation details and perfection later! For now, the goal
is to provide coverage of the API so that we can start building applications with it.
These applications will help us to identify the areas that need the most attention.
They allow us to run the debugger and debug layer, see what is happening and then make
things work as they should.

The typical workflow for this is to find some COM interface or data structure we do not
have yet, make CsWin32 generate a proxy for it and then implement our own version of it
that matches that shape and specs CsWin32 generated from Windows SDK metadata. This usually
gives us structures and types that work out of the box, which we can then customize to our
needs. However, some things are inherently broken, like some COM interface methods that return
a value structure and require work-arounds (like unmanaged function pointers) to get working.
Once you start implementing a proxy interface, you will encounter dependent types that you
will also need to implement, and so on ...

___
<br/>

### Recent Progress:
- #### Completed *"Basic Sample"* application (v0.1.0)
  - Demonstrates basic usage of the DXSharp API ...
  - Successfully initializes/shuts down the Direct3D 12 API/pipeline ...
  - Successfully compiles shaders at runtime and renders a triangle ...
  - Closely matches the "Hello Triangle" sample from Microsoft ...

- #### Created *"DXSharp.Dxc"* project 
  - Exposes the DirectX Shader Compiler (DXC) API & tooling ...
  - Hooked up CsWin32, Win32Metadata, etc for interop layer ...
  - Created the `NativeMethods.txt` and `NativeMethods.json` files ...
  - Defined list of all Dxc functions, types, and enumerations and generated code ...

- #### Created *"DXSharp.Framework"* project 
  - Can be used as a base for future projects and applications ...
  - Being used for development of a powerful application system and 3D programming API ...
  - Intended for use cases such as game engines, 3D editors, and other complex applications ...

- #### Created new *"Advanced Sample"* application (v0.0.1) ...
