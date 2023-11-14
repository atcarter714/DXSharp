namespace DXSharp.Framework.Debugging ;


[Flags] public enum SDKModules: uint {
	None       = 0x00000000,
	DXGI       = 0x00000001,
	D3D12      = 0x00000002,
	Dxc        = 0x00000004,
	DXCore     = 0x00000008,
	All        = 0xFFFFFFFF,
} ;


/// <summary>Base attribute of all framework debugging system attributes.</summary>
/// <remarks>
/// The <see cref="DebuggingAttribute"/> marks its target as being part of
/// the debug system or otherwise directly related/dependent.
/// </remarks>
[AttributeUsage( AttributeTargets.Class | AttributeTargets.Interface | 
				 AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Event | 
				 AttributeTargets.Parameter | AttributeTargets.Module | AttributeTargets.Assembly,
				 AllowMultiple = false, Inherited = false)]
public class DebuggingAttribute: DXSharpAttribute {
	// -----------------------------------------------------------------------
	
	public SDKModules PertainsTo { get ; }
	
	// -----------------------------------------------------------------------
	/// <summary>Initializes a new instance of <see cref="DebuggingAttribute"/>.</summary>
	public DebuggingAttribute( SDKModules partOf = SDKModules.None ) => PertainsTo = partOf ;
	
	// -----------------------------------------------------------------------
} ;