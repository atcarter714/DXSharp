#region Using Directives

using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI ;


/// <summary>Inherited from objects that are tied to the device so that they can retrieve a pointer to it.</summary>
/// <remarks>
/// Represents a <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nn-dxgi-idxgidevicesubobject">
/// IDXGIDeviceSubObject interface
/// </a>.
/// </remarks>
[Wrapper(typeof(IDXGIDeviceSubObject))]
internal class DeviceSubObject: Object,
								IDeviceSubObject,
								IComObjectRef< IDXGIDeviceSubObject >,
								IUnknownWrapper< IDXGIDeviceSubObject > {
	ComPtr< IDXGIDeviceSubObject >? _comPtr ;
	public new ComPtr< IDXGIDeviceSubObject >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IDXGIDeviceSubObject >( ) ;
	public override IDXGIDeviceSubObject? ComObject => ComPointer?.Interface ;
	
	
	public DeviceSubObject( ) {
		_comPtr = ComResources?.GetPointer< IDXGIDeviceSubObject >( ) ;
	}
	public DeviceSubObject( nint ptr ) {
		_comPtr = new ComPtr< IDXGIDeviceSubObject >( ptr ) ;
	}
	public DeviceSubObject( ComPtr< IDXGIDeviceSubObject > ptr ) {
		_comPtr = ptr ;
	}
	public DeviceSubObject( IDXGIDeviceSubObject ptr ) {
		_comPtr = new ComPtr< IDXGIDeviceSubObject >( ptr ) ;
	}
	
	
	public T GetDevice< T >( ) where T: IDevice {
		unsafe {
			var riid = typeof( T ).GUID ;
			this.ComObject!.GetDevice( &riid, out var ppDevice ) ;
			return (T)( T.Instantiate( ( ppDevice as IDXGIDevice )! ) ) ;
		}
	}
	
	// ----------------------------------------------------------
	public new static Type ComType => typeof( IDXGIDeviceSubObject ) ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIDeviceSubObject).GUID
																		.ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	// ==========================================================
} ;
