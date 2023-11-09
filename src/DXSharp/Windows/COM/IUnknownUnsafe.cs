#pragma warning disable CS1591,CS1573,CS0465,CS0649,CS8019,CS1570,CS1584,CS1658,CS0436,CS8981

#region Using Directives
using System.Diagnostics.CodeAnalysis ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using System.Runtime.InteropServices.Marshalling ;
using Windows.Foundation.Metadata ;
using Windows.Win32.Foundation ;

using DXSharp.Windows ;
using static DXSharp.InteropUtils ;
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


[global::System.Runtime.InteropServices.Guid( "00000000-0000-0000-C000-000000000046" )]
public unsafe partial struct IUnknownUnsafe: DXSharp.Windows.COM.IUnknown,
											 IComIID {
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
	public struct VTable: IEquatable< VTable > {
		// ---------------------------------------------------------------------------
		//! QueryInterface Function Pointer:
		internal delegate *unmanaged [Stdcall]< IUnknown*, Guid*, void**, HRESULT > 
			QueryInterface_1 ;
		
		//! AddRef Function Pointer:
		internal delegate *unmanaged [Stdcall]< IUnknown*, uint > 
			AddRef_2 ;

		//! Release Function Pointer:
		internal delegate *unmanaged [Stdcall]< IUnknown*, uint > 
			Release_3 ;
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
			HRESULT hr = default ;
			fixed( void* p = &this ) {
				hr = QueryInterface_1( (IUnknown *)p, &guid, &fn ) ;
				if( fn is not null ) ppvObject = (nint)fn! ;
				return hr ;
			}
		}
		
		public uint InvokeAddRef( ) {
			void* fn   = null ;
			uint count = 0x00U ;
			fixed( void* p = &this ) {
				count = AddRef_2( (IUnknown *)p ) ;
				return count ;
			}
		}

		public uint InvokeRelease( ) {
			void* fn    = null ;
			uint  count = 0x00U ;
			fixed( void* p = &this ) {
				count = Release_3( (IUnknown *)p ) ;
				return count ;
			}
		}
		// ---------------------------------------------------------
		
		
		/// <summary>
		/// Gets a managed <see cref="AddRefDelegate"/> wrapper for the COM interface method <c>HRESULT AddRef( )</c>.
		/// </summary>
		/// <returns>
		/// An <see cref="AddRefDelegate"/> delegate that invokes the native <c>HRESULT AddRef( )</c> method.
		/// </returns>
		[MethodImpl(_MAXOPT_)] internal AddRefDelegate GetAddRefDelegate( ) =>
			Marshal.GetDelegateForFunctionPointer< AddRefDelegate >( (nint)AddRef_2 ) ;
		
		/// <summary>
		/// Gets a managed <see cref="ReleaseDelegate"/> wrapper for the COM interface method <c>HRESULT Release( )</c>.
		/// </summary>
		/// <returns>
		/// An <see cref="ReleaseDelegate"/> delegate that invokes the native <c>HRESULT Release( )</c> method.
		/// </returns>
		[MethodImpl(_MAXOPT_)] internal ReleaseDelegate GetReleaseDelegate( ) =>
		 			Marshal.GetDelegateForFunctionPointer< ReleaseDelegate >( (nint)Release_3 ) ;
		
		/// <summary>
		/// Gets a managed <see cref="QueryInterfaceDelegate"/> wrapper for the COM interface method <c>HRESULT QueryInterface(REFIID, void**)</c>.
		/// </summary>
		/// <returns>
		/// An <see cref="QueryInterfaceDelegate"/> delegate that invokes the native <c>HRESULT QueryInterface(REFIID, void**)</c> method.
		/// </returns>
		[MethodImpl(_MAXOPT_)] internal QueryInterfaceDelegate GetQueryInterfaceDelegate( ) =>
		 			Marshal.GetDelegateForFunctionPointer< QueryInterfaceDelegate >( (nint)QueryInterface_1 ) ;


		#region System Overrides
		public bool Equals( VTable other ) =>
			QueryInterface_1 == other.QueryInterface_1
					&& AddRef_2 == other.AddRef_2
						&& Release_3 == other.Release_3 ;
		public override bool Equals( object? obj ) => 
				obj is VTable other && Equals( other ) ;
		public override int GetHashCode( ) =>
					HashCode.Combine( (nint)QueryInterface_1,
										  (nint)AddRef_2,
											(nint)Release_3 ) ;
		#endregion
		
		
		
		public static explicit operator VTable( nint addr ) {
			VTable* vtbl = (VTable*)addr ;
			return *vtbl ;
		}
		
		// --------------------------------------------------------------
		public static bool operator !=( in VTable left, in VTable right ) => !( left == right ) ;
		public static bool operator ==( in VTable left, in VTable right ) => 
												left.AddRef_2 == right.AddRef_2 
													&& left.Release_3 == right.Release_3 
														&& left.QueryInterface_1 == right.QueryInterface_1 ;
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
	[UnscopedRef] public ref void** pVTableRef {
		[MethodImpl( MethodImplOptions.AggressiveInlining )] get {
			fixed ( void* fieldAddress = &lpVtbl ) {
				ref void** fieldRef = ref *( (void***)( fieldAddress ) ) ;
				return ref fieldRef ;
			}
		}
	}
	
	// -----------------------------------------------------------------

	
	public unsafe HRESULT QueryInterface( in Guid riid, out void* ppvObject ) {
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
	public unsafe HRESULT QueryInterface( Guid* riid, void** ppvObject ) {
		return ( (delegate *unmanaged [Stdcall]<IUnknown*, Guid*, void**, HRESULT>)
				   lpVtbl[ 0 ] )( (IUnknown*)Unsafe.AsPointer( ref this ), riid, ppvObject ) ;
	}

	
	/// <summary>Increments the reference count for an interface pointer to a COM object. You should call this method whenever you make a copy of an interface pointer.</summary>
	/// <returns>The method returns the new reference count. This value is intended to be used only for test purposes.</returns>
	/// <remarks>
	/// <para>A COM object uses a per-interface reference-counting mechanism to ensure that the object doesn't outlive references to it. You use **AddRef** to stabilize a copy of an interface pointer. It can also be called when the life of a cloned pointer must extend beyond the lifetime of the original pointer. The cloned pointer must be released by calling [IUnknown::Release](/windows/desktop/api/unknwn/nf-unknwn-iunknown-queryinterface(refiid_void)) on it. The internal reference counter that **AddRef** maintains should be a 32-bit unsigned integer.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/unknwn/nf-unknwn-iunknown-addref#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	public uint AddRef( ) {
		return ((delegate *unmanaged [Stdcall]<IUnknown*,uint>)lpVtbl[1])((IUnknown*)Unsafe.AsPointer(ref this));
		/*return
			( (delegate *unmanaged [Stdcall]<IUnknown*, uint>)lpVtbl[ 1 ] )
				( (IUnknown *)Unsafe.AsPointer( ref this ) ) ;*/
	}

	/// <summary>Decrements the reference count for an interface on a COM object.</summary>
	/// <returns>The method returns the new reference count. This value is intended to be used only for test purposes.</returns>
	/// <remarks>
	/// <para>When the reference count on an object reaches zero, **Release** must cause the interface pointer to free itself. When the released pointer is the only (formerly) outstanding reference to an object (whether the object supports single or multiple interfaces), the implementation must free the object. Note that aggregation of objects restricts the ability to recover interface pointers.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/unknwn/nf-unknwn-iunknown-release#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	public uint Release( ) {
		return
			( (delegate *unmanaged [Stdcall]<IUnknown*, uint>)lpVtbl[ 2 ] )( (IUnknown*)Unsafe.AsPointer( ref this ) ) ;
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

