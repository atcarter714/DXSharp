using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

namespace DXSharp.Direct3D12 ;

public interface IMetaCommand: IPageable,
							   IComObjectRef< ID3D12MetaCommand >,
							   IUnknownWrapper< ID3D12MetaCommand >,
							   IInstantiable {
	
	public new static Type ComType => typeof(ID3D12MetaCommand ) ;
	public new static Guid InterfaceGUID => typeof(ID3D12MetaCommand).GUID ;
	new ComPtr< ID3D12MetaCommand >? ComPointer { get ; }

	public new ID3D12MetaCommand? COMObject => ComPointer?.Interface ;
	ID3D12Pageable? IPageable.COMObject => ComPointer?.Interface ;

	static Type IUnknownWrapper.ComType => ComType ;
	static Guid IUnknownWrapper.InterfaceGUID => InterfaceGUID ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = InterfaceGUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}

	
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
	ulong GetRequiredParameterResourceSize( MetaCommandParameterStage stage, uint parameterIndex ) => 
		COMObject!.GetRequiredParameterResourceSize( (D3D12_META_COMMAND_PARAMETER_STAGE)stage, parameterIndex ) ;


	static IInstantiable IInstantiable. Instantiate( )                      => new MetaCommand( ) ;
	static IInstantiable IInstantiable.Instantiate( IntPtr       ptr )     => new MetaCommand( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new MetaCommand( (ID3D12MetaCommand?) pComObj! ) ;
} ;

/// <summary>Provides a base class for <see cref="IMetaCommand"/>. </summary>
[Wrapper(typeof(ID3D12MetaCommand))]
internal class MetaCommand: Pageable, IMetaCommand {
	public new ComPtr< ID3D12MetaCommand >? ComPointer { get ; protected set ; }
	public new ID3D12MetaCommand? COMObject => ComPointer?.Interface ;

	internal MetaCommand( ) => ComPointer = default ;
	internal MetaCommand( ID3D12MetaCommand obj ) => ComPointer = new( obj ) ;
	internal MetaCommand( nint ptr ) => ComPointer = new( ptr ) ;
	internal MetaCommand( ComPtr< ID3D12MetaCommand > ptr ) => ComPointer = ptr ;
} ;