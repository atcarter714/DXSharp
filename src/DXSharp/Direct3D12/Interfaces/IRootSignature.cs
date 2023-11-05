#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[ProxyFor( typeof(ID3D12RootSignature) )]
public interface IRootSignature: IDeviceChild,
								 IComObjectRef< ID3D12RootSignature >,
								 IUnknownWrapper< ID3D12RootSignature > {
	// ---------------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12RootSignature) ;
	public new static Guid IID => ( ComType.GUID ) ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12RootSignature).GUID
														  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	// ==================================================================================
} ;