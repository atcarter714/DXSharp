#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi ;

using DXSharp.Windows ;
using DXSharp.Windows.COM ;
using static DXSharp.InteropUtils ;
#endregion
namespace DXSharp.Objects ;


[Wrapper(typeof(IUnknown))]
[Wrapper(typeof(IDXGIObject))]
[Wrapper(typeof(ID3D12Object))]
internal abstract class DXComObject: DisposableObject,
									 IDXCOMObject {
	//! ---------------------------------------------------------------------------------
	/// <summary>The COM object management instance.</summary>
	protected COMResource? ComResources { get ; set ; }
	protected internal void _initOrAdd< T >( ComPtr< T > ptr ) where T: IUnknown {
		if( ComResources is null ) ComResources = new( ptr ) ;
		else ComResources.AddPointer<T>( ptr ) ;
	}
	
	ComPtr? _comPtr ;
	
	public ComPtr? ComPtrBase => ComPointer ;
	public virtual ComPtr? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IUnknown >( )! ;
	
	
	public virtual IUnknown? ComObject =>
		(IUnknown)ComPointer?.InterfaceObjectRef! ;
	
	//! ---------------------------------------------------------------------------------
	
	//! IDisposable:
	protected override async ValueTask DisposeUnmanaged( ) {
		if( _comPtr is not null )
			await _comPtr.DisposeAsync( ) ;
	}
	protected override void Dispose( bool disposing ) {
		base.Dispose( disposing ) ;
		if( disposing ) 
			DisposeManaged( ) ;
		DisposeUnmanaged( ) ;
	}
	public override async ValueTask DisposeAsync( ) {
		await base.DisposeAsync( ) ;
		await Task.Run( Dispose ) ;
	}
	
	// ----------------------------------------------------------------------------------
	
	public HResult GetPrivateData( in Guid name, ref uint pDataSize, nint pData = 0x0000 ) {
		// Obtain interface references:
		var _name = name ;
		
		// Get the interface object:
		var comObj = this.ComPtrBase?.InterfaceObjectRef
#if DEBUG || DEV_BUILD || DEBUG_COM
					 ?? throw new ObjectDisposedException( nameof( this.ComPtrBase ) )
#endif
		 					 ;
		 
		// Get the data: ---------------------------------------------------------------
		HResult hr = default ;
		//! Is it a D3D12 object?
		if ( comObj is ID3D12Object d3d12Object ) {
			unsafe { hr = d3d12Object.GetPrivateData( &_name, ref pDataSize, (void*)pData ) ; }
		}
		//! Is it a DXGI object?
		else if ( comObj is IDXGIObject dxgiObject ) {
			unsafe { hr = dxgiObject.GetPrivateData( &_name, ref pDataSize, (void*)pData ) ; }
		}
		// ----------------------------------------------------------------------------------
		
		hr.SetAsLastErrorForThread( ) ;
		return hr ;
	}
	
	public unsafe HResult SetPrivateData( in Guid name, uint DataSize, nint pData ) {
		// Obtain interface references:
		HResult hr = default ;
		var _name = name ;
		
		// Get the interface object:
		var comObj = this.ComPtrBase?.InterfaceObjectRef
#if DEBUG || DEV_BUILD || DEBUG_COM
					 ?? throw new ObjectDisposedException( nameof( this.ComPtrBase ) )
#endif
		 					 ;
		
		// Set the data: ---------------------------------------------------------------
		
		//! Is it a D3D12 object?
		if ( comObj is ID3D12Object d3d12Object ) {
			unsafe { hr = d3d12Object.SetPrivateData( &_name, DataSize, (void*)pData ) ; }
		}
		//! Is it a DXGI object?
		else if ( comObj is IDXGIObject dxgiObject ) {
			unsafe { hr = dxgiObject.SetPrivateData( &_name, DataSize, (void*)pData ) ; }
		}
		hr.SetAsLastErrorForThread( ) ;
		return hr ;
	}
	
	public HResult SetPrivateDataInterface< T >( in Guid name, in T? pUnknown ) where T: IDXCOMObject {
		// Obtain interface references:
		HResult hr = default ;
		var _name = name ;
		
		// Get the interface object:
		var comObj = this.ComPtrBase?.InterfaceObjectRef
#if DEBUG || DEV_BUILD || DEBUG_COM
					 ?? throw new ObjectDisposedException( nameof( this.ComPtrBase ) )
#endif 
					 ;
		
		// Get the ref to the IUnknown interface:
		var dataObj = ( (IComObjectRef< IUnknown >?)pUnknown )?.ComObject
#if DEBUG || DEV_BUILD || DEBUG_COM
					  ?? throw new NullReferenceException( nameof( pUnknown ) )
#endif
					  ;
		
		// Set the data: ---------------------------------------------------------------
		
		//! Is it a D3D12 object?
		if ( comObj is ID3D12Object d3d12Object ) {
			unsafe { hr = d3d12Object.SetPrivateDataInterface( &_name, ( (IUnknown)dataObj! ?? null ) ) ; }
		}
		//! Is it a DXGI object?
		else if ( comObj is IDXGIObject dxgiObject ) {
			unsafe { hr = dxgiObject.SetPrivateDataInterface( &_name, ( (IUnknown)dataObj! ?? null ) ) ; }
		}
		hr.SetAsLastErrorForThread( ) ;
		return hr ;
	}
	
	//! ---------------------------------------------------------------------------------
	public static Type ComType => typeof(IUnknown) ;
	public static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IUnknown).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	// =================================================================================
} ;

