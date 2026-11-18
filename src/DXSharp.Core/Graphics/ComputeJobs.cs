namespace DXSharp.Graphics;

/// <summary>Records ordered one-dimensional compute jobs over a shared set of root UAVs.</summary>
/// <remarks>
/// The caller transitions the buffers into unordered-access state before recording and into the
/// consumer states afterward. The batch inserts UAV barriers between declared conflicting jobs.
/// Keep one batch per ordered sequence and use the same slot for every access to a given buffer.
/// Each pipeline must use root UAV slots followed by an inline constants parameter.
/// </remarks>
public ref struct ComputeJobBatch {
    private readonly GraphicsCommandList _commands;
    private readonly ReadOnlySpan<GpuResource> _buffers;
    private uint _reads, _writes;

    /// <summary>Starts an ordered job sequence on an open command list.</summary>
    /// <param name="commands">The recording command list.</param>
    /// <param name="buffers">Distinct UAV buffers in root slot order.</param>
    public ComputeJobBatch( GraphicsCommandList commands, ReadOnlySpan<GpuResource> buffers ) {
        ArgumentNullException.ThrowIfNull( commands );
        if ( buffers.Length is < 1 or > 31 ) throw new ArgumentOutOfRangeException( nameof(buffers) );
        for ( int i = 0; i < buffers.Length; ++i ) {
            ArgumentNullException.ThrowIfNull( buffers[i] );
            for ( int j = 0; j < i; ++j )
                if ( ReferenceEquals( buffers[i], buffers[j] ) )
                    throw new ArgumentException( "Use one slot per buffer in a compute job batch.", nameof(buffers) );
        }
        _commands = commands;
        _buffers = buffers;
    }

    /// <summary>Dispatches a kernel with one thread per element, rounded up to whole thread groups.</summary>
    /// <param name="pipeline">The compute kernel with the batch root layout.</param>
    /// <param name="elements">The number of active elements. Zero skips the job.</param>
    /// <param name="threadsPerGroup">The kernel's thread group width. The kernel must guard tail threads.</param>
    /// <param name="readSlots">One bit per UAV slot read by this kernel.</param>
    /// <param name="writeSlots">One bit per UAV slot written by this kernel.</param>
    /// <param name="constants">Inline constants for the job.</param>
    public void Dispatch( PipelineState pipeline, uint elements, uint threadsPerGroup,
        uint readSlots, uint writeSlots, scoped ReadOnlySpan<uint> constants ) {
        if ( _commands is null ) throw new InvalidOperationException( "Initialize the batch before recording jobs." );
        if ( elements == 0 ) return;
        ArgumentNullException.ThrowIfNull( pipeline );
        if ( !pipeline.IsCompute ) throw new ArgumentException( "A compute pipeline is required.", nameof(pipeline) );
        if ( threadsPerGroup is 0 or > 1024 ) throw new ArgumentOutOfRangeException( nameof(threadsPerGroup) );
        uint groups = 1 + (elements - 1) / threadsPerGroup;
        if ( groups > 65_535 ) throw new ArgumentOutOfRangeException( nameof(elements) );
        uint mask = (1u << _buffers.Length) - 1;
        if ( (readSlots & ~mask) != 0 ) throw new ArgumentOutOfRangeException( nameof(readSlots) );
        if ( (writeSlots & ~mask) != 0 ) throw new ArgumentOutOfRangeException( nameof(writeSlots) );
        if ( constants.IsEmpty || constants.Length > 64 - 2 * _buffers.Length )
            throw new ArgumentOutOfRangeException( nameof(constants) );
        uint hazards = ((readSlots | writeSlots) & _writes) | (writeSlots & _reads);
        for ( int i = 0; i < _buffers.Length; ++i )
            if ( (hazards & (1u << i)) != 0 ) _commands.UavBarrier( _buffers[i] );

        _commands.SetComputePipeline( pipeline );
        for ( int i = 0; i < _buffers.Length; ++i ) _commands.SetComputeBuffer( (uint)i, _buffers[i] );
        _commands.SetComputeConstants( (uint)_buffers.Length, constants );
        _commands.Dispatch( groups );
        _reads = (_reads & ~hazards) | readSlots;
        _writes = (_writes & ~hazards) | writeSlots;
    }
}
