
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp ;
using DXSharp.DXGI ;


//[ProxyFor(typeof(D3D12_FEATURE_DATA_ARCHITECTURE1))]
/// <summary>
/// See documentation for: 
/// <a href="https://docs.microsoft.com/en-us/windows/win32/api/d3d12/ns-d3d12-d3d12_feature_data_architecture1">D3D12_FEATURE_DATA_ARCHITECTURE1</a>
/// </summary>
/// <remarks>
/// Update to D3D12 since Windows 10 Creators Update (1703).
/// Used by <see cref="ID3D12Device.CheckFeatureSupport"/>.
/// </remarks>
public struct FeatureDataArchitecture1 {
	public uint NodeIndex ;
	public BOOL TileBasedRenderer;
	public BOOL UMA;
	public BOOL CacheCoherentUMA;
	public BOOL IsolatedMMU;
} ;