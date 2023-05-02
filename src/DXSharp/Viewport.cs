


namespace DXSharp;
// ReSharper disable ConvertToAutoPropertyWhenPossible

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

		this._topLeftX = 0f;
		this._topLeftY = 0f;
		this._width = 0f;
		this._height = 0f;
		this._minDepth = 0f;
		this._maxDepth = 1f;
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

		this._topLeftX = topLeftX;
		this._topLeftY = topLeftY;
		this._width = width;
		this._height = height;
		this._minDepth = minDepth;
		this._maxDepth = maxDepth;
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

		this._topLeftX = 0;
		this._topLeftY = 0;
		this._width = width;
		this._height = height;
		this._minDepth = minDepth;
		this._maxDepth = maxDepth;
	}

	float _topLeftX;
	float _topLeftY;
	float _width;
	float _height;
	float _minDepth;
	float _maxDepth;

	/// <summary>
	/// The top-left X coordinate
	/// </summary>
	public float TopLeftX { get => _topLeftX; set => _topLeftX = value; }
	/// <summary>
	/// The top-left Y coordinate
	/// </summary>
	public float TopLeftY { get => _topLeftY; set => _topLeftY = value; }
	/// <summary>
	/// The width of the viewport
	/// </summary>
	public float Width { get => _width; set => _width = value; }
	/// <summary>
	/// The height of the viewport
	/// </summary>
	public float Height { get => _height; set => _height = value; }
	/// <summary>
	/// The minimum depth value (between 0.0 and 1.0)
	/// </summary>
	public float MinDepth { get => _minDepth; set => _minDepth = value; }
	/// <summary>
	/// The maximum depth value (between 0.0 and 1.0)
	/// </summary>
	public float MaxDepth { get => _maxDepth; set => _maxDepth = value; }


	/// <summary>
	/// Write data to all fields of the Viewport structure
	/// </summary>
	/// <param name="pViewport">The viewport pointer</param>
	/// <param name="topLeftX">The viewport Top-Left (X)</param>
	/// <param name="topLeftY">The viewport Top-Left (Y)</param>
	/// <param name="width">The viewport width</param>
	/// <param name="height">The viewport height</param>
	/// <param name="minDepth">The min depth</param>
	/// <param name="maxDepth">The max depth</param>
	internal static unsafe void SetAllMembers( Viewport* pViewport,
		float topLeftX, float topLeftY,
		float width, float height,
		float minDepth = 0f, float maxDepth = 1f ) {

		pViewport->_topLeftX = topLeftX;
		pViewport->_topLeftY = topLeftY;
		pViewport->_width = width;
		pViewport->_height = height;
		pViewport->_minDepth = minDepth;
		pViewport->_maxDepth = maxDepth;
	}

};

// ReSharper restore ConvertToAutoPropertyWhenPossible
