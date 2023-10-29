#region Using Directives
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows.Win32 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI ;


[ProxyFor(typeof(IDXGIResource))]
public interface IResource: IDeviceSubObject,
							IComObjectRef< IDXGIResource >,
							IUnknownWrapper< IDXGIResource >,
							IInstantiable {
	// ---------------------------------------------------------------------------------
	new ComPtr< IDXGIResource >? ComPointer { get ; }
	new IDXGIResource? COMObject => ComPointer?.Interface ;
	IDXGIResource? IComObjectRef< IDXGIResource >.COMObject => COMObject ;

	static IInstantiable IInstantiable. Instantiate( )                => new Resource( ) ;
	static IInstantiable IInstantiable.Instantiate( IntPtr pComObj ) => new Resource( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => 
		new Resource( (IDXGIResource)pComObj! ) ;
	// ==================================================================================
	
	
	void GetEvictionPriority( [Out] out uint pEvictionPriority ) ;
	void SetEvictionPriority( uint EvictionPriority ) ;
	
	void GetUsage( [Out] out Usage pUsage ) ;
	void GetSharedHandle( [Out] out Win32Handle pSharedHandle ) ;
} ;