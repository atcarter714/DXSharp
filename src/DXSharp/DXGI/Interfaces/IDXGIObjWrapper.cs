#region Using Directives
using System ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows.COM ;
#endregion

namespace DXSharp.DXGI ;

public interface IDXGIObjWrapper< out T_DXGI >: IUnknownWrapper
												where T_DXGI: IDXGIObject {
	internal T_DXGI? COMObject { get ; }
} ;

//< IDXGIObjWrapper<T_DXGI>, T_DXGI >
// -----------------------------------------------------------------
