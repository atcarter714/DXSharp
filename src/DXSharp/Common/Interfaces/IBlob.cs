using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Direct3D ;
using DXSharp.Windows.COM ;

namespace DXSharp {
	[Wrapper( typeof( ID3DBlob ) )]
	public interface IBlob: IUnknownWrapper< ID3DBlob > {
		/// <summary>Retrieves a pointer to the blob's data.</summary>
		/// <returns>The address of the blob data.</returns>
		unsafe nint GetBufferPointer( ) => ( nint )ComObject!.GetBufferPointer( ) ;

		/// <summary>Retrieves the size, in bytes, of the blob's data.</summary>
		/// <returns>64-bit unsigned integer specifying the total size of the blob's data.</returns>
		ulong GetBufferSize( ) => ComObject!.GetBufferSize( ) ;
	} ;
}


//! Native COM Interface Definition ::
namespace Windows.Win32.Graphics.Direct3D {
	[ComImport, Guid( "8BA5FB08-5195-40E2-AC58-0D989C3A0102" ),
	 InterfaceType( ComInterfaceType.InterfaceIsIUnknown ),]
	public interface ID3DBlob: IUnknown {
		[PreserveSig] unsafe void* GetBufferPointer( ) ;
		[PreserveSig] nuint GetBufferSize( ) ;
	} ;
}