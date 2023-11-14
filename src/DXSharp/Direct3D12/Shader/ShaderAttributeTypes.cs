namespace DXSharp.Direct3D12.Shader ;


public class SemanticAttribute: DXSharpAttribute {
	/// <summary>Indicates if the semantic is a system value (SV) semantic.</summary>
	public bool IsSystemValue { get ; init ; }
	/// <summary>Indicates if the semantic has an index.</summary>
	public bool HasIndex { get ; init ; }
	
	public SemanticAttribute( string name, bool isSystemValue = false, bool hasIndex = false ) {
		this._name         = ( isSystemValue && name.StartsWith("SV_") ) ? name : "SV_" + name ;
		this.IsSystemValue = isSystemValue ;
		this.HasIndex      = hasIndex ;
	}
} ;


public class SystemValueAttribute: SemanticAttribute {
	public SystemValueAttribute( string name, bool hasIndex = false ) :
		base( name, true, hasIndex ) { }
} ;