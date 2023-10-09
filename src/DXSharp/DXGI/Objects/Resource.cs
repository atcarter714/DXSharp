using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;
using DXSharp.Windows.Win32 ;

namespace DXSharp.DXGI ;

public class Resource: DeviceSubObject, IResource {
	public new IDXGIResource? COMObject => ComPointer?.Interface ;
	public new ComPtr< IDXGIResource >? ComPointer { get ; protected set ; }
	
	IDXGIResource _dxgiInterface => COMObject ??
		 throw ( ComPointer is not null && ComPointer.Disposed
					 ? new ObjectDisposedException( nameof(OutputDuplication) )
					 : new NullReferenceException( $"{nameof(Resource)} :: " +
									$"internal {nameof(IDXGIResource)} null reference." ) ) ;
	
	public void GetEvictionPriority( out uint pEvictionPriority ) {
		_throwIfDestroyed( ) ;
		
	}

	public void SetEvictionPriority( uint EvictionPriority ) {
		_throwIfDestroyed( ) ;
	}

	public HResult GetUsage( out Usage pUsage ) {
		_throwIfDestroyed( ) ;
	}

	public HResult GetSharedHandle( out Win32Handle pSharedHandle ) {
		_throwIfDestroyed( ) ;
	}
}