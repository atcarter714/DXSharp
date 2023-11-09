# DXSharp Coding and Implementation Standards

This document outlines the standards and practices for contributing to the DXSharp project. DXSharp aims to provide a modern, idiomatic .NET wrapper for DirectX APIs, prioritizing readability, maintainability, and performance. Contributors are expected to adhere to these guidelines.

## Wrapper (Re-)Naming Rules

### Namespaces and Class Names
- Prefixes such as "D3D12_" that are common in DirectX APIs should be omitted in favor of appropriate namespaces.
  - Example: `D3D12_GRAPHICS_PIPELINE_STATE_DESC` becomes `GraphicsPipelineStateDesc` in the `Direct3D12` namespace.

      (**Note:** Decision on "*Desc*" vs "*Description*" is currently pending ... current usage is "Description".)

### Enums and Constants
- Enum members should omit redundant prefixes and suffixes, relying on the enum type to provide context.
- Example: `D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET` becomes `AllowRenderTarget` within an `enum ResourceFlags`.

### Methods and Properties
- Method names should be concise and reflect .NET conventions (`PascalCase` without underscores).
- Boolean properties should be named to reflect a true/false question. (e.g., `IsEnabled`, `HasDepthStencil`).

## Wrapper Code Creation Rules

### Naming Conventions
- Follow the (re-)naming convention rules strictly to ensure consistency across the API.
- Use PascalCase for all public members, aligning with .NET's naming convention.

### Idiomatic .NET
- Translate C-style constructs to idiomatic .NET patterns (e.g., use properties instead of getter/setter functions).
- Replace pointer arithmetic with safe constructs like `Span<T>` or `Memory<T>` where possible.

### Safety
- Encapsulate unsafe operations within safe methods, exposing a .NET-friendly API that abstracts away the complexity of unsafe code.
- Use the `SafeHandle` pattern for managing resource lifetimes and ensure that all unmanaged resources are properly disposed of.

### Flags and Bitfields
- Replace integral types used for bitfields with `[Flags]` enums to provide type safety and clear usage.
- Example: Replace `uint flags` with `ResourceFlags flags`.

### Overloads and Extensions
- Provide overloads to reduce complexity for common use cases.
- Use extension methods to add functionality to existing types without altering the underlying API.

### Documentation and Comments
- All public APIs should be accompanied by XML documentation comments explaining their purpose and usage.
- Comments should be clear and concise, avoiding redundancy with the code itself.

### Error Handling
- Translate HRESULTs to exceptions where appropriate, providing meaningful error messages.
- Avoid throwing exceptions for non-exceptional cases; prefer return values or `out` parameters for expected outcomes.

### Interoperability
- Document any deviations from the native API to clarify differences in the .NET wrapper.
- Where direct mapping to .NET constructs isn't possible, provide detailed comments explaining the interoperability decisions.

## Testing and Validation

### Unit Tests
- Write unit tests for all new functionality to ensure correctness and prevent regressions.
- Mock dependencies where possible to isolate tests to the component being validated.

### Performance
- Optimize for performance, minimizing allocations, and memory usage.
- Use benchmarks to measure performance impacts of changes, especially in hot paths.

### Compliance
- Ensure all code complies with the .NET Core and .NET 7 guidelines for cross-platform compatibility.

## Collaboration and Code Reviews

- Submit changes for review before merging into the main branch.
- Reviewers should ensure that contributions adhere to these standards and are consistent with the existing codebase.

By following these standards, contributors will help maintain the quality and integrity of the DXSharp project, ensuring it remains a robust, efficient, and user-friendly wrapper for the DirectX APIs.
