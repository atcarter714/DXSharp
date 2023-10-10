#region Using Directives
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows ;
using DXSharp.Windows.Win32 ;
#endregion
namespace DXSharp.DXGI ;


public interface IResource: IDeviceSubObject,
							DXGIWrapper< IDXGIResource > {
	void GetEvictionPriority( [Out] out uint pEvictionPriority ) ;
	void SetEvictionPriority( uint EvictionPriority ) ;
	
	void GetUsage( [Out] out Usage pUsage ) ;
	void GetSharedHandle( [Out] out Win32Handle pSharedHandle ) ;
};