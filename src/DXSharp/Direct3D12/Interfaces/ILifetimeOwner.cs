#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[ProxyFor(typeof(ID3D12LifetimeOwner))]
public interface ILifetimeOwner: IComIID {
	
	static Type ComType => typeof(ID3D12LifetimeOwner) ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12LifetimeOwner).GUID
														  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}


	void LifetimeStateUpdated( LifetimeState NewState ) ;
} ;


[Wrapper(typeof(ID3D12LifetimeOwner))]
internal class LifetimeOwner: DisposableObject, 
							  ILifetimeOwner,
							  IUnknownWrapper< ID3D12LifetimeOwner >, 
							  IComObjectRef< ID3D12LifetimeOwner > {
	// -----------------------------------------------------------------------------------------------
	
	protected ComObject? ComResources { get ; set ; }
	public int RefCount => ( ComPointer?.RefCount ?? 0 ) ;
	
	ComPtr< ID3D12LifetimeOwner >? _comPtr ;
	public ComPtr< ID3D12LifetimeOwner >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer<ID3D12LifetimeOwner>( ) ;
	ComPtr? IUnknownWrapper< ID3D12LifetimeOwner >.ComPointer => ComPointer ;

	public ComPtr? ComPtrBase => ComPointer ;
	
	// -----------------------------------------------------------------------------------------------
	
	public ID3D12LifetimeOwner? COMObject => ComPointer?.Interface ;
	
	internal LifetimeOwner( ) {
	 _comPtr = ComResources?.GetPointer< ID3D12LifetimeOwner >( ) ;
	}
	
	internal LifetimeOwner( ComPtr< ID3D12LifetimeOwner > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	
	internal LifetimeOwner( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	
	internal LifetimeOwner( ID3D12LifetimeOwner comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}

	void _initOrAdd( ComPtr< ID3D12LifetimeOwner > comPtr ) {
		ArgumentNullException.ThrowIfNull( comPtr, nameof(comPtr) ) ;
		
		if ( ComResources is null ) {
			ComResources ??= new( comPtr ) ;
		}
		else {
			ComResources.AddPointer( comPtr ) ;
		}
	}

	// -----------------------------------------------------------------------------------------------
	
	protected override ValueTask DisposeUnmanaged( ) {
		ComResources?.Dispose( ) ;
		if ( !_comPtr?.Disposed ?? false ) {
			_comPtr?.Dispose( ) ;
			
		}
		
		return ValueTask.CompletedTask ;
	}
	
	// -----------------------------------------------------------------------------------------------
	
	public void LifetimeStateUpdated( LifetimeState NewState ) => 
		COMObject!.LifetimeStateUpdated( NewState ) ;
	
	// -----------------------------------------------------------------------------------------------
	
	public static Type ComType => typeof(ID3D12LifetimeOwner) ;
	
	public static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12LifetimeOwner).GUID
														  .ToByteArray( ) ;
			
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	 // ===============================================================================================
} ;