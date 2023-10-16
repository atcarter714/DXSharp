#region Using Directives
using System.Diagnostics.CodeAnalysis ;
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.Graphics.Dxgi.Common ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI ;


/// <summary>Proxy contract for the native <see cref="IDXGIOutput"/> COM interface.</summary>
public class Output: Object, IOutput {
	public static Type ComType => typeof( IDXGIOutput ) ;
	public static Guid InterfaceGUID => typeof( IDXGIOutput ).GUID ;
	

	public override ComPtr? ComPtrBase => ComPointer ;
	public new IDXGIOutput? COMObject => ComPointer?.Interface ;
	public new ComPtr< IDXGIOutput >? ComPointer { get ; protected set ; }
	
	
	#region Constructors
	internal Output( ) { }
	internal Output( nint ptr ) => ComPointer = new( ptr ) ;
	internal Output( in IDXGIOutput dxgiObj ) => ComPointer = new( dxgiObj ) ;
	internal Output( ComPtr<IDXGIOutput> otherPtr ) => ComPointer = otherPtr ;
	internal Output( in object? comObj ): this( COMUtility.GetIUnknownForObject(comObj) ) { }
	#endregion
	

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
		_ = COMObject ?? throw new NullReferenceException( $"{nameof(Output)} :: " +
														  $"The internal COM interface is destroyed/null." ) ;
		COMObject.SetDisplaySurface( pScanoutSurface.COMObject ) ;
	}

	public void GetDisplaySurfaceData( ISurface pDestination ) {
		_ = COMObject ?? throw new NullReferenceException( $"{nameof(Output)} :: " +
														  $"The internal COM interface is destroyed/null." ) ;
		unsafe {
			COMObject.GetDisplaySurfaceData( pDestination.COMObject ) ;
		}
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
} ;



public class Output1: Output, IOutput1 {
	public new static Type ComType => typeof( IDXGIOutput1 ) ;
	public new static Guid InterfaceGUID => typeof( IDXGIOutput1 ).GUID ;
	
	public new IDXGIOutput1? COMObject => ComPointer?.Interface ;
	public new ComPtr< IDXGIOutput1 >? ComPointer { get ; protected set ; }

	
	internal Output1( ) { }
	internal Output1( nint ptr ): base( ptr ) { }
	internal Output1( in IDXGIOutput1 dxgiObj ): base(dxgiObj) { }
	internal Output1( ComPtr<IDXGIOutput1> otherPtr ): this(otherPtr.Interface!) { }
	internal Output1( in object? comObj ): base( COMUtility.GetIUnknownForObject(comObj) ) { }

	
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
										  IUnknownWrapper pConcernedDevice ) {
		
		pClosestMatch = default ;
		unsafe {
			DXGI_MODE_DESC1 result = default, modeToMatch_ = pModeToMatch ;
			COMObject!.FindClosestMatchingMode1( &modeToMatch_, &result,
												 pConcernedDevice ) ;
			pClosestMatch = new( result ) ;
		}
	}

	public void GetDisplaySurfaceData1( IResource pDestination ) {
		
		unsafe {
			COMObject!.GetDisplaySurfaceData1( pDestination.COMObject ) ;
		}
	}

	public void DuplicateOutput( IDevice pDevice, out IOutputDuplication? ppOutputDuplication ) {
		
		ppOutputDuplication = null ;
		unsafe {
			COMObject!.DuplicateOutput( pDevice, out var dup ) ;
			IDXGIOutputDuplication dxgiDup = ( dup ) ;
			ppOutputDuplication = new OutputDuplication( dup ) ;
		}
	}

} ;