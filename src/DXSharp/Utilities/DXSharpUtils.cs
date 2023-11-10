#region Using Directives
using System.Runtime.CompilerServices ;
#endregion
namespace DXSharp ;


/// <summary>
/// A static "utility" class for DXSharp with globally
/// useful bits of data and functionality ...
/// </summary>
public static class DXSharpUtils {
	public const short _MAXOPT_ = ( 0x100|0x200 ) ;
	const uint _AMD_ = 0x1002, _Intel_ = 0x8086, _Nvidia_ = 0x10DE ;
	public static readonly uint VendorID_AMD       = _AMD_, 
								   VendorID_Intel  = _Intel_, 
								   VendorID_Nvidia = _Nvidia_ ;
	
	public static GPUVendor GPUVendorFromUID( in this UID32 vendorID ) =>
		( (uint)vendorID ) switch {
			_AMD_    => GPUVendor.AMD,
			_Intel_  => GPUVendor.Intel,
			_Nvidia_ => GPUVendor.Nvidia,
			_        => GPUVendor.Unknown,
		} ;
	
	[MethodImpl(_MAXOPT_)]
	public static int ConvertDipsToPixels( float dips, float dpi ) => 
										(int)(dips * dpi / 96.0f + 0.5f) ;
	[MethodImpl(_MAXOPT_)]
	public static float ConvertPixelsToDips( int pixels, float dpi ) => 
														pixels * 96f / dpi ;
} ;