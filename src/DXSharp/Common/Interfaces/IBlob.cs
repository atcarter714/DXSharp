using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;

//! Native COM Interface Definition ::
namespace Windows.Win32.Graphics.Direct3D {
	[ComImport, Guid( "8BA5FB08-5195-40E2-AC58-0D989C3A0102" ),
	 InterfaceType( ComInterfaceType.InterfaceIsIUnknown ),]
	public interface ID3DBlob: IUnknown {
		[PreserveSig] unsafe void* GetBufferPointer( ) ;
		[PreserveSig] nuint GetBufferSize( ) ;
	} ;
}

//! DXSharp Interface Definition ::
namespace DXSharp {
	/// <summary>
	/// This interface is used by Windows and Direct3D
	/// to return data of arbitrary shape and length.
	/// </summary>
	[Wrapper( typeof( ID3DBlob ) )]
	public interface IBlob: IComIID, IDisposable, IAsyncDisposable {
		public ulong Size64 => GetBufferSize( ) ;
		public uint Size => (uint)GetBufferSize( ) ;
		public unsafe nint Pointer => (nint)GetBufferPointer( ) ;
		
		/// <summary>Retrieves a pointer to the blob's data.</summary>
		/// <returns>The address of the blob data.</returns>
		unsafe void* GetBufferPointer( ) ;

		/// <summary>Retrieves the size, in bytes, of the blob's data.</summary>
		/// <returns>64-bit unsigned integer specifying the total size of the blob's data.</returns>
		nuint GetBufferSize( ) ;
		
		static Type ComType => typeof( ID3DBlob ) ;

		static ref readonly Guid IComIID.Guid {
			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			get {
				ReadOnlySpan< byte > data = typeof(ID3DBlob).GUID
															.ToByteArray( ) ;
				return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
														.GetReference(data) ) ;
			}
		}
	} ;
}

