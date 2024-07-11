#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

#endregion
namespace DXSharp.Direct3D12 ;


[ProxyFor( typeof( ID3D12Tools ) )]
public interface ITools: IInstantiable, IComIID,
						 IDisposable, IAsyncDisposable {
	// ---------------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12Tools) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )] get {
			ReadOnlySpan< byte > data = typeof(ID3D12Tools).GUID
														   .ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}

	static IInstantiable IInstantiable.Instantiate( ) => new Tools( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new Tools( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< Com >( Com obj ) => new Tools( ( obj as ID3D12Tools )! ) ;
	// ==================================================================================
} ;



[Wrapper(typeof(ID3D12Tools))]
internal class Tools: DisposableObject,
					 ITools,
					 IComObjectRef< ID3D12Tools >,
					 IUnknownWrapper< ID3D12Tools > {
	
	//! ---------------------------------------------------------------------------------
	/// <summary>The COM object management instance.</summary>
	protected COMResource? ComResources { get ; set ; }
	protected internal void _initOrAdd< T >( ComPtr< T > ptr ) where T: IUnknown {
		if( ComResources is null ) ComResources = new( ptr ) ;
		else ComResources.AddPointer<T>( ptr ) ;
	}

	// ------------------------------------------------------------------------------------------
	ComPtr< ID3D12Tools >? _comPtr ;
	public virtual ID3D12Tools? ComObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12Tools >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer<ID3D12Tools>( ) ;
	// ------------------------------------------------------------------------------------------
	
	internal Tools( ) {
		_comPtr = ComResources?.GetPointer< ID3D12Tools >( ) ;
	}
	internal Tools( ComPtr< ID3D12Tools > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal Tools( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal Tools( ID3D12Tools comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	// ------------------------------------------------------------------------------------------

	protected override async ValueTask DisposeUnmanaged( ) {
		if ( _comPtr is not null )
			await _comPtr.DisposeAsync( ) ;
		_comPtr = null ;
	}

	// ------------------------------------------------------------------------------------------
	public static Type ComType => typeof(ID3D12Tools) ;
	public static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Tools).GUID
														  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	// ==========================================================================================
} ;
