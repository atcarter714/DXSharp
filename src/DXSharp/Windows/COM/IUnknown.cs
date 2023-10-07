#region Using Directives
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.Graphics.Dxgi ;
using static DXSharp.Windows.HResult ;
#endregion

namespace DXSharp.Windows.COM ;


//[ComImport, Guid( "00000000-0000-0000-C000-000000000046" )]
[InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
public unsafe interface IUnknown {
	IntPtr Pointer { get; }
	[PreserveSig] int QueryInterface( ref Guid riid, out IntPtr ppvObject ) ;
	[PreserveSig] uint AddRef( ) => (uint)Marshal.AddRef( Pointer ) ;
	[PreserveSig] uint Release( ) => (uint)Marshal.Release( Pointer ) ;
} ;



/// <summary>Contract for .NET objects wrapping native COM types.</summary>
public interface IUnknownWrapper: IUnknown,
								  IDisposable,
								  IAsyncDisposable {
	bool Disposed { get; }
	HResult QueryInterface< T >( out T ppvObject ) where T : IUnknown {
		var riid = typeof(T).GUID ;
		
		var hr = (HResult)Marshal
			.QueryInterface( Pointer, ref riid, out var ppCOMObj ) ;
		_ = hr.ThrowOnFailure( ) ;
		
		object comObjectRef = Marshal.GetUniqueObjectForIUnknown( ppCOMObj ) ;
		ppvObject = (T)comObjectRef ;
		
		return hr ;
	}
} ;

/// <summary>Contract for COM object wrapper.</summary>
public interface IUnknown< TSelf >: IUnknownWrapper
									where TSelf: IUnknown< TSelf >,
												 IUnknownWrapper {
	internal ComPtr< IUnknown >? ComPtr { get ; }
	new IntPtr Pointer => ComPtr?.Address ?? IntPtr.Zero ;
} ;
