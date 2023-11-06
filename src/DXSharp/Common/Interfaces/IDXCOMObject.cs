#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;

using DXSharp.Windows ;
using DXSharp.Windows.COM ;
using static DXSharp.InteropUtils ;
#endregion
namespace DXSharp ;


/// <summary>Contract for wrapper objects with reference to a COM object interface.</summary>
public interface IComObjectRef< T > where T: IUnknown {
	/// <summary>A reference to the native COM object interface.</summary>
	T? ComObject { get; }
} ;


/// <summary>Contract for instantiable wrapper objects.</summary>
public interface IInstantiable {
	public static virtual IInstantiable Instantiate( ) => throw new NotImplementedException( ) ;
	public static virtual IInstantiable Instantiate( nint pComObj ) => throw new NotImplementedException( ) ;
	public static virtual IInstantiable Instantiate< ICom >( ICom pComObj ) where ICom: IUnknown? => throw new NotImplementedException( ) ;
} ;


/// <summary>
/// A not-so-useful idea being removed/phased out ...
/// </summary>
/// <typeparam name="T">The type of class to instantiate.</typeparam>
[Obsolete( "Use IInstantiable instead." )]
public interface IInstantiable< T >: IInstantiable where T: class, IInstantiable< T > {
	new static abstract T? Instantiate( nint ptr ) ;
} ;


/// <summary>
/// Contract for the common COM interface shared by
/// objects in DirectX and the Windows API.
/// </summary>
public interface IDXCOMObject: IComIID,
							   IDisposable,
							   IAsyncDisposable {
	// ----------------------------------------------------------
	// Interface Methods:
	// ----------------------------------------------------------
	
	/// <summary>Get a pointer to the object's data.</summary>
	/// <param name="name">User-defined name or GUID to associate with the data.</param>
	/// <param name="pDataSize">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a>*</b> The size of the data.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-getprivatedata#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="pData">
	/// <para>Type: <b>void*</b> Pointer to the data.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-getprivatedata#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns one of the following <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-getprivatedata">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	HResult GetPrivateData( in Guid name, ref uint pDataSize, nint pData = 0x0000 ) ;
	
	/// <summary><para>
	/// Set data to an object's private data storage and associate it with a custom user-defined GUID
	/// (used to retrieve it with <see cref="GetPrivateData"/>).
	/// </para></summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/openspecs/windows_protocols/ms-oaut/6e7d7108-c213-40bc-8294-ac13fe68fd50">REFGUID</a></b> A GUID that identifies the data.Use this GUID in a call to<a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiobject-getprivatedata"> GetPrivateData</a> to get the data.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-setprivatedata#parameters">Read more on docs.microsoft.com</a>.</para>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-setprivatedata">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	HResult SetPrivateData( in Guid name, uint DataSize, nint pData ) ;
	
	/// <summary>Set an interface in the object's private data.</summary>
	/// <param name="name">User-defined name or GUID to associate with the data.</param>
	/// <param name="pUnknown">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/unknwn/nn-unknwn-iunknown">IUnknown</a>*</b> The interface to set.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-setprivatedatainterface#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns one of the following <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-setprivatedatainterface">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	HResult SetPrivateDataInterface< T >( in Guid name, in T? pUnknown ) where T: IDXCOMObject ;
	
	
	// ----------------------------------------------------------
	// Static Properties:
	// ----------------------------------------------------------
	/// <summary>The type of COM RCW object managed by this proxy interface.</summary>
	public static Type ComType => typeof( IUnknown ) ;
	
	/// <summary>The COM IID (interface identifier) <see cref="Guid"/> of the DirectX/COM interface.</summary>
	public static Guid IID => ( ComType.GUID ) ;

	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )] get {
			ReadOnlySpan< byte > data = typeof(IUnknown).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	// ==========================================================
} ;