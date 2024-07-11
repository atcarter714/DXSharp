#region Using Directives
using System.Runtime.InteropServices ;
using DXSharp.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace Windows.Win32.Graphics.Direct3D12 ;


/// <summary>
/// An unmanaged, blittable version of the <see cref="ID3D12Resource"/> interface.
/// </summary>
/// <remarks>
/// This is a special "helper" type generated from CsWin32 to implement D3D12 resource barriers
/// (<see cref="D3D12_RESOURCE_BARRIER"/>) which hold a pointer to an <see cref="ID3D12Resource"/>.
/// It may be possible for us to do away with this and just use the underlying pointer to the COM
/// interface, and handle conversion/marshaling ourselves.
/// </remarks>
public unsafe partial struct ID3D12Resource_unmanaged: IComIID {
	
	public ID3D12Resource_unmanaged( nint pInterface ) {
		this.lpVtbl = (void**)pInterface ;
	}

	public static ComPtr< ID3D12Resource > GetComPtr( in ID3D12Resource_unmanaged pResource ) =>
		new( (nint)pResource.lpVtbl ) ;

	
	public static implicit operator ResourceUnmanaged( in ID3D12Resource_unmanaged pResource ) => 
		new( pResource.lpVtbl ) ;
	
	public static implicit operator ID3D12Resource_unmanaged( in ResourceUnmanaged pResource ) {
		fixed( ResourceUnmanaged* pResourcePtr = &pResource ) {
			return new ID3D12Resource_unmanaged( (nint)pResourcePtr ) ;
		}
	}
	
} ;
