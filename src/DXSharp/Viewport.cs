


namespace DXSharp;


/// <summary>
/// Represents a graphics viewport for DirectX
/// </summary>
/// <remarks>
/// <para><h3><b>NOTE:</b></h3></para> 
/// <para>D3D11 and D3D12 each define their own viewport structures:</para>
/// <para><a href="https://learn.microsoft.com/en-us/windows/win32/api/d3d12/ns-d3d12-d3d12_viewport">D3D12_VIEWPORT</a></para>
/// <para><a href="https://learn.microsoft.com/en-us/windows/win32/api/d3d11/ns-d3d11-d3d11_viewport">D3D11_VIEWPORT</a></para>
/// 
/// As these structures have identical definitions with matching fields, C#/.NET projections
/// of DirectX APIs are using this standardized viewport struct instead of maintaining two
/// identical structures with different names.
/// </remarks>
public struct Viewport
{
	/// <summary>
	/// Creates a new Viewport structure
	/// </summary>
	public Viewport() {

		this.topLeftX = 0f;
		this.topLeftY = 0f;
		this.width = 0f;
		this.height = 0f;
		this.minDepth = 0f;
		this.maxDepth = 1f;
	}

	/// <summary>
	/// Creates a new Viewport structure
	/// </summary>
	/// <param name="topLeftX">Top-left X coordinate</param>
	/// <param name="topLeftY">Top-left Y coordinate</param>
	/// <param name="width">Width of viewport</param>
	/// <param name="height">Height of viewport</param>
	/// <param name="minDepth">Minimum depth value (between 0.0 and 1.0)</param>
	/// <param name="maxDepth">Maximum depth value (between 0.0 and 1.0)</param>
	public Viewport(
		float topLeftX, float topLeftY,
		float width, float height,
		float minDepth = 0f, float maxDepth = 1f ) {

		this.topLeftX = topLeftX;
		this.topLeftY = topLeftY;
		this.width = width;
		this.height = height;
		this.minDepth = minDepth;
		this.maxDepth = maxDepth;
	}

	/// <summary>
	/// Creates a new Viewport structure
	/// </summary>
	/// <param name="width">Width of viewport</param>
	/// <param name="height">Height of viewport</param>
	/// <param name="minDepth">Minimum depth value (between 0.0 and 1.0)</param>
	/// <param name="maxDepth">Maximum depth value (between 0.0 and 1.0)</param>
	/// <remarks>
	/// Assumes 0.0, 0.0 for the top-left X and Y coordinates
	/// </remarks>
	public Viewport(
		float width, float height,
		float minDepth = 0f, float maxDepth = 1f ) {

		this.topLeftX = 0;
		this.topLeftY = 0;
		this.width = width;
		this.height = height;
		this.minDepth = minDepth;
		this.maxDepth = maxDepth;
	}

	float topLeftX;
	float topLeftY;
	float width;
	float height;
	float minDepth;
	float maxDepth;

	/// <summary>
	/// The top-left X coordinate
	/// </summary>
	public float TopLeftX { get => topLeftX; set => topLeftX = value; }
	/// <summary>
	/// The top-left Y coordinate
	/// </summary>
	public float TopLeftY { get => topLeftY; set => topLeftY = value; }
	/// <summary>
	/// The width of the viewport
	/// </summary>
	public float Width { get => width; set => width = value; }
	/// <summary>
	/// The height of the viewport
	/// </summary>
	public float Height { get => height; set => height = value; }
	/// <summary>
	/// The minimum depth value (between 0.0 and 1.0)
	/// </summary>
	public float MinDepth { get => minDepth; set => minDepth = value; }
	/// <summary>
	/// The maximum depth value (between 0.0 and 1.0)
	/// </summary>
	public float MaxDepth { get => maxDepth; set => maxDepth = value; }



	internal static unsafe void setAllMembers( Viewport* pViewport,
		float topLeftX, float topLeftY,
		float width, float height,
		float minDepth = 0f, float maxDepth = 1f ) {

		pViewport->topLeftX = topLeftX;
		pViewport->topLeftY = topLeftY;
		pViewport->width = width;
		pViewport->height = height;
		pViewport->minDepth = minDepth;
		pViewport->maxDepth = maxDepth;
	}

};