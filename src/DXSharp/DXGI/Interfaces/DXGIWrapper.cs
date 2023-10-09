﻿#region Using Directives
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI ;


/// <summary>Helper interface for DXGI wrapper objects.</summary>
public interface DXGIWrapper< out T_DXGI >: IUnknownWrapper
								where T_DXGI: IDXGIObject {
	internal T_DXGI? COMObject { get ; }
} ;
