#region Using Directives
#pragma warning disable CS1591, CS1573, CS0465, CS0649, CS8019, CS1570, CS1584, CS1658, CS0436, CS8981

using global::System;
using global::System.Diagnostics;
using global::System.Diagnostics.CodeAnalysis;
using global::System.Runtime.CompilerServices;
using global::System.Runtime.InteropServices;
using global::System.Runtime.Versioning;

using winmdroot = global::Windows.Win32;
#endregion

namespace DXSharp.Windows {
	/// <summary>A 32-bit value that is used to describe an error or warning.</summary>
	[DebuggerDisplay( "{Value}" )]
	public readonly struct HResult: IEquatable< HResult > {
		/// <summary>Gets the value of the HRESULT as an Int32 value</summary>
		public readonly int Value ;

		/// <summary>Creates a new HRESULT value</summary>
		/// <param name="value">Value of the HRESULT code</param>
		internal HResult( int value ) => this.Value = value ;

		
		public static bool operator !=( HResult left, HResult right ) => !( left == right ) ;
		public static bool operator ==( HResult left, HResult right ) => left.Value == right.Value ;
		
		public static implicit operator int( HResult value ) => value.Value ;
		public static explicit operator HResult( int value ) => new HResult( value ) ;
		public static implicit operator uint( HResult value ) => (uint)value.Value ;
		public static explicit operator HResult( uint value ) => new HResult( (int)value ) ;
		public static implicit operator HResult( winmdroot.Foundation.HRESULT value ) =>
			new HResult( value.Value ) ;
		public static implicit operator winmdroot.Foundation.HRESULT( HResult value ) =>
			new winmdroot.Foundation.HRESULT( value.Value ) ;
		
		

		/// <summary>
		/// Determines if the given HRESULT is equal to this one
		/// </summary>
		/// <param name="other">HRESULT to compare</param>
		/// <returns>True if they are equal, otherwise false</returns>
		public bool Equals( HResult other ) => this.Value == other.Value ;

		/// <summary>
		/// Determines if the given object and this HRESULT value are equal
		/// </summary>
		/// <param name="obj">Object to compare to</param>
		/// <returns>True if equal, otherwise false</returns>
		public override bool Equals( object obj ) => obj is HResult other && this.Equals( other ) ;

		/// <summary>
		/// Gets the hash code of this HRESULT value
		/// </summary>
		/// <returns>Int32 hash code</returns>
		public override int GetHashCode() => this.Value.GetHashCode( ) ;


		/// <summary>
		/// Indicates if the operation was successful
		/// </summary>
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		public bool Succeeded => this.Value >= 0 ;

		/// <summary>
		/// Indicates if the operation was a failure
		/// </summary>
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		public bool Failed => this.Value < 0 ;

		
		/// <inheritdoc cref="Marshal.ThrowExceptionForHR(int, IntPtr)" />
		/// <param name="errorInfo">
		/// A pointer to the IErrorInfo interface that provides more information about the
		/// error. You can specify <see cref="IntPtr.Zero"/> to use the current IErrorInfo interface, or
		/// <c>new IntPtr(-1)</c> to ignore the current IErrorInfo interface and construct the exception
		/// just from the error code.
		/// </param>
		/// <returns><see langword="this"/> <see cref="HResult"/>, if it does not reflect an error.</returns>
		/// <seealso cref="Marshal.ThrowExceptionForHR(int, IntPtr)"/>
		public HResult ThrowOnFailure( nint errorInfo = default ) {
			Marshal.ThrowExceptionForHR( this.Value, errorInfo ) ;
			return this ;
		}

		/// <summary>
		/// Converts the HRESULT value to a string representation
		/// </summary>
		/// <returns>HRESULT in string form</returns>
		public override string ToString( ) =>
			string.Format( global::System.Globalization.CultureInfo.InvariantCulture, "0x{0:X8}", this.Value ) ;

		/// <summary>
		/// Converts the HRESULT value to a specially formatted string representation
		/// </summary>
		/// <param name="format">Formatting string</param>
		/// <param name="formatProvider">The format provider</param>
		/// <returns>HRESULT in special formatted string form</returns>
		public string ToString( string format, IFormatProvider formatProvider ) =>
			( (uint)this.Value ).ToString( format, formatProvider ) ;
		
		

		#region Readonly HRESULT Values

		/// <summary>
		/// Result was successful / OK
		/// </summary>
		public static readonly HResult S_OK = (HResult)( 0 ) ;

		/// <summary>
		/// You tried to use a resource to which you did not have the required access privileges. 
		/// This error is most typically caused when you write to a shared resource with read-only access.
		/// </summary>
		public static readonly HResult DXGI_ERROR_ACCESS_DENIED = (HResult)( -2005270485 ) ;

		/// <summary>
		/// The desktop duplication interface is invalid. The desktop duplication interface 
		/// typically becomes invalid when a different type of image is displayed on the desktop.
		/// </summary>
		public static readonly HResult DXGI_ERROR_ACCESS_LOST = (HResult)( -2005270490 ) ;

		/// <summary>
		/// The desired element already exists. This is returned by DXGIDeclareAdapterRemovalSupport 
		/// if it is not the first time that the function is called.
		/// </summary>
		public static readonly HResult DXGI_ERROR_ALREADY_EXISTS = (HResult)( -2005270474 ) ;

		/// <summary>
		/// DXGI can't provide content protection on the swap chain. This error is typically caused by 
		/// an older driver, or when you use a swap chain that is incompatible with content protection.
		/// </summary>
		public static readonly HResult DXGI_ERROR_CANNOT_PROTECT_CONTENT = (HResult)( -2005270486 ) ;

		/// <summary>
		/// The application's device failed due to badly formed commands sent by the application. 
		/// This is an design-time issue that should be investigated and fixed.
		/// </summary>
		public static readonly HResult DXGI_ERROR_DEVICE_HUNG = (HResult)( -2005270522 ) ;

		/// <summary>
		/// The video card has been physically removed from the system, or a driver upgrade for 
		/// the video card has occurred. The application should destroy and recreate the device. 
		/// For help debugging the problem, call ID3D10Device::GetDeviceRemovedReason.
		/// </summary>
		public static readonly HResult DXGI_ERROR_DEVICE_REMOVED = (HResult)( -2005270523 ) ;

		/// <summary>
		/// The device failed due to a badly formed command. This is a run-time issue; 
		/// The application should destroy and recreate the device.
		/// </summary>
		public static readonly HResult DXGI_ERROR_DEVICE_RESET = (HResult)( -2005270521 ) ;

		/// <summary>
		/// The driver encountered a problem and was put into the device removed state.
		/// </summary>
		public static readonly HResult DXGI_ERROR_DRIVER_INTERNAL_ERROR = (HResult)( -2005270496 ) ;

		/// <summary>
		/// An event (for example, a power cycle) interrupted the gathering of presentation statistics.
		/// </summary>
		public static readonly HResult DXGI_ERROR_FRAME_STATISTICS_DISJOINT = (HResult)( -2005270517 ) ;

		/// <summary>
		/// The application attempted to acquire exclusive ownership of an output, but failed 
		/// because some other application (or device within the application) already acquired ownership.
		/// </summary>
		public static readonly HResult DXGI_ERROR_GRAPHICS_VIDPN_SOURCE_IN_USE = (HResult)( -2005270516 ) ;

		/// <summary>
		/// The application provided invalid parameter data; this must be debugged and fixed before the application is released.
		/// </summary>
		public static readonly HResult DXGI_ERROR_INVALID_CALL = (HResult)( -2005270527 ) ;

		/// <summary>
		/// The buffer supplied by the application is not big enough to hold the requested data.
		/// </summary>
		public static readonly HResult DXGI_ERROR_MORE_DATA = (HResult)( -2005270525 ) ;

		/// <summary>
		/// The supplied name of a resource in a call to DXGI.IResource1.CreateSharedHandle 
		/// is already associated with some other resource.
		/// </summary>
		public static readonly HResult DXGI_ERROR_NAME_ALREADY_EXISTS = (HResult)( -2005270484 ) ;

		/// <summary>
		/// A global counter resource is in use, and the Direct3D device can't currently use the counter resource.
		/// </summary>
		public static readonly HResult DXGI_ERROR_NONEXCLUSIVE = (HResult)( -2005270495 ) ;

		/// <summary>
		/// The resource or request is not currently available, but it might become available later.
		/// </summary>
		public static readonly HResult DXGI_ERROR_NOT_CURRENTLY_AVAILABLE = (HResult)( -2005270494 ) ;

		/// <summary>
		/// When calling DXGI.IObject.GetPrivateData, the GUID passed in is not recognized as 
		/// one previously passed to DXG.IObject.SetPrivateData or DXGI.IObject.SetPrivateDataInterface. 
		/// When calling DXGI.IFactory.EnumAdapters or DXGI.IAdapter.EnumOutputs, the enumerated ordinal 
		/// is out of range.
		/// </summary>
		public static readonly HResult DXGI_ERROR_NOT_FOUND = (HResult)( -2005270526 ) ;

		/// <summary>
		/// Reserved
		/// </summary>
		public static readonly HResult DXGI_ERROR_REMOTE_CLIENT_DISCONNECTED = (HResult)( -2005270493 ) ;

		/// <summary>
		/// Reserved
		/// </summary>
		public static readonly HResult DXGI_ERROR_REMOTE_OUTOFMEMORY = (HResult)( -2005270492 ) ;

		/// <summary>
		/// The DXGI output (monitor) to which the swap chain content was restricted is now disconnected or changed.
		/// </summary>
		public static readonly HResult DXGI_ERROR_RESTRICT_TO_OUTPUT_STALE = (HResult)( -2005270487 ) ;

		/// <summary>
		/// The operation depends on an SDK component that is missing or mismatched.
		/// </summary>
		public static readonly HResult DXGI_ERROR_SDK_COMPONENT_MISSING = (HResult)( -2005270483 ) ;

		/// <summary>
		/// The Remote Desktop Services session is currently disconnected.
		/// </summary>
		public static readonly HResult DXGI_ERROR_SESSION_DISCONNECTED = (HResult)( -2005270488 ) ;

		/// <summary>
		/// The requested functionality is not supported by the device or the driver.
		/// </summary>
		public static readonly HResult DXGI_ERROR_UNSUPPORTED = (HResult)( -2005270524 ) ;

		/// <summary>
		/// The time-out interval elapsed before the next desktop frame was available.
		/// </summary>
		public static readonly HResult DXGI_ERROR_WAIT_TIMEOUT = (HResult)( -2005270489 ) ;

		/// <summary>
		/// The GPU was busy at the moment when a call was made to perform an operation, 
		/// and did not execute or schedule the operation.
		/// </summary>
		public static readonly HResult DXGI_ERROR_WAS_STILL_DRAWING = (HResult)( -2005270518 ) ;

		#endregion
	} ;
}

