using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D.Dxc ;
using DXSharp.Dxc ;
using DXSharp.Windows.COM ;

namespace AdvancedDXS.HLSL ;

public class ShaderBuilder {
	IDxcUtils?       pUtils ;
	IDxcCompiler3?   pCompiler ;
	
	public ShaderBuilder( ) {
		Guid CLSID_DxcCompiler = new( 0x73e22d93, 0xe6ce, 0x47f3, 0xbc, 0x43, 0x69, 0x73, 0x8b, 0x20, 0x0e, 0x43 ) ;
		Guid IID_IDxcCompiler3 = new( 0x5e7fceef, 0x8587, 0x4cfb, 0xa2, 0x50, 0x7e, 0x1f, 0x0e, 0x1d, 0x0e, 0x27 ) ;

		IDxcPdbUtils?     pPdbUtils  = null ;
		
		Guid              g = DxcPInvoke.CLSID_DxcCompiler ;
	}
} ;