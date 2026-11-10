namespace DXSharp.Graphics;

/// <summary>Pixel formats accepted by the graphics API.</summary>
public enum PixelFormat {
    /// <summary>Four unsigned normalized eight-bit channels in red, green, blue, alpha order.</summary>
    Rgba8Unorm = 28,
    /// <summary>Four unsigned normalized eight-bit channels in blue, green, red, alpha order.</summary>
    Bgra8Unorm = 87
}

/// <summary>The type of work a command queue and its command lists accept.</summary>
public enum QueueKind {
    /// <summary>Graphics, compute and copy commands, including commands for presentation.</summary>
    Direct = 0,
    /// <summary>Compute dispatches and supported copy commands.</summary>
    Compute = 2,
    /// <summary>Copy commands on a dedicated copy queue.</summary>
    Copy = 3
}

/// <summary>Explicit native resource states. Supply states that match the resource's actual usage on the queue.</summary>
[Flags]
public enum ResourceState : uint {
    /// <summary>Common state used for eligible implicit promotion and cross-engine handoff.</summary>
    Common = 0,
    /// <summary>State required when presenting a swap-chain buffer. It has the same native value as <see cref="Common"/>.</summary>
    Present = 0,
    /// <summary>Read as a vertex buffer or constant buffer.</summary>
    VertexAndConstantBuffer = 1,
    /// <summary>Write as a color render target.</summary>
    RenderTarget = 4,
    /// <summary>Access through an unordered-access view.</summary>
    UnorderedAccess = 8,
    /// <summary>Read as a shader resource from a stage other than the pixel shader.</summary>
    NonPixelShaderResource = 0x40,
    /// <summary>Read as a shader resource from the pixel shader.</summary>
    PixelShaderResource = 0x80,
    /// <summary>Write as the destination of a copy operation.</summary>
    CopyDestination = 0x400,
    /// <summary>Read as the source of a copy operation.</summary>
    CopySource = 0x800,
    /// <summary>The combined read states required for upload buffers.</summary>
    GenericRead = 0xac3,
    /// <summary>The creation state for a DXR acceleration structure. Acceleration structures remain in this state.</summary>
    RaytracingAccelerationStructure = 0x400000
}

/// <summary>CPU and GPU access characteristics of a buffer allocation.</summary>
public enum BufferMemory {
    /// <summary>GPU working memory. Stage CPU transfers through upload and readback buffers.</summary>
    Default = 1,
    /// <summary>CPU-writable staging memory that the GPU can read.</summary>
    Upload = 2,
    /// <summary>CPU-readable staging memory. Wait for the copy fence before reading.</summary>
    Readback = 3
}
/// <summary>Device selection and validation options.</summary>
public sealed record DeviceOptions {
    /// <summary>Use the packaged Agility SDK through a local device factory.</summary>
    public bool UseAgility { get; init; } = true;
    /// <summary>Use Microsoft's software adapter for deterministic headless validation.</summary>
    public bool UseWarp { get; init; }
    /// <summary>Enable validation before creating the device.</summary>
    public bool EnableDebugLayer { get; init; }
}
/// <summary>A snapshot of device status and recent validation messages.</summary>
/// <param name="Adapter">Description of the selected adapter.</param>
/// <param name="AgilityRequested">Whether creation used the packaged Agility runtime.</param>
/// <param name="DeviceRemovedReason">The device status HRESULT. A negative value identifies a failure.</param>
/// <param name="Messages">Recent validation messages, in retrieval order, when an info queue is available.</param>
public sealed record GraphicsDiagnostics( string Adapter, bool AgilityRequested, int DeviceRemovedReason, IReadOnlyList<string> Messages );

/// <summary>Base for deterministic graphics owners. Dispose after their final CPU use; submissions retain their native references.</summary>
public abstract class GraphicsObject : IDisposable {
    private IDisposable? _owner;
    internal GraphicsObject( IDisposable owner ) => _owner = owner;
    /// <summary>Whether this public owner has been disposed.</summary>
    public bool IsDisposed => Volatile.Read( ref _owner ) is null;
    internal object DeviceIdentity { get; init; } = new( );
    internal void RequireSameDevice( GraphicsObject other ) {
        ObjectDisposedException.ThrowIf( IsDisposed, this );
        ArgumentNullException.ThrowIfNull( other );
        ObjectDisposedException.ThrowIf( other.IsDisposed, other );
        if ( !ReferenceEquals( DeviceIdentity, other.DeviceIdentity ) ) throw new ArgumentException( "Objects belong to different devices.", nameof(other) );
    }
    internal abstract Interop.ComReference Retain( );
    /// <summary>Queries an independently owned native interface.</summary>
    /// <param name="interfaceId">The IID passed to native QueryInterface.</param>
    /// <returns>A safe handle that releases the acquired COM reference when disposed.</returns>
    /// <exception cref="GraphicsException">The native QueryInterface call failed.</exception>
    public System.Runtime.InteropServices.SafeHandle QueryNativeInterface( Guid interfaceId ) {
        using var owner = Retain( );
        int result = Interop.UnknownCalls.QueryInterface( owner.DangerousGetHandle( ), interfaceId, out nint pointer );
        Results.Check( result, "QueryInterface" );
        return new Interop.ComReference( pointer );
    }
    /// <summary>Releases this owner's references. Native submission leases are independent.</summary>
    public virtual void Dispose( ) => Interlocked.Exchange( ref _owner, null )?.Dispose( );
}
