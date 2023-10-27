#region Using Directives

using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


public abstract class DeviceChild: Object, IDeviceChild {
	public new ID3D12DeviceChild? COMObject => ComPointer?.Interface ;
	public new virtual ComPtr< ID3D12DeviceChild >? ComPointer { get ; }
	
	ID3D12Object? IObject.COMObject => ComPointer?.Interface ;
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
	

	protected DeviceChild( ) =>
		ComPointer = null ;
	protected DeviceChild( nint childAddr ) =>
		ComPointer = new( childAddr ) ;
	protected DeviceChild( ID3D12DeviceChild child ) =>
		ComPointer = new( child ) ;
	protected DeviceChild( ComPtr< ID3D12DeviceChild > childPtr ) =>
		ComPointer = childPtr ;
} ;