#region Using Directives
using System.Runtime.CompilerServices ;
using System.Diagnostics.CodeAnalysis ;
using System.Runtime.InteropServices ;
using System.Runtime.Versioning ;

using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.Foundation ;
using Windows.Win32 ;

using DXSharp.DXGI.Debug ;
using DXSharp.Windows ;
using static DXSharp.DXSharpUtils ;
#endregion
namespace DXSharp.DXGI ;


/// <summary>
/// Valid values include the DXGI_CREATE_FACTORY_DEBUG (0x01) flag, and zero.
/// </summary>
/// <remarks>
/// <b><h3>Note:</h3></b> 
/// This flag will be set by the D3D runtime if:
/// The system creates an implicit factory during device creation.
/// The D3D11_CREATE_DEVICE_DEBUG flag is specified during device creation, 
/// for example using D3D11CreateDevice (or the swapchain method, or the 
/// Direct3D 10 equivalents).
/// </remarks>
public enum FactoryCreateFlags: uint {
	/// <summary>No flags</summary>
	None  = 0x0000,
	/// <summary>Enable debug layer</summary>
	Debug = 0x0001,
} ;



/// <summary>
/// Defines the DXGI-related functions of the Windows SDK
/// </summary>
/// <remarks>
/// See the documentation for the 
/// <a href="https://learn.microsoft.com/en-us/windows/win32/direct3ddxgi/d3d10-graphics-reference-dxgi-functions">DXGI functions</a> 
/// for a complete list with additional information
/// </remarks>
public static partial class DXGIFunctions {
	// ---------------------------------------------------------------------------------------------------
	//! PInvoke/External Functions:
	// ---------------------------------------------------------------------------------------------------
	//! For some reason, CsWin32 doesn't create this function in PInvoke class:
	[DllImport("dxgi.dll", EntryPoint = "CreateDXGIFactory", 
			   SetLastError = true, ExactSpelling = true)]
	static extern unsafe HResult DXGIGetDebugInterface(
			Guid*                                             riid,
			[MarshalAs( UnmanagedType.IUnknown )] out object? debugInterface // void** ppDebug
		) ;
	// ---------------------------------------------------------------------------------------------------

	
	// ---------------------------------------------------------------------------------------------------
	//! Factory creation functions:
	[SuppressMessage( "Interoperability", "CA1416:Validate platform compatibility" )] 
	static readonly Dictionary< Guid, Func< IDXGIFactory, IInstantiable > > _factoryCreators = new( ) {
		{ IFactory.IID, (f) => new Factory(f) },
		{ IFactory1.IID, (f) => new Factory1( (f as IDXGIFactory1)! ) },
		{ IFactory2.IID, (f) => new Factory2( (f as IDXGIFactory2)! ) },
		{ IFactory3.IID, (f) => new Factory3( (f as IDXGIFactory3)! ) },
		{ IFactory4.IID, (f) => new Factory4( (f as IDXGIFactory4)! ) },
		{ IFactory5.IID, (f) => new Factory5( (f as IDXGIFactory5)! ) },
		{ IFactory6.IID, (f) => new Factory6( (f as IDXGIFactory6)! ) },
		{ IFactory7.IID, (f) => new Factory7( (f as IDXGIFactory7)! ) },
	} ;
	// ---------------------------------------------------------------------------------------------------
	
	
	
	// ---------------------------------------------------------------------------------------------------
	//! DXGICreateFactoryX Functions/Overloads:
	// ---------------------------------------------------------------------------------------------------
	
	public static HResult CreateFactory< T >( out T? factory ) where T : IFactory, IInstantiable {
		var riid = T.Guid ;
		var hr = CreateFactory( riid, out var _f ) ;
		factory = (T?) _f ;
		return hr ;
	}

	public static HResult CreateFactory( in Guid riid, out IFactory? factory ) {
		HResult hr   = PInvoke.CreateDXGIFactory( in riid, out var _fact ) ;
		
		var dxgiFactory = _fact as IDXGIFactory
#if DEBUG || DEBUG_COM || DEV_BUILD
						  ?? throw new DirectXComError( $"The object returned from {nameof(CreateDXGIFactory)} is invalid!" )
#endif
		;
		
		#region Debug Checks
#if !STRIP_CHECKS
		if( hr.Failed ) {
#if DEBUG_COM || DEV_BUILD
			hr.SetAsLastErrorForThread( ) ;
			
			var hrB = hr.ThrowOnFailure( ) ;
			hrB.SetAsLastErrorForThread( ) ;
#endif
			factory = default ;
			return hr ;
		}
#endif
		
		//! Verify the type argument is a wrapper of `IDXGIFactory`:
#if !STRIP_CHECKS
		if( !_factoryCreators.ContainsKey(riid) ) {
#if DEBUG_COM
			throw new DirectXComError( $"No such interface is supported.", 
									   new( $"\"{nameof(riid)}\" is not a recognized " +
											$"{nameof(IDXGIFactory)} interface identifier.",
											HResult.E_NOINTERFACE ) ) ;
#else
			factory = default ;
			return HResult.E_NOINTERFACE ;
#endif
		}
#endif
		#endregion
		
		var _createWrapperFn = _factoryCreators[ riid ] ;
		
		factory = (IFactory) _createWrapperFn( dxgiFactory! ) ;
		return hr ;
	}
	
	public static HResult CreateFactory1< T >( out T? factory ) where T: IFactory1, IInstantiable {
		var riid  = T.Guid ;
		var hr = CreateFactory1( riid, out var _f1 ) ;
		factory = (T?) _f1 ;
		return hr ;
	}

	public static HResult CreateFactory1( in Guid riid, out IFactory1? factory ) {
		
		HResult hr   = PInvoke.CreateDXGIFactory1( in riid, out var _fact ) ;
		
		var dxgiFactory = _fact as IDXGIFactory1
#if DEBUG || DEBUG_COM || DEV_BUILD
						  ?? throw new DirectXComError( $"The object returned from {nameof(CreateDXGIFactory1)} is invalid!" )
#endif
		;
		
		#region Debug Checks
#if !STRIP_CHECKS
		if( hr.Failed ) {
#if DEBUG_COM || DEV_BUILD
			hr.SetAsLastErrorForThread( ) ;
			
			var hrB = hr.ThrowOnFailure( ) ;
			hrB.SetAsLastErrorForThread( ) ;
#endif
			factory = default ;
			return hr ;
		}
#endif
		 		
		//! Verify the type argument is a wrapper of `IDXGIFactory`:
#if !STRIP_CHECKS
		if( !_factoryCreators.ContainsKey(riid) ) {
#if DEBUG_COM
			throw new DirectXComError( $"No such interface is supported.",
									   new( $"\"{nameof(riid)}\" is not a recognized " +
											$"{nameof(IDXGIFactory)} interface identifier.",
											HResult.E_NOINTERFACE ) ) ;
#else
 			factory = default ;
			return HResult.E_NOINTERFACE ;
#endif
		}
#endif
		#endregion
		
		var _createWrapperFn = _factoryCreators[ riid ] ;
		
		factory = (IFactory1) _createWrapperFn( dxgiFactory! ) ;
		return hr ;
	}
	
	public static HResult CreateFactory2< T >( out T factory,
											   FactoryCreateFlags flags = FactoryCreateFlags.None )
																		where T: IFactory2, IInstantiable {
		var riid = T.Guid ;
		var hr = CreateFactory2( riid, out var _f2, flags ) ;
		factory = (T) _f2 ;
		return hr ;
	}

	public static HResult CreateFactory2( in Guid riid, out IFactory2 factory,
										  FactoryCreateFlags flags = FactoryCreateFlags.None ) {
		
#if DEBUG || DEBUG_COM || DEV_BUILD
		if ( !_factoryCreators.ContainsKey( riid ) )
			throw new ArgumentException( $"Unrecognized GUID value: " +
										 $"{riid.ToString()}", nameof( riid ) ) ;
#endif
		
		HResult hr = PInvoke.CreateDXGIFactory2( (uint)flags, riid, out var factoryObj ) ;
		hr.SetAsLastErrorForThread( ) ;

		// Convert RCW object to IDXGIFactory:
		var dxgiFactoryBase = factoryObj as IDXGIFactory2
#if DEBUG || DEBUG_COM || DEV_BUILD
							  ?? throw new DirectXComError( $"The object returned from {nameof(CreateFactory2)} is invalid!" )
#else
			!
#endif
			;
		
		factory = (IFactory2)_factoryCreators[ riid ]( dxgiFactoryBase ) ;
		return hr ;
	}

	
	// ---------------------------------------------------------------------------------------------------
	//! DXGI Utility Functions/Overloads:
	// ---------------------------------------------------------------------------------------------------
	
	[SupportedOSPlatform("windows10.0.17134")]
	public static void DeclareAdapterRemovalSupport( ) => PInvoke.DXGIDeclareAdapterRemovalSupport( ) ;
	
	[SupportedOSPlatform("windows10.0.17134")]
	public static void DisableVBlankVirtualization( ) => PInvoke.DXGIDeclareAdapterRemovalSupport( ) ;
	
	
	// ---------------------------------------------------------------------------------------------------
	//! DXGIGetDebugInterfaceX Functions/Overloads:
	// ---------------------------------------------------------------------------------------------------
	
	public static HResult GetDebugInterface( in Guid riid, out IDebug? debugInterface ) {
		unsafe {
			fixed ( Guid* _riid = &riid ) {
				var hr = DXGIGetDebugInterface( _riid, out var _dbgObj ) ;
				
				hr.SetAsLastErrorForThread( ) ;
				var _dbg = _dbgObj as IDXGIDebug ;

#if !STRIP_CHECKS
				hr.ThrowOnFailure( ) ;
				
				if ( _dbg is null ) {
					debugInterface = default ;
#if DEBUG || DEBUG_COM || DEV_BUILD
					throw new DirectXComError( $"The RCW object returned from " +
											   $"{nameof(DXGIGetDebugInterface)} is invalid!" ) ;
#endif
					return hr ;
				}
				
				// Check if the given GUID is a recognized interface identifier:
				if ( !IDebug._layerCreationFunctions.ContainsKey(riid)
					 && !IInfoQueue._infoQueueCreationFunctions.ContainsKey(riid) ) {
					debugInterface = default ;
#if DEBUG || DEBUG_COM || DEV_BUILD
					throw new DirectXComError( $"Unrecognized or unsupported {nameof(Guid)} " +
											   $"used as interface identifier (IID)!" ) ;
#else
					return ;
#endif
				}
#endif
				
				var _createFn = IDebug._layerCreationFunctions[ riid ] ;
				debugInterface = (IDebug) _createFn( _dbg ) ;
				
				return hr ;
			}
		}
	}
	
	/// <summary>
	/// Retrieves an interface that Windows Store apps use for debugging the Microsoft DirectX Graphics Infrastructure (DXGI).
	/// </summary>
	/// <param name="riid">
	/// The globally unique identifier (GUID) of the requested interface type, which can be the identifier for the
	/// IDXGIDebug, IDXGIDebug1, or IDXGIInfoQueue interfaces.
	/// </param>
	/// <param name="debugInterface"></param>
	public static HResult GetDebugInterface1( in Guid riid, out IDebug1? debugInterface ) {
		unsafe {
			fixed ( Guid* _riid = &riid ) {
				HResult hr = PInvoke.DXGIGetDebugInterface1( 0x00, _riid, out object _dbgObj ) ;
				hr.SetAsLastErrorForThread( ) ;
				var _dbg = _dbgObj as IDXGIDebug ;
#if !STRIP_CHECKS
				hr.ThrowOnFailure( ) ;
				
				if ( _dbg is null ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
					throw new DirectXComError( $"The RCW object returned from " +
											   $"{nameof(PInvoke.DXGIGetDebugInterface1)} is invalid!" ) ;
#else
					debugInterface = default ;
					return hr ;
#endif
				}
				
				// Check if the given GUID is a recognized interface identifier:
				if ( !IDebug._layerCreationFunctions.ContainsKey(riid)
					 && !IInfoQueue._infoQueueCreationFunctions.ContainsKey(riid) ) {
					debugInterface = default ;
#if DEBUG || DEBUG_COM || DEV_BUILD
					throw new DirectXComError( $"Unrecognized or unsupported {nameof(Guid)} " +
											   $"used as interface identifier (IID)!" ) ;
#else
 					return ;
#endif
				}
#endif
				 				
				var _createFn = IDebug._layerCreationFunctions[ riid ] ;
				debugInterface = (IDebug1) _createFn( _dbg ) ;
				
				return hr ;
			}
		}
	}
	
	// ---------------------------------------------------------------------------------------------------
	
	#region Old Methods (To be Deprecated & Removed)
	internal static unsafe TFactory? CreateDXGIFactory< TFactory >( Guid riid, out HRESULT hr ) {
		hr = PInvoke.CreateDXGIFactory( &riid, out object? factoryObj ) ;
		return hr.Succeeded ? (TFactory)factoryObj : default ;
	}

	internal static unsafe TFactory? CreateDXGIFactory1< TFactory >( Guid riid, out HRESULT hr ) 
																	 where TFactory: IDXGIFactory1 {
		hr = PInvoke.CreateDXGIFactory1( &riid, out object? factoryObj ) ;
		return hr.Succeeded ? (TFactory)factoryObj : default ;
	}
	
	internal static unsafe TFactory? CreateDXGIFactory2< TFactory >( FactoryCreateFlags Flags, Guid riid, out HRESULT hr ) 
																	 where TFactory: IDXGIFactory2 {
		hr = PInvoke.CreateDXGIFactory2( (uint)Flags, &riid, out object? factoryObj ) ;
		return hr.Succeeded ? (TFactory)factoryObj : default ;
	}
	
	
	/// <summary>
	/// Creates a DXGIFactoryX COM object you can use to generate other DXGI objects
	/// </summary>
	/// <typeparam name="T">Type of DXGIFactoryX</typeparam>
	/// <param name="hr">HRESULT to capture the result and indicate success/failure</param>
	/// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
	[MethodImpl(_MAXOPT_)] internal static T? CreateDXGIFactory< T >( out HRESULT hr ) where T: IDXGIFactory {
		return DXGIFunctions.CreateDXGIFactory< T >( typeof(T).GUID, out hr ) ;
	}

	/// <summary>
	/// Creates a DXGIFactoryX COM object you can use to generate other DXGI objects
	/// </summary>
	/// <typeparam name="T">Type of DXGIFactoryX</typeparam>
	/// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
	/// <exception cref="COMException">Thrown if the call fails and contains detailed error information</exception>
	[MethodImpl(_MAXOPT_)] internal static T? CreateDXGIFactory< T >( ) where T : IDXGIFactory {
		var factory = DXGIFunctions.CreateDXGIFactory< T >( typeof(T).GUID, out var hr ) ;
		_ = hr.ThrowOnFailure( ) ;
		return factory ;
	}
	
	/// <summary>
	/// Creates a DXGIFactoryX COM object you can use to generate other DXGI objects
	/// </summary>
	/// <typeparam name="TFactory">Type of DXGIFactoryX</typeparam>
	/// <param name="hr">HRESULT to capture the result and indicate success/failure</param>
	/// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
	[MethodImpl(_MAXOPT_)]
	internal static TFactory? CreateDXGIFactory1< TFactory >( out HRESULT hr ) where TFactory : IDXGIFactory1 {
		hr = PInvoke.CreateDXGIFactory1( typeof( TFactory ).GUID, out var factory ) ;
		_ = hr.ThrowOnFailure( ) ;
		return (TFactory)factory ;
	}
	
	/// <summary>
	/// Creates a DXGIFactoryX COM object you can use to generate other DXGI objects
	/// </summary>
	/// <typeparam name="T">Type of DXGIFactoryX</typeparam>
	/// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
	/// <exception cref="COMException">Thrown if the call fails and contains detailed error information</exception>
	[MethodImpl(_MAXOPT_)] internal static T? CreateDXGIFactory1< T >( ) where T : IDXGIFactory1 {
		var factory = CreateDXGIFactory1< T >( out var hr ) ;
		_ = hr.ThrowOnFailure( ) ;
		return factory;
	}
	
	/// <summary>
	/// Creates a DXGIFactoryX COM object you can use to generate other DXGI objects
	/// </summary>
	/// <typeparam name="T">Type of DXGIFactoryX</typeparam>
	/// <param name="Flags">Creation flags</param>
	/// <param name="hr">HRESULT to capture the result and indicate success/failure</param>
	/// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
	[MethodImpl(_MAXOPT_)] internal static T? CreateDXGIFactory2< T >( FactoryCreateFlags Flags, out HRESULT hr ) 
																							where T : IDXGIFactory2 {
		var factory2 = CreateDXGIFactory2< T >( Flags, typeof(T).GUID, out hr ) ;
		return (T)factory2! ;
	}

	/// <summary>
	/// Creates a DXGIFactoryX COM object you can use to generate other DXGI objects
	/// </summary>
	/// <typeparam name="T">Type of DXGIFactoryX</typeparam>
	/// <param name="Flags">Creation flags</param>
	/// <returns>A DXGIFactoryX object  of specified type T, or potentially null</returns>
	[MethodImpl(_MAXOPT_)] internal static T? CreateDXGIFactory2< T >( FactoryCreateFlags Flags ) where T : IDXGIFactory2 {
		var factory = CreateDXGIFactory2< T >( Flags, out var hr ) ;
		_ = hr.ThrowOnFailure( ) ;
		return factory ;
	}
	#endregion
	
	// ===================================================================================================
}

// ---------------------------------------------------------------------------------------------------