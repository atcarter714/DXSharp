#pragma warning disable CS8981, CS1591

#region Using Directives
#endregion

namespace Windows.Win32.Graphics.Direct3D12;

//! TODO:
// Define a "Viewport" struct that works for both
// D3D12 and D3D11 since these are two essentially
// identical structures with the same fields and
// the same layout ...

public partial struct D3D12_VIEWPORT
{
	public D3D12_VIEWPORT( ) {

		this.TopLeftX = 0f;
		this.TopLeftY = 0f;
		this.Width = 0f;
		this.Height = 0f;
		this.MinDepth = 0f;
		this.MaxDepth = 1f;
	}

	public D3D12_VIEWPORT(
		float topLeftX, float topLeftY,
		float width, float height,
		float minDepth = 0f, float maxDepth = 1f ) {

		this.TopLeftX = topLeftX;
		this.TopLeftY = topLeftY;
		this.Width = width;
		this.Height = height;
		this.MinDepth = minDepth;
		this.MaxDepth = maxDepth;
	}

	public D3D12_VIEWPORT(
		float width, float height,
		float minDepth = 0f, float maxDepth = 1f ) {

		this.TopLeftX = 0;
		this.TopLeftY = 0;
		this.Width = width;
		this.Height = height;
		this.MinDepth = minDepth;
		this.MaxDepth = maxDepth;
	}


	public static implicit operator D3D12_VIEWPORT( DXSharp.Viewport vp ) =>
		new( vp.TopLeftX, vp.TopLeftY, vp.Width, vp.Height, vp.MinDepth, vp.MaxDepth );

	public static implicit operator DXSharp.Viewport( D3D12_VIEWPORT vp ) =>
		new( vp.TopLeftX, vp.TopLeftY, vp.Width, vp.Height, vp.MinDepth, vp.MaxDepth );
} ;