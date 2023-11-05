#region Using Directives
using Windows.Win32.Graphics.Direct3D ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp ;


internal class Blob: DisposableObject,
					 IBlob,
					 IComObjectRef< ID3DBlob >, 
					 IUnknownWrapper< ID3DBlob > {
	protected COMResource? ComResources { get ; set ; }
	ComPtr< ID3DBlob >? _comPtr ;
	
	public ComPtr? ComPtrBase => ComPointer ;
	public virtual ComPtr? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< ID3DBlob >( ) ;
	
	public virtual ID3DBlob? ComObject => ( (ComPtr<ID3DBlob>)ComPointer! )?.Interface ;
	public int RefCount => (int)( ComPointer?.RefCount ?? 0 ) ;
	public nint PointerToIUnknown => ComPointer?.BaseAddress ?? 0x0000 ;
	
	
	public Blob( ) {
		_comPtr = ComResources?.GetPointer< ID3DBlob >( ) ;
	}
	public Blob( ComPtr< ID3DBlob > comPtr ) {
		_comPtr = comPtr ;
		_initOrAdd( _comPtr ) ;
	}
	public Blob( nint address ) {
		_comPtr = new( address ) ;
		_initOrAdd( _comPtr ) ;
	}
	public Blob( ID3DBlob comObject ) {
		_comPtr = new( comObject ) ;
		_initOrAdd( _comPtr ) ;
	}
	void _initOrAdd( ComPtr< ID3DBlob > comPtr ) {
		ArgumentNullException.ThrowIfNull( comPtr, nameof(comPtr) ) ;
		
		if ( ComResources is null ) {
			ComResources ??= new( comPtr ) ;
		}
		else {
			ComResources.AddPointer( comPtr ) ;
		}
	}

	
	//! IDisposable:
	public override bool Disposed => ComPointer?.Disposed ?? true ;
	
	protected override async ValueTask DisposeUnmanaged( ) {
		if( ComPointer is not null && !ComPointer.Disposed ) {
			await ComPointer.DisposeAsync( ) ;
			_comPtr = null ;
		}
	}
	
	public unsafe void* GetBufferPointer( ) => ComObject!.GetBufferPointer( ) ;
	
	public nuint GetBufferSize( ) => ComObject!.GetBufferSize( ) ;
} ;