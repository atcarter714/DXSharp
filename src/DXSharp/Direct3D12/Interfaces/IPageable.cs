#region Using Directives

using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


/// <summary>
/// A core D3D12 base interface which indicates that the object type encapsulates
/// some amount of GPU-accessible memory; but does not strongly indicate whether
/// the application can manipulate the object's residency.
/// </summary>
/// <remarks>
/// To learn more about the <see cref="IPageable"/> interface, see the documentation for
/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/d3d12/nn-d3d12-id3d12pageable">ID3D12Pageable</a>.
/// </remarks>
[ProxyFor( typeof( ID3D12Pageable ) )]
public interface IPageable: IDeviceChild,
							IComObjectRef< ID3D12Pageable >, 
							IUnknownWrapper< ID3D12Pageable > {
	// ---------------------------------------------------------------------------------
	new ComPtr< ID3D12Pageable >? ComPointer { get ; }
	new ID3D12Pageable? COMObject => ComPointer?.Interface ;
	ID3D12Pageable? IComObjectRef< ID3D12Pageable >.COMObject => COMObject ;
	ComPtr< ID3D12Pageable >? IUnknownWrapper< ID3D12Pageable >.ComPointer => ComPointer ;
	
	ID3D12DeviceChild? IDeviceChild.COMObject => COMObject ;
	// ---------------------------------------------------------------------------------
	
	// ---------------------------------------------------------------------------------
	static Type IUnknownWrapper.ComType => typeof(ID3D12Pageable) ;
	static Guid IUnknownWrapper.InterfaceGUID => typeof(ID3D12Pageable).GUID ;
	
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Pageable).GUID
															.ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	// ==================================================================================
} ;

