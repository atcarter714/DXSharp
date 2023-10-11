﻿#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp ;


/// <summary>Static class with simple helpers/utilities for interop and "translating" code.</summary>
public static class InteropUtils {
	/// <summary>
	/// Flags for <see cref="MethodImplAttribute"/> equal to
	/// <see cref="MethodImplOptions.AggressiveOptimization"/> |
	/// <see cref="MethodImplOptions.AggressiveInlining"/> ...
	/// </summary>
	/// <remarks>
	/// This is just a short-hand way to specify the most aggressive
	/// optimization and inlining options for the runtime, which is
	/// helpful because it is frequently used and a lot to type out.
	/// </remarks>
	internal const short _MAXOPT_ = (0x100|0x200) ;
	
	/// <summary>Value of a "null" (zero) pointer/address.</summary>
	public const int NULL_PTR = 0x0000000000000000 ;
	
	/// <summary>A static method that accomplishes the same thing as the VC++ __uuidof operator.</summary>
	/// <remarks>Exists simply to make translating C++ code to C#/DXSharp easier and quicker.</remarks>
	internal static Guid __uuidof< T >( ) => typeof(T).GUID ;
	/// <summary>A static method that accomplishes the same thing as the VC++ __uuidof operator.</summary>
	/// <remarks>Exists simply to make translating C++ code to C#/DXSharp easier and quicker.</remarks>
	internal static Guid __uuidof< T >( in T obj ) => typeof(T).GUID ;
	
	/// <summary>
	/// A static method that accomplishes the same thing as the VC++ IID_PPV_ARGS macro.
	/// It gets the GUID of the given object reference's type, but doesn't have actual
	/// compile-time enforcement on the type beyond being a COM interface (IUnknown) ...
	/// </summary>
	/// <remarks>Exists simply to make translating C++ code to C#/DXSharp easier and quicker.</remarks>
	internal static Guid IID_PPV_ARGS< T >( in T obj ) where T: IUnknown => typeof(T).GUID ; //obj?.GetType().GUID ?? Guid.Empty ;
	/// <summary>
	/// A static method that accomplishes the same thing as the VC++ IID_PPV_ARGS macro.
	/// It gets the GUID of the given object reference's type, but doesn't have actual
	/// compile-time enforcement on the type beyond being a COM interface (IUnknown) ...
	/// </summary>
	/// <remarks>Exists simply to make translating C++ code to C#/DXSharp easier and quicker.</remarks>
	internal static unsafe Guid IID_PPV_ARGS< T >( T* pInterface ) where T: unmanaged, IUnknown => typeof(T).GUID ;
	/// <summary>
	/// A static method that accomplishes the same thing as the VC++ IID_PPV_ARGS macro.
	/// It gets the GUID of the given object reference's type, but doesn't have actual
	/// compile-time enforcement on the type beyond being a COM interface (IUnknown) ...
	/// </summary>
	/// <remarks>Exists simply to make translating C++ code to C#/DXSharp easier and quicker.</remarks>
	internal static Guid IID_PPV_ARGS< T >( ComPtr< T > pInterface ) where T: unmanaged, IUnknown => typeof(T).GUID ;
	
	/// <summary>Allocates unmanaged memory and copies the given string to it.</summary>
	/// <param name="str">String to allocate in global unmanaged memory.</param>
	/// <returns>Untyped (void*) pointer to the memory.</returns>
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
	
} ;