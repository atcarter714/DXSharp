namespace DXSharp.Applications ;

public static class HardwareInfo {
	public static int AvailableProcessors => Environment.ProcessorCount ;
	public static int MaxParallelism => Math.Max( 1, Environment.ProcessorCount - 1 ) ;
	
} ;