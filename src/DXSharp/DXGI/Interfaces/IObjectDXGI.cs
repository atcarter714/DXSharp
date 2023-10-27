#region Using Directives
#pragma warning disable CS1591
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using DXSharp.Windows.COM ;
using Windows.Win32.Graphics.Dxgi ;
#endregion
namespace DXSharp.DXGI ;


/// <summary>Wrapper interface for the native IDXGIObject COM interface</summary>
[Wrapper(typeof(IDXGIObject))]
public interface IObject: IDXCOMObject,
						  IComObjectRef< IDXGIObject >,
						  IUnknownWrapper< IDXGIObject > {
	
	// ----------------------------------------------------------
	// Property Overrides ::
	// ----------------------------------------------------------
	/// <inheritdoc cref="IUnknownWrapper{TInterface}.ComPointer"/>
	new ComPtr< IDXGIObject >? ComPointer { get ; }
	
	/// <inheritdoc cref="IComObjectRef{TInterface}.COMObject"/>
	new IDXGIObject? COMObject => ComPointer?.Interface ;

	static Type IUnknownWrapper.ComType => typeof(IDXGIObject) ;
	static Guid IUnknownWrapper.InterfaceGUID => typeof(IDXGIObject).GUID ;
	
	void IDisposable.Dispose( ) {
		ComPointer?.Dispose( ) ;
	}

	// ----------------------------------------------------------
    // Explicit Interface Implementations / Disambiguation ::
    // ----------------------------------------------------------
    IDXGIObject? IComObjectRef< IDXGIObject >.COMObject => COMObject ;
    ComPtr< IDXGIObject >? IUnknownWrapper< IDXGIObject >.ComPointer => ComPointer ;
	
	int IUnknownWrapper.RefCount => (int)( ComPointer?.RefCount ?? 0 ) ;

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
	void GetParent< T >( out T ppParent ) where T: IUnknownWrapper< IDXGIObject >, IInstantiable {
		unsafe {
			var riid = T.InterfaceGUID ;
			COMObject!.GetParent( &riid, out var _parent ) ;
			ppParent = (T) T.Instantiate( _parent ) ;
		}
	}
	
	// ========================================================================================
} ;




	//static Guid IUnknownWrapper< IDXGIObject >.InterfaceGUID => typeof(IDXGIObject).GUID ;
	// ============================================================

// ----------------------------------------------------------------------------------------
	
/*void GetPrivateData< TData >( ref uint pDataSize, nint pData = 0x0000 ) where TData: unmanaged {
	unsafe {
		var guid = typeof(TData).GUID ;
		var hr = ComObject!.GetPrivateData( &guid, ref pDataSize, (void*)pData ) ;
	}
}*/
	
	
/*internal static abstract IObject ConstructInstance< TObject, TInterface >( TInterface pComObj )
	where TObject: class, IObject, IUnknownWrapper< TInterface >
	where TInterface: IDXGIObject ;

internal static T Construct< T, I >( nint ptr )
	where T: Object, IObject, IUnknownWrapper< I >
	where I: IDXGIObject {
	ComPtr< I > comPtr = new( ptr ) ;
	return (T)T.ConstructInstance<T, I>( comPtr.Interface! ) ;
}*/
