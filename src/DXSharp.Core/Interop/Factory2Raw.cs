using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dxgi;

namespace DXSharp.Interop;

// CsWin32's typed IDXGISwapChain1 output uses a shared projection. This narrow
// generated declaration preserves the native output reference for unique ownership.
// All inherited slots come from the metadata-generated IDXGIFactory1 declaration.
[GeneratedComInterface( Options = ComInterfaceOptions.ComObjectWrapper )]
[Guid( "50c83a1c-e072-4c48-87b0-3630fa36a6d0" )]
internal unsafe partial interface IFactory2Raw : IDXGIFactory1 {
    [PreserveSig] BOOL IsWindowedStereoEnabled( );
    [PreserveSig] int CreateSwapChainForHwnd( nint device, HWND window, in DXGI_SWAP_CHAIN_DESC1 description,
        DXGI_SWAP_CHAIN_FULLSCREEN_DESC* fullscreen, nint output, out nint swapChain );
}
