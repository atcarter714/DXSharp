using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace DXSharp.Framework.WinUI;

// Pinned Windows App SDK 2.7.0 include/microsoft.ui.xaml.media.dxinterop.h.
// This is WinUI's interface, distinct from the similarly named UWP interface.
[GeneratedComInterface( Options = ComInterfaceOptions.ComObjectWrapper )]
[Guid( "63aad0b8-7c24-40ff-85a8-640d944cc325" )]
internal partial interface ISwapChainPanelNative {
    [PreserveSig] int SetSwapChain( nint swapChain );
}
