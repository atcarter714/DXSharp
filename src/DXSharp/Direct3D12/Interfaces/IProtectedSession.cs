#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[ProxyFor(typeof(ID3D12ProtectedSession))]
public interface IProtectedSession: IDeviceChild,
									IComObjectRef< ID3D12ProtectedSession >,
									IUnknownWrapper< ID3D12ProtectedSession >,
									IInstantiable {
	new ComPtr< ID3D12ProtectedSession >? ComPointer { get ; }
	new ID3D12ProtectedSession? COMObject => ComPointer?.Interface ;
	ID3D12DeviceChild? IDeviceChild.COMObject => COMObject ;
	ID3D12Object? IObject.COMObject => COMObject ;
	ComPtr< ID3D12DeviceChild >? IDeviceChild.ComPointer => new( COMObject! ) ;

	
	/// <summary>Retrieves the fence for the protected session. From the fence, you can retrieve the current uniqueness validity value (using ID3D12Fence::GetCompletedValue), and add monitors for changes to its value. This is a read-only fence.</summary>
	/// <param name="riid">The GUID of the interface to a fence. Most commonly, <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12fence">ID3D12Fence</a>, although it may be any GUID for any interface. If the protected session object doesn’t support the interface for this GUID, the function returns <b>E_NOINTERFACE</b>.</param>
	/// <param name="ppFence">A pointer to a memory block that receives a pointer to the fence for the given protected session.</param>
	/// <returns>If this method succeeds, it returns <b>S_OK</b>. Otherwise, it returns an <b>HRESULT</b> error code.</returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12protectedsession-getstatusfence">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	void GetStatusFence< TFence >( in Guid riid, out TFence ppFence ) where TFence: IFence, IInstantiable {
		unsafe {
			fixed ( Guid* pRiid = &riid ) {
				COMObject!.GetStatusFence( pRiid, out var fenceObj ) ;
				
				ppFence = (TFence)TFence.Instantiate( (ID3D12Fence)fenceObj ) ;
			}
		}
	}
	
	/// <summary>Gets the status of the protected session.</summary>
	/// <returns>
	/// <para>Type: **[D3D12_PROTECTED_SESSION_STATUS](/windows/desktop/api/d3d12/ne-d3d12-d3d12_protected_session_status)** The status of the protected session. If the returned value is [D3D12_PROTECTED_SESSION_STATUS_INVALID](/windows/desktop/api/d3d12/ne-d3d12-d3d12_protected_session_status), then you need to wait for a uniqueness value bump to reuse the resource if the session is an [ID3D12ProtectedResourceSession](/windows/desktop/api/d3d12/nn-d3d12-id3d12protectedresourcesession).</para>
	/// </returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12protectedsession-getsessionstatus">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	ProtectedSessionStatus GetSessionStatus( ) =>
		(ProtectedSessionStatus)COMObject!.GetSessionStatus( ) ;

	
	new static Type ComType => typeof(ID3D12ProtectedSession) ;
	static Type IUnknownWrapper.ComType => typeof(ID3D12ProtectedSession) ;
	new static Guid InterfaceGUID => typeof(ID3D12ProtectedSession).GUID ;
	static Guid IUnknownWrapper.InterfaceGUID => typeof(ID3D12ProtectedSession).GUID ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12ProtectedSession).GUID
																	   .ToByteArray( ) ;

			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference( data ) ) ;
		}
	}

	static IDXCOMObject IInstantiable.Instantiate( ) => new ProtectedSession( ) ;
	static IDXCOMObject IInstantiable.Instantiate( nint ptr ) => new ProtectedSession( ptr ) ;
	static IDXCOMObject IInstantiable.Instantiate< ICom >( ICom pComObj ) => new ProtectedSession( (ID3D12ProtectedSession?) pComObj! ) ;
} ;


internal class ProtectedSession: DeviceChild, IProtectedSession {
	public new ComPtr< ID3D12ProtectedSession >? ComPointer { get ; protected set ; }
	public new ID3D12ProtectedSession? COMObject => ComPointer?.Interface ;

	internal ProtectedSession( ) => ComPointer = default ;
	internal ProtectedSession( ID3D12ProtectedSession obj ) => ComPointer = new( obj ) ;
	internal ProtectedSession( nint ptr ) => ComPointer = new( ptr ) ;
	internal ProtectedSession( ComPtr< ID3D12ProtectedSession > ptr ) => ComPointer = ptr ;
} ;