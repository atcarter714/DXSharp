#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices;
using Windows.Win32 ;
#endregion
namespace DXSharp.Windows.COM ;


// -----------------------------------------------------------------
// DXSharp Wrapper Interfaces:
// -----------------------------------------------------------------

/// <summary>Base interface of all DXSharp COM wrapper types.</summary>
public interface IUnknownWrapper: IDisposable,
								  IAsyncDisposable, 
								  IComIID {
	// -----------------------------------------------------------------
	/// <summary>Indicates if the instance has disposed of its COM resources.</summary>
	bool Disposed { get; }
	
	/// <summary>Reference to the internal <see cref="ComPtr"/> for the COM resource.</summary>
	internal ComPtr? ComPtrBase { get; }
	
	/// <summary>Gets the reference count for the COM resource.</summary>
	internal int RefCount => ( ComPtrBase?.RefCount ?? 0 ) ;
	
	/// <summary>Gets the base <see cref="IUnknown"/> pointer of the COM resource.</summary>
	internal nint BasePointer => ComPtrBase?.BaseAddress ?? 0x0000 ;
	
	/// <summary>Indicates if this instance is fully initialized.</summary>
	bool IsInitialized => ( ComPtrBase is { Disposed: false } ) ;
	// -----------------------------------------------------------------
	
	/// <summary>Increments the reference count on the COM interface.</summary>
	/// <returns>The new reference count on the COM interface.</returns>
	uint AddReference( ) => (uint)Marshal.AddRef( BasePointer ) ;
	
	/// <summary>Decrements the reference count on the COM interface.</summary>
	/// <returns>The new reference count on the COM interface.</returns>
	uint ReleaseReference( ) => (uint)Marshal.Release( BasePointer ) ;
	
	// -----------------------------------------------------------------
	/// <summary>The type of managed COM (RCW) interface for the native COM interface.</summary>
	public static virtual Type ComType => typeof( IUnknown ) ;
	// =================================================================
} ;


/// <summary>Contract for COM object wrapper.</summary>
/// <typeparam name="TInterface">The native COM interface type.</typeparam>
public interface IUnknownWrapper< TInterface >: IUnknownWrapper
												where TInterface: IUnknown {
	// -----------------------------------------------------------------
	/// <inheritdoc cref="IUnknownWrapper.ComType"/>
	static Type IUnknownWrapper.ComType => typeof(TInterface) ;
	
	/// <summary>A reference to the IID guid for this interface.</summary>
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(TInterface).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal.GetReference(data) ) ;
		}
	}
	
	/// <summary>
	/// A <see cref="ComPtr{T}"/> with addresses and references to
	/// the native COM interface and RCW object(s).
	/// </summary>
	ComPtr? ComPointer { get ; }
	
	/// <inheritdoc cref="IUnknownWrapper.ComPtrBase"/>
	ComPtr? IUnknownWrapper.ComPtrBase => ComPointer ;
	
	// =================================================================
} ;
