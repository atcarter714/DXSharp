#region Using Directives
using System.Numerics ;
using System.Runtime.CompilerServices ;

using SysVec2 = System.Numerics.Vector2 ;
using SysVec3 = System.Numerics.Vector3 ;
using SysVec4 = System.Numerics.Vector4 ;
using DXVec2 = DXSharp.Vector2 ;
using DXVec3 = DXSharp.Vector3 ;
using DXVec4 = DXSharp.Vector4 ;

using DXSharp ;
#endregion
namespace Windows.Win32 ;


public partial struct __float_4 {
	
	public __float_4( float x = 0, float y = 0, float z = 0, float w = 0 ) {
		unsafe {
			Unsafe.SkipInit( out this ) ;
			Value[ 0 ] = x ;
			Value[ 1 ] = y ;
			Value[ 2 ] = z ;
			Value[ 3 ] = w ;
		}
	}
	
	public __float_4( in ColorF colorF ) {
		unsafe {
			Unsafe.SkipInit( out this ) ;
			fixed( void* pColorF = &colorF, pValue = Value )
				*( (__float_4 *)pValue ) = *( (__float_4 *)pColorF ) ;
		}
	}

	public __float_4( in Color color ) {
		unsafe {
			Unsafe.SkipInit( out this ) ;
			Value[ 0 ] = color.R ;
			Value[ 1 ] = color.G ;
			Value[ 2 ] = color.B ;
			Value[ 3 ] = color.A ;
		}
	}
	
	public __float_4( in DXVec4 vec ) {
		unsafe {
			Unsafe.SkipInit( out this ) ;
			fixed( void* pVec = &vec, pValue = Value )
				*( (__float_4 *)pValue ) = *( (__float_4 *)pVec ) ;
		}
	}
	
	public __float_4( in DXVec3 vec, float w = 0 ) {
		unsafe {
			Unsafe.SkipInit( out this ) ;
			Value[ 0 ] = vec.X ;
			Value[ 1 ] = vec.Y ;
			Value[ 2 ] = vec.Z ;
			Value[ 3 ] = w ;
		}
	}
	
} ;