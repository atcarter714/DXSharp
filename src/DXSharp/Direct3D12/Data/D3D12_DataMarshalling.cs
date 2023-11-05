#region Using Directives

using System.Runtime.CompilerServices ;
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
 /// <summary>An empty resource barrier data structure.</summary>
 public static readonly ResourceBarrier Empty = new ResourceBarrier {
  Type = default,
  Flags = default,
  Anonymous = default,
 } ;

 
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
  [FieldOffset( 0 )] public _ResourceTransitionBarrier TransitionBarrier ;
  [FieldOffset( 0 )] public _ResourceAliasingBarrier AliasingBarrier ;
  [FieldOffset( 0 )] public _ResourceUAVBarrier UAVBarrier ;
  
  public _res_barrier_union( in _ResourceTransitionBarrier transitionBarrier ) {
   Unsafe.SkipInit( out this ) ;
   TransitionBarrier = transitionBarrier ;
  }
  public _res_barrier_union( in _ResourceAliasingBarrier aliasingBarrier ) {
   Unsafe.SkipInit( out this ) ;
   AliasingBarrier = aliasingBarrier ;
  }
  public _res_barrier_union( in _ResourceUAVBarrier uavBarrier ) {
   Unsafe.SkipInit( out this ) ;
   UAVBarrier = uavBarrier ;
  }
  
  public static implicit operator _res_barrier_union( in _ResourceTransitionBarrier transitionBarrier ) =>
   new _res_barrier_union( transitionBarrier ) ;
  public static implicit operator _res_barrier_union( in _ResourceAliasingBarrier aliasingBarrier ) =>
   new _res_barrier_union( aliasingBarrier ) ;
  public static implicit operator _res_barrier_union( in _ResourceUAVBarrier uavBarrier ) =>
   new _res_barrier_union( uavBarrier ) ;
 }

 
 public ResourceBarrier( ResourceBarrierType type, ResourceBarrierFlags flags, _ResourceTransitionBarrier transitionBarrier ) {
  Type = type ;
  Flags = flags ;
  Anonymous.TransitionBarrier = transitionBarrier ;
 }
 
 public ResourceBarrier( ResourceBarrierType type, ResourceBarrierFlags flags, _ResourceAliasingBarrier aliasingBarrier ) {
  Type = type ;
  Flags = flags ;
  Anonymous.AliasingBarrier = aliasingBarrier ;
 }
 
 public ResourceBarrier( ResourceBarrierType type, ResourceBarrierFlags flags, _ResourceUAVBarrier uavBarrier ) {
  Type = type ;
  Flags = flags ;
  Anonymous.UAVBarrier = uavBarrier ;
 }



 public static ResourceBarrier Transition( IResource resource, 
                                           ResourceStates stateBefore = ResourceStates.Common, 
                                           ResourceStates stateAfter = ResourceStates.Common, 
                                           uint subresource = 0 ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
  ArgumentNullException.ThrowIfNull( resource, nameof(resource) ) ;
#endif
  
	 ResourceBarrier resBarrier = default ;
	 resBarrier.Anonymous.TransitionBarrier = _ResourceTransitionBarrier.Default ;
	 resBarrier.Anonymous.TransitionBarrier.StateBefore = stateBefore ;
	 resBarrier.Anonymous.TransitionBarrier.StateAfter = stateAfter ;
	 resBarrier.Anonymous.TransitionBarrier.Subresource = subresource ;
	 unsafe {
   var _resource = (Resource)resource
#if DEBUG || DEBUG_COM || DEV_BUILD
    ?? throw new ArgumentNullException( nameof(resource) ) 
#endif
    ;
		 resBarrier.Anonymous.TransitionBarrier.pResource =
			 (ResourceUnmanaged *)( _resource?.ComPointer?.InterfaceVPtr ) ;
	 }
	 
	 return resBarrier ;
 }
 
 public static ResourceBarrier Aliasing( IResource resourceBefore, IResource resourceAfter ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
  ArgumentNullException.ThrowIfNull( resourceBefore, nameof(resourceBefore) ) ;
  ArgumentNullException.ThrowIfNull( resourceAfter, nameof(resourceAfter) ) ;
#endif
  ResourceBarrier resBarrier = default ;
  
  unsafe {
   Resource before = (Resource)resourceBefore, 
            after = (Resource)resourceAfter ;
#if DEBUG || DEBUG_COM || DEV_BUILD
   ObjectDisposedException.ThrowIf( before?.ComPointer?.InterfaceVPtr is null, nameof(resourceBefore) ) ;
   ObjectDisposedException.ThrowIf( after?.ComPointer?.InterfaceVPtr is null, nameof(resourceAfter) ) ;
#endif
   
   resBarrier.Anonymous.AliasingBarrier.pResourceBefore =
    (ResourceUnmanaged *)(before?.ComPointer?.InterfaceVPtr) ;
   resBarrier.Anonymous.AliasingBarrier.pResourceAfter =
    (ResourceUnmanaged *)(after?.ComPointer?.InterfaceVPtr) ;
  }
  
  return resBarrier ;
 }
 
 public static ResourceBarrier UAV( IResource resource ) {
  ResourceBarrier resBarrier = default ;
  unsafe {
   Resource _resource = (Resource)resource ;
#if DEBUG || DEBUG_COM || DEV_BUILD
   ObjectDisposedException.ThrowIf( _resource?.ComPointer?.InterfaceVPtr is null, nameof(resource) ) ;
#endif
   
   resBarrier.Anonymous.UAVBarrier.pResource =
    (ResourceUnmanaged *)( _resource?.ComPointer?.InterfaceVPtr ) ;
  }
  
  return resBarrier ;
 }

} ;



[EquivalentOf(typeof(D3D12_RESOURCE_TRANSITION_BARRIER_unmanaged))]
public unsafe struct _ResourceTransitionBarrier {
	/// <summary>Default/empty <see cref="_ResourceTransitionBarrier"/>.</summary>
	/// <remarks>StateBefore = ResourceStates.Common, StateAfter = ResourceStates.Present</remarks>
	public static readonly _ResourceTransitionBarrier Default = new _ResourceTransitionBarrier {
		Subresource = 0,
     pResource = default,
     StateBefore = ResourceStates.Common,
     StateAfter = ResourceStates.Present,
 } ;
 
 
 public ResourceUnmanaged* pResource ;
 public uint Subresource ;
 public ResourceStates StateBefore ;
 public ResourceStates StateAfter ;

 public ref ResourceUnmanaged ResourceUnmanagedRef => ref *( pResource ) ;
 public IResource ResourceRef => ResourceUnmanaged.GetManaged( pResource ) ;
 
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

 public _ResourceTransitionBarrier( IResource resource, 
                                    uint subresource = 0U, 
                                    ResourceStates stateBefore = ResourceStates.Common, 
                                    ResourceStates stateAfter  = ResourceStates.Common ) {
  unsafe {
   Resource _resource = (Resource)resource ;
#if DEBUG || DEBUG_COM || DEV_BUILD
   ObjectDisposedException.ThrowIf( _resource?.ComPointer?.InterfaceVPtr is null, nameof(resource) ) ;
#endif
   
   this.pResource = (ResourceUnmanaged *)( _resource?.ComPointer?.InterfaceVPtr ) ;
  }
  
  Subresource = subresource ;
  StateBefore = stateBefore ;
  StateAfter = stateAfter ;
 }
 
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