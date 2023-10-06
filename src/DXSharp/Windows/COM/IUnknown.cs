#region Using Directives
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using static DXSharp.Windows.HRESULT ;
#endregion

namespace DXSharp.Windows.COM ;

[ComImport, Guid( "00000000-0000-0000-C000-000000000046" )]
[InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
public unsafe interface IUnknown
{
	[PreserveSig]
	int QueryInterface( ref Guid riid, out IntPtr ppvObject );

	[PreserveSig]
	uint AddRef();

	[PreserveSig]
	uint Release();
};

public interface IUnknownWrapper: IDisposable, IAsyncDisposable
{
	IntPtr Pointer { get; }
	bool Disposed { get; }

	uint AddRef() => (uint)Marshal.AddRef( Pointer );
	uint Release() => (uint)Marshal.Release( Pointer );

	HRESULT QueryInterface<T>( out T ppvObject ) where T : IUnknown {
		var riid = typeof(T).GUID;
		var hr = (HRESULT)Marshal.QueryInterface(Pointer, ref riid, out var ppObj);

		_ = hr.ThrowOnFailure();

		object comObjectRef = Marshal.GetUniqueObjectForIUnknown(ppObj);

		ppvObject = (T)comObjectRef;
		return hr;
	}
};

public interface IUnknown<T>: IUnknownWrapper where T : class
{
	internal ComPtr<T>? ComPtr { get; }

	new IntPtr Pointer => (ComPtr is null) ? IntPtr.Zero : ComPtr.Address ;
};
