using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace DXSharp.Interop;

// The only hand-written vtable calls in the modern core are the three invariant IUnknown slots.
internal static unsafe class UnknownCalls {
    internal static int QueryInterface( nint value, in Guid iid, out nint result ) {
        result = 0;
        fixed ( Guid* id = &iid ) fixed ( nint* output = &result ) {
            return ((delegate* unmanaged[MemberFunction]<nint, Guid*, nint*, int>)(*(nint**)value)[0])( value, id, output );
        }
    }
    internal static uint AddRef( nint value ) => ((delegate* unmanaged[MemberFunction]<nint, uint>)(*(nint**)value)[1])( value );
    internal static uint Release( nint value ) => ((delegate* unmanaged[MemberFunction]<nint, uint>)(*(nint**)value)[2])( value );
}

internal sealed class ComReference : SafeHandle {
    internal ComReference( nint owned ) : base( 0, true ) => SetHandle( owned );
    public override bool IsInvalid => handle == 0;
    protected override bool ReleaseHandle( ) { UnknownCalls.Release( handle ); return true; }
}

/// <summary>Owns one native interface reference and one private generated projection.</summary>
/// <remarks>Callers synchronize disposal with method calls. A borrow keeps its native reference live during disposal.</remarks>
internal sealed class ComOwner<T> : IDisposable where T : class {
    private static readonly StrategyBasedComWrappers Wrappers = new( );
    private readonly ComReference _native;
    private readonly ComObject _projection;
    private readonly T _interface;
    private int _disposed;

    private ComOwner( ComReference native, ComObject projection, T value ) {
        _native = native;
        _projection = projection;
        _interface = value;
    }

    internal T Interface {
        get { ObjectDisposedException.ThrowIf( Volatile.Read( ref _disposed ) != 0, this ); return _interface; }
    }
    internal bool IsDisposed => Volatile.Read( ref _disposed ) != 0;

    internal static ComOwner<T> Adopt( nint owned ) {
        if ( owned == 0 ) throw new ArgumentNullException( nameof(owned) );
        var native = new ComReference( owned );
        ComObject? projection = null;
        try {
            projection = (ComObject)Wrappers.GetOrCreateObjectForComInstance( owned, CreateObjectFlags.UniqueInstance );
            return new( native, projection, (T)(object)projection );
        } catch {
            projection?.FinalRelease( );
            native.Dispose( );
            throw;
        }
    }

    internal static ComOwner<T> FromBorrowed( nint borrowed ) {
        if ( borrowed == 0 ) throw new ArgumentNullException( nameof(borrowed) );
        UnknownCalls.AddRef( borrowed );
        return Adopt( borrowed );
    }

    internal NativeBorrow Borrow( ) {
        ObjectDisposedException.ThrowIf( IsDisposed, this );
        return new( _native );
    }

    internal ComOwner<T> Clone( ) {
        using var lease = Borrow( );
        return FromBorrowed( lease.Pointer );
    }

    internal ComReference Retain( ) {
        using var lease = Borrow( );
        UnknownCalls.AddRef( lease.Pointer );
        return new( lease.Pointer );
    }

    internal bool TryQuery<TInterface>( in Guid iid, out ComOwner<TInterface>? result ) where TInterface : class {
        using var lease = Borrow( );
        int hr = UnknownCalls.QueryInterface( lease.Pointer, iid, out nint pointer );
        if ( hr < 0 ) {
            result = null;
            if ( hr != unchecked((int)0x80004002) ) Results.Check( hr, "QueryInterface" );
            return false;
        }
        result = ComOwner<TInterface>.Adopt( pointer );
        return true;
    }

    public void Dispose( ) {
        if ( Interlocked.Exchange( ref _disposed, 1 ) != 0 ) return;
        try { _projection.FinalRelease( ); }
        finally { _native.Dispose( ); }
    }
}

internal ref struct NativeBorrow {
    private ComReference? _owner;
    internal nint Pointer { get; private set; }
    internal NativeBorrow( ComReference owner ) {
        bool acquired = false;
        owner.DangerousAddRef( ref acquired );
        _owner = owner;
        Pointer = owner.DangerousGetHandle( );
    }
    public void Dispose( ) { var owner = _owner; _owner = null; Pointer = 0; owner?.DangerousRelease( ); }
}
