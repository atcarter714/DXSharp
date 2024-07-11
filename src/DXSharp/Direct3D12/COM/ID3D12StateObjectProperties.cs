#pragma warning disable CS1591,CS1573,CS0465,CS0649,CS8019,CS1570,CS1584,CS1658,CS0436,CS8981
#region Using Directives
using System.Runtime.InteropServices;
using Windows.Win32.Foundation ;
using DXSharp.Windows.COM ;
#endregion
namespace Windows.Win32.Graphics.Direct3D12 ;


[ComImport, Guid("DE5FA827-9BF9-4F26-89FF-D7F56FDE3860"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown),]
public interface ID3D12StateObjectProperties: IUnknown {
	
	/// <summary>Retrieves the unique identifier for a shader that can be used in a shader record.</summary>
	/// <param name="pExportName">Entrypoint in the state object for which to retrieve an identifier.</param>
	/// <returns>
	/// <para>A pointer to the shader identifier. The data referenced by this pointer is valid as long as the state object it came from is valid.  The size of the data returned is <a href="https://docs.microsoft.com/windows/desktop/direct3d12/constants">D3D12_SHADER_IDENTIFIER_SIZE_IN_BYTES</a>.  Applications should copy and cache this data to avoid the cost of searching for it in the state object if it will need to be retrieved many times.  The identifier is used in shader records within shader tables in GPU memory, which the app must populate. The data itself globally identifies the shader, so even if the shader appears in a different state object with same associations, like any root signatures, it will have the same identifier. If the shader isn’t fully resolved in the state object, the return value is <b>nullptr</b>.</para>
	/// </returns>
	/// <remarks>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12stateobjectproperties-getshaderidentifier">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	[PreserveSig] unsafe void* GetShaderIdentifier( PCWSTR pExportName ) ;

	/// <summary>Gets the amount of stack memory required to invoke a raytracing shader in HLSL.</summary>
	/// <param name="pExportName">
	/// <para>The shader entrypoint in the state object for which to retrieve stack size.  For hit groups, an individual shader within the hit group must be specified using the syntax: hitGroupName::shaderType Where <i>hitGroupName</i> is the entrypoint name for the hit group and <i>shaderType</i> is one of: </para>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12stateobjectproperties-getshaderstacksize#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>Amount of stack memory, in bytes, required to invoke the shader.  If the shader isn’t fully resolved in the state object, or the shader is unknown or of a type for which a stack size isn’t relevant, such as a hit group, the return value is 0xffffffff.  The 32-bit 0xffffffff value is used  for the UINT64 return value to ensure that bad return values don’t get lost when summed up with other values as part of calculating an overall pipeline stack size.</returns>
	/// <remarks>
	/// <para>This method only needs to be called if the app wants to configure the stack size by calling <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12stateobjectproperties-setpipelinestacksize">SetPipelineStackSize</a>, rather than relying on the conservative default stack size. This method is only valid for ray generation shaders, hit groups, miss shaders, and callable shaders. Even ray generation shaders may return a non-zero value despite being at the bottom of the stack. For hit groups, stack size must be queried for the individual shaders comprising it (intersection shaders, any hit shaders, closest hit shaders), as each likely has a different stack size requirement.  The stack size can’t be queried on these individual shaders directly, as the way they are compiled can be influenced by the overall hit group that contains them.  The <i>pExportName</i> parameter includes syntax for identifying individual shaders within a hit group. This API can be called on either collection state objects or raytracing pipeline state objects.</para>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12stateobjectproperties-getshaderstacksize#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	[PreserveSig] ulong GetShaderStackSize( PCWSTR pExportName ) ;

	/// <summary>Gets the current pipeline stack size.</summary>
	/// <returns>The current pipeline stack size in bytes. When called on non-executable state objects, such as collections, the return value is 0.</returns>
	/// <remarks>This method and <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12stateobjectproperties-setpipelinestacksize">SetPipelineStackSize</a> are not re-entrant.  This means if calling either or both from separate threads, the app must synchronize on its own.</remarks>
	[PreserveSig] ulong GetPipelineStackSize( ) ;

	/// <summary>Set the current pipeline stack size.</summary>
	/// <param name="PipelineStackSizeInBytes">
	/// <para>Stack size in bytes to use during pipeline execution for each shader thread. There can be many thousands of threads in flight at once on the GPU. If the value is greater than 0xffffffff (the maximum value of a 32-bit UINT) the runtime will drop the call, and the debug layer will print an error, as this is likely the result of summing up invalid stack sizes returned from <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12stateobjectproperties-getshaderstacksize">GetShaderStackSize</a> called with invalid parameters, which return 0xffffffff.  In this case, the previously set stack size, or the default, remains.</para>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12stateobjectproperties-setpipelinestacksize#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <remarks>
	/// <para>This method and <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12stateobjectproperties-getpipelinestacksize">GetPipelineStackSize</a> are not re-entrant.  This means if calling either or both from separate threads, the app must synchronize on its own. The runtime drops calls to state objects other than raytracing pipelines, such as collections.</para>
	/// <para><a href="https://learn.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12stateobjectproperties-setpipelinestacksize#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	[PreserveSig] void SetPipelineStackSize( ulong PipelineStackSizeInBytes ) ;
} ;