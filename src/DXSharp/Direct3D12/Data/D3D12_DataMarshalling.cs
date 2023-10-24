#region Using Directives
using System.Runtime.InteropServices ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp ;
using DXSharp.DXGI ;
#endregion
namespace DXSharp.Direct3D12 ;


// -----------------------------------------------------------------
// Resource Barrier Structures:
// -----------------------------------------------------------------

[StructLayout( LayoutKind.Sequential ),
 EquivalentOf( typeof(D3D12_RESOURCE_BARRIER) )]
public struct ResourceBarrier {

 /// <summary>
 /// <para>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_barrier_type">D3D12_RESOURCE_BARRIER_TYPE</a>-typed value that specifies the type of resource barrier. This member determines which type to use in the union below.</para>
 /// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_resource_barrier#members">Read more on docs.microsoft.com</a>.</para>
 /// </summary>
 public ResourceBarrierType Type ;

 /// <summary>
 /// Specifies a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_barrier_flags">D3D12_RESOURCE_BARRIER_FLAGS</a>
 /// enumeration constant such as for "begin only" or "end only".
 /// </summary>
 public ResourceBarrierFlags Flags ;

 public _res_barrier_union Anonymous ;
 [StructLayout( LayoutKind.Explicit )]
 public partial struct _res_barrier_union {
  [FieldOffset( 0 )] public _ResourceTransitionBarrier Transition ;
  [FieldOffset( 0 )] public _ResourceAliasingBarrier Aliasing ;
  [FieldOffset( 0 )] public _ResourceUAVBarrier UAV ;
 } ;



 //public ref _ResourceTransitionBarrier TransitionBarrier => ref Anonymous.Transition ;
 
 public ResourceBarrier( ResourceBarrierType type, ResourceBarrierFlags flags, _ResourceTransitionBarrier transition ) {
  Type = type ;
  Flags = flags ;
  Anonymous.Transition = transition ;
 }
 
 public ResourceBarrier( ResourceBarrierType type, ResourceBarrierFlags flags, _ResourceAliasingBarrier aliasing ) {
  Type = type ;
  Flags = flags ;
  Anonymous.Aliasing = aliasing ;
 }
 
 public ResourceBarrier( ResourceBarrierType type, ResourceBarrierFlags flags, _ResourceUAVBarrier uav ) {
  Type = type ;
  Flags = flags ;
  Anonymous.UAV = uav ;
 }



 public static ResourceBarrier Transition( Resource renderTarget, 
											ResourceStates stateBefore, 
											ResourceStates stateAfter ) {
	 ResourceBarrier resBarrier = default ;
	 resBarrier.Anonymous.Transition = _ResourceTransitionBarrier.Default ;
	 resBarrier.Anonymous.Transition.StateBefore = stateBefore ;
	 resBarrier.Anonymous.Transition.StateAfter = stateAfter ;
	 resBarrier.Anonymous.Transition.Subresource = 0 ;
	 unsafe {
		 resBarrier.Anonymous.Transition.pResource =
			 (ResourceUnmanaged*)(renderTarget.ComPointer.InterfaceVPtr);
	 }
	 
	 return resBarrier ;
 }

} ;



[EquivalentOf(typeof(D3D12_RESOURCE_TRANSITION_BARRIER_unmanaged))]
public unsafe struct _ResourceTransitionBarrier {
 public ResourceUnmanaged* pResource ;
 public uint Subresource ;
 public ResourceStates StateBefore ;
 public ResourceStates StateAfter ;

 public ref ResourceUnmanaged ResourceRef => ref *( pResource ) ;
 
 
 public _ResourceTransitionBarrier( ResourceUnmanaged* pResource, uint subresource, 
                                    ResourceStates stateBefore, ResourceStates stateAfter ) {
  this.pResource = pResource ;
  Subresource = subresource ;
  StateBefore = stateBefore ;
  StateAfter = stateAfter ;
 }
 

 public _ResourceTransitionBarrier( ID3D12Resource_unmanaged* pResource, uint subresource, 
                                    ResourceStates stateBefore, ResourceStates stateAfter ) {
  this.pResource = (ResourceUnmanaged *)pResource ;
  Subresource = subresource ;
  StateBefore = stateBefore ;
  StateAfter = stateAfter ;
 }


	/// <summary>Default/empty <see cref="_ResourceTransitionBarrier"/>.</summary>
	/// <remarks>StateBefore = ResourceStates.Common, StateAfter = ResourceStates.Present</remarks>
	public static readonly _ResourceTransitionBarrier Default = new _ResourceTransitionBarrier {
		Subresource = 0,
     pResource = default,
     StateBefore = ResourceStates.Common,
     StateAfter = ResourceStates.Present,
 };


 /*public _ResourceTransitionBarrier( IResource pResource, uint subresource, 
                                    ResourceStates stateBefore, ResourceStates stateAfter ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
  ArgumentNullException.ThrowIfNull( pResource, nameof(pResource) ) ;
  ObjectDisposedException.ThrowIf( pResource.COMObject is null, nameof(pResource) ) ;
#endif
  
  Guid iid = typeof( ID3D12Resource ).GUID ;
  pResource.COMObject.QueryInterface( ref iid, out nint pInterface ) ;
  this.pResource = (ResourceUnmanaged *)pInterface ;
  pResource.COMObject.Release( ) ;
  
  Subresource = subresource ;
  StateBefore = stateBefore ;
  StateAfter = stateAfter ;
 }*/
} ;

[EquivalentOf(typeof(D3D12_RESOURCE_TRANSITION_BARRIER_unmanaged))]
public unsafe struct _ResourceAliasingBarrier {
 public ResourceUnmanaged* pResourceBefore ;
 public ResourceUnmanaged* pResourceAfter ;
 
 public _ResourceAliasingBarrier( ResourceUnmanaged* pResourceBefore, ResourceUnmanaged* pResourceAfter ) {
  this.pResourceBefore = pResourceBefore ;
  this.pResourceAfter = pResourceAfter ;
 }
 
 public _ResourceAliasingBarrier( ID3D12Resource_unmanaged* pResourceBefore, ID3D12Resource_unmanaged* pResourceAfter ) {
  this.pResourceBefore = (ResourceUnmanaged *)pResourceBefore ;
  this.pResourceAfter = (ResourceUnmanaged *)pResourceAfter ;
 }
} ;

[EquivalentOf(typeof(D3D12_RESOURCE_UAV_BARRIER_unmanaged))]
public unsafe struct _ResourceUAVBarrier {
 public ResourceUnmanaged* pResource ;
 
 public _ResourceUAVBarrier( ResourceUnmanaged* pResource ) => 
  this.pResource = pResource ;
 public _ResourceUAVBarrier( ID3D12Resource_unmanaged* pResource ) => 
  this.pResource = (ResourceUnmanaged *)pResource ;
} ;