#region Using Directives
using System.Diagnostics.CodeAnalysis ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using System.Runtime.Versioning ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.Graphics.Dxgi.Common ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI ;


/// <summary>Proxy contract for the native <see cref="IDXGIOutput"/> COM interface.</summary>
[Wrapper(typeof(IDXGIOutput))]
internal class Output: Object,
					   IOutput,
					   IComObjectRef< IDXGIOutput >,
					   IUnknownWrapper< IDXGIOutput > {
	// ---------------------------------------------------------------------------------
	ComPtr< IDXGIOutput >? _comPtr ;
	public new ComPtr< IDXGIOutput >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer< IDXGIOutput >(  ) ;
	public override IDXGIOutput? COMObject => ComPointer?.Interface ;

	// ---------------------------------------------------------------------------------

	public Output( ) {
		_comPtr = ComResources?.GetPointer< IDXGIOutput >(  ) ;
	}
	public Output( nint ptr ) {
		_comPtr = new( ptr ) ;
		_initOrAdd( _comPtr ) ;
	}
	public Output( in IDXGIOutput dxgiObj ) {
		_comPtr = new( dxgiObj ) ;
		_initOrAdd( _comPtr ) ;
	}
	public Output( ComPtr<IDXGIOutput> otherPtr ) {
		_comPtr = otherPtr ;
		_initOrAdd( _comPtr ) ;
	}
	
	// ---------------------------------------------------------------------------------

	public void GetDescription( out OutputDescription pDescription ) {
		_ = COMObject ?? throw new NullReferenceException( $"{nameof(Output)} :: " +
														  $"The internal COM interface is destroyed/null." ) ;
		pDescription = new OutputDescription( ) ;
		unsafe {
			DXGI_OUTPUT_DESC result = default ;
			COMObject.GetDesc( &result ) ;
			pDescription = new OutputDescription( result ) ;
		}
	}

	public uint GetDisplayModeCount( Format enumFormat, uint flags ) {
		_ = COMObject ?? throw new NullReferenceException( $"{nameof(Output)} :: " +
														  $"The internal COM interface is destroyed/null." ) ;
		uint modeCount = 0U ;
		unsafe {
			COMObject.GetDisplayModeList( (DXGI_FORMAT)enumFormat,
										   flags, ref modeCount ) ;
		}
		return modeCount ;
	}
	
	public void GetDisplayModeList( Format enumFormat,
									uint flags, out uint pNumModes,
									out Span< ModeDescription > pDescription ) {
		_ = COMObject ?? throw new NullReferenceException( $"{nameof(Output)} :: " +
														  $"The internal COM interface is destroyed/null." ) ;
		
		pDescription = default ; pNumModes = 0U ; uint modeCount = 0U ;
		unsafe {
			// First, call the function just to get the count (no pointer for results):
			COMObject.GetDisplayModeList( (DXGI_FORMAT)enumFormat,
										   flags, ref modeCount ) ;
			pNumModes = modeCount ;
			if ( pNumModes is 0U ) return ;
			
			// Now, allocate the memory and call the function again:
			var _alloc = stackalloc DXGI_MODE_DESC[ (int)pNumModes ] ;
			
			// This time, we have a pointer telling it where to write the results:
			COMObject.GetDisplayModeList( (DXGI_FORMAT)enumFormat,
										   flags, ref pNumModes,
											_alloc ) ; // (ptr to stack allocation)
			
			// Initialize the Span (out) with the pointer and length:
			var descSpan = new Span< ModeDescription >( _alloc, (int)pNumModes ) ;
			pDescription = new ModeDescription[ pNumModes ] ;
			descSpan.CopyTo( pDescription ) ;
			
			// Copies the results into the managed array:
			/*pDescription = new ModeDescription[ pNumModes ] ;
			for ( int i = 0 ; i < pNumModes && i < pDescription.Length ; ++i )
				pDescription[ i ] = new( _alloc[ i ] ) ;*/
		}
	}

	public void FindClosestMatchingMode( in ModeDescription pModeToMatch, 
										 out ModeDescription pClosestMatch,
										 IUnknownWrapper pConcernedDevice ) {
		_ = COMObject ?? throw new NullReferenceException( $"{nameof(Output)} :: " +
														  $"The internal COM interface is destroyed/null." ) ;
		pClosestMatch = default ;
		unsafe {
			DXGI_MODE_DESC result = default, modeToMatch_ = pModeToMatch ;
			COMObject.FindClosestMatchingMode( &modeToMatch_, &result,
											   pConcernedDevice ) ;
			pClosestMatch = new( result ) ;
		}
	}

	public void WaitForVBlank( ) => COMObject!.WaitForVBlank( ) ;

	public void TakeOwnership( IUnknownWrapper pDevice, bool exclusive ) {
		if ( pDevice is null ) throw new ArgumentNullException( nameof(pDevice) ) ;
		_ = COMObject ?? throw new NullReferenceException( $"{nameof(Output)} :: " +
														  $"The internal COM interface is destroyed/null." ) ;
		COMObject.TakeOwnership( pDevice, exclusive ) ;
	}

	public void ReleaseOwnership( ) => COMObject!.ReleaseOwnership( ) ;

	public void GetGammaControlCapabilities( out GammaControlCapabilities pGammaCaps ) {
		_ = COMObject ?? throw new NullReferenceException( $"{nameof(Output)} :: " +
														  $"The internal COM interface is destroyed/null." ) ;
		pGammaCaps = default ;
		unsafe {
			DXGI_GAMMA_CONTROL_CAPABILITIES result = default ;
			COMObject.GetGammaControlCapabilities( &result ) ;
			pGammaCaps = new( result ) ;
		}
	}

	public void SetGammaControl( in GammaControl pGammaData ) {
		_ = COMObject ?? throw new NullReferenceException( $"{nameof(Output)} :: " +
														  $"The internal COM interface is destroyed/null." ) ;
		unsafe { fixed( GammaControl* pGamma = &pGammaData )
			COMObject.SetGammaControl( (DXGI_GAMMA_CONTROL *)pGamma ) ; }
	}

	public void GetGammaControl( out GammaControl pGammaData ) {
		_ = COMObject ?? throw new NullReferenceException( $"{nameof(Output)} :: " +
														  $"The internal COM interface is destroyed/null." ) ;
		pGammaData = default ;
		unsafe {
			DXGI_GAMMA_CONTROL result = default ;
			COMObject.GetGammaControl( &result ) ;
			pGammaData = new( result ) ;
		}
	}

	public void SetDisplaySurface<T>( T pScanoutSurface ) where T: class, ISurface {
		ArgumentNullException.ThrowIfNull( pScanoutSurface, nameof(pScanoutSurface) ) ;
		var output = COMObject ?? throw new NullReferenceException( $"{nameof(Output)} :: " +
														  $"The internal COM interface is destroyed/null." ) ;
		var surface = (IComObjectRef< IDXGISurface >)pScanoutSurface ;
		output.SetDisplaySurface( surface.COMObject ) ;
	}

	public void GetDisplaySurfaceData( ISurface pDestination ) {
		var output = COMObject ?? throw new NullReferenceException( $"{nameof(Output)} :: " +
																	$"The internal COM interface is destroyed/null." ) ;
		var dst = (IComObjectRef< IDXGISurface >)pDestination ;
		output.GetDisplaySurfaceData( dst.COMObject ) ;
	}
	
	public void GetFrameStatistics( out FrameStatistics pStats ) {
		_ = COMObject ?? throw new NullReferenceException( $"{nameof(Output)} :: " +
														  $"The internal COM interface is destroyed/null." ) ;
		pStats = default ;
		unsafe {
			pStats = default ;
			DXGI_FRAME_STATISTICS result = default ;
			var pResult = &result ;
			COMObject.GetFrameStatistics( pResult ) ;
			pStats = *( (FrameStatistics *)pResult ) ;
		}
	}
	
	
	public new static Type ComType => typeof( IDXGIOutput ) ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIOutput ).GUID
															   .ToByteArray( ) ;

			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference( data ) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new Output( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new Output( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Output( (IDXGIOutput4)pComObj! ) ;
	// ==================================================================================
} ;


[Wrapper(typeof(IDXGIOutput1))]
internal class Output1: Output, IOutput1,
						IComObjectRef< IDXGIOutput1 >,
						IUnknownWrapper< IDXGIOutput1 > {
	// ---------------------------------------------------------------------------------
	ComPtr< IDXGIOutput1 >? _comPtr ;
	public new ComPtr< IDXGIOutput1 >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IDXGIOutput1 >(  ) ;
	public override IDXGIOutput1? COMObject => ComPointer?.Interface ;
	
	// ---------------------------------------------------------------------------------
	
	internal Output1( ) {
		_comPtr = ComResources?.GetPointer< IDXGIOutput1 >(  ) ;
	}
	internal Output1( nint ptr ) {
		_comPtr = new( ptr ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Output1( in IDXGIOutput1 dxgiObj ) {
		_comPtr = new( dxgiObj ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Output1( ComPtr<IDXGIOutput1> otherPtr ) {
		_comPtr = otherPtr ;
		_initOrAdd( _comPtr ) ;
	}
	
	// ---------------------------------------------------------------------------------
	
	public void GetDisplayModeList1( Format enumFormat, uint flags, 
									 out uint pNumModes, 
									 out Span< ModeDescription1 > pDescription ) {
		
		pDescription = default ; pNumModes = 0U ; uint modeCount = 0U ;
		 
		unsafe {
			// First, call the function just to get the count (no pointer for results):
			COMObject!.GetDisplayModeList1( (DXGI_FORMAT)enumFormat,
											flags, ref modeCount ) ;
			pNumModes = modeCount ;
			if ( pNumModes is 0U ) return ;
			
			// Now, allocate the memory and call the function again:
			var _alloc = stackalloc DXGI_MODE_DESC1[ (int)pNumModes ] ;
			
			// This time, we have a pointer telling it where to write the results:
			COMObject!.GetDisplayModeList1( (DXGI_FORMAT)enumFormat,
											flags, ref pNumModes,
											_alloc ) ; // (ptr to stack allocation)
			
			// Initialize the Span (out) with the pointer and length:
			var descSpan = new Span< ModeDescription1 >( _alloc, (int)pNumModes ) ;
			pDescription = new ModeDescription1[ pNumModes ] ;
			descSpan.CopyTo( pDescription ) ;
		}
	}

	public void FindClosestMatchingMode1( in ModeDescription1 pModeToMatch, 
										  out ModeDescription1 pClosestMatch,
										  Direct3D12.IDevice pConcernedDevice ) {
		
		pClosestMatch = default ;
		var device = (IComObjectRef< ID3D12Device >)pConcernedDevice ;
		unsafe {
			DXGI_MODE_DESC1 result = default, modeToMatch_ = pModeToMatch ;
			COMObject!.FindClosestMatchingMode1( &modeToMatch_, &result, device.COMObject ) ;
			pClosestMatch = new( result ) ;
		}
	}

	public void GetDisplaySurfaceData1( IResource pDestination ) {
		var resource = (IComObjectRef< IDXGIResource >)pDestination ;
		COMObject!.GetDisplaySurfaceData1( resource.COMObject ) ;
	}

	public void DuplicateOutput( IDevice pDevice, out IOutputDuplication? ppOutputDuplication ) {
		
		ppOutputDuplication = null ;
		unsafe {
			COMObject!.DuplicateOutput( pDevice, out var dup ) ;
			IDXGIOutputDuplication dxgiDup = ( dup ) ;
			ppOutputDuplication = new OutputDuplication( dup ) ;
		}
	}

	// ---------------------------------------------------------------------------------
	public new static Type ComType => typeof( IDXGIOutput1 ) ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIOutput1 ).GUID
															   .ToByteArray( ) ;

			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	// ==================================================================================
} ;


[Wrapper( typeof( IDXGIOutput2 ) )]
internal class Output2: Output1, 
						IOutput2,
						IComObjectRef< IDXGIOutput2 >,
						IUnknownWrapper< IDXGIOutput2 > {
	// ---------------------------------------------------------------------------------
	ComPtr< IDXGIOutput2 >? _comPtr ;

	public new ComPtr< IDXGIOutput2 >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IDXGIOutput2 >( ) ;

	public override IDXGIOutput2? COMObject => ComPointer?.Interface ;

	// ---------------------------------------------------------------------------------

	internal Output2( ) {
		_comPtr = ComResources?.GetPointer< IDXGIOutput2 >( ) ;
	}
	internal Output2( nint ptr ) {
		_comPtr = new( ptr ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Output2( in IDXGIOutput2 dxgiObj ) {
		_comPtr = new( dxgiObj ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Output2( ComPtr< IDXGIOutput2 > otherPtr ) {
		_comPtr = otherPtr ;
		_initOrAdd( _comPtr ) ;
	}

	// ---------------------------------------------------------------------------------
	
	public bool SupportsOverlays( ) {
		var output = COMObject ?? throw new NullReferenceException( $"{nameof(Output2)} :: " +
														  $"The internal COM interface is destroyed/null." ) ;
		return output.SupportsOverlays( ) ;
	}
	
	// ---------------------------------------------------------------------------------
	
	public new static Type ComType => typeof( IDXGIOutput2 ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIOutput2 ).GUID
															  .ToByteArray( ) ;

			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	// ==================================================================================
}


[Wrapper( typeof( IDXGIOutput3 ) )]
internal class Output3: Output2,
						IOutput3,
						IComObjectRef< IDXGIOutput3 >,
						IUnknownWrapper< IDXGIOutput3 > {
	// ---------------------------------------------------------------------------------
	ComPtr< IDXGIOutput3 >? _comPtr ;

	public new ComPtr< IDXGIOutput3 >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IDXGIOutput3 >( ) ;

	public override IDXGIOutput3? COMObject => ComPointer?.Interface ;
	
	// ---------------------------------------------------------------------------------

	internal Output3( ) {
		_comPtr = ComResources?.GetPointer< IDXGIOutput3 >( ) ;
	}
	internal Output3( nint ptr ) {
		_comPtr = new( ptr ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Output3( in IDXGIOutput3 dxgiObj ) {
		_comPtr = new( dxgiObj ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Output3( ComPtr< IDXGIOutput3 > otherPtr ) {
		_comPtr = otherPtr ;
		_initOrAdd( _comPtr ) ;
	}

	// ---------------------------------------------------------------------------------
	
	public void CheckOverlaySupport( Format enumFormat, 
									 Direct3D12.IDevice pConcernedDevice, 
									 out OverlaySupportFlag pFlags ) {
		var output = COMObject ?? throw new NullReferenceException( $"{nameof(Output3)} :: " +
														  $"The internal COM interface is destroyed/null." ) ;
		var device = (IComObjectRef< ID3D12Device >)pConcernedDevice ;
		output.CheckOverlaySupport( (DXGI_FORMAT)enumFormat, device.COMObject, out uint _flags ) ;
		pFlags = (OverlaySupportFlag)_flags ;
	}

	// ---------------------------------------------------------------------------------
	public new static Type ComType => typeof( IDXGIOutput3 ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIOutput3 ).GUID
															  .ToByteArray( ) ;

			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	// ==================================================================================
} ;


[SupportedOSPlatform("windows10.0.10240")]
[Wrapper( typeof( IDXGIOutput4 ) )]
internal class Output4: Output3,
						IOutput4,
						IComObjectRef< IDXGIOutput4 >,
						IUnknownWrapper< IDXGIOutput4 > {
	// ---------------------------------------------------------------------------------
	ComPtr< IDXGIOutput4 >? _comPtr ;
	
	public new virtual ComPtr< IDXGIOutput4 >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IDXGIOutput4 >( ) ;
	
	public override IDXGIOutput4? COMObject => ComPointer?.Interface ;

	// ---------------------------------------------------------------------------------

	internal Output4( ) {
		_comPtr = ComResources?.GetPointer< IDXGIOutput4 >( ) ;
	}
	internal Output4( nint ptr ) {
		_comPtr = new( ptr ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Output4( in IDXGIOutput4 dxgiObj ) {
		_comPtr = new( dxgiObj ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Output4( ComPtr< IDXGIOutput4 > otherPtr ) {
		_comPtr = otherPtr ;
		_initOrAdd( _comPtr ) ;
	}
	
	// ---------------------------------------------------------------------------------
	
	public void CheckOverlayColorSpaceSupport( Format Format,
											   ColorSpaceType ColorSpace,
											   Direct3D12.IDevice pConcernedDevice,
											   out OverlayColorSpaceSupportFlag pFlags ) {
		var output = COMObject ?? throw new NullReferenceException( $"{nameof(Output4)} :: " +
														  $"The internal COM interface is destroyed/null." ) ;
		var device = (IComObjectRef< ID3D12Device >)pConcernedDevice ;
		output.CheckOverlayColorSpaceSupport( (DXGI_FORMAT)Format, 
											  (DXGI_COLOR_SPACE_TYPE)ColorSpace,
											  device.COMObject, 
											  out uint pflags ) ;
		pFlags = (OverlayColorSpaceSupportFlag)pflags ;
	}
	
	// ---------------------------------------------------------------------------------
	public new static Type ComType => typeof( IDXGIOutput4 ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIOutput4 ).GUID
															  .ToByteArray( ) ;

			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	// ==================================================================================
} ;

 
[SupportedOSPlatform("windows10.0.10240")]
[Wrapper( typeof( IDXGIOutput5 ) )]
internal class Output5: Output4,
						IOutput5,
						IComObjectRef< IDXGIOutput5 >,
						IUnknownWrapper< IDXGIOutput5 > {
	// ---------------------------------------------------------------------------------
	ComPtr< IDXGIOutput5 >? _comPtr ;
	public new ComPtr< IDXGIOutput5 >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IDXGIOutput5 >( ) ;

	public override IDXGIOutput5? COMObject => ComPointer?.Interface ;
	// ---------------------------------------------------------------------------------

	internal Output5( ) {
		_comPtr = ComResources?.GetPointer< IDXGIOutput5 >( ) ;
	}
	internal Output5( nint ptr ) {
		_comPtr = new( ptr ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Output5( in IDXGIOutput5 dxgiObj ) {
		_comPtr = new( dxgiObj ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Output5( ComPtr< IDXGIOutput5 > otherPtr ) {
		_comPtr = otherPtr ;
		_initOrAdd( _comPtr ) ;
	}
	
	public HResult DuplicateOutput1( Direct3D12.IDevice   pDevice,
								  uint                    Flags,
								  uint                    SupportedFormatsCount,
								  in  Span< Format >      pSupportedFormats,
								  out IOutputDuplication? ppOutputDuplication ) {
		ppOutputDuplication = default ;
		unsafe {
			var device = (IComObjectRef< ID3D12Device >)pDevice ;
			fixed( Format* pFormats = &pSupportedFormats[ 0 ] ) {
				var hr = (HResult)COMObject!.DuplicateOutput1( device.COMObject, Flags, SupportedFormatsCount,
											 (DXGI_FORMAT*)pFormats,
											 out var dup ) ;
				
				ppOutputDuplication = new OutputDuplication( dup ) ;
				return hr ;
			}
		}
	}
	
	// ---------------------------------------------------------------------------------
	public new static Type ComType => typeof( IDXGIOutput5 ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIOutput5 ).GUID
															  .ToByteArray( ) ;

			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	// ==================================================================================
} ; 


[SupportedOSPlatform("windows10.0.19041")]
[Wrapper( typeof( IDXGIOutput6 ) )]
internal class Output6: Output5,
						IOutput6,
						IComObjectRef< IDXGIOutput6 >,
						IUnknownWrapper< IDXGIOutput6 > {
	// ---------------------------------------------------------------------------------
	ComPtr< IDXGIOutput6 >? _comPtr ;

	public new ComPtr< IDXGIOutput6 >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IDXGIOutput6 >( ) ;

	public override IDXGIOutput6? COMObject => ComPointer?.Interface ;

	// ---------------------------------------------------------------------------------

	internal Output6( ) {
		_comPtr = ComResources?.GetPointer< IDXGIOutput6 >( ) ;
	}
	internal Output6( nint ptr ) {
		_comPtr = new( ptr ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Output6( in IDXGIOutput6 dxgiObj ) {
		_comPtr = new( dxgiObj ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Output6( ComPtr< IDXGIOutput6 > otherPtr ) {
		_comPtr = otherPtr ;
		_initOrAdd( _comPtr ) ;
	}

	// ---------------------------------------------------------------------------------
	public new static Type ComType => typeof( IDXGIOutput6 ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIOutput6 ).GUID
															  .ToByteArray( ) ;

			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	// ==================================================================================
} ;
