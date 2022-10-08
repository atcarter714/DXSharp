using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using DXSharp;
using DXSharp.DXGI;
using DXSharp.DXGI.XTensions;

using DXSharp.Windows;
using DXSharp.Windows.COM;

namespace DXSharp.Windows.COM;

public class COMBaseObject<T> : IUnknown, IAsyncDisposable
{
	// TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
	~COMBaseObject()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: false);
	}
	
	private bool disposedValue;
	public IntPtr Pointer { get; set; }
	
	internal virtual ComPtr<object>? COMPointer { get; set; }

	#region Disposable Pattern

	public bool Disposed { get; }

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
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	async ValueTask DisposeAsyncCore()
	{

	}

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

	#endregion

	//internal T As<T>() where T: BaseCOM
}


