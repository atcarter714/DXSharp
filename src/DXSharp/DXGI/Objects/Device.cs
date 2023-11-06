#region Using Directives
using System.Diagnostics ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;
using DXSharp.Windows.Win32 ;
#endregion
namespace DXSharp.DXGI ;


/// <summary>Represents a DXGI Device object.</summary>
[DebuggerDisplay($"{nameof(Device)}: {nameof(ComPointer)} = ComPointer")]
[Wrapper(typeof(IDXGIDevice))]
internal class Device: Object,
					   IDevice,
					   IComObjectRef< IDXGIDevice >,
					   IUnknownWrapper< IDXGIDevice > {
	// ----------------------------------------------------------
	ComPtr< IDXGIDevice >? _comPtr ;
	public new ComPtr< IDXGIDevice >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IDXGIDevice >( ) ;
	public override IDXGIDevice? ComObject => ComPointer?.Interface ;
	// ----------------------------------------------------------

	public Device( ) {
		_comPtr = ComResources?.GetPointer< IDXGIDevice >( ) ;
	}
	public Device( nint ptr ) {
		_comPtr = new ComPtr< IDXGIDevice >( ptr ) ;
	}
	public Device( ComPtr< IDXGIDevice > ptr ) {
		_comPtr = ptr ;
	}
	public Device( IDXGIDevice ptr ) {
		_comPtr = new ComPtr< IDXGIDevice >( ptr ) ;
	}
	
	// ----------------------------------------------------------
	
	public HResult GetAdapter( out IAdapter adapter ) {
		var device = this.ComObject
#if DEBUG
					 ?? throw new NullReferenceException( $"{nameof( Device )} :: " +
														  $"The internal COM interface is destroyed/null." )
#endif
			;

		var hr = device!.GetAdapter( out var ppAdapter ) ;
		adapter = new Adapter( ppAdapter ) ;
		return hr ;
	}
	
	/* NOTE: -----------------------------------
		 The CreateSurface method creates a buffer to exchange data between one or more devices. 
		 It is used internally, and you should not directly call it.
		 The runtime automatically creates an IDXGISurface interface when it creates a Direct3D 
		 resource object that represents a surface. 
		 ---------------------------------------------------------------------------------------- */
	
	public void CreateSurface( in SurfaceDescription pDesc,
							   uint numSurfaces, uint usage,
							   ref SharedResource? pSharedResource,
							   out Span< Surface > ppSurface ) {
		ppSurface = default! ;
		if( numSurfaces <= 0U ) return ;
		
		var device = this.ComObject
#if DEBUG
					 ?? throw new NullReferenceException( $"{nameof( Device )} :: " +
														  $"The internal COM interface is destroyed/null." )
#endif
			;

		unsafe {
			DXGI_SHARED_RESOURCE* sharedResOut =  null ;
			DXGI_SHARED_RESOURCE localSharedRes = default ;
			if ( pSharedResource is not null ) {
				localSharedRes = pSharedResource.Value ;
				sharedResOut = &localSharedRes ;
			}
			
			fixed ( SurfaceDescription* pDescPtr = &pDesc ) {
				IDXGISurface[ ] surfaceData = new IDXGISurface[ numSurfaces ] ;
				device!.CreateSurface( (DXGI_SURFACE_DESC *) pDescPtr, 
									  numSurfaces,
									  (DXGI_USAGE)usage,
									  sharedResOut,
									  surfaceData ) ;
				
				pSharedResource = localSharedRes ;
				
				ppSurface = new Surface[ numSurfaces ] ;
				for ( int i = 0; i < numSurfaces; ++i ) {
					ppSurface[ i ] = new( surfaceData[i] ) ;
				}
			}
		}
	}

	
	public void QueryResourceResidency( in IResource?[ ] ppResources, 
										out Span< Residency > pResidencyStatus, 
										uint numResources ) {
		var device = this.ComObject
#if DEBUG
					 ?? throw new NullReferenceException( $"{nameof( Device )} :: " +
														  $"The internal COM interface is destroyed/null." )
#endif
			;

		unsafe {
			var array = new object[ ppResources.Length ] ;
			for ( var i = 0; i < ppResources.Length; ++i )
				array[ i ] = ( (IComObjectRef< IDXGIResource >?)ppResources[ i ] )!
								.ComObject! ;
			
			var _residencies = new Residency[ ppResources.Length ] ;
			fixed( Residency* pResidencies = _residencies )
				device!.QueryResourceResidency( array, (DXGI_RESIDENCY *)pResidencies, numResources ) ;
			
			pResidencyStatus = _residencies ;
		}
	}

	
	public void SetGPUThreadPriority( int priority ) {
		var device = this.ComObject
#if DEBUG
					 ?? throw new NullReferenceException( $"{nameof( Device )} :: " +
														  $"The internal COM interface is destroyed/null." )
#endif
			;
		
		device!.SetGPUThreadPriority( priority ) ;
	}

	
	public void GetGPUThreadPriority( out int pPriority ) {
		var device = this.ComObject
#if DEBUG
					 ?? throw new NullReferenceException( $"{nameof( Device )} :: " +
														  $"The internal COM interface is destroyed/null." )
#endif
			;
		device!.GetGPUThreadPriority( out pPriority ) ;
	}
	
	// ----------------------------------------------------------
	
	public new static Type ComType => typeof(IDXGIDevice) ;

	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIDevice).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}

	static IInstantiable IInstantiable.Instantiate( ) => new Device( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new Device( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom obj ) => 
		new Device( ( obj as IDXGIDevice )! ) ;
	
	// ============================================================
} ;


[Wrapper(typeof(IDXGIDevice1))]
internal class Device1: Device,
						IDevice1,
						IComObjectRef< IDXGIDevice1 >,
						IUnknownWrapper< IDXGIDevice1 > {
	// ----------------------------------------------------------
	ComPtr< IDXGIDevice1 >? _comPtr ;
	public new virtual ComPtr< IDXGIDevice1 >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IDXGIDevice1 >( ) ;
	public override IDXGIDevice1? ComObject => ComPointer?.Interface ;
	// ----------------------------------------------------------
	
	public Device1( ) {
		_comPtr = ComResources?.GetPointer< IDXGIDevice1 >( ) ;
	}
	public Device1( nint ptr ) {
		_comPtr = new ComPtr< IDXGIDevice1 >( ptr ) ;
	}
	public Device1( ComPtr< IDXGIDevice1 > ptr ) {
		_comPtr = ptr ;
	}
	public Device1( IDXGIDevice1 ptr ) {
		_comPtr = new ComPtr< IDXGIDevice1 >( ptr ) ;
	}
	
	// ----------------------------------------------------------

	public void SetMaximumFrameLatency( uint maxLatency ) =>
		this.ComObject!.SetMaximumFrameLatency( maxLatency ) ;
	
	public void GetMaximumFrameLatency( out uint pMaxLatency ) =>
		this.ComObject!.GetMaximumFrameLatency( out pMaxLatency ) ;
	
	// ----------------------------------------------------------
	
	public new static Type ComType => typeof(IDXGIDevice1) ;
	 
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIDevice1).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	// ============================================================
} ;


[Wrapper(typeof(IDXGIDevice2))]
internal class Device2: Device1,
						IDevice2,
						IComObjectRef< IDXGIDevice2 >,
						IUnknownWrapper< IDXGIDevice2 > {
	// ----------------------------------------------------------
	ComPtr< IDXGIDevice2 >? _comPtr ;
	public new virtual ComPtr< IDXGIDevice2 >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IDXGIDevice2 >( ) ;
	public override IDXGIDevice2? ComObject => ComPointer?.Interface ;
	// ----------------------------------------------------------
	
	public Device2( ) {
		_comPtr = ComResources?.GetPointer< IDXGIDevice2 >( ) ;
	}
	public Device2( nint ptr ) {
		_comPtr = new ComPtr< IDXGIDevice2 >( ptr ) ;
	}
	public Device2( ComPtr< IDXGIDevice2 > ptr ) {
		_comPtr = ptr ;
	}
	public Device2( IDXGIDevice2 ptr ) {
		_comPtr = new ComPtr< IDXGIDevice2 >( ptr ) ;
	}
	
	// ----------------------------------------------------------

	public void OfferResources( uint NumResources, IResource[ ] ppResources, OfferResourcePriority Priority ) {
		var device = this.ComObject
#if DEBUG
					 ?? throw new NullReferenceException( $"{nameof( Device )} :: " +
														  $"The internal COM interface is destroyed/null." )
#endif
		 			;

		var array = new IDXGIResource[ NumResources ] ;
		for ( var i = 0; i < NumResources; ++i )
			array[ i ] = ( (IComObjectRef< IDXGIResource >)ppResources[ i ] )
								.ComObject! ;
		
		device!.OfferResources( NumResources, array, (DXGI_OFFER_RESOURCE_PRIORITY)Priority ) ;
	}

	public void ReclaimResources( uint NumResources, IResource[ ] ppResources, out Span< BOOL > pDiscarded ) {
		var device = this.ComObject
#if DEBUG
					 ?? throw new NullReferenceException( $"{nameof( Device )} :: " +
														  $"The internal COM interface is destroyed/null." )
#endif
		 			;

		var array = new IDXGIResource[ NumResources ] ;
		for ( var i = 0; i < NumResources; ++i )
			array[ i ] = ( (IComObjectRef< IDXGIResource >)ppResources[ i ] )
								.ComObject! ;
		
		unsafe {
			var discarded = new BOOL[ (int)NumResources ] ;
			fixed( BOOL* stackPtr = discarded )
				device!.ReclaimResources( NumResources, array, stackPtr ) ;
			
			pDiscarded = discarded ;
		}
	}

	public void EnqueueSetEvent( Win32Handle hEvent ) {
		var device = this.ComObject
#if DEBUG
					 ?? throw new NullReferenceException( $"{nameof( Device )} :: " +
														  $"The internal COM interface is destroyed/null." )
#endif
		 			;

		device!.EnqueueSetEvent( hEvent ) ;
	}
	
	// ----------------------------------------------------------
	public new static Type ComType => typeof(IDXGIDevice2) ;
	 
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIDevice2).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	// ============================================================
} ;


[Wrapper(typeof(IDXGIDevice3))]
internal class Device3: Device2,
						IDevice3,
						IComObjectRef< IDXGIDevice3 >,
						IUnknownWrapper< IDXGIDevice3 > {
	// ----------------------------------------------------------
	ComPtr< IDXGIDevice3 >? _comPtr ;
	public new virtual ComPtr< IDXGIDevice3 >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IDXGIDevice3 >( ) ;
	public override IDXGIDevice3? ComObject => ComPointer?.Interface ;
	// ----------------------------------------------------------
	
	public Device3( ) {
		_comPtr = ComResources?.GetPointer< IDXGIDevice3 >( ) ;
	}
	public Device3( nint ptr ) {
		_comPtr = new ComPtr< IDXGIDevice3 >( ptr ) ;
	}
	public Device3( ComPtr< IDXGIDevice3 > ptr ) {
		_comPtr = ptr ;
	}
	public Device3( IDXGIDevice3 ptr ) {
		_comPtr = new ComPtr< IDXGIDevice3 >( ptr ) ;
	}
	
	// ----------------------------------------------------------
	
	public void Trim( ) {
		var device = this.ComObject
#if DEBUG
					 ?? throw new NullReferenceException( $"{nameof( Device )} :: " +
														  $"The internal COM interface is destroyed/null." )
#else
			!
#endif
			;

		device.Trim( ) ;
	}

	// ----------------------------------------------------------

	public new static Type ComType => typeof(IDXGIDevice3) ;
	 
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIDevice3).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	// ============================================================
} ;


[Wrapper(typeof(IDXGIDevice4))]
internal class Device4: Device3,
						IDevice4,
						IComObjectRef< IDXGIDevice4 >,
						IUnknownWrapper< IDXGIDevice4 > {
	// ----------------------------------------------------------
	ComPtr< IDXGIDevice4 >? _comPtr ;
	public new virtual ComPtr< IDXGIDevice4 >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IDXGIDevice4 >( ) ;
	public override IDXGIDevice4? ComObject => ComPointer?.Interface ;
	// ----------------------------------------------------------
	
	public Device4( ) {
		_comPtr = ComResources?.GetPointer< IDXGIDevice4 >( ) ;
	}
	public Device4( nint ptr ) {
		_comPtr = new ComPtr< IDXGIDevice4 >( ptr ) ;
	}
	public Device4( ComPtr< IDXGIDevice4 > ptr ) {
		_comPtr = ptr ;
	}
	public Device4( IDXGIDevice4 ptr ) {
		_comPtr = new ComPtr< IDXGIDevice4 >( ptr ) ;
	}
	
	// ----------------------------------------------------------

	public void OfferResources1( uint                  NumResources,
								 IResource[ ]          ppResources,
								 OfferResourcePriority Priority = OfferResourcePriority.Normal, 
								 OfferResourceFlags    Flags    = OfferResourceFlags.AllowDecommit ) {
		var device = this.ComObject
#if DEBUG
					 ?? throw new NullReferenceException( $"{nameof( Device )} :: " +
														  $"The internal COM interface is destroyed/null." )
#endif
		 					 			;
		
		var array = new IDXGIResource[ NumResources ] ;
		for ( var i = 0; i < NumResources; ++i )
			array[ i ] = ( (IComObjectRef< IDXGIResource >)ppResources[ i ] )
								.ComObject! ;
		
		device!.OfferResources1( NumResources, array, (DXGI_OFFER_RESOURCE_PRIORITY)Priority, (uint)Flags ) ;
	}

	public void ReclaimResources1( uint NumResources, 
								   IResource[ ] ppResources, 
								   out Span< ReclaimResourceResults > pResults ) {
		var device = this.ComObject
#if DEBUG
					 ?? throw new NullReferenceException( $"{nameof( Device )} :: " +
														  $"The internal COM interface is destroyed/null." )
#endif
					 			;
		
		var array = new IDXGIResource[ NumResources ] ;
		for ( var i = 0; i < NumResources; ++i )
			array[ i ] = ( (IComObjectRef< IDXGIResource >)ppResources[ i ] )
								.ComObject! ;
		
		unsafe {
			var reclaimResults = new ReclaimResourceResults[ (int)NumResources ] ;
			fixed ( ReclaimResourceResults* stackPtr = reclaimResults ) {
				device!.ReclaimResources1( NumResources, array, (DXGI_RECLAIM_RESOURCE_RESULTS *)stackPtr ) ;
			}
			
			pResults = reclaimResults ;
		}
	}

	// ----------------------------------------------------------
	
	public new static Type ComType => typeof(IDXGIDevice4) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIDevice4).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	// ============================================================
} ;