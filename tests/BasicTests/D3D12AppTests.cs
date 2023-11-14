using Windows.Win32.Foundation ;
using DXSharp.Windows.Win32 ;
using DXSharp.Windows.Win32.Helpers ;
using DXSharp.Windows.Win32.XTensions ;

using static Windows.Win32.PInvoke;
namespace BasicTests ;

[TestFixture, FixtureLifeCycle( LifeCycle.SingleInstance )]
public class D3D12AppTests {
	static DXApp? _game ;
	//static Win32Application? _win32App ;
	static HMODULE hInstance = HMODULE.Null ;
	
	[OneTimeSetUp] public void SetUp( ) {
		hInstance = GetModuleHandle( (PCWSTR)default ) ;
	}
	 
	[TearDown] public void Cleanup( ) {
		Win32Application.Run( _game, HMODULE.Null, ShowWindowCommands.SW_SHOWDEFAULT ) ;
		_game?.Dispose( ) ;
	}
	
	[Test, Order( 0 )]
	public void Test_Create_DXApp( ) {
		_game = new HelloWin32App( 1280, 720, "D3D12App" ) ;
		Assert.IsNotNull( _game ) ;
		Win32Application.Run( _game, hInstance, ShowWindowCommands.SW_SHOWDEFAULT ) ;
	}
} ;