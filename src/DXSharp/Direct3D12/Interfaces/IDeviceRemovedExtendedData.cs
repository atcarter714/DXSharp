#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[ProxyFor( typeof( ID3D12DeviceRemovedExtendedData ) )]
public interface IDeviceRemovedExtendedData: IInstantiable, IComIID,
											 IDisposable, IAsyncDisposable {
	// ---------------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12DeviceRemovedExtendedData) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )] get {
			ReadOnlySpan< byte > data = typeof(ID3D12DeviceRemovedExtendedData).GUID
																			   .ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}

	static IInstantiable IInstantiable.Instantiate( ) => new DeviceRemovedExtendedData( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new DeviceRemovedExtendedData( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< Com >( Com obj ) => new DeviceRemovedExtendedData( ( obj as ID3D12DeviceRemovedExtendedData )! ) ;
	// ==================================================================================
} ;



[Wrapper(typeof(ID3D12DeviceRemovedExtendedData))]
internal class DeviceRemovedExtendedData: DisposableObject,
					 IDeviceRemovedExtendedData,
					 IComObjectRef< ID3D12DeviceRemovedExtendedData >,
					 IUnknownWrapper< ID3D12DeviceRemovedExtendedData > {
	
	//! ---------------------------------------------------------------------------------
	/// <summary>The COM object management instance.</summary>
	protected COMResource? ComResources { get ; set ; }
	protected internal void _initOrAdd< T >( ComPtr< T > ptr ) where T: IUnknown {
		if( ComResources is null ) ComResources = new( ptr ) ;
		else ComResources.AddPointer<T>( ptr ) ;
	}

	// ------------------------------------------------------------------------------------------
	ComPtr< ID3D12DeviceRemovedExtendedData >? _comPtr ;
	public virtual ID3D12DeviceRemovedExtendedData? ComObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12DeviceRemovedExtendedData >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer<ID3D12DeviceRemovedExtendedData>( ) ;
	// ------------------------------------------------------------------------------------------
	
	internal DeviceRemovedExtendedData( ) {
		_comPtr = ComResources?.GetPointer< ID3D12DeviceRemovedExtendedData >( ) ;
	}
	internal DeviceRemovedExtendedData( ComPtr< ID3D12DeviceRemovedExtendedData > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal DeviceRemovedExtendedData( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal DeviceRemovedExtendedData( ID3D12DeviceRemovedExtendedData comObject ) {
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
	public static Type ComType => typeof(ID3D12DeviceRemovedExtendedData) ;
	public static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12DeviceRemovedExtendedData).GUID
														  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	// ==========================================================================================
} ;
