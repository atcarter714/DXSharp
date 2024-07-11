#region Using Directives
using System.Runtime.InteropServices ;
using Windows.Win32.UI.HiDpi ;
using static Windows.Win32.PInvoke ;
#endregion
namespace DXSharp.Windows.Win32 ;


/// <summary>
/// Identifies the awareness context for a window.
/// </summary>
/// <remarks>
/// In <b><i>windef.h</i></b>, the following values are defined:
/// <code>
/// <b>#define</b> DPI_AWARENESS_CONTEXT_UNAWARE              ((DPI_AWARENESS_CONTEXT)-1)
/// <b>#define</b> DPI_AWARENESS_CONTEXT_SYSTEM_AWARE         ((DPI_AWARENESS_CONTEXT)-2)
/// <b>#define</b> DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE    ((DPI_AWARENESS_CONTEXT)-3)
/// <b>#define</b> DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 ((DPI_AWARENESS_CONTEXT)-4)
/// <b>#define</b> DPI_AWARENESS_CONTEXT_UNAWARE_GDISCALED    ((DPI_AWARENESS_CONTEXT)-5)
/// </code><para/>
/// For more details, see
/// <a href="https://blogs.windows.com/windowsdeveloper/2017/05/19/improving-high-dpi-experience-gdi-based-desktop-apps/#Uwv9gY1SvpbgQ4dK.97">
/// Improving the high-DPI experience in GDI-based Desktop apps</a>.
/// </remarks>
[NativeLibrary( "user32.dll", nameof(DPI_AWARENESS_CONTEXT), "windef.h")]
[EquivalentOf(typeof(DPI_AWARENESS_CONTEXT))]
readonly partial struct DPIAwarenessContext: 
	IEquatable< DPIAwarenessContext > {
	public readonly nint Value ;
	
	public DPIAwarenessContext( nint value ) => this.Value = value ;
	
	
	public override bool Equals( object? obj ) => 
		obj is DPIAwarenessContext other && this.Equals( other ) ;
	public bool Equals( DPIAwarenessContext other ) => this.Value == other.Value ;
	
	public override int    GetHashCode( ) => this.Value.GetHashCode( ) ;
	public override string ToString( )    => $"0x{this.Value:x}" ;
	
	
	public static implicit operator nint( DPIAwarenessContext value ) => value.Value ;
	public static explicit operator DPIAwarenessContext( nint value ) => new( value ) ;

	public static bool operator ==( in DPIAwarenessContext left, in DPIAwarenessContext right ) => left.Value == right.Value ;
	public static bool operator !=( in DPIAwarenessContext left, in DPIAwarenessContext right ) => !( left == right ) ;
} ;