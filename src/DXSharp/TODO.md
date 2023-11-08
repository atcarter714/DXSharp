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

## Complete "Basic Sample" application

- [ ] Full D3D12 pipeline initialization
- [ ] Full D3D12 pipeline shutdown
- [ ] Clear backbuffer and present swapchain
- [ ] Load and compile a simple shader
- [ ] Load and render a simple quad

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

### Missing Types:
 - ...


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