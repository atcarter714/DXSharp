#region Using Directives
using System.Runtime.InteropServices ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp ;
#endregion
namespace DXSharp.Direct3D12 ;


/// <summary>
/// Provides detail about the adapter architecture, so that your application can better optimize for certain adapter properties.
/// </summary>
/// <remarks>
/// <b>NOTE:</b><para/>
/// This structure has been superseded by the <see cref="FeatureDataArchitecture1"/> structure.
/// If your application targets Windows 10, version 1703 (Creators' Update) or higher,
/// then use D3D12_FEATURE_DATA_ARCHITECTURE1 (and D3D12_FEATURE_ARCHITECTURE1) instead.
/// </remarks>
[StructLayout(LayoutKind.Sequential),
 EquivalentOf(typeof(D3D12_FEATURE_DATA_ARCHITECTURE))]
public struct FeatureDataArchitecture {
	/// <summary>
	/// <para>In multi-adapter operation, this indicates which physical adapter of the device is relevant. See <a href="https://docs.microsoft.com/windows/win32/direct3d12/multi-engine">Multi-adapter systems</a>. <b>NodeIndex</b> is filled out by the application before calling <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-checkfeaturesupport">CheckFeatureSupport</a>, as the application can retrieve details about the architecture of each adapter.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint NodeIndex ;

	/// <summary>
	/// <para>Specifies whether the hardware and driver support a tile-based renderer. The runtime sets this member to <b>TRUE</b> if the hardware and driver support a tile-based renderer.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL TileBasedRenderer ;

	/// <summary>
	/// <para>Specifies whether the hardware and driver support UMA. The runtime sets this member to <b>TRUE</b> if the hardware and driver support UMA.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL UMA ;

	/// <summary>
	/// <para>Specifies whether the hardware and driver support cache-coherent UMA. The runtime sets this member to <b>TRUE</b> if the hardware and driver support cache-coherent UMA.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL CacheCoherentUMA ;
} ;


/// <summary>
/// See documentation for: 
/// <a href="https://docs.microsoft.com/en-us/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture1">D3D12_FEATURE_DATA_ARCHITECTURE1</a>
/// </summary>
/// <remarks>
/// Update to D3D12 since Windows 10 Creators Update (1703).
/// Used by <see cref="ID3D12Device.CheckFeatureSupport"/>.
/// </remarks>
[StructLayout(LayoutKind.Sequential),
 EquivalentOf(typeof(D3D12_FEATURE_DATA_ARCHITECTURE1))]
public struct FeatureDataArchitecture1 {
	/// <summary>
	/// <para>In multi-adapter operation, this indicates which physical adapter of the device is relevant. See <a href="https://docs.microsoft.com/windows/win32/direct3d12/multi-engine">Multi-adapter systems</a>. <b>NodeIndex</b> is filled out by the application before calling <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device-checkfeaturesupport">CheckFeatureSupport</a>, as the application can retrieve details about the architecture of each adapter.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint NodeIndex ;

	/// <summary>
	/// <para>Specifies whether the hardware and driver support a tile-based renderer. The runtime sets this member to <b>TRUE</b> if the hardware and driver support a tile-based renderer.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL TileBasedRenderer ;

	/// <summary>
	/// <para>Specifies whether the hardware and driver support UMA. The runtime sets this member to <b>TRUE</b> if the hardware and driver support UMA.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL UMA ;

	/// <summary>
	/// <para>Specifies whether the hardware and driver support cache-coherent UMA. The runtime sets this member to <b>TRUE</b> if the hardware and driver support cache-coherent UMA.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL CacheCoherentUMA ;

	/// <summary>
	/// <para><a href="https://docs.microsoft.com/visualstudio/code-quality/annotating-structs-and-classes">SAL</a>: <c>_Out_</c> Specifies whether the hardware and driver support isolated Memory Management Unit (MMU). The runtime sets this member to <b>TRUE</b> if the GPU honors CPU page table properties like <b>MEM_WRITE_WATCH</b> (for more information, see <a href="https://docs.microsoft.com/windows/win32/api/memoryapi/nf-memoryapi-virtualalloc">VirtualAlloc</a>) and <b>PAGE_READONLY</b> (for more information, see <a href="https://docs.microsoft.com/windows/win32/Memory/memory-protection-constants">Memory Protection Constants</a>). If <b>TRUE</b>, the application must take care to no use memory with these page table properties with the GPU, as the GPU might trigger these page table properties in unexpected ways. For example, GPU write operations might be coarser than the application expects, particularly writes from within shaders. Certain write-watch pages might appear dirty, even when it isn't obvious how GPU writes may have affected them. GPU operations associated with upload and readback heap usage scenarios work well with write-watch pages, but might occasionally generate false positives that can be safely ignored.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL IsolatedMMU ;
} ;