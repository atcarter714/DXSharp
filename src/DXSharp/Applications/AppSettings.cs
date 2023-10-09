namespace DXSharp.Applications ;

public struct USize {
	public uint Width, Height ;
	public USize( uint width = 800, uint height = 600 ) => 
		( Width, Height ) = ( width, height ) ;
	
	public static implicit operator USize( (uint width, uint height) size ) =>
	 		new( size.width, size.height ) ;
	public static implicit operator (uint width, uint height)( USize size ) =>
	 		( size.Width, size.Height ) ;
	public static implicit operator USize( Size size ) =>
	 		new( (uint)size.Width, (uint)size.Height ) ;
	public static implicit operator Size( USize size ) =>
	 		new( (int)size.Width, (int)size.Height ) ;
} ;

public record AppSettings( string Title,
						   USize WindowSize,
						   bool UseWarpDevice = false ) ;