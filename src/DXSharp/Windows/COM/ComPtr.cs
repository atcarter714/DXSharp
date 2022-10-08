#region Using Directives
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
#endregion

namespace DXSharp.Windows.COM;



/// <summary>
/// Wraps a COM object interface
/// </summary>
/// <typeparam name="T">Type of COM interface</typeparam>
internal class ComPtr<T> : IDisposable, IAsyncDisposable
	where T : class
{
	ComPtr() => GUID = typeof(T).GUID;

	internal ComPtr( T comObj ): this()
	{
#if DEBUG || !STRIP_CHECKS
		if (comObj is null)
			throw new ArgumentNullException("comObj");

		if (!Marshal.IsComObject(comObj))
			throw new COMException($"ComPtr<{typeof(T).Name}>( comObj ): " +
				@$"The parameter ""{comObj}"" is not a valid COM interface object!");
#endif

		IntPtr pComObj = Marshal.GetIUnknownForObject(comObj);

		if (pComObj == IntPtr.Zero)
			throw new COMException($"ComPtr< {typeof(T).Name} >( {typeof(T).Name} comObj ): " +
				$"Unable to obtain pointer to IUnknown from {comObj}!");

		// Assign our valid data:
		this.Interface = comObj;
		this.Pointer = pComObj;
	}

	internal ComPtr( IntPtr pComObj ): this()
	{

		if (pComObj == IntPtr.Zero)
			throw new COMException($"ComPtr<{typeof(T).Name}>( IntPtr pComObj ): " +
				"The given address is a null pointer!");

		var objRef = Marshal.GetObjectForIUnknown(pComObj);

		if (objRef is null)
			throw new COMException($"ComPtr< {typeof(T).Name} >( {typeof(T).Name} comObj ): " +
				$"Unable to obtain object reference to COM interface from 0x{Pointer.ToString("X8")}!");

		// Assign our valid data:
		this.Interface = objRef as T ??
			throw new COMException($"ComPtr< {typeof(T).Name} >( {typeof(T).Name} comObj ): " +
				$"Unable to convert COM object reference at 0x{Pointer.ToString("X8")} to COM interface type {typeof(T).Name}!");

		this.Pointer = pComObj;
	}

	~ComPtr() => Dispose(disposing: false);



	/// <summary>
	/// The underlying COM interface
	/// </summary>
	internal T? Interface { get; private set; }

	/// <summary>
	/// Pointer to the underlying COM interface
	/// </summary>
	internal IntPtr Pointer { get; private set; }

	/// <summary>
	/// The GUID of the COM interface
	/// </summary>
	internal Guid GUID { get; private set; }

	/// <summary>
	/// Indicates if this instance has been disposed
	/// and released its resources and memory
	/// </summary>
	public bool Disposed => disposedValue;
	private bool disposedValue = false;


	/// <summary>
	/// Disposes of this instance's resources
	/// </summary>
	/// <param name="disposing">
	/// Indicates if managed resources should be
	/// disposed of from this call
	/// </param>
	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				// TODO: dispose managed state (managed objects)
			}

			// TODO: free unmanaged resources (unmanaged objects) and override finalizer
			// TODO: set large fields to null

			if (Interface is not null)
			{
				int refCount = Marshal.ReleaseComObject(Interface);
			}
			else if (Pointer != IntPtr.Zero)
			{
				int refCount = Marshal.Release(this.Pointer);
			}

			this.Interface = null;
			this.Pointer = IntPtr.Zero;
			this.GUID = Guid.Empty;

			disposedValue = true;
		}
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	async ValueTask DisposeAsyncCore() { }

	public async ValueTask DisposeAsync()
	{
		// Perform async cleanup.
		await DisposeAsyncCore();

		// Dispose of unmanaged resources.
		Dispose(false);

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
		// Suppress finalization.
		GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
	}
};
