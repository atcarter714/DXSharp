#region Using Directives
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D12 ;
#endregion
namespace DXSharp ;


/// <summary>Contract for types which have a special "tag" string associated with them.</summary>
public interface ITagged { string? Tag { get ; } } ;
/// <summary>Contract for types which have a name string associated with them.</summary>
public interface INamed { string? Name { get ; } } ;

/// <summary>
/// Contract for a <see cref="System.Type"/> with a special relationship
/// to another <see cref="System.Type"/>.
/// </summary>
public interface ITypeRelationship { Type? Type { get ; } } ;



// -------------------------------------------------------------------------
// DXSharpAttributes ::
// -------------------------------------------------------------------------

/// <summary>
/// Abstract base class for all <see cref="Attribute"/> types
/// defined in the <b><c>DXSharp</c></b> libraries ...
/// </summary>
public abstract class DXSharpAttribute: Attribute,
										INamed, ITagged {
	protected string  _name ;
	protected string? _tag = string.Empty ;
	
	/// <summary>Gets the name associated with this <see cref="DXSharpAttribute"/>.</summary>
	public virtual string Name => _name ;
	/// <summary>Gets the "tag" associated with this <see cref="DXSharpAttribute"/>.</summary>
	/// <remarks>
	/// The "<see cref="Tag"/>" property is used to store additional information about an
	/// attribute in <b><c>DXSharp</c></b>. Its meaning and semantics are determined by the attribute type,
	/// the attribute's purpose, and the target/context in which it is used.
	/// </remarks>
	public virtual string? Tag => _tag ;
	
	/// <summary>Creates a new <see cref="DXSharpAttribute"/> instance on the target (below).</summary>
	protected DXSharpAttribute( ) => this._name = string.Empty ;
	/// <summary>
	/// Creates a new <see cref="DXSharpAttribute"/> instance on the target (below)
	/// with the specified name and tag strings.
	/// </summary>
	/// <param name="name">
	/// A name to associate with this <see cref="DXSharpAttribute"/> instance.
	/// </param>
	/// <param name="tag">
	/// A special "tag" to associate with this <see cref="DXSharpAttribute"/> instance.
	/// </param>
	protected DXSharpAttribute( string name, string? tag = null ) {
		this._name = name ; this._tag = tag ;
	}
} ;

// ------------------------------------------------------------------------- 


[AttributeUsage( AttributeTargets.Class | AttributeTargets.Interface | 
				 AttributeTargets.Struct | AttributeTargets.Enum
				 | AttributeTargets.Delegate, AllowMultiple = true)]
public class DirectXAttribute: DXSharpAttribute, ITypeRelationship {
	public Type Type { get ; init ; }
	public override string Name => _name ;
	
	public DirectXAttribute( Type type,
							 string? name = null,
							 string? tag = null ): 
								base((name ?? type.Name),
									 (tag ?? type.GUID.ToString())) {
		this.Type = type ;
		if( !string.IsNullOrEmpty(name) ) this._name = name ;
		if( !string.IsNullOrEmpty(tag) ) this._tag = tag ;
	}
} ;

// ------------------------------------------------------------------------- 


/// <summary>
/// Marks a type as being equivalent to another type. For example, many structs in the
/// <b><c>DXSharp</c></b> libraries are marked as being equivalent to structs in the Win32
/// API and/or created by code generation tools (e.g., <a href="https://github.com/microsoft/CsWin32"><c>CsWin32</c></a>).
/// </summary>
/// <remarks>
/// What exactly constitutes "equivalence" is determined by the type of the <see cref="EquivalentOfAttribute"/>
/// target and the context in which it is used. In the case of structs, equivalence means that they have the exact
/// same size and layout in memory. In the case of classes, equivalence means that they have the same public API
/// or purpose. In the case of enumerations, equivalence means that they have the same values and semantics.
/// User-defined types can use this attribute to indicate that they are equivalent to a type defined in the
/// Win32 API or another library, or for any other purpose.
/// <para/>
/// For an example of struct equivalence, see the <see cref="RECT"/>
/// and <see cref="Rect"/> structures.
/// </remarks>
[AttributeUsage( AttributeTargets.Class | AttributeTargets.Interface | 
				 AttributeTargets.Struct | AttributeTargets.Enum, AllowMultiple = true )]
public class EquivalentOfAttribute: DirectXAttribute {
	public EquivalentOfAttribute( Type equivalent ): base(equivalent) { }
	public EquivalentOfAttribute( string symbolName, string? tag = null ): 
		base(typeof(void), symbolName, tag) { }
} ;



/// <summary>
/// Marks a type as being a proxy for another type. For example, many <see langword="interface"/> types in the
/// <b><c>DXSharp</c></b> libraries are marked as being proxies for native COM/DirectX interfaces. A type is a
/// "proxy" if it is used to represent another type in a different programming language or environment or if it
/// otherwise acts as a "stand-in" for another type and plays the same role in its own environment.
/// </summary>
[AttributeUsage( AttributeTargets.Class | AttributeTargets.Interface | 
				 AttributeTargets.Struct | AttributeTargets.Enum, AllowMultiple = true)]
public class ProxyForAttribute: DirectXAttribute {
	public ProxyForAttribute( Type other, string? name = null, string? tag = null ): 
		base(other, name, tag) { }
} ;



/// <summary>
/// Marks a type as being a wrapper for another type. For example, many <see langword="class"/> types in the
/// <b><c>DXSharp</c></b> libraries are marked as being wrappers for native COM/DirectX interfaces. A type is a
/// "wrapper" if it is used to contain and manage another type in a different programming language or environment.
/// For example, the <see cref="DXSharp.Direct3D12.Device"/> class is a wrapper for the native COM interface
/// <see cref="ID3D12Device"/>. The <see cref="DXSharp.Direct3D12.Device"/> class is a wrapper because it contains
/// an instance of the <see cref="ID3D12Device"/> interface and manages its lifetime and usage.
/// </summary>
[AttributeUsage( AttributeTargets.Class | AttributeTargets.Interface | 
				 AttributeTargets.Struct | AttributeTargets.Enum, AllowMultiple = true)]
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



/// <summary>Contains information about the native library an interop type originates from.</summary>
/// <remarks>
/// This attribute is applied to classes, interfaces, structures, and enumerations that are
/// managed wrappers or proxies for native types (i.e., COM interfaces, C++ classes, etc) ... 
/// </remarks>
[AttributeUsage( AttributeTargets.Class | AttributeTargets.Interface | 
				 AttributeTargets.Struct | AttributeTargets.Enum,
				 AllowMultiple = false )]
public class NativeLibraryAttribute: DXSharpAttribute {
	public string? SymbolName { get ; init ; }
	public string? HeaderFile { get ; init ; }

	public NativeLibraryAttribute( string? dllName    = null,
								   string? symbolName = null,
								   string? headerFile = null,
								   string? tag        = null ):
		base( dllName ?? string.Empty, tag ) {
		this.SymbolName = symbolName ;
		this.HeaderFile = headerFile ;
	}
} ;

// -------------------------------------------------------------------------

public class CsWin32Attribute: DXSharpAttribute {
	public CsWin32Attribute( string? name = null, string? tag = null ): 
		base( name ?? string.Empty, tag ) { }
} ;
