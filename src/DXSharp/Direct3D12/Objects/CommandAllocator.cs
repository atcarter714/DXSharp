using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

namespace DXSharp.Direct3D12.Objects ;

public class CommandAllocator: ICommandAllocator {

	public static TInterface Instantiate< TInterface >( )
		where TInterface: class, IDXCOMObject {
		
	}
	
	//public void GetDevice( in Guid riid, out IDevice ppvDevice ) { }
	//public void GetPrivateData< TData >( out uint pDataSize, IntPtr pData ) where TData: unmanaged {}
	//public void SetPrivateData< T >( uint DataSize, IntPtr pData ) {}
	//public void SetPrivateDataInterface< T >( in T pUnknown ) where T: IUnknownWrapper< IUnknown > {}
	//public void SetName( string name ) {}
	//public void GetDevice( in Guid riid, out IDevice ppvDevice ) {}

	public ComPtr< ID3D12CommandAllocator >? ComPointer { get ; }
}