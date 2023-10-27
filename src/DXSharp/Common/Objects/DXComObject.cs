using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
namespace DXSharp.Objects ;


public abstract class DXComObject: DisposableObject, 
								   IDXCOMObject {
	public static Type ComType => typeof(IUnknown) ;
	public static Guid InterfaceGUID => typeof(IUnknown).GUID ;
	
	public abstract ComPtr? ComPtrBase { get ; }
	public int RefCount => (int)( ComPtrBase?.RefCount ?? 0 ) ;

	//! IDisposable:
	protected override ValueTask DisposeUnmanaged( ) {
		ComPtrBase?.Dispose( ) ;
		return ValueTask.CompletedTask ;
	}
	
	
} ;

