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
	internal nint BasePointer { get; }
	
	uint AddRef( ) => (uint)Marshal.AddRef( BasePointer ) ;
	uint Release( ) => (uint)Marshal.Release( BasePointer ) ;
	
	HResult QueryInterface< T >( out nint ppvInterface ) where T: IUnknown =>
		COMUtility.QueryInterface< T >( BasePointer, out ppvInterface ) ;
} ;



/// <summary>Contract for COM object wrapper.</summary>
public interface IUnknownWrapper< TInterface >: IUnknownWrapper 
												where TInterface: IUnknown {
	public static virtual Guid InterfaceGUID => typeof(TInterface).GUID ;
	
	Type ComType => typeof(TInterface) ;
	ComPtr< TInterface >? ComPointer { get ; }
	internal TInterface? ComObject => ComPointer!.Interface ;
	internal nint Pointer => ComPointer?.BaseAddress ?? nint.Zero ;
	
	/// <summary>Indicates if the DXSharp object is fully initialized.</summary>
	bool IsInitialized => (ComPointer is not null)
							&& ComPointer.InterfaceVPtr.IsValid()
								&& ComPointer.Interface is not null ;
	
	
	internal void SetComPointer( ComPtr< TInterface >? comPtr ) =>
									ComPointer?.Set( comPtr!.Interface! ) ;
	
	HResult QueryInterface< T >( out T pInterface ) where T: IUnknown => 
		COMUtility.QueryInterface< T >( Pointer, out pInterface ) ;
} ;


/// <summary>Contract for .NET objects wrapping native COM types.</summary>
public interface IUnknownWrapper< TSelf, TInterface >: IUnknownWrapper< TInterface > 
									where TSelf: IUnknownWrapper<TSelf, TInterface>
									where TInterface: IUnknown {
	static Type WrapperType => typeof(TSelf) ;
} ;

