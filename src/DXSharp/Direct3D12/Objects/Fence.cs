using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

namespace DXSharp.Direct3D12 ;


[Wrapper(typeof(ID3D12Fence))]
public class Fence: Pageable, 
					IFence,
					IInstantiable< Fence > {
	// -----------------------------------------------------------------------------------------------
	public new ID3D12Fence? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12Fence >? ComPointer { get ; protected set ; }
	// -----------------------------------------------------------------------------------------------
	
	internal Fence( ) { }
	internal Fence( ComPtr< ID3D12Fence > comPtr ) => ComPointer = comPtr ;
	internal Fence( nint address ) => ComPointer = new( address ) ;
	internal Fence( ID3D12Fence comObject ) => ComPointer = new( comObject ) ;
	
	// -----------------------------------------------------------------------------------------------
	static IInstantiable IInstantiable.   Instantiate( )                => new Fence( ) ;
	static IInstantiable IInstantiable.  Instantiate( IntPtr pComObj ) => new Fence( pComObj ) ;
	static Fence? IInstantiable< Fence >.Instantiate( IntPtr ptr )     => new( ptr ) ;
	public static IInstantiable Instantiate< ICom >( ICom pComObj ) 
		where ICom: IUnknown? => new Fence( (ID3D12Fence) pComObj! ) ;
	// ===============================================================================================
} ;


[Wrapper( typeof( ID3D12Fence1 ) )]
public class Fence1: Fence, IFence1 {
	// -----------------------------------------------------------------------------------------------
	public new ID3D12Fence1? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12Fence1 >? ComPointer { get ; protected set ; }

	// -----------------------------------------------------------------------------------------------

	internal Fence1( ) { }
	internal Fence1( ComPtr< ID3D12Fence1 > comPtr ) => ComPointer = comPtr ;
	internal Fence1( nint address ) => ComPointer = new( address ) ;
	internal Fence1( ID3D12Fence1 comObject ) => ComPointer = new( comObject ) ;

	// -----------------------------------------------------------------------------------------------
	
	public FenceFlags GetCreationFlags( ) {
		var fence = COMObject ?? throw new NullReferenceException( ) ;
		return (FenceFlags)fence.GetCreationFlags( ) ;
	}
	
	// ===============================================================================================

} ;