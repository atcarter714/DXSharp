#region Using Directives

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
public interface IDescriptorHeap: IPageable,
								  IComObjectRef< ID3D12DescriptorHeap >, 
								  IUnknownWrapper< ID3D12DescriptorHeap > {
	// ---------------------------------------------------------------------------------
	new ComPtr< ID3D12DescriptorHeap >? ComPointer { get ; }
	new ID3D12DescriptorHeap? COMObject => ComPointer?.Interface ;
	ID3D12DescriptorHeap? IComObjectRef< ID3D12DescriptorHeap >.COMObject => COMObject ;
	ComPtr< ID3D12DescriptorHeap >? IUnknownWrapper< ID3D12DescriptorHeap >.ComPointer => ComPointer ;
	// ==================================================================================
	
	
	/// <summary>Gets the descriptor heap description.</summary>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_descriptor_heap_desc">DescriptorHeapDescription</a></b> The description of the descriptor heap, as a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_descriptor_heap_desc">DescriptorHeapDescription</a> structure.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12descriptorheap-getdesc">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	DescriptorHeapDescription GetDesc( ) {
		unsafe {
			DescriptorHeapDescription description = default ;
			var pDescHeap = ComPointer 
							?? throw new NullReferenceException( ) ;
			
			var fnPtr = pDescHeap.GetVTableMethod< ID3D12DescriptorHeap >( 8 ) ;
			var getDescription = (delegate* unmanaged[ Stdcall, MemberFunction ]< ID3D12DescriptorHeap*, DescriptorHeapDescription >)( fnPtr ) ;
			var heap = (ID3D12DescriptorHeap *)pDescHeap.InterfaceVPtr ;
			
			description = getDescription( heap ) ;
			return description ;
		}
	}
	
	
	/// <summary>Gets the CPU descriptor handle that represents the start of the heap.</summary>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cpu_descriptor_handle">D3D12_CPU_DESCRIPTOR_HANDLE</a></b> Returns the CPU descriptor handle that represents the start of the heap.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12descriptorheap-getcpudescriptorhandleforheapstart">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	CPUDescriptorHandle GetCPUDescriptorHandleForHeapStart( ) {
		unsafe {
			D3D12_CPU_DESCRIPTOR_HANDLE handle = default ;
			var pDescHeap = ComPointer 
							?? throw new NullReferenceException( ) ;

			var fnPtr = pDescHeap.GetVTableMethod< ID3D12DescriptorHeap >( 9 ) ;
			var getDescriptor = (delegate* unmanaged[ Stdcall, MemberFunction ]< ID3D12DescriptorHeap*, D3D12_CPU_DESCRIPTOR_HANDLE >)( fnPtr ) ;

			var heap = (ID3D12DescriptorHeap*)ComPointer.InterfaceVPtr ;
			handle = getDescriptor( heap ) ;
			return handle ;
		}
	}




	/// <summary>Gets the GPU descriptor handle that represents the start of the heap.</summary>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_gpu_descriptor_handle">D3D12_GPU_DESCRIPTOR_HANDLE</a></b> Returns the GPU descriptor handle that represents the start of the heap. If the descriptor heap is not shader-visible, a null handle is returned.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12descriptorheap-getgpudescriptorhandleforheapstart">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	GPUDescriptorHandle GetGPUDescriptorHandleForHeapStart( ) {
		unsafe {
			D3D12_GPU_DESCRIPTOR_HANDLE handle = default ;
			var pDescHeap = ComPointer ?? throw new NullReferenceException( ) ;

			var fnPtr = pDescHeap.GetVTableMethod<ID3D12DescriptorHeap>( 10 ) ;
			var getDescriptor = (delegate* unmanaged[ Stdcall, MemberFunction ]< ID3D12DescriptorHeap*, D3D12_GPU_DESCRIPTOR_HANDLE >)( fnPtr ) ;
			
			var heap = (ID3D12DescriptorHeap *)ComPointer.InterfaceVPtr ;
			handle = getDescriptor( heap ) ;

			return handle ;
		}
	}
	

	// ---------------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12DescriptorHeap) ;
	public new static Guid InterfaceGUID => typeof(ID3D12DescriptorHeap).GUID ;
	static Type IUnknownWrapper.ComType => typeof(ID3D12DescriptorHeap) ;
	static Guid IUnknownWrapper.InterfaceGUID => typeof(ID3D12DescriptorHeap).GUID ;
	
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
	
	// ==================================================================================
} ;



//! Work-around for AccessViolationException issues:
// https://github.com/microsoft/CsWin32/issues/167
/* 
 private unsafe D3D12_CPU_DESCRIPTOR_HANDLE GetDescriptorHandle( ID3D12DescriptorHeap* heap ) {
	var vtable = *(nint**)heap;
	var getDescriptor = (delegate* unmanaged[Stdcall, MemberFunction]<ID3D12DescriptorHeap*, D3D12_CPU_DESCRIPTOR_HANDLE>)(vtable[9]); // index in vtable needs to be found by inspecting code generated by cswin32
	var getDescriptorResult = getDescriptor(heap);
	return getDescriptorResult;
}
*/


/*var pHeap = (ID3D12DescriptorHeap*)Unsafe.AsPointer( ref descHeap ) ;
		void** vTbl = (void**)pHeap ;
		var fnPtr = (delegate* unmanaged[Stdcall, MemberFunction]< ID3D12DescriptorHeap*, D3D12_CPU_DESCRIPTOR_HANDLE >)(vTbl[ 9 ]);

		handle = fnPtr( pHeap ) ;*/
