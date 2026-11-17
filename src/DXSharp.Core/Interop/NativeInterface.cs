namespace DXSharp.Interop;

/// <summary>An owning, typed projection of a generated native COM interface.</summary>
/// <typeparam name="T">A source-generated COM interface exposed by DXSharp.</typeparam>
/// <remarks>
/// Dispose after the final CPU call. The independently owned native reference
/// keeps the COM object alive. The caller must also synchronize GPU use and
/// retain resources referenced by native command recording.
/// </remarks>
public sealed class NativeInterface<T> : IDisposable where T : class {
    private readonly ComOwner<T> _owner;

    internal NativeInterface( ComOwner<T> owner ) => _owner = owner;

    /// <summary>Adopts one existing native COM reference and consumes it even if projection fails.</summary>
    /// <param name="ownedPointer">An owned, nonzero pointer to the interface represented by T.</param>
    /// <returns>A typed owner that releases the native reference when disposed.</returns>
    public static NativeInterface<T> Adopt( nint ownedPointer ) => new( ComOwner<T>.Adopt( ownedPointer ) );

    /// <summary>Acquires an independent reference to a borrowed native COM interface.</summary>
    /// <param name="borrowedPointer">A nonzero interface pointer that remains valid during this call.</param>
    /// <returns>A typed owner. The caller retains ownership of the original reference.</returns>
    public static NativeInterface<T> FromBorrowed( nint borrowedPointer ) => new( ComOwner<T>.FromBorrowed( borrowedPointer ) );

    /// <summary>The generated interface. Calls after disposal throw.</summary>
    public T Interface => _owner.Interface;

    /// <summary>
    /// The owned COM pointer. It remains valid until this lease is disposed.
    /// Synchronize disposal with native code that uses or retains this pointer.
    /// </summary>
    public nint Pointer {
        get {
            using var borrow = _owner.Borrow( );
            return borrow.Pointer;
        }
    }

    /// <summary>Releases the projection and its independently owned COM reference.</summary>
    public void Dispose( ) => _owner.Dispose( );
}
