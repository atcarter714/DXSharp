namespace HelloTexture ;

public class GraphicsSettings {
	public static readonly GraphicsSettings Default = new( ) {
		BufferCount = 2,
	} ;
	
	public uint BufferCount { get; set ; } = 2 ;
	
	public GraphicsSettings( ) { }
	public GraphicsSettings( uint bufferCount ) {
		BufferCount = bufferCount ;
	}
}