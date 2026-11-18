using Windows.Win32.Graphics.Direct3D12;

namespace DXSharp.Graphics;

/// <summary>Comparison operations for depth and stencil tests.</summary>
public enum ComparisonFunction {
    /// <summary>Always fail.</summary>
    Never = 1,
    /// <summary>Pass when source is less than destination.</summary>
    Less = 2,
    /// <summary>Pass when source equals destination.</summary>
    Equal = 3,
    /// <summary>Pass when source is less than or equal to destination.</summary>
    LessEqual = 4,
    /// <summary>Pass when source is greater than destination.</summary>
    Greater = 5,
    /// <summary>Pass when source differs from destination.</summary>
    NotEqual = 6,
    /// <summary>Pass when source is greater than or equal to destination.</summary>
    GreaterEqual = 7,
    /// <summary>Always pass.</summary>
    Always = 8
}

/// <summary>Operations performed on an eight-bit stencil value.</summary>
public enum StencilOperation {
    /// <summary>Keep the current value.</summary>
    Keep = 1,
    /// <summary>Write zero.</summary>
    Zero = 2,
    /// <summary>Write the dynamic stencil reference.</summary>
    Replace = 3,
    /// <summary>Increment with saturation at 255.</summary>
    IncrementSaturate = 4,
    /// <summary>Decrement with saturation at zero.</summary>
    DecrementSaturate = 5,
    /// <summary>Invert all bits.</summary>
    Invert = 6,
    /// <summary>Increment and wrap at 255.</summary>
    Increment = 7,
    /// <summary>Decrement and wrap at zero.</summary>
    Decrement = 8
}

/// <summary>Stencil tests and operations for one triangle face orientation.</summary>
public sealed record StencilFaceState {
    /// <summary>The comparison between the masked reference and stored value.</summary>
    public ComparisonFunction Comparison { get; init; } = ComparisonFunction.Always;
    /// <summary>The operation when the stencil test fails.</summary>
    public StencilOperation Fail { get; init; } = StencilOperation.Keep;
    /// <summary>The operation when stencil passes and depth fails.</summary>
    public StencilOperation DepthFail { get; init; } = StencilOperation.Keep;
    /// <summary>The operation when both tests pass.</summary>
    public StencilOperation Pass { get; init; } = StencilOperation.Keep;

    internal D3D12_DEPTH_STENCILOP_DESC ToNative( ) {
        if ( !Enum.IsDefined( Comparison ) || !Enum.IsDefined( Fail ) ||
             !Enum.IsDefined( DepthFail ) || !Enum.IsDefined( Pass ) )
            throw new ArgumentException( "A stencil face contains an undefined comparison or operation." );
        return new( ) {
            StencilFunc = (D3D12_COMPARISON_FUNC)Comparison,
            StencilFailOp = (D3D12_STENCIL_OP)Fail,
            StencilDepthFailOp = (D3D12_STENCIL_OP)DepthFail,
            StencilPassOp = (D3D12_STENCIL_OP)Pass
        };
    }
}

/// <summary>Depth and stencil pipeline state for a single-sample graphics pipeline.</summary>
public sealed record DepthStencilState {
    /// <summary>The format of the bound depth-stencil target.</summary>
    public DepthStencilFormat Format { get; init; } = DepthStencilFormat.D32Float;
    /// <summary>Whether the depth test is enabled.</summary>
    public bool DepthEnabled { get; init; } = true;
    /// <summary>Whether passing fragments write depth.</summary>
    public bool DepthWriteEnabled { get; init; } = true;
    /// <summary>The depth comparison.</summary>
    public ComparisonFunction DepthComparison { get; init; } = ComparisonFunction.Less;
    /// <summary>Whether stencil testing and operations are enabled.</summary>
    public bool StencilEnabled { get; init; }
    /// <summary>The mask applied before stencil comparison.</summary>
    public byte StencilReadMask { get; init; } = byte.MaxValue;
    /// <summary>The mask applied when writing stencil.</summary>
    public byte StencilWriteMask { get; init; } = byte.MaxValue;
    /// <summary>Stencil configuration for front-facing triangles.</summary>
    public StencilFaceState FrontFace { get; init; } = new( );
    /// <summary>Stencil configuration for back-facing triangles.</summary>
    public StencilFaceState BackFace { get; init; } = new( );

    internal D3D12_DEPTH_STENCIL_DESC ToNative( ) {
        if ( !Enum.IsDefined( Format ) ) throw new ArgumentOutOfRangeException( nameof(Format) );
        if ( !Enum.IsDefined( DepthComparison ) ) throw new ArgumentOutOfRangeException( nameof(DepthComparison) );
        if ( StencilEnabled && Format is DepthStencilFormat.D16Unorm or DepthStencilFormat.D32Float )
            throw new ArgumentException( "Stencil operations require a stencil-capable format." );
        ArgumentNullException.ThrowIfNull( FrontFace );
        ArgumentNullException.ThrowIfNull( BackFace );
        return new( ) {
            DepthEnable = DepthEnabled,
            DepthWriteMask = DepthWriteEnabled ? D3D12_DEPTH_WRITE_MASK.D3D12_DEPTH_WRITE_MASK_ALL : D3D12_DEPTH_WRITE_MASK.D3D12_DEPTH_WRITE_MASK_ZERO,
            DepthFunc = (D3D12_COMPARISON_FUNC)DepthComparison,
            StencilEnable = StencilEnabled,
            StencilReadMask = StencilReadMask,
            StencilWriteMask = StencilWriteMask,
            FrontFace = FrontFace.ToNative( ),
            BackFace = BackFace.ToNative( )
        };
    }
}
