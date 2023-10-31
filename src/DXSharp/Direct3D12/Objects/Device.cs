#region Using Directives

using System.Runtime.Versioning ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.DXGI ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[Wrapper(typeof(ID3D12Device))]
public class Device: Object,
					 IDevice {
	public new ID3D12Device? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12Device >? ComPointer { get ; protected set ; }
	
	internal Device( ) { }
	internal Device( ComPtr< ID3D12Device > comPtr ) => ComPointer = comPtr ;
	internal Device( nint address ) => ComPointer = new( address ) ;
	internal Device( ID3D12Device comObject ) => ComPointer = new( comObject ) ;
	
	// -------------------------------------------------------------------------------------------------------
	// Static Methods:
	// -------------------------------------------------------------------------------------------------------

	public static T CreateDevice< T >( IAdapter? adapter = null,
									   FeatureLevel featureLevel = FeatureLevel.D3D12_0 )
															where T: IDevice, IInstantiable {
		var guid = T.InterfaceGUID ;
		PInvoke.D3D12CreateDevice( adapter?.COMObject ?? null, (D3D_FEATURE_LEVEL)featureLevel, guid, out var ppDevice ) ;
		return (T)T.Instantiate( (ID3D12Device)ppDevice ) ;
	}
	
	// =======================================================================================================
} ;


[Wrapper(typeof(ID3D12Device1))]
public class Device1: Device,
					  IDevice1 {
	public new ID3D12Device1? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12Device1 >? ComPointer { get ; protected set ; }
	
	internal Device1( ) { }
	internal Device1( ComPtr< ID3D12Device1 > comPtr ) => ComPointer = comPtr ;
	internal Device1( nint address ) => ComPointer = new( address ) ;
	internal Device1( ID3D12Device1 comObject ) => ComPointer = new( comObject ) ;
	
	// =======================================================================================================
} ;


[Wrapper(typeof(ID3D12Device2))]
public class Device2: Device1,
					  IDevice2 {
	public new ID3D12Device2? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12Device2 >? ComPointer { get ; protected set ; }
	
	internal Device2( ) { }
	internal Device2( ComPtr< ID3D12Device2 > comPtr ) => ComPointer = comPtr ;
	internal Device2( nint address ) => ComPointer = new( address ) ;
	internal Device2( ID3D12Device2 comObject ) => ComPointer = new( comObject ) ;
	
	// =======================================================================================================
} ;


[Wrapper(typeof(ID3D12Device3))]
public class Device3: Device2,
					  IDevice3 {
	public new ID3D12Device3? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12Device3 >? ComPointer { get ; protected set ; }
	
	internal Device3( ) { }
	internal Device3( ComPtr< ID3D12Device3 > comPtr ) => ComPointer = comPtr ;
	internal Device3( nint address ) => ComPointer = new( address ) ;
	internal Device3( ID3D12Device3 comObject ) => ComPointer = new( comObject ) ;
	
	// =======================================================================================================
} ;


[Wrapper(typeof(ID3D12Device4))]
public class Device4: Device3,
					  IDevice4 {
	public new ID3D12Device4? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12Device4 >? ComPointer { get ; protected set ; }
	
	internal Device4( ) { }
	internal Device4( ComPtr< ID3D12Device4 > comPtr ) => ComPointer = comPtr ;
	internal Device4( nint address ) => ComPointer = new( address ) ;
	internal Device4( ID3D12Device4 comObject ) => ComPointer = new( comObject ) ;
	
	// =======================================================================================================
} ;


[SupportedOSPlatform("windows10.0.17763")]
[Wrapper(typeof(ID3D12Device5))]
public class Device5: Device4,
					  IDevice5 {
	public new ID3D12Device5? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12Device5 >? ComPointer { get ; protected set ; }
	
	internal Device5( ) { }
	internal Device5( ComPtr< ID3D12Device5 > comPtr ) => ComPointer = comPtr ;
	
	[SupportedOSPlatform("windows10.0.17763")]
	internal Device5( nint address ) => ComPointer = new( address ) ;
	internal Device5( ID3D12Device5 comObject ) => ComPointer = new( comObject ) ;
	
	
	// =======================================================================================================
} ;


[Wrapper(typeof(ID3D12Device6))]
public class Device6: Device5,
					  IDevice6 {
	public new ID3D12Device6? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12Device6 >? ComPointer { get ; protected set ; }
	
	internal Device6( ) { }
	internal Device6( ComPtr< ID3D12Device6 > comPtr ) => ComPointer = comPtr ;
	internal Device6( nint address ) => ComPointer = new( address ) ;
	internal Device6( ID3D12Device6 comObject ) => ComPointer = new( comObject ) ;
	
	// =======================================================================================================
} ;


[Wrapper(typeof(ID3D12Device7))]
public class Device7: Device6,
					  IDevice7 {
	public new ID3D12Device7? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12Device7 >? ComPointer { get ; protected set ; }
	
	internal Device7( ) { }
	internal Device7( ComPtr< ID3D12Device7 > comPtr ) => ComPointer = comPtr ;
	internal Device7( nint address ) => ComPointer = new( address ) ;
	internal Device7( ID3D12Device7 comObject ) => ComPointer = new( comObject ) ;
	
	// =======================================================================================================
} ;


[Wrapper(typeof(ID3D12Device8))]
public class Device8: Device7,
					  IDevice8 {
	public new ID3D12Device8? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12Device8 >? ComPointer { get ; protected set ; }
	
	internal Device8( ) { }
	internal Device8( ComPtr< ID3D12Device8 > comPtr ) => ComPointer = comPtr ;
	internal Device8( nint address ) => ComPointer = new( address ) ;
	internal Device8( ID3D12Device8 comObject ) => ComPointer = new( comObject ) ;
	
	// =======================================================================================================
} ;


[Wrapper(typeof(ID3D12Device9))]
public class Device9: Device8,
					  IDevice9 {
	public new ID3D12Device9? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12Device9 >? ComPointer { get ; protected set ; }
	
	internal Device9( ) { }
	internal Device9( ComPtr< ID3D12Device9 > comPtr ) => ComPointer = comPtr ;
	internal Device9( nint address ) => ComPointer = new( address ) ;
	internal Device9( ID3D12Device9 comObject ) => ComPointer = new( comObject ) ;
	
	// =======================================================================================================
} ;


[Wrapper(typeof(ID3D12Device10))]
public class Device10: Device9,
					   IDevice10 {
	public new ID3D12Device10? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12Device10 >? ComPointer { get ; protected set ; }
	
	internal Device10( ) { }
	internal Device10( ComPtr< ID3D12Device10 > comPtr ) => ComPointer = comPtr ;
	internal Device10( nint address ) => ComPointer = new( address ) ;
	internal Device10( ID3D12Device10 comObject ) => ComPointer = new( comObject ) ;
	
	// =======================================================================================================
} ;


[Wrapper(typeof(ID3D12Device11))]
public class Device11: Device10,
					   IDevice11 {
	public new ID3D12Device11? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12Device11 >? ComPointer { get ; protected set ; }
	
	internal Device11( ) { }
	internal Device11( ComPtr< ID3D12Device11 > comPtr ) => ComPointer = comPtr ;
	internal Device11( nint address ) => ComPointer = new( address ) ;
	internal Device11( ID3D12Device11 comObject ) => ComPointer = new( comObject ) ;
	
	// =======================================================================================================
} ;


[Wrapper(typeof(ID3D12Device12))]
public class Device12: Device11,
					   IDevice12 {
	public new ID3D12Device12? COMObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12Device12 >? ComPointer { get ; protected set ; }
	
	internal Device12( ) { }
	internal Device12( ComPtr< ID3D12Device12 > comPtr ) => ComPointer = comPtr ;
	internal Device12( nint address ) => ComPointer = new( address ) ;
	internal Device12( ID3D12Device12 comObject ) => ComPointer = new( comObject ) ;
	
	// =======================================================================================================
} ;