using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

namespace BasicTests.COM ;

[TestFixture( Author = "Aaron T. Carter", 
			  Description = "Tests proxy interfaces/structs for native COM IUnknown interface." )]
public class Test_IUnknown_Proxies {
	static ID3D12Device1? testDevice = null ;

	[SetUp] public void Setup( ) {
		Guid riid = typeof( ID3D12Device1 ).GUID ;
		var hr = PInvoke.D3D12CreateDevice( null, D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0, 
								   riid, out var _rcw ) ;
		hr.ThrowOnFailure( ) ;
		testDevice = (ID3D12Device1)_rcw ;
	}

	[TearDown] public void TearDown( ) {
		if( testDevice != null ) {
			Marshal.FinalReleaseComObject( testDevice ) ;
		}
	}
	
	[Test] public void Test_IUnknown_QueryInterface( ) {
		var riid = typeof( ID3D12Device ).GUID ;
		var hr = testDevice!.QueryInterface( riid, out nint _ppv ) ;
		
		hr.ThrowOnFailure( ) ;
		Assert.That( _ppv.IsValid( ) ) ;
	}
} ;