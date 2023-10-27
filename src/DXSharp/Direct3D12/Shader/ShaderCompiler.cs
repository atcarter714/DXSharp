#region Using Directives
using System.Runtime.InteropServices ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D ;
using static Windows.Win32.PInvoke ;

using DXSharp.Windows ;
#endregion


namespace DXSharp.Direct3D12.Shader ;

public static class ShaderCompiler {
	
	/// <summary>
	/// Default flags for compiling shaders.
	/// </summary>
	public const ShaderCompileFlags DefaultFlags =
#if !DEBUG || DEV_BUILD
			ShaderCompileFlags.EnableStrictness | ShaderCompileFlags.Debug
#else
			ShaderCompileFlags.OptimizationLevel3 | ShaderCompileFlags.AvoidFlowControl
#endif
		;
	
	
	public static IBlob CompileFromFile( string             fileName, 
										 ShaderMacro[ ]     defines, 
										 Include[ ]         includes, 
										 string             entryPoint,
										 ShaderProfile      profile,
										 out IBlob?         errors,
										 ShaderCompileFlags flags1 = DefaultFlags,  
										 ShaderCompileFlags flags2 = DefaultFlags ) {
		return CompileFromFile( fileName, defines, includes, entryPoint, profile.ToString( ), 
								(uint)flags1, (uint)flags2, out errors ) ;
	}
	
	public static IBlob CompileFromFile( string         fileName, 
										 ShaderMacro[ ] defines, 
										 Include[ ]     includes, 
										 string         entryPoint,
										 string         target,
										 uint           flags1,  
										 uint           flags2,
										 out IBlob?     errors ) {
		HResult hr = CompileFromFile( fileName, defines.AsSpan(), includes.AsSpan(), 
									  entryPoint, target, flags1, flags2, 
									  out var code, out errors ) ;
		if( hr.Failed ) {
			InteropUtils.SetLastErrorForThread( hr ) ;
			hr.ThrowOnFailure( ) ;
		}
		
		return code!
#if DEBUG || DEV_BUILD
			   ?? throw new DXSharpException( $"Failed to compile") 
#endif
			   ;
	}
	
	
	public static HResult CompileFromFile( PCWSTR                          pFileName,
										   [Optional] in Span< ShaderMacro > pDefines,
										   [Optional] in Span< Include >     pInclude,
										   PCSTR                           pEntrypoint,
										   PCSTR                           pTarget,
										   uint                            Flags1,
										   uint                            Flags2,
										   out IBlob?                      ppCode,
										   out IBlob?                      ppErrorMsgs ) {
		unsafe {
			fixed( void* _defines = pDefines, _includes = pInclude ) {
				var hr = D3DCompileFromFile( pFileName,
											 (D3D_SHADER_MACRO *)_defines, 
											 (ID3DInclude *)_includes, 
											 pEntrypoint, pTarget,
											 Flags1, Flags2, 
											 out var code, 
											 out var errorMsgs ) ;

				ppCode = code is null ? null : new Blob( code ) ;
				ppErrorMsgs = errorMsgs is null ? null : new Blob( errorMsgs ) ;
				return hr ;
			}
		}
	}
	
	
	
	public static string? GetErrorMessage( IBlob errorBlob ) {
		ArgumentNullException.ThrowIfNull( errorBlob, nameof(errorBlob) ) ;
		unsafe {
			var errMsgPtr= errorBlob.GetBufferPointer( ) ;
			PCSTR  msg   = new( (byte *)errMsgPtr ) ;
			string error = msg.ToString( ) ;
			return error ;
		}
	}
	
} ;



/*
#define D3DCOMPILE_DEBUG                                (1 << 0)
#define D3DCOMPILE_SKIP_VALIDATION                      (1 << 1)
#define D3DCOMPILE_SKIP_OPTIMIZATION                    (1 << 2)
#define D3DCOMPILE_PACK_MATRIX_ROW_MAJOR                (1 << 3)
#define D3DCOMPILE_PACK_MATRIX_COLUMN_MAJOR             (1 << 4)
#define D3DCOMPILE_PARTIAL_PRECISION                    (1 << 5)
#define D3DCOMPILE_FORCE_VS_SOFTWARE_NO_OPT             (1 << 6)
#define D3DCOMPILE_FORCE_PS_SOFTWARE_NO_OPT             (1 << 7)
#define D3DCOMPILE_NO_PRESHADER                         (1 << 8)
#define D3DCOMPILE_AVOID_FLOW_CONTROL                   (1 << 9)
#define D3DCOMPILE_PREFER_FLOW_CONTROL                  (1 << 10)
#define D3DCOMPILE_ENABLE_STRICTNESS                    (1 << 11)
#define D3DCOMPILE_ENABLE_BACKWARDS_COMPATIBILITY       (1 << 12)
#define D3DCOMPILE_IEEE_STRICTNESS                      (1 << 13)
#define D3DCOMPILE_OPTIMIZATION_LEVEL0                  (1 << 14)
#define D3DCOMPILE_OPTIMIZATION_LEVEL1                  0
#define D3DCOMPILE_OPTIMIZATION_LEVEL2                  ((1 << 14) | (1 << 15))
#define D3DCOMPILE_OPTIMIZATION_LEVEL3                  (1 << 15)
#define D3DCOMPILE_RESERVED16                           (1 << 16)
#define D3DCOMPILE_RESERVED17                           (1 << 17)
#define D3DCOMPILE_WARNINGS_ARE_ERRORS                  (1 << 18)
#define D3DCOMPILE_RESOURCES_MAY_ALIAS                  (1 << 19)
#define D3DCOMPILE_ENABLE_UNBOUNDED_DESCRIPTOR_TABLES   (1 << 20)
#define D3DCOMPILE_ALL_RESOURCES_BOUND                  (1 << 21)
 */
 
[Flags]
public enum ShaderCompileFlags: uint {
	/// <summary>
	/// Insert debug file/line/type/symbol information.
	/// </summary>
	Debug = 1 << 0,
	/// <summary>
	/// Do not validate the generated code against known capabilities and
	/// constraints.  This option is only recommended when compiling shaders
	/// you KNOW will work.  (ie. have compiled before without this option.)
	/// Shaders are always validated by D3D before they are set to the device.
	/// </summary>
	SkipValidation = 1 << 1,
	/// <summary>
	/// Instructs the compiler to skip optimization steps during code generation.
	/// Unless you are trying to isolate a problem in your code using this option 
	/// is not recommended.
	/// </summary>
	SkipOptimization = 1 << 2,
	/// <summary>
	/// Unless explicitly specified, matrices will be packed in row-major order
	/// on input and output from the shader.
	/// </summary>
	PackMatrixRowMajor = 1 << 3,
	/// <summary>
	/// Unless explicitly specified, matrices will be packed in column-major 
	/// order on input and output from the shader.  This is generally more 
	/// efficient, since it allows vector-matrix multiplication to be performed
	/// using a series of dot-products.
	/// </summary>
	PackMatrixColumnMajor = 1 << 4,
	/// <summary>
	/// Force all computations in resulting shader to occur at partial precision.
	/// This may result in faster evaluation of shaders on some hardware.
	/// </summary>
	PartialPrecision = 1 << 5,
	/// <summary>
	/// Force compiler to compile against the next highest available software
	/// target for vertex shaders.  This flag also turns optimizations off, 
	/// and debugging on.
	/// </summary>
	ForceVertexShaderSoftwareNoOpt = 1 << 6,
	/// <summary>
	/// Force compiler to compile against the next highest available software
	/// target for pixel shaders.  This flag also turns optimizations off, 
	/// and debugging on.
	/// </summary>
	ForcePixelShaderSoftwareNoOpt = 1 << 7,
	/// <summary>
	/// Disables Preshaders. Using this flag will cause the compiler to not 
	/// pull out static expression for evaluation on the host cpu
	/// </summary>
	NoPreshader = 1 << 8,
	/// <summary>Hint compiler to avoid flow-control constructs where possible.</summary>
	AvoidFlowControl = 1 << 9,
	/// <summary>Hint compiler to prefer flow-control constructs where possible.</summary>
	PreferFlowControl = 1 << 10,
	/// <summary>
	/// By default, the HLSL/Effect compilers are not strict on deprecated syntax.
	/// Specifying this flag enables the strict mode. Deprecated syntax may be
	/// removed in a future release, and enabling syntax is a good way to make
	/// sure your shaders comply to the latest spec.
	/// </summary>
	EnableStrictness = 1 << 11,
	/// <summary>This enables older shaders to compile to 4_0 targets.</summary>
	EnableBackwardsCompatibility = 1 << 12,
	/// <summary>Forces the IEEE strict compile which avoids optimizations that may break IEEE rules.</summary>
	IeeeStrictness = 1 << 13,
	/// <summary>
	/// Directs the compiler to use the lowest optimization level. If you set this constant,
	/// the compiler might produce slower code but produces the code more quickly. Set this
	/// constant when you develop the shader iteratively.
	/// </summary>
	OptimizationLevel0 = 1 << 14,
	/// <summary>Directs the compiler to use the second lowest optimization level.</summary>
	OptimizationLevel1 = 0,
	/// <summary>Directs the compiler to use the second highest optimization level.</summary>
	OptimizationLevel2 = (1 << 14) | (1 << 15),
	/// <summary>
	/// Directs the compiler to use the highest optimization level. If you set this constant,
	/// the compiler produces the best possible code but might take significantly longer to
	/// do so. Set this constant for final builds of an application when performance is the
	/// most important factor.
	/// </summary>
	OptimizationLevel3 = 1 << 15,
	/// <summary>Reserved (not used).</summary>
	Reserved16 = 1 << 16,
	/// <summary>Reserved (not used).</summary>
	Reserved17 = 1 << 17,
	/// <summary>
	/// Directs the compiler to treat all warnings as errors when it compiles the shader code.
	/// We recommend that you use this constant for new shader code, so that you can resolve
	/// all warnings and lower the number of hard-to-find code defects.
	/// </summary>
	WarningsAreErrors = 1 << 18,
	/// <summary>
	/// Directs the compiler to assume that unordered access views (UAVs) and shader resource
	/// views (SRVs) may alias for cs_5_0.
	/// </summary>
	ResourcesMayAlias = 1 << 19,
	/// <summary>Directs the compiler to enable unbounded descriptor tables.</summary>
	EnableUnboundedDescriptorTables = 1 << 20,
	/// <summary>
	/// 
	/// </summary>
	AllResourcesBound = 1 << 21
} ;