﻿#pragma warning disable CS1591, CS1573, CS0465, CS0649, CS8019, CS1570, CS1584, CS1658, CS0436, CS8981, CS8500, CS9091, CS8909

#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32.Foundation ;

using DXSharp.Windows ;
using static DXSharp.InteropUtils ;
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
#endregion
namespace Windows.Win32.System.Com ;


// -----------------------------------------------------------------
//[UnmanagedFunctionPointer(CallingConvention.Winapi)]
[UnmanagedFunctionPointer(CallingConvention.StdCall, 
						  BestFitMapping = true, CharSet = CharSet.Auto, SetLastError = true)]
internal unsafe delegate uint AddRefDelegate( IUnknownUnsafe*  pThis ) ;

//[UnmanagedFunctionPointer(CallingConvention.Winapi)]
internal unsafe delegate uint ReleaseDelegate( IUnknownUnsafe* pThis ) ;

//[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal unsafe delegate HResult QueryInterfaceDelegate( IUnknownUnsafe* pThis,
														 Guid* riid, void** ppvObject ) ;
// -----------------------------------------------------------------


[Guid( "00000000-0000-0000-C000-000000000046" )]
public unsafe partial struct IUnknownUnsafe: DXSharp.Windows.COM.IUnknown,
											 IComIID, IVTableData {
	// -----------------------------------------------------------------
	
	
	/// <summary>
	/// Represents the V-Table data structure for the <see cref="IUnknown"/>
	/// COM interface (i.e., the structure where the function pointers are stored).
	/// </summary>
	/// <remarks>
	/// This is a different mode of
	/// <a href="https://learn.microsoft.com/en-us/dotnet/standard/native-interop/qualify-net-types-for-interoperation#consume-com-types-from-net">qualifying .NET types for COM interoperation</a>
	/// than the RCW (Runtime Callable Wrapper) mode used by declaring a managed interface with the <see cref="ComImportAttribute"/>.
	/// <para><para>
	/// Explore the documentation for
	/// <a href="https://learn.microsoft.com/en-us/dotnet/standard/native-interop/cominterop">COM Interop in .NET</a>, 
	/// the <a href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.comimportattribute?view=net-7.0">ComImportAttribute</a> type 
	/// and the <a href="https://docs.microsoft.com/en-us/dotnet/standard/native-interop/">Native Interoperability</a> documentation to learn more.
	/// </para></para>
	/// </remarks>
	public struct VTable: IVTable, IEquatable< VTable >, IEquatable< IVTable > {
		// ---------------------------------------------------------------------------
		/// <summary>The minimum number of function pointers the V-Table is known to have.</summary>
		/// <remarks>
		/// In reality, a V-Table could have more functions than the current <see cref="IVTable"/> implementation
		/// is aware of, because a complex COM interface could have multiple base interfaces, and each of those are
		/// made up of the smaller V-Tables of their ancestors. This is why the <b><see cref="IVTable.PointerCount"/></b>
		/// property is used to determine the bounds of the V-Table, and why the <see cref="IVTable.this"/> indexer
		/// (used to access a function pointer at the specified index/offset) does not consider it "out of bounds" if
		/// you wish to index beyond the known bounds of the V-Table and does not throw an <see cref="Exception"/>.<para/>
		/// For example, <i>all</i> COM interfaces inherit from <see cref="DXSharp.Windows.COM.IUnknown"/>, which has 3 function pointers,
		/// so the methods of <i>any</i> COM interface will be beyond the bounds of the <b><see cref="IUnknownUnsafe.VTable"/></b> (3) ...
		/// </remarks>
		public const int MIN_FNPTR_COUNT = 3 ;
		
		// ---------------------------------------------------------------------------
		//! QueryInterface Function Pointer:
		internal delegate *unmanaged [Stdcall]< IUnknownUnsafe*, Guid*, void**, HRESULT > 
			_0_QueryInterface ;
		
		//! AddRef Function Pointer:
		internal delegate *unmanaged [Stdcall]< IUnknownUnsafe*, uint > 
			_1_AddRef ;

		//! Release Function Pointer:
		internal delegate *unmanaged [Stdcall]< IUnknownUnsafe*, uint > 
			_2_Release ;
		// ---------------------------------------------------------------------------

		public int PointerCount => 3 ;
		
		
		/// <summary>
		/// Gets the value of the [<i><c>n</c></i>th] pointer field in the V-table, at the specified
		/// <i><paramref name="index"/></i> (i.e., the function pointer's field offset).
		/// This is the address of the function the field actually points <i>to</i> ...<para/>
		/// Note that this is <i>not</i> the address of the function pointer <i>data field</i>
		/// within the <see cref="VTable"/> structure.
		/// </summary>
		/// <param name="index">The index or offset of the function pointer in the V-table.</param>
		/// <exception cref="IndexOutOfRangeException">
		/// Thrown if the given index exceeds the bounds of the V-table's memory.
		/// <para><para>
		/// <b>NOTE:</b> <para/>
		/// The exception is thrown when compiled without the <i><b><c>STRIP_CHECKS</c></b></i> symbol,
		/// either when building in <c>Debug</c> configurations or with either of the
		/// <i><b><c>DEV_BUILD</c></b></i> or <i><b><c>DEBUG_COM</c></b></i> symbols defined.
		/// </para></para>
		/// </exception>
		public nint this[ int index ] {
			[MethodImpl(_MAXOPT_)] get { fixed ( void* pThis = &this ) {
#if !STRIP_CHECKS
					if ( index < 0 ) {
#if (DEBUG || DEBUG_COM || DEV_BUILD)
						throw new IndexOutOfRangeException( $"{index}" ) ;
#else
						return nint.Zero ;
#endif
					}
#if DEBUG_COM_LOUD
					if ( index > 3 ) {
						Debug.WriteLine( $"{nameof(VTable)} :: " +
										 $"An interface method beyond the known bounds " +
										 $"was fetched from < offset: {index}, value: {((ulong)pThis):X} > ..." ) ;
					}
#endif
#endif
					void** ptrPtr = *( (void ***)pThis ) ;
					return (nint)ptrPtr[ index ] ;
				}
			}
		}
		
		// ---------------------------------------------------------
		public HResult InvokeQueryInterface( in Guid riid, out nint? ppvObject ) {
			var   guid = riid ;
			void* fn   = null ;
			ppvObject = null ;
			fixed( void* p = &this ) {
				HRESULT hr = _0_QueryInterface( (IUnknownUnsafe *)p, &guid, &fn ) ;
				if( fn is not null ) ppvObject = (nint)fn! ;
				return hr ;
			}
		}
		public uint InvokeAddRef( ) {
			fixed( void* p = &this ) {
				uint count = _1_AddRef( (IUnknownUnsafe *)p ) ;
				return count ;
			}
		}
		public uint InvokeRelease( ) {
			fixed( void* p = &this ) {
				uint count = _2_Release( (IUnknownUnsafe *)p ) ;
				return count ;
			}
		}
		
		
		/*public TOut? InvokeFnPtr< TOut >( int index, IComMethodArgList? args = null ) where TOut: unmanaged {
			fixed( void* p = &this ) {
				uint  count = 0x00U ;
				void* thisAddr = (void *)this[ index ] ;
				
				if( args is null ) {
					var fn = (delegate *unmanaged [Stdcall]< IUnknownUnsafe*, void >)p ;
					fn( (IUnknownUnsafe *)p ) ;
					return null ;
				}
				else {
						var argList = args.Args ;
				}
			}
		}*/
		
		
		// ---------------------------------------------------------
		
		
		/// <summary>
		/// Gets a managed <see cref="AddRefDelegate"/> wrapper for the COM interface method <c>HRESULT AddRef( )</c>.
		/// </summary>
		/// <returns>
		/// An <see cref="AddRefDelegate"/> delegate that invokes the native <c>HRESULT AddRef( )</c> method.
		/// </returns>
		[MethodImpl(_MAXOPT_)] internal AddRefDelegate GetAddRefDelegate( ) =>
			Marshal.GetDelegateForFunctionPointer< AddRefDelegate >( (nint)_1_AddRef ) ;
		
		/// <summary>
		/// Gets a managed <see cref="ReleaseDelegate"/> wrapper for the COM interface method <c>HRESULT Release( )</c>.
		/// </summary>
		/// <returns>
		/// An <see cref="ReleaseDelegate"/> delegate that invokes the native <c>HRESULT Release( )</c> method.
		/// </returns>
		[MethodImpl(_MAXOPT_)] internal ReleaseDelegate GetReleaseDelegate( ) =>
		 			Marshal.GetDelegateForFunctionPointer< ReleaseDelegate >( (nint)_2_Release ) ;
		
		/// <summary>
		/// Gets a managed <see cref="QueryInterfaceDelegate"/> wrapper for the COM interface method <c>HRESULT QueryInterface(REFIID, void**)</c>.
		/// </summary>
		/// <returns>
		/// An <see cref="QueryInterfaceDelegate"/> delegate that invokes the native <c>HRESULT QueryInterface(REFIID, void**)</c> method.
		/// </returns>
		[MethodImpl(_MAXOPT_)] internal QueryInterfaceDelegate GetQueryInterfaceDelegate( ) =>
		 			Marshal.GetDelegateForFunctionPointer< QueryInterfaceDelegate >( (nint)_0_QueryInterface ) ;


		#region System Overrides
		//! TODO: Resolve warning about function pointer comparison (CS8909 - currently suppressed) ...
		public bool Equals( VTable other ) =>
			_0_QueryInterface == other._0_QueryInterface
					&& _1_AddRef == other._1_AddRef
						&& _2_Release == other._2_Release ;

		public bool Equals( IVTable? other ) =>
			other is not null && other[ 0 ] == this[ 0 ]
							  && other[ 1 ] == this[ 1 ]
							  && other[ 2 ] == this[ 2 ] ;
		
		public override bool Equals( object? obj ) => 
				obj is VTable other && Equals( other ) ;
		
		public override int GetHashCode( ) =>
					HashCode.Combine( (nint)_0_QueryInterface,
										  (nint)_1_AddRef,
											(nint)_2_Release ) ;
		#endregion
		
		
		
		// --------------------------------------------------------------
		public static explicit operator VTable( nint addr ) {
			VTable* vtbl = (VTable*)addr ;
			return *vtbl ;
		}
		
		public static explicit operator VTable( in IUnknownUnsafe pUnk ) {
			VTable* vtbl = (VTable*)pUnk.lpVtbl ;
			return *vtbl ;
		}
		
		
		public static bool operator !=( in VTable left, in VTable right ) => !( left == right ) ;
		public static bool operator ==( in VTable left, in VTable right ) => 
												left._1_AddRef == right._1_AddRef 
													&& left._2_Release == right._2_Release 
														&& left._0_QueryInterface == right._0_QueryInterface ;
		// ==============================================================
	} //! <-- VTable Structure
	// -----------------------------------------------------------------
	
	
	internal void** lpVtbl ;
	
	/// <summary>
	/// Gets a reference to the <see cref="IUnknownUnsafe.VTable"/> field for this instance.
	/// The internal field is a pointer of type <see cref="void**"/> (pointer to a pointer).
	/// This is the actual V-Table location for the native
	/// <a href="https://learn.microsoft.com/en-us/windows/win32/com/com-objects-and-interfaces">COM interface</a>.
	/// </summary>
	/// <remarks>
	/// You can learn more about <a href="https://learn.microsoft.com/en-us/dotnet/standard/native-interop/cominterop">COM Interop in .NET</a>
	/// by reading the online <a href="https://learn.microsoft.com/">Microsoft Learn</a> documentation and articles.
	/// </remarks>
	public ref void** pVTableRef {
		[MethodImpl( MethodImplOptions.AggressiveInlining )] get {
			fixed ( void* fieldAddress = &lpVtbl ) {
				ref void** fieldRef = ref *( (void***)( fieldAddress ) ) ;
				return ref fieldRef ;
			}
		}
	}
	
	// -----------------------------------------------------------------

	
	public HRESULT QueryInterface( in Guid riid, out void* ppvObject ) {
		fixed ( void** ppvObjectLocal = &ppvObject ) {
			fixed ( Guid* riidLocal = &riid ) {
				HRESULT __result = this.QueryInterface( riidLocal, ppvObjectLocal ) ;
				return __result ;
			}
		}
	}

	/// <summary>A helper function template that infers an interface identifier, and calls [QueryInterface(REFIID,void)](/windows/desktop/api/unknwn/nf-unknwn-iunknown-queryinterface(refiid_void)).</summary>
	/// <returns>The function passes the return value back from [QueryInterface(REFIID,void)](/windows/desktop/api/unknwn/nf-unknwn-iunknown-queryinterface(refiid_void)).</returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/unknwn/nf-unknwn-iunknown-queryinterface(q)">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	public HRESULT QueryInterface( Guid* riid, void** ppvObject ) {
		return ( (delegate *unmanaged [Stdcall]<IUnknownUnsafe*, Guid*, void**, HRESULT>)
				   lpVtbl[ 0 ] )( (IUnknownUnsafe *)lpVtbl, riid, ppvObject ) ;
	}

	
	/// <summary>Increments the reference count for an interface pointer to a COM object. You should call this method whenever you make a copy of an interface pointer.</summary>
	/// <returns>The method returns the new reference count. This value is intended to be used only for test purposes.</returns>
	/// <remarks>
	/// <para>A COM object uses a per-interface reference-counting mechanism to ensure that the object doesn't outlive references to it. You use **AddRef** to stabilize a copy of an interface pointer. It can also be called when the life of a cloned pointer must extend beyond the lifetime of the original pointer. The cloned pointer must be released by calling [IUnknown::Release](/windows/desktop/api/unknwn/nf-unknwn-iunknown-queryinterface(refiid_void)) on it. The internal reference counter that **AddRef** maintains should be a 32-bit unsigned integer.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/unknwn/nf-unknwn-iunknown-addref#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	public uint AddRef( ) {
		if( lpVtbl is null ) return 0x00U ;
		nint vTableAddr = (nint)lpVtbl ;
		var _marshalStyleCall =
			( (delegate* unmanaged< nint, uint >)( *( *(void ***)vTableAddr + 1) ) ) ;
		/* +1 is for the IUnknown.AddRef slot */
		return _marshalStyleCall( vTableAddr ) ;
		/*return ( (delegate *unmanaged [Stdcall]< IUnknownUnsafe*, uint >)
				   lpVtbl[ 1 ] )( (IUnknownUnsafe *)lpVtbl ) ;*/ //Unsafe.AsPointer( ref this ) ) ;
	}

	/// <summary>Decrements the reference count for an interface on a COM object.</summary>
	/// <returns>The method returns the new reference count. This value is intended to be used only for test purposes.</returns>
	/// <remarks>
	/// <para>When the reference count on an object reaches zero, **Release** must cause the interface pointer to free itself. When the released pointer is the only (formerly) outstanding reference to an object (whether the object supports single or multiple interfaces), the implementation must free the object. Note that aggregation of objects restricts the ability to recover interface pointers.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/unknwn/nf-unknwn-iunknown-release#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	public uint Release( ) {
		if( lpVtbl is null ) return 0x00U ;
		return ( (delegate *unmanaged [Stdcall]< IUnknownUnsafe*, uint >)
				   lpVtbl[ 2 ] )( (IUnknownUnsafe *)lpVtbl ) ;
	}

	
	
	public HResult QueryInterface( ref Guid riid, out nint ppvObject ) {
		fixed( Guid* pRiid = &riid ) {
			fixed( nint* ppvObjectLocal = &ppvObject ) {
				return QueryInterface( pRiid, (void **)ppvObjectLocal ) ;
			}
		}
	}

	public HResult QueryInterface( in Guid riid, out DXSharp.Windows.COM.IUnknown ppvUnk ) {
		#warning Needs test ... this will probably fail, just wanna see
		var guid      = riid ;
		var hr = QueryInterface( ref guid, out nint addr ) ;
		var p        = (DXSharp.Windows.COM.IUnknown *)addr ;
		ppvUnk      = *p ;
		return hr ;
	}
	
	
	// -----------------------------------------------------------------

	public static unsafe ref IUnknownUnsafe CreateUnsafeRef( DXSharp.Windows.COM.IUnknown pUnk ) {
		void* p = &pUnk ;
		var unk = *( (IUnknownUnsafe*)p ) ;
		
		return ref unk ;
	}

	public static unsafe ref IUnknown GetIUnknownStructRef( DXSharp.Windows.COM.IUnknown pUnk ) {
		void* p = &pUnk ;
		var unk = *( (IUnknown *)p ) ;
		
		return ref unk ;
	}
	
	
	// -----------------------------------------------------------------
	/// <summary>The IID guid for this interface.</summary>
	/// <value>{00000000-0000-0000-c000-000000000046}</value>
	public static readonly Guid IID_Guid =
		new Guid( 0x00000000, 0x0000, 0x0000, 0xC0, 0x00, 
				  0x00, 0x00, 0x00, 0x00, 0x00, 0x46 ) ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = new byte[]
			{
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46
			} ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal.GetReference( data ) ) ;
		}
	}
	// =================================================================
} ;

