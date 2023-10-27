#region Using Directives

using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices;
using Windows.Win32 ;
using DXSharp.Direct3D12 ;

#endregion
namespace DXSharp.Windows.COM ;


// -----------------------------------------------------------------
// DXSharp Wrapper Interfaces:
// -----------------------------------------------------------------

/// <summary>Base interface of all DXSharp COM wrapper types.</summary>
public interface IUnknownWrapper: IDisposable,
								  IAsyncDisposable {
	/// <summary>The type of COM runtime proxy interface.</summary>
	public static virtual Type ComType => typeof( IUnknown ) ;
	/// <summary>The GUID (<b>IID</b>) of the COM runtime proxy interface.</summary>
	public static virtual Guid InterfaceGUID => typeof( IUnknown ).GUID ;
	
	bool Disposed { get; }
	internal int RefCount { get ; }
	internal ComPtr? ComPtrBase { get; }
	internal nint BasePointer => ComPtrBase?.BaseAddress ?? 0x0000 ;
	
	/// <summary>Indicates if this instance is fully initialized.</summary>
	bool IsInitialized => ( ComPtrBase is { Disposed: false } ) ;
	
	uint AddRef( ) => (uint)Marshal.AddRef( BasePointer ) ;
	uint Release( ) => (uint)Marshal.Release( BasePointer ) ;
	
	/*HResult QueryInterface< T >( out nint ppvInterface ) where T: IUnknown =>
		COMUtility.QueryInterface< T >( BasePointer, out ppvInterface ) ;*/
	
} ;


/// <summary>Contract for COM object wrapper.</summary>
/// <typeparam name="TInterface">The native COM interface type.</typeparam>
public interface IUnknownWrapper< TInterface >: IUnknownWrapper, IComIID
												where TInterface: IUnknown {
	static Type IUnknownWrapper.ComType => typeof(TInterface) ;
	static Guid IUnknownWrapper.InterfaceGUID => typeof(TInterface).GUID ;
	
	/// <summary>A reference to the IID guid for this interface.</summary>
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(TInterface).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal.GetReference(data) ) ;
		}
	}
	
	/// <summary>ComPtr of the native COM interface.</summary>
	ComPtr< TInterface >? ComPointer { get ; }
	
	
	internal void SetComPointer( ComPtr< TInterface >? otherPtr ) =>
									ComPointer?.Set( otherPtr!.Interface! ) ;
} ;



// <summary>Contract for .NET objects wrapping native COM types.</summary>
/*public interface IUnknownWrapper< TSelf, TInterface >: IUnknownWrapper< TInterface > 
									where TSelf: IUnknownWrapper< TSelf, TInterface >
									where TInterface: IUnknown { } ;*/

	//ComPtr IUnknownWrapper.ComPtrBase => ComPointer! ;
	//internal TInterface? ComObject => ComPointer!.Interface ;
	//internal nint Pointer => ComPointer?.BaseAddress ?? nint.Zero ;