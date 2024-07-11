using DXSharp.Framework.Graphics ;

namespace DXSharp.Framework.XTensions ;

public static class DataTypeXTensions {
	public static Viewport ToViewport( this Resolution resolution ) =>
		new( resolution.Width, resolution.Height ) ;
	
	
} ;