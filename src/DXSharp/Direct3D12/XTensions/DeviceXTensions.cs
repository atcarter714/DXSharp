#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
#region Using Directives
using System.Runtime.CompilerServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12.XTensions ;


public static class DeviceXTensions
{
	static readonly ClearValue optimizedClearValue 
		= new( DXGI.Format.R8G8B8A8_UNORM,
			   new __float_4( 0.0f, 0.0f, 0.0f, 0.0f ) ) ;

	
	public static TRsrc? CreateComittedResource< TRsrc >( this IDevice device,
														  in   HeapProperties heapProperties,
														  in   ResourceDescription desc,
														  ResourceStates initialState ) where TRsrc: IResource {
		CreateComittedResource< TRsrc >( device, heapProperties, desc, initialState, out var resource ) ;
		return resource ;
	}
	
	/// <summary>
	/// Creates both a resource and an implicit heap, such that the heap is big enough to contain the entire resource, and the resource is mapped to the heap.
	/// </summary>
	/// <param name="device">This IDevice instance</param>
	/// <param name="heapProperties">A <see cref="HeapProperties"/> structure that provides properties for the resource's heap.</param>
	/// <param name="desc">A <see cref="HeapDescription"/> structure that describes the resource.</param>
	/// <param name="initialState">
	/// The initial state of the resource, as a bitwise-OR'd combination of <see cref="ResourceStates"/> enumeration constants.<para/>
	/// When you create a resource together with a <see cref="HeapType.Upload"/> heap, you must set InitialResourceState to <see cref="ResourceStates.GenericRead"/>.<para/>
	/// When you create a resource together with a <see cref="HeapType.ReadBack"/> heap, you must set InitialResourceState to <see cref="ResourceStates.CopyDest"/>.
	/// </param>
	/// <param name="ppvResource"></param>
	/// <typeparam name="TRsrc"></typeparam>
	/// <remarks>
	/// This method creates both a resource and a heap, such that the heap is big enough to contain the entire resource, and the resource is mapped to the heap.
	/// The created heap is known as an implicit heap, because the heap object can't be obtained by the application. Before releasing the final reference on the
	/// resource, your application must ensure that the GPU will no longer read nor write to this resource.<para/>
	/// 
	/// The implicit heap is made resident for GPU access before the method returns control to your application. Also see Residency.<para/>
	///
	/// The resource GPU VA mapping can't be changed. See <see cref="ICommandQueue.UpdateTileMappings"/> and
	/// <a href="https://learn.microsoft.com/en-us/windows/win32/direct3d12/volume-tiled-resources">Volume tiled resources</a>.<para/>
	/// <b>Note:</b> This method may be called by multiple threads concurrently.
	/// </remarks>
	public static void CreateComittedResource< TRsrc >( this IDevice device, 
														in HeapProperties heapProperties,
														in ResourceDescription desc, 
														ResourceStates initialState,
														out TRsrc? ppvResource ) where TRsrc : IResource {
		
		device.CreateCommittedResource( heapProperties, HeapFlags.None, desc, initialState,
										optimizedClearValue, TRsrc.Guid, out var resource ) ;
		
		ppvResource = (TRsrc?)resource ;
	}
	
	
	/// <summary>
	/// The equivalent of calling <see cref="ID3D12Device.CreateCommittedResource"/> with a value
	/// of <b>null</b> for the ID3D12Resource pointer, to check the validity of the other parameters.
	/// <b>ppvResource</b> can be <b>null</b>, to enable capability testing. When <b>null</b> is used,
	/// no object is created, and S_FALSE is returned when pDesc is valid.
	/// </summary>
	/// <param name="device">This device instance.</param>
	/// <param name="heapProperties">Data structure that provides properties for the resource's heap.</param>
	/// <param name="desc">Data structure that describes the resource.</param>
	/// <param name="initialState">
	/// The initial state of the resource, as a bitwise-OR'd combination of <see cref="ResourceStates"/> enumeration constants.<para/>
	/// When you create a resource together with a <see cref="HeapType.Upload"/> heap, you must set InitialResourceState to <see cref="ResourceStates.GenericRead"/>.<para/>
	/// When you create a resource together with a <see cref="HeapType.ReadBack"/> heap, you must set InitialResourceState to <see cref="ResourceStates.CopyDest"/>.
	/// </param>
	/// <returns>An <see cref="HResult"/> with the value of <value>0x01</value> or <b>S_FALSE</b> if the properties are valid.</returns>
	/// <exception cref="NullReferenceException">Thrown if the device is dead.</exception>
	public static HResult CreateCommittedResource( this IDevice device,
												   in HeapProperties heapProperties,
												   in ResourceDescription desc,
												   ResourceStates initialState = ResourceStates.Common ) {
		unsafe {
			var _device = ( Device )device ;
			var comPtr = _device.ComPointer ?? throw new NullReferenceException( ) ;
			var method =
				(delegate* unmanaged[Stdcall, MemberFunction]< ID3D12Device*, HeapProperties*, HeapFlags, ResourceDescription*,
					ResourceStates, ClearValue*, Guid*, ResourceUnmanaged*, HResult >)comPtr.GetVTableMethod< ID3D12Device >( 27 ) ;
			
			var devicePtr = (ID3D12Device *)comPtr.InterfaceVPtr ;
			var guid      = typeof( ID3D12Resource ).GUID ;
			fixed( void* pHeapProps = &heapProperties, pResDesc = &desc ) {
				var hr = method( devicePtr,
						(HeapProperties *)pHeapProps, HeapFlags.None, 
						(ResourceDescription *)pResDesc, initialState,
						null, &guid, null ) ;
				return hr ;
			}
		}
	}
	
} ;