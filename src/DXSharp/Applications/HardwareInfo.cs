using Windows.Win32 ;
using Windows.Win32.System.SystemInformation ;
namespace DXSharp.Applications ;


/// <summary>
/// Provides important information about the system's hardware and capabilities.
/// </summary>
public static class HardwareInfo {
	public static bool Is64Bit => Environment.Is64BitOperatingSystem ;
	public static ProcessorArchitecture ProcessorArchitecture { get ; }
	public static int AvailableProcessors => Environment.ProcessorCount ;
	public static int MaxParallelism => Mathf.Max( 1, Environment.ProcessorCount - 1 ) ;
	
	/// <summary>The number of bytes of the computer's free virtual address space.</summary>
	public static ulong VirtualMemory { get ; }
	public static ulong PhysicalMemory { get ; }
	public static ulong InstalledMemory { get ; }
	public static ulong SystemAvailableMemory =>
				VirtualMemory + PhysicalMemory ;
	
	public static readonly DriveInfo[ ] DiskDrives ;
	public static string SystemName => Environment.MachineName ;
	
	public static AddressRange SystemAddressRange { get ; }
	
	
	
	static HardwareInfo( ) {
		// Get the disk/storage drive info for all drives:
		var drives = Environment.GetLogicalDrives( ) ;
		DiskDrives = new DriveInfo[ drives.Length ] ;
		for( int i = 0; i < drives.Length; ++i )
			DiskDrives[ i ] = new DriveInfo( drives[ i ] ) ;
		
		//! Really easy way to get basic memory info fast:
		Microsoft.VisualBasic.Devices.ComputerInfo info = new( ) ;
		VirtualMemory  = info.AvailableVirtualMemory ;
		PhysicalMemory = info.AvailablePhysicalMemory ;
		
		// Get the amount of physically installed memory:
		PInvoke.GetPhysicallyInstalledSystemMemory( out ulong totalKBMemory ) ;
		InstalledMemory = ( totalKBMemory * 1024 ) ;

		// Get advanced system info:
		PInvoke.GetSystemInfo( out var lpSystemInfo ) ;
		var processorArch = 
			lpSystemInfo.Anonymous.Anonymous.wProcessorArchitecture ;
		ProcessorArchitecture = (ProcessorArchitecture)processorArch ;
		var processorType = lpSystemInfo.dwProcessorType ;
		var processorLevel = lpSystemInfo.wProcessorLevel ;
		var processorRevision = lpSystemInfo.wProcessorRevision ;
		var numberOfProcessors = lpSystemInfo.dwNumberOfProcessors ;
		var pageSize = lpSystemInfo.dwPageSize ;
		
		unsafe {
			nint minimumApplicationAddress =
				(nint)lpSystemInfo.lpMinimumApplicationAddress ;
			nint maximumApplicationAddress =
				(nint)lpSystemInfo.lpMaximumApplicationAddress ;
			SystemAddressRange = new( minimumApplicationAddress,
									  maximumApplicationAddress ) ;
		}
	}
} ;

