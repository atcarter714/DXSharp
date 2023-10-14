#region Using Directives
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.DXGI ;
using static DXSharp.Windows.HResult ;
#endregion
namespace DXSharp.Windows.COM ;


// -----------------------------------------------------------------
// DXSharp Wrapper Interfaces:
// -----------------------------------------------------------------

/// <summary>Base interface of all DXSharp COM wrapper types.</summary>
public interface IUnknownWrapper: IDisposable,
								  IAsyncDisposable {
	bool Disposed { get; }
	internal int RefCount { get ; }
	internal ComPtr? ComPtrBase { get; }
	internal nint BasePointer => ComPtrBase?.BaseAddress ?? 0x0000 ;
	
	uint AddRef( ) => (uint)Marshal.AddRef( BasePointer ) ;
	uint Release( ) => (uint)Marshal.Release( BasePointer ) ;
	HResult QueryInterface< T >( out nint ppvInterface ) where T: IUnknown =>
		COMUtility.QueryInterface< T >( BasePointer, out ppvInterface ) ;
} ;


/// <summary>Contract for COM object wrapper.</summary>
/// <typeparam name="TInterface">The native COM interface type.</typeparam>
public interface IUnknownWrapper< TInterface >: IUnknownWrapper
												where TInterface: IUnknown {
	public static virtual Guid InterfaceGUID => typeof(TInterface).GUID ;
	internal Type ComType => typeof(TInterface) ;
	
	/// <summary>ComPtr to the native <typeparam name="TInterface"/> COM interface.</summary>
	ComPtr< TInterface >? ComPointer { get ; }
	ComPtr IUnknownWrapper.ComPtrBase => ComPointer! ;
	internal TInterface? ComObject => ComPointer!.Interface ;
	internal nint Pointer => ComPointer?.BaseAddress ?? nint.Zero ;
	
	/// <summary>Indicates if this instance is fully initialized.</summary>
	bool IsInitialized => ( ComPointer is not null )
							&& ComPointer.InterfaceVPtr.IsValid( )
								&& ComPointer.Interface is not null ;
	
	
	internal void SetComPointer( ComPtr< TInterface >? otherPtr ) =>
									ComPointer?.Set( otherPtr!.Interface! ) ;
} ;


// <summary>Contract for .NET objects wrapping native COM types.</summary>
public interface IUnknownWrapper< TSelf, TInterface >: IUnknownWrapper< TInterface > 
									where TSelf: IUnknownWrapper< TSelf, TInterface >
									where TInterface: IUnknown { } ;
