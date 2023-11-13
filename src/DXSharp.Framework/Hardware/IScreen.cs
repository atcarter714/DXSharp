using System.Drawing ;
using DXSharp.Framework.Graphics ;

namespace DXSharp.Framework.Hardware ;


/// <summary>Defines a simple, abstract contract for a "screen" or display monitor.</summary>
public interface IScreen {
	// ---------------------------------------------------------
	nint Handle { get ; }
	string DisplayName { get ; }
	Resolution Resolution { get ; }
	Rectangle BoundingRect { get ; }
	
	float DPI { get ; }
	float AspectRatio => Resolution.AspectRatio ;
	// =========================================================
} ;