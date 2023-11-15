#region Using Directives

using System.Collections.ObjectModel ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


/// <summary>
/// Represents the state of all currently set shaders as well
/// as certain fixed function state objects.
/// </summary>
[ProxyFor(typeof(ID3D12PipelineState))]
public interface IPipelineState: IPageable, IInstantiable {
	// ---------------------------------------------------------------------------------
	//! Creation Functions:
	// ---------------------------------------------------------------------------------
	internal static readonly ReadOnlyDictionary< Guid, Func<ID3D12PipelineState, IInstantiable> > _pipelineStateCreationFunctions =
		new( new Dictionary<Guid, Func<ID3D12PipelineState, IInstantiable> > {
			{ IPipelineState.IID, ( pComObj ) => new PipelineState( pComObj ) },
		} ) ;

	// ---------------------------------------------------------------------------------

	
	// ---------------------------------------------------------------------------------

	/// <summary>Gets the cached blob representing the pipeline state.</summary>
	/// <returns>
	/// Type: <b>ID3DBlob**</b> After this method returns, points to the cached blob representing the pipeline state.
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12pipelinestate-getcachedblob#parameters">
	/// Read more on docs.microsoft.com</a>.</para>
	/// </returns>
	/// <remarks>
	/// Refer to the remarks for <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_cached_pipeline_state">
	/// D3D12_CACHED_PIPELINE_STATE</a>.
	/// </remarks>
	IBlob GetCachedBlob( ) ;
	
	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof(ID3D12PipelineState) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12PipelineState).GUID
																   .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new PipelineState( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new PipelineState( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom obj ) => new PipelineState( ( obj as ID3D12PipelineState )! ) ;
	// ==================================================================================
} ;
