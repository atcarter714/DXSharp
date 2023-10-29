using Windows.Win32.Graphics.Direct3D12 ;

namespace DXSharp.Direct3D12 ;


/// <summary>Describes the level of GPU-based validation to perform at runtime.</summary>
/// <remarks>
/// This enumeration is used with the <see cref="IDebug2.SetGPUBasedValidationFlags"/>
/// method to configure the amount of runtime validation that will occur.
/// </remarks>
[Flags, EquivalentOf(typeof(D3D12_GPU_BASED_VALIDATION_FLAGS))]
public enum GPUBasedValidationFlags {
	/// <summary>Default behavior; resource states, descriptors, and descriptor tables are all validated.</summary>
	None = 0x00000000,
	/// <summary>When set, GPU-based validation does not perform resource state validation which greatly reduces the performance cost of GPU-based validation. Descriptors and descriptor heaps are still validated.</summary>
	DisableStateTracking = 0x00000001,
} ;