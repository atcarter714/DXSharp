using DXSharp.Windows.COM ;
namespace DXSharp.Objects ;


public abstract class DXComObject: DisposableObject, 
								   IDXCOMObject {
	public static Type ComType => typeof(IUnknown) ;
	public static Guid InterfaceGUID => typeof(IUnknown).GUID ;
	
	public abstract ComPtr? ComPtrBase { get ; }
	public int RefCount => (int)( ComPtrBase?.RefCount ?? 0 ) ;

	//! IDisposable:
	protected override void DisposeUnmanaged( ) => ComPtrBase?.Dispose( ) ;
} ;

