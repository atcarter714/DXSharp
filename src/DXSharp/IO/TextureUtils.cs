#region Using Directives
using System.Drawing.Imaging ;
using System.Collections.Concurrent ;
using System.Diagnostics.CodeAnalysis ;
using System.Runtime.CompilerServices ;
using DXSharp.Direct3D12 ;
using DXSharp.Direct3D12.XTensions;
using DXSharp.DXGI ;
using IDevice = DXSharp.Direct3D12.IDevice ;
using IResource = DXSharp.Direct3D12.IResource ;
using Resource = DXSharp.Direct3D12.Resource ;

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
	/// <param name="allocator">An (optional) command allocator to use (a temporary one is created if argument is null).</param>
	/// <param name="commandQueue">An (optional) command queue to use</param>
	/// <param name="flags">Special <see cref="ResourceFlags"/> value to set for the resource.</param>
	/// <returns></returns>
	public static IResource? CreateTexture( IDevice               device,
											in Bitmap             bitmap,
											IGraphicsCommandList? commandList  = null,
											ICommandAllocator?    allocator    = null,
											ICommandQueue?        commandQueue = null,
											ResourceFlags         flags        = ResourceFlags.None ) {
#if !STRIP_CHECKS || (DEBUG || DEV_BUILD)
		ArgumentNullException.ThrowIfNull( device, nameof(device) ) ;
		ArgumentNullException.ThrowIfNull( bitmap, nameof(bitmap) ) ;
#endif
		var riid = IResource2.IID ;
		var format = ConvertToDxgiFormat( bitmap.PixelFormat ) ;
		
		var desc = new ResourceDescription {
			Format           = format, //DXGI.Format.R8G8B8A8_UNORM,
			Dimension        = ResourceDimension.Texture2D,
			Layout           = TextureLayout.Unknown,
			Width            = (uint)bitmap.Width,
			Height           = (uint)bitmap.Height,
			Flags            = flags,
			MipLevels        = 1,
			DepthOrArraySize = 1,
		} ;
		
		// Create a committed resource for the GPU resource in a default heap:
		device.CreateCommittedResource( new( HeapType.Default ), HeapFlags.None,
										desc, ResourceStates.CopyDest, null,
										riid, out var textureRsrc ) ;
		if ( textureRsrc is null ) throw new DXSharpException( "Failed to create texture resource!" ) ;
		
		
		// Create a committed resource for the texture data upload:
		device.CreateCommittedResource( new( HeapType.Upload ), HeapFlags.None,
										desc, ResourceStates.GenericRead, null,
										riid, out var textureUploadHeap ) ;
		if ( textureUploadHeap is null ) throw new DXSharpException( "Failed to create texture upload heap!" ) ;
		
		
		//! Ensure we have a command allocator, queue and list:
		if( !_ensureObjects( device, ref commandQueue, ref allocator, ref commandList ) )
			throw new DXSharpException( "Failed to create required pipeline objects!" ) ;
		
		// Create the resource barriers for the transition:
		_tempBarrierCache[ 0 ] =
			ResourceBarrier.Transition( textureRsrc, ResourceStates.CopyDest, ResourceStates.PixelShaderResource ) ;
		commandList!.ResourceBarrier( 1, _tempBarrierCache ) ;
		
		
		// Lock bitmap to access pixel data:
		BitmapData bmpData = bitmap.LockBits( new( 0, 0, bitmap.Width, bitmap.Height ),
											  ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb ) ;
		int dataSize    = bmpData.Stride * bitmap.Height ;
		nint srcDataPtr = bmpData.Scan0 ;

		// Map the upload heap and copy data:
		textureUploadHeap.Map( 0, default, 
							   out nint uploadHeapAddr ) ;
		unsafe {
			byte* destDataPtr = (byte *)uploadHeapAddr ;
			Unsafe.CopyBlock( destDataPtr, (void*)srcDataPtr, (uint)dataSize ) ;
		}
		textureUploadHeap.Unmap( 0 ) ;
		commandList.CopyResource( textureRsrc, textureUploadHeap ) ;
		
		//! TODO: Verify that these transitions are correct and work as intended!
		// Create the other resource barrier for the final transition:
		_tempBarrierCache[ 0 ] =
			ResourceBarrier.Transition( textureRsrc, ResourceStates.PixelShaderResource ) ;
		commandList.ResourceBarrier( 1, _tempBarrierCache ) ;
		
		commandList.Close( ) ;
		commandQueue!.ExecuteCommandLists( commandList ) ;
		
		bitmap.UnlockBits( bmpData ) ;
		return textureRsrc ;
	}

	
	
	static bool _ensureObjects( in IDevice device,
								ref ICommandQueue? commandQueue,
								ref ICommandAllocator?    allocator,
								ref IGraphicsCommandList? commandList ) {
		bool _queueOK = commandQueue is not null,
			 _allocOK = allocator is not null,
			 _listOK  = commandList is not null ;
		if ( _queueOK && _allocOK && _listOK ) return true ;
		
		if ( !_allocOK ) {
			device.CreateCommandAllocator( CommandListType.Direct,
										   CommandAllocator.Guid,
										   out allocator ) ;
			if( allocator is null ) throw new DXSharpException( "Failed to create command allocator!" ) ;
			else _allocOK = true ;
		}
		if ( !_listOK ) {
			device.CreateCommandList( 0, CommandListType.Direct, allocator!, 
									  null, GraphicsCommandList.Guid, out var list ) ;
			commandList = (IGraphicsCommandList?)list ;
			if( commandList is null ) throw new DXSharpException( "Failed to create command list!" ) ;
			else _listOK = true ;
		}
		if ( !_queueOK ){
			device.CreateCommandQueue( CommandQueueDescription.Default, CommandQueue.Guid, out commandQueue ) ;
			if( commandQueue is null ) throw new DXSharpException( "Failed to create command queue!" ) ;
			else _queueOK = true ;
		}
			
		return ( _queueOK && _allocOK && _listOK ) ;
	}
	
	
	
	//! ---------------------------------------------------------------------------
	//! WARNING: AI-generated code ahead! Needs unit testing ...
	public static Format ConvertToDxgiFormat( PixelFormat pixelFormat ) {
		return pixelFormat switch {
				   PixelFormat.Format24bppRgb   => Format.R8G8B8A8_UNORM, // Approximate mapping
				   PixelFormat.Format32bppArgb  => Format.R8G8B8A8_UNORM,
				   PixelFormat.Format32bppRgb   => Format.B8G8R8X8_UNORM, // Approximate mapping
				   PixelFormat.Format32bppPArgb => Format.R8G8B8A8_UNORM,
				   PixelFormat.Format48bppRgb   => Format.R16G16B16A16_FLOAT, // Closest match
				   PixelFormat.Format64bppArgb  => Format.R16G16B16A16_FLOAT,
				   PixelFormat.Format64bppPArgb => Format.R16G16B16A16_FLOAT,
				   //! Additional mappings can be added here ...
				   _ => throw new NotSupportedException($"Unsupported PixelFormat: {pixelFormat}")
			   };
	}
	public static PixelFormat ConvertToPixelFormat( Format dxgiFormat ) {
		return dxgiFormat switch {
				   Format.R8G8B8A8_UNORM     => PixelFormat.Format32bppArgb,
				   Format.B8G8R8X8_UNORM     => PixelFormat.Format32bppRgb,
				   Format.R16G16B16A16_FLOAT => PixelFormat.Format64bppArgb, // Closest match
				   //! Additional mappings can be added here ...
				   _ => throw new NotSupportedException( $"Unsupported DXGI Format: {dxgiFormat}" ),
			   };
	}
	//! ---------------------------------------------------------------------------
} ;



/*	
if ( allocator is null )
	device.CreateCommandAllocator( CommandListType.Direct,
								   CommandAllocator.Guid,
								   out allocator ) ;
if ( commandList is null ) {
	device.CreateCommandList( 0, CommandListType.Direct, allocator, null, GraphicsCommandList.Guid,
							  out var list ) ;
	commandList = (IGraphicsCommandList)list ;
}
if ( commandQueue is null ) {
	device.CreateCommandQueue( new( ), CommandQueue.Guid, out commandQueue ) ;
}
 */