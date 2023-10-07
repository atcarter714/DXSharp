#region Using Directives
#pragma warning disable CS1591

using DXSharp.Windows.COM ;
using Windows.Win32.Graphics.Dxgi ;
using System.Diagnostics.CodeAnalysis ;
using global::System.Runtime.InteropServices ;
#endregion

namespace DXSharp.DXGI ;


/// <summary>
/// Wrapper interface for the native IDXGIObject COM interface
/// </summary>
internal interface IObject: IUnknown< IObject > {
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
	//unsafe void SetPrivateData( global::System.Guid* Name, uint DataSize, void* pData );
	void SetPrivateData< T >( uint DataSize, nint pData ) ;

	/// <summary>Set an interface in the object's private data.</summary>
	/// <param name="Name">
	/// <para>Type: <b><a href="https://docs.microsoft.com/openspecs/windows_protocols/ms-oaut/6e7d7108-c213-40bc-8294-ac13fe68fd50">REFGUID</a></b> A GUID identifying the interface.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-setprivatedatainterface#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
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
	//unsafe void SetPrivateDataInterface( global::System.Guid* Name, [MarshalAs( UnmanagedType.IUnknown )] object pUnknown );
	void SetPrivateDataInterface< T >( in T pUnknown ) where T: IUnknown ;
	
	/// <summary>Get a pointer to the object's data.</summary>
	/// <param name="Name">
	/// <para>Type: <b><a href="https://docs.microsoft.com/openspecs/windows_protocols/ms-oaut/6e7d7108-c213-40bc-8294-ac13fe68fd50">REFGUID</a></b> A GUID identifying the data.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-getprivatedata#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
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
	void GetPrivateData( out uint pDataSize, nint pData ) ;

	/// <summary>Gets the parent of the object.</summary>
	/// <param name="riid">
	/// <para>Type: <b>REFIID</b> The ID of the requested interface.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-getparent#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
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
	//unsafe void GetParent( global::System.Guid* riid, [MarshalAs( UnmanagedType.IUnknown )] out object ppParent );
	void GetParent< T >( out IUnknown ppParent ) ;
} ;


internal interface IObject< T >: IObject
							where T: IUnknown< IObject > { } ;


public abstract class Object: IObject {
	internal Object( ) { }
	private protected Object( [NotNull] IObject iObj ) {
		this.m_IObject = iObj ;
		var comPtrToIUnknown = iObj.ComPtr?.Interface ;
		if ( comPtrToIUnknown is null ) throw new ArgumentNullException( nameof( iObj.ComPtr ) ) ;
		this.ComPtr = new ( comPtrToIUnknown ) ;
	}
	private protected Object( nint iUnknownPtr ) {
		if ( iUnknownPtr is 0 ) throw new
			ArgumentNullException( nameof(iUnknownPtr),
						string.Format( LibResources.Object_Object__0_____Cannot_be_null_, 
									   nameof(iUnknownPtr) ) ) ;
		
		this.m_IObject = Marshal.GetObjectForIUnknown( iUnknownPtr ) as IObject ;
		if ( this.m_IObject is null ) throw new COMException( $"DXGI.Object.Create( {iUnknownPtr} ): " +
			$"Unable to initialize COM object reference from the given address!" ) ;
		this.ComPtr = new ( iUnknownPtr ) ;
	}
	~Object( ) => Dispose( false ) ;
	
	readonly IObject? m_IObject ;
	public ComPtr< IUnknown >? ComPtr { get ; private set ; }
	
	/// <summary>Gets the pointer to the underyling COM interface.</summary>
	public IntPtr Pointer { get; private set; }
	
	public int QueryInterface( ref Guid riid, out IntPtr ppvObject ) {
		if ( m_IObject is null ) throw new
			ObjectDisposedException( nameof(Object), $"{nameof(Object)} :: " +
													 $"Internal object \"{nameof(m_IObject)}\" is destroyed/null." ) ;
		return m_IObject.QueryInterface( ref riid, out ppvObject ) ;
	}

	public uint AddRef( ) => ( (IUnknown)m_IObject! ).AddRef( ) ;
	public uint Release( ) => ( (IUnknown)m_IObject! )?.Release( ) ?? 0 ;

	public void GetParent< T >( out IUnknown ppParent ) {
		if ( m_IObject is null ) throw new
			ObjectDisposedException( nameof(Object), $"{nameof(Object)} :: " +
													 $"Internal object \"{nameof(m_IObject)}\" is destroyed/null." ) ;
		m_IObject.GetParent< T >( out ppParent ) ;
	}

	public void GetPrivateData( out uint pDataSize, nint pData ) {
		if ( m_IObject is null ) throw new 
			ObjectDisposedException( nameof(Object), $"{nameof(Object)} :: " +
													 $"Internal object \"{nameof(m_IObject)}\" is destroyed/null." ) ;
		m_IObject.GetPrivateData( out pDataSize, pData ) ;
	}

	public void SetPrivateData< T >( uint DataSize, nint pData ) {
		if ( m_IObject is null ) throw new 
			ObjectDisposedException( nameof(Object), $"{nameof(Object)} :: " +
													 $"Internal object \"{nameof(m_IObject)}\" is destroyed/null." ) ;
		m_IObject.SetPrivateData< T >( DataSize, pData ) ;
	}

	public void SetPrivateDataInterface< T >( in T pUnknown )
												where T: IUnknown {
		if ( m_IObject is null ) throw new 
			ObjectDisposedException( nameof(Object), $"{nameof(Object)} :: " +
													 $"Internal object \"{nameof(m_IObject)}\" is destroyed/null." ) ;
		m_IObject.SetPrivateDataInterface< T >( pUnknown ) ;
	}


	internal static T Create< T >( IObject dxObj )
									where T: Object, new( ) {
#if DEBUG || !STRIP_CHECKS
		if( dxObj is null )
			throw new ArgumentNullException( nameof(dxObj) ) ;
#endif
		T newObj  = new( ) ;
		nint ptr  = Marshal.GetIUnknownForObject( dxObj ) ;
		return Create< T >( ptr ) ;
	}

	public static T Create< T >( IntPtr pComObj )
								where T: Object, new( ) {
#if DEBUG || !STRIP_CHECKS
		if( pComObj == IntPtr.Zero )
			throw new ArgumentNullException( nameof(pComObj) ) ;
#endif

		object? comObj = Marshal.GetObjectForIUnknown( pComObj ) ;
		if( comObj is null )
			throw new COMException( $"DXGI.Object.Create( {pComObj} ): " +
				$"Unable to initialize COM object reference from the given address!" );

		T dxObject = new( ) ;
		dxObject.Pointer = pComObj ;
		return dxObject ;
	}
	
#region IDisposable Implementation
	protected bool disposedValue;
	public bool Disposed => disposedValue;

	protected virtual void Dispose( bool disposing ) {
		if( !disposedValue ) {
			if( disposing ) {
				// TODO: dispose managed state (managed objects)
			}

			// TODO: free unmanaged resources (unmanaged objects) and override finalizer
			// TODO: set large fields to null
			if( m_IObject is not null ) {

				/* Unmerged change from project 'DXSharp (net7.0-windows10.0.22621.0)'
				Before:
								int refCount = Marshal.ReleaseComObject( m_dxgiObject );
				After:
								_ = Marshal.ReleaseComObject( m_dxgiObject );
				*/
				_ = Marshal.ReleaseComObject( m_IObject );
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



/*
public uint AddRef( ) {
	if ( m_IObject is null ) throw new 
		ObjectDisposedException( nameof(Object), $"{nameof(Object)} :: " +
				$"Internal object \"{nameof(m_IObject)}\" is destroyed/null." ) ;
				
	return ( (IUnknown)m_IObject ).AddRef( ) ;
}
 */