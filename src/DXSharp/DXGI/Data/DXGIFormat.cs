#region Using Directives

using System.Diagnostics ;
using System.Diagnostics.CodeAnalysis ;
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Dxgi.Common;
#endregion

namespace DXSharp.DXGI ;


/// <summary>
/// Resource data formats, including fully-typed and typeless formats. A list of modifiers 
/// at the bottom of the page more fully describes each format type.
/// </summary>
/// <remarks>
/// <para><h3><b>Byte Order (LSB/MSB):</b></h3></para>
/// Most formats have byte-aligned components, and the components are in C-array order 
/// (the least address comes first). For those formats that don't have power-of-2-aligned 
/// components, the first named component is in the least-significant bits.
/// <para><h3><b>Portable Coding for Endian-Independence:</b></h3></para>
/// Rather than adjusting for whether a system uses big-endian or little-endian byte ordering, 
/// you should write portable code, as follows.
/// <para></para>
/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgiformat/ne-dxgiformat-dxgi_format#">Read more on docs.microsoft.com</see>.</para>
/// </remarks>
public enum Format: uint {
	/// <summary>The format is not known.</summary>
	UNKNOWN = 0U,
	/// <summary>A four-component, 128-bit typeless format that supports 32 bits per channel including alpha. ¹</summary>
	R32G32B32A32_TYPELESS = 1U,
	/// <summary>A four-component, 128-bit floating-point format that supports 32 bits per channel including alpha. <sup>1,5,8</sup></summary>
	R32G32B32A32_FLOAT = 2U,
	/// <summary>A four-component, 128-bit unsigned-integer format that supports 32 bits per channel including alpha. ¹</summary>
	R32G32B32A32_UINT = 3U,
	/// <summary>A four-component, 128-bit signed-integer format that supports 32 bits per channel including alpha. ¹</summary>
	R32G32B32A32_SINT = 4U,
	/// <summary>A three-component, 96-bit typeless format that supports 32 bits per color channel.</summary>
	R32G32B32_TYPELESS = 5U,
	/// <summary>A three-component, 96-bit floating-point format that supports 32 bits per color channel.<sup>5,8</sup></summary>
	R32G32B32_FLOAT = 6U,
	/// <summary>A three-component, 96-bit unsigned-integer format that supports 32 bits per color channel.</summary>
	R32G32B32_UINT = 7U,
	/// <summary>A three-component, 96-bit signed-integer format that supports 32 bits per color channel.</summary>
	R32G32B32_SINT = 8U,
	/// <summary>A four-component, 64-bit typeless format that supports 16 bits per channel including alpha.</summary>
	R16G16B16A16_TYPELESS = 9U,
	/// <summary>A four-component, 64-bit floating-point format that supports 16 bits per channel including alpha.<sup>5,7</sup></summary>
	R16G16B16A16_FLOAT = 10U,
	/// <summary>A four-component, 64-bit unsigned-normalized-integer format that supports 16 bits per channel including alpha.</summary>
	R16G16B16A16_UNORM = 11U,
	/// <summary>A four-component, 64-bit unsigned-integer format that supports 16 bits per channel including alpha.</summary>
	R16G16B16A16_UINT = 12U,
	/// <summary>A four-component, 64-bit signed-normalized-integer format that supports 16 bits per channel including alpha.</summary>
	R16G16B16A16_SNORM = 13U,
	/// <summary>A four-component, 64-bit signed-integer format that supports 16 bits per channel including alpha.</summary>
	R16G16B16A16_SINT = 14U,
	/// <summary>A two-component, 64-bit typeless format that supports 32 bits for the red channel and 32 bits for the green channel.</summary>
	R32G32_TYPELESS = 15U,
	/// <summary>A two-component, 64-bit floating-point format that supports 32 bits for the red channel and 32 bits for the green channel.<sup>5,8</sup></summary>
	R32G32_FLOAT = 16U,
	/// <summary>A two-component, 64-bit unsigned-integer format that supports 32 bits for the red channel and 32 bits for the green channel.</summary>
	R32G32_UINT = 17U,
	/// <summary>A two-component, 64-bit signed-integer format that supports 32 bits for the red channel and 32 bits for the green channel.</summary>
	R32G32_SINT = 18U,
	/// <summary>A two-component, 64-bit typeless format that supports 32 bits for the red channel, 8 bits for the green channel, and 24 bits are unused.</summary>
	R32G8X24_TYPELESS = 19U,
	/// <summary>A 32-bit floating-point component, and two unsigned-integer components (with an additional 32 bits). This format supports 32-bit depth, 8-bit stencil, and 24 bits are unused.⁵</summary>
	D32_FLOAT_S8X24_UINT = 20U,
	/// <summary>A 32-bit floating-point component, and two typeless components (with an additional 32 bits). This format supports 32-bit red channel, 8 bits are unused, and 24 bits are unused.⁵</summary>
	R32_FLOAT_X8X24_TYPELESS = 21U,
	/// <summary>A 32-bit typeless component, and two unsigned-integer components (with an additional 32 bits). This format has 32 bits unused, 8 bits for green channel, and 24 bits are unused.</summary>
	X32_TYPELESS_G8X24_UINT = 22U,
	/// <summary>A four-component, 32-bit typeless format that supports 10 bits for each color and 2 bits for alpha.</summary>
	R10G10B10A2_TYPELESS = 23U,
	/// <summary>A four-component, 32-bit unsigned-normalized-integer format that supports 10 bits for each color and 2 bits for alpha.</summary>
	R10G10B10A2_UNORM = 24U,
	/// <summary>A four-component, 32-bit unsigned-integer format that supports 10 bits for each color and 2 bits for alpha.</summary>
	R10G10B10A2_UINT = 25U,
	/// <summary>
	/// <para>Three partial-precision floating-point numbers encoded into a single 32-bit value (a variant of s10e5, which is sign bit, 10-bit mantissa, and 5-bit biased (15) exponent). There are no sign bits, and there is a 5-bit biased (15) exponent for each channel, 6-bit mantissa  for R and G, and a 5-bit mantissa for B, as shown in the following illustration.<sup>5,7</sup> </para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgiformat/ne-dxgiformat-dxgi_format#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	R11G11B10_FLOAT = 26U,
	/// <summary>A four-component, 32-bit typeless format that supports 8 bits per channel including alpha.</summary>
	R8G8B8A8_TYPELESS = 27U,
	/// <summary>A four-component, 32-bit unsigned-normalized-integer format that supports 8 bits per channel including alpha.</summary>
	R8G8B8A8_UNORM = 28U,
	/// <summary>A four-component, 32-bit unsigned-normalized integer sRGB format that supports 8 bits per channel including alpha.</summary>
	R8G8B8A8_UNORM_SRGB = 29U,
	/// <summary>A four-component, 32-bit unsigned-integer format that supports 8 bits per channel including alpha.</summary>
	R8G8B8A8_UINT = 30U,
	/// <summary>A four-component, 32-bit signed-normalized-integer format that supports 8 bits per channel including alpha.</summary>
	R8G8B8A8_SNORM = 31U,
	/// <summary>A four-component, 32-bit signed-integer format that supports 8 bits per channel including alpha.</summary>
	R8G8B8A8_SINT = 32U,
	/// <summary>A two-component, 32-bit typeless format that supports 16 bits for the red channel and 16 bits for the green channel.</summary>
	R16G16_TYPELESS = 33U,
	/// <summary>A two-component, 32-bit floating-point format that supports 16 bits for the red channel and 16 bits for the green channel.<sup>5,7</sup></summary>
	R16G16_FLOAT = 34U,
	/// <summary>A two-component, 32-bit unsigned-normalized-integer format that supports 16 bits each for the green and red channels.</summary>
	R16G16_UNORM = 35U,
	/// <summary>A two-component, 32-bit unsigned-integer format that supports 16 bits for the red channel and 16 bits for the green channel.</summary>
	R16G16_UINT = 36U,
	/// <summary>A two-component, 32-bit signed-normalized-integer format that supports 16 bits for the red channel and 16 bits for the green channel.</summary>
	R16G16_SNORM = 37U,
	/// <summary>A two-component, 32-bit signed-integer format that supports 16 bits for the red channel and 16 bits for the green channel.</summary>
	R16G16_SINT = 38U,
	/// <summary>A single-component, 32-bit typeless format that supports 32 bits for the red channel.</summary>
	R32_TYPELESS = 39U,
	/// <summary>A single-component, 32-bit floating-point format that supports 32 bits for depth.<sup>5,8</sup></summary>
	D32_FLOAT = 40U,
	/// <summary>A single-component, 32-bit floating-point format that supports 32 bits for the red channel.<sup>5,8</sup></summary>
	R32_FLOAT = 41U,
	/// <summary>A single-component, 32-bit unsigned-integer format that supports 32 bits for the red channel.</summary>
	R32_UINT = 42U,
	/// <summary>A single-component, 32-bit signed-integer format that supports 32 bits for the red channel.</summary>
	R32_SINT = 43U,
	/// <summary>A two-component, 32-bit typeless format that supports 24 bits for the red channel and 8 bits for the green channel.</summary>
	R24G8_TYPELESS = 44U,
	/// <summary>A 32-bit z-buffer format that supports 24 bits for depth and 8 bits for stencil.</summary>
	D24_UNORM_S8_UINT = 45U,
	/// <summary>A 32-bit format, that contains a 24 bit, single-component, unsigned-normalized integer, with an additional typeless 8 bits. This format has 24 bits red channel and 8 bits unused.</summary>
	R24_UNORM_X8_TYPELESS = 46U,
	/// <summary>A 32-bit format, that contains a 24 bit, single-component, typeless format,  with an additional 8 bit unsigned integer component. This format has 24 bits unused and 8 bits green channel.</summary>
	X24_TYPELESS_G8_UINT = 47U,
	/// <summary>A two-component, 16-bit typeless format that supports 8 bits for the red channel and 8 bits for the green channel.</summary>
	R8G8_TYPELESS = 48U,
	/// <summary>A two-component, 16-bit unsigned-normalized-integer format that supports 8 bits for the red channel and 8 bits for the green channel.</summary>
	R8G8_UNORM = 49U,
	/// <summary>A two-component, 16-bit unsigned-integer format that supports 8 bits for the red channel and 8 bits for the green channel.</summary>
	R8G8_UINT = 50U,
	/// <summary>A two-component, 16-bit signed-normalized-integer format that supports 8 bits for the red channel and 8 bits for the green channel.</summary>
	R8G8_SNORM = 51U,
	/// <summary>A two-component, 16-bit signed-integer format that supports 8 bits for the red channel and 8 bits for the green channel.</summary>
	R8G8_SINT = 52U,
	/// <summary>A single-component, 16-bit typeless format that supports 16 bits for the red channel.</summary>
	R16_TYPELESS = 53U,
	/// <summary>A single-component, 16-bit floating-point format that supports 16 bits for the red channel.<sup>5,7</sup></summary>
	R16_FLOAT = 54U,
	/// <summary>A single-component, 16-bit unsigned-normalized-integer format that supports 16 bits for depth.</summary>
	D16_UNORM = 55U,
	/// <summary>A single-component, 16-bit unsigned-normalized-integer format that supports 16 bits for the red channel.</summary>
	R16_UNORM = 56U,
	/// <summary>A single-component, 16-bit unsigned-integer format that supports 16 bits for the red channel.</summary>
	R16_UINT = 57U,
	/// <summary>A single-component, 16-bit signed-normalized-integer format that supports 16 bits for the red channel.</summary>
	R16_SNORM = 58U,
	/// <summary>A single-component, 16-bit signed-integer format that supports 16 bits for the red channel.</summary>
	R16_SINT = 59U,
	/// <summary>A single-component, 8-bit typeless format that supports 8 bits for the red channel.</summary>
	R8_TYPELESS = 60U,
	/// <summary>A single-component, 8-bit unsigned-normalized-integer format that supports 8 bits for the red channel.</summary>
	R8_UNORM = 61U,
	/// <summary>A single-component, 8-bit unsigned-integer format that supports 8 bits for the red channel.</summary>
	R8_UINT = 62U,
	/// <summary>A single-component, 8-bit signed-normalized-integer format that supports 8 bits for the red channel.</summary>
	R8_SNORM = 63U,
	/// <summary>A single-component, 8-bit signed-integer format that supports 8 bits for the red channel.</summary>
	R8_SINT = 64U,
	/// <summary>A single-component, 8-bit unsigned-normalized-integer format for alpha only.</summary>
	A8_UNORM = 65U,
	/// <summary>A single-component, 1-bit unsigned-normalized integer format that supports 1 bit for the red channel. ².</summary>
	R1_UNORM = 66U,
	/// <summary>
	/// <para>Three partial-precision floating-point numbers encoded into a single 32-bit value all sharing the same 5-bit exponent (variant of s10e5, which is sign bit, 10-bit mantissa, and 5-bit biased (15) exponent). There is no sign bit, and there is a shared 5-bit biased (15) exponent and a 9-bit mantissa for each channel, as shown in the following illustration. <sup>6,7</sup>. </para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgiformat/ne-dxgiformat-dxgi_format#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	R9G9B9E5_SHAREDEXP = 67U,
	/// <summary>
	/// <para>A four-component, 32-bit unsigned-normalized-integer format. This packed RGB format is analogous to the UYVY format. Each 32-bit block describes a pair of pixels: (R8, G8, B8) and (R8, G8, B8) where the R8/B8 values are repeated, and the G8 values are unique to each pixel. ³ Width must be even.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgiformat/ne-dxgiformat-dxgi_format#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	R8G8_B8G8_UNORM = 68U,
	/// <summary>
	/// <para>A four-component, 32-bit unsigned-normalized-integer format. This packed RGB format is analogous to the YUY2 format. Each 32-bit block describes a pair of pixels: (R8, G8, B8) and (R8, G8, B8) where the R8/B8 values are repeated, and the G8 values are unique to each pixel. ³ Width must be even.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgiformat/ne-dxgiformat-dxgi_format#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	G8R8_G8B8_UNORM = 69U,
	/// <summary>Four-component typeless block-compression format. For information about block-compression formats, see <a href="https://docs.microsoft.com/windows/desktop/direct3d11/texture-block-compression-in-direct3d-11">Texture Block Compression in Direct3D 11</a>.</summary>
	BC1_TYPELESS = 70U,
	/// <summary>Four-component block-compression format. For information about block-compression formats, see <a href="https://docs.microsoft.com/windows/desktop/direct3d11/texture-block-compression-in-direct3d-11">Texture Block Compression in Direct3D 11</a>.</summary>
	BC1_UNORM = 71U,
	/// <summary>Four-component block-compression format for sRGB data. For information about block-compression formats, see <a href="https://docs.microsoft.com/windows/desktop/direct3d11/texture-block-compression-in-direct3d-11">Texture Block Compression in Direct3D 11</a>.</summary>
	BC1_UNORM_SRGB = 72U,
	/// <summary>Four-component typeless block-compression format. For information about block-compression formats, see <a href="https://docs.microsoft.com/windows/desktop/direct3d11/texture-block-compression-in-direct3d-11">Texture Block Compression in Direct3D 11</a>.</summary>
	BC2_TYPELESS = 73U,
	/// <summary>Four-component block-compression format. For information about block-compression formats, see <a href="https://docs.microsoft.com/windows/desktop/direct3d11/texture-block-compression-in-direct3d-11">Texture Block Compression in Direct3D 11</a>.</summary>
	BC2_UNORM = 74U,
	/// <summary>Four-component block-compression format for sRGB data. For information about block-compression formats, see <a href="https://docs.microsoft.com/windows/desktop/direct3d11/texture-block-compression-in-direct3d-11">Texture Block Compression in Direct3D 11</a>.</summary>
	BC2_UNORM_SRGB = 75U,
	/// <summary>Four-component typeless block-compression format. For information about block-compression formats, see <a href="https://docs.microsoft.com/windows/desktop/direct3d11/texture-block-compression-in-direct3d-11">Texture Block Compression in Direct3D 11</a>.</summary>
	BC3_TYPELESS = 76U,
	/// <summary>Four-component block-compression format. For information about block-compression formats, see <a href="https://docs.microsoft.com/windows/desktop/direct3d11/texture-block-compression-in-direct3d-11">Texture Block Compression in Direct3D 11</a>.</summary>
	BC3_UNORM = 77U,
	/// <summary>Four-component block-compression format for sRGB data. For information about block-compression formats, see <a href="https://docs.microsoft.com/windows/desktop/direct3d11/texture-block-compression-in-direct3d-11">Texture Block Compression in Direct3D 11</a>.</summary>
	BC3_UNORM_SRGB = 78U,
	/// <summary>One-component typeless block-compression format. For information about block-compression formats, see <a href="https://docs.microsoft.com/windows/desktop/direct3d11/texture-block-compression-in-direct3d-11">Texture Block Compression in Direct3D 11</a>.</summary>
	BC4_TYPELESS = 79U,
	/// <summary>One-component block-compression format. For information about block-compression formats, see <a href="https://docs.microsoft.com/windows/desktop/direct3d11/texture-block-compression-in-direct3d-11">Texture Block Compression in Direct3D 11</a>.</summary>
	BC4_UNORM = 80U,
	/// <summary>One-component block-compression format. For information about block-compression formats, see <a href="https://docs.microsoft.com/windows/desktop/direct3d11/texture-block-compression-in-direct3d-11">Texture Block Compression in Direct3D 11</a>.</summary>
	BC4_SNORM = 81U,
	/// <summary>Two-component typeless block-compression format. For information about block-compression formats, see <a href="https://docs.microsoft.com/windows/desktop/direct3d11/texture-block-compression-in-direct3d-11">Texture Block Compression in Direct3D 11</a>.</summary>
	BC5_TYPELESS = 82U,
	/// <summary>Two-component block-compression format. For information about block-compression formats, see <a href="https://docs.microsoft.com/windows/desktop/direct3d11/texture-block-compression-in-direct3d-11">Texture Block Compression in Direct3D 11</a>.</summary>
	BC5_UNORM = 83U,
	/// <summary>Two-component block-compression format. For information about block-compression formats, see <a href="https://docs.microsoft.com/windows/desktop/direct3d11/texture-block-compression-in-direct3d-11">Texture Block Compression in Direct3D 11</a>.</summary>
	BC5_SNORM = 84U,
	/// <summary>
	/// <para>A three-component, 16-bit unsigned-normalized-integer format that supports 5 bits for blue, 6 bits for green, and 5 bits for red. <b>Direct3D 10 through Direct3D 11:  </b>This value is defined for DXGI. However, Direct3D 10, 10.1, or 11 devices do not support this format. <b>Direct3D 11.1:  </b>This value is not supported until Windows 8.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgiformat/ne-dxgiformat-dxgi_format#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	B5G6R5_UNORM = 85U,
	/// <summary>
	/// <para>A four-component, 16-bit unsigned-normalized-integer format that supports 5 bits for each color channel and 1-bit alpha. <b>Direct3D 10 through Direct3D 11:  </b>This value is defined for DXGI. However, Direct3D 10, 10.1, or 11 devices do not support this format. <b>Direct3D 11.1:  </b>This value is not supported until Windows 8.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgiformat/ne-dxgiformat-dxgi_format#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	B5G5R5A1_UNORM = 86U,
	/// <summary>A four-component, 32-bit unsigned-normalized-integer format that supports 8 bits for each color channel and 8-bit alpha.</summary>
	B8G8R8A8_UNORM = 87U,
	/// <summary>A four-component, 32-bit unsigned-normalized-integer format that supports 8 bits for each color channel and 8 bits unused.</summary>
	B8G8R8X8_UNORM = 88U,
	/// <summary>A four-component, 32-bit 2.8-biased fixed-point format that supports 10 bits for each color channel and 2-bit alpha.</summary>
	R10G10B10_XR_BIAS_A2_UNORM = 89U,
	/// <summary>A four-component, 32-bit typeless format that supports 8 bits for each channel including alpha. ⁴</summary>
	B8G8R8A8_TYPELESS = 90U,
	/// <summary>A four-component, 32-bit unsigned-normalized standard RGB format that supports 8 bits for each channel including alpha. ⁴</summary>
	B8G8R8A8_UNORM_SRGB = 91U,
	/// <summary>A four-component, 32-bit typeless format that supports 8 bits for each color channel, and 8 bits are unused. ⁴</summary>
	B8G8R8X8_TYPELESS = 92U,
	/// <summary>A four-component, 32-bit unsigned-normalized standard RGB format that supports 8 bits for each color channel, and 8 bits are unused. ⁴</summary>
	B8G8R8X8_UNORM_SRGB = 93U,
	/// <summary>A typeless block-compression format. ⁴ For information about block-compression formats, see <a href="https://docs.microsoft.com/windows/desktop/direct3d11/texture-block-compression-in-direct3d-11">Texture Block Compression in Direct3D 11</a>.</summary>
	BC6H_TYPELESS = 94U,
	/// <summary>A block-compression format. ⁴ For information about block-compression formats, see <a href="https://docs.microsoft.com/windows/desktop/direct3d11/texture-block-compression-in-direct3d-11">Texture Block Compression in Direct3D 11</a>.⁵</summary>
	BC6H_UF16 = 95U,
	/// <summary>A block-compression format. ⁴ For information about block-compression formats, see <a href="https://docs.microsoft.com/windows/desktop/direct3d11/texture-block-compression-in-direct3d-11">Texture Block Compression in Direct3D 11</a>.⁵</summary>
	BC6H_SF16 = 96U,
	/// <summary>A typeless block-compression format. ⁴ For information about block-compression formats, see <a href="https://docs.microsoft.com/windows/desktop/direct3d11/texture-block-compression-in-direct3d-11">Texture Block Compression in Direct3D 11</a>.</summary>
	BC7_TYPELESS = 97U,
	/// <summary>A block-compression format. ⁴ For information about block-compression formats, see <a href="https://docs.microsoft.com/windows/desktop/direct3d11/texture-block-compression-in-direct3d-11">Texture Block Compression in Direct3D 11</a>.</summary>
	BC7_UNORM = 98U,
	/// <summary>A block-compression format. ⁴ For information about block-compression formats, see <a href="https://docs.microsoft.com/windows/desktop/direct3d11/texture-block-compression-in-direct3d-11">Texture Block Compression in Direct3D 11</a>.</summary>
	BC7_UNORM_SRGB = 99U,
	/// <summary>
	/// <para>Most common YUV 4:4:4 video resource format. Valid view formats for this video resource format are R8G8B8A8_UNORM and R8G8B8A8_UINT. For UAVs, an additional valid view format is R32_UINT. By using R32_UINT for UAVs, you can both read and write as opposed to just write for R8G8B8A8_UNORM and R8G8B8A8_UINT. Supported view types are SRV, RTV, and UAV. One view provides a straightforward mapping of the entire surface. The mapping to the view channel is V-&gt;R8, U-&gt;G8, Y-&gt;B8, and A-&gt;A8. For more info about YUV formats for video rendering, see <a href="https://docs.microsoft.com/windows/desktop/medfound/recommended-8-bit-yuv-formats-for-video-rendering">Recommended 8-Bit YUV Formats for Video Rendering</a>. <b>Direct3D 11.1:  </b>This value is not supported until Windows 8.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgiformat/ne-dxgiformat-dxgi_format#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	AYUV = 100U,
	/// <summary>
	/// <para>10-bit per channel packed YUV 4:4:4 video resource format. Valid view formats for this video resource format are R10G10B10A2_UNORM and R10G10B10A2_UINT. For UAVs, an additional valid view format is R32_UINT. By using R32_UINT for UAVs, you can both read and write as opposed to just write for R10G10B10A2_UNORM and R10G10B10A2_UINT. Supported view types are SRV and UAV. One view provides a straightforward mapping of the entire surface. The mapping to the view channel is U-&gt;R10, Y-&gt;G10, V-&gt;B10, and A-&gt;A2. For more info about YUV formats for video rendering, see <a href="https://docs.microsoft.com/windows/desktop/medfound/recommended-8-bit-yuv-formats-for-video-rendering">Recommended 8-Bit YUV Formats for Video Rendering</a>. <b>Direct3D 11.1:  </b>This value is not supported until Windows 8.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgiformat/ne-dxgiformat-dxgi_format#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	Y410 = 101U,
	/// <summary>
	/// <para>16-bit per channel packed YUV 4:4:4 video resource format. Valid view formats for this video resource format are R16G16B16A16_UNORM and R16G16B16A16_UINT. Supported view types are SRV and UAV. One view provides a straightforward mapping of the entire surface. The mapping to the view channel is U-&gt;R16, Y-&gt;G16, V-&gt;B16, and A-&gt;A16. For more info about YUV formats for video rendering, see <a href="https://docs.microsoft.com/windows/desktop/medfound/recommended-8-bit-yuv-formats-for-video-rendering">Recommended 8-Bit YUV Formats for Video Rendering</a>. <b>Direct3D 11.1:  </b>This value is not supported until Windows 8.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgiformat/ne-dxgiformat-dxgi_format#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	Y416 = 102U,
	/// <summary>
	/// <para>Most common YUV 4:2:0 video resource format. Valid luminance data view formats for this video resource format are R8_UNORM and R8_UINT. Valid chrominance data view formats (width and height are each 1/2 of luminance view) for this video resource format are R8G8_UNORM and R8G8_UINT. Supported view types are SRV, RTV, and UAV. For luminance data view, the mapping to the view channel is Y-&gt;R8. For chrominance data view, the mapping to the view channel is U-&gt;R8 and V-&gt;G8. For more info about YUV formats for video rendering, see <a href="https://docs.microsoft.com/windows/desktop/medfound/recommended-8-bit-yuv-formats-for-video-rendering">Recommended 8-Bit YUV Formats for Video Rendering</a>. Width and height must be even. Direct3D 11 staging resources and initData parameters for this format use (rowPitch * (height + (height / 2))) bytes. The first (SysMemPitch * height) bytes are the Y plane, the remaining (SysMemPitch * (height / 2)) bytes are the UV plane. An app using the YUY 4:2:0 formats  must map the luma (Y) plane separately from the chroma (UV) planes. Developers do this by calling <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-createshaderresourceview">ID3D12Device::CreateShaderResourceView</a> twice for the same texture and passing in 1-channel and 2-channel formats. Passing in a 1-channel format compatible with the Y plane maps only the Y plane. Passing in a 2-channel format compatible with the UV planes (together) maps only the U and V planes as a single resource view. <b>Direct3D 11.1:  </b>This value is not supported until Windows 8.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgiformat/ne-dxgiformat-dxgi_format#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	NV12 = 103U,
	/// <summary>
	/// <para>10-bit per channel planar YUV 4:2:0 video resource format. Valid luminance data view formats for this video resource format are R16_UNORM and R16_UINT. The runtime does not enforce whether the lowest 6 bits are 0 (given that this video resource format is a 10-bit format that uses 16 bits). If required, application shader code would have to enforce this manually.  From the runtime's point of view, P010 is no different than P016. Valid chrominance data view formats (width and height are each 1/2 of luminance view) for this video resource format are R16G16_UNORM and R16G16_UINT. For UAVs, an additional valid chrominance data view format is R32_UINT. By using R32_UINT for UAVs, you can both read and write as opposed to just write for R16G16_UNORM and R16G16_UINT. Supported view types are SRV, RTV, and UAV. For luminance data view, the mapping to the view channel is Y-&gt;R16. For chrominance data view, the mapping to the view channel is U-&gt;R16 and V-&gt;G16. For more info about YUV formats for video rendering, see <a href="https://docs.microsoft.com/windows/desktop/medfound/recommended-8-bit-yuv-formats-for-video-rendering">Recommended 8-Bit YUV Formats for Video Rendering</a>. Width and height must be even. Direct3D 11 staging resources and initData parameters for this format use (rowPitch * (height + (height / 2))) bytes. The first (SysMemPitch * height) bytes are the Y plane, the remaining (SysMemPitch * (height / 2)) bytes are the UV plane. An app using the YUY 4:2:0 formats  must map the luma (Y) plane separately from the chroma (UV) planes. Developers do this by calling <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-createshaderresourceview">ID3D12Device::CreateShaderResourceView</a> twice for the same texture and passing in 1-channel and 2-channel formats. Passing in a 1-channel format compatible with the Y plane maps only the Y plane. Passing in a 2-channel format compatible with the UV planes (together) maps only the U and V planes as a single resource view. <b>Direct3D 11.1:  </b>This value is not supported until Windows 8.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgiformat/ne-dxgiformat-dxgi_format#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	P010 = 104U,
	/// <summary>
	/// <para>16-bit per channel planar YUV 4:2:0 video resource format. Valid luminance data view formats for this video resource format are R16_UNORM and R16_UINT. Valid chrominance data view formats (width and height are each 1/2 of luminance view) for this video resource format are R16G16_UNORM and R16G16_UINT. For UAVs, an additional valid chrominance data view format is R32_UINT. By using R32_UINT for UAVs, you can both read and write as opposed to just write for R16G16_UNORM and R16G16_UINT. Supported view types are SRV, RTV, and UAV. For luminance data view, the mapping to the view channel is Y-&gt;R16. For chrominance data view, the mapping to the view channel is U-&gt;R16 and V-&gt;G16. For more info about YUV formats for video rendering, see <a href="https://docs.microsoft.com/windows/desktop/medfound/recommended-8-bit-yuv-formats-for-video-rendering">Recommended 8-Bit YUV Formats for Video Rendering</a>. Width and height must be even. Direct3D 11 staging resources and initData parameters for this format use (rowPitch * (height + (height / 2))) bytes. The first (SysMemPitch * height) bytes are the Y plane, the remaining (SysMemPitch * (height / 2)) bytes are the UV plane. An app using the YUY 4:2:0 formats  must map the luma (Y) plane separately from the chroma (UV) planes. Developers do this by calling <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-createshaderresourceview">ID3D12Device::CreateShaderResourceView</a> twice for the same texture and passing in 1-channel and 2-channel formats. Passing in a 1-channel format compatible with the Y plane maps only the Y plane. Passing in a 2-channel format compatible with the UV planes (together) maps only the U and V planes as a single resource view. <b>Direct3D 11.1:  </b>This value is not supported until Windows 8.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgiformat/ne-dxgiformat-dxgi_format#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	P016 = 105U,
	/// <summary>
	/// <para>8-bit per channel planar YUV 4:2:0 video resource format. This format is subsampled where each pixel has its own Y value, but each 2x2 pixel block shares a single U and V value. The runtime requires that the width and height of all resources that are created with this format are multiples of 2. The runtime also requires that the left, right, top, and bottom members of any RECT that are used for this format are multiples of 2. This format differs from NV12 in that the layout of the data within the resource is completely opaque to applications. Applications cannot use the CPU to map the resource and then access the data within the resource. You cannot use shaders with this format. Because of this behavior, legacy hardware that supports a non-NV12 4:2:0 layout (for example, YV12, and so on) can be used. Also, new hardware that has a 4:2:0 implementation better than NV12 can be used when the application does not need the data to be in a standard layout. For more info about YUV formats for video rendering, see <a href="https://docs.microsoft.com/windows/desktop/medfound/recommended-8-bit-yuv-formats-for-video-rendering">Recommended 8-Bit YUV Formats for Video Rendering</a>. Width and height must be even. Direct3D 11 staging resources and initData parameters for this format use (rowPitch * (height + (height / 2))) bytes. An app using the YUY 4:2:0 formats  must map the luma (Y) plane separately from the chroma (UV) planes. Developers do this by calling <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12device-createshaderresourceview">ID3D12Device::CreateShaderResourceView</a> twice for the same texture and passing in 1-channel and 2-channel formats. Passing in a 1-channel format compatible with the Y plane maps only the Y plane. Passing in a 2-channel format compatible with the UV planes (together) maps only the U and V planes as a single resource view. <b>Direct3D 11.1:  </b>This value is not supported until Windows 8.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgiformat/ne-dxgiformat-dxgi_format#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	_420_OPAQUE = 106U,
	/// <summary>
	/// <para>Most common YUV 4:2:2 video resource format. Valid view formats for this video resource format are R8G8B8A8_UNORM and R8G8B8A8_UINT. For UAVs, an additional valid view format is R32_UINT. By using R32_UINT for UAVs, you can both read and write as opposed to just write for R8G8B8A8_UNORM and R8G8B8A8_UINT. Supported view types are SRV and UAV. One view provides a straightforward mapping of the entire surface. The mapping to the view channel is Y0-&gt;R8, U0-&gt;G8, Y1-&gt;B8, and V0-&gt;A8. A unique valid view format for this video resource format is R8G8_B8G8_UNORM. With this view format, the width of the view appears to be twice what the R8G8B8A8_UNORM or R8G8B8A8_UINT view would be when hardware reconstructs RGBA automatically on read and before filtering.  This Direct3D hardware behavior is legacy and is likely not useful any more. With this view format, the mapping to the view channel is Y0-&gt;R8, U0-&gt; G8[0], Y1-&gt;B8, and V0-&gt; G8[1]. For more info about YUV formats for video rendering, see <a href="https://docs.microsoft.com/windows/desktop/medfound/recommended-8-bit-yuv-formats-for-video-rendering">Recommended 8-Bit YUV Formats for Video Rendering</a>. Width must be even. <b>Direct3D 11.1:  </b>This value is not supported until Windows 8.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgiformat/ne-dxgiformat-dxgi_format#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	YUY2 = 107U,
	/// <summary>
	/// <para>10-bit per channel packed YUV 4:2:2 video resource format. Valid view formats for this video resource format are R16G16B16A16_UNORM and R16G16B16A16_UINT. The runtime does not enforce whether the lowest 6 bits are 0 (given that this video resource format is a 10-bit format that uses 16 bits). If required, application shader code would have to enforce this manually.  From the runtime's point of view, Y210 is no different than Y216. Supported view types are SRV and UAV. One view provides a straightforward mapping of the entire surface. The mapping to the view channel is Y0-&gt;R16, U-&gt;G16, Y1-&gt;B16, and V-&gt;A16. For more info about YUV formats for video rendering, see <a href="https://docs.microsoft.com/windows/desktop/medfound/recommended-8-bit-yuv-formats-for-video-rendering">Recommended 8-Bit YUV Formats for Video Rendering</a>. Width must be even. <b>Direct3D 11.1:  </b>This value is not supported until Windows 8.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgiformat/ne-dxgiformat-dxgi_format#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	Y210 = 108U,
	/// <summary>
	/// <para>16-bit per channel packed YUV 4:2:2 video resource format. Valid view formats for this video resource format are R16G16B16A16_UNORM and R16G16B16A16_UINT. Supported view types are SRV and UAV. One view provides a straightforward mapping of the entire surface. The mapping to the view channel is Y0-&gt;R16, U-&gt;G16, Y1-&gt;B16, and V-&gt;A16. For more info about YUV formats for video rendering, see <a href="https://docs.microsoft.com/windows/desktop/medfound/recommended-8-bit-yuv-formats-for-video-rendering">Recommended 8-Bit YUV Formats for Video Rendering</a>. Width must be even. <b>Direct3D 11.1:  </b>This value is not supported until Windows 8.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgiformat/ne-dxgiformat-dxgi_format#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	Y216 = 109U,
	/// <summary>
	/// <para>Most common planar YUV 4:1:1 video resource format. Valid luminance data view formats for this video resource format are R8_UNORM and R8_UINT. Valid chrominance data view formats (width and height are each 1/4 of luminance view) for this video resource format are R8G8_UNORM and R8G8_UINT. Supported view types are SRV, RTV, and UAV. For luminance data view, the mapping to the view channel is Y-&gt;R8. For chrominance data view, the mapping to the view channel is U-&gt;R8 and V-&gt;G8. For more info about YUV formats for video rendering, see <a href="https://docs.microsoft.com/windows/desktop/medfound/recommended-8-bit-yuv-formats-for-video-rendering">Recommended 8-Bit YUV Formats for Video Rendering</a>. Width must be a multiple of 4. Direct3D11 staging resources and initData parameters for this format use (rowPitch * height * 2) bytes. The first (SysMemPitch * height) bytes are the Y plane, the next ((SysMemPitch / 2) * height) bytes are the UV plane, and the remainder is padding. <b>Direct3D 11.1:  </b>This value is not supported until Windows 8.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgiformat/ne-dxgiformat-dxgi_format#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	NV11 = 110U,
	/// <summary>
	/// <para>4-bit palletized YUV format that is commonly used for DVD subpicture. For more info about YUV formats for video rendering, see <a href="https://docs.microsoft.com/windows/desktop/medfound/recommended-8-bit-yuv-formats-for-video-rendering">Recommended 8-Bit YUV Formats for Video Rendering</a>. <b>Direct3D 11.1:  </b>This value is not supported until Windows 8.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgiformat/ne-dxgiformat-dxgi_format#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	AI44 = 111U,
	/// <summary>
	/// <para>4-bit palletized YUV format that is commonly used for DVD subpicture. For more info about YUV formats for video rendering, see <a href="https://docs.microsoft.com/windows/desktop/medfound/recommended-8-bit-yuv-formats-for-video-rendering">Recommended 8-Bit YUV Formats for Video Rendering</a>. <b>Direct3D 11.1:  </b>This value is not supported until Windows 8.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgiformat/ne-dxgiformat-dxgi_format#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	IA44 = 112U,
	/// <summary>
	/// <para>8-bit palletized format that is used for palletized RGB data when the processor processes ISDB-T data and for palletized YUV data when the processor processes BluRay data. For more info about YUV formats for video rendering, see <a href="https://docs.microsoft.com/windows/desktop/medfound/recommended-8-bit-yuv-formats-for-video-rendering">Recommended 8-Bit YUV Formats for Video Rendering</a>. <b>Direct3D 11.1:  </b>This value is not supported until Windows 8.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgiformat/ne-dxgiformat-dxgi_format#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	P8 = 113U,
	/// <summary>
	/// <para>8-bit palletized format with 8 bits of alpha that is used for palletized YUV data when the processor processes BluRay data. For more info about YUV formats for video rendering, see <a href="https://docs.microsoft.com/windows/desktop/medfound/recommended-8-bit-yuv-formats-for-video-rendering">Recommended 8-Bit YUV Formats for Video Rendering</a>. <b>Direct3D 11.1:  </b>This value is not supported until Windows 8.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgiformat/ne-dxgiformat-dxgi_format#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	A8P8 = 114U,
	/// <summary>
	/// <para>A four-component, 16-bit unsigned-normalized integer format that supports 4 bits for each channel including alpha. <b>Direct3D 11.1:  </b>This value is not supported until Windows 8.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgiformat/ne-dxgiformat-dxgi_format#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	B4G4R4A4_UNORM = 115U,
	/// <summary>A video format; an 8-bit version of a hybrid planar 4:2:2 format.</summary>
	P208 = 130U,
	/// <summary>An 8 bit YCbCrA 4:4 rendering format.</summary>
	V208 = 131U,
	/// <summary>An 8 bit YCbCrA 4:4:4:4 rendering format.</summary>
	V408 = 132U,
	/// <summary>
	/// For information about sampler feedback please see the 
	/// <a href="https://microsoft.github.io/DirectX-Specs/d3d/SamplerFeedback.html">article by Microsoft</a> ...
	/// </summary>
	SAMPLER_FEEDBACK_MIN_MIP_OPAQUE = 189U,
	/// <summary>
	/// For information about sampler feedback please see the 
	/// <a href="https://microsoft.github.io/DirectX-Specs/d3d/SamplerFeedback.html">article by Microsoft</a> ...
	/// </summary>
	SAMPLER_FEEDBACK_MIP_REGION_USED_OPAQUE = 190U,
	/// <summary>
	/// <para>Forces this enumeration to compile to 32 bits in size. Without this value, some compilers would allow this enumeration to compile to a size other than 32 bits. This value is not used.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgiformat/ne-dxgiformat-dxgi_format#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	FORCE_UINT = 4294967295U,
	
} ; // <-- enum: DXGI_FORMAT



/// <summary>
/// A utility class for handling <see cref="Format"/> enumeration values (equivalent of <see cref="DXGI_FORMAT"/>).
/// The <b>DXGI_FORMAT</b> enumeration lists the formats that a surface or some kind of resource/data can have.<para/>
/// There are several groups of <see cref="Format"/>:
/// <list type="bullet">
/// <item><description>Block-compression formats</description></item>
/// <item><description>Planar formats</description></item>
/// <item><description>Video formats</description></item>
/// <item><description>Integer formats</description></item>
/// <item><description>Float formats</description></item>
/// <item><description>Depth-stencil formats</description></item>
/// <item><description>SRGB formats</description></item>
/// <item><description>Legacy formats</description></item>
/// <item><description>Reserved formats</description></item>
/// </list>
/// <para>For more info about DXGI formats, see
/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgiformat/ne-dxgiformat-dxgi_format">DXGI_FORMAT</a>.</para>
/// <seealso cref="DXGI_FORMAT"/>
/// </summary>
public static class Formats {
	public static ReadOnlyMemory< Format > AllFormats =
		(Memory<Format>)Enum.GetValues( typeof(Format) ) ;
	
	/// <summary>Utility class for <see cref="Format"/> names in <see cref="string"/> form.</summary>
	public static partial class Names {
		
		/// <summary>The prefix used in naming enumerated format constant values in Direct3D/DXGI headers.</summary>
		public const string FormatPrefix = "DXGI_FORMAT_" ;

		/// <summary>Gets a read-only span of all DXGI format names (not including the "DXGI_FORMAT_" prefix).</summary>
		public static ReadOnlySpan< string > AllFormatNames => _allFormatNames.Span ;
		static readonly ReadOnlyMemory< string > _allFormatNames = new[ ] {
			"UNKNOWN",
			"R32G32B32A32_TYPELESS",
			"R32G32B32A32_FLOAT",
			"R32G32B32A32_UINT",
			"R32G32B32A32_SINT",
			"R32G32B32_TYPELESS",
			"R32G32B32_FLOAT",
			"R32G32B32_UINT",
			"R32G32B32_SINT",
			"R16G16B16A16_TYPELESS",
			"R16G16B16A16_FLOAT",
			"R16G16B16A16_UNORM",
			"R16G16B16A16_UINT",
			"R16G16B16A16_SNORM",
			"R16G16B16A16_SINT",
			"R32G32_TYPELESS",
			"R32G32_FLOAT",
			"R32G32_UINT",
			"R32G32_SINT",
			"R32G8X24_TYPELESS",
			"D32_FLOAT_S8X24_UINT",
			"R32_FLOAT_X8X24_TYPELESS",
			"X32_TYPELESS_G8X24_UINT",
			"R10G10B10A2_TYPELESS",
			"R10G10B10A2_UNORM",
			"R10G10B10A2_UINT",
			"R11G11B10_FLOAT",
			"R8G8B8A8_TYPELESS",
			"R8G8B8A8_UNORM",
			"R8G8B8A8_UNORM_SRGB",
			"R8G8B8A8_UINT",
			"R8G8B8A8_SNORM",
			"R8G8B8A8_SINT",
			"R16G16_TYPELESS",
			"R16G16_FLOAT",
			"R16G16_UNORM",
			"R16G16_UINT",
			"R16G16_SNORM",
			"R16G16_SINT",
			"R32_TYPELESS",
			"D32_FLOAT",
			"R32_FLOAT",
			"R32_UINT",
			"R32_SINT",
			"R24G8_TYPELESS",
			"D24_UNORM_S8_UINT",
			"R24_UNORM_X8_TYPELESS",
			"X24_TYPELESS_G8_UINT",
			"R8G8_TYPELESS",
			"R8G8_UNORM",
			"R8G8_UINT",
			"R8G8_SNORM",
			"R8G8_SINT",
			"R16_TYPELESS",
			"R16_FLOAT",
			"D16_UNORM",
			"R16_UNORM",
			"R16_UINT",
			"R16_SNORM",
			"R16_SINT",
			"R8_TYPELESS",
			"R8_UNORM",
			"R8_UINT",
			"R8_SNORM",
			"R8_SINT",
			"A8_UNORM",
			"R1_UNORM",
			"R9G9B9E5_SHAREDEXP",
			"R8G8_B8G8_UNORM",
			"G8R8_G8B8_UNORM",
			"BC1_TYPELESS",
			"BC1_UNORM",
			"BC1_UNORM_SRGB",
			"BC2_TYPELESS",
			"BC2_UNORM",
			"BC2_UNORM_SRGB",
			"BC3_TYPELESS",
			"BC3_UNORM",
			"BC3_UNORM_SRGB",
			"BC4_TYPELESS",
			"BC4_UNORM",
			"BC4_SNORM",
			"BC5_TYPELESS",
			"BC5_UNORM",
			"BC5_SNORM",
			"B5G6R5_UNORM",
			"B5G5R5A1_UNORM",
			"B8G8R8A8_UNORM",
			"B8G8R8X8_UNORM",
			"R10G10B10_XR_BIAS_A2_UNORM",
			"B8G8R8A8_TYPELESS",
			"B8G8R8A8_UNORM_SRGB",
			"B8G8R8X8_TYPELESS",
			"B8G8R8X8_UNORM_SRGB",
			"BC6H_TYPELESS",
			"BC6H_UF16",
			"BC6H_SF16",
			"BC7_TYPELESS",
			"BC7_UNORM",
			"BC7_UNORM_SRGB",
			"AYUV",
			"Y410",
			"Y416",
			"NV12",
			"P010",
			"P016",
			"420_OPAQUE",
			"YUY2",
			"Y210",
			"Y216",
			"NV11",
			"AI44",
			"IA44",
			"P8",
			"A8P8",
		} ;
		
		static Names( ) {
			// Ensure that the Format enum and the format names array are in sync.
			// If they are not, then the Format enum has been changed without updating this class.
			// This is a serious error that must be fixed.
#if DEBUG
			System.Diagnostics.Debug.Assert( AllFormatNames.Length == AllFormats.Length ) ;
#endif
		}
		
		/// <summary>
		/// Indicates if a format name begins with the common prefix
		/// (i.e., "<c>DXGI_FORMAT_</c>").
		/// </summary>
		public static bool IsPrefixed( string formatName ) =>
			formatName.StartsWith( FormatPrefix, StringComparison.Ordinal ) ;
		
		/// <summary>
		/// Removes the common prefix (i.e., "<c>DXGI_FORMAT_</c>") from a format name.
		/// </summary>
		/// <param name="formatName">The format name to remove the prefix from.</param>
		/// <returns>The format name without the prefix.</returns>
		public static string WithoutPrefix( string formatName ) =>
			IsPrefixed( formatName ) ? formatName.Substring( FormatPrefix.Length ) : formatName ;
		
		/// <summary>
		/// Adds the common prefix (i.e., "<c>DXGI_FORMAT_</c>") to a format name.
		/// </summary>
		/// <param name="formatName">The format name to add the prefix to.</param>
		/// <returns>The format name with the prefix.</returns>
		/// <remarks>
		/// If the format name already has the prefix, it is returned unchanged.
		/// </remarks>
		public static string WithPrefix( string formatName ) =>
			IsPrefixed( formatName ) ? formatName : FormatPrefix + formatName ;
	}
} ;