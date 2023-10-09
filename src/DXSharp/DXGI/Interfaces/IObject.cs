#region Using Directives
#pragma warning disable CS1591

using DXSharp.Windows.COM ;
using Windows.Win32.Graphics.Dxgi ;
using System.Diagnostics.CodeAnalysis ;
using Windows.Win32.Foundation ;
using DXSharp.Windows ;
using global::System.Runtime.InteropServices ;
using Type = ABI.System.Type ;

#endregion

namespace DXSharp.DXGI ;


/// <summary>Wrapper interface for the native IDXGIObject COM interface</summary>
public interface IObject: IUnknownWrapper< IObject, IDXGIObject > {
	
	/// <summary>
	/// Sets application-defined data to the object and associates that data with a GUID.
	/// </summary>
	/// <param name="DataSize">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The size of the object's data.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-setprivatedata#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pData">
	/// <para>Type: <b>const void*</b> A pointer to the object's data.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-setprivatedata#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns one of the <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a> values.</para>
	/// </returns>
	/// <typeparam name="T">
	/// <para>Type: <b><a href="https://docs.microsoft.com/openspecs/windows_protocols/ms-oaut/6e7d7108-c213-40bc-8294-ac13fe68fd50">REFGUID</a></b> A GUID that identifies the data.Use this GUID in a call to<a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiobject-getprivatedata"> GetPrivateData</a> to get the data.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-setprivatedata#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </typeparam>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-setprivatedata">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	//unsafe void SetPrivateData( Guid* Name, uint DataSize, void* pData );
	void SetPrivateData< T >( uint DataSize, nint pData ) ;

	/// <summary>Set an interface in the object's private data.</summary>
	/// <param name="pUnknown">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/unknwn/nn-unknwn-iunknown">IUnknown</a>*</b> The interface to set.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-setprivatedatainterface#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns one of the following <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-setprivatedatainterface">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	//unsafe void SetPrivateDataInterface( Guid* Name, [MarshalAs(UnmanagedType.IUnknown)] object pUnknown );
	void SetPrivateDataInterface< T >( in T pUnknown )
		where T: IUnknownWrapper< IUnknown > ;
	
	/// <summary>Get a pointer to the object's data.</summary>
	/// <param name="pDataSize">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a>*</b> The size of the data.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-getprivatedata#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pData">
	/// <para>Type: <b>void*</b> Pointer to the data.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-getprivatedata#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns one of the following <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-getprivatedata">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	//unsafe void GetPrivateData( global::System.Guid* Name, uint* pDataSize, void* pData );
	void GetPrivateData< TData >( out uint pDataSize, nint pData ) where TData: unmanaged ;

	/// <summary>Gets the parent of the object.</summary>
	/// <param name="ppParent">
	/// <para>Type: <b>void**</b> The address of a pointer to the parent object.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-getparent#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns one of the <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a> values.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-getparent">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	//unsafe void GetParent( Guid* riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppParent );
	void GetParent< T >( out T ppParent ) where T: IUnknownWrapper ;

	
	// ----------------------------------------------------------------------------------------
	
	public static virtual TObject CreateInstanceOf< TObject, TInterface >( TInterface pComObj )
										where TObject: class, IObject,
										IUnknownWrapper< TObject, TInterface >, new()
															where TInterface: IUnknown {
		TObject obj = new( ) ;
		( (IUnknownWrapper<TInterface>)obj )
							.ComPointer!.Set( pComObj ) ;
		return obj ;
	}
	
	// ========================================================================================
} ;

//! Testing an idea (may remove later)
public interface IObject< TWrapper, TInterface >:
					IUnknownWrapper< TWrapper, TInterface >
							where TWrapper: Object, IUnknownWrapper< TWrapper, TInterface >, new( )
							where TInterface: unmanaged, IUnknown { } ;



public abstract class Object: IObject {
	internal Object( ) => this.ComPointer = new( ) ;
	internal Object( IDXGIObject dxgiObject ) {
		if( dxgiObject is null ) throw new
			ArgumentNullException( nameof(dxgiObject) ) ;
		
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


public abstract class WrapperObject< TWrapper, TInterface >: Object,
															 IObject< TWrapper, TInterface >
								where TWrapper: Object, IUnknownWrapper< TWrapper, TInterface >, new( )
								where TInterface: unmanaged, IUnknown {
	public new ComPtr< TInterface >? ComPointer { get ; protected set ; }
}