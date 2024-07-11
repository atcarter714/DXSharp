using System.Runtime.CompilerServices ;

namespace DXSharp ;

public static class MathBin {
	public const long BytesPerKB = 1024,
					  BytesPerMB = 1048576,
					  BytesPerGB = 1073741824,
					  BytesPerTB = 1099511627776 ;
	
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static ushort HIWORD( nuint value ) => (ushort)( ( ((ulong)value) >> 16 ) & 0xFFFFu ) ;
		
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static ushort LOWORD( nuint value ) => (ushort)( ((ulong)value) & 0xFFFFu ) ;

} ;