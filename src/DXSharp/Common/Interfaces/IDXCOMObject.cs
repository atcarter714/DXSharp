#region Using Directives

using System.Diagnostics.CodeAnalysis ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.System.Com ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;
using static DXSharp.InteropUtils ;
using IUnknown = DXSharp.Windows.COM.IUnknown ;

#endregion
namespace DXSharp ;


/// <summary>Contract for wrapper objects with reference to a COM object interface.</summary>
public interface IComObjectRef< out T > where T: IUnknown {
	/// <summary>A reference to the native COM object interface.</summary>
	T? ComObject { get ; }
} ;


/// <summary>Contract for instantiable wrapper objects.</summary>
public interface IInstantiable {
	public static virtual IInstantiable Instantiate( ) => throw new NotImplementedException( ) ;
	public static virtual IInstantiable Instantiate( nint pComObj ) => throw new NotImplementedException( ) ;
	public static virtual IInstantiable Instantiate< TComObj >( TComObj pComObj ) 
		where TComObj: IUnknown? => throw new NotImplementedException( ) ;
	
	
	
	public static TComObj ConvertArg< TComObj >( IUnknown pComObj ) where TComObj: IUnknown? =>
		(TComObj ?)pComObj ?? throw new ArgumentNullException( nameof(pComObj) ) ;
	
	public static virtual W Instantiate< W >( object? _rcwObj ) 
																where W: IInstantiable, IComIID {
		ArgumentNullException.ThrowIfNull( _rcwObj, nameof(_rcwObj) ) ;
		var pUnknown = ( _rcwObj as IUnknown ) 
						?? throw new InvalidCastException( $"{nameof(_rcwObj)}: " +
														   $"Failed cast from {_rcwObj.GetType().Name} " +
														   $"to {nameof(IUnknown)}." ) ;

		unsafe {
			ref var rUnknown = ref IUnknownUnsafe.CreateUnsafeRef( pUnknown ) ;
			rUnknown.QueryInterface( W.Guid, out void* ppv ) ;
			W _wrapper = (W?) W.Instantiate( (nint)ppv )
				?? throw new InvalidCastException( $"{nameof(_rcwObj)}: " +
												   $"Failed cast from {nameof(IUnknown)} " +
												   $"to {typeof(W).Name}." ) ;
			return _wrapper ;
		}
	}
} ;



/// <summary>
/// Contract for the common COM interface shared by
/// objects in DirectX and the Windows API.
/// </summary>
public interface IDXCOMObject: IComIID,
							   IDisposableObject {
	// ----------------------------------------------------------
	// Properties:
	// ----------------------------------------------------------
	
	/// <summary>A name associated with the COM object.</summary>
	/// <remarks>
	/// <para>
	/// This is a shortcut for the <see cref="GetPrivateData"/> and <see cref="SetPrivateData"/> methods
	/// using the <see cref="Guid"/> <see cref="COMUtility.WKPDID_D3DDebugObjectName"/> to get/set the name.
	/// You should not repeatedly access this property (especially in loops) but rather cache the value
	/// somewhere and read/write it as needed, and try to only set it once or the fewest times possible.
	/// </para>
	/// </remarks>
	public string? Name { get ; set ; }
	
	
	// ----------------------------------------------------------
	// Interface Methods:
	// ----------------------------------------------------------
	//! Extra Methods:
	/// <summary>
	/// Gets a string that represents the name associated with the COM object.
	/// </summary>
	/// <returns>
	/// <para>Type: <see cref="string"/> - A string that represents the name associated with the COM object.</para>
	/// </returns>
	/// <remarks><para>
	/// This is a shortcut for the <see cref="GetPrivateData"/> and <see cref="SetPrivateData"/> methods
	/// using the <see cref="Guid"/> <see cref="COMUtility.WKPDID_D3DDebugObjectName"/> to get/set the name.
	/// You should not repeatedly access this property (especially in loops) but rather cache the value
	/// somewhere and read/write it as needed, and try to only set it once or the fewest times possible.
	/// </para></remarks>
	string GetObjectName( ) ;
	
	/// <summary>
	/// Sets a string that represents the name associated with the COM object.
	/// </summary>
	/// <param name="name">A name to associate with the object.</param>
	/// <remarks><para>
	/// This is a shortcut for the <see cref="GetPrivateData"/> and <see cref="SetPrivateData"/> methods
	/// using the <see cref="Guid"/> <see cref="COMUtility.WKPDID_D3DDebugObjectName"/> to get/set the name.
	/// You should not repeatedly access this property (especially in loops) but rather cache the value
	/// somewhere and read/write it as needed, and try to only set it once or the fewest times possible.
	/// </para></remarks>
	void SetObjectName( string name ) ;
	
	// ----------------------------------------------------------
	
	
	/// <summary>Get a pointer to the object's data.</summary>
	/// <param name="name">User-defined name or GUID to associate with the data.</param>
	/// <param name="pDataSize"><para>The size of the data.</para></param>
	/// <param name="pData"><para>Pointer to the data.</para></param>
	/// <returns>
	/// A <a href="https://learn.microsoft.com/en-us/windows/win32/direct3ddxgi/dxgi-error">DXGI_ERROR</a>
	/// code as an <see cref="HResult"/>.
	/// </returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-getprivatedata">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	HResult GetPrivateData( in Guid name, ref uint pDataSize, nint pData = 0x0000 ) ;
	
	/// <summary>
	/// Set data to an object's private data storage and associate it with a custom user-defined GUID
	/// (used to retrieve it with the <see cref="GetPrivateData"/> method).
	/// </summary>
	/// <param name="name">
	/// A <see cref="Guid"/> that identifies the data. Use this value in a call to
	/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiobject-getprivatedata">GetPrivateData</a>
	/// to get the data.
	/// </param>
	/// <param name="DataSize"><para>The size of the data.</para></param>
	/// <param name="pData"><para>Pointer to the data.</para></param>
	/// <remarks>
	/// <a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-setprivatedata">Learn more about this API from docs.microsoft.com</a>.
	/// </remarks>
	HResult SetPrivateData( in Guid name, uint DataSize, nint pData ) ;
	
	/// <summary>Set an interface in the object's private data.</summary>
	/// <param name="name">User-defined name or GUID to associate with the data.</param>
	/// <param name="pUnknown">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/unknwn/nn-unknwn-iunknown">IUnknown</a>*</b> The interface to set.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-setprivatedatainterface#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>A <a href="https://learn.microsoft.com/en-us/windows/win32/direct3ddxgi/dxgi-error">DXGI_ERROR</a> code as an <see cref="HResult"/>.</para>
	/// </returns>
	/// <remarks>
	/// This API associates an interface pointer with the object.
	/// When the interface is set its reference count is incremented.
	/// When the data are overwritten (by calling SPD or SPDI with the same GUID)
	/// or the object is destroyed, ::Release() is called and the interface's
	/// reference count is decremented.<para/>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-setprivatedatainterface">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	HResult SetPrivateDataInterface< T >( in Guid name, in T? pUnknown ) where T: IDXCOMObject ;
	
	
	/// <summary>Increments the reference count for an interface on an object.</summary>
	uint    AddRef( ) ;
	/// <summary>Decrements the reference count for an interface on an object.</summary>
	uint    Release( ) ;
	/// <summary>Gets a pointer to a specified interface (if supported) on an object.</summary>
	HResult QueryInterface( in Guid riid, out nint ppvObject ) ;
	/// <summary>
	/// Gets a pointer to a specified interface (if supported) on an object and returns
	/// a constructed wrapper object for the interface to access it.
	/// </summary>
	HResult QueryInterface< T >( out T? ppvUnk ) where T: IDXCOMObject, IInstantiable ;
	
	
	// ----------------------------------------------------------
	// Static Properties:
	// ----------------------------------------------------------
	/// <summary>The type of COM RCW object managed by this proxy interface.</summary>
	public static Type ComType => typeof( IUnknown ) ;
	
	/// <summary>The COM IID (interface identifier) <see cref="Guid"/> of the DirectX/COM interface.</summary>
	public static Guid IID => ( ComType.GUID ) ;
	
	/// <summary>The <see cref="Guid"/> of the COM interface type.</summary>
	/// <remarks>This is generally the same <see cref="Guid"/> as the IID.</remarks>
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )] get {
			ReadOnlySpan< byte > data = typeof(IUnknown).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	// ==========================================================
} ;

