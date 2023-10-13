using Windows.Win32.Graphics.Direct3D ;
using DXSharp.Windows.COM ;

namespace DXSharp ;

public class Blob: DisposableObject, IBlob {
	public override bool Disposed => ComPointer?.Disposed ?? true ;
	protected override void DisposeUnmanaged( ) => ComPointer?.Dispose( ) ;

	public ID3DBlob? COMObject => ComPointer?.Interface ;
	public int RefCount => (int)( ComPointer?.RefCount ?? 0 ) ;
	public ComPtr< ID3DBlob >? ComPointer { get ; protected set ; }
	public nint PointerToIUnknown => ComPointer?.BaseAddress ?? 0x0000 ;
	
	public Blob( ) { }
	public Blob( nint address ) => ComPointer = new( address ) ;
	public Blob( ID3DBlob blob ) => ComPointer = new( blob ) ;
	public Blob( ComPtr< ID3DBlob > blobPtr ) => ComPointer = blobPtr 
				?? throw new ArgumentNullException( nameof(blobPtr) ) ;
} ;