
using DXSharp.DXGI ;

namespace Windows.Win32 ;


public partial struct __ushort_2
{
	public static implicit operator ushort2( in __ushort_2 data ) => new( data[0], data[1] ) ;

	public static implicit operator __ushort_2( in ushort2 data ) {
		__ushort_2 result ;
		result[0] = data.x ;
		result[1] = data.y ;
		return result ;
	}
}