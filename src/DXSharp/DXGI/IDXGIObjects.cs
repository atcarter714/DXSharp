#pragma warning disable CS1591

#region Using Directives

/* Unmerged change from project 'DXSharp (net7.0-windows10.0.22621.0)'
Before:
using global::System;
After:
using Accessibility;

using DXSharp.DXGI;
using DXSharp.Windows.COM;

using global::System;
*/
using DXSharp.Windows.COM;

using global::System.Runtime.InteropServices;
/* Unmerged change from project 'DXSharp (net7.0-windows10.0.22621.0)'
Before:
using global::System.Runtime.InteropServices.WindowsRuntime;
After:
using global::System.Runtime.InteropServices.WindowsRuntime;
using global::Windows.Win32;
*/


using Windows.Win32.Graphics.Dxgi;
/* Unmerged change from project 'DXSharp (net7.0-windows10.0.22621.0)'
Before:
using global::Windows.Win32;
using Win32 = global::Windows.Win32;


using WinRT.Interop;
using System.Windows.Input;

using DXSharp.DXGI;
using DXSharp.Windows.COM;
using Accessibility;
#endregion
After:
using WinRT.Interop;

using Win32 = global::Windows.Win32;
#endregion
*/

#endregion

namespace DXSharp.DXGI;



/// <summary>
/// Wrapper interface for the native IDXGIObject COM interface
/// </summary>
internal interface IObject: IUnknown<IDXGIObject>
{
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
	void SetPrivateData<T>( uint DataSize, IntPtr pData );

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
	void SetPrivateDataInterface<T>( T pUnknown );

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
	void GetPrivateData( out uint pDataSize, IntPtr pData );

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
	void GetParent( in Guid riid, out object ppParent );
};

internal interface IObject<T>: IObject where T : class
{

};


public class Object: IObject
{
	private protected Object( IDXGIObject dxgiObj ) {
		this.m_dxgiObject = dxgiObj;
	}

	/// <summary>
	/// Destructor
	/// </summary>
	~Object() => Dispose( false );

	readonly IDXGIObject? m_dxgiObject;

	/// <summary>
	/// Gets the pointer to the underyling COM interface
	/// </summary>
	public IntPtr Pointer { get; private set; }



	#region IDisposable Implementation
	protected bool disposedValue;
	public bool Disposed => disposedValue;

	ComPtr<IDXGIObject> IUnknown<IDXGIObject>.ComPtr { get; }

	protected virtual void Dispose( bool disposing ) {
		if( !disposedValue ) {
			if( disposing ) {
				// TODO: dispose managed state (managed objects)
			}

			// TODO: free unmanaged resources (unmanaged objects) and override finalizer
			// TODO: set large fields to null
			if( m_dxgiObject is not null ) {

				/* Unmerged change from project 'DXSharp (net7.0-windows10.0.22621.0)'
				Before:
								int refCount = Marshal.ReleaseComObject( m_dxgiObject );
				After:
								_ = Marshal.ReleaseComObject( m_dxgiObject );
				*/
				_ = Marshal.ReleaseComObject( m_dxgiObject );
			}

			disposedValue = true;
		}
	}

	/// <summary>
	/// Diposes of this instance and frees its native COM resources
	/// </summary>
	public void Dispose() {
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose( disposing: true );
		GC.SuppressFinalize( this );
	}

	public ValueTask DisposeAsync() => throw new NotImplementedException();

	#endregion


	public void GetParent( in Guid riid, out object ppParent ) {
		throw new NotImplementedException();
	}

	public void GetPrivateData( out uint pDataSize, IntPtr pData ) {
		throw new NotImplementedException();
	}

	public void SetPrivateData<T>( uint DataSize, IntPtr pData ) {
		throw new NotImplementedException();
	}

	public void SetPrivateDataInterface<T>( T pUnknown ) {
		throw new NotImplementedException();
	}


	internal static Object Create( IDXGIObject dxgiObj ) {
#if DEBUG || !STRIP_CHECKS
		if( dxgiObj is null )
			throw new ArgumentNullException( "dxgiObj" );
#endif

		var newObj = new Object( dxgiObj );
		newObj.Pointer = Marshal.GetIUnknownForObject( dxgiObj );

		return newObj;
	}

	public static Object Create( IntPtr pComObj ) {
#if DEBUG || !STRIP_CHECKS
		if( pComObj == IntPtr.Zero )
			throw new ArgumentNullException( "dxgiObj" );
#endif

		object? comObj = Marshal.GetObjectForIUnknown( pComObj );
		if( comObj is null )
			throw new COMException( $"DXGI.Object.Create( {pComObj} ): " +
				$"Unable to initialize COM object reference from the given address!" );

		var newDxgiObject = new Object((IDXGIObject)comObj);
		newDxgiObject.Pointer = pComObj;

		return newDxgiObject;
	}
}
