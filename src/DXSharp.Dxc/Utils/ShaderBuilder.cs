#pragma warning disable CS1998, CS8500

/* NOTE: This is a work in progress ...
 * Since I am new to "Dxc" and Shader Model 6.X+ shaders, I am still learning how to use the API ...
 * The best thing I could do is begin creating a more abstract, higher-level "ShaderBuilder" class,
 * as it forces me to make API calls, create object instances and start figuring out what needs to
 * be done to build a shader and use Dxc in general ...
 *
 * For now, this makes a demonstration of how to use the raw interop bindings to Dxc to compile HLSL ...
 */


#region Using Directives
using System.Diagnostics.Contracts ;
using System.Text ;
using System.Runtime.CompilerServices ;

using Windows.Win32 ;
using Windows.Win32.Foundation ;
using Windows.Win32.System.Com ;
using Windows.Win32.Graphics.Direct3D.Dxc ;

using DXSharp ;
using DXSharp.Windows ;
#endregion
namespace DXSharp.Dxc ;


public class ShaderBuilder: DisposableObject {
	IMalloc 		   pMalloc ;
	IDxcUtils          pUtils ;
	IDxcCompiler3      pCompiler ;
	IDxcIncludeHandler pIncludeHandler ;
	
	public IMalloc Malloc => pMalloc ;
	public IDxcUtils Utils => pUtils ;
	public IDxcCompiler3 Compiler => pCompiler ;
	public IDxcIncludeHandler IncludeHandler => pIncludeHandler ;
	
	// --------------------------------------------------------------------------------------------------
	
	public ShaderBuilder( ) {
		HResult hr =
		DxcPInvoke.CoGetMalloc( 1, out var _ppvMalloc ) ;
		if( _ppvMalloc is null ) throw new DxcComError( hr, "CoGetMalloc returned null" ) ;
		hr.ThrowOnFailure( ) ;
		this.pMalloc = _ppvMalloc ;

		hr =
		Dxc.CreateInstance2( _ppvMalloc,
							 Dxc.CLSIDs.Compiler,
							 typeof(IDxcCompiler3).GUID,
							 out var _ppvCompiler ) ;
		if( _ppvCompiler is null ) throw new DxcComError( hr, "CoGetMalloc returned null" ) ;
		hr.ThrowOnFailure( ) ;
		this.pCompiler = (IDxcCompiler3?)_ppvCompiler 
						 ?? throw new DxcComError( hr, $"Cannot create {nameof(IDxcCompiler3)} instance!" ) ;
		
		hr =
		Dxc.CreateInstance2( _ppvMalloc,
							 Dxc.CLSIDs.DxcUtils,
							 typeof(IDxcUtils).GUID,
							 out var _ppvUtils ) ;
		if( _ppvUtils is null ) throw new DxcComError( hr, "CoGetMalloc returned null" ) ;
		hr.ThrowOnFailure( ) ;
		this.pUtils = (IDxcUtils?)_ppvUtils
					  ?? throw new DxcComError( hr, $"Cannot create {nameof(IDxcUtils)} instance!" ) ;
		
		hr = pUtils.CreateDefaultIncludeHandler( out this.pIncludeHandler ) ;
		if( this.pIncludeHandler is null ) throw new DxcComError( hr, "CoGetMalloc returned null" ) ;
		hr.ThrowOnFailure( ) ;
		
		
	}
	~ShaderBuilder( ) => Dispose( false ) ;
	
	// --------------------------------------------------------------------------------------------------
	
	PCWSTR[ ]? GetUnmanagedArgs( params string[ ]? args ) {
		if( args is not { Length: > 0 } ) return null ;
		var _args = new PCWSTR[ args.Length ] ;
		for( int i = 0; i < args.Length; ++i ) {
			string nextArg = args[ i ] ;
			if ( string.IsNullOrEmpty( nextArg ) ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
				throw new ArgumentException( $"{nameof(ShaderBuilder)} :: " +
											 $"Cannot contain null/empty args!", nameof(args) ) ;
#else
				continue ;
#endif
			}
			_args[ i ] = nextArg ;
		}
		return _args ;
	}
	
	// --------------------------------------------------------------------------------------------------
	
	public DxcBuffer CreateBufferFromFile( string fileName, Encoding? encoding = null ) {
		ArgumentException.ThrowIfNullOrEmpty( fileName, nameof(fileName) ) ;
		if( !File.Exists(fileName) ) throw new FileNotFoundException( fileName ) ;
		unsafe {
			DXC_CP cp = GetCPForEncoding( encoding ?? Encoding.UTF8 ) ;
			using PCWSTR pFileName = fileName ;
			
			// load the file:
			var hr = pUtils.LoadFile( pFileName, &cp, out var _blobEncoding ) ;
			if( _blobEncoding is null ) throw new DxcComError( hr, $"{nameof(pUtils.LoadFile)} returned null" ) ;
			hr.ThrowOnFailure( ) ;
			
			// create the buffer:
			var  _ptr = _blobEncoding.GetBufferPointer( ) ;
			nuint _size = _blobEncoding.GetBufferSize( ) ;
			uint _cpi = GetEncodingCPIFor(cp) ;
			var buffer = new DxcBuffer {
				Encoding = _cpi,
				Size     = _size,
				Ptr      = _ptr,
			} ;
			
			return buffer ;
		}
	}

	
	public IDxcResult Build( string fileName, Encoding? encoding = null,
										params string[ ]? compilerArgs ) {
		// Create the buffer:
		DxcBuffer buffer = CreateBufferFromFile( fileName, encoding ) ;
		
		// Build the shader:
		return Build( buffer, compilerArgs ) ;
	}
	
	public IDxcResult Build( DxcBuffer buffer, 
									params string[ ]? compilerArgs ) {
		unsafe {
			DxcBuffer* pSrc = &buffer ;
			var _guid       = typeof( IDxcIncludeHandler ).GUID ;
			uint argCount   = ( compilerArgs is not null ? (uint)compilerArgs.Length : 0U ) ;
			PCWSTR[ ]? args = GetUnmanagedArgs( compilerArgs )
								?? throw new ArgumentNullException( nameof(compilerArgs) ) ;
			
			fixed( PCWSTR* _args = &args![ 0 ] ) {
				HResult hr = pCompiler.Compile( pSrc, _args, argCount, null,
								   &_guid, out var _ppvResult ) ;
				if( _ppvResult is null ) throw new DxcComError( hr, "CoGetMalloc returned null" ) ;
				hr.ThrowOnFailure( ) ;
				
				// Create the results interface:
				IDxcResult? pResult = (IDxcResult?)_ppvResult
										?? throw new DxcErrorException( ) ;
				
				var iid = typeof( IDxcBlobUtf8 ).GUID ;
				hr = pResult.GetOutput( DXC_OUT_KIND.DXC_OUT_ERRORS, &iid,
										out var bb,
										out var pErrors ) ;
				
				// Note that d3dcompiler would return null if no errors or warnings are present.
				// IDxcCompiler3::Compile will always return an error buffer, but its length
				// will be zero if there are no warnings or errors.
				string errorMessageString = string.Empty ;
				if ( pErrors is not null ) {
					ulong stringLength = pErrors.GetStringLength( ) ;
					if( stringLength > 0 ) {
						var errorMsg = pErrors.GetStringPointer( ) ;
						errorMessageString = errorMsg.ToString( ) ;
					}
				}
				
				pResult.GetStatus( (HRESULT *)&hr ) ;
				if( hr.Failed ) throw new DxcErrorException( $"{nameof(ShaderBuilder)} :: Shader compilation has failed!\n" +
															 $"Error Messages: {( !string.IsNullOrEmpty( errorMessageString ) 
																					? errorMessageString : "N/A" )}" ) ;
				
				//  --------------------------------------------------------------------------------------------------
				//! ------- Things that can be done with compiler results: -------------------------------------------
				// Get shader binary:
				ulong binSize = GetShaderBinary( pResult, out string shaderName,
													out void* shaderBytes ) ;

				// Get shader reflection:
				ulong reflectSize = GetShaderReflection( pResult, out string reflectionName, 
														 out IDxcBlob shaderReflection ) ;

				// Get shader PDB:
				ulong pbdSize = GetShaderPBD( pResult, out string pdbName, out IDxcBlob shaderPDB ) ;

				// Get shader disassembly:
				ulong disasmSize = GetShaderDisassembly( pResult, out IDxcBlobUtf16? pDisassemblyName, 
																	out IDxcBlob shaderDisassembly ) ;

				// Get shader hash:
				ulong hashSize = GetShaderHash( pResult, out string hashName, out IDxcBlob shaderHash ) ;

				// Get shader root signature:
				ulong rootSize = GetShaderRootSignature( pResult, out string rootSigName, 
										out IDxcBlob shaderRootSignature ) ;
				// --------------------------------------------------------------------------------------------------
				
				return pResult ;
			}
		}
	}
	
	// --------------------------------------------------------------------------------------------------
	
	[MethodImpl( MethodImplOptions.AggressiveInlining ), Pure]
	public static uint GetEncodingCPIFor( DXC_CP cp ) => (uint)cp ;
	
	[MethodImpl( MethodImplOptions.AggressiveInlining ), Pure]
	public static DXC_CP GetCPForEncoding( Encoding encoding ) {
		uint e = (uint)encoding.CodePage ;
		return e switch {
				   65001 => DXC_CP.DXC_CP_UTF8,
				   1200  => DXC_CP.DXC_CP_UTF16,
				   0     => DXC_CP.DXC_CP_ACP,
				   _     => throw new ArgumentOutOfRangeException( nameof( encoding ) ),
			   } ;
	}
	
	[MethodImpl( MethodImplOptions.AggressiveInlining ), Pure]
	public static Encoding GetEncodingForCP( DXC_CP cp ) => cp switch {
																	   DXC_CP.DXC_CP_UTF8  => Encoding.UTF8,
																	   DXC_CP.DXC_CP_UTF16 => Encoding.Unicode,
																	   DXC_CP.DXC_CP_ACP   => Encoding.Default,
																	   _                   => throw new ArgumentOutOfRangeException( nameof(cp) ),
																   } ;

	
	
	internal static unsafe ulong GetShaderBinary( IDxcResult pResult, 
												  out string shaderName, 
												  out void* shaderBytes ) {
		HResult hr ; Guid iid = typeof( IDxcBlob ).GUID ;
		hr = pResult.GetOutput( DXC_OUT_KIND.DXC_OUT_OBJECT, &iid,
								out var _shaderObj,
								out var pShaderName ) ;
		
		if( _shaderObj is null ) throw new DxcComError( hr, $"{nameof(ShaderBuilder)} :: " +
															$"{nameof(pResult.GetOutput)} returned `nullptr` for shader!" ) ;
		hr.ThrowOnFailure( ) ;

		var _shaderName = pShaderName?.GetStringPointer( ) ;
		shaderName = _shaderName?.ToString( ) 
					 ?? throw new DxcErrorException( $"{nameof(ShaderBuilder)} :: " +
													 $"{nameof(pShaderName)} returned `nullptr`!" ) ;

		var shaderObj = (IDxcBlob?)_shaderObj ??
						throw new DxcErrorException( $"{nameof(ShaderBuilder)} :: " +
													 $"{nameof(pResult.GetOutput)} returned `nullptr` for shader!" ) ;
		shaderBytes = shaderObj.GetBufferPointer( ) ;
		ulong shaderSize = shaderObj.GetBufferSize( ) ;
		
		return shaderSize ;
	}
	
	internal static unsafe ulong GetShaderReflection( IDxcResult pResult, 
													  out string reflectionName, 
													  out IDxcBlob shaderReflection ) {
		HResult hr ; Guid iid = typeof( IDxcBlob ).GUID ;
		hr = pResult.GetOutput( DXC_OUT_KIND.DXC_OUT_REFLECTION, &iid,
								out var _shaderReflection,
								out var pReflectionName ) ;
				
		if( _shaderReflection is null ) 
			throw new DxcComError( hr, $"{nameof(ShaderBuilder)} :: " +
									   $"{nameof(pResult.GetOutput)} returned `nullptr` for shader reflection!" ) ;
		hr.ThrowOnFailure( ) ;
				
		var _reflectionName = pReflectionName?.GetStringPointer( ) ;
		reflectionName = _reflectionName?.ToString( ) 
						 ?? throw new DxcErrorException( $"{nameof(ShaderBuilder)} :: " +
														 $"{nameof(pReflectionName)} returned `nullptr`!" ) ;
				
		shaderReflection = (IDxcBlob?)_shaderReflection ??
						   throw new DxcErrorException( $"{nameof(ShaderBuilder)} :: " +
														$"{nameof(pResult.GetOutput)} returned `nullptr` for shader reflection!" ) ;
		var   reflectionBytes = shaderReflection.GetBufferPointer( ) ;
		ulong reflectionSize  = shaderReflection.GetBufferSize( ) ;
		
		return reflectionSize ;
	}

	internal static unsafe ulong GetShaderPBD( IDxcResult pResult, out string pdbName, 
											   out IDxcBlob shaderPDB ) {
		HResult hr ; Guid iid = typeof( IDxcBlob ).GUID ;
		hr = pResult.GetOutput( DXC_OUT_KIND.DXC_OUT_PDB, &iid,
								out var _shaderPDB,
								out var pPDBName ) ;
				
		if( _shaderPDB is null )
			throw new DxcComError( hr, $"{nameof(ShaderBuilder)} :: " +
									   $"{nameof(pResult.GetOutput)} returned `nullptr` for shader PDB!" ) ;
		hr.ThrowOnFailure( ) ;
				
		var _pdbName = pPDBName?.GetStringPointer( ) ;
		pdbName = _pdbName?.ToString( ) 
				  ?? throw new DxcErrorException( $"{nameof(ShaderBuilder)} :: " +
												  $"{nameof(pPDBName)} returned `nullptr`!" ) ;
				
		shaderPDB = (IDxcBlob?)_shaderPDB ??
					throw new DxcErrorException( $"{nameof(ShaderBuilder)} :: " +
												 $"{nameof(pResult.GetOutput)} returned `nullptr` for shader PDB!" ) ;
				
		var   pdbBytes = shaderPDB.GetBufferPointer( ) ;
		ulong pdbSize  = shaderPDB.GetBufferSize( ) ;
		
		return pdbSize ;
	}
	
	internal static unsafe ulong GetShaderDisassembly( IDxcResult pResult, 
													   out IDxcBlobUtf16? pDisassemblyName,
													   out IDxcBlob shaderDisassembly ) {
		HResult hr ; Guid iid = typeof( IDxcBlob ).GUID ; 
		hr = pResult.GetOutput( DXC_OUT_KIND.DXC_OUT_DISASSEMBLY, &iid,
								out var _shaderDisassembly,
								out pDisassemblyName ) ;
				
		if( _shaderDisassembly is null )
			throw new DxcComError( hr, $"{nameof(ShaderBuilder)} :: " +
									   $"{nameof(pResult.GetOutput)} returned `nullptr` for shader disassembly!" ) ;
		hr.ThrowOnFailure( ) ;
				
		var _disassemblyName = pDisassemblyName?.GetStringPointer( ) ;
		string disassemblyName = _disassemblyName?.ToString( ) 
								 ?? throw new DxcErrorException( $"{nameof(ShaderBuilder)} :: " +
																 $"{nameof(pDisassemblyName)} returned `nullptr`!" ) ;
				
		shaderDisassembly = (IDxcBlob?)_shaderDisassembly ??
							throw new DxcErrorException( $"{nameof(ShaderBuilder)} :: " +
														 $"{nameof(pResult.GetOutput)} returned `nullptr` for shader disassembly!" ) ;
				
		var   disassemblyBytes = shaderDisassembly.GetBufferPointer( ) ;
		ulong disassemblySize  = shaderDisassembly.GetBufferSize( ) ;
		
		return disassemblySize ;
	}
	
	internal static unsafe ulong GetShaderHash( IDxcResult pResult,
												out string hashName,
												out IDxcBlob shaderHash ) {
		HResult hr ;
		Guid iid = typeof( IDxcBlob ).GUID ;
		hr = pResult.GetOutput( DXC_OUT_KIND.DXC_OUT_SHADER_HASH, &iid,
								out var _shaderHash,
								out var pHashName ) ;
		
		if( _shaderHash is null )
			throw new DxcComError( hr, $"{nameof(ShaderBuilder)} :: " +
									   $"{nameof(pResult.GetOutput)} returned `nullptr` for shader hash!" ) ;
		hr.ThrowOnFailure( ) ;
				
		var _hashName = pHashName?.GetStringPointer( ) ;
		hashName = _hashName?.ToString( ) 
				   ?? throw new DxcErrorException( $"{nameof(ShaderBuilder)} :: " +
												   $"{nameof(pHashName)} returned `nullptr`!" ) ;
				
		shaderHash = (IDxcBlob?)_shaderHash ??
					 throw new DxcErrorException( $"{nameof(ShaderBuilder)} :: " +
												  $"{nameof(pResult.GetOutput)} returned `nullptr` for shader hash!" ) ;
				
		var   hashBytes = shaderHash.GetBufferPointer( ) ;
		ulong hashSize  = shaderHash.GetBufferSize( ) ;
		
		return hashSize ;
	}
	
	internal static unsafe ulong GetShaderRootSignature( IDxcResult pResult,
											   out string rootSigName,
											   out IDxcBlob shaderRootSignature ) {
		var iid = typeof( IDxcBlob ).GUID ;
		HResult hr = pResult.GetOutput( DXC_OUT_KIND.DXC_OUT_ROOT_SIGNATURE, &iid,
								out var _shaderRootSignature,
								out var pRootSigName ) ;
				
		if( _shaderRootSignature is null )
			throw new DxcComError( hr, $"{nameof(ShaderBuilder)} :: " +
									   $"{nameof(pResult.GetOutput)} returned `nullptr` for shader root signature!" ) ;
		hr.ThrowOnFailure( ) ;
		
		var _rootSigName = pRootSigName?.GetStringPointer( ) ;
		rootSigName = _rootSigName?.ToString( ) 
					  ?? throw new DxcErrorException( $"{nameof(ShaderBuilder)} :: " +
													  $"{nameof(pRootSigName)} returned `nullptr`!" ) ;
				
		shaderRootSignature = (IDxcBlob?)_shaderRootSignature ??
							  throw new DxcErrorException( $"{nameof(ShaderBuilder)} :: " +
														   $"{nameof(IDxcResult.GetOutput)} returned `nullptr` for shader root signature!" ) ;
				
		var   rootSigBytes = shaderRootSignature.GetBufferPointer( ) ;
		ulong rootSigSize  = shaderRootSignature.GetBufferSize( ) ;
		
		return rootSigSize ;
	}

	
	// --------------------------------------------------------------------------------------------------
	protected override async ValueTask DisposeUnmanaged( ) {
		unsafe {
			//! This ugliness is necessary because the CsWin32-generated code does not implement IDisposable
			//! and does not inherit from DXSharp.Win32.COM.IUknown, thus has no `Release` method ...
			//! This is something we will soon address ... pun intended, if you get my pointer ... ;-)
			IUnknownUnsafe* unsafeCompiler = null ;
			fixed( void* compilerPtr = &pCompiler ) {
				unsafeCompiler = *(IUnknownUnsafe**)compilerPtr ;
				unsafeCompiler->Release( ) ;
			}
			IUnknownUnsafe* unsafeUtils = null ;
			fixed( void* utilsPtr = &pUtils ) {
				unsafeUtils = *(IUnknownUnsafe**)utilsPtr ;
				unsafeUtils->Release( ) ;
			}
			IUnknownUnsafe* unsafeIncludeHandler = null ;
			fixed( void* includeHandlerPtr = &pIncludeHandler ) {
				unsafeIncludeHandler = *(IUnknownUnsafe**)includeHandlerPtr ;
				unsafeIncludeHandler->Release( ) ;
			}
		}
		pMalloc.Release( ) ;
	}
	// ==================================================================================================
} ;


/* -------------------------------------------------------------------------------------------------------------
 * SPECIFYING COMMAND LINE ARGUMENTS FOR THE COMPILER ::
 * (Example from Microsoft Docs)
 * -------------------------------------------------------------------------------------------------------------
	// COMMAND LINE:
	// dxc myshader.hlsl -E main -T ps_6_0 -Zi -D MYDEFINE=1 -Fo myshader.bin -Fd myshader.pdb -Qstrip_reflect
	//
   LPCWSTR pszArgs[] =
	{
		L"myshader.hlsl",            // Optional shader source file name for error reporting and PIX shader source view.
		  
		L"-E", L"main",              // Entry point.
		L"-T", L"ps_6_0",            // Target.
		L"-Zs",                      // Enable debug information (slim format)
		L"-D", L"MYDEFINE=1",        // A single define.
		L"-Fo", L"myshader.bin",     // Optional. Stored in the pdb. 
		L"-Fd", L"myshader.pdb",     // The file name of the pdb. This must either be supplied
									 // or the autogenerated file name must be used.
		L"-Qstrip_reflect",          // Strip reflection into a separate blob.
	};
	
 */