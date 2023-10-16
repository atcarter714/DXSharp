#region Using Directives

using System.Collections.Concurrent ;
using System.Collections.ObjectModel ;
using System.Diagnostics.CodeAnalysis ;
using Windows.Win32 ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Dxgi ;
using System.Runtime.InteropServices ;
using System.Runtime.CompilerServices ;

using static DXSharp.DXSharpUtils ;
#endregion
namespace DXSharp.DXGI ;

//internal static class WrapperManager { } ;


//! May be overkill, but I got annoyed with generics and constraints and wanted to see how this might look ...
//! Now that I'm looking at it, I think it's probably a worse option that just using a Type parameter and if/else, LOL ...

/*internal enum FactoryVersion: uint {
	Factory  = 0, Factory1 = 1, Factory2 = 2, Factory3 = 3, 
	Factory4 = 4, Factory5 = 5, Factory6 = 6, Factory7 = 7,
} ;
internal delegate DXGIType DXGIObjectCreation< out DXGIType >( ) where DXGIType: IDXGIObject ;
internal delegate DXGIFactory DXGIFactoryCreation< out DXGIFactory >( ) where DXGIFactory: IDXGIFactory ;



internal class FactoryBuilder {
	// Read-Only Lookups :: ------------------------------------
	internal static readonly IReadOnlyDictionary< FactoryVersion, Type > DXGITypesByVersion = 
		new Dictionary< FactoryVersion, Type >( ) {
		{ FactoryVersion.Factory,  typeof( IDXGIFactory ) },
		{ FactoryVersion.Factory1, typeof( IDXGIFactory1 ) },
		{ FactoryVersion.Factory2, typeof( IDXGIFactory2 ) },
		{ FactoryVersion.Factory3, typeof( IDXGIFactory3 ) },
		{ FactoryVersion.Factory4, typeof( IDXGIFactory4 ) },
		{ FactoryVersion.Factory5, typeof( IDXGIFactory5 ) },
		{ FactoryVersion.Factory6, typeof( IDXGIFactory6 ) },
		{ FactoryVersion.Factory7, typeof( IDXGIFactory7 ) },
	} ;
	internal static readonly IReadOnlyDictionary< Type, Type > DXGITypesByWrapperType = 
		new Dictionary< Type, Type >( ) {
		{ typeof( Factory ),  typeof( IDXGIFactory ) },
		{ typeof( Factory1 ), typeof( IDXGIFactory1 ) },
		//{ typeof( Factory2 ), typeof( IDXGIFactory2 ) },
		//{ typeof( Factory3 ), typeof( IDXGIFactory3 ) },
		//{ typeof( Factory4 ), typeof( IDXGIFactory4 ) },
		//{ typeof( Factory5 ), typeof( IDXGIFactory5 ) },
		//{ typeof( Factory6 ), typeof( IDXGIFactory6 ) },
		//{ typeof( Factory7 ), typeof( IDXGIFactory7 ) },
	} ;
	internal static readonly IReadOnlyDictionary<Type, Type> WrapperTypeByDXGIType = 
		new Dictionary< Type, Type >( ) {
		{ typeof( IDXGIFactory ),  typeof( Factory ) },
		{ typeof( IDXGIFactory1 ), typeof( Factory1 ) },
		//{ typeof( IDXGIFactory2 ), typeof( Factory2 ) },
		//{ typeof( IDXGIFactory3 ), typeof( Factory3 ) },
		//{ typeof( IDXGIFactory4 ), typeof( Factory4 ) },
		//{ typeof( IDXGIFactory5 ), typeof( Factory5 ) },
		//{ typeof( IDXGIFactory6 ), typeof( Factory6 ) },
		//{ typeof( IDXGIFactory7 ), typeof( Factory7 ) },
	} ;
	// -------------------------------------------------------
	
	internal bool EnableExceptions { get ; set ; }
	internal FactoryVersion Version { get ; set ; }
	internal FactoryCreateFlags Flags { get ; set ; }
	
	internal FactoryBuilder( FactoryVersion version = FactoryVersion.Factory, 
							 FactoryCreateFlags flags = FactoryCreateFlags.None, 
							 bool enableExceptions = false ) {
		EnableExceptions = enableExceptions ;
		Version        = version ;
		Flags          = flags ;
	}
}

internal class FactoryBuilder< T >: FactoryBuilder where T: IDXGIFactory {
	internal DXGIFactoryCreation< T > CreateFunction { get ; private set ; }
	
	internal FactoryBuilder( DXGIFactoryCreation< T > fCreate,
							 FactoryVersion version = FactoryVersion.Factory,
							 FactoryCreateFlags flags = FactoryCreateFlags.None,
							 bool enableExceptions = false ): base( version, flags, enableExceptions ) =>
																					CreateFunction = fCreate ;
	
	internal FactoryBuilder< T > WithFlags( FactoryCreateFlags flags ) {
		//CreateFunction = ( ) => DXGIFunctions.CreateDXGIFactory2< IDXGIFactory2 >( flags ) ;
		return this ;
	}
	
	Guid _selectGuid( FactoryVersion v ) => _selectType( v ).GUID ;
	Type _selectType( FactoryVersion v ) => DXGITypesByVersion[ v ] ;
} ;*/