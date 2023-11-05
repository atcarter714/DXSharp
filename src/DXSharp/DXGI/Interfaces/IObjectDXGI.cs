#pragma warning disable CS1591
#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32 ;
using Windows.Win32.Graphics.Dxgi ;

using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI ;


/// <summary>Wrapper interface for the native IDXGIObject COM interface</summary>
[ProxyFor(typeof(IDXGIObject))]
public interface IObject: IDXCOMObject {
	// ----------------------------------------------------------

	/// <summary>Gets the parent of the object.</summary>
	/// <param name="ppParent">
	/// <para>Type: <b>void**</b> The address of a pointer to the parent object.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-getparent#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns one of the <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a> values.</para>
	/// </returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-getparent">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void GetParent< T >( out T ppParent ) 
		where T: IUnknownWrapper< IDXGIObject >, IInstantiable ;

	// ----------------------------------------------------------
	
	new static Type ComType => typeof(IDXGIObject) ;
	
	static ref readonly Guid IComIID.Guid  {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIObject).GUID
														   .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}

	// ==========================================================
} ;
