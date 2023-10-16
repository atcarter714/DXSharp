// ------------------------------------------------------------------------------
// 
//
// ------------------------------------------------------------------------------

#region Using Directives
#pragma warning disable CS1591,CS1573,CS0465,CS0649,CS8019,CS1570,CS1584,CS1658,CS0436,CS8981
using System ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32.Foundation ;
using winmdroot = Windows.Win32 ;
#endregion
namespace Windows.Win32.System.Com ;


/// <summary>
/// Provides an unmanaged (<see langword="struct" />) helper type for working with COM interfaces
/// in interop scenarios where pointers to interfaces or structures with interface fields are used.
/// </summary>
[Guid( "00000000-0000-0000-C000-000000000046" )]
public unsafe struct IUnknownUnmanaged: IComIID {
	
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
		return
			( (delegate *unmanaged [Stdcall]<IUnknownUnmanaged*, Guid*, void**, HRESULT>)
				lpVtbl[ 0 ] )( (IUnknownUnmanaged*)Unsafe.AsPointer(ref this), 
							  riid,
							  ppvObject ) ;
	}

	
	/// <summary>Increments the reference count for an interface pointer to a COM object. You should call this method whenever you make a copy of an interface pointer.</summary>
	/// <returns>The method returns the new reference count. This value is intended to be used only for test purposes.</returns>
	/// <remarks>
	/// <para>A COM object uses a per-interface reference-counting mechanism to ensure that the object doesn't outlive references to it. You use **AddRef** to stabilize a copy of an interface pointer. It can also be called when the life of a cloned pointer must extend beyond the lifetime of the original pointer. The cloned pointer must be released by calling [IUnknown::Release](/windows/desktop/api/unknwn/nf-unknwn-iunknown-queryinterface(refiid_void)) on it. The internal reference counter that **AddRef** maintains should be a 32-bit unsigned integer.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/unknwn/nf-unknwn-iunknown-addref#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	public uint AddRef( ) {
		return
			( (delegate *unmanaged [Stdcall]<IUnknownUnmanaged*, uint>)
				lpVtbl[ 1 ] )( (IUnknownUnmanaged*)Unsafe.AsPointer(ref this) ) ;
	}

	
	/// <summary>Decrements the reference count for an interface on a COM object.</summary>
	/// <returns>The method returns the new reference count. This value is intended to be used only for test purposes.</returns>
	/// <remarks>
	/// <para>When the reference count on an object reaches zero, **Release** must cause the interface pointer to free itself. When the released pointer is the only (formerly) outstanding reference to an object (whether the object supports single or multiple interfaces), the implementation must free the object. Note that aggregation of objects restricts the ability to recover interface pointers.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/unknwn/nf-unknwn-iunknown-release#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	public uint Release( ) {
		return
			( (delegate *unmanaged [Stdcall]<IUnknownUnmanaged*, uint>)
				lpVtbl[ 2 ] )( (IUnknownUnmanaged *)Unsafe.AsPointer(ref this) ) ;
	}

	
	public struct Vtbl {
		internal delegate *unmanaged [Stdcall]
			<IUnknownUnmanaged*, Guid*, void**, HRESULT> QueryInterface_1 ;

		internal delegate *unmanaged [Stdcall]<IUnknownUnmanaged*, uint> AddRef_2 ;
		internal delegate *unmanaged [Stdcall]<IUnknownUnmanaged*, uint> Release_3 ;
	} ;

	void** lpVtbl ;
	
	// --------------------------------------------------------------------------------------------

	/// <summary>The IID guid for this interface.</summary>
	/// <value>{00000000-0000-0000-c000-000000000046}</value>
	public static readonly Guid IID_Guid =
		new( 0x00000000, 0x0000, 0x0000, 0xC0, 0x00,
			 0x00, 0x00, 0x00, 0x00, 0x00, 0x46 ) ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = new byte[ ] {
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
				0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46
			} ;
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal.GetReference( data ) ) ;
		}
	}
	
	// ============================================================================================
} ;