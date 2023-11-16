using DXSharp ;
using DXSharp.DXGI ;
using DXSharp.Windows ;

namespace DXSharp.Framework.XTensions.DXGI ;

public static class FactoryXTensions
{
	const uint MAX_ADAPTERS = 0x10 ;

	public static List< IAdapter > GetAllInstalledAdapters( this IFactory factory ) {
		List< IAdapter > _availableAdapters = new( 2 ) ;
		for ( uint ordinal = 0; ordinal < MAX_ADAPTERS; ++ordinal ) {
			var hr = factory.EnumAdapters( ordinal, out var _adapter0 ) ;
			if ( hr.Failed || _adapter0 is null ) {
				if ( hr == HResult.DXGI_ERROR_NOT_FOUND ) {
					break ; //! last adapter reached ...
				}

				throw new DirectXComError( hr, "Failed to enumerate adapters" ) ;
			}

			_availableAdapters.Add( _adapter0 ) ;
		}

		return _availableAdapters ;
	}
	
	public static List< IAdapter1 > GetAllInstalledAdapters( this IFactory1 factory ) {
		List< IAdapter1 > _availableAdapters = new( 2 ) ;
		for ( uint ordinal = 0; ordinal < MAX_ADAPTERS; ++ordinal ) {
			var hr = factory.EnumAdapters1( ordinal, out var _adapter1 ) ;
			if ( hr.Failed || _adapter1 is null ) {
				if ( hr == HResult.DXGI_ERROR_NOT_FOUND ) {
					break ; //! last adapter reached ...
				}

				throw new DirectXComError( hr, "Failed to enumerate adapters" ) ;
			}

			_availableAdapters.Add( _adapter1 ) ;
		}

		return _availableAdapters ;
	}
	
	
	public static List< A > GetAllInstalledAdapters< A >( this IFactory factory ) where A: IAdapter1 {
		List< A > _availableAdapters = new( 2 ) ;

		if ( factory is IFactory6 factory6 ) {
			for ( uint ordinal = 0; ordinal < MAX_ADAPTERS; ++ordinal ) {
				var hr = factory6.EnumAdapterByGPUPreference< A >( ordinal, GPUPreference.Unspecified,
																			A.Guid, out var _adapterX ) ;
				if ( hr.Failed || _adapterX is null ) {
					if ( hr == HResult.DXGI_ERROR_NOT_FOUND ) {
						break ; //! last adapter reached ...
					}

					throw new DirectXComError( hr, "Failed to enumerate adapters" ) ;
				}
				
				_availableAdapters.Add( _adapterX ) ;
			}

			return _availableAdapters ;
		}
		
		if ( factory is IFactory1 factory1 ) {
			for ( uint ordinal = 0; ordinal < MAX_ADAPTERS; ++ordinal ) {
				var hr = factory1.EnumAdapters1( ordinal, out var _adapter1 ) ;
				if ( hr.Failed || _adapter1 is null ) {
					if ( hr == HResult.DXGI_ERROR_NOT_FOUND ) {
						break ; //! last adapter reached ...
					}

					throw new DirectXComError( hr, "Failed to enumerate adapters" ) ;
				}
				
				if ( typeof(A) == typeof(IAdapter1) ) {
					_availableAdapters.Add( (A)_adapter1 ) ;
					continue ;
				}
				
				//! Query for the right adapter type (if necessary):
				var hr2 = _adapter1.QueryInterface< A >( out var _adapterX ) ;
				if ( hr2.Failed || _adapterX is null ) {
					try {
						_adapter1.Release( ) ;
						if ( _adapter1 is IDisposable disposable )
							disposable.Dispose( ) ;
					}
					finally {
						throw new DirectXComError( hr2, "Failed to query adapter" ) ;
					}
				}
				
				_availableAdapters.Add( _adapterX ) ;
				_adapter1.Release( ) ;
				_adapter1.Dispose( ) ;
			}

			return _availableAdapters ;
		}

		if ( factory is { } factory0 ) {
			for ( uint ordinal = 0; ordinal < MAX_ADAPTERS; ++ordinal ) {
				var hr = factory0.EnumAdapters( ordinal, out var _adapter0 ) ;
				if ( hr.Failed || _adapter0 is null ) {
					if ( hr == HResult.DXGI_ERROR_NOT_FOUND ) {
						break ; //! last adapter reached ...
					}
					
					throw new DirectXComError( hr, "Failed to enumerate adapters" ) ;
				}
				
				if ( typeof(A) == typeof(IAdapter) ) {
					_availableAdapters.Add( (A)_adapter0 ) ;
					continue ;
				}
				
				// Query for the right adapter type (if requested):
				var hr2 = _adapter0.QueryInterface< A >( out var _adapterX ) ;
				if ( hr2.Failed || _adapterX is null ) {
					try {
						_adapter0.Release( ) ;
						if ( _adapter0 is IDisposable disposable )
							disposable.Dispose( ) ;
					}
					finally {
						throw new DirectXComError( hr2, $"Failed to query adapter interface: " +
														$"\"{typeof(A).Name}\" (IID: {A.Guid})!" ) ;
					}
				}
				
				_availableAdapters.Add( _adapterX ) ;
				_adapter0.Release( ) ;
				_adapter0.Dispose( ) ;
			}

			return _availableAdapters ;
		}

		throw new NotSupportedException( $"The factory interface ({factory.GetType( ).Name}) " +
										 $"does not support an adapter interface for {typeof( A ).Name} (IID: {A.Guid})" ) ;
	}
	
	
	public static List< A > GetAllInstalledAdapterExcluding< A >( this IFactory1 factory, uint excludeFlags =
																	  (uint)( AdapterFlag3.Software |
																			  AdapterFlag3.Remote ) )
		where A: IAdapter1 {
		List< A > allAdapters = new( 2 ) ;
		
		if( factory is IFactory6 factory6 ) {
			for ( uint ordinal = 0; ordinal < MAX_ADAPTERS; ordinal++ ) {
				// Try to get the next adapter:
				var hr = factory6.EnumAdapterByGPUPreference( ordinal, GPUPreference.HighPerformance,
																		A.Guid, out A? _nextAdapterX ) ;
				if ( hr.Failed || _nextAdapterX is null ) {
					if ( hr == HResult.DXGI_ERROR_NOT_FOUND ) {
						break ; //! last adapter reached ...
					}
					
					throw new DirectXComError( hr, "Failed to enumerate adapters" ) ;
				}

				_resolveAdapterRequirements( _nextAdapterX ) ;
			}
			
			return allAdapters ;
		}
		
		if( factory is { } factory1 ) {
			for ( uint ordinal = 0; ordinal < MAX_ADAPTERS; ordinal++ ) {
				// Try to get the next adapter:
				var hr = factory1.EnumAdapters1( ordinal, out IAdapter1? _nextAdapter1 ) ;
				if ( hr.Failed || _nextAdapter1 is null ) {
					if ( hr == HResult.DXGI_ERROR_NOT_FOUND ) {
						break ; //! last adapter reached ...
					}
					
					throw new DirectXComError( hr, "Failed to enumerate adapters" ) ;
				}

				_resolveAdapterRequirements( _nextAdapter1 ) ;
			}
			
			return allAdapters ;
		}
		
		throw new NotSupportedException( $"The factory interface ({factory.GetType( ).Name}) " +
										 $"does not support an adapter interface for {typeof( A ).Name} (IID: {A.Guid})" ) ;
		
		void _resolveAdapterRequirements( IAdapter _nextAdapterX ) {
			// Get the adapter's description:
			if( _nextAdapterX is IAdapter4 _nextAdapter3 ) {
				_nextAdapter3.GetDesc3( out var desc3 ) ;
				if ( desc3.Flags.HasAnyBit( (AdapterFlag3)excludeFlags ) ) {
					_nextAdapterX.Release( ) ;
					_nextAdapterX.Dispose( ) ;
					return ;
				}
				
				allAdapters.Add( (A)_nextAdapter3 ) ;
				return ;
			}
				
			if( _nextAdapterX is IAdapter1 _nextAdapter1 ) {
				_nextAdapter1.GetDesc1( out var desc1 ) ;
				if ( desc1.Flags.HasAnyBit( (AdapterFlag)excludeFlags ) ) {
					_nextAdapterX.Release( ) ;
					_nextAdapterX.Dispose( ) ;
					return ;
				}
					
				allAdapters.Add( (A)_nextAdapterX ) ;
				return ;
			}
			
			//! Query for the right adapter type (if necessary):
			var hr2 = _nextAdapterX.QueryInterface( out IAdapter1? _adapter1Q ) ;
			if ( hr2.Failed || _adapter1Q is null ) {
				try {
					_nextAdapterX.Release( ) ;
					if ( _nextAdapterX is IDisposable disposable )
						disposable.Dispose( ) ;
				}
				finally { throw new DirectXComError( hr2, "Failed to query adapter interface: " +
														  $"\"{typeof(A).Name}\" (IID: {A.Guid})" ) ; }
			}
			
			_adapter1Q.GetDesc1( out var _desc1 ) ;
			if ( _desc1.Flags.HasAnyBit( (AdapterFlag)excludeFlags ) ) {
				_adapter1Q.Release( ) ;
				_adapter1Q.Dispose( ) ;
				_nextAdapterX.Release( ) ;
				_nextAdapterX.Dispose( ) ;
				return ;
			}
				
			allAdapters.Add( (A)_adapter1Q ) ;
			_nextAdapterX.Release( ) ;
			_nextAdapterX.Dispose( ) ;
		}
	}

	public static List< A > GetAllInstalledAdapterExcluding< A >( this IFactory1 factory, AdapterFlag excludeFlags =
																	  ( AdapterFlag.Software | AdapterFlag.Remote ) ) where A: IAdapter1 {
		return GetAllInstalledAdapterExcluding< A >( factory, (uint)excludeFlags ) ;
	}
	public static List< A > GetAllInstalledAdapterExcluding< A >( this IFactory1 factory, AdapterFlag3 excludeFlags =
																	  ( AdapterFlag3.Software | AdapterFlag3.Remote ) ) where A: IAdapter1 {
		return GetAllInstalledAdapterExcluding< A >( factory, (uint)excludeFlags ) ;
	}

} ;