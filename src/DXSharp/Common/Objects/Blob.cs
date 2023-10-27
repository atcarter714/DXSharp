using Windows.Win32.Graphics.Direct3D ;
using DXSharp.Windows.COM ;

namespace DXSharp ;

public class Blob: DisposableObject, IBlob {
	public ComPtr? ComPtrBase => ComPointer ;
	public ComPtr< ID3DBlob >? ComPointer { get ; protected set ; }
	
	public ID3DBlob? COMObject => ComPointer?.Interface ;
	public int RefCount => (int)( ComPointer?.RefCount ?? 0 ) ;
	public nint PointerToIUnknown => ComPointer?.BaseAddress ?? 0x0000 ;
	
	public Blob( ) { }
	public Blob( nint address ) => ComPointer = new( address ) ;
	public Blob( ID3DBlob blob ) => ComPointer = new( blob ) ;
	public Blob( ComPtr< ID3DBlob > blobPtr ) => ComPointer = blobPtr 
				?? throw new ArgumentNullException( nameof(blobPtr) ) ;
	
	//! IDisposable:
	public override bool Disposed => ComPointer?.Disposed ?? true ;
	protected override async ValueTask DisposeUnmanaged( ) =>
		await Task.Run( ( ) => ComPointer?.Dispose() ) ;
} ;