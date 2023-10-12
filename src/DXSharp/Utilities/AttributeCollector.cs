#region Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices ;
#endregion
namespace DXSharp.Utilities ;


public static class AttributeCollector {
	static readonly Type comImport = typeof( ComImportAttribute ) ;
	static readonly Type codeGeneration = typeof( System.CodeDom.Compiler.GeneratedCodeAttribute ) ;
	
	// Dictionary to store results of each attribute base type queried
	static readonly Dictionary< Type, HashSet<Type> > _attributeLookup = new( ) ;
	public static HashSet< Type > NativeComTypes { get; }
	public static HashSet< Type > GeneratedTypes { get; }

	
	static AttributeCollector( ) {
		// Initialize the special collections
		var typesInAssembly = Assembly.GetExecutingAssembly( )
									  .GetTypes( ) ;

		NativeComTypes = new ( typesInAssembly.Where( t => t
														   .GetCustomAttributes( comImport,
																					false ).Any( ) ) ) ;

		GeneratedTypes = new(
											 typesInAssembly.Where( t => t
																		 .GetCustomAttributes( codeGeneration,
																			 false ).Any( ) ) ) ;
	}
	
	static HashSet< Type > _tempBuilder = new( ) ;
	public static HashSet< Type > GetAttributesDerivedFrom< T >( ) where T: Attribute {
		if ( _attributeLookup
				.TryGetValue( typeof(T), out var cachedResult) )
			return cachedResult ;
		
		foreach ( var type in Assembly
									.GetExecutingAssembly( )
										.GetTypes( ) ) {
			if ( type.IsSubclassOf( typeof(T) ) && !type.IsAbstract ) {
				_tempBuilder.Add( type ) ;
			}
		}

		// Cache the result and return
		_attributeLookup[ typeof(T) ] = _tempBuilder.ToHashSet( ) ;
		_tempBuilder.Clear( ) ;
		
		return _attributeLookup[ typeof(T) ] ;
	}
}
