#region Using Directives
#pragma warning disable CS1591
using DXSharp.Windows.COM ;
using Windows.Win32.Graphics.Dxgi ;
#endregion
namespace DXSharp.DXGI ;


/// <summary>Wrapper interface for the native IDXGIObject COM interface</summary>
public interface IObject: IDirectXCOMObject,
						  IUnknownWrapper< IDXGIObject > {
	
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
	
	// ========================================================================================
} ;