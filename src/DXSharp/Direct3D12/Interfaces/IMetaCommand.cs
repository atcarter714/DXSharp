#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


/// <summary>
/// Represents a meta command. A meta command is a Direct3D 12 object representing an algorithm that is accelerated by independent
/// hardware vendors (IHVs). It's an opaque reference to a command generator that is implemented by the driver.<para/>
/// The lifetime of a meta command is tied to the lifetime of the command list that references it. So, you should only free a meta command
/// if no command list referencing it is currently executing on the GPU.<para/>
/// A meta command can encapsulate a set of pipeline state objects (PSOs), bindings, intermediate resource states, and Draw/Dispatch calls.
/// You can think of the signature of a meta command as being similar to a C-style function, with multiple in/out parameters, and no return value.
/// </summary>
/// <remarks>
/// For more information, please see:
/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/d3d12/nn-d3d12-id3d12metacommand">ID3D12MetaCommand</a>
/// </remarks>
[ProxyFor(typeof(ID3D12MetaCommand))]
public interface IMetaCommand: IPageable,
							   IInstantiable {
	
	/// <summary>Retrieves the amount of memory required for the specified runtime parameter resource for a meta command, for the specified stage.</summary>
	/// <param name="stage">
	/// <para>Type: <b>D3D12_META_COMMAND_PARAMETER_STAGE</b> A <b>D3D12_META_COMMAND_PARAMETER_STAGE</b> specifying the stage to which the parameter belongs.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12metacommand-getrequiredparameterresourcesize#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <param name="parameterIndex">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The zero-based index of the parameter within the stage.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12metacommand-getrequiredparameterresourcesize#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT64</a></b> The number of bytes required for the  specified  runtime parameter resource.</para>
	/// </returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12metacommand-getrequiredparameterresourcesize">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	ulong GetRequiredParameterResourceSize( MetaCommandParameterStage stage, uint parameterIndex ) ;
	
	
	public new static Type ComType => typeof( ID3D12MetaCommand ) ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12MetaCommand).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable. Instantiate( ) => new MetaCommand( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new MetaCommand( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new MetaCommand( (ID3D12MetaCommand?) pComObj! ) ;
} ;



/// <summary>Provides a base class for <see cref="IMetaCommand"/>. </summary>
[Wrapper(typeof(ID3D12MetaCommand))]
internal class MetaCommand: Pageable, 
							IMetaCommand,
							IComObjectRef< ID3D12MetaCommand >,
							IUnknownWrapper< ID3D12MetaCommand > {
	// -----------------------------------------------------------------------------------------------
	ComPtr< ID3D12MetaCommand >? _comPtr ;
	public new ComPtr< ID3D12MetaCommand >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< ID3D12MetaCommand >( ) ;
	public override ID3D12MetaCommand? ComObject => ComPointer?.Interface ;
	// -----------------------------------------------------------------------------------------------
	
	internal MetaCommand( ) {
		_comPtr = ComResources?.GetPointer< ID3D12MetaCommand >( ) ;
	}
	internal MetaCommand( ComPtr< ID3D12MetaCommand > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal MetaCommand( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal MetaCommand( ID3D12MetaCommand? comObject ) {
		_comPtr = new( comObject! ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// -----------------------------------------------------------------------------------------------
	
	public ulong GetRequiredParameterResourceSize( MetaCommandParameterStage stage, uint parameterIndex ) => 
		ComObject!.GetRequiredParameterResourceSize( (D3D12_META_COMMAND_PARAMETER_STAGE)stage, parameterIndex ) ;
	
	// -----------------------------------------------------------------------------------------------
	
	public new static Type ComType => typeof(ID3D12MetaCommand) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12MetaCommand).GUID
															  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	// ===============================================================================================
} ;