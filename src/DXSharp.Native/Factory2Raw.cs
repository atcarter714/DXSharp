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
    [PreserveSig] int CreateSwapChainForCoreWindow( nint device, nint window, in DXGI_SWAP_CHAIN_DESC1 description, nint output, out nint swapChain );
    [PreserveSig] int GetSharedResourceAdapterLuid( HANDLE resource, out LUID luid );
    [PreserveSig] int RegisterStereoStatusWindow( HWND window, uint message, out uint cookie );
    [PreserveSig] int RegisterStereoStatusEvent( HANDLE signal, out uint cookie );
    [PreserveSig] void UnregisterStereoStatus( uint cookie );
    [PreserveSig] int RegisterOcclusionStatusWindow( HWND window, uint message, out uint cookie );
    [PreserveSig] int RegisterOcclusionStatusEvent( HANDLE signal, out uint cookie );
    [PreserveSig] void UnregisterOcclusionStatus( uint cookie );
    [PreserveSig] int CreateSwapChainForComposition( nint device, in DXGI_SWAP_CHAIN_DESC1 description, nint output, out nint swapChain );
}
