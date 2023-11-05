#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.Security ;
using DXSharp.Windows.COM ;
using DXSharp.Windows.Win32 ;
#endregion
namespace DXSharp.DXGI ;


[Wrapper(typeof(IDXGIResource))]
internal class Resource: DeviceSubObject, 
						 IResource,
						 IComObjectRef< IDXGIResource >,
						 IUnknownWrapper< IDXGIResource > {
	// ----------------------------------------------------------------------------------------------
	ComPtr< IDXGIResource >? _comPtr ;
	public new ComPtr< IDXGIResource >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer<IDXGIResource>(  ) ;
	public new IDXGIResource? COMObject => ComPointer?.Interface ;
	IDXGIResource _dxgiInterface => COMObject ??
		 throw ( ComPointer is not null && ComPointer.Disposed
					 ? new ObjectDisposedException( nameof(OutputDuplication) )
					 : new NullReferenceException( $"{nameof(Resource)} :: " +
									$"internal {nameof(IDXGIResource)} null reference." ) ) ;
	// ----------------------------------------------------------------------------------------------

	internal Resource( ) {
		_comPtr = ComResources?.GetPointer<IDXGIResource>(  ) ;
		if ( _comPtr is not null ) _initOrAdd( _comPtr ) ;
	}
	internal Resource( nint pComObj ) {
		_comPtr = new( pComObj ) ;
		if ( _comPtr is not null ) _initOrAdd( _comPtr ) ;
	}
	internal Resource( IDXGIResource pComObj ) {
		_comPtr = new( pComObj ) ;
		if ( _comPtr is not null ) _initOrAdd( _comPtr ) ;
	}
	internal Resource( ComPtr< IDXGIResource > ptr ) {
		_comPtr = ptr ;
		if ( _comPtr is not null ) _initOrAdd( _comPtr ) ;
	}
	
	// ----------------------------------------------------------------------------------------------
	
	public void GetEvictionPriority( out ResourcePriority pEvictionPriority ) {
		_dxgiInterface.GetEvictionPriority( out uint priority ) ;
		pEvictionPriority = (ResourcePriority) priority ;
	}

	public void SetEvictionPriority( ResourcePriority EvictionPriority ) =>
		_dxgiInterface.SetEvictionPriority( (uint)EvictionPriority ) ;

	public void GetUsage( out Usage pUsage ) {
		unsafe {
			pUsage = default ;
			DXGI_USAGE usage = default ;
			_dxgiInterface.GetUsage( &usage ) ;
			pUsage = (Usage) usage ;
		}
	}

	public void GetSharedHandle( out Win32Handle pSharedHandle ) {
		pSharedHandle = default ;
		unsafe {
			HANDLE handle = default ;
			_dxgiInterface.GetSharedHandle( &handle ) ;
			pSharedHandle = new( handle ) ;
		}
	}

	// ----------------------------------------------------------------------------------------------
	public new static Type ComType => typeof( IDXGIResource ) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIResource ).GUID
															   .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	// ==============================================================================================
} ;


[Wrapper(typeof(IDXGIResource1))]
internal class Resource1: Resource, 
						  IResource1,
						  IComObjectRef< IDXGIResource1 >,
						  IUnknownWrapper< IDXGIResource1 > {
	// ----------------------------------------------------------------------------------------------
	ComPtr< IDXGIResource1 >? _comPtr ;
	public new ComPtr< IDXGIResource1 >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer<IDXGIResource1>(  ) ;
	public new IDXGIResource1? COMObject => ComPointer?.Interface ;
	IDXGIResource1 _dxgiInterface => COMObject ??
		 throw ( ComPointer is not null && ComPointer.Disposed
					 ? new ObjectDisposedException( nameof(OutputDuplication) )
					 : new NullReferenceException( $"{nameof(Resource1)} :: " +
									$"internal {nameof(IDXGIResource1)} null reference." ) ) ;
	// ----------------------------------------------------------------------------------------------

	internal Resource1( ) {
		_comPtr = ComResources?.GetPointer<IDXGIResource1>(  ) ;
		if ( _comPtr is not null ) _initOrAdd( _comPtr ) ;
	}
	internal Resource1( nint pComObj ) {
		_comPtr = new( pComObj ) ;
		if ( _comPtr is not null ) _initOrAdd( _comPtr ) ;
	}
	internal Resource1( IDXGIResource1 pComObj ) {
		_comPtr = new( pComObj ) ;
		if ( _comPtr is not null ) _initOrAdd( _comPtr ) ;
	}
	internal Resource1( ComPtr< IDXGIResource1 > ptr ) {
		_comPtr = ptr ;
		if ( _comPtr is not null ) _initOrAdd( _comPtr ) ;
	}
	
	// ----------------------------------------------------------------------------------------------
	
	public void CreateSubresourceSurface( uint index, out ISurface2 ppSurface ) {
		_dxgiInterface.CreateSubresourceSurface( index, out IDXGISurface2 surface ) ;
		ppSurface = new Surface2( surface ) ;
	}

	public void CreateSharedHandle( [Optional] in SecurityAttributes pAttributes, 
									uint dwAccess, 
									string lpName,
									in Win32Handle pHandle ) {
		unsafe {
			using PCWSTR _name = lpName ;
			fixed ( SecurityAttributes* pAttr = &pAttributes ) {
				fixed ( Win32Handle* handle = &pHandle ) {
					_dxgiInterface.CreateSharedHandle( (SECURITY_ATTRIBUTES *)pAttr, 
													   dwAccess, 
													   _name, 
													   (HANDLE *)handle ) ;
				}
			}
		}
	}

	// ----------------------------------------------------------------------------------------------
	public new static Type ComType => typeof( IDXGIResource1 ) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof( IDXGIResource1 ).GUID.ToByteArray( ) ;

			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference( data ) ) ;
		}
	}
	// ==============================================================================================
} ;

