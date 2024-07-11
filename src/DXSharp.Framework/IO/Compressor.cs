using System.Diagnostics ;
using System.IO.Compression ;

namespace DXSharp.Framework.IO ;

public class Compressor {
	
	public static byte[ ] Compress( Span< byte > data ) {
		Debug.Assert( data is { Length: > 0 } ) ;
		using MemoryStream  mem     = new( ) ;
		using DeflateStream deflate = 
			new( mem, CompressionLevel.Optimal, true ) ;
		
		deflate.Write( data ) ;
		return mem.ToArray( ) ;
	}
	
	public static byte[ ] Decompress( byte[ ] data ) {
		Debug.Assert( data is { Length: > 0 } ) ;
		using DeflateStream deflate = 
			new( new MemoryStream(data), 
				 CompressionMode.Decompress ) ;
		using MemoryStream  result  = new( ) ;
		deflate.CopyTo( result) ;
		
		return result.ToArray( ) ;
	}
}