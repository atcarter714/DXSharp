#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[ProxyFor( typeof( ID3D12LifetimeTracker ) )]
public interface ILifetimeTracker: IDeviceChild,
								   IUnknownWrapper< ID3D12LifetimeTracker >,
								   IComObjectRef< ID3D12LifetimeTracker >,
								   IInstantiable {
	new ID3D12LifetimeTracker? COMObject => ComPointer?.Interface ;
	ID3D12LifetimeTracker? IComObjectRef< ID3D12LifetimeTracker >.COMObject => COMObject ;
	
	new ComPtr< ID3D12LifetimeTracker >? ComPointer { get ; }
	ComPtr< ID3D12LifetimeTracker >? IUnknownWrapper< ID3D12LifetimeTracker >.ComPointer => ComPointer ;

	new static Type ComType => typeof( ID3D12LifetimeTracker ) ;
	new static Guid InterfaceGUID => typeof( ILifetimeTracker ).GUID ;
	static Type IUnknownWrapper.ComType => typeof( ID3D12LifetimeTracker ) ;
	static Guid IUnknownWrapper.InterfaceGUID => typeof( ILifetimeOwner ).GUID ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( ID3D12LifetimeTracker ).GUID
																	   .ToByteArray( ) ;

			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference( data ) ) ;
		}
	}
	
	
	/// <summary>Destroys a lifetime-tracked object.</summary>
	/// <param name="pObject">
	/// <para>Type: **[ID3D12DeviceChild](./nn-d3d12-id3d12devicechild.md)\*** A pointer to an **ID3D12DeviceChild** interface representing the lifetime-tracked object.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12lifetimetracker-destroyownedobject#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	void DestroyOwnedObject( IDeviceChild pObject ) => 
		COMObject!.DestroyOwnedObject( pObject.COMObject ) ;

	static IDXCOMObject IInstantiable.Instantiate( ) => new LifetimeTracker( ) ;
	static IDXCOMObject IInstantiable.Instantiate( nint ptr ) => new LifetimeTracker( ptr ) ;
	static IDXCOMObject IInstantiable.Instantiate< ICom >( ICom pComObj ) => new LifetimeTracker( (ID3D12LifetimeTracker?) pComObj! ) ;
} ;


/// <summary>Provides a base class for <see cref="ILifetimeTracker"/>. </summary>
[Wrapper(typeof(ID3D12LifetimeTracker))]
internal class LifetimeTracker: DeviceChild, ILifetimeTracker {
	public new ComPtr< ID3D12LifetimeTracker >? ComPointer { get ; protected set ; }
	public new ID3D12LifetimeTracker? COMObject => ComPointer?.Interface ;

	internal LifetimeTracker( ) => ComPointer = default ;
	internal LifetimeTracker( ID3D12LifetimeTracker obj ) => ComPointer = new( obj ) ;
	internal LifetimeTracker( nint ptr ) => ComPointer = new( ptr ) ;
	internal LifetimeTracker( ComPtr< ID3D12LifetimeTracker > ptr ) => ComPointer = ptr ;
} ;