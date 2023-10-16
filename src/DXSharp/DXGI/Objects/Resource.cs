#region Using Directives
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;
using DXSharp.Windows.Win32 ;
#endregion
namespace DXSharp.DXGI ;


public class Resource: DeviceSubObject, IResource {
	public static Type ComType => typeof( IDXGIResource ) ;
	public static Guid InterfaceGUID => typeof( IDXGIResource ).GUID ;
	
	public new ComPtr< IDXGIResource >? ComPointer { get ; protected set ; }
	public new IDXGIResource? COMObject => ComPointer?.Interface ;
	IDXGIResource _dxgiInterface => COMObject ??
		 throw ( ComPointer is not null && ComPointer.Disposed
					 ? new ObjectDisposedException( nameof(OutputDuplication) )
					 : new NullReferenceException( $"{nameof(Resource)} :: " +
									$"internal {nameof(IDXGIResource)} null reference." ) ) ;
	
	
	internal Resource( ) { }
	internal Resource( nint ptr ): base( ptr ) => 
		ComPointer = new( ptr ) ;
	internal Resource( IDXGIResource dxgiObj ): base( dxgiObj ) => 
		ComPointer = new( dxgiObj ) ;
	internal Resource( ComPtr< IDXGIResource > otherPtr ) => 
		ComPointer = otherPtr ;
	

	public void GetEvictionPriority( out uint pEvictionPriority ) => 
		_dxgiInterface.GetEvictionPriority( out pEvictionPriority ) ;

	public void SetEvictionPriority( uint EvictionPriority ) =>
		_dxgiInterface.SetEvictionPriority( EvictionPriority ) ;

	public void GetUsage( out Usage pUsage ) {
		unsafe {
			pUsage = default ;
			DXGI_USAGE usage = default ;
			_dxgiInterface.GetUsage( &usage ) ;
			pUsage = (Usage) usage ;
		}
	}

	public void GetSharedHandle( out Win32Handle pSharedHandle ) {
		pSharedHandle = default ;
		unsafe {
			HANDLE handle = default ;
			_dxgiInterface.GetSharedHandle( &handle ) ;
			pSharedHandle = new( handle ) ;
		}
	}

} ;