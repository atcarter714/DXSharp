namespace DXSharp;

/// <summary>A native graphics failure, preserving its HRESULT and operation.</summary>
public sealed class GraphicsException : Exception {
    /// <summary>The operation which failed.</summary>
    public string Operation { get; }
    /// <summary>Creates an exception from a failing native result.</summary>
    public GraphicsException( string operation, int result ) : base( $"{operation} failed (HRESULT 0x{result:X8})." ) {
        Operation = operation;
        HResult = result;
    }
}

internal static class Results {
    internal static void Check( int result, string operation ) { if ( result < 0 ) throw new GraphicsException( operation, result ); }
}
