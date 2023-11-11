using System.Runtime.CompilerServices ;

namespace DXSharp ;

public static class MathBin {
	
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static ushort HIWORD( nuint value ) => (ushort)( ( ((ulong)value) >> 16 ) & 0xFFFFu ) ;
		
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static ushort LOWORD( nuint value ) => (ushort)( ((ulong)value) & 0xFFFFu ) ;

} ;