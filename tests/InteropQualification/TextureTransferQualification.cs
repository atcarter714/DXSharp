using DXSharp;
using DXSharp.Graphics;
using DXSharp.Interop;
using Windows.Win32.Graphics.Direct3D12;
using Windows.Win32.Graphics.Dxgi.Common;

namespace InteropQualification;

internal static unsafe class TextureTransferQualification {
    private static void Check( bool condition, string message ) { if ( !condition ) throw new InvalidOperationException( "Texture transfer: " + message ); }
    private static void Invalid<T>( Action action ) where T : Exception {
        try { action( ); } catch ( T ) { return; }
        throw new InvalidOperationException( $"Expected {typeof(T).Name}." );
    }
    internal static void Run( GraphicsDevice device ) {
        RoundTrip( device, new( 7, 5, DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM, 3, 2 ), compressed: false );
        RoundTrip( device, new( 8, 8, DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM, 4, 2 ), compressed: true );
        RoundTrip( device, new( 4, 4, DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM, 3, 12, Cube: true ), compressed: false );
        ReadbackLifetime( device );
        using var views = device.CreateDescriptorPool( 1 );
        Invalid<ArgumentOutOfRangeException>( ( ) => device.CreateTexture2D( new( 0, 1, DXGI_FORMAT.DXGI_FORMAT_R8_UNORM ), views ) );
        Invalid<ArgumentOutOfRangeException>( ( ) => device.CreateTexture2D( new( 4, 4, DXGI_FORMAT.DXGI_FORMAT_R8_UNORM, 9 ), views ) );
        Invalid<ArgumentException>( ( ) => device.CreateTexture2D( new( 4, 2, DXGI_FORMAT.DXGI_FORMAT_R8_UNORM, ArraySize: 6, Cube: true ), views ) );
        Invalid<NotSupportedException>( ( ) => device.CreateTexture2D( new( 4, 4, DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_TYPELESS ), views ) );
        Console.WriteLine( "PASS: padded mip/array/cube and BC1 block roundtrips, partial subresource updates, and readback lifetime guards." );
    }
    private static void RoundTrip( GraphicsDevice device, Texture2DDescription description, bool compressed ) {
        using var views = device.CreateDescriptorPool( 1 );
        using var uploads = device.CreateUploadRing( 1024 * 1024 );
        using var readbacks = device.CreateReadbackRing( 1024 * 1024 );
        using var texture = device.CreateTexture2D( description, views );
        var data = new TextureSubresourceData[description.SubresourceCount]; var expected = new byte[data.Length][];
        for ( int i = 0; i < data.Length; ++i ) {
            int mip = i % description.MipLevels;
            int width = Math.Max( 1, (int)description.Width >> mip ), height = Math.Max( 1, (int)description.Height >> mip );
            int rowBytes = compressed ? Math.Max( 1, (width + 3) / 4 ) * 8 : width * 4;
            int rows = compressed ? Math.Max( 1, (height + 3) / 4 ) : height;
            int pitch = rowBytes + 7;
            var source = new byte[(rows - 1) * pitch + rowBytes]; var packed = new byte[rows * rowBytes];
            for ( int row = 0; row < rows; ++row ) for ( int col = 0; col < rowBytes; ++col ) {
                byte value = (byte)((i * 31 + row * 17 + col * 3) & 255);
                source[row * pitch + col] = value; packed[row * rowBytes + col] = value;
            }
            data[i] = new( source, (uint)pitch ); expected[i] = packed;
        }
        using var queue = device.CreateCommandQueue( );
        using var allocator = device.CreateCommandAllocator( );
        using var commands = device.CreateCommandList( allocator );
        var invalid = data.ToArray( ); invalid[0] = new( new byte[1], 1 );
        Invalid<ArgumentException>( ( ) => commands.UploadTexture( texture, uploads, invalid ) );
        Check( uploads.AvailableBytes == uploads.Capacity, "invalid input does not reserve staging memory" );
        commands.UploadTexture( texture, uploads, data );
        commands.Transition( texture, ResourceState.CopyDestination, ResourceState.CopySource );
        using var result = commands.ReadbackTexture( texture, readbacks );
        Invalid<InvalidOperationException>( ( ) => result.Read( 0, new byte[result.GetDataSize( 0 )] ) );
        commands.Close( ); queue.Wait( queue.Submit( commands ) );
        for ( uint i = 0; i < description.SubresourceCount; ++i ) {
            var actual = new byte[result.GetDataSize( i )]; result.Read( i, actual );
            Check( actual.AsSpan( ).SequenceEqual( expected[i] ), $"mip/array block payload {i}" );
        }
        result.Dispose( ); Check( readbacks.AvailableBytes == readbacks.Capacity && uploads.AvailableBytes == uploads.Capacity, "staging ranges retire" );

        allocator.Reset( ); commands.Reset( allocator );
        uint selected = description.SubresourceCount - 1;
        byte[] changed = expected[selected].Select( value => (byte)(value ^ 0xa5) ).ToArray( );
        int selectedMip = (int)selected % description.MipLevels;
        int selectedWidth = Math.Max( 1, (int)description.Width >> selectedMip );
        uint selectedPitch = (uint)(compressed ? Math.Max( 1, (selectedWidth + 3) / 4 ) * 8 : selectedWidth * 4);
        commands.Transition( texture, ResourceState.CopySource, ResourceState.CopyDestination );
        commands.UploadTexture( texture, uploads, [new( changed, selectedPitch )], selected );
        commands.Transition( texture, ResourceState.CopyDestination, ResourceState.CopySource );
        using var final = commands.ReadbackTexture( texture, readbacks );
        texture.Dispose( ); views.Dispose( ); uploads.Dispose( ); readbacks.Dispose( );
        commands.Close( ); queue.Wait( queue.Submit( commands ) );
        for ( uint i = 0; i < description.SubresourceCount; ++i ) {
            var actual = new byte[final.GetDataSize( i )]; final.Read( i, actual );
            Check( actual.AsSpan( ).SequenceEqual( i == selected ? changed : expected[i] ), "partial update preserves other subresources and early-disposed backing" );
        }
    }
    private static void ReadbackLifetime( GraphicsDevice device ) {
        using var ring = device.CreateReadbackRing( 512 );
        using var uploads = device.CreateUploadRing( 512 );
        using var source = uploads.Allocate( 512 ); source.Write<uint>( [714, 1234] );
        using var destination = ring.Allocate( 512 );
        using var allocator = device.CreateCommandAllocator( ); using var commands = device.CreateCommandList( allocator );
        using var queue = device.CreateCommandQueue( );
        Guid iid = typeof(ID3D12Fence).GUID;
        Results.Check( device.Native.Interface.CreateFence( 0, 0, &iid, out nint pointer ), "CreateFence(readback gate)" );
        using var gate = ComOwner<ID3D12Fence>.Adopt( pointer );
        commands.CopyBuffer( source, destination, 8 );
        Invalid<InvalidOperationException>( ( ) => destination.Read<uint>( new uint[2] ) );
        commands.Close( );
        try {
            Results.Check( queue.Native.Interface.Wait( gate.Interface, 1 ), "Queue.Wait(readback gate)" );
            ulong submitted = queue.Submit( commands );
            Invalid<InvalidOperationException>( ( ) => destination.Read<uint>( new uint[2] ) );
            Check( !ring.TryAllocate( 1, out _ ), "pending result prevents range reuse" );
            ring.Dispose( );
            Results.Check( gate.Interface.Signal( 1 ), "Signal(readback gate)" ); queue.Wait( submitted );
            Span<uint> actual = stackalloc uint[2]; destination.Read<uint>( actual );
            Check( actual.SequenceEqual( [714u, 1234u] ), "readback remains readable after ring owner disposal" );
        } finally { Results.Check( gate.Interface.Signal( 1 ), "Release readback gate" ); }
        destination.Dispose( ); Invalid<ObjectDisposedException>( ( ) => destination.Read<byte>( new byte[1] ) );
    }
}
