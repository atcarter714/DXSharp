#region Using Directives
using System.Collections.ObjectModel ;
using System.Diagnostics.CodeAnalysis ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using System.Runtime.Versioning ;

using Windows.Win32 ;
using Windows.Win32.Graphics.Dxgi ;

using DXSharp.Windows ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI.Debugging ;


// ---------------------------------------------------------------------------
// Proxy Interfaces ::
// ---------------------------------------------------------------------------

/// <summary>
/// This interface controls debug settings, and can
/// only be used if the debug layer is turned on.
/// </summary>
[SupportedOSPlatform("windows8.0")]
[ProxyFor(typeof(IDXGIDebug))]
public interface IDebug: IComIID, IInstantiable, 
						 IDisposable, IAsyncDisposable {
	// ---------------------------------------------------------------------------
	//! Creation Functions ::
	[SuppressMessage( "Interoperability", "CA1416:Validate platform compatibility" )] 
	internal static readonly ReadOnlyDictionary< Guid, Func<IDXGIDebug, IInstantiable> > _layerCreationFunctions =
		new( new Dictionary< Guid, Func<IDXGIDebug, IInstantiable> > {
			{ IDebug.IID, ( pComObj ) => new Debug( pComObj ) },
			{ IDebug1.IID, ( pComObj ) => new Debug1( (pComObj as IDXGIDebug1)! ) },
		} ) ;
	// ---------------------------------------------------------------------------
	
	
	/// <summary>Reports info about the lifetime of an object or objects.</summary>
	/// <param name="apiid">The globally unique identifier (GUID) of the object or objects to get info about.
	/// Use one of the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> GUIDs.</param>
	/// <param name="flags">
	/// A <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/ne-dxgidebug-dxgi_debug_rlo_flags">DXGI_DEBUG_RLO_FLAGS</a>-typed value
	/// that specifies the amount of info to report.
	/// </param>
	/// <returns>
	/// Returns <b>S_OK</b> if successful; an error code otherwise. For a list of error codes, see
	/// <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.
	/// </returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b> This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgidebug-reportliveobjects#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	HResult ReportLiveObjects( Guid apiid, DebugRLOFlags flags ) ;
	
	// ---------------------------------------------------------------------------
	public static Type ComType => typeof(IDXGIDebug) ;
	public static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid  {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIDebug).GUID
															.ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}

	static IInstantiable IInstantiable.Instantiate( ) => new Debug( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new Debug( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Debug( (pComObj as IDXGIDebug)! ) ;

	// ===========================================================================
} ;


/// <summary>
/// Controls debug settings for Microsoft DirectX Graphics Infrastructure (DXGI).
/// </summary>
/// <remarks>You can use the IDXGIDebug1 interface in Windows Store apps.</remarks>
[SupportedOSPlatform("windows8.1")]
[ProxyFor( typeof( IDXGIDebug1 ) )]
public interface IDebug1: IDebug {
	// ---------------------------------------------------------------------------

	/// <summary>Starts tracking leaks for the current thread.</summary>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgidebug1-enableleaktrackingforthread">
	/// Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	HResult EnableLeakTrackingForThread( ) ;

	/// <summary>Stops tracking leaks for the current thread.</summary>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgidebug1-disableleaktrackingforthread">
	/// Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void DisableLeakTrackingForThread( ) ;

	/// <summary>Gets a value indicating whether leak tracking is turned on for the current thread.</summary>
	/// <returns><b>TRUE</b> if leak tracking is turned on for the current thread; otherwise, <b>FALSE</b>.</returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgidebug1-isleaktrackingenabledforthread">
	/// Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	bool IsLeakTrackingEnabledForThread( ) ;
	
	// ---------------------------------------------------------------------------
	public new static Type ComType => typeof(IDXGIDebug1) ;
	public static new Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid  {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIDebug1).GUID
															 .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new Debug1( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new Debug1( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Debug1( (pComObj as IDXGIDebug1)! ) ;
	// ===========================================================================
} ;



// ---------------------------------------------------------------------------
// Wrapper Classes ::
// ---------------------------------------------------------------------------

[SupportedOSPlatform("windows8.0")]
[Wrapper( typeof( IDXGIDebug ) )]
internal class Debug: DisposableObject, 
					  IDebug,
					  IComObjectRef< IDXGIDebug >,
					  IUnknownWrapper< IDXGIDebug > {
	protected COMResource? ComResources { get ; set ; }
	protected internal void _initOrAdd< T >( ComPtr< T > ptr ) where T: IUnknown {
		if( ComResources is null ) ComResources = new( ptr ) ;
		else ComResources.AddPointer<T>( ptr ) ;
	}

	
	ComPtr< IDXGIDebug >? _comPtr ;
	public virtual ComPtr< IDXGIDebug >? ComPointer => _comPtr ??= ComResources?.GetPointer< IDXGIDebug >( ) ;
	public virtual ComPtr? ComPtrBase => this.ComPointer ;
	
	public virtual IDXGIDebug? ComObject => 
		ComPointer?.InterfaceObjectRef as IDXGIDebug ;

	
	public Debug( ) {
		_comPtr = ComResources?.GetPointer< IDXGIDebug >(  ) ;
	}
	public Debug( nint ptr ) {
		_comPtr = new( ptr ) ;
		_initOrAdd( _comPtr ) ;
	}
	public Debug( in IDXGIDebug dxgiObj ) {
		_comPtr = new( dxgiObj ) ;
		_initOrAdd( _comPtr ) ;
	}
	public Debug( ComPtr<IDXGIDebug> otherPtr ) {
		_comPtr = otherPtr ;
		_initOrAdd( _comPtr ) ;
	}
	~Debug( ) => Dispose( false ) ;
	
	// ---------------------------------------------------------------------------
	protected override ValueTask DisposeUnmanaged( ) {
		_comPtr?.Dispose( ) ;
		return ValueTask.CompletedTask ;
	}

	public override async ValueTask DisposeAsync( ) {
		await base.DisposeAsync( ) ;
		await Task.Run( Dispose ) ;
	}
	// ---------------------------------------------------------------------------

	public HResult ReportLiveObjects( Guid apiid, DebugRLOFlags flags ) {
		HResult hr = ComObject?.ReportLiveObjects( apiid, flags ) ?? HResult.E_FAIL ;
		return hr ;
	}
	
	
	// ---------------------------------------------------------------------------
	public static Type ComType => typeof(IDXGIDebug) ;
	public static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIDebug).GUID
														  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	// ===========================================================================
} ;



[SupportedOSPlatform("windows8.1")]
[Wrapper(typeof(IDXGIDebug1))]
internal class Debug1:  Debug, 
						IDebug1,
						IComObjectRef< IDXGIDebug1 >,
						IUnknownWrapper< IDXGIDebug1 > {
	protected ComPtr< IDXGIDebug1 >? _comptr ;
	public new virtual ComPtr< IDXGIDebug1 >? ComPointer => 
		_comptr ??= ComResources?.GetPointer< IDXGIDebug1 >( ) ;
	public override ComPtr? ComPtrBase => this.ComPointer ;

	public override IDXGIDebug1? ComObject => 
		ComPointer?.InterfaceObjectRef as IDXGIDebug1 ;

	// ---------------------------------------------------------------------------
	
	public Debug1( ) {
		_comptr = ComResources?.GetPointer< IDXGIDebug1 >(  ) ;
	}
	public Debug1( nint ptr ) {
		_comptr = new( ptr ) ;
		_initOrAdd( _comptr ) ;
	}
	public Debug1( in IDXGIDebug1 dxgiObj ) {
		_comptr = new( dxgiObj ) ;
		_initOrAdd( _comptr ) ;
	}
	public Debug1( ComPtr<IDXGIDebug1> otherPtr ) {
		_comptr = otherPtr ;
		_initOrAdd( _comptr ) ;
	}
	~Debug1( ) => Dispose( false ) ;
	
	// ---------------------------------------------------------------------------


	public HResult EnableLeakTrackingForThread( ) =>
		ComObject!.EnableLeakTrackingForThread( ) ;
	
	public void DisableLeakTrackingForThread( ) =>
		ComObject!.DisableLeakTrackingForThread( ) ;

	public bool IsLeakTrackingEnabledForThread( ) =>
		ComObject!.IsLeakTrackingEnabledForThread( ) ;
	
	// ---------------------------------------------------------------------------
	public new static Type ComType => typeof(IDXGIDebug1) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )] get {
			ReadOnlySpan< byte > data = typeof( IDXGIDebug1 ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	// ===========================================================================
} ;
