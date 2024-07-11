#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[ProxyFor(typeof(ID3D12ProtectedResourceSession))]
public interface IProtectedResourceSession: IProtectedSession {
	
	/// <summary>Retrieves a description of the protected resource session. (ID3D12ProtectedResourceSession.GetDesc)</summary>
	/// <returns>A <see cref="ProtectedResourceSessionDescription"/> that describes the protected resource session.</returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12protectedresourcesession-getdesc">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	ProtectedResourceSessionDescription GetDesc( ) ;
	
	new static Type ComType => typeof(ID3D12ProtectedResourceSession) ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12ProtectedResourceSession).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new ProtectedResourceSession( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new ProtectedResourceSession( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new ProtectedResourceSession( (ID3D12ProtectedResourceSession?) pComObj! ) ;
} ;


[ProxyFor( typeof( ID3D12ProtectedResourceSession1 ) )]
public interface IProtectedResourceSession1: IProtectedResourceSession,
											 IComObjectRef< ID3D12ProtectedResourceSession1 >,
											 IUnknownWrapper< ID3D12ProtectedResourceSession1 > {
	
	/// <summary>Retrieves a description of the protected resource session. (ID3D12ProtectedResourceSession1::GetDesc1)</summary>
	/// <returns>A [D3D12_PROTECTED_RESOURCE_SESSION_DESC1](./ns-d3d12-d3d12_protected_resource_session_desc1.md) that describes the protected resource session.</returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12protectedresourcesession1-getdesc1">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	ProtectedResourceSessionDescription1 GetDesc1( ) ;

	
	new static Type ComType => typeof( ID3D12ProtectedResourceSession1 ) ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( ID3D12ProtectedResourceSession1 ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new ProtectedResourceSession1( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new ProtectedResourceSession1( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new ProtectedResourceSession1( (ID3D12ProtectedResourceSession1?) pComObj! ) ;
} ;



[Wrapper( typeof( ID3D12ProtectedResourceSession ) )]
internal class ProtectedResourceSession: ProtectedSession, 
										 IProtectedResourceSession,
										 IComObjectRef< ID3D12ProtectedResourceSession >,
										 IUnknownWrapper< ID3D12ProtectedResourceSession > {
	ComPtr< ID3D12ProtectedResourceSession >? _comPtr ;
	public new ComPtr< ID3D12ProtectedResourceSession >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< ID3D12ProtectedResourceSession >( ) ;
	public override ID3D12ProtectedResourceSession? ComObject => ComPointer?.Interface ;

	internal ProtectedResourceSession( ) {
		_comPtr = ComResources?.GetPointer< ID3D12ProtectedResourceSession >( ) ;
	}
	internal ProtectedResourceSession( ComPtr< ID3D12ProtectedResourceSession > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal ProtectedResourceSession( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal ProtectedResourceSession( ID3D12ProtectedResourceSession comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}

	
	public ProtectedResourceSessionDescription GetDesc( ) => ComObject!.GetDesc( ) ;
	
	public new static Type ComType => typeof( ID3D12ProtectedResourceSession ) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( ID3D12ProtectedResourceSession ).GUID
																			 .ToByteArray( ) ;
			
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
														.GetReference( data ) ) ;
		}
	}
} ;


[Wrapper( typeof( ID3D12ProtectedResourceSession1 ) )]
internal class ProtectedResourceSession1: ProtectedResourceSession, 
										  IProtectedResourceSession1 {
	ComPtr< ID3D12ProtectedResourceSession1 >? _comPtr ;
	public new ComPtr< ID3D12ProtectedResourceSession1 >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< ID3D12ProtectedResourceSession1 >( ) ;
	public override ID3D12ProtectedResourceSession1? ComObject => ComPointer?.Interface ;

	internal ProtectedResourceSession1( ) {
		_comPtr = ComResources?.GetPointer< ID3D12ProtectedResourceSession1 >( ) ;
	}
	internal ProtectedResourceSession1( ComPtr< ID3D12ProtectedResourceSession1 > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal ProtectedResourceSession1( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal ProtectedResourceSession1( ID3D12ProtectedResourceSession1 comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	
	public ProtectedResourceSessionDescription1 GetDesc1( ) => ComObject!.GetDesc1( ) ;
	
	public new static Type ComType => typeof( ID3D12ProtectedResourceSession1 ) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( ID3D12ProtectedResourceSession1 ).GUID
																			  .ToByteArray( ) ;
			
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
														.GetReference( data ) ) ;
		}
	}
} ;
