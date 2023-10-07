using System.Diagnostics ;
using DXSharp.Windows.Win32 ;
using Windows.Win32.Graphics.Gdi ;

namespace DXSharp.Windows ;


[DebuggerDisplay("{Value}")]
public struct HMonitor: IEquatable< HMonitor > {
	public readonly nint Value ;
	public HMonitor( nint value ) => Value = value ;
	public HMonitor( in Win32Handle handle ) => Value = handle.Value ;
	public HMonitor( in HMONITOR hMonitor ) => Value = hMonitor.Value ;
	
	public override int GetHashCode( ) => Value.GetHashCode( ) ;
	public bool Equals( HMonitor other ) => Value == other.Value ;
	public override bool Equals( object? obj ) => obj is HMonitor other && Equals( other ) ;

	
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