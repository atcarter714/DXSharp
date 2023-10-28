#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[ProxyFor(typeof(ID3D12ProtectedResourceSession))]
public interface IProtectedResourceSession: IProtectedSession,
											IComObjectRef< ID3D12ProtectedResourceSession >,
											IUnknownWrapper< ID3D12ProtectedResourceSession >,
											IInstantiable {
	new ComPtr< ID3D12ProtectedResourceSession >? ComPointer { get ; }
	ComPtr< ID3D12ProtectedSession >? IProtectedSession.ComPointer => new( COMObject! ) ;
	ComPtr<ID3D12ProtectedSession>? IUnknownWrapper< ID3D12ProtectedSession >.ComPointer => new( COMObject! ) ;
	
	new ID3D12ProtectedResourceSession? COMObject => ComPointer?.Interface ;
	ID3D12ProtectedSession? IProtectedSession.COMObject => COMObject ;
	ID3D12ProtectedSession? DXSharp.IComObjectRef<ID3D12ProtectedSession>.COMObject => COMObject ;
	
	
	/// <summary>Retrieves a description of the protected resource session. (ID3D12ProtectedResourceSession.GetDesc)</summary>
	/// <returns>A <see cref="ProtectedResourceSessionDescription"/> that describes the protected resource session.</returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12protectedresourcesession-getdesc">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	ProtectedResourceSessionDescription GetDesc( ) {
		var desc = COMObject!.GetDesc( ) ;
		return desc ;
	}
	
	
	new static Type ComType => typeof(ID3D12ProtectedResourceSession) ;
	static Type IUnknownWrapper.ComType => typeof(ID3D12ProtectedResourceSession) ;
	new static Guid InterfaceGUID => typeof(ID3D12ProtectedResourceSession).GUID ;
	static Guid IUnknownWrapper.InterfaceGUID => typeof(ID3D12ProtectedResourceSession).GUID ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = InterfaceGUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	static IDXCOMObject IInstantiable.Instantiate( ) => new ProtectedResourceSession( ) ;
	static IDXCOMObject IInstantiable.Instantiate( nint ptr ) => new ProtectedResourceSession( ptr ) ;
	static IDXCOMObject IInstantiable.Instantiate< ICom >( ICom pComObj ) => new ProtectedResourceSession( (ID3D12ProtectedResourceSession?) pComObj! ) ;
} ;


[ProxyFor( typeof( ID3D12ProtectedResourceSession1 ) )]
public interface IProtectedResourceSession1: IProtectedResourceSession,
											 IComObjectRef< ID3D12ProtectedResourceSession1 >,
											 IUnknownWrapper< ID3D12ProtectedResourceSession1 > {
	new ComPtr< ID3D12ProtectedResourceSession1 >? ComPointer { get ; }
	new ID3D12ProtectedResourceSession1? COMObject => ComPointer?.Interface ;
	ID3D12ProtectedSession? IProtectedSession.COMObject => COMObject ;
	ComPtr< ID3D12ProtectedSession >? IProtectedSession.ComPointer => new( COMObject! ) ;

	ComPtr< ID3D12ProtectedResourceSession >? IProtectedResourceSession.ComPointer => new( COMObject! ) ;
	ID3D12ProtectedResourceSession? IComObjectRef<ID3D12ProtectedResourceSession>.COMObject => COMObject ;
	ComPtr<ID3D12ProtectedResourceSession>? IUnknownWrapper<ID3D12ProtectedResourceSession>.ComPointer => new( COMObject! ) ;
	
	/// <summary>Retrieves a description of the protected resource session. (ID3D12ProtectedResourceSession1::GetDesc1)</summary>
	/// <returns>A [D3D12_PROTECTED_RESOURCE_SESSION_DESC1](./ns-d3d12-d3d12_protected_resource_session_desc1.md) that describes the protected resource session.</returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12protectedresourcesession1-getdesc1">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	ProtectedResourceSessionDescription1 GetDesc1( ) => COMObject!.GetDesc1( ) ;

	
	new static Type ComType => typeof( ID3D12ProtectedResourceSession1 ) ;
	static Type IUnknownWrapper.ComType => typeof( ID3D12ProtectedResourceSession1 ) ;
	new static Guid InterfaceGUID => typeof( ID3D12ProtectedResourceSession1 ).GUID ;
	static Guid IUnknownWrapper.InterfaceGUID => typeof( ID3D12ProtectedResourceSession1 ).GUID ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = InterfaceGUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	
	static IDXCOMObject IInstantiable.Instantiate( ) => new ProtectedResourceSession1( ) ;
	static IDXCOMObject IInstantiable.Instantiate( nint ptr ) => new ProtectedResourceSession1( ptr ) ;
	static IDXCOMObject IInstantiable.Instantiate< ICom >( ICom pComObj ) => new ProtectedResourceSession1( (ID3D12ProtectedResourceSession1?) pComObj! ) ;
} ;



internal class ProtectedResourceSession: DeviceChild, IProtectedResourceSession {
	public new ComPtr< ID3D12ProtectedResourceSession >? ComPointer { get ; protected set ; }
	public new ID3D12ProtectedResourceSession? COMObject => ComPointer?.Interface ;

	internal ProtectedResourceSession( ) => ComPointer = default ;
	internal ProtectedResourceSession( ID3D12ProtectedResourceSession obj ) => ComPointer = new( obj ) ;
	internal ProtectedResourceSession( nint ptr ) => ComPointer = new( ptr ) ;
	internal ProtectedResourceSession( ComPtr< ID3D12ProtectedResourceSession > ptr ) => ComPointer = ptr ;
} ;


internal class ProtectedResourceSession1: DeviceChild, IProtectedResourceSession1 {
	public new ComPtr< ID3D12ProtectedResourceSession1 >? ComPointer { get ; protected set ; }
	public new ID3D12ProtectedResourceSession1? COMObject => ComPointer?.Interface ;

	internal ProtectedResourceSession1( ) => ComPointer = default ;
	internal ProtectedResourceSession1( ID3D12ProtectedResourceSession1           obj ) => ComPointer = new( obj ) ;
	internal ProtectedResourceSession1( nint                                     ptr ) => ComPointer = new( ptr ) ;
	internal ProtectedResourceSession1( ComPtr< ID3D12ProtectedResourceSession1 > ptr ) => ComPointer = ptr ;
} ;
