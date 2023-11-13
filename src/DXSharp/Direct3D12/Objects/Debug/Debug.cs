#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12.Debug ;


[Wrapper(typeof(ID3D12Debug))]
internal class Debug: DisposableObject, 
					  IDebug,
					  IComObjectRef< ID3D12Debug >,
					  IUnknownWrapper< ID3D12Debug > {
	protected COMResource? ComResources { get ; set ; }
	protected void _initOrAdd<T>( ComPtr<T> ptr ) where T: IUnknown {
		if( ComResources is null ) ComResources = new( ptr ) ;
		else ComResources.AddPointer<T>( ptr ) ;
	}
	
	ComPtr< ID3D12Debug >? _comPointer ;
	ComPtr? IUnknownWrapper.ComPtrBase => ComPointer ;
	public virtual ID3D12Debug? ComObject => _comPointer?.Interface ;
	public virtual ComPtr< ID3D12Debug >? ComPointer => 
		_comPointer ??= ComResources?.GetPointer< ID3D12Debug >( ) ;
	
	public static Type ComType => typeof(ID3D12Debug) ;
	

	internal Debug( ) {
		_comPointer = ComResources?.GetPointer< ID3D12Debug >( ) ;
	}
	internal Debug( nint ptr ) {
		_comPointer = new( ptr ) ;
		_initOrAdd( _comPointer ) ;
	}
	internal Debug( ID3D12Debug obj ) {
		_comPointer = new( obj ) ;
		_initOrAdd( _comPointer ) ;
	}
	internal Debug( ComPtr< ID3D12Debug >? ptr ) {
		ArgumentNullException.ThrowIfNull( ptr ) ;
		_comPointer = ptr ;
		_initOrAdd( _comPointer ) ;
	}
	
	
	protected override async ValueTask DisposeUnmanaged( ) {
		if( _comPointer is null || _comPointer.Disposed ) return ;
		await _comPointer!.DisposeAsync( ) ;
		return;
	}
	
	public override async ValueTask DisposeAsync( ) {
		await base.DisposeAsync( ) ;
		await Task.Run( Dispose ) ;
	}
	
	public virtual HResult EnableDebugLayer( ) => ComObject!.EnableDebugLayer( ) ;


	public static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Debug).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}

} ;


[Wrapper(typeof(ID3D12Debug1))]
internal class Debug1: DisposableObject,
					   IDebug1,
					   IComObjectRef< ID3D12Debug1 >,
					   IUnknownWrapper< ID3D12Debug1 > {
	protected COMResource? ComResources { get ; set ; }
	protected void _initOrAdd<T>( ComPtr<T> ptr ) where T: IUnknown {
		if( ComResources is null ) ComResources = new( ptr ) ;
		else ComResources.AddPointer<T>( ptr ) ;
	}

	
	ComPtr< ID3D12Debug1 >? _comPointer ;
	ComPtr? IUnknownWrapper.ComPtrBase => ComPointer ;
	public ComPtr< ID3D12Debug1 >? ComPointer => _comPointer ??= ComResources?.GetPointer< ID3D12Debug1 >( ) ;
	public ID3D12Debug1? ComObject => (ID3D12Debug1?)ComPointer?.InterfaceObjectRef ;
	

	internal Debug1( ) {
		_comPointer = ComResources?.GetPointer< ID3D12Debug1 >( ) ;
	}
	internal Debug1( nint ptr ) {
		_comPointer = new( ptr ) ;
		_initOrAdd( _comPointer ) ;
	}
	internal Debug1( ID3D12Debug1 obj ) {
		_comPointer = new( obj ) ;
		_initOrAdd( _comPointer ) ;
	}
	internal Debug1( ComPtr< ID3D12Debug1 >? ptr ) {
		ArgumentNullException.ThrowIfNull( ptr ) ;
		_comPointer = ptr ;
		_initOrAdd( _comPointer ) ;
	}
	
	protected override async ValueTask DisposeUnmanaged( ) {
		if( ComPointer is null || ComPointer.Disposed ) return ;
		await ComPointer!.DisposeAsync( ) ;
	}
	
	public override async ValueTask DisposeAsync( ) {
		await DisposeUnmanaged( ) ;
		await base.DisposeAsync( ) ;
	}
	
	public void EnableDebugLayer( ) => ComObject!.EnableDebugLayer( ) ;
	public void SetEnableGPUBasedValidation( bool Enable ) => ComObject!.SetEnableGPUBasedValidation( Enable ) ;
	public void SetEnableSynchronizedCommandQueueValidation( bool Enable ) => ComObject!.SetEnableSynchronizedCommandQueueValidation( Enable ) ;

	public static Type ComType => typeof(ID3D12Debug1) ;
	
	public static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Debug1).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
} ;


[Wrapper(typeof(ID3D12Debug2))]
internal class Debug2: DisposableObject,
					   IDebug2,
					   IComObjectRef< ID3D12Debug2 >,
					   IUnknownWrapper< ID3D12Debug2 > {
	protected COMResource? ComResources { get ; set ; }
	protected void _initOrAdd<T>( ComPtr<T> ptr ) where T: IUnknown {
		if( ComResources is null ) ComResources = new( ptr ) ;
		else ComResources.AddPointer<T>( ptr ) ;
	}

	
	ComPtr< ID3D12Debug2 >? _comPointer ;
	ComPtr? IUnknownWrapper.ComPtrBase => ComPointer ;
	public ComPtr< ID3D12Debug2 >? ComPointer => _comPointer ??= ComResources?.GetPointer< ID3D12Debug2 >( ) ;
	public ID3D12Debug2? ComObject => (ID3D12Debug2?)ComPointer?.InterfaceObjectRef ;
	
	internal Debug2( ) {
		_comPointer = ComResources?.GetPointer< ID3D12Debug2 >( ) ;
	}
	internal Debug2( nint ptr ) {
		_comPointer = new( ptr ) ;
		_initOrAdd( _comPointer ) ;
	}
	internal Debug2( ID3D12Debug2 obj ) {
		_comPointer = new( obj ) ;
		_initOrAdd( _comPointer ) ;
	}
	internal Debug2( ComPtr< ID3D12Debug2 >? ptr ) {
		ArgumentNullException.ThrowIfNull( ptr ) ;
		_comPointer = ptr ;
		_initOrAdd( _comPointer ) ;
	}
	
	
	protected override async ValueTask DisposeUnmanaged( ) {
		if( ComPointer is null || ComPointer.Disposed ) return ;
		await ComPointer!.DisposeAsync( ) ;
	}

	public void SetGPUBasedValidationFlags( GPUBasedValidationFlags Flags ) => ComObject!.SetGPUBasedValidationFlags( Flags ) ;
	
	
	
	public static Type ComType => typeof(ID3D12Debug2) ;
	
	public static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Debug2).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
} ;


[Wrapper(typeof(ID3D12Debug3))]
internal class Debug3: Debug,
					   IComObjectRef< ID3D12Debug3 >,
					   IUnknownWrapper< ID3D12Debug3 >,
					   IDebug3 {
	ComPtr< ID3D12Debug3 >? _comPointer3 ;
	public new virtual ComPtr< ID3D12Debug3 >? ComPointer => 
		_comPointer3 ??= ComResources?.GetPointer< ID3D12Debug3 >( ) ;
	
	public override ID3D12Debug3? ComObject => _comPointer3?.Interface ;
	public ComPtr? ComPtrBase => ComPointer ;
	
	internal Debug3( ) {
		_comPointer3 = ComResources?.GetPointer< ID3D12Debug3 >( ) ;
	}
	internal Debug3( nint ptr ) {
		_comPointer3 = new( ptr ) ;
		_initOrAdd( _comPointer3 ) ;
	}
	internal Debug3( ID3D12Debug3 obj ) {
		_comPointer3 = new( obj ) ;
		_initOrAdd( _comPointer3 ) ;
	}
	internal Debug3( ComPtr< ID3D12Debug3 >? ptr ) {
		ArgumentNullException.ThrowIfNull( ptr ) ;
		_comPointer3 = ptr ;
		_initOrAdd( _comPointer3 ) ;
	}
	~Debug3( ) => Dispose( false ) ;
	
	protected override async ValueTask DisposeUnmanaged( ) {
		await base.DisposeUnmanaged( ) ;
		if( _comPointer3 is null || _comPointer3.Disposed ) return ;
		_comPointer3?.DisposeAsync( ) ;
	}

	public override async ValueTask DisposeAsync( ) {
		await base.DisposeAsync( ) ;
		await Task.Run( Dispose ) ;
	}

	//public override void EnableDebugLayer( ) => COMObject!.EnableDebugLayer( ) ;
	public void SetEnableGPUBasedValidation( bool Enable ) => ComObject!.SetEnableGPUBasedValidation( Enable ) ;
	public void SetGPUBasedValidationFlags( GPUBasedValidationFlags Flags ) => ComObject!.SetGPUBasedValidationFlags( Flags ) ;
	public void SetEnableSynchronizedCommandQueueValidation( bool Enable )  => ComObject!.SetEnableSynchronizedCommandQueueValidation( Enable ) ;

	
	public new static Type ComType => typeof(ID3D12Debug3) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Debug3).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
} ;


[Wrapper(typeof(ID3D12Debug4))]
internal class Debug4: Debug3,
					   IDebug4,
					   IComObjectRef< ID3D12Debug4 >,
					   IUnknownWrapper< ID3D12Debug4 > {
	protected ComPtr< ID3D12Debug4 >? _comPointer4 ;
	public new virtual ComPtr< ID3D12Debug4 >? ComPointer => 
		_comPointer4 ??= ComResources?.GetPointer< ID3D12Debug4 >( ) ;
	
	public override ID3D12Debug4? ComObject => _comPointer4?.Interface ;
	
	
	internal Debug4( ) {
		_comPointer4 = ComResources?.GetPointer< ID3D12Debug4 >( ) ;
	}
	internal Debug4( nint ptr ) {
		_comPointer4 = new( ptr ) ;
		_initOrAdd( _comPointer4 ) ;
	}
	internal Debug4( ID3D12Debug4 obj ) {
		_comPointer4 = new( obj ) ;
		_initOrAdd( _comPointer4 ) ;
	}
	internal Debug4( ComPtr< ID3D12Debug4 >? ptr ) {
		ArgumentNullException.ThrowIfNull( ptr ) ;
		_comPointer4 = ptr ;
		_initOrAdd( _comPointer4 ) ;
	}
	~Debug4( ) => Dispose( false ) ;
	
	
	protected override async ValueTask DisposeUnmanaged( ) {
		await base.DisposeUnmanaged( ) ;
		if( _comPointer4 is null || _comPointer4.Disposed ) return ;
		await _comPointer4!.DisposeAsync( ) ;
	}
	
	public override async ValueTask DisposeAsync( ) {
		await base.DisposeAsync( ) ;
		await Task.Run( Dispose ) ;
	}
	
	public HResult DisableDebugLayer( ) => ComObject!.DisableDebugLayer( ) ;

	
	public new static Type ComType => typeof(ID3D12Debug4) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Debug4).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
} ;


[Wrapper(typeof(ID3D12Debug5))]
internal class Debug5: Debug4,
					   IDebug5,
					   IComObjectRef< ID3D12Debug5 >,
					   IUnknownWrapper< ID3D12Debug5 > {
	ComPtr< ID3D12Debug5 >? _comPointer5 ;
	public new virtual ComPtr< ID3D12Debug5 >? ComPointer => 
		_comPointer5 ??= ComResources?.GetPointer< ID3D12Debug5 >( ) ;
	
	public override ID3D12Debug5? ComObject => _comPointer5?.Interface ;
	
	
	
	internal Debug5( ) {
		_comPointer5 = ComResources?.GetPointer< ID3D12Debug5 >( ) ;
	}
	internal Debug5( nint ptr ) {
		_comPointer5 = new( ptr ) ;
		_initOrAdd( _comPointer5 ) ;
	}
	internal Debug5( ID3D12Debug5 obj ) {
		_comPointer5 = new( obj ) ;
		_initOrAdd( _comPointer5 ) ;
	}
	internal Debug5( ComPtr< ID3D12Debug5 >? ptr ) {
		ArgumentNullException.ThrowIfNull( ptr ) ;
		_comPointer5 = ptr ;
		_initOrAdd( _comPointer5 ) ;
	}
	~Debug5( ) => Dispose( false ) ;
	
	protected override async ValueTask DisposeUnmanaged( ) {
		await base.DisposeUnmanaged( ) ;
		if( _comPointer5 is null || _comPointer5.Disposed ) return ;
		await _comPointer5.DisposeAsync( ) ;
	}

	public override async ValueTask DisposeAsync( ) {
		await base.DisposeAsync( ) ;
		await Task.Run( Dispose ) ;
	}

	public virtual void SetEnableAutoName( bool enable ) => 
		ComObject!.SetEnableAutoName( enable ) ;

	
	public new static Type ComType => typeof(ID3D12Debug5) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Debug5).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
} ;


[Wrapper(typeof(ID3D12Debug6))]
internal class Debug6: Debug5,
					   IDebug6,
					   IComObjectRef< ID3D12Debug6 >,
					   IUnknownWrapper< ID3D12Debug6 > {
	ComPtr< ID3D12Debug6 >? _comPointer6 ;
	public new virtual ComPtr< ID3D12Debug6 >? ComPointer => 
		_comPointer6 ??= ComResources?.GetPointer< ID3D12Debug6 >( ) ;
	public override ID3D12Debug6? ComObject => _comPointer6?.Interface ;
	
	
	internal Debug6( ) {
		_comPointer6 = ComResources?.GetPointer< ID3D12Debug6 >( ) ;
	}
	internal Debug6( nint ptr ) {
		_comPointer6 = new( ptr ) ;
		_initOrAdd( _comPointer6 ) ;
	}
	internal Debug6( ID3D12Debug6 obj ) {
		_comPointer6 = new( obj ) ;
		_initOrAdd( _comPointer6 ) ;
	}
	internal Debug6( ComPtr< ID3D12Debug6 >? ptr ) {
		ArgumentNullException.ThrowIfNull( ptr ) ;
		_comPointer6 = ptr ;
		_initOrAdd( _comPointer6 ) ;
	}
	
	~Debug6( ) => Dispose( false ) ;
	
	protected override async ValueTask DisposeUnmanaged( ) {
		await base.DisposeUnmanaged( ) ;
		if( _comPointer6 is null || _comPointer6.Disposed ) return ;
		if( ComResources?.IsAlive ?? false ) ComResources.Dispose( ) ;
	}

	public override async ValueTask DisposeAsync( ) {
		await base.DisposeAsync( ) ;
		await Task.Run( Dispose ) ;
	}
	
	public void SetForceLegacyBarrierValidation( bool enable ) => 
		ComObject!.SetForceLegacyBarrierValidation( enable ) ;

	
	public new static Type ComType => typeof(ID3D12Debug6) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Debug6).GUID .ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
} ;

