namespace DXSharp.MachineLearning;

/// <summary>An immutable, contiguous, row-major Float32 tensor shape.</summary>
public sealed class TensorShape {
    private readonly uint[] _dimensions;
    /// <summary>Tensor rank, from one through eight.</summary>
    public int Rank => _dimensions.Length;
    /// <summary>The number of elements, with checked dimension multiplication.</summary>
    public ulong ElementCount { get; }
    /// <summary>The required number of buffer bytes, including DirectML's four-byte alignment.</summary>
    public ulong SizeInBytes => checked(ElementCount * sizeof(float));
    /// <summary>A read-only view of the copied dimensions.</summary>
    public ReadOnlySpan<uint> Dimensions => _dimensions;
    /// <summary>Creates a dense Float32 shape. Dimensions must be nonzero.</summary>
    public TensorShape( params ReadOnlySpan<uint> dimensions ) {
        if ( dimensions.Length is < 1 or > 8 ) throw new ArgumentOutOfRangeException( nameof(dimensions) );
        ulong count = 1;
        foreach ( uint dimension in dimensions ) {
            if ( dimension == 0 ) throw new ArgumentOutOfRangeException( nameof(dimensions) );
            count = checked(count * dimension);
        }
        _dimensions = dimensions.ToArray( ); ElementCount = count;
        _ = SizeInBytes;
    }
}
