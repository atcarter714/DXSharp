#region Using Directives

using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp ;


/// <summary>
/// The <see cref="IBlob"/> interface is used by Windows and Direct3D to return data of arbitrary shape and length.
/// </summary>
[Wrapper(typeof(ID3DBlob))]
internal class Blob: DisposableObject,
					 IBlob,
					 IComObjectRef< ID3DBlob >, 
					 IUnknownWrapper< ID3DBlob > {
	protected COMResource? ComResources { get ; set ; }
	ComPtr< ID3DBlob >? _comPtr ;
	
	public ComPtr? ComPtrBase => ComPointer ;
	public virtual ComPtr< ID3DBlob >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< ID3DBlob >( ) ;
	
	public virtual ID3DBlob? ComObject => ( (ComPtr<ID3DBlob>)ComPointer! )?.Interface ;
	public int RefCount => (int)( ComPointer?.RefCount ?? 0 ) ;
	public nint PointerToIUnknown => ComPointer?.BaseAddress ?? 0x0000 ;
	
	// ----------------------------------------------------------------------------------
	
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
	~Blob( ) => Dispose( false ) ;
	
	void _initOrAdd( ComPtr< ID3DBlob > comPtr ) {
		ArgumentNullException.ThrowIfNull( comPtr, nameof(comPtr) ) ;
		
		if ( ComResources is null ) {
			ComResources ??= new( comPtr ) ;
		}
		else {
			ComResources.AddPointer( comPtr ) ;
		}
	}

	
	// ----------------------------------------------------------------------------------
	//! IDisposable:
	// ----------------------------------------------------------------------------------
	public override bool Disposed => ComPointer?.Disposed ?? true ;
	
	protected override ValueTask DisposeUnmanaged( ) {
		if( ComResources?.Disposed ?? true ) 
			return ValueTask.CompletedTask ;
		
		_comPtr = null ;
		ComResources.Dispose( ) ;
		return ValueTask.CompletedTask ;
	}
	
	
	// ----------------------------------------------------------------------------------
	// Interface Methods:
	// ----------------------------------------------------------------------------------
	
	
	public nuint GetBufferSize( ) => ComObject!.GetBufferSize( ) ;
	
	public unsafe void* GetBufferPointer( ) => ComObject!.GetBufferPointer( ) ;
	
	
	// ----------------------------------------------------------------------------------
	public static Type ComType => typeof( ID3DBlob ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3DBlob).GUID
														.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	// ==================================================================================
} ;