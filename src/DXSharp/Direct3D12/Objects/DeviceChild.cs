#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[Wrapper(typeof(ID3D12DeviceChild))]
internal abstract class DeviceChild: Object,
									 IDeviceChild,
									 IComObjectRef< ID3D12DeviceChild >,
									 IUnknownWrapper< ID3D12DeviceChild > {
	// ---------------------------------------------------------------------------------
	public override ID3D12DeviceChild? COMObject => ComPointer?.Interface ;

	ComPtr< ID3D12DeviceChild >? _comPtr ;
	public new virtual ComPtr< ID3D12DeviceChild >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< ID3D12DeviceChild >( ) ;
	
	// ---------------------------------------------------------------------------------
	
	protected DeviceChild( ) {
		_comPtr = ComResources?.GetPointer<ID3D12DeviceChild>(  ) ;
	}
	protected DeviceChild( nint childAddr ) {
		_comPtr = new( childAddr ) ;
		_initOrAdd( _comPtr ) ;
	}
	protected DeviceChild( ID3D12DeviceChild child ) {
		_comPtr = new( child ) ;
		_initOrAdd( _comPtr ) ;
	}
	protected DeviceChild( ComPtr< IUnknown > childPtr ) => _initOrAdd( _comPtr! ) ;


	// ---------------------------------------------------------------------------------

	public void GetDevice( in Guid riid, out IDevice ppvDevice ) {
		unsafe {
			Guid _riid = riid ;
			COMObject!.GetDevice( &_riid, out var _ppvDevice ) ;
			ppvDevice = (IDevice)_ppvDevice ;
		}
	}

	// ---------------------------------------------------------------------------------
	
	public new static Type ComType => typeof( ID3D12DeviceChild ) ;
		public new static ref readonly Guid Guid {
    		[MethodImpl( MethodImplOptions.AggressiveInlining )]
    		get {
    			ReadOnlySpan< byte > data = typeof(ID3D12DeviceChild).GUID
    															.ToByteArray( ) ;
    			
    			return ref Unsafe
    					   .As< byte, Guid >( ref MemoryMarshal
    											  .GetReference(data) ) ;
    		}
    	}
	// ---------------------------------------------------------------------------------
} ;