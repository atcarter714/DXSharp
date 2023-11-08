#region Using Directives

using System.Runtime.InteropServices ;
using Windows.Win32.Foundation ;

using DXSharp ;
using DXSharp.DXGI ;
using DXSharp.Windows.Win32.Helpers ;

#endregion
namespace Windows.Win32.Graphics.Dxgi ;


[CsWin32, EquivalentOf(typeof(OutputDescription1))]
public partial struct DXGI_OUTPUT_DESC1 {
	public static implicit operator OutputDescription1( in DXGI_OUTPUT_DESC1 src ) {
		unsafe {
			fixed ( DXGI_OUTPUT_DESC1* pSrc = &src )
				return *( (OutputDescription1 *)pSrc ) ;
		}
	}
	public static implicit operator DXGI_OUTPUT_DESC1( in OutputDescription1 src ) {
		unsafe {
			fixed ( OutputDescription1* pSrc = &src )
				return *( (DXGI_OUTPUT_DESC1 *)pSrc ) ;
		}
	}
} ;

[CsWin32, EquivalentOf(typeof(SharedResource))]
public partial struct DXGI_SHARED_RESOURCE {
	public DXGI_SHARED_RESOURCE( HANDLE handle ) => Handle = handle ;
	public DXGI_SHARED_RESOURCE( nint   handle ) => Handle = new( handle ) ;


	public static implicit operator SharedResource( in       DXGI_SHARED_RESOURCE res ) => new( res.Handle ) ;
	public static implicit operator DXGI_SHARED_RESOURCE( in SharedResource       src ) => new( src.Handle ) ;
} ;


[CsWin32, EquivalentOf(typeof(QueryVideoMemoryInfo))]
public partial struct DXGI_QUERY_VIDEO_MEMORY_INFO {
	public DXGI_QUERY_VIDEO_MEMORY_INFO( ulong budget, ulong currentUsage, ulong availableForReservation, ulong currentReservation ) {
		Budget = budget ;
		CurrentUsage = currentUsage ;
		AvailableForReservation = availableForReservation ;
		CurrentReservation = currentReservation ;
	}
	
	public static implicit operator QueryVideoMemoryInfo( in DXGI_QUERY_VIDEO_MEMORY_INFO src ) => 
		new( src.Budget, src.CurrentUsage, src.AvailableForReservation, src.CurrentReservation ) ;
	
	public static implicit operator DXGI_QUERY_VIDEO_MEMORY_INFO( in QueryVideoMemoryInfo src ) =>
	 		new( src.Budget, src.CurrentUsage, src.AvailableForReservation, src.CurrentReservation ) ;
} ;


[CsWin32, EquivalentOf(typeof(AdapterDescription3))]
public partial struct DXGI_ADAPTER_DESC3 {
	public DXGI_ADAPTER_DESC3( uint vendorId, uint deviceId, uint subSysId, uint revision,
								nuint dedicatedVideoMemory, nuint dedicatedSystemMemory, nuint sharedSystemMemory,
								LUID adapterLuid,
								AdapterFlag3 flags = AdapterFlag3.None,
								GraphicsPreemptionGranularity graphicsPreemption = default,
								ComputePreemptionGranularity computePreemption = default ) {
		VendorId = vendorId ;
		DeviceId = deviceId ;
		SubSysId = subSysId ;
		Revision = revision ;
		DedicatedVideoMemory = dedicatedVideoMemory ;
		DedicatedSystemMemory = dedicatedSystemMemory ;
		SharedSystemMemory = sharedSystemMemory ;
		AdapterLuid = adapterLuid ;
		Flags = (DXGI_ADAPTER_FLAG3)flags ;
		GraphicsPreemptionGranularity = (DXGI_GRAPHICS_PREEMPTION_GRANULARITY)graphicsPreemption ;
		ComputePreemptionGranularity = (DXGI_COMPUTE_PREEMPTION_GRANULARITY)computePreemption ;
	}
	
	public static implicit operator AdapterDescription3( in DXGI_ADAPTER_DESC3 src ) =>
		new( src.Description, src.VendorId, src.DeviceId, src.SubSysId, 
			 src.Revision, src.DedicatedVideoMemory, src.DedicatedSystemMemory, 
			 src.SharedSystemMemory, src.AdapterLuid, 
			 (AdapterFlag3)src.Flags,
			 (GraphicsPreemptionGranularity)src.GraphicsPreemptionGranularity,
			 (ComputePreemptionGranularity)src.ComputePreemptionGranularity ) ;
	
	public static implicit operator DXGI_ADAPTER_DESC3( in AdapterDescription3 src ) =>
		new( src.VendorId, src.DeviceId, src.SubSysId, src.Revision, 
			 src.DedicatedVideoMemory, src.DedicatedSystemMemory, src.SharedSystemMemory, 
			 src.AdapterLuid, src.Flags,
			 src.GraphicsPreemptionGranularity,
			 src.ComputePreemptionGranularity ) ;
} ;


[CsWin32, EquivalentOf(typeof(HDRMetaDataHDR10))]
public partial struct DXGI_HDR_METADATA_HDR10
{
	public DXGI_HDR_METADATA_HDR10( ushort2 redPrimary, ushort2 greenPrimary, ushort2 bluePrimary, ushort2 whitePoint,
									uint maxMasteringLuminance, uint minMasteringLuminance,
									ushort maxContentLightLevel, ushort maxFrameAverageLightLevel ) {
		RedPrimary = redPrimary ;
		GreenPrimary = greenPrimary ;
		BluePrimary = bluePrimary ;
		WhitePoint = whitePoint ;
		MaxMasteringLuminance = maxMasteringLuminance ;
		MinMasteringLuminance = minMasteringLuminance ;
		MaxContentLightLevel = maxContentLightLevel ;
		MaxFrameAverageLightLevel = maxFrameAverageLightLevel ;
	}
	
	public static implicit operator HDRMetaDataHDR10( in DXGI_HDR_METADATA_HDR10 src ) =>
		new( src.RedPrimary, src.GreenPrimary, src.BluePrimary, src.WhitePoint,
			 src.MaxMasteringLuminance, src.MinMasteringLuminance,
			 src.MaxContentLightLevel, src.MaxFrameAverageLightLevel ) ;
	
	public static implicit operator DXGI_HDR_METADATA_HDR10( in HDRMetaDataHDR10 src ) =>
		new( src.RedPrimary, src.GreenPrimary, src.BluePrimary, src.WhitePoint,
			 src.MaxMasteringLuminance, src.MinMasteringLuminance,
			 src.MaxContentLightLevel, src.MaxFrameAverageLightLevel ) ;
}

