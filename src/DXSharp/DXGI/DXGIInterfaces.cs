#region Using Directives
using System.Runtime.InteropServices;
using DXSharp.DXGI;
using DXSharp.Windows.COM;

#endregion

/*
namespace DXGI ;
public interface IObject : IUnknown<IObject>
{
	void SetPrivateData(ref Guid name, uint dataSize, IntPtr pData);
	void SetPrivateDataInterface(ref Guid name, IUnknown pUnknown);
	void GetPrivateData(ref Guid name, ref uint pDataSize, IntPtr pData);
	void GetParent<T>(ref Guid riid, out T ppParent) where T : class;
}*/

/*public interface IFactory : IObject
{
	void EnumAdapters(uint adapter, out IAdapter ppAdapter);
	void MakeWindowAssociation(IntPtr windowHandle, uint flags);
	void GetWindowAssociation(out IntPtr pWindowHandle);
	void CreateSwapChain(IUnknown pDevice, ref SwapChainDescription pDescription, out ISwapChain ppSwapChain);
	void CreateSoftwareAdapter(IntPtr module, out IAdapter ppAdapter);
}

internal interface IAdapter: IObject {
	void EnumOutputs( uint output, out IOutput ppOutput ) ;
	void GetDescription( out AdapterDescription pDescription ) ;
	void CheckInterfaceSupport( ref Guid interfaceName, out long pUMDVersion ) ;
} ;




*/