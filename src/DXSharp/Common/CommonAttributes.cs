﻿using System.Runtime.InteropServices ;

namespace DXSharp ;

public interface ITagged { string? Tag { get ; } } ;
public interface INamed { string? Name { get ; } } ;
public interface ITypeRelationship { Type? Type { get ; } } ;
public abstract class DXSharpAttribute: Attribute,
										INamed, ITagged {
	public virtual string Name { get ; }
	public virtual string? Tag { get ; protected set ; }
	protected DXSharpAttribute( ) => this.Name = string.Empty ;
	protected DXSharpAttribute( string name, string? tag = null ) {
		this.Name = name ; this.Tag = tag ;
	}
} ;

// ------------------------------------------------------------------------- 

[AttributeUsage( AttributeTargets.Class | AttributeTargets.Interface | 
				 AttributeTargets.Struct | AttributeTargets.Enum, 
				 AllowMultiple = true)]
public class DirectXAttribute: DXSharpAttribute, ITypeRelationship {
	string _name = string.Empty ;
	public Type Type { get ; init ; }
	public override string Name => _name ;

	public DirectXAttribute( Type type, string? name = null, string? tag = null ):
		base(type.Name) {
		this.Type = type ;
		if( !string.IsNullOrEmpty(name) ) this._name = name ;
		if( !string.IsNullOrEmpty(tag) ) this.Tag = tag ;
	}
} ;


[AttributeUsage( AttributeTargets.Class | AttributeTargets.Interface | 
				 AttributeTargets.Struct | AttributeTargets.Enum,
				 AllowMultiple = false )]
public class EquivalentAttribute: DirectXAttribute {
	public EquivalentAttribute( Type equivalent ): base(equivalent) { }
} ;



[AttributeUsage( AttributeTargets.Class | AttributeTargets.Interface | 
				 AttributeTargets.Struct | AttributeTargets.Enum,
				 AllowMultiple = false)]
public class ProxyForAttribute: DirectXAttribute {
	public ProxyForAttribute( Type other, string? name = null, string? tag = null ): 
		base(other, name, tag) { }
} ;



[AttributeUsage( AttributeTargets.Class | AttributeTargets.Interface | 
				 AttributeTargets.Struct | AttributeTargets.Enum,
				 AllowMultiple = true)]
public class WrapperAttribute: DirectXAttribute {
	public WrapperAttribute( Type wrapped, string? name = null, string? tag = null ): 
		base(wrapped, name, tag) { }
} ;


// -------------------------------------------------------------------------


[AttributeUsage( AttributeTargets.Method | 
				 AttributeTargets.Delegate,
				 AllowMultiple = true )]
public class NativeCallAttribute: DirectXAttribute {
	public NativeCallAttribute( Type? native = null, string? functionName = null, string? tag = null ): 
		base( native ?? typeof(void), functionName, tag ) { }
} ;