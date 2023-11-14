#region Using Directives
using System.Diagnostics ;
using System.Runtime.CompilerServices ;

using DXSharp.Direct3D12 ;
#endregion
namespace DXSharp ;


/// <summary>
/// A static "utility" class for DXSharp with globally
/// useful bits of data and functionality ...
/// </summary>
public static class DXSharpUtils {
	#region Constant & ReadOnly Values
	public const short _MAXOPT_ = ( 0x100|0x200 ) ;
	const uint _AMD_ = 0x1002, _Intel_ = 0x8086, _Nvidia_ = 0x10DE ;
	public static readonly uint VendorID_AMD    = _AMD_,
								VendorID_Intel  = _Intel_,
								VendorID_Nvidia = _Nvidia_ ;
	
	public const int ComponentMappingMask  = 0x07,
					 ComponentMappingShift = 3,
					 ComponentMappingAlwaysSetBitAvoidingZeromemMistakes = (1 << ( ComponentMappingShift * 4 )) ;
	/// <summary>The default 1:1 <see cref="ShaderComponentMapping"/> value.</summary>
	public const ShaderComponentMapping DefaultComponentMapping = (ShaderComponentMapping)0x1688 ;
		// = EncodeShader4ComponentMapping( 0, 1, 2, 3 ) ;
		// ! 0x1688 = 5768 = 0b1011010000000
	
	public const int D3D12_CONSTANT_BUFFER_DATA_PLACEMENT_ALIGNMENT = 256 ;
	
	public const uint D3D12_RESOURCE_BARRIER_ALL_SUBRESOURCES = 0xffffffff ;
	
	public const int D3D12_SIMULTANEOUS_RENDER_TARGET_COUNT = 8 ;
	#endregion
	
	
	static DXSharpUtils( ) {
#if DEBUG || DEV_BUILD
		// ReSharper disable RedundantArgumentDefaultValue
		var a = EncodeShader4ComponentMapping( 0, 1, 2, 3 ) ;
		// ReSharper restore RedundantArgumentDefaultValue
		Debug.Assert( a is DefaultComponentMapping 
						   and ShaderComponentMapping.Default4ComponentMapping ) ;
#endif
	}

	
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

	
	/// <summary>
	/// Generates flags to specify the options for each shader component [0..3] (corresponding to RGBA)..
	/// </summary>
	/// <param name="src0">Return component 0 (red).</param>
	/// <param name="src1">Return component 1 (green).</param>
	/// <param name="src2">Return component 2 (blue).</param>
	/// <param name="src3">Return component 3 (alpha).</param>
	/// <returns>An encoded <see cref="ShaderComponentMapping"/> value.</returns>
	public static ShaderComponentMapping EncodeShader4ComponentMapping( int src0 = 0, int src1 = 1,
																		int src2 = 2, int src3 = 3 ) =>
																(ShaderComponentMapping)
																( ( src0 & ComponentMappingMask ) |
																( ( src1 & ComponentMappingMask ) << ComponentMappingShift ) |
																( ( src2 & ComponentMappingMask ) << (ComponentMappingShift * 2) ) |
																( ( src3 & ComponentMappingMask ) << (ComponentMappingShift * 3) ) |
																	ComponentMappingAlwaysSetBitAvoidingZeromemMistakes) ;
	
	//! Does this even make sense? Lol I'm not sure yet ... perhaps only integers make sense for this ...
	//  We will find out from building stuff and using the code and seeing what we actually need and use ...
	//  Will leave this commented out here for now, and either delete or uncomment and enable it later ...
	/*public static ShaderComponentMapping EncodeShader4ComponentMapping( ShaderComponentMapping src0 = ShaderComponentMapping.FromMemoryComponent0, 
																		ShaderComponentMapping src1 = ShaderComponentMapping.FromMemoryComponent1, 
																		ShaderComponentMapping src2 = ShaderComponentMapping.FromMemoryComponent2,
																		ShaderComponentMapping src3 = ShaderComponentMapping.FromMemoryComponent3 ) =>
																			EncodeShader4ComponentMapping( (int)src0, (int)src1, (int)src2, (int)src3 ) ;*/
	
	public static ShaderComponentMapping DecodeShader4ComponentMapping( int componentToExtract, int mapping ) =>
		(ShaderComponentMapping)( (mapping >> (ComponentMappingShift * componentToExtract)) & ComponentMappingMask ) ;
	public static ShaderComponentMapping DecodeShader4ComponentMapping( int componentToExtract, ShaderComponentMapping mapping ) =>
																	DecodeShader4ComponentMapping( componentToExtract, (int)mapping ) ;
	
} ;