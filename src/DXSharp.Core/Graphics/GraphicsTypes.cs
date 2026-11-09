namespace DXSharp.Graphics;

/// <summary>Formats supported by the first modern graphics surface.</summary>
public enum PixelFormat { Rgba8Unorm = 28, Bgra8Unorm = 87 }
/// <summary>The type of work a command queue accepts.</summary>
public enum QueueKind { Direct = 0, Compute = 2, Copy = 3 }
/// <summary>Explicit native resource states.</summary>
[Flags]
public enum ResourceState : uint {
    Common = 0, Present = 0, VertexAndConstantBuffer = 1, RenderTarget = 4, UnorderedAccess = 8,
    CopyDestination = 0x400, CopySource = 0x800, GenericRead = 0xac3
}
/// <summary>CPU/GPU buffer placement.</summary>
public enum BufferMemory { Default = 1, Upload = 2, Readback = 3 }
/// <summary>Device selection and validation options.</summary>
public sealed record DeviceOptions {
    /// <summary>Use the packaged Agility SDK through a local device factory.</summary>
    public bool UseAgility { get; init; } = true;
    /// <summary>Use Microsoft's software adapter for deterministic headless validation.</summary>
    public bool UseWarp { get; init; }
    /// <summary>Enable validation before creating the device.</summary>
    public bool EnableDebugLayer { get; init; }
}
/// <summary>Diagnostics suitable for retaining alongside a crash dump.</summary>
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
    /// <summary>Queries an independently owned native interface. Dispose the returned handle; do not manually Release its pointer.</summary>
    public System.Runtime.InteropServices.SafeHandle QueryNativeInterface( Guid interfaceId ) {
        using var owner = Retain( );
        int result = Interop.UnknownCalls.QueryInterface( owner.DangerousGetHandle( ), interfaceId, out nint pointer );
        Results.Check( result, "QueryInterface" );
        return new Interop.ComReference( pointer );
    }
    /// <summary>Releases this owner's references. Native submission leases are independent.</summary>
    public virtual void Dispose( ) => Interlocked.Exchange( ref _owner, null )?.Dispose( );
}
