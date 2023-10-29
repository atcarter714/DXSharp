using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

namespace DXSharp.Direct3D12 ;

public class Heap: Pageable, 
				   IHeap,
				   IInstantiable< Heap > {
	// ------------------------------------------------------------------------------------------
	public new ID3D12Heap? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12Heap >? ComPointer { get ; protected set ; }
	// ------------------------------------------------------------------------------------------
	
	internal Heap( ) { }
	internal Heap( ComPtr< ID3D12Heap > ptr ) => ComPointer = ptr ;
	internal Heap( nint ptr ) => ComPointer = new( ptr ) ;
	internal Heap( ID3D12Heap d3D12Heap ) => ComPointer = new( d3D12Heap ) ;
	
	// ------------------------------------------------------------------------------------------
	static IInstantiable IInstantiable. Instantiate( )                => new Heap( ) ;
	static IInstantiable IInstantiable.Instantiate( IntPtr pComObj ) => new Heap( pComObj ) ;
	
	static Heap? IInstantiable< Heap >.Instantiate( nint ptr ) => new( ptr ) ;
	public static IInstantiable Instantiate< ICom >( ICom pComObj ) where ICom: IUnknown? =>
		new Heap( (ID3D12Heap)pComObj! ) ;
	// ==========================================================================================
} ;