#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[Wrapper(typeof(ID3D12GraphicsCommandList))]
public class GraphicsCommandList: CommandList,
								  IGraphicsCommandList {
	
	public new static Type ComType => typeof(ID3D12GraphicsCommandList ) ;
	public new static Guid InterfaceGUID => typeof(ID3D12GraphicsCommandList).GUID ;
	public new ComPtr< ID3D12GraphicsCommandList >? ComPointer { get ; protected set ; }

	public new ID3D12GraphicsCommandList? COMObject => ComPointer?.Interface ;
	
	//! This makes the COMObject ref not null when interpreted as an ICommandList:
	ID3D12CommandList? ICommandList.COMObject => ComPointer?.Interface ;

	internal GraphicsCommandList( ) { }
	internal GraphicsCommandList( nint ptr ) => ComPointer = new(ptr) ;
	internal GraphicsCommandList( ComPtr< ID3D12GraphicsCommandList > comPointer ) => ComPointer = comPointer ;
	internal GraphicsCommandList( ID3D12GraphicsCommandList obj ) => ComPointer = new(obj) ;

	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Heap).GUID
														  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}

} ;