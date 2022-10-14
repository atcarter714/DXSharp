
#pragma warning disable CS8981, CS1591

#region Using Directives
using global::System;
using global::System.Diagnostics;
using global::System.Diagnostics.CodeAnalysis;
using global::System.Runtime.CompilerServices;
using global::System.Runtime.InteropServices;
using global::System.Runtime.Versioning;

using Windows.Win32.Graphics.Direct3D11;
using Windows.Win32.Graphics.Direct3D12;

using winmdroot = global::Windows.Win32;
#endregion

namespace Windows.Win32.Graphics.Direct3D11;



internal partial struct D3D11_VIEWPORT
{
	public D3D11_VIEWPORT() {

		this.TopLeftX = 0f;
		this.TopLeftY = 0f;
		this.Width = 0f;
		this.Height = 0f;
		this.MinDepth = 0f;
		this.MaxDepth = 1f;
	}

	internal D3D11_VIEWPORT(
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

	internal D3D11_VIEWPORT(
		float width, float height,
		float minDepth = 0f, float maxDepth = 1f ) {

		this.TopLeftX = 0;
		this.TopLeftY = 0;
		this.Width = width;
		this.Height = height;
		this.MinDepth = minDepth;
		this.MaxDepth = maxDepth;
	}

	internal static ReadOnlySpan<D3D11_VIEWPORT> GetFrom( ID3D11DeviceContext? context ) {
		
		try {
			unsafe {
				uint count = 0x08u;
				var pViewports = stackalloc D3D11_VIEWPORT[ 8 ];
				context?.RSGetViewports( &count, pViewports );

				var sviewports = new ReadOnlySpan<D3D11_VIEWPORT>( pViewports, (int)count );
				return sviewports;
			}
		}
		// Failed:
		catch ( Exception ) { return null; }
	}


	public static implicit operator D3D11_VIEWPORT( DXSharp.Viewport vp ) =>
		new( vp.TopLeftX, vp.TopLeftY, vp.Width, vp.Height, vp.MinDepth, vp.MaxDepth );

	public static implicit operator DXSharp.Viewport( D3D11_VIEWPORT vp ) =>
		new( vp.TopLeftX, vp.TopLeftY, vp.Width, vp.Height, vp.MinDepth, vp.MaxDepth );
};