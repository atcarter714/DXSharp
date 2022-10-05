#region Using Directives
using System;
using System.Runtime.CompilerServices;

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
			internal unsafe DXGI_MODE_DESC( ModeDescription* pMode )
			{
				fixed ( DXGI_MODE_DESC* pThis = &this )
					*pThis = *((DXGI_MODE_DESC*) pMode);
			}

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

			public static implicit operator DXGI_MODE_DESC( ModeDescription mode ) => MemCopyFrom( mode );
			public static implicit operator ModeDescription( DXGI_MODE_DESC mode ) => ModeDescription.MemCopyFrom( mode );
		};
	}

	internal partial struct DXGI_MODE_DESC1
	{
		internal unsafe DXGI_MODE_DESC1( ModeDescription1* pMode )
		{
			fixed ( DXGI_MODE_DESC1* pThis = &this )
				*pThis = *((DXGI_MODE_DESC1*) pMode);
		}

		public static implicit operator DXGI_MODE_DESC1( ModeDescription1 mode ) => MemCopyFrom( mode );
		public static implicit operator ModeDescription1( DXGI_MODE_DESC1 mode ) => ModeDescription1.MemCopyFrom( mode );

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