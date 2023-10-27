using System.Runtime.InteropServices ;
using DXSharp.Direct3D12 ;
using DXSharp.Windows.COM ;

namespace Windows.Win32.Graphics.Direct3D12 ;

[ComImport, Guid( "E667AF9F-CD56-4F46-83CE-032E595D70A8" ),
 InterfaceType( ComInterfaceType.InterfaceIsIUnknown ),]
public interface ID3D12LifetimeOwner: IUnknown {
	/// <summary>Called when the lifetime state of a lifetime-tracked object changes.</summary>
	/// <param name="NewState">The new state.
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12lifetimeowner-lifetimestateupdated#parameters">Read more on docs.microsoft.com</a>.</para>
	/// </param>
	[PreserveSig]
	void LifetimeStateUpdated( LifetimeState NewState ) ;
} ;
