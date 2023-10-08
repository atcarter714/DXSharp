using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows ;
using DXSharp.Windows.Win32 ;

namespace DXSharp.DXGI ;

public interface IResource: IDeviceSubObject,
							IDXGIObjWrapper< IDXGIResource > {
	// IDXGIResource methods
	void GetEvictionPriority( [Out] out uint pEvictionPriority ) ;
	void SetEvictionPriority( uint EvictionPriority ) ;
	
	HResult GetUsage( [Out] out Usage pUsage ) ;
	HResult GetSharedHandle( [Out] out Win32Handle pSharedHandle ) ;
}