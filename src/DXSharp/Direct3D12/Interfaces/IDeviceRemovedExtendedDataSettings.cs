#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

#endregion
namespace DXSharp.Direct3D12 ;


[ProxyFor( typeof( ID3D12DeviceRemovedExtendedDataSettings ) )]
public interface IDeviceRemovedExtendedDataSettings: IInstantiable, IComIID,
													 IDisposable, IAsyncDisposable {
	// ---------------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12DeviceRemovedExtendedDataSettings) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )] get {
			ReadOnlySpan< byte > data = typeof(ID3D12DeviceRemovedExtendedDataSettings).GUID
				.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}

	static IInstantiable IInstantiable.Instantiate( ) => new DeviceRemovedExtendedDataSettings( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new DeviceRemovedExtendedDataSettings( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< Com >( Com obj ) => new DeviceRemovedExtendedDataSettings( ( obj as ID3D12DeviceRemovedExtendedDataSettings )! ) ;
	// ==================================================================================
} ;



[Wrapper(typeof(ID3D12DeviceRemovedExtendedDataSettings))]
internal class DeviceRemovedExtendedDataSettings: DisposableObject,
					 IDeviceRemovedExtendedDataSettings,
					 IComObjectRef< ID3D12DeviceRemovedExtendedDataSettings >,
					 IUnknownWrapper< ID3D12DeviceRemovedExtendedDataSettings > {
	
	//! ---------------------------------------------------------------------------------
	/// <summary>The COM object management instance.</summary>
	protected COMResource? ComResources { get ; set ; }
	protected internal void _initOrAdd< T >( ComPtr< T > ptr ) where T: IUnknown {
		if( ComResources is null ) ComResources = new( ptr ) ;
		else ComResources.AddPointer<T>( ptr ) ;
	}

	// ------------------------------------------------------------------------------------------
	ComPtr< ID3D12DeviceRemovedExtendedDataSettings >? _comPtr ;
	public virtual ID3D12DeviceRemovedExtendedDataSettings? ComObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12DeviceRemovedExtendedDataSettings >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer<ID3D12DeviceRemovedExtendedDataSettings>( ) ;
	// ------------------------------------------------------------------------------------------
	
	internal DeviceRemovedExtendedDataSettings( ) {
		_comPtr = ComResources?.GetPointer< ID3D12DeviceRemovedExtendedDataSettings >( ) ;
	}
	internal DeviceRemovedExtendedDataSettings( ComPtr< ID3D12DeviceRemovedExtendedDataSettings > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal DeviceRemovedExtendedDataSettings( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal DeviceRemovedExtendedDataSettings( ID3D12DeviceRemovedExtendedDataSettings comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// ------------------------------------------------------------------------------------------

	protected override async ValueTask DisposeUnmanaged( ) {
		if ( _comPtr is not null )
			await _comPtr.DisposeAsync( ) ;
		_comPtr = null ;
	}

	// ------------------------------------------------------------------------------------------
	public static Type ComType => typeof(ID3D12DeviceRemovedExtendedDataSettings) ;
	public static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12DeviceRemovedExtendedDataSettings).GUID
														  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	// ==========================================================================================
} ;
