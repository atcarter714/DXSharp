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

/*internal interface IObjectConstruction {
	internal static abstract ConstructWrapper
		< IUnknownWrapper<IDXGIObject>, IDXGIObject >? 
							ConstructFunction { get ; }
}*/


/// <summary>Wrapper interface for the native IDXGIObject COM interface</summary>
public interface IObject: IUnknownWrapper< IDXGIObject >/*, IObjectConstruction*/ {

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
	internal static abstract IObject ConstructInstance< TObject, TInterface >( TInterface pComObj )
		where TObject: class, IObject, IUnknownWrapper< TInterface > 
		where TInterface: IDXGIObject ;
	
	internal static T Construct< T, I >( nint ptr )
		where T: Object, IObject, IUnknownWrapper< I >
		where I: IDXGIObject {
		ComPtr< I > comPtr = new( ptr ) ;
		return (T)T.ConstructInstance<T, I>( comPtr.Interface! ) ;
	}
	
	
	/*public static virtual TObject CreateInstanceOf< TObject, TInterface >( TInterface pComObj )
										where TObject: class, IObject,
										IUnknownWrapper< TObject, TInterface >, new()
															where TInterface: IUnknown {
		TObject obj = new( ) ;
		( (IUnknownWrapper<TInterface>)obj )
							.ComPointer!.Set( pComObj ) ;
		return obj ;


	internal static abstract IFactory Create< TFactory >( )
								where TFactory: class, IFactory ;
	}*/

	// ========================================================================================
} ;

//! Testing an idea (may remove later)
/*public interface IObject< TWrapper, TInterface >:
					IUnknownWrapper< TWrapper, TInterface >
							where TWrapper: Object, IUnknownWrapper< TWrapper, TInterface >, new( )
							where TInterface: unmanaged, IUnknown { } ;*/



/*
public abstract class WrapperObject< TWrapper, TInterface >: Object,
															 IObject< TWrapper, TInterface >
								where TWrapper: Object, IUnknownWrapper< TWrapper, TInterface >, new( )
								where TInterface: unmanaged, IUnknown {
	public new ComPtr< TInterface >? ComPointer { get ; protected set ; }
}*/