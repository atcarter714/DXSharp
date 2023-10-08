﻿using System.Runtime.InteropServices ;
using Windows.Win32.Foundation ;

namespace DXSharp ;

[StructLayout(LayoutKind.Sequential)]
public struct Rect {
	public int Left, Top, Right, Bottom ;
	
	public Rect( Rect rect ) {
		Left  = rect.Left ; Top     = rect.Top ; 
		Right = rect.Right ; Bottom = rect.Bottom ;
	}
	public Rect( in RECT rect ) {
		Left  = rect.left ; Top     = rect.top ; 
		Right = rect.right ; Bottom = rect.bottom ;
	}
	public Rect( int left  = 0, int top    = 0, 
				 int right = 0, int bottom = 0 ) {
		Left  = left ; Top     = top ; 
		Right = right ; Bottom = bottom ;
	}
	public unsafe Rect( RECT* pRect ) {
		unsafe { fixed( Rect* pThis = &this )
			*((RECT*)pThis) = *pRect ;
		}
	}
	public unsafe Rect( RECT rect ): this( &rect ) { }
	
	public static unsafe implicit operator Rect( RECT rect ) => new( &rect ) ;
	public static implicit operator RECT( in Rect rect ) {
		RECT r = default ;
		unsafe { *( (Rect *)&r ) = rect ; }
		return r ;
	}
	public static RECT FromXYWH( int x, int y, int width, int height ) =>
		new RECT( x, y, unchecked( x + width ), unchecked( y + height ) ) ;
	
	public readonly Size Size => new( this.Width, this.Height ) ;
	public readonly int Width => unchecked( this.Right - this.Left ) ;
	public readonly int Height => unchecked( this.Bottom - this.Top ) ;
	public readonly int X => this.Left ; public readonly int Y => this.Top ;
	public readonly bool IsEmpty => this is { Left: 0, Top: 0, Right: 0, Bottom: 0 } ;

	public static implicit operator Rect( Rectangle value ) => new RECT( value ) ;
	public static implicit operator Rectangle( Rect value ) => new( value.Left, value.Top,
																	value.Width, value.Height ) ;
	public static implicit operator RectangleF( Rect value ) => new( value.Left, value.Top,
																	 value.Width, value.Height ) ;
} ;