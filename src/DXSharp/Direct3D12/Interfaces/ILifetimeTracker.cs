﻿#region Using Directives
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
	
	/// <summary>Destroys a lifetime-tracked object.</summary>
	/// <param name="pObject">
	/// <para>Type: **[ID3D12DeviceChild](./nn-d3d12-id3d12devicechild.md)\*** A pointer to an **ID3D12DeviceChild** interface representing the lifetime-tracked object.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12lifetimetracker-destroyownedobject#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	void DestroyOwnedObject( IDeviceChild pObject ) ;

	
	new static Type ComType => typeof( ID3D12LifetimeTracker ) ;
	
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
	
	static IInstantiable IInstantiable. Instantiate( ) => new LifetimeTracker( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new LifetimeTracker( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new LifetimeTracker( (ID3D12LifetimeTracker?) pComObj! ) ;
} ;



/// <summary>Provides a base class for <see cref="ILifetimeTracker"/>. </summary>
[Wrapper(typeof(ID3D12LifetimeTracker))]
internal class LifetimeTracker: DeviceChild, 
								ILifetimeTracker {
	ComPtr< ID3D12LifetimeTracker >? _comPtr ;
	public new ComPtr< ID3D12LifetimeTracker >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< ID3D12LifetimeTracker >( ) ;
	public new ID3D12LifetimeTracker? ComObject => ComPointer?.Interface ;

	
	internal LifetimeTracker( ) {
		_comPtr = ComResources?.GetPointer< ID3D12LifetimeTracker >( ) ;
	}
	internal LifetimeTracker( ComPtr< ID3D12LifetimeTracker > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal LifetimeTracker( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal LifetimeTracker( ID3D12LifetimeTracker? comObject ) {
		_comPtr = new( comObject! ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	
	public void DestroyOwnedObject( IDeviceChild pObject ) {
		var deviceChild = pObject as IComObjectRef< ID3D12DeviceChild > ;
		ComObject!.DestroyOwnedObject( deviceChild!.ComObject ) ;
	}

	
	public new static Type ComType => typeof( ID3D12LifetimeTracker ) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( ID3D12LifetimeTracker ).GUID
																	   .ToByteArray( ) ;

			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference( data ) ) ;
		}
	}

} ;