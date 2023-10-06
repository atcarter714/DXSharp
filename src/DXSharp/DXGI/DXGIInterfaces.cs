using System.Runtime.InteropServices;
using DXSharp.DXGI;
using DXSharp.Windows.COM;
using IUnknown = DXSharp.Windows.COM.IUnknown;

namespace DXGI
{
	public interface IObject : IUnknown<IObject>
	{
		void SetPrivateData(ref          Guid name, uint     dataSize, IntPtr pData);
		void SetPrivateDataInterface(ref Guid name, IUnknown pUnknown);
		void GetPrivateData(ref          Guid name, ref uint pDataSize, IntPtr pData);
		void GetParent<T>(ref            Guid riid, out T    ppParent) where T : class;
	}

	public interface IFactory : IObject
	{
		void EnumAdapters(uint adapter, out IAdapter ppAdapter);
		void MakeWindowAssociation(IntPtr windowHandle, uint flags);
		void GetWindowAssociation(out IntPtr pWindowHandle);
		void CreateSwapChain(IUnknown pDevice, ref SwapChainDescription pDescription, out ISwapChain ppSwapChain);
		void CreateSoftwareAdapter(IntPtr module, out IAdapter ppAdapter);
	}

	public interface IAdapter : IObject
	{
		void EnumOutputs(uint                      output, out IOutput ppOutput);
		void GetDescription(out               AdapterDescription pDescription);
		void CheckInterfaceSupport(ref Guid        interfaceName, out long pUMDVersion);
	}

	public interface IOutput : IObject
	{
		void GetDescription( out OutputDescription pDescription );
		void GetDisplayModeList( Format enumFormat, uint flags, out uint pNumModes, ModeDescription[] pDescription );

		void FindClosestMatchingMode( ref ModeDescription pModeToMatch, out ModeDescription pClosestMatch,
									  IUnknown            pConcernedDevice );

		void WaitForVBlank();
		void TakeOwnership( IUnknown pDevice, bool exclusive );
		void ReleaseOwnership();
		void GetGammaControlCapabilities( out GammaControlCapabilities pGammaCaps );
		void SetGammaControl( ref             GammaControl             pArray );
		void GetGammaControl( out             GammaControl             pArray );
		void SetDisplaySurface( ISurface                               pScanoutSurface );
		void GetDisplaySurfaceData( ISurface                           pDestination );
		void GetFrameStatistics( out FrameStatistics                   pStats );
	};


	[ComImport, Guid( "cafcb56c-6ac3-4889-bf47-9e23bbd260ec" )]
	[InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
	public interface ISurface
	{
		void GetDesc( out SurfaceDescription pDesc );
		void Map( ref     MappedRect         pLockedRect, uint MapFlags );
		void Unmap();
	};

	[StructLayout(LayoutKind.Sequential)]
	public struct SurfaceDescription
	{
		public uint       Width;
		public uint       Height;
		public Format     Format;
		public SampleDescription SampleDesc;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MappedRect
	{
		public int    Pitch;
		public IntPtr pBits;
	}
}