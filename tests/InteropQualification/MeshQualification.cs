using System.Numerics;
using System.Runtime.InteropServices;
using DXSharp;
using DXSharp.Assets;
using DXSharp.Graphics;
using DXSharp.Interop;
using DXSharp.Shaders;
using FleetBattler;
using Windows.Win32.Graphics.Direct3D12;
using Windows.Win32.Graphics.Dxgi.Common;

namespace InteropQualification;

internal static unsafe class MeshQualification {
    private const uint Width = 61, Height = 47;
    private const string Shader = """
        struct Input {
            float2 position : POSITION;
            float2 offset : TEXCOORD;
            float4 color : COLOR;
        };
        struct Output { float4 position : SV_Position; float4 color : COLOR; };
        Output VS(Input input) {
            Output value;
            value.position = float4(input.position + input.offset, 0, 1);
            value.color = input.color;
            return value;
        }
        float4 PS(Output input) : SV_Target { return input.color; }
        """;

    [StructLayout(LayoutKind.Sequential)]
    private readonly record struct Instance( Vector2 Offset, Vector4 Color );

    internal static void Run( ) {
#if DEBUG
        const bool debug = true;
#else
        const bool debug = false;
#endif
        using var device = GraphicsDevice.Create( new( ) { UseWarp = true, EnableDebugLayer = debug } );
        using var compiler = new ShaderCompiler( );
        var vertex = compiler.Compile( Shader, "VS", "vs_6_0" );
        var pixel = compiler.Compile( Shader, "PS", "ps_6_0" );
        ValidateArguments( device, vertex, pixel );
        Render( device, vertex, pixel, instanceStepRate: 1, splitIndexFormats: true );
        Render( device, vertex, pixel, instanceStepRate: 1, splitIndexFormats: false );
        Render( device, vertex, pixel, instanceStepRate: 2, splitIndexFormats: false );
        RenderFleet( device, compiler );
        VerifyComputeTraffic( device, compiler );
        RenderSystem( device, compiler );
        var diagnostics = device.GetDiagnostics( );
        if ( diagnostics.DeviceRemovedReason < 0 || diagnostics.Messages.Any(
            message => message.Contains( "SEVERITY_ERROR" ) || message.Contains( "SEVERITY_CORRUPTION" ) ) )
            throw new InvalidOperationException( $"Mesh validation failed: {string.Join( Environment.NewLine, diagnostics.Messages )}" );
        Console.WriteLine( "PASS: indexed mesh pixels, UInt16/UInt32, instance streams, and early owner disposal." );
    }

    private static void Render( GraphicsDevice device, ShaderCompilation vertex, ShaderCompilation pixel,
        uint instanceStepRate, bool splitIndexFormats ) {
        using var queue = device.CreateCommandQueue( );
        using var uploadAllocator = device.CreateCommandAllocator( );
        using var uploads = device.CreateCommandList( uploadAllocator );
        Vector2[] vertices = [new(-0.3f,-0.6f), new(0.3f,-0.6f), new(0.3f,0.6f), new(-0.3f,0.6f)];
        using var small = device.CreateMesh<Vector2>( uploads, vertices, new ushort[] { 0, 1, 2, 0, 2, 3 } );
        var largeVertices = new Vector2[65_537];
        vertices.AsSpan( ).CopyTo( largeVertices.AsSpan( 65_533 ) );
        using var large = device.CreateMesh<Vector2>( uploads, largeVertices,
            new uint[] { 65_533, 65_534, 65_535, 65_533, 65_535, 65_536 } );
        uploads.Close( );
        using var root = device.CreateRootSignature( );
        using var pipeline = device.CreateGraphicsPipeline( root, vertex, pixel, [
            new( "POSITION", VertexFormat.Float2, 0 ),
            new( "TEXCOORD", VertexFormat.Float2, 0, Slot: 1, InstanceStepRate: instanceStepRate ),
            new( "COLOR", VertexFormat.Float4, 8, Slot: 1, InstanceStepRate: instanceStepRate )
        ] );
        using var instances = device.CreateBuffer( 3 * 24, BufferMemory.Upload );
        instances.Write<Instance>( [
            new( new(99,99), new(0,0,1,1) ),
            new( new(-0.45f,0), new(1,0,0,1) ), new( new(0.45f,0), new(0,1,0,1) )
        ] );
        using var target = CreateTarget( device );
        using var heap = device.CreateRenderTargetHeap( 1 );
        var view = device.CreateRenderTargetView( target, heap, 0 );
        D3D12_RESOURCE_DESC description = target.Native.Interface.GetDesc( );
        D3D12_PLACED_SUBRESOURCE_FOOTPRINT footprint;
        ulong readbackBytes;
        device.Native.Interface.GetCopyableFootprints( &description, 0, 1, 0, &footprint, null, null, &readbackBytes );
        using var readback = device.CreateBuffer( readbackBytes, BufferMemory.Readback );
        using var drawAllocator = device.CreateCommandAllocator( );
        using var commands = device.CreateCommandList( drawAllocator );
        commands.SetViewport( Width, Height );
        commands.Clear( view, 0, 0, 0 );
        commands.SetRenderTarget( view );
        commands.SetGraphicsPipeline( pipeline );
        commands.SetVertexBuffer( 1, instances, 24, byteOffset: 24, sizeInBytes: 48 );
        if ( splitIndexFormats ) {
            commands.DrawMesh( small );
            commands.DrawMesh( large, startInstance: 1 );
        } else commands.DrawMesh( small, instanceCount: 2 * instanceStepRate );
        commands.Transition( target, ResourceState.RenderTarget, ResourceState.CopySource );
        CopyPixels( commands, target, readback, footprint );
        commands.Close( );

        // Both recordings must retain their resources independently before either submission.
        small.Dispose( );
        large.Dispose( );
        instances.Dispose( );
        root.Dispose( );
        pipeline.Dispose( );
        target.Dispose( );
        heap.Dispose( );
        GC.Collect( );
        GC.WaitForPendingFinalizers( );
        _ = queue.Submit( uploads );
        queue.Wait( queue.Submit( commands ) );
        var bytes = new byte[checked((int)readbackBytes)];
        readback.Read<byte>( bytes );
        ValidatePixels( bytes, footprint );
    }

    private static void RenderFleet( GraphicsDevice device, ShaderCompiler compiler ) {
        var model = GltfModel.Load( Path.Combine( AppContext.BaseDirectory, "Models", "Patrol.gltf" ) );
        using var queue = device.CreateCommandQueue( );
        using var uploadAllocator = device.CreateCommandAllocator( );
        using var uploads = device.CreateCommandList( uploadAllocator );
        var meshes = new Mesh[model.Primitives.Length];
        try {
            FleetDraw.RecordUploads( device, uploads, model, meshes );
            uploads.Close( );
            using var root = device.CreateGraphicsConstantsRootSignature( 16 );
            using var pipeline = FleetDraw.CreatePipeline( device, root, compiler, depthEnabled: true );
            using var instances = device.CreateBuffer( 2 * FleetDraw.InstanceStride, BufferMemory.Upload );
            instances.Write<ShipInstance>( [
                new( new( -5, 0 ), Vector2.UnitX, new( 0.2f, 0.55f, 2.15f, 1 ) ),
                new( new( 5, 0 ), -Vector2.UnitX, new( 2.55f, 0.55f, 0.18f, 1 ) )
            ] );
            using var target = CreateTarget( device );
            using var heap = device.CreateRenderTargetHeap( 1 );
            var view = device.CreateRenderTargetView( target, heap, 0 );
            using var depth = device.CreateDepthTarget( Width, Height );
            var description = target.Native.Interface.GetDesc( );
            D3D12_PLACED_SUBRESOURCE_FOOTPRINT footprint;
            ulong readbackBytes;
            device.Native.Interface.GetCopyableFootprints( &description, 0, 1, 0, &footprint, null, null, &readbackBytes );
            using var readback = device.CreateBuffer( readbackBytes, BufferMemory.Readback );
            using var allocator = device.CreateCommandAllocator( );
            using var commands = device.CreateCommandList( allocator );
            commands.SetViewport( Width, Height );
            commands.Clear( view, 0, 0, 0 );
            commands.SetRenderTarget( view, depth, clearDepth: true );
            var camera = Matrix4x4.CreateLookAt( new( 0, 20, 0 ), Vector3.Zero, -Vector3.UnitZ );
            var projection = Matrix4x4.CreateOrthographic( 18, 12, 0.1f, 40 );
            FleetDraw.Record( commands, pipeline, meshes, instances, 2, camera * projection );
            commands.Transition( target, ResourceState.RenderTarget, ResourceState.CopySource );
            CopyPixels( commands, target, readback, footprint );
            commands.Close( );
            queue.Submit( uploads );
            queue.Wait( queue.Submit( commands ) );
            var bytes = new byte[checked((int)readbackBytes)];
            readback.Read<byte>( bytes );

            int cyan = 0, amber = 0;
            for ( uint y = 0; y < Height; ++y ) {
                for ( uint x = 0; x < Width; ++x ) {
                    int offset = checked((int)(footprint.Offset + y * footprint.Footprint.RowPitch + x * 4));
                    byte r = bytes[offset], g = bytes[offset + 1], b = bytes[offset + 2];
                    if ( (r | g | b) == 0 ) continue;
                    if ( x < Width / 2 - 4 && b > r + 24 ) ++cyan;
                    if ( x > Width / 2 + 4 && r > b + 24 ) ++amber;
                    if ( x >= Width / 2 - 4 && x <= Width / 2 + 4 )
                        throw new InvalidOperationException( "Fleet pixels crossed the clear gap between instances." );
                }
            }
            if ( cyan < 24 || amber < 24 )
                throw new InvalidOperationException( $"Missing tinted fleet geometry: {cyan} cyan, {amber} amber pixels." );
            Console.WriteLine( $"PASS: patrol fleet indexed instancing read back {cyan} cyan and {amber} amber pixels." );
        } finally {
            foreach ( var mesh in meshes ) mesh?.Dispose( );
        }
    }

    private static void VerifyComputeTraffic( GraphicsDevice device, ShaderCompiler compiler ) {
        const int ships = 8_193, debris = 2_049;
        var fleet = new FleetSimulation( FleetSimulation.DefaultShips, 37 );
        using var traffic = new ComputeTraffic( device, compiler, ships, debris );
        using var queue = device.CreateCommandQueue( );
        using var allocator = device.CreateCommandAllocator( );
        using var commands = device.CreateCommandList( allocator );
        ulong strideBytes = ships * FleetDraw.InstanceStride;
        using var shipResults = device.CreateBuffer( 2 * strideBytes, BufferMemory.Readback );
        using var bodyResults = device.CreateBuffer( (debris + CelestialSystem.BodyCount) * 16, BufferMemory.Readback );
        traffic.RecordCompute( commands, fleet, 1f / 60 );
        traffic.Finish( commands );
        Copy( traffic.Instances, shipResults, strideBytes );
        for ( int i = 0; i < 30; ++i ) fleet.Step( );
        traffic.RecordCompute( commands, fleet, 1f / 60 );
        traffic.Finish( commands );
        Copy( traffic.Instances, shipResults, strideBytes, strideBytes );
        Copy( traffic.Bodies, bodyResults, bodyResults.SizeInBytes );
        commands.Close( );
        queue.Wait( queue.Submit( commands ) );
        var a = new ShipInstance[ships];
        var b = new ShipInstance[ships];
        var bodies = new Vector4[debris + CelestialSystem.BodyCount];
        shipResults.Read<ShipInstance>( a );
        shipResults.Read<ShipInstance>( b, strideBytes );
        bodyResults.Read<Vector4>( bodies );
        for ( int i = 0; i < fleet.Bodies.Length; ++i ) {
            var body = fleet.Bodies[i];
            if ( bodies[i] != new Vector4( body.Center.X, body.Center.Y, body.Radius, 0 ) )
                throw new InvalidOperationException( $"Compute body {i} disagrees with the tactical body catalog." );
        }
        int moving = 0;
        for ( int i = 0; i < b.Length; ++i ) {
            var agent = b[i];
            if ( !float.IsFinite( agent.Center.X + agent.Center.Y ) ||
                MathF.Abs( agent.Heading.LengthSquared( ) - 1 ) > 0.0001f || agent.Motion.Z != 0.28f )
                throw new InvalidOperationException( $"Invalid compute steering output for ship {i}." );
            if ( Vector2.DistanceSquared( a[i].Center, agent.Center ) > 1e-7f ) ++moving;
        }
        for ( int i = CelestialSystem.BodyCount; i < bodies.Length; ++i )
            if ( bodies[i].Z is < 0.09f or > 0.62f || !float.IsFinite( bodies[i].X + bodies[i].Y ) )
                throw new InvalidOperationException( $"Invalid compute orbit output for debris {i}." );
        if ( moving < ships * 0.98 || a[^1].Center == b[^1].Center )
            throw new InvalidOperationException( $"Compute steering moved only {moving} ships." );
        Console.WriteLine( $"PASS: compute orbit field and steering moved {moving} of {ships} ships around {debris} debris bodies." );

        void Copy( GpuResource source, GpuResource destination, ulong size, ulong offset = 0 ) {
            commands.Transition( source, ResourceState.Common, ResourceState.CopySource );
            commands.CopyBuffer( source, destination, size, destinationOffset: offset );
            commands.Transition( source, ResourceState.CopySource, ResourceState.Common );
        }
    }

    private static void RenderSystem( GraphicsDevice device, ShaderCompiler compiler ) {
        const uint width = 640, height = 400;
        var model = GltfModel.Load( Path.Combine( AppContext.BaseDirectory, "Models", "Patrol.gltf" ) );
        var fleet = new FleetSimulation( FleetSimulation.DefaultShips, 37, battle: true );
        using var queue = device.CreateCommandQueue( );
        using var allocator = device.CreateCommandAllocator( );
        using var commands = device.CreateCommandList( allocator );
        using var traffic = new ComputeTraffic( device, compiler, ComputeTraffic.DefaultAgents, ComputeTraffic.DefaultDebris );
        var meshes = new Mesh[model.Primitives.Length];
        try {
            FleetDraw.RecordUploads( device, commands, model, meshes );
            using var fleetRoot = device.CreateGraphicsConstantsRootSignature( 16 );
            using var thrustRoot = device.CreateGraphicsConstantsRootSignature( 20 );
            using var thrust = FleetDraw.CreateThrusters( device, thrustRoot, compiler );
            using var battleRoot = device.CreateGraphicsConstantsRootSignature( BattleDraw.ConstantCount );
            using var effects = BattleDraw.CreateEffects( device, battleRoot, compiler );
            using var markers = BattleDraw.CreateMarkers( device, battleRoot, compiler );
            using var effectData = device.CreateBuffer( FleetSimulation.EffectCapacity * BattleDraw.EffectStride, BufferMemory.Upload );
            using var spaceRoot = device.CreateGraphicsConstantsRootSignature( SpaceDraw.Constants );
            using var hudRoot = device.CreateGraphicsConstantsRootSignature( 4 );
            using var fleetPipeline = FleetDraw.CreatePipeline( device, fleetRoot, compiler, depthEnabled: true );
            using var skyPipeline = SpaceDraw.CreatePipeline( device, spaceRoot, compiler );
            using var hudPipeline = FleetHud.CreatePipeline( device, hudRoot, compiler );
            using var glyphs = device.CreateBuffer( FleetHud.BufferBytes, BufferMemory.Upload );
            using var instances = device.CreateBuffer( (uint)fleet.Count * FleetDraw.InstanceStride, BufferMemory.Upload );
            fleet.Move( 0, fleet.Site + new Vector2( -5, 0 ) );
            fleet.Move( 6, fleet.Site + new Vector2( 5, 0 ) );
            for ( int i = 0; i < 480 && fleet.ShotsFired < 6; ++i ) fleet.Step( );
            var pulses = new BattleEffect[FleetSimulation.EffectCapacity];
            int pulseCount = fleet.FillEffects( pulses );
            if ( pulseCount == 0 ) throw new InvalidOperationException( "The WARP fixture did not produce a skirmish." );
            effectData.Write<BattleEffect>( pulses.AsSpan( 0, pulseCount ) );
            var ships = new ShipInstance[fleet.ActiveCount];
            fleet.FillInstances( ships, new( 0 ) );
            instances.Write<ShipInstance>( ships );
            using var target = CreateTarget( device, width, height );
            using var heap = device.CreateRenderTargetHeap( 1 );
            var view = device.CreateRenderTargetView( target, heap, 0 );
            using var depth = device.CreateDepthTarget( width, height );
            var description = target.Native.Interface.GetDesc( );
            D3D12_PLACED_SUBRESOURCE_FOOTPRINT footprint;
            ulong readbackBytes;
            device.Native.Interface.GetCopyableFootprints( &description, 0, 1, 0, &footprint, null, null, &readbackBytes );
            using var readback = device.CreateBuffer( readbackBytes, BufferMemory.Readback );
            commands.SetViewport( width, height );
            commands.Clear( view, 0, 0, 0 );
            commands.SetRenderTarget( view, depth, clearDepth: true );
            float radius = fleet.BoardRadius, halfHeight = radius * 0.94f;
            var camera = Matrix4x4.CreateLookAt( new( 0, radius * 1.6f, radius * 0.88f ), Vector3.Zero, Vector3.UnitY );
            var projection = Matrix4x4.CreateOrthographic( 2 * halfHeight * width / height, 2 * halfHeight, 0.1f, 4 * radius );
            var matrix = camera * projection;
            Matrix4x4.Invert( matrix, out var inverse );
            traffic.RecordCompute( commands, fleet, 1f / 60 );
            SpaceDraw.Record( commands, skyPipeline, fleet, new( 0 ), matrix, inverse, (int)width, (int)height, halfHeight );
            traffic.RecordDebris( commands, matrix, (int)width, (int)height, halfHeight );
            traffic.RecordFreighters( commands, matrix );
            traffic.Finish( commands );
            FleetDraw.RecordThrusters( commands, thrust, instances, (uint)ships.Length, matrix, (float)(fleet.Tick * FleetSimulation.StepSeconds) );
            FleetDraw.Record( commands, fleetPipeline, meshes, instances, (uint)ships.Length, matrix );
            BattleDraw.Record( commands, markers, instances, (uint)ships.Length, matrix, (int)width, (int)height, halfHeight, effects: false );
            BattleDraw.Record( commands, effects, effectData, (uint)pulseCount, matrix, (int)width, (int)height, halfHeight, effects: true );
            Span<Vector4> bodyLabels = stackalloc Vector4[CelestialSystem.BodyCount];
            for ( int i = 0; i < bodyLabels.Length; ++i )
                bodyLabels[i] = CelestialSystem.Project( fleet.Bodies[i], radius, matrix, (int)width, (int)height, halfHeight );
            new FleetHud( ).Record( commands, hudPipeline, glyphs, fleet, new( 0 ), (int)width, (int)height, false, bodyLabels,
                selectionCount: fleet.ShipCount( 0 ), planet: 1, beacon: CommandSelection.Project( fleet.Site, matrix, new( width, height ) ) );
            commands.Transition( target, ResourceState.RenderTarget, ResourceState.CopySource );
            CopyPixels( commands, target, readback, footprint );
            commands.Close( );
            queue.Wait( queue.Submit( commands ) );
            var bytes = new byte[checked((int)readbackBytes)];
            readback.Read<byte>( bytes );
            var rgb = new byte[width * height * 3];
            int sun = 0, title = 0, sky = 0;
            for ( uint y = 0; y < height; ++y ) {
                for ( uint x = 0; x < width; ++x ) {
                    int source = checked((int)(footprint.Offset + y * footprint.Footprint.RowPitch + x * 4));
                    int destination = checked((int)((y * width + x) * 3));
                    bytes.AsSpan( source, 3 ).CopyTo( rgb.AsSpan( destination, 3 ) );
                    byte r = bytes[source], g = bytes[source + 1], b = bytes[source + 2];
                    if ( x > 160 && x < width - 165 && y > 80 && y < height - 65 && r > 220 && g > 130 && b < r - 40 ) ++sun;
                    if ( x is >= 23 and < 152 && y is >= 23 and < 36 && r > 225 && g > 235 && b > 245 ) ++title;
                    if ( y > 100 && y < 320 && b > r + 5 && r is > 0 and < 50 ) ++sky;
                }
            }
            if ( sun < 500 || title < 100 || sky < 10_000 )
                throw new InvalidOperationException( $"Missing fleet scene pixels: {sun} stellar, {title} HUD, {sky} sky." );
            if ( Environment.GetEnvironmentVariable( "DXSHARP_FLEET_EVIDENCE" ) is { Length: > 0 } evidence ) {
                string path = Path.GetFullPath( evidence );
                Directory.CreateDirectory( Path.GetDirectoryName( path )! );
                using var file = File.Create( path );
                file.Write( "P6\n640 400\n255\n"u8 );
                file.Write( rgb );
            }
            Console.WriteLine( $"PASS: rendered fleet skirmish, freighters, and planet HUD ({pulseCount} pulses, {sun} stellar, {title} title, {sky} sky pixels)." );
        } finally {
            foreach ( var mesh in meshes ) mesh?.Dispose( );
        }
    }

    private static GpuResource CreateTarget( GraphicsDevice device, uint width = Width, uint height = Height ) {
        D3D12_RESOURCE_DESC description = new( ) {
            Dimension = D3D12_RESOURCE_DIMENSION.D3D12_RESOURCE_DIMENSION_TEXTURE2D,
            Width = width, Height = height, DepthOrArraySize = 1, MipLevels = 1,
            Format = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM, SampleDesc = new( ) { Count = 1 },
            Flags = D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET
        };
        D3D12_HEAP_PROPERTIES heap = new( ) {
            Type = D3D12_HEAP_TYPE.D3D12_HEAP_TYPE_DEFAULT, CreationNodeMask = 1, VisibleNodeMask = 1
        };
        Guid id = typeof(ID3D12Resource).GUID;
        nint pointer = 0;
        Results.Check( device.Native.Interface.CreateCommittedResource( &heap, 0, &description,
            D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_RENDER_TARGET, null, &id, (void**)&pointer ), "Create mesh qualification target" );
        return new( ComOwner<ID3D12Resource>.Adopt( pointer ), 0, BufferMemory.Default ) { DeviceIdentity = device.DeviceIdentity };
    }

    private static void CopyPixels( GraphicsCommandList commands, GpuResource source, GpuResource destination,
        D3D12_PLACED_SUBRESOURCE_FOOTPRINT footprint ) {
        commands.RetainDependency( destination );
        using var sourceLease = source.Native.Borrow( );
        using var destinationLease = destination.Native.Borrow( );
        D3D12_TEXTURE_COPY_LOCATION from = new( ) {
            pResource = (ID3D12Resource_unmanaged*)sourceLease.Pointer,
            Type = D3D12_TEXTURE_COPY_TYPE.D3D12_TEXTURE_COPY_TYPE_SUBRESOURCE_INDEX
        };
        D3D12_TEXTURE_COPY_LOCATION to = new( ) {
            pResource = (ID3D12Resource_unmanaged*)destinationLease.Pointer,
            Type = D3D12_TEXTURE_COPY_TYPE.D3D12_TEXTURE_COPY_TYPE_PLACED_FOOTPRINT
        };
        to.Anonymous.PlacedFootprint = footprint;
        commands.Native.Interface.CopyTextureRegion( to, 0, 0, 0, from, null );
    }

    private static void ValidatePixels( byte[] bytes, D3D12_PLACED_SUBRESOURCE_FOOTPRINT footprint ) {
        int colored = 0;
        for ( uint y = 0; y < Height; ++y ) {
            for ( uint x = 0; x < Width; ++x ) {
                float px = 2 * (x + 0.5f) / Width - 1;
                float py = 1 - 2 * (y + 0.5f) / Height;
                // Ignore the rasterization boundary while checking every interior and exterior pixel.
                if ( MathF.Abs( MathF.Abs( py ) - 0.6f ) < 0.025f ||
                     MathF.Abs( MathF.Abs( px ) - 0.15f ) < 0.025f ||
                     MathF.Abs( MathF.Abs( px ) - 0.75f ) < 0.025f ) continue;
                bool inside = MathF.Abs( py ) < 0.6f && MathF.Abs( px ) > 0.15f && MathF.Abs( px ) < 0.75f;
                byte red = inside && px < 0 ? byte.MaxValue : (byte)0;
                byte green = inside && px > 0 ? byte.MaxValue : (byte)0;
                int offset = checked((int)(footprint.Offset + y * footprint.Footprint.RowPitch + x * 4));
                if ( bytes[offset] != red || bytes[offset + 1] != green || bytes[offset + 2] != 0 || bytes[offset + 3] != 255 )
                    throw new InvalidOperationException( $"Indexed mesh pixel ({x}, {y}) differs." );
                if ( inside ) ++colored;
            }
        }
        if ( colored < 700 ) throw new InvalidOperationException( "Mesh qualification checked too few colored pixels." );
    }

    private static void ValidateArguments( GraphicsDevice device, ShaderCompilation vertex, ShaderCompilation pixel ) {
        using var allocator = device.CreateCommandAllocator( );
        using var commands = device.CreateCommandList( allocator );
        using var root = device.CreateRootSignature( );
        Vector2[] vertices = [new(0,0), new(1,0), new(0,1)];
        Expect<ArgumentException>( ( ) => device.CreateMesh<Vector2>( commands, Array.Empty<Vector2>( ), new ushort[] { 0 } ) );
        Expect<ArgumentOutOfRangeException>( ( ) => device.CreateMesh<Vector2>( commands, vertices, new ushort[] { 3 } ) );
        Expect<ArgumentOutOfRangeException>( ( ) => device.CreateMesh<Vector2>( commands, vertices, new uint[] { uint.MaxValue } ) );
        Expect<ArgumentException>( ( ) => device.CreateGraphicsPipeline( root, vertex, pixel, [default(VertexElement)] ) );
        Expect<ArgumentException>( ( ) => device.CreateGraphicsPipeline( root, vertex, pixel, [new("POSITION", (VertexFormat)999, 0)] ) );
        Expect<ArgumentException>( ( ) => device.CreateGraphicsPipeline( root, vertex, pixel, [new("POSITION", VertexFormat.Float4, 2040)] ) );
        using var buffer = device.CreateBuffer( 64, BufferMemory.Upload );
        using var readback = device.CreateBuffer( 64, BufferMemory.Readback );
        Expect<ArgumentOutOfRangeException>( ( ) => commands.SetVertexBuffer( 32, buffer, 8 ) );
        Expect<ArgumentOutOfRangeException>( ( ) => commands.SetVertexBuffer( 0, buffer, 0 ) );
        Expect<ArgumentException>( ( ) => commands.SetVertexBuffer( 0, readback, 8 ) );
        Expect<ArgumentException>( ( ) => commands.SetIndexBuffer( buffer, IndexFormat.UInt16, byteOffset: 1 ) );
        Expect<ArgumentOutOfRangeException>( ( ) => commands.SetIndexBuffer( buffer, (IndexFormat)0 ) );
        using var mesh = device.CreateMesh<Vector2>( commands, vertices, new ushort[] { 0, 1, 2 } );
        mesh.Dispose( );
        Expect<ObjectDisposedException>( ( ) => commands.DrawMesh( mesh ) );
        commands.Close( );
        Expect<InvalidOperationException>( ( ) => device.CreateMesh<Vector2>( commands, vertices, new ushort[] { 0, 1, 2 } ) );
        using var computeAllocator = device.CreateCommandAllocator( QueueKind.Compute );
        using var compute = device.CreateCommandList( computeAllocator );
        Expect<InvalidOperationException>( ( ) => device.CreateMesh<Vector2>( compute, vertices, new ushort[] { 0, 1, 2 } ) );
        Expect<InvalidOperationException>( ( ) => compute.SetVertexBuffer( 0, buffer, 8 ) );
    }

    private static void Expect<T>( Action action ) where T : Exception {
        try { action( ); }
        catch ( T ) { return; }
        throw new InvalidOperationException( $"Expected {typeof(T).Name}." );
    }
}
