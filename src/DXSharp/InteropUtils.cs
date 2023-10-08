#region Using Directives
using System.Runtime.InteropServices ;
using DXSharp.Windows.COM ;
#endregion

namespace DXSharp ;


public static class InteropUtils {
	public const short _MAXOPT_ = (0x0100|0x0200) ;
	
	public static Guid  __uuidof< T >( ) => typeof( T ).GUID ;
	
	public static Guid GetIID< T >( this T comObject )
		where T: class, IUnknown => __uuidof<T>( ) ;
	
	public static unsafe void* StoreStringGlobal( in string str ) {
		if ( str is not {Length: > 0} ) return null ;
		var len = str.Length ;
		var ptr = Marshal.AllocHGlobal( len ) ;
		Marshal.Copy( str.ToCharArray( ), 0, ptr, len ) ;
		return (void *)ptr ;
	}
	
	/// <summary>Gets the address of the given data (unsafe pointer).</summary>
	/// <param name="data">An unmanaged value type (i.e., blittable struct).</param>
	/// <typeparam name="T">The type of unmanaged data structure.</typeparam>
	/// <returns>A pointer to the given data.</returns>
	/// <remarks>
	/// <b>WARNING ::</b><para/>
	/// This makes no guarantees that the runtime will not move the data around in memory.
	/// It is the responsibility of the caller to use this judiciously, when they are certain
	/// that the data they are obtaining a pointer to is fixed in memory or will be pinned and
	/// immobile for the duration of the pointer's use.
	/// </remarks>
	public static unsafe T* GetPtr< T >( in T data ) where T: unmanaged {
		fixed ( T* p = &data ) return p ;
	}
}