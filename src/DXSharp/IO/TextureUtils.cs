#region Using Directives
using System.Drawing.Imaging ;
using System.Collections.Concurrent ;
using System.Runtime.CompilerServices ;
using DXSharp.Direct3D12 ;
using DXSharp.Direct3D12.XTensions;
#endregion
namespace DXSharp.IO ;


/// <summary>Utilities for handling texture resources.</summary>
public static class TextureUtils {
	static ResourceBarrier[ ] _tempBarrierCache = new ResourceBarrier[ 1 ] ;
	static ConcurrentDictionary< UID32, ICommandList > _tempCommandLists = new( ) ;

	
	/// <summary>
	/// Loads a Bitmap from the specified path.
	/// </summary>
	/// <param name="path">The path of the file.</param>
	/// <returns></returns>
	/// <exception cref="IOException">Throw if file cannot be loaded successfully.</exception>
	public static Bitmap? LoadBitmap( string? path ) {
		ArgumentException.ThrowIfNullOrEmpty( path, nameof( path ) ) ;

		var image = Image.FromFile( path )
					?? throw new IOException( $"{nameof( TextureUtils )} :: " +
											  $"Cannot load file at path: \"{path}\"!" )
			;

		var bitmap = image as Bitmap ;
		return bitmap ;
	}


	/// <summary>
	/// Creates a D3D12 texture Resource for a Bitmap.
	/// </summary>
	/// <param name="device">Graphics device interface</param>
	/// <param name="bitmap">Bitmap to create resource from</param>
	/// <param name="commandList">An (optional) existing command list to use (one is created if argument is null).</param>
	/// <param name="allocator">An (optional) comand allocator to use (a temporary one is created if argument is null).</param>
	/// <param name="flags">Special <see cref="ResourceFlags"/> value to set for the resource.</param>
	/// <returns></returns>
	public static IResource CreateTexture( IDevice device,
										   in Bitmap bitmap,
										   IGraphicsCommandList? commandList  = null,
										   ICommandAllocator?    allocator    = null,
										   ICommandQueue?        commandQueue = null,
										   ResourceFlags         flags        = ResourceFlags.None ) {
		ResourceDescription desc = new ResourceDescription
		{
			Dimension        = ResourceDimension.Texture2D,
			Format           = DXGI.Format.R8G8B8A8_UNORM, // Match this to the format you've loaded/converted
			Layout           = TextureLayout.Unknown,
			Width            = (uint)bitmap.Width,
			Height           = (uint)bitmap.Height,
			Flags            = flags,
			MipLevels        = 1,
			DepthOrArraySize = 1
		} ;

		device.CreateCommittedResource( new( HeapType.Default ), HeapFlags.None,
										desc, ResourceStates.CopyDest, null,
										IResource.InterfaceGUID,
										out var resrc ) ;

		device.CreateCommittedResource( new( HeapType.Upload ), HeapFlags.None,
										desc, ResourceStates.GenericRead, null,
										IResource.InterfaceGUID,
										out var textureUploadHeap ) ;

		if ( allocator is null )
			device.CreateCommandAllocator( CommandListType.Direct,
										   CommandAllocator.InterfaceGUID,
										   out allocator ) ;

		if ( commandList is null ) {
			device.CreateCommandList( 0, CommandListType.Direct, allocator, null, IGraphicsCommandList.InterfaceGUID,
									  out var list ) ;
			commandList = (IGraphicsCommandList)list ;
		}

		if ( commandQueue is null ) {
			device.CreateCommandQueue( new( ), ICommandQueue.InterfaceGUID, out commandQueue ) ;
		}

		// Assuming you have a method to update the resource from a byte array.
		_tempBarrierCache[ 0 ] =
			ResourceBarrier.Transition( resrc, ResourceStates.CopyDest, ResourceStates.PixelShaderResource ) ;
		commandList.ResourceBarrier( 1, _tempBarrierCache ) ;

		// Lock bitmap to access pixel data
		BitmapData bmpData = bitmap.LockBits( new( 0, 0, bitmap.Width, bitmap.Height ),
											  ImageLockMode.ReadOnly,
											  PixelFormat.Format32bppArgb ) ;
		IntPtr srcDataPtr = bmpData.Scan0 ;
		int    dataSize   = bmpData.Stride * bitmap.Height ;

		// Map upload heap and copy data
		textureUploadHeap.Map( 0, default, out var mappedData ) ;
		unsafe {
			byte* destDataPtr = (byte*)mappedData ;
			Unsafe.CopyBlock( destDataPtr, (void*)srcDataPtr, (uint)dataSize ) ;
		}

		textureUploadHeap.Unmap( 0 ) ;

		commandList.CopyResource( resrc, textureUploadHeap ) ;

		_tempBarrierCache[ 0 ] =
			ResourceBarrier.Transition( resrc, ResourceStates.PixelShaderResource, ResourceStates.CopyDest ) ;
		commandList.ResourceBarrier( 1, _tempBarrierCache ) ;

		commandList.Close( ) ;
		bitmap.UnlockBits( bmpData ) ;
		commandQueue.ExecuteCommandLists( commandList ) ;

		return resrc ;
	}

} ;


		
// Lock the bitmap and get the pixel data
/*BitmapData bmpData = bitmap.LockBits( new( 0, 0, bitmap.Width, bitmap.Height ),
									  ImageLockMode.ReadOnly,
									  PixelFormat.Format32bppArgb ) ;

// Prepare the data to copy
IntPtr pSrcData = bmpData.Scan0 ;
int    stride   = bmpData.Stride ;
int    dataSize = stride * bitmap.Height ;

// Map the upload heap and copy the pixel data
Box mappedData ;
textureUploadHeap.Map(0, default, out nint dataPtr ) ;
unsafe {
	Vertex*          pVertexDataBegin = (Vertex *)mappedResource ;
	Memory< byte > pixels         = new( new Span<byte>(pSrcData) ) ;
	using var        hVertices        = pixels.Pin( ) ;
	Unsafe.CopyBlock( pVertexDataBegin, hVertices.Pointer, (uint)vertexBufferSize ) ;
}
textureUploadHeap.Unmap( 0 ) ;
*/