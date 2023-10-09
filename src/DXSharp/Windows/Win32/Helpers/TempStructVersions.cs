using System.Diagnostics ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Gdi ;

namespace DXSharp.Windows.Win32 ;


[DebuggerDisplay("{Value}")]
public struct HMonitor: IEquatable< HMonitor >,
						IEquatable< HMONITOR > {
	public readonly nint Value ;
	public HMonitor( nint value ) => Value = value ;
	public HMonitor( in Win32Handle handle ) => Value = handle.Value ;
	public HMonitor( in HMONITOR hMonitor ) => Value = hMonitor.Value ;
	
	public override int GetHashCode( ) => Value.GetHashCode( ) ;
	public bool Equals( HMonitor other ) => Value == other.Value ;
	public bool Equals( HMONITOR other ) => Value == other.Value ;
	public override bool Equals( object?  obj ) =>
			obj is HMonitor other && Equals( other ) ;
	
	public static implicit operator nint( HMonitor hMonitor ) => hMonitor.Value ;
	public static implicit operator HMONITOR( HMonitor hMonitor ) => new( hMonitor.Value ) ;
	public static implicit operator HMonitor( nint value ) => new( value ) ;
	public static implicit operator HMonitor( in Win32Handle handle ) => new( handle ) ;
	
	public static bool operator ==( HMonitor left, HMonitor right ) => left.Value == right.Value ;
	public static bool operator !=( HMonitor left, HMonitor right ) => left.Value != right.Value ;
	public static bool operator ==( HMonitor left, nint right ) => left.Value == right ;
	public static bool operator !=( HMonitor left, nint right ) => left.Value != right ;
	public static bool operator ==( nint left, HMonitor right ) => left == right.Value ;
	public static bool operator !=( nint left, HMonitor right ) => left != right.Value ;
} ;

[DebuggerDisplay("{Value}")]
public readonly struct HModule: IEquatable< HModule >,
								IEquatable< HMODULE > {
	public readonly nint Value ;
	public HModule( nint value ) => Value = value ;
	
	public override int GetHashCode( ) => Value.GetHashCode( ) ;
	public bool Equals( HModule other ) => Value == other.Value ;
	public override bool Equals( object? obj ) => obj is HModule other && Equals( other ) ;
	public bool Equals( HMODULE other ) => Value == other.Value ;
	 
	public static implicit operator nint( HModule hModule ) => hModule.Value ;
	public static implicit operator HModule( nint value ) => new( value ) ;
	public static implicit operator HModule( in HMODULE hModule ) => new( hModule.Value ) ;
	public static implicit operator HMODULE( HModule hModule ) => new( hModule.Value ) ;
	
	public static bool operator ==( HModule left, nint right ) => left.Value == right ;
	public static bool operator !=( HModule left, nint right ) => left.Value != right ;
	public static bool operator ==( HModule left, HModule right ) => left.Value == right.Value ;
	public static bool operator !=( HModule left, HModule right ) => left.Value != right.Value ;
	public static bool operator ==( HModule left, HMODULE right ) => left.Value == right.Value ;
	public static bool operator !=( HModule left, HMODULE right ) => left.Value != right.Value ;
	public static bool operator ==( HMODULE left, HModule right ) => left.Value == right.Value ;
	public static bool operator !=( HMODULE left, HModule right ) => left.Value != right.Value ;
} ;