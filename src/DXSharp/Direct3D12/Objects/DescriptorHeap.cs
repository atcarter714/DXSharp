#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


internal class DescriptorHeap: Pageable,
							   IDescriptorHeap,
							   IComObjectRef< ID3D12DescriptorHeap >,
							   IUnknownWrapper< ID3D12DescriptorHeap >  {
	// ---------------------------------------------------------------------------------
	ComPtr< ID3D12DescriptorHeap >? _comPtr ;
	public new ComPtr< ID3D12DescriptorHeap >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer<ID3D12DescriptorHeap>( ) ;
	public override ID3D12DescriptorHeap? ComObject => ComPointer?.Interface ;
	
	internal DescriptorHeap( ) { }
	internal DescriptorHeap( ComPtr< ID3D12DescriptorHeap > ptr ) => _comPtr = ptr ;
	internal DescriptorHeap( nint                           ptr ) => _comPtr = new( ptr ) ;
	internal DescriptorHeap( ID3D12DescriptorHeap           ptr ) => _comPtr = new( ptr ) ;
	
	// ---------------------------------------------------------------------------------
	
	public DescriptorHeapDescription GetDesc( ) {
		unsafe {
			var pDescHeap = ComPointer 
							?? throw new NullReferenceException( ) ;
			
			var fnPtr = pDescHeap.GetVTableMethod< ID3D12DescriptorHeap >( 8 ) ;
			var getDescription = (delegate* unmanaged[ Stdcall, MemberFunction ]< ID3D12DescriptorHeap*, DescriptorHeapDescription >)( fnPtr ) ;
			var heap = (ID3D12DescriptorHeap *)pDescHeap.InterfaceVPtr ;
			
			DescriptorHeapDescription description = getDescription( heap ) ;
			return description ;
		}
	}
	
	public CPUDescriptorHandle GetCPUDescriptorHandleForHeapStart( ) {
		unsafe {
			var pDescHeap = ComPointer 
							?? throw new NullReferenceException( ) ;

			var fnPtr = pDescHeap.GetVTableMethod< ID3D12DescriptorHeap >( 9 ) ;
			var getDescriptor = (delegate* unmanaged[ Stdcall, MemberFunction ]< ID3D12DescriptorHeap*, D3D12_CPU_DESCRIPTOR_HANDLE >)( fnPtr ) ;

			var                         heap   = (ID3D12DescriptorHeap*)ComPointer.InterfaceVPtr ;
			D3D12_CPU_DESCRIPTOR_HANDLE handle = getDescriptor( heap ) ;
			return handle ;
		}
	}
	
	public GPUDescriptorHandle GetGPUDescriptorHandleForHeapStart( ) {
		unsafe {
			var pDescHeap = ComPointer ?? throw new NullReferenceException( ) ;

			var fnPtr = pDescHeap.GetVTableMethod<ID3D12DescriptorHeap>( 10 ) ;
			var getDescriptor = (delegate* unmanaged[ Stdcall, MemberFunction ]< ID3D12DescriptorHeap*, D3D12_GPU_DESCRIPTOR_HANDLE >)( fnPtr ) ;
			
			var                         heap   = (ID3D12DescriptorHeap *)ComPointer.InterfaceVPtr ;
			D3D12_GPU_DESCRIPTOR_HANDLE handle = getDescriptor( heap ) ;

			return handle ;
		}
	}
	
	
	// ---------------------------------------------------------------------------------
	
	public new static Type ComType => typeof(ID3D12DescriptorHeap) ;
	
	public new static ref readonly Guid Guid {
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12DescriptorHeap).GUID
															  .ToByteArray( ) ;
			
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	// ==================================================================================
} ;