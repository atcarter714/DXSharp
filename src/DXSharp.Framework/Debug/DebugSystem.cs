#region Using Directives
using System.Runtime.Versioning ;
using DXSharp.Direct3D12 ;
using DXSharp.DXGI ;
using DebugRLOFlags = DXSharp.DXGI.Debugging.DebugRLOFlags ;
#endregion
namespace DXSharp.Framework.Debugging ;


/// <summary>
/// A record that contains a configuration for the framework debugging system services.
/// </summary>
/// <param name="EnableD3D12">Enable the Direct3D12 debug layer?</param>
/// <param name="EnableDXGI">Enable the DXGI debug layer?</param>
/// <param name="EnableAutoName">Enable DirectX12 object auto-naming?</param>
/// <param name="EnableGPUBasedValidation">Enable DirectX12 GPU-based validation?</param>
/// <param name="GPUBasedValidationFlags">Flags for GPU-based validation (typically <see cref="GPUBasedValidationFlags.None"/>).</param>
/// <param name="EnableSyncCmdQueue">Enable dependent command queue synchronization for Direct3D12 device?</param>
/// <param name="EnableForceBarrier">Should DirectX12 debug layer force barrier validation?</param>
/// <param name="EnableLeakTrackingForMainThread"></param>
/// <param name="iidFilter">
/// <b>[Optional]</b> Globally unique identifier (GUID) values that identify producers of debug messages.
/// These values can be obtained from <see cref="DXGI.DebugID"/>.
/// </param>
[Debugging]
public record DebuggingConfig(  bool EnableD3D12 = true,
								bool EnableDXGI = true,
								bool EnableAutoName = true,
								bool EnableGPUBasedValidation = true,
								GPUBasedValidationFlags GPUBasedValidationFlags =
									GPUBasedValidationFlags.None,
								bool EnableSyncCmdQueue = true,
								bool EnableForceBarrier = true,
								bool EnableLeakTrackingForMainThread = true,
								params Guid[ ]? iidFilter ) {
	/// <summary>The default set of debugging configuration options.</summary>
	/// <remarks>(This essentially enables everything.)</remarks>
	public static readonly DebuggingConfig Default = new( ) ;
	
	/// <summary>Set of debugging configuration settings with all options turned off/false.</summary>
	public static readonly DebuggingConfig Off = new( false, false,
													  false, false,
													   GPUBasedValidationFlags.None,
													   false, false ) ;
} ;



/// <summary>Provides access to the Direct3D12 and DXGI debug layers.</summary>
[Debugging( SDKModules.All )]
[SupportedOSPlatform("windows8.1")]
public class DebugSystem< TD3d, TDxgi >: DisposableObject,
										 IDebugSetup< TD3d, TDxgi >
											where TD3d  : Direct3D12.IDebug
											where TDxgi : DXGI.Debugging.IDebug {
	// -----------------------------------------------------------------------
	protected DebuggingConfig? _config ;
	
	/// <summary>
	/// Gets the debugging configuration for the
	/// framework debugging system services.
	/// </summary>
	public DebuggingConfig Config =>
		_config ??= DebuggingConfig.Default ;
	
	public TD3d Direct3DLayer { get ; }
	public TDxgi DXGILayer { get ; }
	
	/// <summary>Indicates if leak tracking is enabled on the accessing thread.</summary>
	public bool IsLeakTrackingEnabled =>
		( this.DXGILayer as DXGI.Debugging.IDebug1 )?
			.IsLeakTrackingEnabledForThread( ) ?? false ;
	
	// -----------------------------------------------------------------------

	public DebugSystem( ) {
		var hr = Direct3D12.D3D12.GetDebugInterface( out TD3d? d3d ) ;
		hr.ThrowOnFailure( ) ;
		this.Direct3DLayer = (TD3d?)d3d 
							 ?? throw new DirectXComError( $"Failed to create Direct3D12 debug layer!" ) ;
		
		hr = DXGI.DXGIFunctions.GetDebugInterface1( DXGI.Debugging.IDebug1.IID, out var dxgi ) ;
		hr.ThrowOnFailure( ) ;
		this.DXGILayer = (TDxgi?)dxgi 
						 ?? throw new DirectXComError( $"Failed to create DXGI debug layer!" ) ;
	}
	~DebugSystem( ) => this.Dispose( false ) ;
	
	// -----------------------------------------------------------------------
	
	public void Enable( DebuggingConfig? config = default ) {
		if( config is null ) config = DebuggingConfig.Default ;
		
		if( config.EnableDXGI ) {
			this.EnableDXGILayers( DebugRLOFlags.All, config.iidFilter ) ;
			var dxgi1 = this.DXGILayer as DXGI.Debugging.IDebug1 
								?? throw new NullReferenceException( ) ;
			
			// Set leak tracking option for main thread:
			if( Config.EnableLeakTrackingForMainThread )
				dxgi1.EnableLeakTrackingForThread( ) ;
			else {
				if( this.IsLeakTrackingEnabled )
					dxgi1?.DisableLeakTrackingForThread( ) ;
			}
		}
		if( config.EnableD3D12 ) {
			this.EnableD3D12Layers( config.EnableGPUBasedValidation,
									config.GPUBasedValidationFlags,
									config.EnableSyncCmdQueue,
									config.EnableAutoName,
									config.EnableForceBarrier ) ;
		}
	}
	
	
	public void EnableDXGILayers( DebugRLOFlags flags = DebugRLOFlags.All, 
												params Guid[ ]? iidFilter ) {
		var dxgi1 = this.DXGILayer as DXGI.Debugging.IDebug1 
							?? throw new NullReferenceException( ) ;
		var hr = dxgi1.ReportLiveObjects( IFactory.IID, flags ) ;
		hr.ThrowOnFailure( ) ;
	}
	
	
	public void EnableD3D12Layers( bool gpuValidation = true,
								   GPUBasedValidationFlags flags = 
									   GPUBasedValidationFlags.None,
								   bool syncCmdQueue = true,
								   bool autoNaming = true,
								   bool forceBarrier = true ) {
		var d3d6 = this.Direct3DLayer as Direct3D12.IDebug6 
							?? throw new NullReferenceException( ) ;
		var hr = d3d6.EnableDebugLayer( ) ;
		hr.ThrowOnFailure( ) ;
		
		d3d6.SetEnableGPUBasedValidation( gpuValidation ) ;
		d3d6.SetGPUBasedValidationFlags( flags ) ;
		
		d3d6.SetEnableAutoName( autoNaming ) ;
		
		d3d6.SetEnableSynchronizedCommandQueueValidation( syncCmdQueue ) ;
		d3d6.SetForceLegacyBarrierValidation( forceBarrier ) ;
	}

	
	public void DisableD3D12Layers( ) {
		var d3d6 = this.Direct3DLayer as Direct3D12.IDebug6 
							?? throw new NullReferenceException( ) ;
		var hr = d3d6.DisableDebugLayer( ) ;
		hr.ThrowOnFailure( ) ;
	}

	
	public void SetLeakTrackingOnThisThread( bool enable = true ) {
		var dxgi1 = this.DXGILayer as DXGI.Debugging.IDebug1
							?? throw new NullReferenceException( ) ;
		if( enable ) dxgi1!.EnableLeakTrackingForThread( ) ;
		else dxgi1!.DisableLeakTrackingForThread( ) ;
	}
	
	// -----------------------------------------------------------------------
	protected override async ValueTask DisposeUnmanaged( ) {
		var task1 = this.Direct3DLayer.DisposeAsync( ).AsTask(  ) ;
		var task2 = this.DXGILayer.DisposeAsync( ).AsTask(  ) ;
		await Task.WhenAll( task1, task2 ) ;
	}
	public override async ValueTask DisposeAsync( ) {
		var task1 = Task.Run( Dispose ) ;
		var task2 = base.DisposeAsync( ).AsTask( ) ;
		await Task.WhenAll( task1, task2 ) ;
	}
	// =======================================================================
} ;