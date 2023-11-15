#region Using Directives

using Windows.Win32 ;
using DXSharp ;
using DXSharp.Windows ;
using DXSharp.DXGI.Debugging ;
using DXSharp.Direct3D12 ;
using DXSharp.Windows.COM ;
using DXSharp.Direct3D12.Debugging ;

using D3D12Debug = DXSharp.Direct3D12.IDebug ;
using DXGIDebug = DXSharp.DXGI.Debugging.IDebug ;
#endregion
namespace DXSharp.Framework.Debugging ;


public interface IDebugSupport: IInstantiable {
	/// <summary>Gets the debug layer interface wrapper for the application.</summary>
	IInstantiable? DebugLayer { get ; }
	
	static abstract IDebugSupport Create< TOut, TLayer >( )
									where TOut: IDebugSupport
									where TLayer: IInstantiable, IComIID ;
} ;

//! D3D12 Layers:
public interface IDebugD3D12Support< T >: IDebugSupport
							   where T: D3D12Debug {
	T Instance { get ; }
	IInstantiable? IDebugSupport.DebugLayer =>
		Instance as IInstantiable ;
} ;

//! DXGI Layers:
public interface IDebugDXGISupport< T >: IDebugSupport
							  where T: DXGIDebug {
	T Instance { get ; }
	IInstantiable? IDebugSupport.DebugLayer =>
		Instance as IInstantiable ;
} ;


// --------------------------------------------------------------------------
//! Setup for Debugging Systems:
public interface IDebugSetup< TD3D, TDxgi > where TD3D: D3D12Debug
											 where TDxgi: DXGIDebug {
	// -----------------------------------------------------------------------
	/// <summary>Gets the debugging configuration for the framework debugging system services.</summary>
	DebuggingConfig? Config { get ; }
	/// <summary>Gets the Direct3D12 debugging layer interface wrapper for the application.</summary>
	TD3D Direct3DLayer { get ; }
	/// <summary>Gets the DXGI debugging layer interface wrapper for the application.</summary>
	TDxgi DXGILayer { get ; }
	
	
	/// <summary>
	/// Indicates if leak tracking is enabled on the accessing thread
	/// (i.e., <see cref="Thread"/>.<see cref="Thread.CurrentThread"/>).
	/// </summary>
	bool IsLeakTrackingEnabled { get ; }
	
	// -----------------------------------------------------------------------
	
	/// <summary>Enables the debugging system for the application.</summary>
	/// <param name="config"><b>[Optional]</b> The debugging configuration for the application.</param>
	public void Enable( DebuggingConfig? config = default ) ;
	
	/// <summary>Disables the debugging system for the application.</summary>
	public void DisableD3D12Layers( ) ;
	
	
	/// <summary>Enables the DXGI debugging system for the application.</summary>
	/// <param name="flags"><b>[Optional]</b> A set of flags that specify which DXGI debugging features to enable.</param>
	/// <param name="iidFilter"><b>[Optional]</b> Globally unique identifier (GUID) values that identify producers of debug messages.</param>
	public void EnableDXGILayers( DebugRLOFlags flags = DebugRLOFlags.All,
								  params Guid[ ]? iidFilter ) ;
	
	
	/// <summary>Enables the Direct3D12 debugging system for the application.</summary>
	/// <param name="gpuValidation"><b>[Optional]</b> Enable GPU-based validation for the Direct3D12 debugging system?</param>
	/// <param name="flags"><b>[Optional]</b> A set of flags that specify which Direct3D12 debugging features to enable.</param>
	/// <param name="syncCmdQueue"><b>[Optional]</b> Enable dependent command queue synchronization for the Direct3D12 debugging system?</param>
	/// <param name="autoNaming"><b>[Optional]</b> Enable auto-naming for Direct3D12 objects?</param>
	/// <param name="forceBarrier"><b>[Optional]</b> Force barrier validation for the Direct3D12 debugging system?</param>
	public void EnableD3D12Layers( bool gpuValidation = true,
								   GPUBasedValidationFlags flags =
									   GPUBasedValidationFlags.None,
								   bool syncCmdQueue = true,
								   bool autoNaming   = true,
								   bool forceBarrier = true ) ;
	
	
	/// <summary>
	/// Sets leak tracking on or off for the current thread.
	/// </summary>
	/// <param name="enable">
	/// <b>[Optional]</b> Leak tracking state flag ...<para/>
	/// Use <b><see langword="true"/></b> to enable leak tracking<para/>
	/// Use <b><see langword="false"/></b> to disable leak tracking
	/// </param>
	void SetLeakTrackingOnThisThread( bool enable = true ) ;
	
	// =========================================================================
} ;