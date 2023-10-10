#region Using Directives
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI ;


/// <summary>Wrapper interface for the native IDXGIObject COM interface</summary>
public abstract class Object: IObject {
	internal Object( ) => 
		this.ComPointer = new( ) ;
	internal Object( IDXGIObject dxgiObject ) {
		ArgumentNullException.ThrowIfNull( dxgiObject, nameof(dxgiObject) ) ;
		this.ComPointer = new( COMUtility.GetAddressIUnknown(dxgiObject) ) ;
		if ( this.ComPointer.Interface is null ) throw new
			COMException( $"DXGI.Object.Create( {dxgiObject} ): " +
				$"Unable to initialize COM object reference from the given address!" ) ;
	}
	internal Object( nint iUnknownPtr ) {
		if ( iUnknownPtr is 0 ) throw new
			ArgumentNullException( nameof(iUnknownPtr),
		string.Format( LibResources.CantBeNull, nameof(iUnknownPtr) ) ) ;
		
		var dxgiObj = COMUtility.GetDXGIObject< IDXGIObject >( iUnknownPtr ) ;
		this.ComPointer = new( dxgiObj! ) ;
		
		if ( this.ComPointer.Interface is null ) throw new
			COMException( $"DXGI.Object.Create( {iUnknownPtr} ): " +
				$"Unable to initialize COM object reference from the given address!" ) ;
	}
	~Object( ) => Dispose( false ) ;
	
	public int RefCount { get ; protected set ; }
	public nint BasePointer { get ; internal init ; }
	public ComPtr< IDXGIObject >? ComPointer { get ; init ; }
	IDXGIObject? _interface => ComPointer?.Interface ;
	
	public uint AddRef( ) => this.ComPointer?.Interface?.AddRef( ) ?? 0U ;
	public uint Release( ) => this.ComPointer?.Interface?.Release( ) ?? 0U ;
	
	protected virtual void _throwIfDestroyed( ) {
		if ( ComPointer is null || ComPointer.Disposed || ComPointer.Interface is null || BasePointer is 0x00 )
			throw new
				ObjectDisposedException( nameof( Object ), $"{nameof( Object )} :: " + 
							$"Internal object \"{nameof( ComPointer )}\" is destroyed/null." ) ;
	}
	
	public void GetParent< T >( out T ppParent ) where T: IUnknownWrapper {
		_throwIfDestroyed( ) ;
		
		ppParent = default! ;
		unsafe {
			var riid = typeof( T ).GUID ;
			_interface?.GetParent( &riid, out IUnknown ppObj ) ;
		}
		ppParent = (T)ppParent ;
	}

	public void GetPrivateData< TData >( out uint pDataSize, nint pData ) where TData: unmanaged {
		_throwIfDestroyed( ) ;
		
		uint dataSize = 0U ;
		Guid name     = typeof( IDXGIObject ).GUID ;
		unsafe {
			_interface!.GetPrivateData( &name, ref dataSize, (void *)pData ) ;
		}
		pDataSize = dataSize ;
	}
	public void SetPrivateData< T >( uint DataSize, nint pData ) {
		_throwIfDestroyed( ) ;
		
		uint dataSize = 0U ;
		Guid name     = typeof(IDXGIObject).GUID ;
		unsafe {
			_interface!.SetPrivateData( &name, dataSize, (void *)pData ) ;
		}
	}
	
	public void SetPrivateDataInterface< T >( in T pUnknown )
												where T: IUnknownWrapper< IUnknown > {
#if DEBUG || DEV_BUILD
		ArgumentNullException.ThrowIfNull( pUnknown, nameof(pUnknown) ) ;
		ArgumentNullException.ThrowIfNull( pUnknown.ComObject, nameof(pUnknown.ComObject) ) ;
#endif
		
		_throwIfDestroyed( ) ;
		Guid name     = typeof(IDXGIObject).GUID ;
		unsafe {
			_interface!.SetPrivateDataInterface( &name, pUnknown.ComObject! ) ;
		}
	}
	
	
#region IDisposable Implementation
	protected bool disposedValue ;
	public bool Disposed => disposedValue;

	protected virtual void Dispose( bool disposing ) {
		if( !disposedValue ) {
			if( disposing ) {
				// TODO: dispose managed state (managed objects)
			}

			// TODO: free unmanaged resources (unmanaged objects) and override finalizer
			// TODO: set large fields to null
			if( _interface is not null ) {

				/* Unmerged change from project 'DXSharp (net7.0-windows10.0.22621.0)'
				Before:
								int refCount = Marshal.ReleaseComObject( m_dxgiObject );
				After:
								_ = Marshal.ReleaseComObject( m_dxgiObject );
				*/
				_ = Marshal.ReleaseComObject( _interface );
			}

			disposedValue = true;
		}
	}

	/// <summary>Diposes of this instance and frees its native COM resources</summary>
	public void Dispose( ) {
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose( disposing: true ) ;
		GC.SuppressFinalize( this ) ;
	}

	public ValueTask DisposeAsync( ) {
		Dispose( ) ;
		return ValueTask.CompletedTask ;
	}
#endregion
} ;
