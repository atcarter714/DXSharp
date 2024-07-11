#region Using Directives

using System.Collections.ObjectModel ;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


/// <summary>
/// A descriptor heap is a collection of contiguous allocations of descriptors,
/// one allocation for every descriptor. Descriptor heaps contain many object
/// types that are not part of a Pipeline State Object (PSO), such as Shader
/// Resource Views (SRVs), Unordered Access Views (UAVs), Constant Buffer
/// Views (CBVs), and Samplers.
/// </summary>
/// <remarks>
/// See documentation to learn more about 
/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/d3d12/nn-d3d12-id3d12descriptorheap">
/// ID3D12DescriptorHeap
/// </a>.
/// </remarks>
[ProxyFor(typeof(ID3D12DescriptorHeap))]
public interface IDescriptorHeap: IPageable, IInstantiable {
	// ---------------------------------------------------------------------------------
	//! Creation Functions:
	// ---------------------------------------------------------------------------------
	internal static readonly ReadOnlyDictionary< Guid, Func<ID3D12DescriptorHeap, IInstantiable> > _descHeapCreationFunctions =
		new( new Dictionary<Guid, Func<ID3D12DescriptorHeap, IInstantiable> > {
			{ IDescriptorHeap.IID, ( pComObj ) => new DescriptorHeap( pComObj ) },
		} ) ;

	// ---------------------------------------------------------------------------------

	
	/// <summary>Gets the descriptor heap description.</summary>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_descriptor_heap_desc">DescriptorHeapDescription</a></b> The description of the descriptor heap, as a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_descriptor_heap_desc">DescriptorHeapDescription</a> structure.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12descriptorheap-getdesc">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	DescriptorHeapDescription GetDesc( ) ;
	
	/// <summary>Gets the CPU descriptor handle that represents the start of the heap.</summary>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">D3D12_CPU_DESCRIPTOR_HANDLE</a></b> Returns the CPU descriptor handle that represents the start of the heap.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12descriptorheap-getcpudescriptorhandleforheapstart">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	CPUDescriptorHandle GetCPUDescriptorHandleForHeapStart( ) ;

	/// <summary>Gets the GPU descriptor handle that represents the start of the heap.</summary>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_gpu_descriptor_handle">D3D12_GPU_DESCRIPTOR_HANDLE</a></b> Returns the GPU descriptor handle that represents the start of the heap. If the descriptor heap is not shader-visible, a null handle is returned.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12descriptorheap-getgpudescriptorhandleforheapstart">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	GPUDescriptorHandle GetGPUDescriptorHandleForHeapStart( ) ;
	

	// ---------------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12DescriptorHeap) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12DescriptorHeap).GUID
														  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}

	static IInstantiable IInstantiable.Instantiate( ) => new DescriptorHeap( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new DescriptorHeap( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< Com >( Com obj ) => new DescriptorHeap( ( obj as ID3D12DescriptorHeap )! ) ;
	// ==================================================================================
} ;
