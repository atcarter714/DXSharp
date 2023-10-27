using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows.COM ;

namespace DXSharp.DXGI ;

// ------------------------------------------------------------------------------------------------
// Version: IDXGIOutput
// ------------------------------------------------------------------------------------------------
// https://learn.microsoft.com/en-us/windows/desktop/api/DXGI/nn-dxgi-idxgioutput


public interface IOutput: IObject,
						  IComObjectRef< IDXGIOutput >,
						  IUnknownWrapper< IDXGIOutput >,
						  IInstantiable {
	// ---------------------------------------------------------------------------------
	new ComPtr< IDXGIOutput >? ComPointer { get ; }
	new IDXGIOutput? COMObject => ComPointer?.Interface ;
	IDXGIOutput? IComObjectRef< IDXGIOutput >.COMObject => COMObject ;

	ComPtr< IDXGIOutput >? IUnknownWrapper< IDXGIOutput >.ComPointer => 
		new( ComPointer?.InterfaceVPtr ?? 0 ) ;

	
	static Type IUnknownWrapper.ComType => typeof(IDXGIOutput) ;
	static Guid IUnknownWrapper.InterfaceGUID => typeof(IDXGIOutput).GUID ;

	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIOutput).GUID
															.ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}


	static IDXCOMObject IInstantiable.Instantiate( ) => new Output( ) ;
	static IDXCOMObject IInstantiable.Instantiate( nint pComObj ) => new Output( pComObj ) ;
	static IDXCOMObject IInstantiable.Instantiate< ICom >( ICom pComObj ) => 
		new Output( pComObj! ) ;
	// ==================================================================================
	
	
	
	void GetDescription( out OutputDescription pDescription ) ;
	
	void GetDisplayModeList( Format enumFormat,
							 uint flags,
							 out uint pNumModes,
							 out Span< ModeDescription > pDescription ) ;

	void FindClosestMatchingMode( in  ModeDescription pModeToMatch, 
								  out ModeDescription pClosestMatch,
								  IUnknownWrapper     pConcernedDevice ) ;

	void WaitForVBlank( ) ;
	
	void TakeOwnership( IUnknownWrapper pDevice, bool exclusive ) ;
	void ReleaseOwnership( ) ;
	
	void GetGammaControlCapabilities( out GammaControlCapabilities pGammaCaps ) ;
	void SetGammaControl( in GammaControl pGammaData ) ;
	void GetGammaControl( out GammaControl pGammaData ) ;
	
	void SetDisplaySurface<T>( T pScanoutSurface ) where T : class, ISurface ;
	void GetDisplaySurfaceData( ISurface pDestination ) ;
	
	void GetFrameStatistics( out FrameStatistics pStats ) ;
} ;


// ------------------------------------------------------------------------------------------------
// Version: IDXGIOutput1
// https://docs.microsoft.com/en-us/windows/win32/api/dxgi1_2/nn-dxgi1_2-idxgioutput1
// ------------------------------------------------------------------------------------------------

public interface IOutput1: IOutput,
						   IComObjectRef< IDXGIOutput1 >,
						   IUnknownWrapper< IDXGIOutput1 >,
						   IInstantiable {
	// ---------------------------------------------------------------------------------
	new ComPtr< IDXGIOutput1 >? ComPointer { get ; }
	new IDXGIOutput1? COMObject => ComPointer?.Interface ;
	IDXGIOutput1? IComObjectRef< IDXGIOutput1 >.COMObject => COMObject ;
	
	ComPtr< IDXGIOutput1 >? IUnknownWrapper< IDXGIOutput1 >.ComPointer => 
		new( ComPointer?.InterfaceVPtr ?? 0 ) ;
	
	static Type IUnknownWrapper.ComType => typeof(IDXGIOutput1) ;
	static Guid IUnknownWrapper.InterfaceGUID => typeof(IDXGIOutput1).GUID ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIOutput1).GUID
														   .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}


	static IDXCOMObject IInstantiable.Instantiate( ) => new Output1( ) ;
	static IDXCOMObject IInstantiable.Instantiate( nint pComObj ) => new Output1( pComObj ) ;
	static IDXCOMObject IInstantiable.Instantiate< ICom >( ICom pComObj ) => 
		new Output1( pComObj! ) ;
	// ==================================================================================
	
	void GetDisplayModeList1( Format enumFormat,
							  uint flags,
							  out uint pNumModes,
							  out Span< ModeDescription1 > pDescription ) ;
	
	void FindClosestMatchingMode1( in  ModeDescription1 pModeToMatch, 
								   out ModeDescription1 pClosestMatch,
								   IUnknownWrapper      pConcernedDevice ) ;
	
	void GetDisplaySurfaceData1( IResource pDestination ) ;
	
	void DuplicateOutput( IDevice pDevice, out IOutputDuplication? ppOutputDuplication ) ;
} ;



public interface IOutput2: IOutput1,
						   IComObjectRef< IDXGIOutput2 >,
						   IUnknownWrapper< IDXGIOutput2 >,
						   IInstantiable {
	// ---------------------------------------------------------------------------------
	new ComPtr< IDXGIOutput2 >? ComPointer { get ; }
	new IDXGIOutput2? COMObject => ComPointer?.Interface ;
	
	ComPtr< IDXGIOutput2 >? IUnknownWrapper< IDXGIOutput2 >.ComPointer => 
		new( ComPointer?.InterfaceVPtr ?? 0 ) ;
	
	IDXGIOutput1? IOutput1.COMObject => COMObject ;
	IDXGIOutput1? IComObjectRef< IDXGIOutput1 >.COMObject => COMObject ;
	IDXGIOutput2? IComObjectRef< IDXGIOutput2 >.COMObject => COMObject ;

	static Type IUnknownWrapper.ComType => typeof( IDXGIOutput2 ) ;
	static Guid IUnknownWrapper.InterfaceGUID => typeof( IDXGIOutput2 ).GUID ;

	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIOutput2 ).GUID
															  .ToByteArray( ) ;

			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference( data ) ) ;
		}
	}

}


public interface IOutput3: IOutput2,
						   IComObjectRef< IDXGIOutput3 >,
						   IUnknownWrapper< IDXGIOutput3 >,
						   IInstantiable
{
	// ---------------------------------------------------------------------------------
	new ComPtr< IDXGIOutput3 >? ComPointer { get ; }
	new IDXGIOutput3? COMObject => ComPointer?.Interface ;
	IDXGIOutput3? IComObjectRef< IDXGIOutput3 >.COMObject => COMObject ;
	
	ComPtr< IDXGIOutput2 >? IUnknownWrapper< IDXGIOutput2 >.ComPointer => 
		new( ComPointer?.InterfaceVPtr ?? 0 ) ;
	
	IDXGIOutput2? IOutput2.COMObject => COMObject ;
	IDXGIOutput2? IComObjectRef< IDXGIOutput2 >.COMObject => COMObject ;

	static Type IUnknownWrapper.ComType => typeof( IDXGIOutput3 ) ;
	static Guid IUnknownWrapper.InterfaceGUID => typeof( IDXGIOutput3 ).GUID ;

	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIOutput3 ).GUID
															  .ToByteArray( ) ;

			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference( data ) ) ;
		}
	}

}

