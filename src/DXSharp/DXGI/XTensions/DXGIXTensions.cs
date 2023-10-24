#region Using Directives
using System.Reflection ;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices ;

using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Dxgi.Common;

using DXSharp.Windows ;
using DXSharp.Windows.COM ;

using static DXSharp.DXSharpUtils ;
#endregion
namespace DXSharp.DXGI.XTensions ;


/// <summary>
/// Contains DXGI related extension methods
/// </summary>
public static partial class DXGIXTensions {
	internal static ScanlineOrder AsScanlineOrder( this DXGI_MODE_SCANLINE_ORDER slOrder ) => 
																		(ScanlineOrder)slOrder ;
	
	internal static DXGI_MODE_SCANLINE_ORDER AsDXGI_MODE_SCANLINE_ORDER( this ScanlineOrder slOrder ) => 
																					(DXGI_MODE_SCANLINE_ORDER)slOrder ;
	
	/// <summary>
	/// Indicates if this IUknown COM interface is alive
	/// </summary>
	/// <param name="comObj">This IUnknown instance</param>
	/// <returns>True if alive, otherwise false</returns>
	public static bool IsAlive( this IUnknownWrapper? comObj ) => comObj is { BasePointer: not 0 } ;
	
	internal static DXGI_FORMAT AsDXGI_FORMAT( this Format format ) => (DXGI_FORMAT)format ;
	internal static Format AsFormat( this DXGI_FORMAT format ) => (Format)format ;

	// -----------------------------------------------------------------------------------------------
	
	[MethodImpl(_MAXOPT_)] public static bool Is( this HResult hr, in HResult other ) => hr.Value == other.Value ;
	[MethodImpl(_MAXOPT_)] public static bool Is( this HResult hr, in HRESULT other ) => hr.Value == other.Value ;
	
	// -----------------------------------------------------------------------------------------------
	
	
	public static IAdapter? FindBestAdapter< A >( this IFactory factory )
													where A: class, IAdapter {
		ArgumentNullException.ThrowIfNull( factory, nameof(factory) ) ;
		const int MAX = IFactory.MAX_ADAPTER_COUNT ;
		
		//! TODO: Decide if we keep or ditch generic versions of these methods ...
		// Maybe we can keep generic version and route calls like this:
		if( factory is IFactory1 factory1 ) {
			if ( typeof(A).IsAssignableTo( typeof(IAdapter1 ) ) ) {
				return factory1.FindBestAdapter1( ) ;
			}
		}
		
		A? adapter = null ;
		AdapterDescription desc = default ;
		
		for ( int i = 0; i < MAX; ++i ) {
			var hr = factory.EnumAdapters< A >( (uint)i, out var _adapter ) ;
			if ( hr.Failed ) {
				if ( hr.Is( HResult.DXGI_ERROR_NOT_FOUND ) ) break ;
				throw new DirectXComError( hr, $"{nameof( DXGIXTensions )} :: " +
											   $"Error enumerating adapters! " +
											   $"HRESULT: 0x{hr.Value:X} ({hr.Value})" ) ;
			}
			
			if ( _adapter is null ) continue ;
			_adapter.GetDesc( out var _desc ) ;
			
			// Assign the "best" adapter by greatest amount of VRAM:
			if ( adapter is null ) {
				adapter = _adapter ;
				desc	= _desc ;
				continue ;
			}
			if ( _desc.DedicatedVideoMemory > desc.DedicatedVideoMemory ) {
				adapter = _adapter ;
				desc	= _desc ;
			}
		}
		
		return adapter ;
	}

	public static IAdapter1? FindBestAdapter1( this IFactory1 factory1 ) {
		ArgumentNullException.ThrowIfNull( factory1, nameof(factory1) ) ;
		const int MAX = IFactory.MAX_ADAPTER_COUNT ;
		
		IAdapter1? adapter = null ;
		AdapterDescription1 desc = default ;
		
		for ( int i = 0; i < MAX; ++i ) {
			var hr = factory1.EnumAdapters1< IAdapter1 >( (uint)i, out var _adapter ) ;
			if ( hr.Failed ) {
				if ( hr.Is( HResult.DXGI_ERROR_NOT_FOUND ) ) break ;
				throw new DirectXComError( hr, $"{nameof( DXGIXTensions )} :: " +
											   $"Error enumerating adapters! " +
											   $"HRESULT: 0x{hr.Value:X} ({hr.Value})" ) ;
			}
			
			if ( _adapter is null ) continue ;
			_adapter.GetDesc1( out var _desc ) ;
			
			// Assign the "best" adapter by greatest amount of VRAM:
			if ( adapter is null ) {
				adapter = _adapter ;
				desc    = _desc ;
				continue ;
			}
			if ( _desc.DedicatedVideoMemory > desc.DedicatedVideoMemory ) {
				adapter = _adapter ;
				desc    = _desc ;
			}
		}
		
		return adapter ;
	}
	
	/*
	public static (IAdapter? adapter, AdapterDescription desc) GetAllAdapters< F, A >( this F factory )
													where F: class, IFactory
													where A: class, IAdapter {
		ArgumentNullException.ThrowIfNull( factory, nameof(factory) ) ;
		const int MAX = IFactory.MAX_ADAPTER_COUNT ;
		
		Adapter? adapter = null ;
		AdapterDescription desc = default ;
		//List< (Adapter Adapter, AdapterDescription Description) > allAdapters = new( ) ;

		for ( int i = 0; i < MAX; ++i ) {
			var hr = factory.EnumAdapters< Adapter >( 0, out var _adapter ) ;
			if ( hr.Failed ) {
				if ( hr.Is( HResult.DXGI_ERROR_NOT_FOUND ) ) break ;
				throw new DirectXComError( hr, $"{nameof( DXGIXTensions )} :: " +
											   $"Error enumerating adapters! " +
											   $"HRESULT: 0x{hr.Value:X} ({hr.Value})" ) ;
			}
			
			if ( _adapter is null ) continue ;
			_adapter.GetDesc( out var _desc ) ;
			//allAdapters.Add( (_adapter, _desc) ) ;
			
			if ( adapter is null ) {
				adapter = _adapter ;
				desc	= _desc ;
				continue ;
			}
			
			// Does the adapter have more available VRAM?
			if ( _desc.DedicatedVideoMemory > desc.DedicatedVideoMemory ) {
				adapter = _adapter ;
				desc	= _desc ;
			}
		}
		
		return adapter ;
	}*/
} ;