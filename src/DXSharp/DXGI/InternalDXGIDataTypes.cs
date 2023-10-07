#region Using Directives

/* Unmerged change from project 'DXSharp (net7.0-windows10.0.22621.0)'
Before:
using System;
After:
using DXSharp.DXGI;
using DXSharp.Windows;

using System;
*/
using DXSharp.DXGI;

using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dxgi.Common;

using Format = DXSharp.DXGI.Format;
#endregion

namespace Windows.Win32.Graphics.Dxgi
{
	namespace Common
	{
		public partial struct DXGI_SAMPLE_DESC
		{
			public DXGI_SAMPLE_DESC( SampleDescription desc ) {
				this.Count = desc.Count;
				this.Quality = desc.Quality;
			}

			public DXGI_SAMPLE_DESC( uint count, uint quality ) {
				this.Count = count;
				this.Quality = quality;
			}

			public DXGI_SAMPLE_DESC( (uint count, uint quality) values ) {
				this.Count = values.count;
				this.Quality = values.quality;
			}



			public static implicit operator SampleDescription( DXGI_SAMPLE_DESC desc ) =>
				new( desc );

			public static implicit operator DXGI_SAMPLE_DESC( SampleDescription desc ) =>
				new( desc );

			public static implicit operator DXGI_SAMPLE_DESC(
				(uint count, uint quality) values ) => new( values );
		};


		public partial struct DXGI_RATIONAL
		{
			public unsafe DXGI_RATIONAL( Rational* pRational ) {
				fixed( DXGI_RATIONAL* pThis = &this )
					*pThis = *((DXGI_RATIONAL*)pRational);
			}
			public DXGI_RATIONAL( uint numerator ) : this( numerator, 1 ) { }
			public DXGI_RATIONAL( uint numerator, uint denominator ) {
				this.Numerator = numerator;
				this.Denominator = denominator;
			}

			public DXGI_RATIONAL( (uint numerator, uint denominator) values ) {
				this.Numerator = values.numerator;
				this.Denominator = values.denominator;
			}



			public static implicit operator DXGI_RATIONAL( Rational r ) =>
				new( r.Numerator, r.Denominator );

			public static implicit operator Rational( DXGI_RATIONAL r ) =>
				new( r.Numerator, r.Denominator );

			public static implicit operator DXGI_RATIONAL(
				(uint numerator, uint denominator) values ) => new( values );
		};


		public partial struct DXGI_MODE_DESC
		{
			public DXGI_MODE_DESC( in ModeDescription mode ) {
				this.Width = mode.Width;
				this.Height = mode.Height;
				this.RefreshRate = mode.RefreshRate;
				this.Format = (DXGI_FORMAT)mode.Format;
				this.ScanlineOrdering = (DXGI_MODE_SCANLINE_ORDER)mode.ScanlineOrdering;
				this.Scaling = (DXGI_MODE_SCALING)mode.Scaling;
			}

			public unsafe DXGI_MODE_DESC( ModeDescription* pMode ) {
				fixed( DXGI_MODE_DESC* pThis = &this )
					*pThis = *((DXGI_MODE_DESC*)pMode);
			}

			public DXGI_MODE_DESC(
				uint width, uint height, Rational refreshRate, Format format = DXSharp.DXGI.Format.R8G8B8A8_UNORM,
				ScanlineOrder scanlineOrdering = ScanlineOrder.Unspecified, ScalingMode scaling = ScalingMode.Centered ) {
				this.Width = width;
				this.Height = height;
				this.RefreshRate = refreshRate;
				this.Format = (DXGI_FORMAT)format;
				this.ScanlineOrdering = (DXGI_MODE_SCANLINE_ORDER)scanlineOrdering;
				this.Scaling = (DXGI_MODE_SCALING)scaling;
			}

			public DXGI_MODE_DESC(
				uint width, uint height, DXGI_RATIONAL refreshRate,
				DXGI_FORMAT format = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM,
				DXGI_MODE_SCANLINE_ORDER scanlineOrdering = DXGI_MODE_SCANLINE_ORDER.DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED,
				DXGI_MODE_SCALING scaling = DXGI_MODE_SCALING.DXGI_MODE_SCALING_CENTERED ) {
				this.Width = width;
				this.Height = height;
				this.RefreshRate = refreshRate;
				this.Format = format;
				this.ScanlineOrdering = scanlineOrdering;
				this.Scaling = scaling;
			}



			public static implicit operator DXGI_MODE_DESC( ModeDescription mode ) => new( mode );
			public static implicit operator ModeDescription( DXGI_MODE_DESC mode ) => new( mode );
			public static explicit operator DXGI_MODE_DESC( DXGI_MODE_DESC1 mode ) {
				unsafe {
					var p = (DXGI_MODE_DESC*)&mode;
					DXGI_MODE_DESC result = default;
					result = *p;
					return result;
				}
			}
		};
	}


	public partial struct DXGI_MODE_DESC1
	{
		public DXGI_MODE_DESC1( in ModeDescription1 mode ) {
			this.Width = mode.Width;
			this.Height = mode.Height;
			this.RefreshRate = mode.RefreshRate;
			this.Format = (DXGI_FORMAT)mode.Format;
			this.ScanlineOrdering = (DXGI_MODE_SCANLINE_ORDER)mode.ScanlineOrdering;
			this.Scaling = (DXGI_MODE_SCALING)mode.Scaling;
			this.Stereo = mode.Stereo;
		}

		public unsafe DXGI_MODE_DESC1( ModeDescription1* pMode ) {
			fixed( DXGI_MODE_DESC1* pThis = &this )
				*pThis = *((DXGI_MODE_DESC1*)pMode);
		}

		public DXGI_MODE_DESC1( uint width, uint height, Rational refreshRate, Format format = DXSharp.DXGI.Format.R8G8B8A8_UNORM,
			ScanlineOrder scanlineOrdering = ScanlineOrder.Unspecified, ScalingMode scaling = ScalingMode.Centered, bool stereo = false ) {
			this.Width = width;
			this.Height = height;
			this.RefreshRate = refreshRate;
			this.Format = (DXGI_FORMAT)format;
			this.ScanlineOrdering = (DXGI_MODE_SCANLINE_ORDER)scanlineOrdering;
			this.Scaling = (DXGI_MODE_SCALING)scaling;
			this.Stereo = stereo;
		}

		public DXGI_MODE_DESC1( in DXGI_MODE_DESC mode ) : this( mode.Width, mode.Height, mode.RefreshRate,
				mode.Format, mode.ScanlineOrdering, mode.Scaling, false ) { }

		public DXGI_MODE_DESC1(
			uint width, uint height, DXGI_RATIONAL refreshRate,
			DXGI_FORMAT format = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM,
			DXGI_MODE_SCANLINE_ORDER scanlineOrdering = DXGI_MODE_SCANLINE_ORDER.DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED,
			DXGI_MODE_SCALING scaling = DXGI_MODE_SCALING.DXGI_MODE_SCALING_CENTERED, bool stereo = false ) {
			this.Width = width;
			this.Height = height;
			this.RefreshRate = refreshRate;
			this.Format = format;
			this.ScanlineOrdering = scanlineOrdering;
			this.Scaling = scaling;
			this.Stereo = stereo;
		}


		public static implicit operator DXGI_MODE_DESC1( ModeDescription mode ) =>
			new( mode.Width, mode.Height, mode.RefreshRate, mode.Format, mode.ScanlineOrdering, mode.Scaling, false );
		public static implicit operator DXGI_MODE_DESC1( DXGI_MODE_DESC mode ) =>
			new( mode.Width, mode.Height, mode.RefreshRate,
				mode.Format, mode.ScanlineOrdering, mode.Scaling, false );

		public static implicit operator DXGI_MODE_DESC1( ModeDescription1 mode ) => new( mode );
		public static implicit operator ModeDescription1( DXGI_MODE_DESC1 mode ) => new( mode );

	};


	public partial struct DXGI_SWAP_CHAIN_DESC
	{
		public DXGI_SWAP_CHAIN_DESC( in SwapChainDescription desc ) {
			this.BufferDesc = desc.BufferDesc;
			this.SampleDesc = desc.SampleDesc;
			this.BufferUsage = (DXGI_USAGE)desc.BufferUsage;
			this.BufferCount = desc.BufferCount;
			this.OutputWindow = desc.OutputWindow;
			this.Windowed = desc.Windowed;
			this.SwapEffect = (DXGI_SWAP_EFFECT)desc.SwapEffect;
			this.Flags = (uint)desc.Flags;
		}

		public unsafe DXGI_SWAP_CHAIN_DESC( SwapChainDescription* pDesc ) {
			fixed( DXGI_SWAP_CHAIN_DESC* pThis = &this ) {
				*pThis = *((DXGI_SWAP_CHAIN_DESC*)pDesc);
			}
		}

		public DXGI_SWAP_CHAIN_DESC( ModeDescription bufferDesc, SampleDescription sampleDesc, Usage bufferUsage,
			uint bufferCount, HWND outputWindow, bool windowed, SwapEffect swapEffect, SwapChainFlags flags ) {
			this.BufferDesc = bufferDesc;
			this.SampleDesc = (DXGI_SAMPLE_DESC)sampleDesc;
			this.BufferUsage = (DXGI_USAGE)bufferUsage;
			this.BufferCount = bufferCount;
			this.OutputWindow = outputWindow;
			this.Windowed = windowed;
			this.SwapEffect = (DXGI_SWAP_EFFECT)swapEffect;
			this.Flags = (uint)flags;
		}

		public DXGI_SWAP_CHAIN_DESC( DXGI_MODE_DESC bufferDesc, DXGI_SAMPLE_DESC sampleDesc, Usage bufferUsage,
	uint bufferCount, HWND outputWindow, bool windowed, DXGI_SWAP_EFFECT swapEffect, DXGI_SWAP_CHAIN_FLAG flags ) {
			this.BufferDesc = bufferDesc;
			this.SampleDesc = sampleDesc;
			this.BufferUsage = (DXGI_USAGE)bufferUsage;
			this.BufferCount = bufferCount;
			this.OutputWindow = outputWindow;
			this.Windowed = windowed;
			this.SwapEffect = swapEffect;
			this.Flags = (uint)flags;
		}


		public static implicit operator DXGI_SWAP_CHAIN_DESC( SwapChainDescription desc ) => desc.InternalValue;
		public static implicit operator SwapChainDescription( DXGI_SWAP_CHAIN_DESC desc ) => new( desc );

	};


	public partial struct DXGI_SWAP_CHAIN_DESC1
	{
		public DXGI_SWAP_CHAIN_DESC1( in SwapChainDescription1 desc ) {
			this.Width = desc.Width;
			this.Height = desc.Height;
			this.Format = (DXGI_FORMAT)desc.Format;
			this.Stereo = desc.Stereo;
			this.SampleDesc = desc.SampleDesc;
			this.BufferUsage = (DXGI_USAGE)desc.BufferUsage;
			this.BufferCount = desc.BufferCount;
			this.Scaling = (DXGI_SCALING)desc.Scaling;
			this.SwapEffect = (DXGI_SWAP_EFFECT)desc.SwapEffect;
			this.AlphaMode = (DXGI_ALPHA_MODE)desc.AlphaMode;
			this.Flags = (DXGI_SWAP_CHAIN_FLAG)(uint)desc.Flags;
		}

		public unsafe DXGI_SWAP_CHAIN_DESC1( SwapChainDescription1* pDesc ) {
			fixed( DXGI_SWAP_CHAIN_DESC1* pThis = &this ) {
				*pThis = *((DXGI_SWAP_CHAIN_DESC1*)pDesc);
			}
		}

		public DXGI_SWAP_CHAIN_DESC1(
			uint width, uint height, Format format, bool stereo, SampleDescription sampleDesc,
			Usage bufferUsage, uint bufferCount, Scaling scaling, SwapEffect swapEffect,
			AlphaMode alphaMode, SwapChainFlags flags = default ) {
			this.Width = width;
			this.Height = height;
			this.Format = (DXGI_FORMAT)format;
			this.Stereo = stereo;
			this.SampleDesc = sampleDesc;
			this.BufferUsage = (DXGI_USAGE)bufferUsage;
			this.BufferCount = bufferCount;
			this.Scaling = (DXGI_SCALING)scaling;
			this.SwapEffect = (DXGI_SWAP_EFFECT)swapEffect;
			this.AlphaMode = (DXGI_ALPHA_MODE)alphaMode;
			this.Flags = (DXGI_SWAP_CHAIN_FLAG)(uint)flags;
		}

		public static implicit operator DXGI_SWAP_CHAIN_DESC1( SwapChainDescription1 desc ) => desc._InternalValue;
		public static implicit operator SwapChainDescription1( DXGI_SWAP_CHAIN_DESC1 desc ) => new( desc );
	}
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