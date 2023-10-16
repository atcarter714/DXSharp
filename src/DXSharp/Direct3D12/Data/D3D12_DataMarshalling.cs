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
 EquivalentOf( typeof( D3D12_RESOURCE_BARRIER ) )]
public struct ResourceBarrier {

 /// <summary>
 /// <para>A <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_barrier_type">D3D12_RESOURCE_BARRIER_TYPE</a>-typed value that specifies the type of resource barrier. This member determines which type to use in the union below.</para>
 /// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_resource_barrier#members">Read more on docs.microsoft.com</a>.</para>
 /// </summary>
 public D3D12_RESOURCE_BARRIER_TYPE Type ;

 /// <summary>Specifies a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_barrier_flags">D3D12_RESOURCE_BARRIER_FLAGS</a> enumeration constant such as for "begin only" or "end only".</summary>
 public D3D12_RESOURCE_BARRIER_FLAGS Flags ;

 public _res_barrier_union Anonymous ;
 [StructLayout(LayoutKind.Explicit)]
 public partial struct _res_barrier_union {
  [FieldOffset( 0 )] public _resourceTransitionBarrier Transition ;
  [FieldOffset( 0 )] public D3D12_RESOURCE_ALIASING_BARRIER_unmanaged Aliasing ;
  [FieldOffset( 0 )] public D3D12_RESOURCE_UAV_BARRIER_unmanaged UAV ;
 } ;
} ;



[EquivalentOf(typeof(D3D12_RESOURCE_TRANSITION_BARRIER_unmanaged))]
public unsafe struct _resourceTransitionBarrier {
 public ID3D12Resource_unmanaged* pResource ;
 public uint Subresource ;
 public ResourceStates StateBefore ;
 public ResourceStates StateAfter ;
 
} ;