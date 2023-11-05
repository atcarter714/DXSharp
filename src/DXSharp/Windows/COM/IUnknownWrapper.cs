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
								  IAsyncDisposable, IComIID {
	/// <summary>The type of COM runtime proxy interface.</summary>
	public static virtual Type ComType => typeof( IUnknown ) ;
	
	bool Disposed { get; }
	internal ComPtr? ComPtrBase { get; }
	internal int RefCount => ( ComPtrBase?.RefCount ?? 0 ) ;
	internal nint BasePointer => ComPtrBase?.BaseAddress ?? 0x0000 ;
	
	/// <summary>Indicates if this instance is fully initialized.</summary>
	bool IsInitialized => ( ComPtrBase is { Disposed: false } ) ;
	
	uint AddReference( ) => (uint)Marshal.AddRef( BasePointer ) ;
	uint ReleaseReference( ) => (uint)Marshal.Release( BasePointer ) ;
} ;


/// <summary>Contract for COM object wrapper.</summary>
/// <typeparam name="TInterface">The native COM interface type.</typeparam>
public interface IUnknownWrapper< TInterface >: IUnknownWrapper
												where TInterface: IUnknown {
	static Type IUnknownWrapper.ComType => typeof(TInterface) ;
	//static Guid IUnknownWrapper.InterfaceGUID => typeof(TInterface).GUID ;
	
	/// <summary>A reference to the IID guid for this interface.</summary>
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(TInterface).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal.GetReference(data) ) ;
		}
	}
	
	/// <summary>ComPtr of the native COM interface.</summary>
	ComPtr? ComPointer { get ; }
	ComPtr? IUnknownWrapper.ComPtrBase => ComPointer ;
	
	internal void SetComPointer( ComPtr< TInterface >? otherPtr ) =>
									ComPointer?.Set( otherPtr!.Interface! ) ;
} ;
