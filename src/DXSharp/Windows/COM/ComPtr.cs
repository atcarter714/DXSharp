// COPYRIGHT NOTICES:
// --------------------------------------------------------------------------------
// Copyright © 2022 DXSharp - ATC - Aaron T. Carter
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// -----------------------------------------------------------------------------



#region Using Directives
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
#endregion

namespace DXSharp.Windows.COM;



/// <summary>
/// Base class of COM object pointers which provides a somewhat
/// similar set of functionality to the native 
/// <a href="https://learn.microsoft.com/en-us/cpp/cppcx/wrl/comptr-class?view=msvc-170"><c>wrl::ComPtr&lt;T&gt;</c></a> template 
/// class used in C++ COM programming, like DirectX.
/// </summary>
/// <remarks>
/// The <c>ComPtr</c> type is a type of "smart pointer" which holds the actual memory address of a COM object
/// and helps automatically relinquish native resources when it is <i>disposed</i> or goes out of scope. It 
/// also provides methods for reference counting and COM interface management functionality.
/// </remarks>
internal class ComPtr: IDisposable, IAsyncDisposable
{
	internal ComPtr( object comObj ) {
#if DEBUG || !STRIP_CHECKS
		if ( comObj is null )
			throw new ArgumentNullException( "comObj" );

		if ( !Marshal.IsComObject(comObj) )
			throw new COMException( $"ComPtr( {comObj.GetType().Name} {nameof(comObj)} ): " +
				@$"The parameter ""{nameof(comObj)}"" ({comObj.GetType().Name}) is not a valid COM object!" );
#endif

		// Get pointer to IUnknown interface:
		IntPtr pComObj = getPointerTo( comObj );

		// Assign our valid data:
		this.Interface = comObj;
		this.Pointer = pComObj;
		this.GUID = comObj.GetType().GUID;
	}

	internal ComPtr( IntPtr pComObj ) {

		if ( pComObj == IntPtr.Zero )
			throw new COMException( $"ComPtr< object >( IntPtr pComObj ): " +
				"The given address is a null pointer!" );

		// Get object ref to COM interface:
		var comObj = getCOMObjectFrom( pComObj );
		
		// Assign our valid data:
		this.Interface = comObj;
		this.Pointer = pComObj;
		this.GUID = comObj.GetType().GUID;
	}

	~ComPtr() => Dispose( disposing: false );

	/// <summary>
	/// The underlying COM interface
	/// </summary>
	internal virtual object? Interface { get; private set; }

	/// <summary>
	/// Pointer to the underlying COM interface
	/// </summary>
	internal IntPtr Pointer { get; private set; }

	/// <summary>
	/// The GUID of the COM interface
	/// </summary>
	internal Guid GUID { get; private protected set; }



	#region IDisposable/IAsyncDisposable

	/// <summary>
	/// Indicates if this instance has been disposed
	/// and released its resources and memory
	/// </summary>
	public bool Disposed => disposedValue;
	bool disposedValue = false;

	/// <summary>
	/// Disposes of this instance's resources
	/// </summary>
	/// <param name="disposing">
	/// Indicates if managed resources should be
	/// disposed of from this call
	/// </param>
	protected virtual void Dispose( bool disposing )
	{
		if ( !disposedValue )
		{
			if ( disposing )
			{
				// TODO: dispose managed state (managed objects)
			}

			// TODO: free unmanaged resources (unmanaged objects) and override finalizer
			// TODO: set large fields to null

			if ( Interface is not null )
			{
				int refCount = Marshal.ReleaseComObject( this.Interface );
			}
			else if ( Pointer != IntPtr.Zero )
			{
				int refCount = Marshal.Release( this.Pointer );
			}

			this.Interface = null;
			this.Pointer = IntPtr.Zero;
			
			// We may actually want to leave GUID alone so it can be determined
			// after Dispose is called in case it's ever needed -- it's not a
			// thing that ever changes anyways and can't really do any harm ...
			//this.GUID = Guid.Empty;

			this.disposedValue = true;
		}
	}

	/// <summary>
	/// Releases native COM interface references so that they can be
	/// freed from memory when the reference count reaches zero.
	/// </summary>
	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose( disposing: true );
		GC.SuppressFinalize( this );
	}

	protected virtual async ValueTask DisposeAsyncCore() { }

	/// <summary>
	/// Asynchronously releases references to underlying native COM interfaces
	/// so that the memory can be freed when the reference count reaches zero.
	/// </summary>
	/// <returns>An async ValueTask</returns>
	public async ValueTask DisposeAsync()
	{
		// Perform async cleanup.
		await DisposeAsyncCore();

		// Dispose of unmanaged resources.
		Dispose( disposing: false );

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
		// Suppress finalization.
		GC.SuppressFinalize( this );
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
	}

	#endregion

	//! TODO: Find out if there's a better way!
	internal int getCurrentRefCount() {
		try {
			int refCount	= Marshal.AddRef(this.Pointer);
			int prev		= refCount;
			refCount		= Marshal.Release(this.Pointer);
			return refCount;
		}
		catch { return -1; }
	}

	protected static IntPtr getPointerTo( object comObj ) {

		IntPtr pComObj = Marshal.GetIUnknownForObject( comObj );

#if DEBUG || !STRIP_CHECKS
		if ( pComObj == IntPtr.Zero )
			throw new COMException( $"ComPtr< object >( object comObj ): " +
				$"Unable to obtain pointer to IUnknown from {comObj}!" );
#endif

		return pComObj;
	}

	protected static object getCOMObjectFrom( IntPtr ptr ) {

		var comObj = Marshal.GetObjectForIUnknown( ptr )
#if DEBUG || !STRIP_CHECKS
			?? throw new COMException($"ComPtr< object >( IntPtr ): " +
				$"Unable to obtain object reference to COM interface from 0x{ptr.ToString("X8")}!");
#else
			;
#endif

		return comObj;
	}


};

/// <summary>
/// Provides a set of functionality similar to the native 
/// <a href="https://learn.microsoft.com/en-us/cpp/cppcx/wrl/comptr-class?view=msvc-170"><c>wrl::ComPtr&lt; <typeparamref name="T"/> &gt;</c></a> template.
/// </summary>
/// <remarks>
/// The <c>ComPtr&lt; <typeparamref name="T"/> &gt;</c> type is a type of "smart pointer" which holds the actual memory address of a COM object
/// and helps automatically relinquish native resources when it is <i>disposed</i> or goes out of scope. It 
/// also provides methods for reference counting and COM interface management functionality.
/// </remarks>
/// <typeparam name="T">Type of COM interface</typeparam>
internal sealed class ComPtr<T>: ComPtr where T: class
{
	//ComPtr() => GUID = typeof(T).GUID;
	
	internal ComPtr( T comObj ): base( comObj ) {
		
		// Assign our valid data:
		this.Interface = comObj as T ??
			throw new COMException( $"ComPtr< {typeof(T).Name} >( {typeof(T).Name} comObj ): " +
				$"Unable to obtain COM interface reference to {comObj} of type {typeof(T).Name}!" +
				$"There may be a COM type mismatch between {comObj.GetType().Name} and {typeof(T).Name}.");

		this.GUID = typeof(T).GUID;
	}

	internal ComPtr( IntPtr pComObj ): base( pComObj ) {
#if DEBUG || !STRIP_CHECKS
		if (base.Interface is null)
			throw new NullReferenceException($"ComPtr( object ) constructor was unable to obtain a valid COM interface " +
				$"reference from the given address 0x{Pointer.ToString("X8")}!");
#endif

		// Assign our valid data:
		this.Interface = base.Interface as T ??
			throw new COMException($"ComPtr< {typeof(T).Name} >( IntPtr pComObj ): " +
				$"Unable to convert COM object reference at 0x{Pointer.ToString("X8")} to COM interface type {typeof(T).Name}!");

		//this.Pointer = pComObj;
		this.GUID = typeof(T).GUID;
	}

	~ComPtr() => Dispose( disposing: false );

	/// <summary>
	/// The underlying COM interface
	/// </summary>
	internal new T? Interface { get; private set; }

	protected override void Dispose( bool disposing )
	{
		if ( !Disposed )
		{
			base.Dispose( disposing );
			this.Interface = null;
		}
	}

	//	/// <summary>
	//	/// The underlying COM interface
	//	/// </summary>
	//	internal T? Interface { get; private set; }

	//	/// <summary>
	//	/// Pointer to the underlying COM interface
	//	/// </summary>
	//	internal IntPtr Pointer { get; private set; }

	//	/// <summary>
	//	/// The GUID of the COM interface
	//	/// </summary>
	//	internal Guid GUID { get; private set; }

	//	/// <summary>
	//	/// Indicates if this instance has been disposed
	//	/// and released its resources and memory
	//	/// </summary>
	//	public bool Disposed => disposedValue;
	//	private bool disposedValue = false;


	//	/// <summary>
	//	/// Disposes of this instance's resources
	//	/// </summary>
	//	/// <param name="disposing">
	//	/// Indicates if managed resources should be
	//	/// disposed of from this call
	//	/// </param>
	//	protected virtual void Dispose(bool disposing)
	//	{
	//		if (!disposedValue)
	//		{
	//			if (disposing)
	//			{
	//				// TODO: dispose managed state (managed objects)
	//			}

	//			// TODO: free unmanaged resources (unmanaged objects) and override finalizer
	//			// TODO: set large fields to null

	//			if (Interface is not null)
	//			{
	//				int refCount = Marshal.ReleaseComObject(Interface);
	//			}
	//			else if (Pointer != IntPtr.Zero)
	//			{
	//				int refCount = Marshal.Release(this.Pointer);
	//			}

	//			this.Interface = null;
	//			this.Pointer = IntPtr.Zero;
	//			this.GUID = Guid.Empty;

	//			disposedValue = true;
	//		}
	//	}

	//	public void Dispose()
	//	{
	//		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
	//		Dispose(disposing: true);
	//		GC.SuppressFinalize(this);
	//	}

	//	protected virtual async ValueTask DisposeAsyncCore() { }

	//	public async ValueTask DisposeAsync()
	//	{
	//		// Perform async cleanup.
	//		await DisposeAsyncCore();

	//		// Dispose of unmanaged resources.
	//		Dispose(false);

	//#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
	//		// Suppress finalization.
	//		GC.SuppressFinalize(this);
	//#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
	//	}

};

//#if DEBUG || !STRIP_CHECKS
//		if (comObj is null)
//			throw new ArgumentNullException("comObj");

//		if (!Marshal.IsComObject(comObj))
//			throw new COMException($"ComPtr<{typeof(T).Name}>( comObj ): " +
//				@$"The parameter ""{comObj}"" is not a valid COM interface object!");
//#endif

//		IntPtr pComObj = Marshal.GetIUnknownForObject(comObj);

//		if (pComObj == IntPtr.Zero)
//			throw new COMException($"ComPtr< {typeof(T).Name} >( {typeof(T).Name} comObj ): " +
//				$"Unable to obtain pointer to IUnknown from {comObj}!");

// ----------------------------------------------------------------------------------------

//if ( pComObj == IntPtr.Zero )
//	throw new COMException($"ComPtr<{typeof(T).Name}>( IntPtr pComObj ): " +
//		"The given address is a null pointer!");

//var objRef = Marshal.GetObjectForIUnknown(pComObj) ??
//	throw new COMException($"ComPtr< {typeof(T).Name} >( IntPtr pComObj ): " +
//		$"Unable to obtain object reference to COM interface from 0x{Pointer.ToString("X8")}!");
