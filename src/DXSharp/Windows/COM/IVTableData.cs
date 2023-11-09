using System.Runtime.CompilerServices ;
using DXSharp.Windows ;

namespace Windows.Win32.System.Com ;

public interface IVTable {
	/// <summary>
	/// Gets the address of the function at the specified index in the V-Table.
	/// </summary>
	/// <param name="index">Index or offset from the start of the V-table.</param>
	nint this[ int index ] {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get ;
	}
}


public interface IVTableData {
	/// <summary>
	/// Gets a reference to the <see cref="IUnknownUnsafe.VTable"/> field for this instance.
	/// The internal field is a pointer to a pointer. 
	/// This is the actual V-Table location for the native
	/// <a href="https://learn.microsoft.com/en-us/windows/win32/com/com-objects-and-interfaces">COM interface</a>.
	/// </summary>
	/// <remarks>
	/// You can learn more about <a href="https://learn.microsoft.com/en-us/dotnet/standard/native-interop/cominterop">COM Interop in .NET</a>
	/// by reading the online <a href="https://learn.microsoft.com/">Microsoft Learn</a> documentation and articles.
	/// </remarks>
	unsafe ref void** pVTableRef {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get ;
	}

	HResult QueryInterface( ref Guid riid, out nint ppvObject ) ;
	HResult QueryInterface( in  Guid riid, out DXSharp.Windows.COM.IUnknown ppvUnk ) ;

	/// <summary>Increments the reference count for an interface pointer to a COM object. You should call this method whenever you make a copy of an interface pointer.</summary>
	/// <returns>The method returns the new reference count. This value is intended to be used only for test purposes.</returns>
	/// <remarks>
	/// <para>A COM object uses a per-interface reference-counting mechanism to ensure that the object doesn't outlive references to it. You use **AddRef** to stabilize a copy of an interface pointer. It can also be called when the life of a cloned pointer must extend beyond the lifetime of the original pointer. The cloned pointer must be released by calling [IUnknown::Release](/windows/desktop/api/unknwn/nf-unknwn-iunknown-queryinterface(refiid_void)) on it. The internal reference counter that **AddRef** maintains should be a 32-bit unsigned integer.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/unknwn/nf-unknwn-iunknown-addref#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	uint AddRef( ) ;

	/// <summary>Decrements the reference count for an interface on a COM object.</summary>
	/// <returns>The method returns the new reference count. This value is intended to be used only for test purposes.</returns>
	/// <remarks>
	/// <para>When the reference count on an object reaches zero, **Release** must cause the interface pointer to free itself. When the released pointer is the only (formerly) outstanding reference to an object (whether the object supports single or multiple interfaces), the implementation must free the object. Note that aggregation of objects restricts the ability to recover interface pointers.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/unknwn/nf-unknwn-iunknown-release#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	uint Release( ) ;
} ;


//! TODO: See if we can work out a dynamic invocation system without reflection and using unmanaged type constraints ...
public enum ComMethodReturnType {
	Void,
	Byte, SByte,
	Short, UShort,
	Int, UInt,
	Long, ULong,
	Half, Float,
	Double, Decimal,
	CString, CWString,
	Struct, Enum,
	Array, Pointer,
	FunctionPointer,
	Unknown,
} ;

public interface IComMethodArg { object BoxedValue { get ; } }
public interface IComMethodArg< T >: IComMethodArg where T: unmanaged { T Value { get ; } }
public interface IComMethodArgList: IEnumerable< IComMethodArg > {
	int Count { get ; }
	IComMethodArg[ ]? Args { get ; }
	IComMethodArg this[ int index ] { get ; }
}