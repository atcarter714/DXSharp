/* ----------------------------------------------------
 * NOTES ::
 * ----------------------------------------------------
 * This test is ensuring our MSBuild process has copied
 * the test assets to the correct output location. The
 * first basic test is to ensure we ended up with an
 * "\Assets" folder in the current directory, which will
 * be the test's build output directory in "\bin\Tests".
 * There is a subfolder in "\Assets" called "\Shaders",
 * corresponding to the "Shaders" folder in the project.
 * The build script should automatically gather any hlsl
 * files in the "\Shaders" folder and copy them to them
 * to a matching folder in the build output directory.
 * This is so applications and tests that need to load
 * and build a shader at runtime simply include them in
 * the project and the build scripts handles the rest.
 *	- Placing a shader in any "\omit" folder will disable this.
 * ---------------------------------------------------- */

namespace BasicTests.Assets ;


[TestFixture( Author = "Aaron T. Carter",
			  Category = "Build Validation",
			  Description = "Checks that all the assets are processed correctly.")]
public class Test_Build_Assets {
	// -------------------------
	//! Test Constants & Config:
	// -------------------------
	
	#region Const & ReadOnly Values
	const string ASSETS_DIR_NAME     = @"Assets" ;
	const string SHADERS_DIR_NAME    = @"Shaders" ;
	const string TEST_HLSL_FILE_NAME = @"tests_shader_0.hlsl" ;
	
	static readonly string _AssetsDir = 
		Path.Combine( Directory.GetCurrentDirectory( ), ASSETS_DIR_NAME ) ;
	static readonly string _ShadersDir = 
		Path.Combine( _AssetsDir, SHADERS_DIR_NAME ) ;
	static readonly string _TestShaderPath = 
		Path.Combine( _ShadersDir, TEST_HLSL_FILE_NAME ) ;
	static readonly string _SolutionDir = 
		Path.Combine( Directory.GetCurrentDirectory( ), 
					  @"..\..\" ) ;

	
	static readonly string dotNetTargetDir 
			= Environment.CurrentDirectory ;
	const string basictestpath =
		@"C:\Users\atcar\source\repos\AC\DXSharp\bin\Tests\BasicTests\AnyCPU\Debug\net7.0-windows10.0.22621.0" ;

	static readonly string[ ] _ShaderExtensions = {
		".hlsl", ".hlsli", ".fx", ".h", ".fxh", ".cginc", ".glsl", "inc", ".shader",
		".ps", ".vs", ".gs", ".hs", ".ds", ".cs", ".ms", ".as", ".pas", ".psh", ".vsh",
		".vert", ".geom", ".frag", ".comp", ".compute", ".mesh", ".indices", 
		".obj", ".blob", ".d3dblob", ".cso",
	} ;
	#endregion
	
	
	
	// ----------------------------------------------------
	// Fields & Properties:
	// ----------------------------------------------------
	
	static string startup_dir ;
	static string _CurrentDir => 
		Directory.GetCurrentDirectory( ) ;
	
	
	// ----------------------------------------------------
	// Setup & Teardown:
	// ----------------------------------------------------
	[SetUp] public void Setup( ) => 
		startup_dir = Directory.GetCurrentDirectory( ) ;
	
	[TearDown] public void TearDown( ) { }
	// ----------------------------------------------------
	
	
	
	// ----------------------------------------------------
	// Tests:
	// ----------------------------------------------------
	
	[ Order(0),
		Test(Description = "Checks that the assets directory exists.") ]
	public void Test_Ensure_Assets_Folder_Exists( ) {
		if( !IsTestValid( ) )
			throw new InconclusiveException( $"Test deemed invalid! " +
											 $"Working directory does not match startup." ) ;
		
		// Check that the Assets directory exists:
		Assert.That( Directory.Exists( _AssetsDir ),
					   "\"\\Assets\" directory does not exist." ) ;
	}
	
	
	[ Order(1),
		Test(Description = "Checks that the shaders directory exists.") ]
	public void Test_Ensure_Shaders_Folder_Exists( ) {
		// Check that the Shaders directory exists:
		Assert.That( Directory.Exists( _ShadersDir ),
					   "\"\\Assets\\Shaders\" directory does not exist." ) ;
	}
	
	
	[ Order(2),
		Test(Description = "Checks that the test shader exists.") ]
	public void Test_Ensure_Test_Shader_Exists( ) {
		// Check that the test shader exists:
		Assert.That( File.Exists( _TestShaderPath ),
					   $"\"\\Assets\\Shaders\\{TEST_HLSL_FILE_NAME}\" does not exist." ) ;
	}
	
	
	
	
	// ----------------------------------------------------
	// Static Helper Methods:
	// ----------------------------------------------------
	/// <summary>
	/// Makes sure the test is running in the correct directory
	/// and matches the startup directory. This should always be
	/// true, but it will fail if anything is broken or being
	/// tampered with by other code/scripts and alert you.
	/// </summary>
	/// <returns>
	/// Should always return true, but will return false if paths
	/// get messed up in any sort of way (including messing up the
	/// const strings in this test).
	/// </returns>
	static bool IsTestValid( ) {
		var currentDir = Directory.GetCurrentDirectory( ) ;
		Assert.That( currentDir, Is.EqualTo(startup_dir),
					 "Test Invalid: Current directory does not match startup directory." ) ;
		return currentDir == startup_dir ;
	}
	// ====================================================
} ;