#region Using Directives
using System.Collections.ObjectModel ;
using System.Globalization ;
using Windows.Win32.Foundation ;
using DXSharp.XTensions ;
#endregion
namespace DXSharp.Direct3D12.Shader ;


/// <summary>Defines a simple contract for a shader semantic name and value.</summary>
public interface ISemantic {
	ShaderSemantic Value { get ; }
	bool HasIndex { get ; }
	string Name { get ; }
	int Index { get ; }
} ;


/// <summary>Represents a Direct3D shader semantic and provides ways to create and use them.</summary>
public class Semantics: ISemantic,
						IFormattable,
						IEquatable< ISemantic >, IEquatable< Semantics > {
	// ------------------------------------------------------------------------
	public bool HasIndex => Index is > -1 && Names.HasSemanticIndex( Name ) ;
	public string Name => Names.NamesBySemanticLookup[ Value ] ;
	public ShaderSemantic Value { get ; }
	public int Index { get ; }
	// ------------------------------------------------------------------------
	
	public Semantics( ShaderSemantic value = default, int? index = null ) {
		Value      = value ;
		this.Index = index ?? -1 ;
	}
	public Semantics( string? name = null, int? index = null ) {
#if DEBUG || DEV_BUILD
		if( !Names.IsValidSemantic(name) ) throw new ArgumentException( ) ;
#endif
		Value = Names.NamesBySemanticLookup
						 .First( kvp => (kvp.Value == name) )
							.Key ;
		
		this.Index = index ?? -1 ;
	}
	
	// ------------------------------------------------------------------------
	
	#region System Overrides
	public override bool Equals( object? obj ) =>
			( obj is ISemantic other ) ? Equals( other ) :
				( obj is Semantics otherSem ) && Equals( otherSem ) ;
	
	public bool Equals( ISemantic? other ) =>
						( other?.Value == Value )
							&& !HasIndex || (other?.Index == Index) ;
	
	public bool Equals( Semantics? other ) {
		if ( other is null ) return false ;
		if ( ReferenceEquals(this, other) ) return true ;
		return Value == other.Value &&
			   (HasIndex && Index == other.Index) 
			   || !HasIndex ;
	}
	
	public override int GetHashCode( ) => HashCode.Combine( Name, Value, HasIndex, Index ) ;
	public override string ToString( ) => ToString( "G", CultureInfo.CurrentCulture ) ;
	public string ToString( string? format, IFormatProvider? formatProvider = null ) {
		if( format is null ) format = "G" ;
		if( formatProvider is null ) formatProvider = CultureInfo.CurrentCulture ;

		string _formattedStr =
			HasIndex ? string.Format( formatProvider, 
									  Name, Index.ToString(format) ) : Name ;
		
		return _formattedStr ;
	}
	#endregion
	
	// ------------------------------------------------------------------------
	
	public static implicit operator ShaderSemantic( Semantics semantic ) => semantic.Value ;
	public static implicit operator string( Semantics semantic ) => semantic.ToString( ) ;
	public static implicit operator Semantics( ShaderSemantic value ) => new( value ) ;
	public static implicit operator Semantics( string name ) => new( name ) ;
	
	public static implicit operator Semantics( (ShaderSemantic, int) semantic ) => 
		new( semantic.Item1, semantic.Item2 ) ;
	
	public static bool operator !=( Semantics left, ISemantic right ) => !left.Equals( right ) ;
	public static bool operator ==( Semantics left, ISemantic right ) => left.Equals( right ) ;
	public static bool operator !=( ISemantic left, Semantics right ) => !right.Equals( left ) ;
	public static bool operator ==( ISemantic left, Semantics right ) => right.Equals( left ) ;
	
	// ------------------------------------------------------------------------
	/// <summary>
	/// A specialized helper class for working with Direct3D shader semantics
	/// and their names (in the form of <see cref="string"/> and/or unmanaged
	/// strings and <see cref="char"/> buffers).
	/// </summary>
	public static class Names {
		#region Const Strings
		/// <summary>
		/// A <b>const</b> <see cref="string"/> storing the
		/// SV prefix (<i>"SV_"</i>) of HLSL shader semantics.
		/// </summary>
		public const string SV_PREFIX = "SV_" ;
		
		//! Common Semantics Names:
		public const string Binormal     = "BINORMAL{0}",
							BlendIndices = "BLENDINDICES{0}",
							BlendWeight  = "BLENDWEIGHT{0}",
							Color        = "COLOR{0}",
							Normal       = "NORMAL{0}",
							PositionN    = "POSITION{0}",
							PositionT    = "POSITIONT",
							PSize        = "PSIZE{0}",
							Tangent      = "TANGENT{0}",
							TexCoord     = "TEXCOORD{0}" ;
		
		//! SV Semantics Names:
		public const string ClipDistance           = "SV_ClipDistance{0}",
							CullDistance           = "SV_CullDistance{0}",
							Coverage               = "SV_Coverage",
							Depth                  = "SV_Depth",
							DepthGreaterEqual      = "SV_DepthGreaterEqual",
							DepthLessEqual         = "SV_DepthLessEqual",
							DispatchThreadID       = "SV_DispatchThreadID",
							DomainLocation         = "SV_DomainLocation",
							GroupID                = "SV_GroupID",
							GroupIndex             = "SV_GroupIndex",
							GroupThreadID          = "SV_GroupThreadID",
							GSInstanceID           = "SV_GSInstanceID",
							InnerCoverage          = "SV_InnerCoverage",
							InsideTessFactor       = "SV_InsideTessFactor",
							InstanceID             = "SV_InstanceID",
							IsFrontFace            = "SV_IsFrontFace",
							OutputControlPointID   = "SV_OutputControlPointID",
							Position               = "SV_Position",
							PrimitiveID            = "SV_PrimitiveID",
							RenderTargetArrayIndex = "SV_RenderTargetArrayIndex",
							SampleIndex            = "SV_SampleIndex",
							StencilRef             = "SV_StencilRef",
							Target                 = "SV_Target{0}",
							VertexID               = "SV_VertexID",
							ViewportArrayIndex     = "SV_ViewportArrayIndex",
							ShadingRate            = "SV_ShadingRate" ;
		#endregion
		
		// ------------------------------------------------------------------------
		// Static Lookup Dictionaries & Data Collections:
		// ------------------------------------------------------------------------
		
		static readonly char[ ] _indexFormattingChars = { '{', '0', '}' } ;
		
		internal static readonly ReadOnlyDictionary< ShaderSemantic, string > NamesBySemanticLookup ;
		internal static readonly ReadOnlyDictionary< string, ShaderSemantic > ValuesByNameLookup ;
		
		//! Collection of all ShaderSemantic enum/flag values:
		internal static readonly ReadOnlyMemory< ShaderSemantic > AllSemanticValues =
			Enum.GetValues( typeof(ShaderSemantic) ) as ShaderSemantic[ ]
			?? throw new DXSharpException( $"{nameof(Names)} :: " +
										   $"Failed to get all semantic enum values." ) ;
		
		/// <summary>A read-only collection containing the names of all Direct3D shader semantics.</summary>
		internal static readonly ReadOnlyCollection< string > AllNames = new( new[ ] {
			// ------------------------------------------------------------------------------
			Binormal,  BlendIndices, BlendWeight, Color,   Normal,
			PositionN, PositionT,    PSize,       Tangent, TexCoord,
			// ------------------------------------------------------------------------------
			ClipDistance,         CullDistance,     Coverage,         Depth,
			DepthGreaterEqual,    DepthLessEqual,   DispatchThreadID, DomainLocation,
			GroupID,              GroupIndex,       GroupThreadID,    GSInstanceID,
			InnerCoverage,        InsideTessFactor, InstanceID,       IsFrontFace,
			OutputControlPointID, Position,         PrimitiveID,      RenderTargetArrayIndex,
			SampleIndex,          StencilRef,       Target,           VertexID,
			ViewportArrayIndex,   ShadingRate,
			// ------------------------------------------------------------------------------
		} ) ;
		
		internal static readonly IReadOnlyList< ShaderSemantic > AllIndexedValues ;
		internal static readonly ReadOnlyMemory< string > AllIndexedNames =
			AllNames.Where( HasIndexFormatting ).ToArray( ) ;
		
		internal static readonly ReadOnlyMemory< string > AllPrefixedNames =
			AllNames.Where( s => s.StartsWith(SV_PREFIX) )
											.ToArray( ) ;
		
		internal static readonly ReadOnlyMemory< string > AllUnprefixedNames =
			AllNames.Where( s => !s.StartsWith(SV_PREFIX) ).ToArray( ) ;
		
		// ========================================================================
		
		static Names( ) {
			int nTotalSemantics  = AllNames.Count ;
			Dictionary< ShaderSemantic, string > _namesBySemantic = new( ) ;
			
			// Create a lookup dictionary for all semantic names by enum value:
			for ( int i = 0; i < nTotalSemantics; ++i ) {
				var nextName          = AllNames[ i ] ;
				var nextValue = AllSemanticValues.Span[ i ] ;
				_namesBySemantic.Add( nextValue, nextName ) ;
			}
			NamesBySemanticLookup = new( _namesBySemantic ) ;
			
			// Create a lookup dictionary for all semantic values by name:
			Dictionary< string, ShaderSemantic > _semanticsByName = new( ) ;
			for ( int i = 0; i < nTotalSemantics; ++i ) {
				var nextName          = AllNames[ i ] ;
				var nextValue = AllSemanticValues.Span[ i ] ;
				_semanticsByName.Add( nextName, nextValue ) ;
			}
			ValuesByNameLookup = new( _semanticsByName ) ;
			
			
			// Create a collection of all semantic values that have index formatting:
			var allIndexedNames = AllNames
										  .Where( HasIndexFormatting )
											.ToArray( ) ;
			
			// Create a lookup dictionary for all semantic values that have index formatting:
			var indexedValues = new ShaderSemantic[ allIndexedNames.Length ] ;
			for ( int i = 0; i < allIndexedNames.Length; ++i ) {
				var nextName = allIndexedNames[ i ] ;
				var nextValue = ValuesByNameLookup[ nextName ] ;
				indexedValues[ i ] = nextValue ;
			}
			AllIndexedValues = indexedValues ;
		}
		
		// ------------------------------------------------------------------------
		// Static Methods:
		// ------------------------------------------------------------------------
		
		/// <summary>
		/// Indicates if the semantic name includes formatting characters to be replaced by a digit or "index" character value
		/// (e.g., <c>"SV_Target{0}"</c> or <c>"SV_ClipDistance{0}"</c>). 
		/// </summary>
		/// <param name="semanticName">The semantic name to check for the presence of an index value.</param>
		/// <returns>
		/// <see langword="true"/> if the semantic name includes a digit or "index" character value;
		/// otherwise, <see langword="false"/>.
		/// </returns>
		/// <remarks>
		/// Use <see cref="HasSemanticIndex"/> to check for an actual semantic index value
		/// (e.g., the <c>'1'</c> in <c>"SV_Target1"</c> or <c>'2'</c> in <c>"SV_ClipDistance2"</c>).
		/// This method only checks for the presence of formatting characters that would be replaced
		/// by a digit or "index" character value when a complete semantic name is constructed.
		/// </remarks>
		public static bool HasIndexFormatting( string semanticName ) => 
										semanticName.EndsWith( '}' )
											|| semanticName.AsSpan( )
													  .IndexOfAny( _indexFormattingChars ) != -1 ;
		
		/// <summary>Indicates if the semantic name includes a semantic index value.</summary>
		/// <param name="semanticName">The semantic name to check for the presence of a semantic index value.</param>
		/// <returns>
		/// <see langword="true"/> if the semantic name includes a semantic index value;
		/// otherwise, the method returns <see langword="false"/>.
		/// </returns>
		/// <remarks>
		/// Use <see cref="HasIndexFormatting"/> to check for the presence of the formatting characters
		/// that would be replaced by a digit or "index" character value when a complete semantic name
		/// is constructed. This method simply checks for an actual semantic index value in a name 
		/// (e.g., the <c>'1'</c> in <c>"SV_Target1"</c> or <c>'2'</c> in <c>"SV_ClipDistance2"</c>).
		/// </remarks>
		public static bool HasSemanticIndex( string semanticName ) => 
			semanticName.EndsWithAny( char.IsDigit ) ;
		
		/// <summary>Indicates if the specified <see cref="string"/> is a valid Direct3D shader semantic name.</summary>
		/// <param name="semanticName">The name of a shader semantic (in <see cref="string"/> form).</param>
		/// <returns>
		/// <see langword="true"/> if the specified <see cref="string"/> is a valid Direct3D shader semantic name;
		/// otherwise, the method returns <see langword="false"/>.
		/// </returns>
		public static bool IsValidSemantic( string? semanticName ) {
			if( string.IsNullOrEmpty(semanticName) ) return false ;
			
			bool hasEntry = AllNames.Contains(semanticName) ;
			if( hasEntry ) return true ;
			
			// Does name have index formatting or digits?
			if( HasIndexFormatting(semanticName)
				|| semanticName.EndsWithAny(char.IsDigit) ) {
				var trimmed = semanticName.TrimEnd( _indexFormattingChars ) ;
				hasEntry = AllNames.Any( str=> str.StartsWith(trimmed) ) ;
				return hasEntry ;
			}
			
			return false ;
		}
		
		public static bool IsFormatted( string semanticName ) =>
									IsValidSemantic( semanticName )
										&& !HasIndexFormatting( semanticName )
												&& semanticName.EndsWithAny( char.IsDigit ) ;
		
	}
	// ========================================================================
} ;



/// <summary>
/// An "unmanaged" (blittable <see langword="struct"/>)
/// implementation of a shader semantic.
/// </summary>
public partial struct SemanticUnmanaged: ISemantic,
										 IFormattable,
										 IEquatable< ISemantic >, 
										 IEquatable< SemanticUnmanaged > {
	public bool HasIndex => Index is > -1 && Semantics.Names.HasSemanticIndex( Name ) ;
	public string Name => Semantics.Names.NamesBySemanticLookup[ Value ] ;
	ShaderSemantic _value ; int _index ;
	public ShaderSemantic Value => _value ;
	public int Index => _index ;
	
	
	public SemanticUnmanaged( ShaderSemantic value = default, int? index = null ) {
		this._value = value ;
		this._index = index ?? -1 ;
	}
	public SemanticUnmanaged( string? name = null, int? index = null ) {
#if DEBUG || DEV_BUILD
		if( !Semantics.Names.IsValidSemantic(name) ) throw new ArgumentException( ) ;
#endif
		
 		this._value = Semantics.Names.NamesBySemanticLookup
								 .First( kvp => (kvp.Value == name) )
									.Key ;
		
		this._index = index ?? -1 ;
	}
	
	

	#region System Overrides
	public override bool Equals( object? obj ) => 
			obj is ISemantic other && Equals( other ) ;
	
	public bool Equals( SemanticUnmanaged other ) =>
								( _value == other._value )
									&& (_index == other._index)
										|| (!HasIndex && !other.HasIndex) ;

	public bool Equals( ISemantic? other ) {
		if( other is null ) return false ;
		return ( _value == other.Value )
				   && ( _index == other.Index )
						|| ( !HasIndex && !other.HasIndex ) ;
	}
	
	
	public override int GetHashCode( ) => HashCode.Combine( Name, Value, HasIndex, Index ) ;
	public override string ToString( ) => ToString( "G", CultureInfo.CurrentCulture ) ;
	public string ToString( string? format, IFormatProvider? formatProvider ) {
		if( format is null ) format = "G" ;
		if( formatProvider is null ) formatProvider = CultureInfo.CurrentCulture ;
		
		string _formattedStr =
			HasIndex ? string.Format( formatProvider, Name, Index.ToString(format) ) 
						: Name ;
		
		return _formattedStr ;
	}
	#endregion
	
	#region Operator Overloads
	public static implicit operator SemanticUnmanaged( ShaderSemantic value )    => new( value ) ;
	public static implicit operator SemanticUnmanaged( string         name )     => new( name ) ;
	public static implicit operator ShaderSemantic( SemanticUnmanaged semantic ) => semantic.Value ;
	
	public static implicit operator string( SemanticUnmanaged semantic ) => semantic.ToString( ) ;
	public static implicit operator PCSTR( SemanticUnmanaged   semantic ) => semantic.ToString( ) ;
	public static implicit operator PCWSTR( SemanticUnmanaged  semantic ) => semantic.ToString( ) ;
	
	public static bool operator ==( SemanticUnmanaged left, ISemantic         right ) => left.Equals( right ) ;
	public static bool operator !=( SemanticUnmanaged left, ISemantic         right ) => !left.Equals( right ) ;
	public static bool operator ==( ISemantic         left, SemanticUnmanaged right ) => left.Equals( right ) ;
	public static bool operator !=( ISemantic         left, SemanticUnmanaged right ) => !left.Equals( right ) ;
	#endregion
} ;