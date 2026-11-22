using System ;
using System.IO ;
using System.Linq ;
using System.Collections ;
using System.Collections.Generic ;
using System.Collections.ObjectModel ;
using Microsoft.Build.Framework ;
using Microsoft.Build.Utilities ;
using MSBuildTask = Microsoft.Build.Utilities.Task ;

namespace DXSharp.Tools.Builds.Shaders {

/* NOTES:
 * The command-line arguments for dxc.exe take the form of:
 *	dxc.exe [options] <inputs>
 */

public class DxcTool: MSBuildTask {
	public static class ErrorCodes {
		public const int Success = 0, InvalidArguments = -1, CompilationFailed = -2, InternalError = -3, 
							InvalidBytecode = -4, ValidationFailed = -5, InvalidOperation = -6, 
							OutOfMemory = -7, InvalidState = 8, InvalidFile = -9 ;
	}
	

	public static class Info {
		public const string TOOLNAME = "dxc.exe" ;
		public const string OVERVIEW = "HLSL Compiler" ;
		static readonly string[ ] DLLs  = { "dxcompiler.dll", //! As of 11/23/23: 1.5 - 10.0.19041.1
											"dxil.dll" } ;    //! As of 11/23/23: 1.5 - 10.0.19041.1
		public static string[ ] DllNames => DLLs.ToArray( ) ;
		public const string USAGE    = "dxc.exe [options] <inputs>" ;
		static readonly string[ ] _profileStrings = {
			"ps_6_0", "ps_6_1", "ps_6_2", "ps_6_3", "ps_6_4", "ps_6_5", 
			"vs_6_0", "vs_6_1", "vs_6_2", "vs_6_3", "vs_6_4", "vs_6_5", 
			"cs_6_0", "cs_6_1", "cs_6_2", "cs_6_3", "cs_6_4", "cs_6_5", 
			"gs_6_0", "gs_6_1", "gs_6_2", "gs_6_3", "gs_6_4", "gs_6_5", 
			"ds_6_0", "ds_6_1", "ds_6_2", "ds_6_3", "ds_6_4", "ds_6_5",
			"hs_6_0", "hs_6_1", "hs_6_2", "hs_6_3", "hs_6_4", "hs_6_5", 
			"lib_6_3", "lib_6_4", "lib_6_5", "ms_6_5", "as_6_5",
		} ;
		public static readonly ReadOnlyDictionary< string, string > CommonOptions = 
			new( new Dictionary< string, string >( ) {
			{ "-help",              "Display available options" },
			{ "-nologo",            "Suppress copyright message" },
			{ "-Qunused-arguments", "Don't emit warning for unused driver arguments" },
		} );
		public static readonly ReadOnlyDictionary< string, string > OptimizationOptions = 
			new( new Dictionary< string, string >( ) {
			{ "-O0", "Optimization Level 0" },
			{ "-O1", "Optimization Level 1" },
			{ "-O2", "Optimization Level 2" },
			{ "-O3", "Optimization Level 3 (Default)" },
		} );
		public static readonly ReadOnlyDictionary< string, string > UtilityOptions = 
			new( new Dictionary< string, string >( ) {
			{ "-dumpbin",                    "Load a binary file rather than compiling" },
			{ "-extractrootsignature",       "Extract root signature from shader bytecode (must be used with /Fo <file>)" },
			{ "-getprivate <file>",          "Save private data from shader blob" },
			{ "-P <value>",                  "Preprocess to file (must be used alone)" },
			{ "-Qembed_debug",               "Embed PDB in shader container (must be used with /Zi)" },
			{ "-Qstrip_debug",               "Strip debug information from 4_0+ shader bytecode  (must be used with /Fo <file>)" },
			{ "-Qstrip_priv",                "Strip private data from shader bytecode  (must be used with /Fo <file>)" },
			{ "-Qstrip_reflect",             "Strip reflection data from shader bytecode  (must be used with /Fo <file>)" },
			{ "-Qstrip_rootsignature",       "Strip root signature data from shader bytecode  (must be used with /Fo <file>)" },
			{ "-setprivate <file>",          "Private data to add to compiled shader blob" },
			{ "-setrootsignature <file>",    "Attach root signature to shader bytecode" },
			{ "-verifyrootsignature <file>", "Verify shader bytecode with root signature" },
		} );
		public static readonly ReadOnlyDictionary< string, string > SPIRVCodeGenerationOptions = new( 
		 new Dictionary< string, string >( ) {
			{ "-fspv-debug=<value>",           "Specify whitelist of debug info category (file -> source -> line, tool)" },
			{ "-fspv-extension=<value>",       "Specify SPIR-V extension permitted to use" },
			{ "-fspv-flatten-resource-arrays", "Flatten arrays of resources so each array element takes one binding number" },
			{ "-fspv-reflect",                 "Emit additional SPIR-V instructions to aid reflection" },
			{ "-fspv-target-env=<value>",      "Specify the target environment: vulkan1.0 (default) or vulkan1.1" },
			{ "-fvk-b-shift <shift> <space>",  "Specify Vulkan binding number shift for b-type register" },
			{ "-fvk-bind-globals <binding> <set>", "Specify Vulkan binding number and set number for the $Globals cbuffer" },
			{ "-fvk-bind-register <type-number> <space> <binding> <set>", "Specify Vulkan descriptor set and binding for a specific register" },
			{ "-fvk-invert-y",                 "Negate SV_Position.y before writing to stage output in VS/DS/GS to accommodate Vulkan's coordinate system" },
			{ "-fvk-s-shift <shift> <space>",  "Specify Vulkan binding number shift for s-type register" },
			{ "-fvk-t-shift <shift> <space>",  "Specify Vulkan binding number shift for t-type register" },
			{ "-fvk-u-shift <shift> <space>",  "Specify Vulkan binding number shift for u-type register" },
			{ "-fvk-use-dx-layout",            "Use DirectX memory layout for Vulkan resources" },
			{ "-fvk-use-dx-position-w",        "Reciprocate SV_Position.w after reading from stage input in PS to accommodate the difference between Vulkan and DirectX" },
			{ "-fvk-use-gl-layout",            "Use strict OpenGL std140/std430 memory layout for Vulkan resources" },
			{ "-fvk-use-scalar-layout",        "Use scalar memory layout for Vulkan resources" },
			{ "-Oconfig=<value>",              "Specify a comma-separated list of SPIRV-Tools passes to customize optimization configuration (see http://khr.io/hlsl2spirv#optimization)" },
			{ "-spirv",                        "Generate SPIR-V code" },
		} ) ;
		public static readonly ReadOnlyDictionary< string, string > CompilationOptions = new ( 
		 new Dictionary< string, string >( ) {
			{ "-all_resources_bound",        "Enables agressive flattening" },
			{ "-auto-binding-space <value>", "Set auto binding space - enables auto resource binding in libraries" },
			{ "-Cc",                         "Output color coded assembly listings" },
			{ "-default-linkage <value>",    "Set default linkage for non-shader functions when compiling or linking to a library target (internal, external)" },
			{ "-denorm <value>",             "Select denormal value options (any, preserve, ftz). any is the default." },
			// Previous items...
			{ "-D <value>", "Define macro" },
			{ "-enable-16bit-types",  "Enable 16bit types and disable min precision types. Available in HLSL 2018 and shader model 6.2" },  
			{ "-export-shaders-only", "Only export shaders when compiling a library" },
			{ "-exports <value>",     "Specify exports when compiling a library: export1[[,export1_clone,...]=internal_name][;...]" },
			{ "-E <value>",           "Entry point name" },
			{ "-Fc <file>",           "Output assembly code listing file" },
			{ "-Fd <file>",           "Write debug information to the given file, or automatically named file in directory when ending in '\\'" },
			// Previous items...
		    { "-Fe <file>",                      "Output warnings and errors to the given file" },
		    { "-Fh <file>",                      "Output header file containing object code" },
		    { "-flegacy-macro-expansion",        "Expand the operands before performing token-pasting operation (fxc behavior)" },
		    { "-flegacy-resource-reservation",   "Reserve unused explicit register assignments for compatibility with shader model 5.0 and below" },
		    { "-force_rootsig_ver <profile>",    "force root signature version (rootsig_1_1 if omitted)" },
		    { "-Fo <file>", "Output object file" },
		    { "-Gec",       "Enable backward compatibility mode" },
		    { "-Ges",       "Enable strict mode" },
		    { "-Gfa",       "Avoid flow control constructs" },
		    { "-Gfp",       "Prefer flow control constructs" },
		    { "-Gis",       "Force IEEE strictness" },
			{ "-HV <value>", "HLSL version (2016, 2017, 2018). Default is 2018" },
		    { "-H",         "Show header includes and nesting depth" },
		    { "-ignore-line-directives", "Ignore line directives" },
		    { "-I <value>",                "Add directory to include search path" },
		    { "-Lx",                       "Output hexadecimal literals" },
		    { "-Ni",                       "Output instruction numbers in assembly listings" },
		    { "-no-warnings",              "Suppress warnings" },
		    { "-not_use_legacy_cbuf_load", "Do not use legacy cbuffer load" },
		    { "-No",                       "Output instruction byte offsets in assembly listings" },
			{ "-Odump",                    "Print the optimizer commands." },
			{ "-Od",                       "Disable optimizations" },
		    { "-pack_optimized",           "Optimize signature packing assuming identical signature provided for each connecting stage" },
			{ "-pack_prefix_stable",       "(default) Pack signatures preserving prefix-stable property - appended elements will not disturb placement of prior elements" },
			{ "-recompile",                "Recompile from DXIL container with Debug Info or Debug Info bitcode file" },
			{ "-res_may_alias",            "Assume that UAVs/SRVs may alias (Allows resources to alias the same register space)" },
		    { "-rootsig-define <value>",   "Read root signature from a #define" },
		    { "-T <profile>",              "Set target profile." },
		    { "<profile>",  @"""ps_6_0, ps_6_1, ps_6_2, ps_6_3, ps_6_4, ps_6_5, 
							vs_6_0, vs_6_1, vs_6_2, vs_6_3, vs_6_4, vs_6_5, 
							cs_6_0, cs_6_1, cs_6_2, cs_6_3, cs_6_4, cs_6_5, 
							gs_6_0, gs_6_1, gs_6_2, gs_6_3, gs_6_4, gs_6_5, 
							ds_6_0, ds_6_1, ds_6_2, ds_6_3, ds_6_4, ds_6_5, 
							hs_6_0, hs_6_1, hs_6_2, hs_6_3, hs_6_4, hs_6_5, 
							lib_6_3, lib_6_4, lib_6_5, ms_6_5, as_6_5
""" },
			{ "-Vd", "Disable validation" },
		    { "-Vi", "Display details about the include process" },
		    { "-Vn <name>", "Use <name> as variable name in header file" },
		    { "-WX", "Warnings are errors" },
		    { "-Zi", "Enable debug information" },
		    { "-Zpc", "Pack matrices in column-major order" },
		    { "-Zpr", "Pack matrices in row-major order" },
			{ "-Zsb", "Build debug name considering only output binary" },
			{ "-Zss", "Build debug name considering source information" },
		} );
		
		public static int LogLevel { get; set; } = 3 ;
		public static int LogLevels( string str ) => str.ToLowerInvariant( ) switch {
			"none" => 0, "min" => 1, "low" => 2, "normal" or "default" => 3, "high" => 4, "debug" => 5,  _ => 3,
		} ;
	}
	const string _LOGPREFIX = nameof(DxcTool) + " ::" ;
	const string DEFAULT_OUT_DIR      = ".\fxbin", DEFAULT_OUT_FILE = "shader_bytecode", 
				 DEFAULT_SHADER_MODEL = "vs_6_0", DEFAULT_ENTRY_POINT = "main" ;
	public const string DxSharpShaderType = "dxsharp", VertexShaderType = "vertex", PixelShaderType = "pixel", 
						GeometryShaderType = "geometry", HullShaderType = "hull", DomainShaderType = "domain", 
						ComputeShaderType = "compute" ;
	static readonly string[ ] _shaderTypes = { DxSharpShaderType, VertexShaderType, PixelShaderType, 
												GeometryShaderType, HullShaderType, DomainShaderType, 
												ComputeShaderType, } ;
	
	
	//! List of hlsl files to compile:
	[Required] public ITaskItem[ ] ShaderFiles { get; set; }
	
	public string EntryPoint { get; set; }
	[Required] public string ShaderType { get; set; }
	public string ShaderModel { get; set; }
	public string AssemblerOutDir { get; set; }
	public string AssemblerOutputFile { get; set; }
	public bool EnableDebuggingInfo { get; set; }
	public bool DisableOptimizations { get; set; }
	public string[ ] PreprocessorDefinitions { get; set; }
	public string[ ] AdditionalIncludeDirectories { get; set; }

	
	// Helper methods
	bool isValidShaderType( string str ) => 
		_shaderTypes.Contains( ShaderType.ToLowerInvariant( ) ) ;
	

	
	public override bool Execute( ) {
		foreach ( var item in ShaderFiles ) {
			string shaderType = item.GetMetadata( "ShaderType" ) ;
			return shaderType switch {
					   "vertex"  => ( CompileVertexShader( item ) is 0 ),
					   "pixel"   => ( CompilePixelShader( item ) is 0 ),
					   "dxsharp" => throw new Exception( "DXSharp shader type is not supported yet!" ),
					   _         => throw new Exception( $"Unknown shader type: {ShaderType}" ),
				   } ;
		}
		return false ;
	}
	
	public int CompileVertexShader( ITaskItem item ) {
		if ( item is null ) {
			Log.LogError( $"{_LOGPREFIX} :: TaskItem is null/empty!", -1 ) ;
			return -1 ;
		}
		var itemSpec        = item.ItemSpec ?? throw new ArgumentNullException( nameof(item) ) ;
		var shaderName      = item.GetMetadata( "ShaderName" ) ?? Path.GetFileNameWithoutExtension( itemSpec ) ;
		var entryPoint      = item.GetMetadata( "EntryPoint" ) ?? DEFAULT_ENTRY_POINT ;
		var shaderModel     = item.GetMetadata( "ShaderModel" ) ?? DEFAULT_SHADER_MODEL ;
		var assemblerOutDir = item.GetMetadata( "AssemblerOutDir" ) ?? DEFAULT_OUT_DIR ;
		
		var assemblerOutputFile = item.GetMetadata( "AssemblerOutputFile" ) ?? DEFAULT_OUT_FILE ;
		
		bool enableDebugParsed = bool.TryParse( item.GetMetadata( "EnableDebuggingInfo" ) 
												 ?? "false", out var _enableDebuggingInfo ) ;
		bool disableOptimizationsParsed = bool.TryParse( item.GetMetadata( "DisableOptimizations" ) 
														 ?? "false", out var _disableOptimizations ) ;
		
		Log.LogMessage( MessageImportance.High, $"Compiling vertex shader: {item.GetMetadata( "EntryPoint" )}" ) ;
		
		return 0 ;
	}
	
	public int CompilePixelShader( ITaskItem item ) {
		return 0 ;
	}
} ;
}