#region Using Directives

using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.DXGI ;
using DXSharp.Windows.COM ;
#endregion

namespace DXSharp.DXGI ;

[Wrapper(typeof(IDXGIDevice))]
public interface IDevice: IObject,
						  IComObjectRef< IDXGIDevice >,
						  IUnknownWrapper< IDXGIDevice >, IInstantiable {
	public new static Guid InterfaceGUID => typeof( IDXGIDevice ).GUID ;
	public new static Type ComType => typeof( IDXGIDevice ) ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIDevice).GUID
																	.ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	new IDXGIDevice? COMObject => ComPointer?.Interface ;
	new ComPtr< IDXGIDevice >? ComPointer { get ; }
	
	// ----------------------------------------------------------
	// Interface Methods:
	// ----------------------------------------------------------

	IAdapter GetAdapter< T >( ) where T: class, IAdapter, IInstantiable ;
	
	internal void CreateSurface( in SurfaceDescription pDesc,
								 uint numSurfaces,
								 uint usage,
								 ref SharedResource? pSharedResource,
								 out Span< Surface > ppSurface  ) ;
	
	void QueryResourceResidency( in  Resource?[ ] ppResources, 
								 out Span< Residency > statusSpan, 
								 uint numResources ) ;
	
	void SetGPUThreadPriority( int priority ) ;
	void GetGPUThreadPriority( out int pPriority ) ;
} ;


// -----------------------------------------------------------------
// Device Sub-Object Types:
// -----------------------------------------------------------------

//! Abstract Base Interface:
public interface IDeviceSubObject: IObject {
	T GetDevice<T>( ) where T: Device ;

	static Type IUnknownWrapper.ComType => typeof(IDXGIDeviceSubObject) ;
	static Guid IUnknownWrapper.InterfaceGUID => typeof(IDXGIDeviceSubObject).GUID ;
	
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIDeviceSubObject).GUID
														   .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
} ;
