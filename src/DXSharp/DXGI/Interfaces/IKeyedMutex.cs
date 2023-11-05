#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32 ;
using Windows.Win32.Graphics.Dxgi ;

using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI ;


/// <summary>
/// Represents a keyed mutex, which allows exclusive access
/// to a shared resource that is used by multiple devices.
/// </summary>
/// <remarks>
/// <see cref="IFactory1"/> interface is required to create a resource
/// capable of supporting the <see cref="IKeyedMutex"/> interface.
/// </remarks>
[ProxyFor(typeof(IDXGIKeyedMutex))]
public interface IKeyedMutex: IDeviceSubObject, IInstantiable {
	// ----------------------------------------------------------
	// Interface Methods:
	// ----------------------------------------------------------
	
	/// <summary>Using a key, acquires exclusive rendering access to a shared resource.</summary>
	/// <param name="Key">
	/// <para>A value that indicates which device to give access to. This method will succeed when the device that currently
	/// owns the surface calls the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgikeyedmutex-releasesync">IDXGIKeyedMutex::ReleaseSync</a>
	/// method using the same value. This value can be any UINT64 value.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgikeyedmutex-acquiresync#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="dwMilliseconds">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">DWORD</a></b> The time-out interval, in milliseconds.
	/// This method will return if the interval elapses, and the keyed mutex has not been released  using the specified <i>Key</i>.
	/// If this value is set to zero, the <b>AcquireSync</b> method will test to see if the keyed mutex has been released and returns immediately.
	/// If this value is set to INFINITE (<see cref="uint.MaxValue"/> or <b>0xFFFFFFFF</b>), the time-out interval will never elapse.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgikeyedmutex-acquiresync#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b>
	/// Return S_OK if successful. If the owning device attempted to create another keyed mutex on the same shared resource,
	/// <b>AcquireSync</b> returns E_FAIL. <b>AcquireSync</b> can also return the following
	/// <a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">DWORD</a> constants.
	/// Therefore, you should explicitly check for these constants. If you only use the
	/// <a href="https://docs.microsoft.com/windows/desktop/api/winerror/nf-winerror-succeeded">SUCCEEDED</a>
	/// macro on the return value to determine if  <b>AcquireSync</b> succeeded, you will not catch these constants. </para>
	/// </returns>
	/// <remarks>
	/// <para>The <b>AcquireSync</b> method creates a lock to a surface that is shared between multiple devices, allowing only
	/// one device to render to a surface at a time. This method uses a key to determine which device currently has exclusive
	/// access to the surface. When a surface is created using the <b>D3D10_RESOURCE_MISC_SHARED_KEYEDMUTEX</b> value of the
	/// <a href="https://docs.microsoft.com/windows/desktop/api/d3d10/ne-d3d10-d3d10_resource_misc_flag">D3D10_RESOURCE_MISC_FLAG</a>
	/// enumeration, you must call the <b>AcquireSync</b> method before rendering to the surface.  You must call the
	/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgikeyedmutex-releasesync">ReleaseSync</a> method when
	/// you are done rendering to a surface. To acquire a reference to the keyed mutex object of a shared resource, call the
	/// <a href="https://docs.microsoft.com/windows/desktop/api/unknwn/nf-unknwn-iunknown-queryinterface(q)">QueryInterface</a> method
	/// of the resource and pass in the <b>UUID</b> of the
	/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgikeyedmutex">IDXGIKeyedMutex</a> interface.
	/// For more information about acquiring this reference, see the following code example.
	/// The <b>AcquireSync</b> method uses the key as follows, depending on the state of the surface: </para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgikeyedmutex-acquiresync#">Read more on docs.microsoft.com</a>.</para>
	/// <seealso cref="ReleaseSync"/>
	/// </remarks>
	void AcquireSync( ulong Key, uint dwMilliseconds ) ;

	/// <summary>Using a key, releases exclusive rendering access to a shared resource.</summary>
	/// <param name="Key">
	/// <para>A value that indicates which device to give access to. This method succeeds when the device that currently owns the surface calls the
	/// <b>ReleaseSync</b> method using the same value. This value can be any UINT64 value.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgikeyedmutex-releasesync#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns S_OK if successful.
	/// If the device attempted to release a keyed mutex that is not valid or owned by the device, <b>ReleaseSync</b> returns E_FAIL.</para>
	/// </returns>
	/// <remarks>
	/// <para>The <b>ReleaseSync</b> method releases a lock to a surface that is shared between multiple devices.
	/// This method uses a key to determine which device currently has exclusive access to the surface.
	/// When a surface is created using the <b>D3D10_RESOURCE_MISC_SHARED_KEYEDMUTEX</b> value of the
	/// <a href="https://docs.microsoft.com/windows/desktop/api/d3d10/ne-d3d10-d3d10_resource_misc_flag">D3D10_RESOURCE_MISC_FLAG</a>
	/// enumeration, you must call the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgikeyedmutex-acquiresync">IDXGIKeyedMutex::AcquireSync</a>
	/// method before rendering to the surface.  You must call the <b>ReleaseSync</b> method when you are done rendering to a surface.
	/// After you call the <b>ReleaseSync</b> method, the shared resource is unset from the rendering pipeline.
	/// To acquire a reference to the keyed mutex object of a shared resource, call the
	/// <a href="https://docs.microsoft.com/windows/desktop/api/unknwn/nf-unknwn-iunknown-queryinterface(q)">QueryInterface</a> method of
	/// the resource and pass in the <b>UUID</b> of the
	/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgikeyedmutex">IDXGIKeyedMutex</a> interface.
	/// For more information about acquiring this reference, see the following code example.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/nf-dxgi-idxgikeyedmutex-releasesync#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	/// <seealso cref="AcquireSync"/>
	void ReleaseSync( ulong Key ) ;
	
	// ----------------------------------------------------------
	public new static Type ComType => typeof( IDXGIKeyedMutex ) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIKeyedMutex).GUID
															.ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new KeyedMutex( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new KeyedMutex( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new KeyedMutex( pComObj! ) ;
	// ==========================================================
} ;


[Wrapper( typeof( IDXGIKeyedMutex ) )]
internal class KeyedMutex: DeviceSubObject,
						   IKeyedMutex,
						   IComObjectRef< IDXGIKeyedMutex >,
						   IUnknownWrapper< IDXGIKeyedMutex > {
	// ----------------------------------------------------------
	// Constructors
	// ----------------------------------------------------------

	public KeyedMutex( ) {
		_comPtr = ComResources?.GetPointer< IDXGIKeyedMutex >( ) ;
		if( _comPtr is not null ) {
			_initOrAdd( _comPtr ) ;
		}
	}
	public KeyedMutex( nint address ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
		if( !address.IsValid( ) ) throw new NullReferenceException( $"The pointer for argument " +
																	$"\"{nameof(address)}\" is null (0x00000000)!" ) ;
#endif
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	public KeyedMutex( IUnknown comObject ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
			ArgumentNullException.ThrowIfNull( comObject, nameof(comObject) ) ;
#endif
		_comPtr = new( ( comObject as IDXGIKeyedMutex )! ) ;
		_initOrAdd( _comPtr ) ;
	}
	public KeyedMutex( IDXGIKeyedMutex comObject ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
			ArgumentNullException.ThrowIfNull( comObject, nameof(comObject) ) ;
#endif
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	public KeyedMutex( ComPtr< IDXGIKeyedMutex > comPtr ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
    		ArgumentNullException.ThrowIfNull( comPtr, nameof(comPtr) ) ;
#endif
    		this._comPtr = comPtr ;
    		_initOrAdd( _comPtr ) ;
	}
	
	// ----------------------------------------------------------
	// Properties
	// ----------------------------------------------------------
	ComPtr< IDXGIKeyedMutex >? _comPtr ;
	public new ComPtr< IDXGIKeyedMutex >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer<IDXGIKeyedMutex>( ) ;
	public override IDXGIKeyedMutex? ComObject => ComPointer?.Interface ;
	
	
	// ----------------------------------------------------------
	// Methods
	// ----------------------------------------------------------
	
	public void AcquireSync( ulong Key, uint dwMilliseconds ) =>
		ComObject?.AcquireSync( Key, dwMilliseconds ) ;
	
	public void ReleaseSync( ulong Key ) =>
		ComObject?.ReleaseSync( Key ) ;
	
	// ----------------------------------------------------------
	// IDisposable:
	// ----------------------------------------------------------
	
	public override bool Disposed => ComPointer?.Disposed ?? true ;
	 
	protected override async ValueTask DisposeUnmanaged( ) {
		if( ComPointer is not null && !ComPointer.Disposed ) {
			await ComPointer.DisposeAsync( ) ;
			_comPtr = null ;
		}
	}
	
	// ----------------------------------------------------------
	public new static Type ComType => typeof( IDXGIKeyedMutex ) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIKeyedMutex).GUID
															   .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	// ==========================================================
}