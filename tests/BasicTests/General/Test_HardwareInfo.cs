using DXSharp ;
using DXSharp.Applications ;

namespace BasicTests.General ;


[TestFixture]
public class Test_HardwareInfo {
	[SetUp] public void Setup( ) { }
	[TearDown] public void TearDown( ) { }
	
	[Test, Order(0)] public void Test_HardwareInfo_Properties( ) {
#if DEBUG
		System.Diagnostics.Debug.WriteLine( $"{nameof(HardwareInfo)} Unit Tests started ..." ) ;
#endif
		Assert.That( HardwareInfo.Is64Bit ) ;
		Assert.That( HardwareInfo.MaxParallelism, Is.GreaterThan(2) ) ;
		Assert.That( HardwareInfo.AvailableProcessors, Is.GreaterThan(2) ) ;
		Assert.That( HardwareInfo.PhysicalMemory, Is.GreaterThan(65535) ) ;
		Assert.That( HardwareInfo.VirtualMemory, Is.GreaterThan(65535) ) ;
		Assert.That( HardwareInfo.InstalledMemory, Is.GreaterThan(65535) ) ;
		Assert.That( HardwareInfo.SystemAvailableMemory, Is.GreaterThan(65535) ) ;
		
		/*#warning This is OK for now, on MY machine, but must be removed later!
		Assert.That( (int)HardwareInfo.ProcessorArchitecture, 
											Is.GreaterThan(4) ) ;*/
		
		Assert.That( HardwareInfo.SystemName, Is.Not.Null ) ;
		Assert.That( HardwareInfo.SystemName, Is.Not.Empty ) ;
		
		//! Check system disk drives data:
		Assert.That( HardwareInfo.DiskDrives, Is.Not.Null ) ;
		var _drives = HardwareInfo.DiskDrives ;
		Assert.That( _drives, Is.Not.Null) ;
		Assert.That( _drives, Is.Not.Empty) ;
		foreach ( var drive in _drives ) {
			Assert.That( drive, Is.Not.Null) ;
			Assert.That( drive.IsReady ) ;
			string driveInfo = $"Drive: {drive.Name} - " +
							   $"Type: {drive.DriveType} - " +
							   $"Label: {drive.VolumeLabel} - " +
							   $"Size: {( drive.TotalSize / 1024 )} - " +
							   $"{drive.AvailableFreeSpace}" ;
#if DEBUG
			System.Diagnostics.Debug.WriteLine( driveInfo ) ;
#else
			Console.WriteLine( driveInfo ) ;
#endif
			Assert.That( drive.TotalSize, Is.GreaterThan(0) ) ;
			Assert.That( drive.AvailableFreeSpace, Is.GreaterThan(0) ) ;
		}
	}
} ;