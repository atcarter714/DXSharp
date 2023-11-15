#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

#endregion
namespace DXSharp.Direct3D12 ;


[ProxyFor( typeof( ID3D12StateObjectProperties ) )]
public interface IStateObjectProperties: IInstantiable, IComIID, 
										 IDisposable, IAsyncDisposable {
	
	// ---------------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12StateObjectProperties) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )] get {
			ReadOnlySpan< byte > data = typeof(ID3D12StateObjectProperties).GUID
																		   .ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}

	static IInstantiable IInstantiable.Instantiate( ) => new StateObjectProperties( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new StateObjectProperties( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< Com >( Com obj ) => new StateObjectProperties( ( obj as ID3D12StateObjectProperties )! ) ;
	// ==================================================================================
} ;




[Wrapper(typeof(ID3D12StateObjectProperties))]
internal class StateObjectProperties: DisposableObject,
					 IStateObjectProperties,
					 IComObjectRef< ID3D12StateObjectProperties >,
					 IUnknownWrapper< ID3D12StateObjectProperties > {
	
	//! ---------------------------------------------------------------------------------
	/// <summary>The COM object management instance.</summary>
	protected COMResource? ComResources { get ; set ; }
	protected internal void _initOrAdd< T >( ComPtr< T > ptr ) where T: IUnknown {
		if( ComResources is null ) ComResources = new( ptr ) ;
		else ComResources.AddPointer<T>( ptr ) ;
	}

	// ------------------------------------------------------------------------------------------
	ComPtr< ID3D12StateObjectProperties >? _comPtr ;
	public virtual ID3D12StateObjectProperties? ComObject => ComPointer?.Interface ;
	public new ComPtr< ID3D12StateObjectProperties >? ComPointer => 
		_comPtr ??= ComResources?.GetPointer<ID3D12StateObjectProperties>( ) ;
	// ------------------------------------------------------------------------------------------
	
	internal StateObjectProperties( ) {
		_comPtr = ComResources?.GetPointer< ID3D12StateObjectProperties >( ) ;
	}
	internal StateObjectProperties( ComPtr< ID3D12StateObjectProperties > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	internal StateObjectProperties( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal StateObjectProperties( ID3D12StateObjectProperties comObject ) {
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
	public static Type ComType => typeof(ID3D12StateObjectProperties) ;
	public static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12StateObjectProperties).GUID
														  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	// ==========================================================================================
} ;
