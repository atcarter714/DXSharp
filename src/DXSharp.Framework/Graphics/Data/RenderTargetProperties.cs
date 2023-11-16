#region Using Directives
using System.Drawing ;
using DXSharp.Direct3D12 ;
using DXSharp.DXGI ;
#endregion
namespace DXSharp.Framework.Graphics ;


/// <summary>Represents options for the pipeline render target surfaces.</summary>
public partial struct RenderTargetProperties {
	const uint DEFAULT_BUFFER_COUNT = 2,
			   MAX_BUFFER_COUNT     = DXSharpUtils.D3D12_SIMULTANEOUS_RENDER_TARGET_COUNT ;
	
	// -----------------------------------------------------
	/// <summary>A simple set of default backbuffer options.</summary>
	/// <remarks>
	/// Double-buffering (<see cref="BufferCount"/>: <c>2</c>) with the default clear color of
	/// <see cref="ClearValue.TextureDefault"/> (i.e., <see cref="Color.Black"/>).
	/// </remarks>
	public static readonly RenderTargetProperties Default 
		= new( 2, ClearValue.TextureDefault ) ;
	// -----------------------------------------------------
	
	uint _bufferCount ;
	bool _allowTearing ;
	ClearValue _clearValue ;
	
	/// <summary>The number of backbuffers to use.</summary>
	public uint BufferCount {
		get => _bufferCount ;
		set {
#if DEBUG || DEV_BUILD
			if( value > MAX_BUFFER_COUNT )
				throw new ArgumentOutOfRangeException( nameof(BufferCount),
													   $"The maximum number of backbuffers is {MAX_BUFFER_COUNT}." ) ;
			else if( value < 1 )
				throw new ArgumentOutOfRangeException( nameof(BufferCount),
													   $"The minimum number of backbuffers is 1." ) ;
#endif
			_bufferCount = value ;
		}
	}
	
	/// <summary>Whether or not to allow tearing.</summary>
	/// <remarks><b>WARNING</b>:
	/// <para>Be sure to first check for the presence of the <see cref="Feature.AllowTearing"/> flag
	/// before setting this value to ensure it is supported by the hardware.</para>
	/// </remarks>
	public bool AllowTearing {
		get => _allowTearing ;
		set => _allowTearing = value ;
	}
	
	/// <summary>The color data format for the backbuffer(s).</summary>
	public Format BufferFormat {
		get => _clearValue.Format ;
		set => _clearValue = new( value, _clearValue.ClearValueData.Color ) ;
	}
	
	/// <summary>The clear color to clear the backbuffer(s) with.</summary>
	public ColorF ClearColor {
		get => _clearValue.ClearValueData.Color ;
		set => _clearValue = new( _clearValue.Format, value ) ;
	}
	
	// -----------------------------------------------------
	
	public RenderTargetProperties( uint bufferCount = 2, ClearValue clearValue = default ) {
		BufferCount      = bufferCount ;
		_clearValue = clearValue ;
	}
	
	public RenderTargetProperties( uint bufferCount, ColorF clearColor, 
								   Format format = Format.R8G8B8A8_UNORM ) {
		BufferCount = bufferCount ;
		_clearValue = new(format, clearColor) ;
	}
	
	// =====================================================
} ;