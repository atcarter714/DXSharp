#region Using Directives
#pragma warning disable CS8981, CS1591
using System ;
using System.Runtime.InteropServices ;
using System.Runtime.CompilerServices ;

using Windows.Win32 ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.Graphics.Direct3D12 ;
#endregion
namespace DXSharp ;


/// <summary>
/// Enumeration representing the major GPU vendors
/// </summary>
public enum GPUVendor: uint {
	Unknown     = 0x0000U,
	AMD         = 0x1002U,
	Intel       = 0x8086U,
	Nvidia      = 0x10DEU,
} ;