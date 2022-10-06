#region Using Directives
using System;
using System.Runtime.CompilerServices;

using Windows.Win32.Graphics.Dxgi.Common;
using Format = DXSharp.DXGI.Format;

using DXSharp.DXGI;
#endregion

namespace Windows.Win32.Graphics.Dxgi
{
	namespace Common
	{
		internal partial struct DXGI_SAMPLE_DESC
		{
			internal DXGI_SAMPLE_DESC( SampleDescription desc ) {
				this.Count = desc.Count;
				this.Quality = desc.Quality;
			}

			public static implicit operator SampleDescription( DXGI_SAMPLE_DESC desc ) =>
				new SampleDescription( desc );

			public static implicit operator DXGI_SAMPLE_DESC( SampleDescription desc ) =>
				new DXGI_SAMPLE_DESC( desc );
		};


		internal partial struct DXGI_RATIONAL
		{
			internal unsafe DXGI_RATIONAL( Rational* pRational ) {
				fixed(DXGI_RATIONAL* pThis = &this)
					*pThis = *( (DXGI_RATIONAL *)pRational );
			}
			internal DXGI_RATIONAL( uint numerator ): this(numerator, 1 ) { }
			internal DXGI_RATIONAL( uint numerator, uint denominator ) {
				this.Numerator = numerator;
				this.Denominator = denominator;
			}

			public static implicit operator DXGI_RATIONAL( Rational r ) =>
				new DXGI_RATIONAL( r.Numerator, r.Denominator );

			public static implicit operator Rational( DXGI_RATIONAL r ) =>
				new Rational( r.Numerator, r.Denominator );
		};


		internal partial struct DXGI_MODE_DESC
		{
			internal DXGI_MODE_DESC( in ModeDescription mode ) {
				this.Width = mode.Width;
				this.Height = mode.Height;
				this.RefreshRate = mode.RefreshRate;
				this.Format = (DXGI_FORMAT) mode.Format;
				this.ScanlineOrdering = (DXGI_MODE_SCANLINE_ORDER) mode.ScanlineOrdering;
				this.Scaling = (DXGI_MODE_SCALING) mode.Scaling;
			}

			internal unsafe DXGI_MODE_DESC( ModeDescription* pMode )
			{
				fixed ( DXGI_MODE_DESC* pThis = &this )
					*pThis = *((DXGI_MODE_DESC*) pMode);
			}

			internal DXGI_MODE_DESC( uint width, uint height, Rational refreshRate, Format format = DXSharp.DXGI.Format.R8G8B8A8_UNORM,
				ScanlineOrder scanlineOrdering = ScanlineOrder.Unspecified, ScalingMode scaling = ScalingMode.Centered )
			{
				this.Width = width;
				this.Height = height;
				this.RefreshRate = refreshRate;
				this.Format = (DXGI_FORMAT)format;
				this.ScanlineOrdering = (DXGI_MODE_SCANLINE_ORDER)scanlineOrdering;
				this.Scaling = (DXGI_MODE_SCALING)scaling;
			}

			//! NOTE: These may be useless now ... consider removing
			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			internal unsafe static DXGI_MODE_DESC MemCopyFrom( in ModeDescription mode )
			{
				fixed ( ModeDescription* pMode = &mode )
					return MemCopyFrom( pMode );
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			internal static unsafe DXGI_MODE_DESC MemCopyFrom( ModeDescription* pMode )
			{
				DXGI_MODE_DESC desc = default;
				*(&desc) = *((DXGI_MODE_DESC*) pMode);
				return desc;
			}



			public static implicit operator DXGI_MODE_DESC( ModeDescription mode ) => new DXGI_MODE_DESC( mode );
			public static implicit operator ModeDescription( DXGI_MODE_DESC mode ) => new ModeDescription( mode );
		};
	}

	internal partial struct DXGI_MODE_DESC1
	{
		internal DXGI_MODE_DESC1( in ModeDescription1 mode ) {
			this.Width = mode.Width;
			this.Height = mode.Height;
			this.RefreshRate = mode.RefreshRate;
			this.Format = (DXGI_FORMAT) mode.Format;
			this.ScanlineOrdering = (DXGI_MODE_SCANLINE_ORDER) mode.ScanlineOrdering;
			this.Scaling = (DXGI_MODE_SCALING) mode.Scaling;
			this.Stereo = mode.Stereo;
		}

		internal unsafe DXGI_MODE_DESC1( ModeDescription1* pMode )
		{
			fixed ( DXGI_MODE_DESC1* pThis = &this )
				*pThis = *((DXGI_MODE_DESC1*) pMode);
		}

		internal DXGI_MODE_DESC1( uint width, uint height, Rational refreshRate, Format format = DXSharp.DXGI.Format.R8G8B8A8_UNORM,
			ScanlineOrder scanlineOrdering = ScanlineOrder.Unspecified, ScalingMode scaling = ScalingMode.Centered, bool stereo = false )
		{
			this.Width = width;
			this.Height = height;
			this.RefreshRate = refreshRate;
			this.Format = (DXGI_FORMAT) format;
			this.ScanlineOrdering = (DXGI_MODE_SCANLINE_ORDER) scanlineOrdering;
			this.Scaling = (DXGI_MODE_SCALING) scaling;
			this.Stereo = stereo;
		}


		public static implicit operator DXGI_MODE_DESC1( ModeDescription1 mode ) => new DXGI_MODE_DESC1( mode );
		public static implicit operator ModeDescription1( DXGI_MODE_DESC1 mode ) => new ModeDescription1( mode );

		//! NOTE: May be removed (might be useless now)
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		internal unsafe static DXGI_MODE_DESC1 MemCopyFrom( in ModeDescription1 mode )
		{
			fixed ( ModeDescription1* pMode = &mode )
				return MemCopyFrom( pMode );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		internal static unsafe DXGI_MODE_DESC1 MemCopyFrom( ModeDescription1* pMode )
		{
			DXGI_MODE_DESC1 desc = default;
			*(&desc) = *((DXGI_MODE_DESC1*) pMode);
			return desc;
		}
	};

	internal partial struct DXGI_SWAP_CHAIN_DESC
	{
		internal DXGI_SWAP_CHAIN_DESC( SwapChainDescription desc )
		{
			this.BufferCount = desc.BufferCount;
			this.BufferDesc = desc.BufferDesc;
			this.BufferUsage = (uint)desc.BufferUsage;
			this.OutputWindow = desc.OutputWindow;
			this.Flags = (uint)desc.Flags;
		}
	};
}



// wtf was I thinking lol? this is an enum
//internal partial struct DXGI_SWAP_EFFECT
//{
//	internal DXGI_SWAP_EFFECT( SwapEffect swapEffect )
//	{
//		this. = desc.Count;
//		this.Quality = desc.Quality;
//	}

//	public static implicit operator SwapEffect( DXGI_SWAP_EFFECT desc ) =>
//		new SwapEffect( desc );

//	public static implicit operator DXGI_SWAP_EFFECT( SwapEffect desc ) =>
//		new DXGI_SAMPLE_DESC( desc );
//};