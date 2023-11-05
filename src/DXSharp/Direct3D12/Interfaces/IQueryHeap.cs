#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[ProxyFor( typeof(ID3D12QueryHeap) )]
public interface IQueryHeap: IPageable {
	// ---------------------------------------------------------------------------------
	
	new static Type ComType => typeof(ID3D12QueryHeap) ;

	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12QueryHeap).GUID
														   .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}

	// ==================================================================================
} ;


[Wrapper(typeof(ID3D12QueryHeap))]
internal class QueryHeap: Pageable, 
						  IQueryHeap,
						  IComObjectRef< ID3D12QueryHeap >,
						  IUnknownWrapper< ID3D12QueryHeap > {
	ComPtr< ID3D12QueryHeap >? _comPtr ;
	public new virtual ComPtr< ID3D12QueryHeap >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< ID3D12QueryHeap >( ) ;
	public override ID3D12QueryHeap? COMObject => ComPointer?.Interface ;
	
	public new static Type ComType => typeof(ID3D12QueryHeap) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12QueryHeap).GUID
															   .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
} ;