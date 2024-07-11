using Windows.Win32 ;
using Windows.Win32.Graphics.DXCore ;

namespace DXSharp.DXCore ;

//! TODO: Implement DXCore
/* NOTE :: Implementation needed for the following functions & COM types:
 * -----------------------------------------------------------------------
	 * PInvoke.DXCoreCreateAdapterFactory
	 * IDXCoreAdapterFactory
	 * IDXCoreAdapterList
	 * IDXCoreAdapter
 * -----------------------------------------------------------------------
 * Also note that a few data types/structures need to be implemented, such as:
 * DXCoreSegmentGroup, PFN_DXCORE_NOTIFICATION_CALLBACK, DXCoreAdapterMemoryBudget,
 * DXCoreAdapterMemoryBudgetNodeSegmentGroup, DXCoreHardwareID, etc ...
 *
 * Doc: https://learn.microsoft.com/en-us/windows/win32/dxcore/dxcore
 * -----------------------------------------------------------------------
*/


/// <summary>
/// <b>DXCore</b> is an adapter enumeration API for graphics and compute devices, so some
/// of its facilities overlap with those of DXGI. DXCore is used by Direct3D 12 ...
/// </summary>
/// <remarks>
/// <para>
/// For more information, see the
/// <a href="https://learn.microsoft.com/en-us/windows/win32/dxcore/dxcore">DXCore documentation</a>.
/// </para>
/// </remarks>
public static class DxCore {
	
	/// <summary>DXCORE_ADAPTER_ATTRIBUTE_D3D11_GRAPHICS</summary>
	public static readonly Guid D3D11_Graphics = new( "8c47866b-7583-450d-f0f0-6bada895af4b" ) ;

	/// <summary>DXCORE_ADAPTER_ATTRIBUTE_D3D12_GRAPHICS</summary>
	public static readonly Guid D3D12_Graphics = new( "0c9ece4d-2f6e-4f01-8c96-e89e331b47b1" ) ;

	/// <summary>DXCORE_ADAPTER_ATTRIBUTE_D3D12_CORE_COMPUTE</summary>
	public static readonly Guid D3D12_CoreCompute = new( "248e2800-a793-4724-abaa-23a6de1be090" ) ;
	
	
	public static void CreateAdapterFactory( in Guid riid, out object? factory ) {
		throw new NotImplementedException( ) ;
		//PInvoke.DXCoreCreateAdapterFactory( riid, out factory ) ;
	}
	
	
} ;