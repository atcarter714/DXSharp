// NOTE: Commenting this class out as it is not used and is not complete.
// This was part of the original DXSharp project's experimental code.

//#region Using Directives
///* Unmerged change from project 'DXSharp (net7.0-windows10.0.22621.0)'
//Before:
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;

//using DXSharp;
//using DXSharp.DXGI;
//using DXSharp.DXGI.XTensions;

//using DXSharp.Windows;
//using DXSharp.Windows.COM;
//After:
//using DXSharp;
//using DXSharp.DXGI;
//using DXSharp.DXGI.XTensions;
//using DXSharp.Windows;
//using DXSharp.Windows.COM;

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;
//*/
//#endregion

//namespace DXSharp.Windows.COM;



//public class COMBaseObject<T>: IUnknown, IAsyncDisposable where T : class
//{
//	// TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
//	~COMBaseObject() {
//		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
//		Dispose( disposing: false );
//	}

//	/// <inheritdoc/>
//	public IntPtr Pointer { get; set; }

//	/// <summary>
//	/// A ComPtr "smart pointer" to a native COM interface
//	/// </summary>
//	internal virtual ComPtr<T>? COMPointer { get; set; }

//	#region Disposable Pattern

//	/// <inheritdoc/>
//	public bool Disposed { get; }
//	private bool disposedValue;

//	protected virtual void Dispose( bool disposing ) {
//		if( !disposedValue ) {
//			if( disposing ) {
//				// TODO: dispose managed state (managed objects)
//			}

//			// TODO: free unmanaged resources (unmanaged objects) and override finalizer
//			// TODO: set large fields to null
//			disposedValue = true;
//		}
//	}

//	/// <inheritdoc/>
//	public void Dispose() {
//		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
//		Dispose( disposing: true );
//		GC.SuppressFinalize( this );
//	}

//	async ValueTask DisposeAsyncCore() {

//	}

//	/// <summary>
//	/// Asynchronously disposes of this instance's resources
//	/// </summary>
//	/// <returns>A ValueTask</returns>
//	public async ValueTask DisposeAsync() {
//		// Perform async cleanup.
//		await DisposeAsyncCore();

//		// Dispose of unmanaged resources.
//		Dispose( false );

//#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
//		// Suppress finalization.
//		GC.SuppressFinalize( this );
//#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
//	}

//	#endregion

//	//internal T As<T>() where T: BaseCOM
//}


