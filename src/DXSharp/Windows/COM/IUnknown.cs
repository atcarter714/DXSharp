#region Using Directives
using global::System;
using global::System.Runtime.InteropServices;
using global::System.Runtime.InteropServices.ComTypes;
using global::System.Runtime.InteropServices.WindowsRuntime;

using global::Windows.Win32;
using Windows.Win32.Foundation;
using Win32 = global::Windows.Win32;
#endregion

namespace DXSharp.Windows.COM;

//interface IUnknown
//{
//	virtual HRESULT QueryInterface(REFIID riid, void** ppvObject) = 0;
//	virtual ULONG AddRef() = 0;
//	virtual ULONG Release() = 0;
//};

/// <summary>
/// Wrapper of the base IUnknown COM interface
/// </summary>
/// <remarks>
/// More info on COM IUnknown interface can be found
/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/unknwn/nn-unknwn-iunknown">here</a>.
/// </remarks>
//[ComImport, Guid("00000000-0000-0000-C000-000000000046")]
public interface IUnknown : IDisposable
{
	/// <summary>
	/// Gets the address of the underlying IUnknown COM interface
	/// </summary>
	IntPtr Pointer { get; }

	/// <summary>
	/// Indicates if this instance has disposed of
	/// its native COM resources
	/// </summary>
	bool Disposed { get; }



	/// <summary>
	/// Increments the reference count for an interface pointer to a COM object. 
	/// You should call this method whenever you make a copy of an interface pointer.
	/// </summary>
	/// <returns>Updated reference count</returns>
	uint AddRef() => (uint)Marshal.AddRef(Pointer);

	/// <summary>
	/// Decrements the reference count for an interface on a COM object.
	/// </summary>
	/// <returns>Updated reference count</returns>
	uint Release() => (uint)Marshal.Release(Pointer);

	/// <summary>
	/// Retrieves reference to the supported interfaces on an object.
	/// </summary>
	/// <typeparam name="T">Type of COM interface</typeparam>
	/// <param name="ppvObject">object to hold COM interface reference</param>
	/// <returns>A Win32 HRESULT error code indicating success or failure</returns>
	/// <remarks>
	/// Wrapper of native <a href="https://learn.microsoft.com/en-us/windows/win32/api/unknwn/nf-unknwn-iunknown-queryinterface(refiid_void)">QueryInterface</a>(REFIID,void) function.
	/// </remarks>
	HRESULT QueryInterface<T>(out T ppvObject) where T : IUnknown
	{
		var riid = typeof(T).GUID;
		HRESULT hr = (HRESULT)Marshal.QueryInterface(
			Pointer, ref riid, out var ppObj);

		hr.ThrowOnFailure();
		//if (hr.Failed)
		//	throw new COMException("ERROR: Call to QueryInterface failed!", hr);

		object comObjectRef = Marshal.GetUniqueObjectForIUnknown(ppObj);
		//if (comObjectRef is null)
		//	throw new COMException("ERROR: Call to QueryInterface failed!");

		ppvObject = (T)comObjectRef;
		return hr;
	}
};