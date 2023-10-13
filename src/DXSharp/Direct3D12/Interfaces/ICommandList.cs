using Windows.Win32.Graphics.Direct3D12 ;

namespace DXSharp.Direct3D12 ;

public interface ICommandList: IDeviceChild {
	CommandListType GetType( ) ;
} ;