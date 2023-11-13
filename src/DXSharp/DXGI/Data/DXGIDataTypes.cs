#region Using Directives

using System.Buffers ;
using System.Collections ;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts ;
using System.Numerics ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices;
using Windows.Win32 ;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi;
using Windows.Win32.Graphics.Dxgi.Common;
using DXSharp.Windows.Win32 ;
using DXSharp.Windows.Win32.Helpers ;
using winMD = Windows.Win32.Foundation ;

using static DXSharp.InteropUtils ;
using DXGI_MODE_DESC = Windows.Win32.Graphics.Dxgi.Common.DXGI_MODE_DESC;
using DXGI_MODE_DESC1 = Windows.Win32.Graphics.Dxgi.DXGI_MODE_DESC1;
using DXGI_SAMPLE_DESC = Windows.Win32.Graphics.Dxgi.Common.DXGI_SAMPLE_DESC;
using DXGI_SWAP_EFFECT = Windows.Win32.Graphics.Dxgi.DXGI_SWAP_EFFECT;
#endregion

namespace DXSharp.DXGI ;


// ---------------------------------------------------------------------------------------
// DXGI Enumerations ::
// ---------------------------------------------------------------------------------------

// Issue: the auto-documentation had "D2D1_ALPHA_MODE" instead of DXGI_ALPHA_MODE
// but the link was actually correct, oddly enough ...


[StructLayout( LayoutKind.Sequential ),
 EquivalentOf(typeof(DXGI_SURFACE_DESC))]
public struct SurfaceDescription {
	public uint Width, 
				Height ;
	public Format            Format ;
	public SampleDescription SampleDesc ;
	public SurfaceDescription( uint              width, uint height, Format format,
							   SampleDescription sampleDesc ) {
		Width  = width ; Height      = height ;
		Format = format ; SampleDesc = sampleDesc ;
	}
	
	public static implicit operator SurfaceDescription( in DXGI_SURFACE_DESC desc ) =>
		new( desc.Width, desc.Height, (Format)desc.Format, desc.SampleDesc ) ;
	public static unsafe implicit operator DXGI_SURFACE_DESC( in SurfaceDescription desc ) =>
		new DXGI_SURFACE_DESC {
			Width  = desc.Width, Height                   = desc.Height,
			Format = (DXGI_FORMAT)desc.Format, SampleDesc = desc.SampleDesc, } ;
} ;


[StructLayout( LayoutKind.Sequential ),
 EquivalentOf(typeof(DXGI_MAPPED_RECT))]
public struct MappedRect { 
	public int Pitch ; public nint pBits ;
	public MappedRect( int pitch, nint bits ) {
		Pitch = pitch ; pBits = bits ;
	}
	public MappedRect( in DXGI_MAPPED_RECT rect ) {
		Pitch = rect.Pitch ;
		unsafe { pBits = (nint)rect.pBits ; }
	}
	public static implicit operator MappedRect( in DXGI_MAPPED_RECT rect ) => new( rect ) ;
	public static unsafe implicit operator DXGI_MAPPED_RECT( in MappedRect rect ) => new DXGI_MAPPED_RECT {
		Pitch = rect.Pitch, pBits = (byte*)rect.pBits, } ;
} ;




// ---------------------------------------------------------------------------------------
// DXGI Structures ::
// ---------------------------------------------------------------------------------------


[DebuggerDisplay( "[ {M11}, {M12} ]\n" +
				  "[ {M21}, {M22} ]\n" +
				  "[ {M31}, {M32} ]" ) ]
[StructLayout( LayoutKind.Sequential, Size = SizeInBytes ),
 EquivalentOf( typeof( DXGI_MATRIX_3X2_F ) )]
public struct Matrix3x2F: IEquatable< Matrix3x2F >, 
						  IEquatable<Matrix3x2> {
	static Matrix3x2F( ) {
#if DEBUG || DEV_BUILD
		System.Diagnostics.Debug.Assert( SizeInBytes == Marshal.SizeOf<DXGI_MATRIX_3X2_F >() 
					  && SizeInBytes == Marshal.SizeOf<DXGI_MATRIX_3X2_F>( )) ;
#endif
	}
	public const int ElementCount = 6,
					 SizeInBytes  = ElementCount * sizeof( float ) ;
	
	unsafe fixed float _buffer[ 6 ] ;
	
	public unsafe ref float M11 => ref _buffer[ 0 ] ;
	public unsafe ref float M12 => ref _buffer[ 1 ] ;
	public unsafe ref float M21 => ref _buffer[ 2 ] ;
	public unsafe ref float M22 => ref _buffer[ 3 ] ;
	public unsafe ref float M31 => ref _buffer[ 4 ] ;
	public unsafe ref float M32 => ref _buffer[ 5 ] ;
	
	
	public Matrix3x2F( float m11 = 0f, float m12 = 0f, float m21 = 0f,
					   float m22 = 0f, float m31 = 0f, float m32 = 0f ) {
		this.M11 = m11 ; this.M12 = m12 ;
		this.M21 = m21 ; this.M22 = m22 ;
		this.M31 = m31 ; this.M32 = m32 ;
	}
	
	public Matrix3x2F( in System.Numerics.Matrix3x2 matrix ) {
		Unsafe.SkipInit( out this ) ;
		this = FromMatrix3x2( matrix ) ;
	}
	
	public Matrix3x2F( params float[ ] values ) {
		unsafe {
			fixed( void* pThis = &this, pValues = &values[ 0 ] ) {
				Matrix3x2F* pMatrix = (Matrix3x2F *)pThis ;
				Matrix3x2F* pSrcData = (Matrix3x2F *)pValues ;
				*pMatrix = *pSrcData ;
			}
		}
		
	}

	public Matrix3x2F( in Span< float > values ) {
		unsafe {
			fixed( void* pThis = &this, pValues = &values[ 0 ] ) {
				Matrix3x2F* pMatrix = (Matrix3x2F *)pThis ;
				Matrix3x2F* pSrcData = (Matrix3x2F *)pValues ;
				*pMatrix = *pSrcData ;
			}
		}
	}
	
	
	
	public unsafe ref float this[ int index ] {
		get {
			if( index < 0 || index >= ElementCount )
				throw new IndexOutOfRangeException( ) ;
			fixed( float* pThis = &_buffer[ 0 ] )
				return ref pThis[ index ] ;
		}
	}
	
	[Pure] System.Numerics.Matrix3x2 ToMatrix3x2( ) {
		unsafe {
			Unsafe.SkipInit( out Matrix3x2 matrix ) ;
			fixed ( float* pSrc = &_buffer[ 0 ] ) {
				Matrix3x2* pSrcMatrix = (Matrix3x2 *)pSrc ;
				Matrix3x2* pDst       = &matrix ;
				*pDst = *pSrcMatrix ;
			}
			return matrix ;
		}
	}
	
	[UnscopedRef] public unsafe Span< float > AsSpan( ) {
		fixed( float* pThis = &_buffer[ 0 ] )
			return new( pThis, 6 ) ;
	}
	
	[Pure] public static Matrix3x2F FromMatrix3x2( in Matrix3x2 matrix ) {
		unsafe {
			Unsafe.SkipInit( out Matrix3x2F result ) ;
			fixed ( Matrix3x2* pSrc = &matrix ) {
				Matrix3x2* pDst = (Matrix3x2 *)&result ;
				*pDst = *pSrc ;
			}
			return result ;
		}
	}
	
	
	public override int GetHashCode( ) => 
		HashCode.Combine( M11, M12, M21, M22, M31, M32 ) ;
	
	public override string ToString( ) =>  $"[ {M11}, {M12} ]\n" +
										   $"[ {M21}, {M22} ]\n" +
										   $"[ {M31}, {M32} ]" ;
	
	public bool Equals( Matrix3x2 other ) => other == (Matrix3x2)this ;
	
	public bool Equals( Matrix3x2F other ) => Mathf.Approximately( other.M11, M11 )
											  && Mathf.Approximately( other.M12, M12 )
											  && Mathf.Approximately( other.M21, M21 )
											  && Mathf.Approximately( other.M22, M22 )
											  && Mathf.Approximately( other.M31, M31 )
											  && Mathf.Approximately( other.M32, M32 ) ;
	
	public override bool Equals( object? obj ) => obj is Matrix3x2F other && Equals( other ) || 
												 obj is Matrix3x2   other2 && Equals( other2 ) ;


	public static implicit operator Matrix3x2F( in System.Numerics.Matrix3x2 matrix ) => FromMatrix3x2( matrix ) ;
	public static implicit operator System.Numerics.Matrix3x2( in Matrix3x2F matrix ) => matrix.ToMatrix3x2( ) ;
	
	public static bool operator ==( in Matrix3x2F left, in Matrix3x2F right ) => left.Equals( right ) ;
	public static bool operator !=( in Matrix3x2F left, in Matrix3x2F right ) => !left.Equals( right ) ;
	public static bool operator ==( in Matrix3x2F left, in Matrix3x2 right ) => left.Equals( right ) ;
	public static bool operator !=( in Matrix3x2F left, in Matrix3x2 right ) => !left.Equals( right ) ;
	public static bool operator ==( in Matrix3x2 left, in Matrix3x2F right ) => right.Equals( left ) ;
	public static bool operator !=( in Matrix3x2 left, in Matrix3x2F right ) => !right.Equals( left ) ;
} ;



/// <summary>Represents a rational number.</summary>
/// <remarks>
/// <para>This structure is a member of the 
/// <a href="https://docs.microsoft.com/previous-versions/windows/desktop/legacy/bb173064(v=vs.85)">DXG.ModeDescription</a> structure.
/// The <b>DXGI_RATIONAL</b> structure operates under the following rules: </para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgicommon/ns-dxgicommon-dxgi_rational#">Read more on docs.microsoft.com</a>.</para>
/// <para>The DXGI_RATIONAL structure operates under the following rules:</para>
/// <para>0/0 is legal and will be interpreted as 0/1.</para>
/// <para>0/anything is interpreted as zero.</para>
/// <para>If you are representing a whole number, the denominator should be 1.</para>
/// 
/// </remarks>
[DebuggerDisplay( "Fraction: {numerator}/{denominator} (Float: {AsFloat}f)" )]
public struct Rational: IEquatable< Rational > {
	/// <summary>
	/// A Rational with a value of zero
	/// </summary>
	public static readonly Rational Zero = (0x00u, 0x01u);

	/// <summary>
	/// Gets the rational value as a float
	/// </summary>
	[DebuggerBrowsable( DebuggerBrowsableState.Never )]
	public float AsFloat => denominator == 0 ? 0f : numerator / denominator;



	internal Rational( in DXGI_RATIONAL rational ) {
		this.numerator = rational.Numerator;
		this.denominator = rational.Denominator;
	}

	internal unsafe Rational( DXGI_RATIONAL* pRational ) {
		fixed( Rational* pThis = &this )
			*pThis = *((Rational*)pRational);
	}

	/// <summary>
	/// Creates a new rational value
	/// </summary>
	public Rational() {
		this.numerator = 0;
		this.denominator = 1;
	}

	/// <summary>
	/// Creates a new rational value
	/// </summary>
	/// <param name="numerator">
	/// The numerator value (denominator will be set to 1)
	/// </param>
	public Rational( uint numerator ) {
		this.numerator = numerator;
		this.denominator = 1;
	}

	/// <summary>
	/// Creates a new rational value
	/// </summary>
	/// <param name="numerator">The numerator value</param>
	/// <param name="denominator">The denominator value</param>
	public Rational( uint numerator, uint denominator ) {
#if DEBUG || !STRIP_CHECKS
		if( denominator == 0 )
			throw new ArgumentOutOfRangeException( nameof( denominator ),
				"Rationals should not have a denominator of zero!" );
#endif

		this.numerator = numerator;
		this.denominator = 1;
	}

	/// <summary>
	/// Creates a new Rational value
	/// </summary>
	/// <param name="values">Tuple holding numerator and denominator values</param>
	public Rational( (uint numerator, uint denominator) values ) {
#if DEBUG || !STRIP_CHECKS
		if( values.denominator == 0 )
			throw new ArgumentOutOfRangeException( "denominator",
				"Rationals should not have a denominator of zero!" );
#endif

		this.numerator = values.numerator;
		this.denominator = values.denominator;
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	uint numerator;
	[DebuggerBrowsable( DebuggerBrowsableState.Never )]
	uint denominator;



	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> An unsigned integer value representing the top of the rational number.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgicommon/ns-dxgicommon-dxgi_rational#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint Numerator { get => numerator; set => numerator = value; }

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> An unsigned integer value representing the bottom of the rational number.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgicommon/ns-dxgicommon-dxgi_rational#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint Denominator { get => denominator; set => denominator = value; }





	/// <summary>
	/// Reduces the rational/fraction value
	/// </summary>
	public void Reduce() {
		uint k = GDC( this );
		numerator /= k;
		denominator /= k;
	}

	/// <summary>
	/// Gets the reduced value of this rational/fraction value
	/// </summary>
	/// <returns>Reduced rationa/fraction value</returns>
	public Rational Reduced() {
		var copy = this;
		copy.Reduce();
		return copy;
	}

	/// <summary>
	/// Determines if the given object and this value are equal
	/// </summary>
	/// <param name="obj">object to compare to</param>
	/// <returns>True if equal, otherwise false</returns>
	public override bool Equals( [NotNullWhen( true )] object? obj ) =>
		obj is Rational r && Equals( r );

	/// <summary>
	/// Determines if the given rational value and this value are equal
	/// </summary>
	/// <param name="other">Rational value to compare to</param>
	/// <returns>True if equal, otherwise false</returns>
	public bool Equals( Rational other ) =>
		(other.numerator == 0 && this.numerator == 0) ||
		(other.denominator == 0 && this.denominator == 0) ||
		(other.numerator == this.numerator && other.denominator == this.denominator);

	/// <summary>
	/// Gets the 32-bit hash code of this rational value
	/// </summary>
	/// <returns>32-bit hash code</returns>
	public override int GetHashCode() => (numerator, denominator).GetHashCode();



	/// <summary>
	/// Finds the greatest common denominator of a rational value
	/// </summary>
	/// <param name="r">A rational/fraction value</param>
	/// <returns>Greatest common denominator</returns>
	/// <exception cref="PerformanceException">
	/// Thrown in DEBUG build if the search for greatest common 
	/// denominator takes an unreasonably long amount of time 
	/// </exception>
	public static uint GDC( Rational r ) {
		uint n = r.numerator, m = r.denominator;

		if( n < m ) {
			(m, n) = (n, m);
		}

#if DEBUG || !STRIP_CHECKS
		uint count = 0x00;
		const uint MAX_ITERATIONS = 100000;
#endif
		while( m > 0 ) {
			uint tmp = n % m;
			n = m;
			m = tmp;

#if DEBUG || !STRIP_CHECKS
			if( ++count > MAX_ITERATIONS ) {
				throw new PerformanceException( $"Rational.GDC(): " +
					$"Algorithm has run over {MAX_ITERATIONS} iterations to determine greatest common demoninator. " +
					$"Consider expressing this value in a more reduced form or check to ensure it is valid." );
			}
#endif
		}

		return n;
	}



	/// <summary>
	/// Converts a tuple of uints to Rational
	/// </summary>
	/// <param name="values">Value tuple of two uint values</param>
	public static implicit operator Rational( (uint numerator, uint denominator) values ) => new( values );

	/// <summary>
	/// Converts an unsigned value into rational form
	/// </summary>
	/// <param name="value">A whole, unsigned value</param>
	public static implicit operator Rational( uint value ) => new( value );

	public static bool operator ==( Rational a, Rational b ) => a.Equals( b );
	public static bool operator !=( Rational a, Rational b ) => !a.Equals( b );
	public static bool operator ==( Rational a, uint b ) => a.Equals( b );
	public static bool operator !=( Rational a, uint b ) => !a.Equals( b );
	public static bool operator ==( uint a, Rational b ) => a.Equals( b );
	public static bool operator !=( uint a, Rational b ) => !a.Equals( b );
} ;


/// <summary>Describes multi-sampling parameters for a resource.</summary>
/// <remarks>
/// <para>This structure is a member of the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ns-dxgi1_2-dxgi_swap_chain_desc1">DXGI_SWAP_CHAIN_DESC1</a> structure. The default sampler mode, with no anti-aliasing, has a count of 1 and a quality level of 0. If multi-sample antialiasing is being used, all bound render targets and depth buffers must have the same sample counts and quality levels. </para>
/// <para>This doc was truncated.</para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgicommon/ns-dxgicommon-dxgi_sample_desc#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
public struct SampleDescription {
	public static readonly SampleDescription Default = new( 1, 0 ) ;
	
	internal SampleDescription( in DXGI_SAMPLE_DESC desc ) {
		this.count = desc.Count;
		this.quality = desc.Quality;
	}

	internal unsafe SampleDescription( DXGI_SAMPLE_DESC* pDesc ) {
		fixed( SampleDescription* pThis = &this ) {
			*pThis = *((SampleDescription*)pDesc);
		}
	}

	/// <summary>
	/// Creates a new SampleDescription
	/// </summary>
	/// <param name="count">Number of multisamples per pixel</param>
	/// <param name="quality">The image quality level</param>
	public SampleDescription( uint count, uint quality ) {
		this.count = count;
		this.quality = quality;
	}

	/// <summary>
	/// Creates a new SampleDescription
	/// </summary>
	/// <param name="values">Tuple holding count and quality values</param>
	public SampleDescription( (uint count, uint quality) values ) {
		this.count = values.count;
		this.quality = values.quality;
	}

	[DebuggerBrowsable( DebuggerBrowsableState.Never )] uint count, quality ;

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The number of multisamples per pixel.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgicommon/ns-dxgicommon-dxgi_sample_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint Count { get => count; set => count = value; }

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The image quality level. The higher the quality, the lower the performance. The valid range is between zero and one less than the level returned by <a href="https://docs.microsoft.com/windows/desktop/api/d3d10/nf-d3d10-id3d10device-checkmultisamplequalitylevels">ID3D10Device::CheckMultisampleQualityLevels</a> for Direct3D 10 or <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/nf-d3d11-id3d11device-checkmultisamplequalitylevels">ID3D11Device::CheckMultisampleQualityLevels</a> for Direct3D 11. For Direct3D 10.1 and Direct3D 11, you can use two special quality level values. For more information about these quality level values, see Remarks.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgicommon/ns-dxgicommon-dxgi_sample_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint Quality { get => quality; set => quality = value; }

	
	public static implicit operator SampleDescription(
		(uint count, uint quality) values ) => new( values.count, values.quality );

	public static implicit operator (uint count, uint quality)( SampleDescription sDesc ) =>
		(count: sDesc.Count, quality: sDesc.Quality);
} ;


//typedef struct DXGI_SWAP_CHAIN_DESC
//{
//	DXGI_MODE_DESC BufferDesc;
//	DXGI_SAMPLE_DESC SampleDesc;
//	DXGI_USAGE BufferUsage;
//	UINT BufferCount;
//	HWND OutputWindow;
//	BOOL Windowed;
//	DXGI_SWAP_EFFECT SwapEffect;
//	UINT Flags;
//} DXGI_SWAP_CHAIN_DESC;

/// <summary>Describes a swap chain.</summary>
/// <remarks>
/// <para>This structure is used by the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiswapchain-getdesc">GetDesc</a> and <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgifactory-createswapchain">CreateSwapChain</a> methods. In full-screen mode, there is a dedicated front buffer; in windowed mode, the desktop is the front buffer. If you create a swap chain with one buffer, specifying <b>DXGI_SWAP_EFFECT_SEQUENTIAL</b> does not cause the contents of the single buffer to be swapped with the front buffer. For performance information about flipping swap-chain buffers in full-screen application, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/d3d10-graphics-programming-guide-dxgi">Full-Screen Application Performance Hints</a>.</para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
public struct SwapChainDescription {
	internal SwapChainDescription( in DXGI_SWAP_CHAIN_DESC desc ) {
		//this.BufferDesc = desc.BufferDesc;
		//this.SampleDesc = desc.SampleDesc;
		//this.BufferUsage = (Usage) desc.BufferUsage;
		//this.BufferCount = desc.BufferCount;
		//this.OutputWindow = desc.OutputWindow;
		//this.Windowed = desc.Windowed;
		//this.SwapEffect = (SwapEffect) desc.SwapEffect;
		//this.Flags = (SwapChainFlags) desc.Flags;
		this.desc = desc;
	}

	internal unsafe SwapChainDescription( DXGI_SWAP_CHAIN_DESC* pDesc ) => desc = *pDesc;

	/// <summary>
	/// Creates a new SwapChainDescription
	/// </summary>
	/// <param name="bufferDesc">The swapchain ModeDescription for the display</param>
	/// <param name="sampleDesc">The multisampling settings description</param>
	/// <param name="bufferUsage">The DXGI Usage flags</param>
	/// <param name="bufferCount">The backbuffer count</param>
	/// <param name="outputWindow">Handle to the output window</param>
	/// <param name="windowed">Indicates if swapchain is in windowed mode</param>
	/// <param name="swapEffect">The swap effect flags</param>
	/// <param name="flags">Additional swapchain flags</param>
	public SwapChainDescription( ModeDescription bufferDesc, SampleDescription sampleDesc, Usage bufferUsage,
		uint bufferCount, HWND outputWindow, bool windowed, SwapEffect swapEffect, SwapChainFlags flags ) {
		this.BufferDesc = bufferDesc;
		this.SampleDesc = sampleDesc;
		this.BufferUsage = bufferUsage;
		this.BufferCount = bufferCount;
		this.OutputWindow = outputWindow;
		this.Windowed = windowed;
		this.SwapEffect = swapEffect;
		this.Flags = flags;
	}

	//ModeDescription bufferDesc;
	//SampleDescription sampleDesc;
	//Usage bufferUsage;
	//uint bufferCount;
	//HWND outputWindow;
	//BOOL windowed;
	//SwapEffect swapEffect;
	//uint flags;

	// just wrap the internal type:
	[DebuggerBrowsable( DebuggerBrowsableState.Never )]
	DXGI_SWAP_CHAIN_DESC desc;
	[DebuggerBrowsable( DebuggerBrowsableState.Never )]
	internal DXGI_SWAP_CHAIN_DESC InternalValue => desc;

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/previous-versions/windows/desktop/legacy/bb173064(v=vs.85)">DXGI_MODE_DESC</a></b> A <a href="https://docs.microsoft.com/previous-versions/windows/desktop/legacy/bb173064(v=vs.85)">DXGI_MODE_DESC</a> structure that describes the backbuffer display mode.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public ModeDescription BufferDesc { get => desc.BufferDesc; set => desc.BufferDesc = value; }
	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgicommon/ns-dxgicommon-dxgi_sample_desc">DXGI_SAMPLE_DESC</a></b> A <a href="https://docs.microsoft.com/windows/desktop/api/dxgicommon/ns-dxgicommon-dxgi_sample_desc">DXGI_SAMPLE_DESC</a> structure that describes multi-sampling parameters.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public SampleDescription SampleDesc { get => desc.SampleDesc; set => desc.SampleDesc = value; }
	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-usage">DXGI_USAGE</a></b> A member of the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-usage">DXGI_USAGE</a> enumerated type that describes the surface usage and CPU access options for the back buffer. The back buffer can be used for shader input or render-target output.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public Usage BufferUsage { get => (Usage)desc.BufferUsage; set => desc.BufferUsage = (DXGI_USAGE)value; }
	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> A value that describes the number of buffers in the swap chain. When you call  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgifactory-createswapchain">IDXGIFactory::CreateSwapChain</a> to create a full-screen swap chain, you typically include the front buffer in this value. For more information about swap-chain buffers, see Remarks.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint BufferCount { get => desc.BufferCount; set => desc.BufferCount = value; }
	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HWND</a></b> An <a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HWND</a> handle to the output window. This member must not be <b>NULL</b>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public HWND OutputWindow { get => desc.OutputWindow; set => desc.OutputWindow = value; }
	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">BOOL</a></b> A Boolean value that specifies whether the output is in windowed mode. <b>TRUE</b> if the output is in windowed mode; otherwise, <b>FALSE</b>. We recommend that you create a windowed swap chain and allow the end user to change the swap chain to full screen through <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiswapchain-setfullscreenstate">IDXGISwapChain::SetFullscreenState</a>; that is, do not set this member to FALSE to force the swap chain to be full screen. However, if you create the swap chain as full screen, also provide the end user with a list of supported display modes through the <b>BufferDesc</b> member because a swap chain that is created with an unsupported display mode might cause the display to go black and prevent the end user from seeing anything. For more information about choosing windowed verses full screen, see <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgifactory-createswapchain">IDXGIFactory::CreateSwapChain</a>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public bool Windowed { get => desc.Windowed; set => desc.Windowed = value; }
	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ne-dxgi-dxgi_swap_effect">DXGI_SWAP_EFFECT</a></b> A member of the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ne-dxgi-dxgi_swap_effect">DXGI_SWAP_EFFECT</a> enumerated type that describes options for handling the contents of the presentation buffer after presenting a surface.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public SwapEffect SwapEffect { get => (SwapEffect)desc.SwapEffect; set => desc.SwapEffect = (DXGI_SWAP_EFFECT)value; }
	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> A member of the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ne-dxgi-dxgi_swap_chain_flag">DXGI_SWAP_CHAIN_FLAG</a> enumerated type that describes options for swap-chain behavior.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public SwapChainFlags Flags { get => (SwapChainFlags)desc.Flags; set => desc.Flags = (uint)value; }
} ;



// -----------------------------------------------

#region SwapChainDescription1 Structure Layout
//ModeDescription bufferDesc;
//SampleDescription sampleDesc;
//Usage bufferUsage;
//uint bufferCount;
//HWND outputWindow;
//BOOL windowed;
//SwapEffect swapEffect;
//uint flags;
#endregion

// -----------------------------------------------

/// <summary>Describes a swap chain.</summary>
/// <remarks>
/// <para>This structure is used by the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiswapchain-getdesc">GetDesc</a> and <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgifactory-createswapchain">CreateSwapChain</a> methods. In full-screen mode, there is a dedicated front buffer; in windowed mode, the desktop is the front buffer. If you create a swap chain with one buffer, specifying <b>DXGI_SWAP_EFFECT_SEQUENTIAL</b> does not cause the contents of the single buffer to be swapped with the front buffer. For performance information about flipping swap-chain buffers in full-screen application, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/d3d10-graphics-programming-guide-dxgi">Full-Screen Application Performance Hints</a>.</para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[DebuggerDisplay("{this.ToString()}")]
[StructLayout(LayoutKind.Sequential)]
public struct SwapChainDescription1 {
	internal SwapChainDescription1( in DXGI_SWAP_CHAIN_DESC1 desc ) => this.desc = desc ;
	internal unsafe SwapChainDescription1( DXGI_SWAP_CHAIN_DESC1* pDesc ) => desc = *pDesc;

	public SwapChainDescription1( USize size = default, Format format = Format.R8G8B8A8_UNORM, 
								  bool stereo = false, SampleDescription sampleDesc = default,
								  Usage     bufferUsage = Usage.BackBuffer, uint bufferCount = 2,
								  Scaling   scaling     = Scaling.None, SwapEffect swapEffect = SwapEffect.FlipDiscard,
								  AlphaMode alphaMode   = AlphaMode.Unspecified, SwapChainFlags flags = default ):
														this( size.Width, size.Height, format, stereo, sampleDesc, 
															  bufferUsage, bufferCount, scaling, swapEffect, alphaMode, flags ) { }
	
	
	/// <summary>
	/// Creates a new SwapChainDescription1
	/// </summary>
	/// <param name="width">The buffer width</param>
	/// <param name="height">The buffer height</param>
	/// <param name="format">The DXGI buffer format</param>
	/// <param name="stereo">Will swapchain render in stereoscopic mode (e.g., VR mode?)</param>
	/// <param name="sampleDesc">The multisample settings description</param>
	/// <param name="bufferUsage">The buffer usage flags</param>
	/// <param name="bufferCount">Number of backbuffers to use</param>
	/// <param name="scaling">The scaling flags</param>
	/// <param name="swapEffect">The swap effect flags</param>
	/// <param name="alphaMode">The alpha blending settings description</param>
	/// <param name="flags">Additional swapchain flags</param>
	public SwapChainDescription1(
		uint width, uint height, Format format, bool stereo, SampleDescription sampleDesc,
		Usage bufferUsage, uint bufferCount, Scaling scaling, SwapEffect swapEffect,
		AlphaMode alphaMode, SwapChainFlags flags = default ) {
		this.Width = width;
		this.Height = height;
		this.Format = format;
		this.Stereo = stereo;
		this.SampleDesc = sampleDesc;
		this.BufferUsage = bufferUsage;
		this.BufferCount = bufferCount;
		this.Scaling = scaling;
		this.SwapEffect = swapEffect;
		this.AlphaMode = alphaMode;
		this.Flags = flags;
	}

	/// <summary>
	/// Creates a new SwapChainDescription1
	/// </summary>
	/// <param name="width">The buffer width</param>
	/// <param name="height">The buffer height</param>
	/// <param name="format">The DXGI buffer format</param>
	/// <param name="stereo">Will swapchain render in stereoscopic mode (e.g., VR mode?)</param>
	/// <param name="sampleDesc">The multisample settings description</param>
	/// <param name="bufferUsage">The buffer usage flags</param>
	/// <param name="bufferCount">Number of backbuffers to use</param>
	/// <param name="scaling">The scaling flags</param>
	/// <param name="swapEffect">The swap effect flags</param>
	/// <param name="alphaMode">The alpha blending settings description</param>
	/// <param name="flags">Additional swapchain flags</param>
	public SwapChainDescription1(
		int width, int height, Format format, bool stereo, SampleDescription sampleDesc,
		Usage bufferUsage, uint bufferCount, Scaling scaling, SwapEffect swapEffect,
		AlphaMode alphaMode, SwapChainFlags flags = default ) {
		this.Width = (uint)width;
		this.Height = (uint)height;
		this.Format = format;
		this.Stereo = stereo;
		this.SampleDesc = sampleDesc;
		this.BufferUsage = bufferUsage;
		this.BufferCount = bufferCount;
		this.Scaling = scaling;
		this.SwapEffect = swapEffect;
		this.AlphaMode = alphaMode;
		this.Flags = flags;
	}

	/// <summary>
	/// Creates a SwapChainDescription1 from a SwapChainDescription
	/// </summary>
	/// <param name="desc">A SwapChainDescription structure</param>
	/// <param name="scaling">Additional Scaling flags (default is None)</param>
	/// <param name="alphaMode">Additional AlphaMode flags (default is Straight)</param>
	public SwapChainDescription1( SwapChainDescription desc, Scaling scaling = Scaling.None,
		AlphaMode alphaMode = AlphaMode.Straight ) {
		this.Width = desc.BufferDesc.Width;
		this.Height = desc.BufferDesc.Height;
		this.Format = desc.BufferDesc.Format;
		this.Stereo = false;
		this.SampleDesc = desc.SampleDesc;
		this.BufferUsage = desc.BufferUsage;
		this.BufferCount = desc.BufferCount;
		this.Scaling = scaling;
		this.SwapEffect = desc.SwapEffect;
		this.AlphaMode = alphaMode;
		this.Flags = desc.Flags;
	}
	
	
	//! simply wrap up the internal struct type:
	[DebuggerBrowsable( DebuggerBrowsableState.Never )]
	DXGI_SWAP_CHAIN_DESC1 desc ;
	[DebuggerBrowsable( DebuggerBrowsableState.Never )]
	internal DXGI_SWAP_CHAIN_DESC1 _InternalValue => desc ;


	/// <summary>
	/// A value that describes the resolution width. If you specify the width as zero 
	/// when you call the DXGI.IFactory.CreateSwapChain method to create a swap chain, 
	/// the runtime obtains the width from the output window and assigns this width 
	/// value to the swap-chain description. You can subsequently call the 
	/// DXGI.ISwapChain.GetDesc method to retrieve the assigned width value.
	/// </summary>
	public uint Width { get => desc.Width; set => desc.Width = value; }

	/// <summary>
	/// A value that describes the resolution height. If you specify the height as zero 
	/// when you call the DXGI.IFactory.CreateSwapChain method to create a swap chain, 
	/// the runtime obtains the height from the output window and assigns this height 
	/// value to the swap-chain description. You can subsequently call the 
	/// DXGI.ISwapChain.GetDesc method to retrieve the assigned height value.
	/// </summary>
	public uint Height { get => desc.Height; set => desc.Height = value; }

	/// <summary>
	/// A DXGI.Format structure describing the display format.
	/// </summary>
	public Format Format { get => (Format)desc.Format; set => desc.Format = (DXGI_FORMAT)value; }

	/// <summary>
	/// Specifies whether the full-screen display mode is stereo. True if stereo; otherwise, false.
	/// </summary>
	public bool Stereo { get => desc.Stereo; set => desc.Stereo = value; }

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgicommon/ns-dxgicommon-dxgi_sample_desc">DXGI_SAMPLE_DESC</a></b> A <a href="https://docs.microsoft.com/windows/desktop/api/dxgicommon/ns-dxgicommon-dxgi_sample_desc">DXGI_SAMPLE_DESC</a> structure that describes multi-sampling parameters.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public SampleDescription SampleDesc { get => desc.SampleDesc; set => desc.SampleDesc = value; }

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-usage">DXGI_USAGE</a></b> A member of the <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-usage">DXGI_USAGE</a> enumerated type that describes the surface usage and CPU access options for the back buffer. The back buffer can be used for shader input or render-target output.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public Usage BufferUsage { get => (Usage)desc.BufferUsage; set => desc.BufferUsage = (DXGI_USAGE)value; }

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> A value that describes the number of buffers in the swap chain. When you call  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgifactory-createswapchain">IDXGIFactory::CreateSwapChain</a> to create a full-screen swap chain, you typically include the front buffer in this value. For more information about swap-chain buffers, see Remarks.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint BufferCount { get => desc.BufferCount; set => desc.BufferCount = value; }

	/// <summary>
	/// <para>A <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ne-dxgi1_2-dxgi_scaling">DXGI_SCALING</a>-typed value that identifies resize behavior if the size of the back buffer is not equal to the target output.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi1_2/ns-dxgi1_2-dxgi_swap_chain_desc1#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public Scaling Scaling { get => (Scaling)desc.Scaling; set => desc.Scaling = (DXGI_SCALING)value; }

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ne-dxgi-dxgi_swap_effect">DXGI_SWAP_EFFECT</a></b> A member of the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ne-dxgi-dxgi_swap_effect">DXGI_SWAP_EFFECT</a> enumerated type that describes options for handling the contents of the presentation buffer after presenting a surface.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public SwapEffect SwapEffect { get => (SwapEffect)desc.SwapEffect; set => desc.SwapEffect = (DXGI_SWAP_EFFECT)value; }

	/// <summary>
	/// <para>A <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ne-dxgi1_2-dxgi_alpha_mode">DXGI_ALPHA_MODE</a>-typed value that identifies the transparency behavior of the swap-chain back buffer.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi1_2/ns-dxgi1_2-dxgi_swap_chain_desc1#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	internal AlphaMode AlphaMode { get => (AlphaMode)desc.AlphaMode; set => desc.AlphaMode = (DXGI_ALPHA_MODE)value; }

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> A member of the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ne-dxgi-dxgi_swap_chain_flag">DXGI_SWAP_CHAIN_FLAG</a> enumerated type that describes options for swap-chain behavior.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api//dxgi/ns-dxgi-dxgi_swap_chain_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public SwapChainFlags Flags { get => (SwapChainFlags)desc.Flags; set => 
			desc.Flags = (DXGI_SWAP_CHAIN_FLAG)(uint)value; }


	/// <summary>
	/// Converts this <see cref="SwapChainDescription1"/> structure to a string
	/// formatted in a JSON-like way, which is easy to save, load and parse ...
	/// </summary>
	/// <returns>This SwapChainDescription1 value as a formatted string.</returns>
	/// <remarks>May need to change ordering/naming a bit in the future (OK for now) ...</remarks>
	public override string ToString( ) =>
		$"\"SwapChainDescription1\": [\n" +
			$"\t\"Resolution\": \"{Width}x{Height}\", \"Format\": \"{Format}\", \n" +
			$"\t\"Buffers\": [ \"Count\": {BufferCount}, \"Usage\": {BufferUsage} ], \n" +
			$"\t\"SampleDesc\": [ \"Count\": {SampleDesc.Count}, \"Quality\": {SampleDesc.Quality} ], \n" +
			$"\t\"SwapEffect\": {SwapEffect}, \"Stereo\": {Stereo}, \"Scaling\": {Scaling}, \n" +
			$"\t\"AlphaMode\": {AlphaMode}, \"Flags\": {Flags} \n" +
		$"]" ;


	/// <summary>
	/// Converts a SwapChainDescription to a SwapChainDescription1 structure
	/// </summary>
	/// <param name="desc">A SwapChainDescription strcuture</param>
	public static explicit operator SwapChainDescription1( SwapChainDescription desc ) => new( desc );

	
} ;



//typedef struct DXGI_SWAP_CHAIN_FULLSCREEN_DESC
//{
//	DXGI_RATIONAL RefreshRate;
//	DXGI_MODE_SCANLINE_ORDER ScanlineOrdering;
//	DXGI_MODE_SCALING Scaling;
//	BOOL Windowed;
//}
//DXGI_SWAP_CHAIN_FULLSCREEN_DESC;

/// <summary>
/// Describes full-screen mode for a swap chain.
/// </summary>
[DebuggerDisplay( "FullScreen Mode = Windowed: {Windowed} @ {RefreshRate.AsFloat}Hz ({Scaling})" )]
public struct SwapChainFullscreenDescription {
	internal SwapChainFullscreenDescription( in DXGI_SWAP_CHAIN_FULLSCREEN_DESC desc ) {
		//this.desc.RefreshRate = desc.RefreshRate;
		//this.desc.ScanlineOrdering = desc.ScanlineOrdering;
		//this.desc.Scaling = desc.Scaling;
		//this.desc.Windowed = desc.Windowed;
		this.desc = desc;
	}

	internal unsafe SwapChainFullscreenDescription( DXGI_SWAP_CHAIN_FULLSCREEN_DESC* pDesc ) => this.desc = *pDesc;

	/// <summary>
	/// Creates a new SwapChainFullscreenDescription structure
	/// </summary>
	/// <param name="refreshRate">The refresh rate</param>
	/// <param name="scanlineOrdering">The scaline ordering flags</param>
	/// <param name="scalingMode">The scaling mode flags</param>
	/// <param name="windowed">Indicates if display mode is windowed</param>
	public SwapChainFullscreenDescription( Rational refreshRate, ScanlineOrder scanlineOrdering,
		ScalingMode scalingMode, bool windowed ) {
		this.desc.RefreshRate = refreshRate;
		this.desc.ScanlineOrdering = (DXGI_MODE_SCANLINE_ORDER)scanlineOrdering;
		this.desc.Scaling = (DXGI_MODE_SCALING)scalingMode;
		this.desc.Windowed = windowed;
	}



	[DebuggerBrowsable( DebuggerBrowsableState.Never )]
	DXGI_SWAP_CHAIN_FULLSCREEN_DESC desc;
	[DebuggerBrowsable( DebuggerBrowsableState.Never )]
	internal DXGI_SWAP_CHAIN_FULLSCREEN_DESC InternalValue => desc;



	/// <summary>
	/// A <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgicommon/ns-dxgicommon-dxgi_rational?redirectedfrom=MSDN">DXGI.Rational</a> 
	/// structure describing the refresh rate in hertz
	/// </summary>
	public Rational RefreshRate { get => desc.RefreshRate; set => desc.RefreshRate = value; }

	/// <summary>
	/// A member of the DXGI.ScanlineOrdering enumerated type describing the scanline drawing mode.
	/// </summary>
	public ScanlineOrder ScanlineOrdering { get => (ScanlineOrder)desc.ScanlineOrdering; set => desc.ScanlineOrdering = (DXGI_MODE_SCANLINE_ORDER)value; }

	/// <summary>
	/// A member of the DXGI.ScalingMode enumerated type describing the scaling mode.
	/// </summary>
	public ScalingMode Scaling { get => (ScalingMode)desc.Scaling; set => desc.Scaling = (DXGI_MODE_SCALING)value; }

	/// <summary>
	/// A Boolean value that specifies whether the swap chain is in windowed mode. 
	/// TRUE if the swap chain is in windowed mode; otherwise, FALSE.
	/// </summary>
	public bool Windowed { get => desc.Windowed; set => desc.Windowed = value; }
};


//typedef struct DXGI_MODE_DESC
//{
//	UINT Width;
//	UINT Height;
//	DXGI_RATIONAL RefreshRate;
//	DXGI_FORMAT Format;
//	DXGI_MODE_SCANLINE_ORDER ScanlineOrdering;
//	DXGI_MODE_SCALING Scaling;
//}
//DXGI_MODE_DESC;

/// <summary>
/// Describes a display mode.
/// </summary>
/// <remarks>
/// For more info see: <a href="https://learn.microsoft.com/en-us/previous-versions/windows/desktop/legacy/bb173064(v=vs.85)">DXGI_MODE_DESC</a>
/// </remarks>
[DebuggerDisplay( "Mode = {Width}x{Height} @ {RefreshRate.AsFloat}Hz ({Scaling})" )]
public struct ModeDescription {
	internal ModeDescription( in DXGI_MODE_DESC modeDesc ) => this.desc = modeDesc;

	internal unsafe ModeDescription( DXGI_MODE_DESC* pModeDesc ) => this.desc = *pModeDesc;

	/// <summary>
	/// Creates a new DXGI.ModeDescription
	/// </summary>
	/// <param name="width">Width of the display mode</param>
	/// <param name="height">Height of the display mode</param>
	/// <param name="refreshRate">Refresh rate of the monitor</param>
	/// <param name="format">The resource format of the display mode</param>
	/// <param name="scanlineOrdering">The scanline ordering of the display mode</param>
	/// <param name="scaling">The scaling of the display mode</param>
	public ModeDescription( uint width, uint height, Rational refreshRate, Format format = Format.R8G8B8A8_UNORM,
		ScanlineOrder scanlineOrdering = ScanlineOrder.Unspecified, ScalingMode scaling = ScalingMode.Centered ) =>
		this.desc = new DXGI_MODE_DESC( width, height, refreshRate, format, scanlineOrdering, scaling );

	/// <summary>
	/// Creates a new ModeDescription out of a ModeDescription1
	/// </summary>
	/// <param name="modeDesc1">A ModeDescription1 structure</param>
	public ModeDescription( in ModeDescription1 modeDesc1 ) {
		unsafe {
			fixed( ModeDescription1* pData = (&modeDesc1) )
				this.desc = *((DXGI_MODE_DESC*)pData);
		}
	}



	//uint width;
	//uint height;
	//Rational refreshRate;
	//Format format;
	//ScanlineOrder scanlineOrdering;
	//ScalingMode scaling;
	[DebuggerBrowsable( DebuggerBrowsableState.Never )] DXGI_MODE_DESC desc ;



	/// <summary>
	/// A value that describes the resolution width. If you specify the width as zero 
	/// when you call the DXGI.IFactory.CreateSwapChain method to create a swap chain, 
	/// the runtime obtains the width from the output window and assigns this width 
	/// value to the swap-chain description. You can subsequently call the 
	/// DXGI.ISwapChain.GetDesc method to retrieve the assigned width value.
	/// </summary>
	public uint Width { get => desc.Width; set => desc.Width = value; }

	/// <summary>
	/// A value that describes the resolution height. If you specify the height as zero 
	/// when you call the DXGI.IFactory.CreateSwapChain method to create a swap chain, 
	/// the runtime obtains the height from the output window and assigns this height 
	/// value to the swap-chain description. You can subsequently call the 
	/// DXGI.ISwapChain.GetDesc method to retrieve the assigned height value.
	/// </summary>
	public uint Height { get => desc.Height; set => desc.Height = value; }

	/// <summary>
	/// A <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgicommon/ns-dxgicommon-dxgi_rational?redirectedfrom=MSDN">DXGI.Rational</a> 
	/// structure describing the refresh rate in hertz
	/// </summary>
	public Rational RefreshRate { get => desc.RefreshRate; set => desc.RefreshRate = value; }

	/// <summary>
	/// A DXGI.Format structure describing the display format.
	/// </summary>
	public Format Format { get => (Format)desc.Format; set => desc.Format = (DXGI_FORMAT)value; }

	/// <summary>
	/// A member of the DXGI.ScanlineOrder enumerated type describing the scanline drawing mode.
	/// </summary>
	public ScanlineOrder ScanlineOrdering { get => (ScanlineOrder)desc.ScanlineOrdering; set => desc.ScanlineOrdering = (DXGI_MODE_SCANLINE_ORDER)value; }

	/// <summary>
	/// A member of the DXGI.ScalingMode enumerated type describing the scaling mode.
	/// </summary>
	public ScalingMode Scaling { get => (ScalingMode)desc.Scaling; set => desc.Scaling = (DXGI_MODE_SCALING)value; }
	
	public static explicit operator ModeDescription( ModeDescription1 mode ) => new( mode );
} ;


//typedef struct DXGI_MODE_DESC1
//{
//	UINT Width;
//	UINT Height;
//	DXGI_RATIONAL RefreshRate;
//	DXGI_FORMAT Format;
//	DXGI_MODE_SCANLINE_ORDER ScanlineOrdering;
//	DXGI_MODE_SCALING Scaling;
//	BOOL Stereo;
//}
//DXGI_MODE_DESC1;

/// <summary>
/// Describes a display mode and whether the display mode supports stereo.
/// </summary>
/// <remarks>
/// ModeDescription1 is identical to ModeDescription except that ModeDescription1 includes the Stereo member.
/// </remarks>
[DebuggerDisplay( "Mode = {Width}x{Height} @ {RefreshRate.AsFloat}Hz ({Scaling})" )]
public struct ModeDescription1 {
	internal ModeDescription1( in DXGI_MODE_DESC1 modeDesc ) => this.desc = modeDesc;

	internal unsafe ModeDescription1( DXGI_MODE_DESC1* pModeDesc ) => desc = *pModeDesc;

	/// <summary>
	/// Creates a new DXGI.ModeDescription
	/// </summary>
	/// <param name="width">Width of the display mode</param>
	/// <param name="height">Height of the display mode</param>
	/// <param name="refreshRate">Refresh rate of the monitor</param>
	/// <param name="format">The resource format of the display mode</param>
	/// <param name="scanlineOrdering">The scanline ordering of the display mode</param>
	/// <param name="scaling">The scaling of the display mode</param>
	/// <param name="stereo">Indicates if rendering in stereo (e.g., for VR/Mixed Reality) mode</param>
	public ModeDescription1( uint width, uint height, Rational refreshRate, Format format = Format.R8G8B8A8_UNORM,
		ScanlineOrder scanlineOrdering = ScanlineOrder.Unspecified, ScalingMode scaling = ScalingMode.Centered, bool stereo = false ) {
		//this.desc.Width = width;
		//this.desc.Height = height;
		//this.desc.RefreshRate = refreshRate;
		//this.desc.Format = (DXGI_FORMAT)format;
		//this.desc.ScanlineOrdering = (DXGI_MODE_SCANLINE_ORDER)scanlineOrdering;
		//this.desc.Scaling = (DXGI_MODE_SCALING)scaling;
		//this.desc.Stereo = stereo;

		this.desc = new DXGI_MODE_DESC1( width, height, refreshRate,
			format, scanlineOrdering, scaling, stereo );
	}

	/// <summary>
	/// Creates a new ModeDescription1 from a ModeDescription and optional stereo (bool) value
	/// </summary>
	/// <param name="desc">A ModeDescription structure</param>
	/// <param name="stereo">Indicates if rendering in stereo (e.g., for VR/Mixed Reality) mode</param>
	public ModeDescription1( ModeDescription desc, bool stereo = false ) {
		this.desc = desc;
		this.desc.Stereo = stereo;
	}

	//uint width;
	//uint height;
	//Rational refreshRate;
	//Format format;
	//ScanlineOrder scanlineOrdering;
	//ScalingMode scaling;
	//bool stereo;
	DXGI_MODE_DESC1 desc;


	/// <summary>
	/// A value that describes the resolution width. If you specify the width as zero 
	/// when you call the DXGI.IFactory.CreateSwapChain method to create a swap chain, 
	/// the runtime obtains the width from the output window and assigns this width 
	/// value to the swap-chain description. You can subsequently call the 
	/// DXGI.ISwapChain.GetDesc method to retrieve the assigned width value.
	/// </summary>
	public uint Width { get => desc.Width; set => desc.Width = value; }

	/// <summary>
	/// A value that describes the resolution height. If you specify the height as zero 
	/// when you call the DXGI.IFactory.CreateSwapChain method to create a swap chain, 
	/// the runtime obtains the height from the output window and assigns this height 
	/// value to the swap-chain description. You can subsequently call the 
	/// DXGI.ISwapChain.GetDesc method to retrieve the assigned height value.
	/// </summary>
	public uint Height { get => desc.Height; set => desc.Height = value; }

	/// <summary>
	/// A <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgicommon/ns-dxgicommon-dxgi_rational?redirectedfrom=MSDN">DXGI.Rational</a> 
	/// structure describing the refresh rate in hertz
	/// </summary>
	public Rational RefreshRate { get => desc.RefreshRate; set => desc.RefreshRate = value; }

	/// <summary>
	/// A DXGI.Format structure describing the display format.
	/// </summary>
	public Format Format { get => (Format)desc.Format; set => desc.Format = (DXGI_FORMAT)value; }

	/// <summary>
	/// A member of the DXGI.ScanlineOrder enumerated type describing the scanline drawing mode.
	/// </summary>
	public ScanlineOrder ScanlineOrdering { get => (ScanlineOrder)desc.ScanlineOrdering; set => desc.ScanlineOrdering = (DXGI_MODE_SCANLINE_ORDER)value; }

	/// <summary>
	/// A member of the DXGI.ScalingMode enumerated type describing the scaling mode.
	/// </summary>
	public ScalingMode Scaling { get => (ScalingMode)desc.Scaling; set => desc.Scaling = (DXGI_MODE_SCALING)value; }

	/// <summary>
	/// Specifies whether the full-screen display mode is stereo. True if stereo; otherwise, false.
	/// </summary>
	public bool Stereo { get => desc.Stereo; set => desc.Stereo = value; }



	/// <summary>
	/// Converts a ModeDescription to a ModeDescription1 structure
	/// </summary>
	/// <param name="desc">A ModeDescription structure</param>
	public static implicit operator ModeDescription1( ModeDescription desc ) => new( desc );


} ;



[StructLayout( LayoutKind.Sequential )]
public struct OutputDescription {
	[MarshalAs(UnmanagedType.LPStr, SizeConst = 32)]
	public FixedStr32 DeviceName ;
	
	public Rect         DesktopCoordinates ;
	public bool         AttachedToDesktop ;
	public ModeRotation ModeRotation ;
	public Win32Handle  Monitor ;
	
	public OutputDescription( in OutputDescription desc ) {
		unsafe { fixed( OutputDescription* pThis = &this )
				*( (OutputDescription*)pThis ) = desc ; }
	}
	
	public OutputDescription( in FixedStr32 deviceName, 
							  in Rect desktopCoordinates, 
							  bool attachedToDesktop,
							  ModeRotation modeRotation, 
							  nint monitor ) {
		this.DeviceName = deviceName ;
		this.DesktopCoordinates = desktopCoordinates ;
		this.AttachedToDesktop = attachedToDesktop ;
		this.ModeRotation = modeRotation ;
		this.Monitor = monitor ;
	}
	
	public OutputDescription( in FixedStr32 deviceName, 
							  in Rect desktopCoordinates, 
							  bool attachedToDesktop, 
							  ModeRotation modeRotation, 
							  Win32Handle monitor ) {
		this.DeviceName = deviceName ;
		this.DesktopCoordinates = desktopCoordinates ;
		this.AttachedToDesktop = attachedToDesktop ;
		this.ModeRotation = modeRotation ;
		this.Monitor = monitor ;
	}
	
	public OutputDescription( in DXGI_OUTPUT_DESC desc) {
		this.DeviceName         = new( desc.DeviceName ) ;
		this.DesktopCoordinates = desc.DesktopCoordinates ;
		this.AttachedToDesktop  = desc.AttachedToDesktop ;
		this.ModeRotation           = (ModeRotation)desc.Rotation ;
		this.Monitor            = desc.Monitor ;
	}
	
	public static implicit operator OutputDescription( in DXGI_OUTPUT_DESC desc ) => new( desc ) ;
	public static implicit operator DXGI_OUTPUT_DESC( in OutputDescription desc ) {
		DXGI_OUTPUT_DESC d = default ;
		d.DeviceName         = desc.DeviceName.ToString( ) ;
		d.DesktopCoordinates = desc.DesktopCoordinates ;
		d.AttachedToDesktop  = desc.AttachedToDesktop ;
		d.Rotation           = (DXGI_MODE_ROTATION)desc.ModeRotation ;
		d.Monitor            = desc.Monitor ;
		return d ;
	}
} ;


/// <summary>Describes an output or physical connection between the adapter (video card) and a device, including additional information about color capabilities and connection type.</summary>
/// <remarks>The <b>DXGI_OUTPUT_DESC1</b> structure is initialized by the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_6/nf-dxgi1_6-idxgioutput6-getdesc1">IDXGIOutput6::GetDesc1</a> method.</remarks>
[EquivalentOf( typeof( DXGI_OUTPUT_DESC1 ) )]
public struct OutputDescription1 {
	
	/// <summary>
	/// <para>Type: <b>WCHAR[32]</b> A string that contains the name of the output device.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/ns-dxgi1_6-dxgi_output_desc1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public FixedStr32 DeviceName;

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/windef/ns-windef-rect">Rect</a></b> A <a href="https://docs.microsoft.com/windows/desktop/api/windef/ns-windef-rect">Rect</a> structure containing the bounds of the output in desktop coordinates. Desktop coordinates depend on the dots per inch (DPI) of the desktop. For info about writing DPI-aware Win32 apps, see <a href="https://docs.microsoft.com/windows/desktop/hidpi/high-dpi-desktop-application-development-on-windows">High DPI</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/ns-dxgi1_6-dxgi_output_desc1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public Rect DesktopCoordinates;

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">BOOL</a></b> True if the output is attached to the desktop; otherwise, false.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/ns-dxgi1_6-dxgi_output_desc1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public BOOL AttachedToDesktop;

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/previous-versions/windows/desktop/legacy/bb173065(v=vs.85)">DXGI_MODE_ROTATION</a></b> A member of the <a href="https://docs.microsoft.com/previous-versions/windows/desktop/legacy/bb173065(v=vs.85)">DXGI_MODE_ROTATION</a> enumerated type describing on how an image is rotated by the output.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/ns-dxgi1_6-dxgi_output_desc1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public ModeRotation Rotation;

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HMONITOR</a></b> An <a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">HMONITOR</a> handle that represents the display monitor. For more information, see <a href="https://docs.microsoft.com/windows/desktop/gdi/hmonitor-and-the-device-context">HMONITOR and the Device Context</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/ns-dxgi1_6-dxgi_output_desc1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public HMonitor Monitor;

	/// <summary>
	/// <para>Type: <b>UINT</b> The number of bits per color channel for the active wire format of the display attached to this output.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/ns-dxgi1_6-dxgi_output_desc1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public uint BitsPerColor;

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/dxgicommon/ne-dxgicommon-dxgi_color_space_type">DXGI_COLOR_SPACE_TYPE</a></b> The current advanced color capabilities of the display attached to this output. Specifically, whether its capable of reproducing color and luminance values outside of the sRGB color space. A value of DXGI_COLOR_SPACE_RGB_FULL_G22_NONE_P709 indicates that the display is limited to SDR/sRGB; A value of DXGI_COLOR_SPACE_RGB_FULL_G2048_NONE_P2020 indicates that the display supports advanced color capabilities. For detailed luminance and color capabilities, see additional members of this struct.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/ns-dxgi1_6-dxgi_output_desc1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public ColorSpaceType ColorSpace;

	/// <summary>
	/// <para>Type: <b>FLOAT[2]</b> The red color primary, in xy coordinates, of the display attached to this output. This value will usually come from the EDID of the corresponding display or sometimes from an override.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/ns-dxgi1_6-dxgi_output_desc1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public Vector2 RedPrimary;

	/// <summary>
	/// <para>Type: <b>FLOAT[2]</b> The green color primary, in xy coordinates, of the display attached to this output. This value will usually come from the EDID of the corresponding display or sometimes from an override.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/ns-dxgi1_6-dxgi_output_desc1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public Vector2 GreenPrimary;

	/// <summary>
	/// <para>Type: <b>FLOAT[2]</b> The blue color primary, in xy coordinates, of the display attached to this output. This value will usually come from the EDID of the corresponding display or sometimes from an override.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/ns-dxgi1_6-dxgi_output_desc1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public Vector2 BluePrimary;

	/// <summary>
	/// <para>Type: <b>FLOAT[2]</b> The white point, in xy coordinates, of the display attached to this output. This value will usually come from the EDID of the corresponding display or sometimes from an override.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/ns-dxgi1_6-dxgi_output_desc1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public Vector2 WhitePoint;

	/// <summary>
	/// <para>Type: <b>FLOAT</b> The minimum luminance, in nits, that the display attached to this output is capable of rendering. Content should not exceed this minimum value for optimal rendering. This value will usually come from the EDID of the corresponding display or sometimes from an override.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/ns-dxgi1_6-dxgi_output_desc1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float MinLuminance;

	/// <summary>
	/// <para>Type: <b>FLOAT</b> The maximum luminance, in nits, that the display attached to this output is capable of rendering; this value is likely only valid for a small area of the panel. Content should not exceed this minimum value for optimal rendering. This value will usually come from the EDID of the corresponding display or sometimes from an override.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/ns-dxgi1_6-dxgi_output_desc1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float MaxLuminance;

	/// <summary>
	/// <para>Type: <b>FLOAT</b> The maximum luminance, in nits, that the display attached to this output is capable of rendering; unlike MaxLuminance, this value is valid for a color that fills the entire area of the panel. Content should not exceed this value across the entire panel for optimal rendering. This value will usually come from the EDID of the corresponding display or sometimes from an override.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/ns-dxgi1_6-dxgi_output_desc1#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public float MaxFullFrameLuminance;
} ;


/// <summary>Describes timing and presentation statistics for a frame.</summary>
/// <remarks>
/// <para>You initialize the <b>DXGI_FRAME_STATISTICS</b> structure with the
/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgioutput-getframestatistics">IDXGIOutput::GetFrameStatistics</a>
/// or <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiswapchain-getframestatistics">IDXGISwapChain::GetFrameStatistics</a>
/// method. You can only use
/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiswapchain-getframestatistics">IDXGISwapChain::GetFrameStatistics</a>
/// for swap chains that either use the flip presentation model or draw in full-screen mode. You set the
/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ne-dxgi-dxgi_swap_effect">DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL</a> value in the
/// <b>SwapEffect</b> member of the
/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ns-dxgi1_2-dxgi_swap_chain_desc1">DXGI_SWAP_CHAIN_DESC1</a> structure to specify
/// that the swap chain uses the flip presentation model. The values in the <b>PresentCount</b> and <b>PresentRefreshCount</b> members indicate
/// information about when a frame was presented on the display screen. You can use these values to determine whether a glitch occurred. The values
/// in the <b>SyncRefreshCount</b> and <b>SyncQPCTime</b> members indicate timing information that you can use for audio and video synchronization or
/// very precise animation. If the swap chain draws in full-screen mode, these values are based on when the computer booted. If the swap chain draws
/// in windowed mode, these values are based on when the swap chain is created.</para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/ns-dxgi-dxgi_frame_statistics#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(DXGI_FRAME_STATISTICS))]
[StructLayout( LayoutKind.Sequential )]
public struct FrameStatistics {
	public uint PresentCount, PresentRefreshCount, SyncRefreshCount ;
	public long SyncQPCTime, SyncGPUTime ;
	
	public FrameStatistics( in DXGI_FRAME_STATISTICS stats ) {
		this.PresentCount       = stats.PresentCount ;
		this.PresentRefreshCount = stats.PresentRefreshCount ;
		this.SyncRefreshCount   = stats.SyncRefreshCount ;
		this.SyncQPCTime        = stats.SyncQPCTime ;
		this.SyncGPUTime        = stats.SyncGPUTime ;
	}
	
	public static implicit operator FrameStatistics( in DXGI_FRAME_STATISTICS stats ) => new( stats ) ;
	public static implicit operator DXGI_FRAME_STATISTICS( in FrameStatistics stats ) {
		 DXGI_FRAME_STATISTICS s = default ;
		 s.PresentCount       = stats.PresentCount ;
		 s.PresentRefreshCount = stats.PresentRefreshCount ;
		 s.SyncRefreshCount   = stats.SyncRefreshCount ;
		 s.SyncQPCTime        = stats.SyncQPCTime ;
		 s.SyncGPUTime        = stats.SyncGPUTime ;
		 return s ;
	 }
} ;


/// <summary>Describes a debug message in the information queue.</summary>
/// <remarks>
/// <para>
/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-getmessage">IDXGIInfoQueue::GetMessage</a>
/// returns a pointer to this structure.
/// <div class="alert"><b>Note</b> This API requires the Windows Software Development Kit (SDK) for Windows 8.</div>
/// </para>
/// <para>
/// <a href="https://docs.microsoft.com/windows/win32/api/dxgidebug/ns-dxgidebug-dxgi_info_queue_message#">
/// Read more on docs.microsoft.com</a>.
/// </para>
/// </remarks>
[EquivalentOf( "DXGI_INFO_QUEUE_MESSAGE", "DXGI" )]
public partial struct InfoQueueMessage {
	
	/// <summary>
	/// A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value
	/// that identifies the entity that produced the message.
	/// </summary>
	public Guid Producer ;

	/// <summary>
	/// A <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/ne-dxgidebug-dxgi_info_queue_message_category">DXGI_INFO_QUEUE_MESSAGE_CATEGORY</a>-typed
	/// value that specifies the category of the message.
	/// </summary>
	public InfoQueueMessageCategory Category ;

	/// <summary>
	/// A <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/ne-dxgidebug-dxgi_info_queue_message_severity">DXGI_INFO_QUEUE_MESSAGE_SEVERITY</a>-typed
	/// value that specifies the severity of the message.
	/// </summary>
	public InfoQueueMessageSeverity Severity ;

	/// <summary>An integer that uniquely identifies the message.</summary>
	public int ID ;

	/// <summary>The message string.</summary>
	public unsafe byte* pDescription ;

	/// <summary>The length of the message string at <b>pDescription</b>, in bytes.</summary>
	public nuint DescriptionByteLength ;
	
	
	public unsafe InfoQueueMessage( in Guid producer                        = default, 
							 InfoQueueMessageCategory category              = default, 
							 InfoQueueMessageSeverity severity              = default, 
							 int                      id                    = 0,
							 byte*                    pDescription          = null,
							 nuint                    descriptionByteLength = 0x00000000U ) {
		this.Producer = producer ;
		this.Category = category ;
		this.Severity = severity ;
		this.ID = id ;
		this.pDescription = pDescription ;
		this.DescriptionByteLength = descriptionByteLength ;
	}
} ;


[StructLayout( LayoutKind.Sequential )]
public struct AdapterDescription {
	public FixedStr128 Description; // A string that contains the adapter description.
	public uint VendorId; // The PCI ID of the hardware vendor.
	public uint DeviceId; // The PCI ID of the hardware device.
	public uint SubSysId; // The PCI ID of the sub system.
	public uint Revision; // The PCI ID of the revision number of the adapter.
	public ulong DedicatedVideoMemory; // The amount of video memory that is not shared with the CPU.
	public ulong DedicatedSystemMemory; // The amount of system memory that is not shared with the CPU.
	public ulong SharedSystemMemory; // The amount of system memory that is shared with the CPU.
	public Luid AdapterLuid; // A unique value that identifies the adapter.
	
	//! DXGI_ADAPTER_DESC1 has a "Flags" field, but DXGI_ADAPTER_DESC doesn't ...
	//public uint Flags; // Identifies the adapter type. The value is a bitwise OR of DXGI_ADAPTER_FLAG enumeration constants.
	
	public AdapterDescription( in AdapterDescription desc ) {
		this.Description = desc.Description ;
		this.VendorId = desc.VendorId ;
		this.DeviceId = desc.DeviceId ;
		this.SubSysId = desc.SubSysId ;
		this.Revision = desc.Revision ;
		this.DedicatedVideoMemory = desc.DedicatedVideoMemory ;
		this.DedicatedSystemMemory = desc.DedicatedSystemMemory ;
		this.SharedSystemMemory = desc.SharedSystemMemory ;
		this.AdapterLuid = desc.AdapterLuid ;
	}
	
	public AdapterDescription( in DXGI_ADAPTER_DESC desc ) {
		this.Description           = new( desc.Description ) ;
		this.VendorId              = desc.VendorId ;
		this.DeviceId              = desc.DeviceId ;
		this.SubSysId              = desc.SubSysId ;
		this.Revision              = desc.Revision ;
		this.DedicatedVideoMemory  = desc.DedicatedVideoMemory ;
		this.DedicatedSystemMemory = desc.DedicatedSystemMemory ;
		this.SharedSystemMemory    = desc.SharedSystemMemory ;
		this.AdapterLuid           = new( desc.AdapterLuid ) ;
	}
	
	public static implicit operator AdapterDescription( in DXGI_ADAPTER_DESC desc ) => new( desc ) ;
	public static implicit operator DXGI_ADAPTER_DESC( in AdapterDescription desc ) {
		DXGI_ADAPTER_DESC d = default ;
		d.Description           = desc.Description.ToString( ) ;
		d.VendorId              = desc.VendorId ;
		d.DeviceId              = desc.DeviceId ;
		d.SubSysId              = desc.SubSysId ;
		d.Revision              = desc.Revision ;
		d.DedicatedVideoMemory  = (nuint)desc.DedicatedVideoMemory ;
		d.DedicatedSystemMemory = (nuint)desc.DedicatedSystemMemory ;
		d.SharedSystemMemory    = (nuint)desc.SharedSystemMemory ;
		d.AdapterLuid           = desc.AdapterLuid ;
		return d ;
	}
} ;



[Serializable,
 DebuggerDisplay("LUID: {LowPart}:{HighPart}"),
 StructLayout(LayoutKind.Sequential)]
public struct Luid: IEquatable<Luid> {
	public uint LowPart ; public int HighPart ;
	public Luid( uint low, int high ) { LowPart = low ; HighPart = high ; }
	public Luid( in LUID luid ) { LowPart = luid.LowPart ; HighPart = luid.HighPart ; }
	public unsafe Luid( LUID* pLuid ) { LowPart = pLuid->LowPart ; HighPart = pLuid->HighPart ; }
	public static implicit operator Luid( in LUID luid ) => new( luid ) ;
	public static implicit operator LUID( in Luid luid ) => new LUID {
		LowPart = luid.LowPart, HighPart = luid.HighPart
	} ;
	
	public static bool operator ==( in Luid a, in Luid b ) => 
		a.LowPart == b.LowPart && a.HighPart == b.HighPart ;
	public static bool operator !=( in Luid a, in Luid b ) =>
	 		a.LowPart != b.LowPart || a.HighPart != b.HighPart ;
	
	public override int GetHashCode( ) => HashCode.Combine( LowPart, HighPart ) ;
	public override bool Equals( object? obj ) => obj is Luid luid && this == luid ;
	public override string ToString( ) => $"LUID: {{ {LowPart}, {HighPart} }}" ;
	public bool Equals( Luid other ) => LowPart == other.LowPart 
										&& HighPart == other.HighPart ;
} ;


public struct SharedResource: IEquatable< SharedResource >, 
							  IEquatable< Win32Handle > {
	public Win32Handle Handle ;
	
	public SharedResource( Win32Handle handle ) => Handle = handle ;
	
	
	public override int GetHashCode( ) => Handle.GetHashCode( ) ;
	public bool Equals( Win32Handle other ) => Handle == other ;
	public bool Equals( SharedResource other ) => Handle == other.Handle ;
	public override bool Equals( object? obj ) => obj is SharedResource other && Equals( other ) ;
	public override string ToString( ) => $"{nameof(SharedResource)}: {Handle}" ;
	
	public static implicit operator SharedResource( Win32Handle handle ) => new( handle ) ;
	public static implicit operator Win32Handle( SharedResource handle ) => handle.Handle ;
	
	public static bool operator ==( in SharedResource a, in SharedResource b ) => a.Handle == b.Handle ;
	public static bool operator !=( in SharedResource a, in SharedResource b ) => a.Handle != b.Handle ;
	public static bool operator ==( in SharedResource a, in Win32Handle b ) => a.Handle == b ;
	public static bool operator !=( in SharedResource a, in Win32Handle b ) => a.Handle != b ;
	public static bool operator ==( in Win32Handle a, in SharedResource b ) => a == b.Handle ;
	public static bool operator !=( in Win32Handle a, in SharedResource b ) => a != b.Handle ;
} ;


/// <summary>
/// Represents flags that can be used with the DXGI present operations.
/// </summary>
/// <remarks>
/// These flags specify how the presentation will behave in various situations.
/// For detailed information, see the
/// <a href="https://learn.microsoft.com/en-us/windows/win32/direct3ddxgi/dxgi-present">
/// official documentation
/// </a>.
/// </remarks>
[Flags] public enum PresentFlags: uint {
	Default = 0,                             // Present a frame from each buffer (starting with the current buffer) to the output.
	DoNotSequence = 0x00000002,              // Present a frame from the current buffer to the output.
	Test = 0x00000001,                       // Do not present the frame to the output.
	Restart = 0x00000004,                    // Specifies that the runtime will discard outstanding queued presents.
	DoNotWait = 0x00000008,                  // Specifies that the runtime will fail the presentation.
	RestrictToOutput = 0x00000010,           // Indicates that presentation content will be shown only on the particular output.
	StereoPreferRight = 0x00000020,          // Indicates that if the stereo present must be reduced to mono, right-eye viewing is used.
	StereoTemporaryMono = 0x00000040,        // Indicates that the presentation should use the left buffer as a mono buffer.
	UseDuration = 0x00000100,                // This flag must be set by media apps that are currently using a custom present duration.
	AllowTearing = 0x00000200                // This value is supported starting in Windows 8.1.
} ;


public struct PresentParameters {
	/// <summary>
	/// The number of updated rectangles that you update in the
	/// back buffer for the presented frame. The operating system
	/// uses this information to optimize presentation. You can set
	/// this member to 0 to indicate that you update the whole frame.
	/// </summary>
	public uint DirtyRectsCount ;

	/// <summary>
	/// A list of updated rectangles that you update in the back buffer
	/// for the presented frame. An application must update every single
	/// pixel in each rectangle that it reports to the runtime; the
	/// application cannot assume that the pixels are saved from the
	/// previous frame. For more information about updating dirty
	/// rectangles, see Remarks. You can set this member to <b>NULL</b>
	/// if <b>DirtyRectsCount</b> is 0. An application must not update
	/// any pixel outside of the dirty rectangles.
	/// </summary>
	public unsafe Rect* pDirtyRects ;

	/// <summary>
	/// <para>A pointer to the scrolled rectangle. The scrolled rectangle is the
	/// rectangle of the previous frame from which the runtime bit-block transfers
	/// (bitblts) content. The runtime also uses the scrolled rectangle to optimize
	/// presentation in terminal server and indirect display scenarios. The scrolled
	/// rectangle also describes the destination rectangle, that is, the region on the
	/// current frame that is filled with scrolled content. You can set this member to
	/// <b>NULL</b> to indicate that no content is scrolled from the previous frame.</para>
	/// <para>
	/// <a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/ns-dxgi1_2-dxgi_present_parameters#members">
	/// Read more on docs.microsoft.com</a>.
	/// </para>
	/// </summary>
	public unsafe Rect* pScrollRect ;

	/// <summary>
	/// A pointer to the offset of the scrolled area that goes from the source rectangle
	/// (of previous frame) to the destination rectangle (of current frame). You can set
	/// this member to <b>NULL</b> to indicate no offset.
	/// </summary>
	public unsafe Point* pScrollOffset ;
	
	
	public unsafe PresentParameters( uint dirtyRectsCount = 0,
										  Rect* pDirtyRects = null,
										  Rect* pScrollRect = null, 
										  Point* pScrollOffset = null ) {
		this.pDirtyRects     = pDirtyRects ;
		this.pScrollRect     = pScrollRect ;
		this.pScrollOffset   = pScrollOffset ;
		this.DirtyRectsCount = dirtyRectsCount ;
	}
	
	public unsafe PresentParameters( DXGI_PRESENT_PARAMETERS parameters ) {
		this.pDirtyRects     = (Rect *)parameters.pDirtyRects ;
		this.pScrollRect     = (Rect *)parameters.pScrollRect ;
		this.DirtyRectsCount = parameters.DirtyRectsCount ;
		this.pScrollOffset   = parameters.pScrollOffset ;
	}
	
	public static implicit operator PresentParameters( in DXGI_PRESENT_PARAMETERS parameters ) => new( parameters ) ;
	public static unsafe implicit operator DXGI_PRESENT_PARAMETERS( in PresentParameters parameters ) => new( ) {
			pDirtyRects     = (RECT *)parameters.pDirtyRects,
			pScrollRect     = (RECT *)parameters.pScrollRect,
			DirtyRectsCount = parameters.DirtyRectsCount,
			pScrollOffset   = parameters.pScrollOffset
		} ;
} ;





[StructLayout( LayoutKind.Sequential )]
public struct RGBA {
	public float R, G, B, A ;
	public RGBA(float r, float g, float b, float a) {
		R = r; G = g; B = b; A = a;
	}
	
	public static implicit operator DXGI_RGBA(RGBA color) => new DXGI_RGBA {
			r = color.R, g = color.G, b = color.B, a = color.A
	} ;
	public static implicit operator RGBA(DXGI_RGBA color) => new RGBA(color.r, color.g, color.b, color.a);
} ;



public enum ColorSpaceType: uint {
	RGBFullG22NoneP709           = 0,
	RGBFullG10NoneP709           = 1,
	RGBStudioG22NoneP709         = 2,
	RGBStudioG22NoneP2020        = 3,
	Reserved                     = 4,
	YCbCrFullG22NoneP709X601     = 5,
	YCbCrStudioG22LeftP601       = 6,
	YCbCrFullG22LeftP601         = 7,
	YCbCrStudioG22LeftP709       = 8,
	YCbCrFullG22LeftP709         = 9,
	YCbCrStudioG22LeftP2020      = 10,
	YCbCrFullG22LeftP2020        = 11,
	RGBFullG2084NoneP2020        = 12,
	YCbCrStudioG2084LeftP2020    = 13,
	RGBStudioG2084NoneP2020      = 14,
	YCbCrStudioG22TopLeftP2020   = 15,
	YCbCrStudioG2084TopLeftP2020 = 16,
	RGBFullG22NoneP2020          = 17,
	YCbCrStudioGHlgTopLeftP2020  = 18,
	YCbCrFullGHlgTopLeftP2020    = 19,
	RGBStudioG24NoneP709         = 20,
	RGBStudioG24NoneP2020        = 21,
	YCbCrStudioG24LeftP709       = 22,
	YCbCrStudioG24LeftP2020      = 23,
	YCbCrStudioG24TopLeftP2020   = 24,
	Custom                       = 0xFFFFFFFF,
} ;



// ----------------------------------------------------
// Output Duplication Data Structures:
// ----------------------------------------------------

[StructLayout( LayoutKind.Sequential )]
public struct OutputDuplicationDescription {
	/// <summary>A <a href="https://docs.microsoft.com/previous-versions/windows/desktop/legacy/bb173064(v=vs.85)">DXGI_MODE_DESC</a> structure that describes the display mode of the duplicated output.</summary>
	public ModeDescription ModeDescription ;
	/// <summary>A member of the <a href="https://docs.microsoft.com/previous-versions/windows/desktop/legacy/bb173065(v=vs.85)">DXGI_MODE_ROTATION</a> enumerated type that describes how the duplicated output rotates an image.</summary>
	public ModeRotation Rotation ;
	/// <summary>Specifies whether the resource that contains the desktop image is already located in system memory. <b>TRUE</b> if the resource is in system memory; otherwise, <b>FALSE</b>. If this value is <b>TRUE</b> and  the application requires CPU access, it can use the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgioutputduplication-mapdesktopsurface">IDXGIOutputDuplication::MapDesktopSurface</a> and <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgioutputduplication-unmapdesktopsurface">IDXGIOutputDuplication::UnMapDesktopSurface</a> methods to avoid copying the data into a staging buffer.</summary>
	public bool DesktopImageInSystemMemory ;
	
	public OutputDuplicationDescription( in DXGI_OUTDUPL_DESC desc ) {
		this.ModeDescription = desc.ModeDesc ;
		this.Rotation = (ModeRotation)desc.Rotation ;
		this.DesktopImageInSystemMemory = desc.DesktopImageInSystemMemory ;
	}
	
	public static implicit operator OutputDuplicationDescription( in DXGI_OUTDUPL_DESC desc ) => new( desc ) ;
	public static implicit operator DXGI_OUTDUPL_DESC( in OutputDuplicationDescription desc ) => new( ) {
		ModeDesc = desc.ModeDescription,
		Rotation = (DXGI_MODE_ROTATION)desc.Rotation,
		DesktopImageInSystemMemory = desc.DesktopImageInSystemMemory
	} ;
} ;


[StructLayout( LayoutKind.Sequential )]
public struct OutputDuplicationPointerPosition {
	/// <summary>
	/// The position of the hardware cursor relative to the top-left of the adapter output.
	/// </summary>
	public Point Position ;

	/// <summary>
	/// Specifies whether the hardware cursor is visible. <b>TRUE</b> if visible; otherwise, <b>FALSE</b>.
	/// If the hardware cursor is not visible, the calling application does not display the cursor in the client.
	/// </summary>
	public bool Visible {
		get => _visible != 0 ;
		set => _visible = value ? 1 : 0 ;
	}
	internal BOOL _visible ;

	public OutputDuplicationPointerPosition( in DXGI_OUTDUPL_POINTER_POSITION pos ) {
		this.Position = pos.Position ;
		this._visible = pos.Visible ;
	}

	public static implicit operator OutputDuplicationPointerPosition( in DXGI_OUTDUPL_POINTER_POSITION pos ) => new( pos ) ;
	public static implicit operator DXGI_OUTDUPL_POINTER_POSITION( in OutputDuplicationPointerPosition pos ) => new( ) {
		Position = pos.Position,
		Visible  = pos.Visible
	} ;
} ;


[StructLayout(LayoutKind.Sequential)]
public struct OutputDuplicationFrameInfo {
	/// <summary>
	/// <para>The time stamp of the last update of the desktop image.  The operating system calls the <a href="https://docs.microsoft.com/windows/desktop/api/profileapi/nf-profileapi-queryperformancecounter">QueryPerformanceCounter</a> function to obtain the value. A zero value indicates that the desktop image was not updated since an application last called the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgioutputduplication-acquirenextframe">IDXGIOutputDuplication::AcquireNextFrame</a> method to acquire the next frame of the desktop image.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/ns-dxgi1_2-dxgi_outdupl_frame_info#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public long LastPresentTime ;

	/// <summary>
	/// <para>The time stamp of the last update to the mouse.  The operating system calls the <a href="https://docs.microsoft.com/windows/desktop/api/profileapi/nf-profileapi-queryperformancecounter">QueryPerformanceCounter</a> function to obtain the value. A zero value indicates that the position or shape of the mouse was not updated since an application last called the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nf-dxgi1_2-idxgioutputduplication-acquirenextframe">IDXGIOutputDuplication::AcquireNextFrame</a> method to acquire the next frame of the desktop image.  The mouse position is always supplied for a mouse update. A new pointer shape is indicated by a non-zero value in the <b>PointerShapeBufferSize</b> member.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/ns-dxgi1_2-dxgi_outdupl_frame_info#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public long LastMouseUpdateTime ;

	/// <summary>
	/// The number of frames that the operating system accumulated in the desktop image surface since
	/// the calling application processed the last desktop image.  For more information about this
	/// number, see Remarks.
	/// </summary>
	public uint AccumulatedFrames ;

	/// <summary>
	/// Specifies whether the operating system accumulated updates by coalescing dirty regions.
	/// Therefore, the dirty regions might contain unmodified pixels. <b>TRUE</b> if dirty regions
	/// were accumulated; otherwise, <b>FALSE</b>.
	/// </summary>
	public bool RectsCoalesced { get => _rectsCoalesced != 0 ; set => _rectsCoalesced = value ? 1 : 0 ; }
	internal BOOL _rectsCoalesced ;

	/// <summary>
	/// Specifies whether the desktop image might contain protected content that was already
	/// blacked out in the desktop image.  <b>TRUE</b> if protected content was already blacked;
	/// otherwise, <b>FALSE</b>. The application can use this information to notify the remote
	/// user that some of the desktop content might be protected and therefore not visible.
	/// </summary>
	public bool ProtectedContentMaskedOut { get => _protectedContentMaskedOut != 0 ; set => _protectedContentMaskedOut = value ? 1 : 0 ; }
	internal BOOL _protectedContentMaskedOut ;

	/// <summary>
	/// A <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ns-dxgi1_2-dxgi_outdupl_pointer_position">
	/// DXGI_OUTDUPL_POINTER_POSITION
	/// </a>
	/// structure that describes the most recent mouse position if the <b>LastMouseUpdateTime</b> member is a non-zero value;
	/// otherwise, this value is ignored. This value provides the coordinates of the location where the top-left-hand corner
	/// of the pointer shape is drawn; this value is not the desktop position of the hot spot.
	/// </summary>
	public OutputDuplicationPointerPosition PointerPosition ;

	/// <summary>
	/// Size in bytes of the buffers to store all the desktop update metadata for this frame.
	/// For more information about this size, see Remarks.
	/// </summary>
	public uint TotalMetadataBufferSize ;

	/// <summary>
	/// Size in bytes of the buffer to hold the new pixel data for the mouse shape.
	/// For more information about this size, see Remarks.
	/// </summary>
	public uint PointerShapeBufferSize ;
	
	public OutputDuplicationFrameInfo( in DXGI_OUTDUPL_FRAME_INFO info ) {
		this.LastPresentTime = info.LastPresentTime ;
		this.LastMouseUpdateTime = info.LastMouseUpdateTime ;
		this.AccumulatedFrames = info.AccumulatedFrames ;
		this._rectsCoalesced = info.RectsCoalesced ;
		this._protectedContentMaskedOut = info.ProtectedContentMaskedOut ;
		this.PointerPosition = info.PointerPosition ;
		this.TotalMetadataBufferSize = info.TotalMetadataBufferSize ;
		this.PointerShapeBufferSize = info.PointerShapeBufferSize ;
	}
	
	public static implicit operator OutputDuplicationFrameInfo( in DXGI_OUTDUPL_FRAME_INFO info ) => new( info ) ;
	public static implicit operator DXGI_OUTDUPL_FRAME_INFO( in OutputDuplicationFrameInfo info ) => new( ) {
		LastPresentTime = info.LastPresentTime,
		LastMouseUpdateTime = info.LastMouseUpdateTime,
		AccumulatedFrames = info.AccumulatedFrames,
		RectsCoalesced = info.RectsCoalesced,
		ProtectedContentMaskedOut = info.ProtectedContentMaskedOut,
		PointerPosition = info.PointerPosition,
		TotalMetadataBufferSize = info.TotalMetadataBufferSize,
		PointerShapeBufferSize = info.PointerShapeBufferSize
	} ;
} ;


[StructLayout(LayoutKind.Sequential)]
public struct OutputDuplicationMoveRect {
	/// <summary>The starting position of a rectangle.</summary>
	public Point SourcePoint ;

	/// <summary>The target region to which to move a rectangle.</summary>
	public Rect DestinationRect ;
	
	public OutputDuplicationMoveRect( Point srcPoint, Rect dstRect ) {
		this.SourcePoint = srcPoint ;
		this.DestinationRect = dstRect ;
	}
	public OutputDuplicationMoveRect( in DXGI_OUTDUPL_MOVE_RECT rect ) {
		this.SourcePoint = rect.SourcePoint ;
		this.DestinationRect = rect.DestinationRect ;
	}
	
	public static implicit operator OutputDuplicationMoveRect( in DXGI_OUTDUPL_MOVE_RECT rect ) => new( rect ) ;
	public static implicit operator DXGI_OUTDUPL_MOVE_RECT( in OutputDuplicationMoveRect rect ) => new( ) {
		SourcePoint = rect.SourcePoint,
		DestinationRect = rect.DestinationRect
	} ;
} ;


[StructLayout(LayoutKind.Sequential)]
public struct OutputDuplicationPointerShapeInfo {
	/// <summary>
	/// A <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ne-dxgi1_2-dxgi_outdupl_pointer_shape_type">
	/// DXGI_OUTDUPL_POINTER_SHAPE_TYPE</a>-typed value that specifies the type of cursor shape.
	/// </summary>
	public uint Type ;

	/// <summary>The width in pixels of the mouse cursor.</summary>
	public uint Width ;

	/// <summary>The height in scan lines of the mouse cursor.</summary>
	public uint Height ;

	/// <summary>The width in bytes of the mouse cursor.</summary>
	public uint Pitch ;

	/// <summary>
	/// The position of the cursor's hot spot relative to its upper-left pixel.
	/// An application does not use the hot spot when it determines where to
	/// draw the cursor shape.
	/// </summary>
	public Point HotSpot ;
	
	public OutputDuplicationPointerShapeInfo( uint type, uint width, uint height, uint pitch, Point hotSpot ) {
		this.Type = type ;
		this.Width = width ;
		this.Height = height ;
		this.Pitch = pitch ;
		this.HotSpot = hotSpot ;
	}
	internal OutputDuplicationPointerShapeInfo( in DXGI_OUTDUPL_POINTER_SHAPE_INFO info ) {
		this.Type = info.Type ;
		this.Width = info.Width ;
		this.Height = info.Height ;
		this.Pitch = info.Pitch ;
		this.HotSpot = info.HotSpot ;
	}
	
	public static implicit operator OutputDuplicationPointerShapeInfo( in DXGI_OUTDUPL_POINTER_SHAPE_INFO info ) => new( info ) ;
	public static implicit operator DXGI_OUTDUPL_POINTER_SHAPE_INFO( in OutputDuplicationPointerShapeInfo info ) => new( ) {
		Type = info.Type,
		Width = info.Width,
		Height = info.Height,
		Pitch = info.Pitch,
		HotSpot = info.HotSpot
	} ;
}


// ====================================================

/// <summary>Identifies the type of DXGI adapter.</summary>
[EquivalentOf(typeof(DXGI_ADAPTER_FLAG))]
public enum AdapterFlag: uint {
	/// <summary>Specifies no flags.</summary>
	None = 0,
	
	/// <summary>Value always set to 0. This flag is reserved.</summary>
	Remote = 1,
	
	/// <summary>
	/// <para>Specifies a software adapter. For more info about this flag, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/d3d10-graphics-programming-guide-dxgi">new info in Windows 8 about enumerating adapters</a>. <b>Direct3D 11:  </b>This enumeration value is supported starting with Windows 8.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/ne-dxgi-dxgi_adapter_flag#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Software = 2,
	
	/// <summary>
	/// Forces this enumeration to compile to 32 bits in size. Without this value, some compilers would allow this enumeration to compile
	/// to a size other than 32 bits. This value is not used by applications.
	/// </summary>
	/// <remarks>Exists to match native enumeration definition.</remarks>
	ForceDWord = 0xffffffff
} ;



/// <summary>Identifies the type of DXGI adapter.</summary>
[EquivalentOf( typeof( AdapterFlag3 ) )]
public enum AdapterFlag3: uint {
	/// <summary>Specifies no flags.</summary>
	None = 0x00000000,

	/// <summary>Value always set to 0. This flag is reserved.</summary>
	Remote = 0x00000001,

	/// <summary>
	/// <para>Specifies a software adapter. For more info about this flag, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/d3d10-graphics-programming-guide-dxgi">new info in Windows 8 about enumerating adapters</a>. <b>Direct3D 11:  </b>This enumeration value is supported starting with Windows 8.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/ne-dxgi1_6-dxgi_adapter_flag3#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	Software = 0x00000002,

	/// <summary>Specifies that the adapter's driver has been confirmed to work in an OS process where Arbitrary Code Guard (ACG) is enabled (i.e. dynamic code generation is disallowed).</summary>
	ACGCompatible = 0x00000004,

	/// <summary>Specifies that the adapter supports monitored fences. These adapters support the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12device">ID3D12Device::CreateFence</a> and <a href="https://docs.microsoft.com/windows/desktop/api/d3d11_4/nn-d3d11_4-id3d11device5">ID3D11Device5::CreateFence</a> functions.</summary>
	SupportMonitoredFences = 0x00000008,

	/// <summary>
	/// <para>Specifies that the adapter supports non-monitored fences. These adapters support the
	/// <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nn-d3d12-id3d12device">ID3D12Device::CreateFence</a>
	/// function together with the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_fence_flags">D3D12_FENCE_FLAG_NON_MONITORED</a>
	/// flag. <div class="alert"><b>Note</b>  For adapters that support both monitored and non-monitored fences, non-monitored fences are only supported
	/// when created with the <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_fence_flags">D3D12_FENCE_FLAG_SHARED</a> and
	/// <b>D3D12_FENCE_FLAG_SHARED_CROSS_ADAPTER</b> flags. Monitored fences should always be used by supporting adapters unless communicating with an adapter
	/// that only supports non-monitored fences.</div> <div> </div></para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_6/ne-dxgi1_6-dxgi_adapter_flag3#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	SupportNonMonitoredFences = 0x00000010,

	/// <summary>
	/// Specifies that the adapter claims keyed mutex conformance. This signals a stronger guarantee that the
	/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgikeyedmutex">IDXGIKeyedMutex</a>
	/// interface behaves correctly.
	/// </summary>
	KeyedMutexConformance = 0x00000020,
} ;



[StructLayout( LayoutKind.Sequential )]
public struct AdapterDescription1: IEquatable< AdapterDescription1 > {
	
	/// <summary>
	/// <para>Type: <b>WCHAR[128]</b> A string that contains the adapter description. On <a href="https://docs.microsoft.com/windows/desktop/direct3d11/overviews-direct3d-11-devices-downlevel-intro">feature level</a> 9 graphics hardware, <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiadapter1-getdesc1">GetDesc1</a> returns “Software Adapter” for the description string.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/ns-dxgi-dxgi_adapter_desc1#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public FixedStr128 Description ;

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The PCI ID of the hardware vendor. On <a href="https://docs.microsoft.com/windows/desktop/direct3d11/overviews-direct3d-11-devices-downlevel-intro">feature level</a> 9 graphics hardware, <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiadapter1-getdesc1">GetDesc1</a> returns zeros for the PCI ID of the hardware vendor.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/ns-dxgi-dxgi_adapter_desc1#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint VendorId ;

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The PCI ID of the hardware device. On <a href="https://docs.microsoft.com/windows/desktop/direct3d11/overviews-direct3d-11-devices-downlevel-intro">feature level</a> 9 graphics hardware, <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiadapter1-getdesc1">GetDesc1</a> returns zeros for the PCI ID of the hardware device.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/ns-dxgi-dxgi_adapter_desc1#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint DeviceId ;

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The PCI ID of the sub system. On <a href="https://docs.microsoft.com/windows/desktop/direct3d11/overviews-direct3d-11-devices-downlevel-intro">feature level</a> 9 graphics hardware, <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiadapter1-getdesc1">GetDesc1</a> returns zeros for the PCI ID of the sub system.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/ns-dxgi-dxgi_adapter_desc1#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint SubSysId ;

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The PCI ID of the revision number of the adapter. On <a href="https://docs.microsoft.com/windows/desktop/direct3d11/overviews-direct3d-11-devices-downlevel-intro">feature level</a> 9 graphics hardware, <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiadapter1-getdesc1">GetDesc1</a> returns zeros for the PCI ID of the revision number of the adapter.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/ns-dxgi-dxgi_adapter_desc1#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint Revision ;

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">SIZE_T</a></b> The number of bytes of dedicated video memory that are not shared with the CPU.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/ns-dxgi-dxgi_adapter_desc1#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public nuint DedicatedVideoMemory ;

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">SIZE_T</a></b> The number of bytes of dedicated system memory that are not shared with the CPU. This memory is allocated from available system memory at boot time.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/ns-dxgi-dxgi_adapter_desc1#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public nuint DedicatedSystemMemory ;

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">SIZE_T</a></b> The number of bytes of shared system memory. This is the maximum value of system memory that may be consumed by the adapter during operation. Any incidental memory consumed by the driver as it manages and uses video memory is additional.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/ns-dxgi-dxgi_adapter_desc1#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public nuint SharedSystemMemory ;

	/// <summary>
	/// <para>Type: <b><a href="https://docs.microsoft.com/previous-versions/windows/hardware/drivers/ff549708(v=vs.85)">LUID</a></b> A unique value that identifies the adapter. See <a href="https://docs.microsoft.com/previous-versions/windows/hardware/drivers/ff549708(v=vs.85)">LUID</a> for a definition of the structure. <b>LUID</b> is defined in dxgi.h.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi/ns-dxgi-dxgi_adapter_desc1#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public Luid AdapterLuid ;

	/// <summary>
	/// <para>Type:
	/// <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b>
	/// A value of the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/ne-dxgi-dxgi_adapter_flag">DXGI_ADAPTER_FLAG</a>
	/// enumerated type that describes the adapter type. The <b>DXGI_ADAPTER_FLAG_REMOTE</b> flag is reserved.</para>
	/// <para>
	/// <a href="https://docs.microsoft.com/windows/win32/api/dxgi/ns-dxgi-dxgi_adapter_desc1#members">Read more on docs.microsoft.com</a>.
	/// </para>
	/// </summary>
	public AdapterFlag Flags ;
	
	public AdapterDescription1( in DXGI_ADAPTER_DESC1 desc ) {
		this.Description           = new( desc.Description ) ;
		this.VendorId              = desc.VendorId ;
		this.DeviceId              = desc.DeviceId ;
		this.SubSysId              = desc.SubSysId ;
		this.Revision              = desc.Revision ;
		this.DedicatedVideoMemory  = (nuint)desc.DedicatedVideoMemory ;
		this.DedicatedSystemMemory = (nuint)desc.DedicatedSystemMemory ;
		this.SharedSystemMemory    = (nuint)desc.SharedSystemMemory ;
		this.AdapterLuid           = desc.AdapterLuid ;
		this.Flags                 = (AdapterFlag)desc.Flags ;
	}
	
	
	
	public override bool Equals( object? obj ) => 
		obj is AdapterDescription1 other && other == this ;
	public bool Equals( AdapterDescription1 other ) => other == this ;
	public override int GetHashCode( ) {
		HashCode hashCode = new HashCode( ) ;
		hashCode.Add( Description ) ;
		hashCode.Add( VendorId ) ;
		hashCode.Add( DeviceId ) ;
		hashCode.Add( SubSysId ) ;
		hashCode.Add( Revision ) ;
		hashCode.Add( DedicatedVideoMemory ) ;
		hashCode.Add( DedicatedSystemMemory ) ;
		hashCode.Add( SharedSystemMemory ) ;
		hashCode.Add( AdapterLuid ) ;
		hashCode.Add( (int)Flags ) ;
		return hashCode.ToHashCode( ) ;
	}

	
	public static implicit operator AdapterDescription1( in DXGI_ADAPTER_DESC1 desc ) => new( desc ) ;
	public static implicit operator DXGI_ADAPTER_DESC1( in AdapterDescription1 desc ) => new( ) {
		Description           = desc.Description,
		VendorId              = desc.VendorId,
		DeviceId              = desc.DeviceId,
		SubSysId              = desc.SubSysId,
		Revision              = desc.Revision,
		DedicatedVideoMemory  = (uint)desc.DedicatedVideoMemory,
		DedicatedSystemMemory = (uint)desc.DedicatedSystemMemory,
		SharedSystemMemory    = (uint)desc.SharedSystemMemory,
		AdapterLuid           = desc.AdapterLuid,
		Flags                 = (uint)desc.Flags
	} ;
	
	public static bool operator ==( in AdapterDescription1 a, in AdapterDescription1 b ) => a.Description == b.Description && a.VendorId == b.VendorId && a.DeviceId == b.DeviceId && a.SubSysId == b.SubSysId && a.Revision == b.Revision && a.DedicatedVideoMemory == b.DedicatedVideoMemory && a.DedicatedSystemMemory == b.DedicatedSystemMemory && a.SharedSystemMemory == b.SharedSystemMemory && a.AdapterLuid == b.AdapterLuid && a.Flags == b.Flags ;
	public static bool operator !=( in AdapterDescription1 a, in AdapterDescription1 b ) => a.Description != b.Description || a.VendorId != b.VendorId || a.DeviceId != b.DeviceId || a.SubSysId != b.SubSysId || a.Revision != b.Revision || a.DedicatedVideoMemory != b.DedicatedVideoMemory || a.DedicatedSystemMemory != b.DedicatedSystemMemory || a.SharedSystemMemory != b.SharedSystemMemory || a.AdapterLuid != b.AdapterLuid || a.Flags != b.Flags ;
} ;


[ProxyFor(typeof(DXGI_ADAPTER_DESC2))]
public struct AdapterDescription2: IEquatable< AdapterDescription2 > { 
	AdapterDescription1 _description1 ;
	public FixedStr128 Description { 
		get => _description1.Description ;
		set => _description1.Description = value ;
	}
	public uint VendorId { 
		get => _description1.VendorId ;
		set => _description1.VendorId = value ;
	}
	public uint DeviceId {
		get => _description1.DeviceId ;
		set => _description1.DeviceId = value ;
	}
	public uint SubSysId {
		get => _description1.SubSysId ;
		set => _description1.SubSysId = value ;
	}
	public uint Revision {
		get => _description1.Revision ;
		set => _description1.Revision = value ;
	}
	public nuint DedicatedVideoMemory {
		get => _description1.DedicatedVideoMemory ;
		set => _description1.DedicatedVideoMemory = value ;
	}
	public nuint DedicatedSystemMemory {
		get => _description1.DedicatedSystemMemory ;
		set => _description1.DedicatedSystemMemory = value ;
	}
	public nuint SharedSystemMemory {
		get => _description1.SharedSystemMemory ;
		set => _description1.SharedSystemMemory = value ;
	}
	public Luid AdapterLuid {
		get => _description1.AdapterLuid ;
		set => _description1.AdapterLuid = value ;
	}
	public AdapterFlag Flags {
		get => _description1.Flags ;
		set => _description1.Flags = value ;
	}
	public GraphicsPreemptionGranularity GraphicsPreemptionGranularity ;
	public ComputePreemptionGranularity ComputePreemptionGranularity ;
	
	internal AdapterDescription1 Description1 => this._description1 ;
	
	public AdapterDescription2( in DXGI_ADAPTER_DESC2 desc ) {
		unsafe { fixed ( DXGI_ADAPTER_DESC2* src = &desc ) {
				this._description1                  = *(AdapterDescription1*)src ;
				this.GraphicsPreemptionGranularity = (GraphicsPreemptionGranularity)desc.GraphicsPreemptionGranularity ;
				this.ComputePreemptionGranularity  = (ComputePreemptionGranularity)desc.ComputePreemptionGranularity ;
			}
		}
	}
	
	
	public override int GetHashCode( ) =>
		HashCode.Combine( _description1,
						  (int)GraphicsPreemptionGranularity,
						  (int)ComputePreemptionGranularity ) ;
	public override bool Equals( object? obj ) => 
		obj is AdapterDescription2 other && other == this ;
	public bool Equals( AdapterDescription2 other ) => other == this ;
	
	
	
	public static implicit operator AdapterDescription2( in DXGI_ADAPTER_DESC2 desc ) => new( desc ) ;
	public static implicit operator DXGI_ADAPTER_DESC2( in AdapterDescription2 desc ) => new( ) {
		Description           = desc.Description,
		VendorId              = desc.VendorId,
		DeviceId              = desc.DeviceId,
		SubSysId              = desc.SubSysId,
		Revision              = desc.Revision,
		DedicatedVideoMemory  = (uint)desc.DedicatedVideoMemory,
		DedicatedSystemMemory = (uint)desc.DedicatedSystemMemory,
		SharedSystemMemory    = (uint)desc.SharedSystemMemory,
		AdapterLuid           = desc.AdapterLuid,
		Flags                 = (uint)desc.Flags,
		GraphicsPreemptionGranularity = (DXGI_GRAPHICS_PREEMPTION_GRANULARITY)desc.GraphicsPreemptionGranularity,
		ComputePreemptionGranularity  = (DXGI_COMPUTE_PREEMPTION_GRANULARITY)desc.ComputePreemptionGranularity
	} ;
	
	
	public static bool operator ==( in AdapterDescription2 a, in AdapterDescription2 b ) => 
		a._description1 == b._description1 && a.GraphicsPreemptionGranularity == b.GraphicsPreemptionGranularity 
										 && a.ComputePreemptionGranularity == b.ComputePreemptionGranularity ;
	public static bool operator !=( in AdapterDescription2 a, in AdapterDescription2 b ) => 
		a._description1 != b._description1 || a.GraphicsPreemptionGranularity != b.GraphicsPreemptionGranularity 
										 || a.ComputePreemptionGranularity != b.ComputePreemptionGranularity ;
} ;


/// <summary>Describes an adapter (or video card) that uses Microsoft DirectX Graphics Infrastructure (DXGI) 1.6.</summary>
/// <remarks>
/// The <b>DXGI_ADAPTER_DESC3</b> structure provides a DXGI 1.6 description of an adapter.
/// This structure is initialized by using the <see cref="IAdapter4.GetDesc3"/> method.<para/>
/// 
/// <para>Learn more about:</para>
/// The native 
/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_6/ns-dxgi1_6-dxgi_adapter_desc3">DXGI_ADAPTER_DESC3</a> structure and the
/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_6/nf-dxgi1_6-idxgiadapter4-getdesc3">IDXGIAdapter4::GetDesc3</a> method.
/// </remarks>
[ProxyFor(typeof(DXGI_ADAPTER_DESC3))]
public partial struct AdapterDescription3
{
	AdapterDescription2 _description2 ;
	
	/// <summary>A string that contains the adapter description.</summary>
	public FixedStr128 Description {
		get => _description2.Description ;
		set => _description2.Description = value ;
	}

	/// <summary>The PCI ID of the hardware vendor.</summary>
	public uint VendorId {
		get => _description2.VendorId ;
		set => _description2.VendorId = value ;
	}

	/// <summary>The PCI ID of the hardware device.</summary>
	public uint DeviceId {
		get => _description2.DeviceId ;
		set => _description2.DeviceId = value ;
	}

	/// <summary>The PCI ID of the sub system.</summary>
	public uint SubSysId {
		get => _description2.SubSysId ;
		set => _description2.SubSysId = value ;
	}

	/// <summary>The PCI ID of the revision number of the adapter.</summary>
	public uint Revision {
		get => _description2.Revision ;
		set => _description2.Revision = value ;
	}

	/// <summary>The number of bytes of dedicated video memory that are not shared with the CPU.</summary>
	public nuint DedicatedVideoMemory {
		get => _description2.DedicatedVideoMemory ;
		set => _description2.DedicatedVideoMemory = value ;
	}

	/// <summary>The number of bytes of dedicated system memory that are not shared with the CPU. This memory is allocated from available system memory at boot time.</summary>
	public nuint DedicatedSystemMemory {
		get => _description2.DedicatedSystemMemory ;
		set => _description2.DedicatedSystemMemory = value ;
	}

	/// <summary>The number of bytes of shared system memory. This is the maximum value of system memory that may be consumed by the adapter during operation. Any incidental memory consumed by the driver as it manages and uses video memory is additional.</summary>
	public nuint SharedSystemMemory {
		get => _description2.SharedSystemMemory ;
		set => _description2.SharedSystemMemory = value ;
	}

	/// <summary>A unique value that identifies the adapter. See <a href="https://docs.microsoft.com/previous-versions/windows/hardware/drivers/ff549708(v=vs.85)">LUID</a> for a definition of the structure. <b>LUID</b> is defined in dxgi.h.</summary>
	public Luid AdapterLuid {
		get => _description2.AdapterLuid ;
		set => _description2.AdapterLuid = value ;
	}

	/// <summary>A value of the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_6/ne-dxgi1_6-dxgi_adapter_flag3">DXGI_ADAPTER_FLAG3</a> enumeration that describes the adapter type.  The <b>DXGI_ADAPTER_FLAG_REMOTE</b> flag is reserved.</summary>
	public AdapterFlag3 Flags {
		get => (AdapterFlag3)_description2.Flags ;
		set => _description2.Flags = (AdapterFlag)value ;
	}

	/// <summary>A value of the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ne-dxgi1_2-dxgi_graphics_preemption_granularity">DXGI_GRAPHICS_PREEMPTION_GRANULARITY</a> enumerated type that describes the granularity level at which the GPU can be preempted from performing its current graphics rendering task.</summary>
	public GraphicsPreemptionGranularity GraphicsPreemptionGranularity {
		get => _description2.GraphicsPreemptionGranularity ;
		set => _description2.GraphicsPreemptionGranularity = value ;
	}

	/// <summary>A value of the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/ne-dxgi1_2-dxgi_compute_preemption_granularity">DXGI_COMPUTE_PREEMPTION_GRANULARITY</a> enumerated type that describes the granularity level at which the GPU can be preempted from performing its current compute task.</summary>
	public ComputePreemptionGranularity ComputePreemptionGranularity {
		get => _description2.ComputePreemptionGranularity ;
		set => _description2.ComputePreemptionGranularity = value ;
	}
	
	public AdapterDescription3( FixedStr128 description,
								uint vendorId, uint deviceId, uint subSysId, uint revision, 
								nuint dedicatedVideoMemory, nuint dedicatedSystemMemory, nuint sharedSystemMemory,
								Luid adapterLuid, AdapterFlag3 flags,
								GraphicsPreemptionGranularity graphicsPreemptionGranularity, 
								ComputePreemptionGranularity computePreemptionGranularity ) {
		this._description2 = new AdapterDescription2( ) {
			Description           = description,
			VendorId              = vendorId,
			DeviceId              = deviceId,
			SubSysId              = subSysId,
			Revision              = revision,
			DedicatedVideoMemory  = dedicatedVideoMemory,
			DedicatedSystemMemory = dedicatedSystemMemory,
			SharedSystemMemory    = sharedSystemMemory,
			AdapterLuid           = adapterLuid,
			Flags                 = (AdapterFlag)flags,
			GraphicsPreemptionGranularity = graphicsPreemptionGranularity,
			ComputePreemptionGranularity  = computePreemptionGranularity
		} ;
	}
	
}


/// <summary>Describes the current video memory budgeting parameters.</summary>
/// <remarks>
/// <para>Use this structure with <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_4/nf-dxgi1_4-idxgiadapter3-queryvideomemoryinfo">QueryVideoMemoryInfo</a>. Refer to the remarks for <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_memory_pool">D3D12_MEMORY_POOL</a>.</para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_4/ns-dxgi1_4-dxgi_query_video_memory_info#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(DXGI_QUERY_VIDEO_MEMORY_INFO))]
public partial struct QueryVideoMemoryInfo {
	/// <summary>Specifies the OS-provided video memory budget, in bytes, that the application should target. If <i>CurrentUsage</i> is greater than <i>Budget</i>, the application may incur stuttering or performance penalties due to background activity by the OS to provide other applications with a fair usage of video memory.</summary>
	public ulong Budget ;

	/// <summary>Specifies the application’s current video memory usage, in bytes.</summary>
	public ulong CurrentUsage ;

	/// <summary>The amount of video memory, in bytes, that the application has available for reservation. To reserve this video memory, the application should call <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_4/nf-dxgi1_4-idxgiadapter3-setvideomemoryreservation">IDXGIAdapter3::SetVideoMemoryReservation</a>.</summary>
	public ulong AvailableForReservation ;

	/// <summary>The amount of video memory, in bytes, that is reserved by the application. The OS uses the reservation as a hint to determine the application’s minimum working set. Applications should attempt to ensure that their video memory usage can be trimmed to meet this requirement.</summary>
	public ulong CurrentReservation ;
	
	
	public QueryVideoMemoryInfo( ulong budget = 0UL, 
								 ulong currentUsage = 0UL, 
								 ulong availableForReservation = 0UL, 
								 ulong currentReservation = 0UL ) {
		this.Budget = budget ;
		this.CurrentUsage = currentUsage ;
		this.AvailableForReservation = availableForReservation ;
		this.CurrentReservation = currentReservation ;
	}
}


/// <summary>Used with IDXGIFactoryMedia::CreateDecodeSwapChainForCompositionSurfaceHandle to describe a decode swap chain.</summary>
/// <remarks>
/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/ns-dxgi1_3-dxgi_decode_swap_chain_desc">Learn more about this API from docs.microsoft.com</see>.</para>
/// </remarks>
[EquivalentOf(typeof(DXGI_DECODE_SWAP_CHAIN_DESC))]
public partial struct DecodeSwapChainDescription {
	public const SwapChainFlags DecodeSwapChainFlags = SwapChainFlags.FullscreenVideo | SwapChainFlags.YUVVideo ;
	public static readonly DecodeSwapChainDescription Default = new( DecodeSwapChainFlags ) ;
	
	/// <summary>
	/// <para>Can be 0, or a combination of **DXGI_SWAP_CHAIN_FLAG_FULLSCREEN_VIDEO** and/or **DXGI_SWAP_CHAIN_FLAG_YUV_VIDEO**.
	/// Those named values are members of the <a href="https://docs.microsoft.com/windows/win32/api/dxgi/ne-dxgi-dxgi_swap_chain_flag">DXGI_SWAP_CHAIN_FLAG</a> enumerated type, and you can combine them by using a bitwise OR operation. The resulting value specifies options for decode swap-chain behavior.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_3/ns-dxgi1_3-dxgi_decode_swap_chain_desc#members">Read more on docs.microsoft.com</a>.</para>
	/// </summary>
	public uint flags ;
	
	/// <summary>Gets or sets the underlying flags value.</summary>
	public SwapChainFlags Flags {
		get => (SwapChainFlags)flags ;
		set => flags = (uint)value ;
	}
	
	public DecodeSwapChainDescription( uint flags = (uint)DecodeSwapChainFlags ) {
		this.flags = flags ;
	}
	public DecodeSwapChainDescription( SwapChainFlags flags = DecodeSwapChainFlags ) {
		this.flags = (uint)flags ;
	}
	
	public static implicit operator SwapChainFlags( DecodeSwapChainDescription desc ) => (SwapChainFlags)desc.flags ;
	public static implicit operator DecodeSwapChainDescription( SwapChainFlags flags ) => new( (uint)flags ) ;
	public static implicit operator DecodeSwapChainDescription( uint flags ) => new( flags ) ;
	public static implicit operator uint( DecodeSwapChainDescription desc ) => desc.flags ;
} ;


/// <summary>
/// Describes the metadata for HDR10, used when video is compressed using High Efficiency Video Coding (HEVC).
/// This is used to describe the capabilities of the display used to master the content and the luminance values of the content.
/// </summary>
/// <remarks>
/// <para>
/// This structure represents the definition of HDR10 metadata used with HEVC, not HDR10 metadata for ST.2086.
/// These are closely related but defined differently. Example: Mastering display with DCI-P3 color primaries and D65
/// white point, maximum luminance of 1000 nits and minimum luminance of 0.001 nits; content has maximum luminance of
/// 2000 nits and maximum frame average light level (MaxFALL) of 500 nits.
/// </para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgi1_5/ns-dxgi1_5-dxgi_hdr_metadata_hdr10#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf(typeof(DXGI_HDR_METADATA_HDR10))]
public struct HDRMetaDataHDR10: IEquatable<HDRMetaDataHDR10> {
	public static readonly HDRMetaDataHDR10 Default = new( ) ;
	
	/// <summary>The chromaticity coordinates of the red value in the CIE1931 color space. Index 0 contains the X coordinate and index 1 contains the Y coordinate. The values are normalized to 50,000.</summary>
	public ushort2 RedPrimary ;

	/// <summary>The chromaticity coordinates of the green value in the CIE1931 color space. Index 0 contains the X coordinate and index 1 contains the Y coordinate. The values are normalized to 50,000.</summary>
	public ushort2 GreenPrimary ;

	/// <summary>The chromaticity coordinates of the blue value in the CIE1931 color space. Index 0 contains the X coordinate and index 1 contains the Y coordinate. The values are normalized to 50,000.</summary>
	public ushort2 BluePrimary ;

	/// <summary>The chromaticity coordinates of the white point in the CIE1931 color space. Index 0 contains the X coordinate and index 1 contains the Y coordinate. The values are normalized to 50,000.</summary>
	public ushort2 WhitePoint ;

	/// <summary>The maximum number of nits of the display used to master the content. Values are in whole nits.</summary>
	public uint MaxMasteringLuminance ;

	/// <summary>The minimum number of nits of the display used to master the content. Values are 1/10000th of a nit (0.0001 nit).</summary>
	public uint MinMasteringLuminance ;

	/// <summary>The maximum content light level (MaxCLL). This is the nit value corresponding to the brightest pixel used anywhere in the content.</summary>
	public ushort MaxContentLightLevel ;

	/// <summary>The maximum frame average light level (MaxFALL). This is the nit value corresponding to the average luminance of the frame which has the brightest average luminance anywhere in the content.</summary>
	public ushort MaxFrameAverageLightLevel ;
	
	
	public HDRMetaDataHDR10( ushort2 redPrimary, 
							 ushort2 greenPrimary, 
							 ushort2 bluePrimary, 
							 ushort2 whitePoint, 
							 uint maxMasteringLuminance, 
							 uint minMasteringLuminance, 
							 ushort maxContentLightLevel, 
							 ushort maxFrameAverageLightLevel ) {
		this.RedPrimary = redPrimary ;
		this.GreenPrimary = greenPrimary ;
		this.BluePrimary = bluePrimary ;
		this.WhitePoint = whitePoint ;
		this.MaxMasteringLuminance = maxMasteringLuminance ;
		this.MinMasteringLuminance = minMasteringLuminance ;
		this.MaxContentLightLevel = maxContentLightLevel ;
		this.MaxFrameAverageLightLevel = maxFrameAverageLightLevel ;
	}
	
	public static bool operator==( in HDRMetaDataHDR10 a, in HDRMetaDataHDR10 b ) =>
		a.RedPrimary == b.RedPrimary && a.GreenPrimary == b.GreenPrimary && a.BluePrimary == b.BluePrimary && a.WhitePoint == b.WhitePoint &&
		a.MaxMasteringLuminance == b.MaxMasteringLuminance && a.MinMasteringLuminance == b.MinMasteringLuminance &&
		a.MaxContentLightLevel == b.MaxContentLightLevel && a.MaxFrameAverageLightLevel == b.MaxFrameAverageLightLevel ;
	public static bool operator!=( in HDRMetaDataHDR10 a, in HDRMetaDataHDR10 b ) =>
	 		a.RedPrimary != b.RedPrimary || a.GreenPrimary != b.GreenPrimary || a.BluePrimary != b.BluePrimary || a.WhitePoint != b.WhitePoint ||
		a.MaxMasteringLuminance != b.MaxMasteringLuminance || a.MinMasteringLuminance != b.MinMasteringLuminance ||
		a.MaxContentLightLevel != b.MaxContentLightLevel || a.MaxFrameAverageLightLevel != b.MaxFrameAverageLightLevel ;

	public bool Equals( HDRMetaDataHDR10 other ) => other == this ;
	
	public override bool Equals( object? obj ) => obj is HDRMetaDataHDR10 other && Equals( other ) ;

	public override int GetHashCode( ) => HashCode.Combine( RedPrimary, GreenPrimary, BluePrimary, 
															WhitePoint, MaxMasteringLuminance, MinMasteringLuminance, 
															MaxContentLightLevel, MaxFrameAverageLightLevel ) ;

} ;


/// <summary>Describes a debug message filter, which contains lists of message types to allow and deny.</summary>
/// <remarks>
/// <para>Use with an <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/nn-dxgidebug-idxgiinfoqueue">IDXGIInfoQueue</a> interface.</para>
/// <para><div class="alert"><b>Note:</b> This API requires the Windows Software Development Kit (SDK) for Windows 8.</div><div></div></para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgidebug/ns-dxgidebug-dxgi_info_queue_filter#">Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[EquivalentOf( typeof( DXGI_INFO_QUEUE_FILTER ) )]
public partial struct InfoQueueFilter {
	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/ns-dxgidebug-dxgi_info_queue_filter_desc">DXGI_INFO_QUEUE_FILTER_DESC</a> structure that describes the types of messages to allow.</summary>
	public InfoQueueFilterDescription AllowList ;

	/// <summary>A <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/ns-dxgidebug-dxgi_info_queue_filter_desc">DXGI_INFO_QUEUE_FILTER_DESC</a> structure that describes the types of messages to deny.</summary>
	public InfoQueueFilterDescription DenyList ;


	/// <summary>
	/// <para>Initializes a new instance of the <see cref="InfoQueueFilter"/> struct.</para>
	/// This is done by setting the <see cref="AllowList"/> and <see cref="DenyList"/> using
	/// a <see cref="InfoQueueFilterDescription"/> for each.
	/// </summary>
	/// <param name="allowList"><para>A <see cref="InfoQueueFilterDescription"/> that describes the types of messages to allow.</para></param>
	/// <param name="denyList"><para>A <see cref="InfoQueueFilterDescription"/> that describes the types of messages to deny.</para></param>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/ns-dxgidebug-dxgi_info_queue_filter#members">
	/// Read more on docs.microsoft.com
	/// </see>.</para>
	/// </remarks>
	public InfoQueueFilter( in InfoQueueFilterDescription allowList = default,
								in InfoQueueFilterDescription denyList = default ) {
		this.AllowList = allowList ;
		this.DenyList  = denyList ;
	}
	
	/// <summary>Computes total memory required for the filter.</summary>
	/// <returns>
	/// <para>Returns the total memory required for the filter.</para>
	/// <para>See the 
	/// <a href="https://docs.microsoft.com/windows/win32/api/dxgidebug/ns-dxgidebug-dxgi_info_queue_filter#members">DXGI_INFO_QUEUE_FILTER structure</a>
	/// documentation for more information about the structure <see cref="InfoQueueFilterDescription"/> is based upon.
	/// </para>
	/// </returns>
	/// <remarks>
	/// <para>This finds the total size of the filter, including the size of the two <see cref="InfoQueueFilterDescription"/> structs.</para>
	/// Although this package offers a lot of help with memory management, it is still important to know how managed vs. unmanaged memory works in .NET,
	/// because these structs store pointers to unmanaged memory or pinned managed memory in some cases.<para/>
	/// See <a href="https://docs.microsoft.com/en-us/dotnet/standard/automatic-memory-management">Automatic Memory Management</a>
	/// to understand how managed "garbage collected" memory works in .NET Core.
	/// See <a href="https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/unmanaged">Cleaning up unmanaged resources</a>
	/// to learn more about unmanaged memory and cleanup in .NET Core. 
	/// See <a href="https://docs.microsoft.com/en-us/dotnet/standard/native-interop/pinvoke">Platform Invoke</a>
	/// to learn more about interoperability in .NET Core (i.e., interaction with external native code, such as Windows DLLs).
	/// <para/>
	/// </remarks>
	public unsafe ulong GetAllocatedSize( ) {
		//! account for the two InfoQueueFilterDescription structs:
		int descSize = sizeof(InfoQueueFilterDescription) 
							* 2 ;
		
		ulong categoriesSize = 0UL ;
		if ( AllowList.pCategoryList is not null ) {
			var categories = AllowList.CategoryList ;
			categoriesSize = ( sizeof( InfoQueueMessageCategory ) 
							   * (ulong)categories.Length ) ;
		}
		
		ulong severitiesSize = 0UL ;
		if ( AllowList.pSeverityList is not null ) {
			var severities = AllowList.SeverityList ;
			severitiesSize = ( sizeof( InfoQueueMessageSeverity ) 
							   * (ulong)severities.Length ) ;
		}
		
		ulong idsSize = 0UL ;
		if ( AllowList.pIDList is not null ) {
			var ids = AllowList.IDList ;
			idsSize = (ulong)( sizeof( ulong ) * ids.Length ) ;
		}
		
		ulong denyCategoriesSize = 0UL ;
		if ( DenyList.pCategoryList is not null ) {
			var categories = DenyList.CategoryList ;
			denyCategoriesSize = ( sizeof( InfoQueueMessageCategory ) 
								   * (ulong)categories.Length ) ;
		}
		
		ulong denySeveritiesSize = 0UL ;
		if ( DenyList.pSeverityList is not null ) {
			var severities = DenyList.SeverityList ;
			denySeveritiesSize = ( sizeof(InfoQueueMessageSeverity) 
								   * (ulong)severities.Length ) ;
		}
		
		ulong denyIdsSize = 0UL ;
		if ( DenyList.pIDList is not null ) {
			var ids = DenyList.IDList ;
			denyIdsSize = ( sizeof(int) 
							* (ulong)ids.Length ) ;
		}
		
		return ( (ulong)descSize
					   + (categoriesSize + severitiesSize + idsSize)
					   + (denyCategoriesSize + denySeveritiesSize + denyIdsSize) ) ;
	}
} ;



/// <summary>Describes the types of messages to allow or deny to pass through a filter.</summary>
/// <remarks>
/// <para>This structure is a member of the
/// <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/ns-dxgidebug-dxgi_info_queue_filter">DXGI_INFO_QUEUE_FILTER</a> structure.
/// This API requires the Windows Software Development Kit (SDK) for Windows 8.</para>
/// <para><a href="https://docs.microsoft.com/windows/win32/api/dxgidebug/ns-dxgidebug-dxgi_info_queue_filter_desc#">
/// Read more on docs.microsoft.com</a>.</para>
/// </remarks>
[DebuggerDisplay("ToString()")]
[EquivalentOf(typeof(DXGI_INFO_QUEUE_FILTER_DESC))]
public partial struct InfoQueueFilterDescription {
	/// <summary>The number of message categories to allow or deny.</summary>
	public uint NumCategories ;

	/// <summary>An array of <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/ne-dxgidebug-dxgi_info_queue_message_category">DXGI_INFO_QUEUE_MESSAGE_CATEGORY</a> enumeration values that describe the message categories to allow or deny. The array must have at least <b>NumCategories</b> number of elements.</summary>
	public unsafe InfoQueueMessageCategory* pCategoryList ;
	[UnscopedRef] public unsafe Span< InfoQueueMessageCategory > CategoryList =>
										new( pCategoryList, (int)NumCategories ) ;
	
	/// <summary>The number of message severity levels to allow or deny.</summary>
	public uint NumSeverities ;

	/// <summary>An array of <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/ne-dxgidebug-dxgi_info_queue_message_severity">DXGI_INFO_QUEUE_MESSAGE_SEVERITY</a> enumeration values that describe the message severity levels to allow or deny. The array must have at least <b>NumSeverities</b> number of elements.</summary>
	public unsafe InfoQueueMessageSeverity* pSeverityList ;
	[UnscopedRef] public unsafe Span< InfoQueueMessageSeverity > SeverityList =>
										new( pSeverityList, (int)NumSeverities ) ;

	/// <summary>The number of message IDs to allow or deny.</summary>
	public uint NumIDs ;

	/// <summary>An array of integers that represent the message IDs to allow or deny. The array must have at least <b>NumIDs</b> number of elements.</summary>
	public unsafe int* pIDList ;
	[UnscopedRef] public unsafe Span< int > IDList =>
										new( pIDList, (int)NumIDs ) ;
	
	
	public unsafe InfoQueueFilterDescription( uint numCategories, InfoQueueMessageCategory* categoryBuffer, 
											  uint numSeverities, InfoQueueMessageSeverity* severityBuffer, 
											  uint numIDs, int* idBuffer ) {
		this.NumCategories = numCategories ;
		this.pCategoryList = categoryBuffer ;
		this.NumSeverities = numSeverities ;
		this.pSeverityList = severityBuffer ;
		this.NumIDs = numIDs ;
		this.pIDList = idBuffer ;
	}

	
	//! TODO: Test this:
	// I think this represents a good pattern for dealing with structs that need pinned or native memory ...
	public InfoQueueFilterDescription( InfoQueueMessageCategory[ ] categoryList,
											  InfoQueueMessageSeverity[ ] severityList,
																		int[ ] idList, out MemoryHandle[ ] handles ) {
		uint numCategories = (uint)categoryList.Length ;
		Memory< InfoQueueMessageCategory > categoryMemory = categoryList ;
		var hCategories = categoryMemory.Pin( ) ;
		
		uint numSeverities = (uint)severityList.Length ;
		Memory< InfoQueueMessageSeverity > severityMemory = severityList ;
		var hSeverities    = severityMemory.Pin( ) ;
		
		uint numIDs = (uint)idList.Length ;
		Memory< int > idMemory = idList ;
		var hIDs = idMemory.Pin( ) ;
		
		this.NumIDs = numIDs ;
		this.NumSeverities = numSeverities ;
		this.NumCategories = numCategories ;
		unsafe {
			this.pCategoryList = (InfoQueueMessageCategory *)hCategories.Pointer ;
			this.pSeverityList = (InfoQueueMessageSeverity *)hSeverities.Pointer ;
			this.pIDList       = (int *)hIDs.Pointer ;
		}
		
		handles = new[ ] { hCategories, hSeverities, hIDs } ;
	}

	public override string ToString( ) {
		uint allocSize = (uint)sizeof( InfoQueueMessageCategory ) * NumCategories +
						 (uint)sizeof( InfoQueueMessageSeverity ) * NumSeverities +
						 (uint)sizeof( int ) * NumIDs ;
		string _part1 = $"{nameof(InfoQueueFilterDescription)} (Allocated: {(allocSize / 1024):D}KB) :: " ;
		string _part2 = $"{{ Categories: [Count: {CategoryList.Length:D}], " +
						$"Severities: [Count: {SeverityList.Length:D}], " +
						$"IDs: [Count: {IDList.Length:D}] }}" ;
		return _part1 + _part2 ;
	}
} ;