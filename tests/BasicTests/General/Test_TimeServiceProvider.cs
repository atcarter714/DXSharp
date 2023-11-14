using DXSharp.Applications ;

namespace BasicTests.General ;

[TestFixture]
public class Test_App_ServiceProvider_Time {
	static Task? _runTask ;
	static readonly Time _time = new( ) ;
	
	[SetUp] public void Setup( ) { }
	[TearDown] public void TearDown( ) { }

	[Test, Order( 0 )]
	public void Test_Initialize_Time( ) {
		Assert.That( _time, Is.Not.Null ) ;
		
		_time.Start( ) ;
		_runTask = _time.RunAsync( ) ;
	}
}