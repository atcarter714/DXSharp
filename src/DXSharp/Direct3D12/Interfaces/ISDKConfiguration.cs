#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.Windows ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


/// <inheritdoc cref="ID3D12SDKConfiguration"/>
[ProxyFor(typeof(ID3D12SDKConfiguration))]
public interface ISDKConfiguration {
	/// <summary>Configures the SDK version to use.</summary>
	/// <param name="SDKVersion">
	/// <para>The SDK version to set.</para>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12sdkconfiguration-setsdkversion#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="SDKPath">
	/// <para>A NULL-terminated string that provides the relative path to `d3d12core.dll` at the specified *SDKVersion*.
	/// The path is relative to the process exe of the caller. If `d3d12core.dll` isn't found, or isn't of the specified *SDKVersion*,
	/// then Direct3D 12 device creation fails.</para>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12sdkconfiguration-setsdkversion#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>If the function succeeds, then it returns <b><c>S_OK</c></b>. Otherwise, it returns one of the
	/// <a href="https://learn.microsoft.com/en-us/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 return codes</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para>This method can be used only in Windows Developer Mode. To set the SDK version using this API, you must call it before you create the Direct3D 12 device.
	/// Calling this API *after* creating the Direct3D 12 device will cause the Direct3D 12 runtime to remove the device. If the `d3d12core.dll` installed with the OS
	/// is newer than the SDK version specified, then the OS version is used instead. You can retrieve the version of a particular `D3D12Core.dll` from the exported symbol
	/// <b><c>D3D12SDKVersion</c></b>, which is a variable of type <see cref="uint"/>, just like the variables exported from applications to enable use of the Agility SDK.</para>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12sdkconfiguration-setsdkversion#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	HResult SetSDKVersion( uint SDKVersion, string SDKPath ) ;
} ;



/// <inheritdoc cref="ISDKConfiguration"/>
[Wrapper(typeof(ID3D12SDKConfiguration))]
internal class SDKConfiguration: DisposableObject,
								 ISDKConfiguration,
								 IComObjectRef< ID3D12SDKConfiguration >,
								 IUnknownWrapper< ID3D12SDKConfiguration > {
	// -------------------------------------------------------------------------------------------------------
	//! ---------------------------------------------------------------------------------
	/// <summary>The COM object management instance.</summary>
	COMResource? ComResources { get ; set ; }
	private protected void _initOrAdd< T >( ComPtr< T > ptr ) where T: IUnknown {
		if( ComResources is null ) ComResources = new( ptr ) ;
		else ComResources.AddPointer( ptr ) ;
	}
	
	// ----------------------------------------------------------------------------------
	ComPtr< ID3D12SDKConfiguration >? _comPtr ;
	public virtual ID3D12SDKConfiguration? ComObject => ComPointer?.Interface ;
	public virtual ComPtr< ID3D12SDKConfiguration >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer<ID3D12SDKConfiguration>(  ) ;
	// -------------------------------------------------------------------------------------------------------
	
	
	internal SDKConfiguration( ) {
		_comPtr = ComResources?.GetPointer<ID3D12SDKConfiguration>(  ) ;
	}
	internal SDKConfiguration( nint childAddr ) {
		_comPtr = new( childAddr ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal SDKConfiguration( ID3D12SDKConfiguration child ) {
		_comPtr = new( child ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal SDKConfiguration( ComPtr< ID3D12SDKConfiguration > comPtr ) {
		ArgumentNullException.ThrowIfNull( comPtr, nameof(comPtr) ) ;
		_comPtr = comPtr ;
		_initOrAdd( _comPtr! ) ;
	}
	
	
	// -------------------------------------------------------------------------------------------------------
	
	/// <inheritdoc cref="ISDKConfiguration.SetSDKVersion"/>
	public HResult SetSDKVersion( uint SDKVersion, string SDKPath ) => 
		ComObject!.SetSDKVersion( SDKVersion, SDKPath ) ;

	
	// -------------------------------------------------------------------------------------------------------
	protected override async ValueTask DisposeUnmanaged( ) {
		if ( _comPtr is null || _comPtr.Disposed ) return ;
		await _comPtr.DisposeAsync( ) ;
	}

	public override async ValueTask DisposeAsync( ) {
		var _taskBaseDispose = base.DisposeAsync( ).AsTask( ) ;
		var _taskDispose = Task.Run( Dispose ) ;
		await Task.WhenAll( _taskBaseDispose, _taskDispose ) ;
	}
	// -------------------------------------------------------------------------------------------------------
	public static Type ComType => typeof(ID3D12SDKConfiguration) ;
	public static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )] get {
			ReadOnlySpan< byte > data = typeof(ID3D12SDKConfiguration).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	// =======================================================================================================
} ;