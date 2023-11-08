#region Using Directives
using System.Collections ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Dxgi.Common ;
#endregion

namespace DXSharp.DXGI ;

[StructLayout(LayoutKind.Sequential)]
public struct GammaCurve: IEnumerable< RGB > {
	public const int ControlPointCount = 1025,
					 MAX_INDEX = ControlPointCount - 1 ;
	__DXGI_RGB_1025 controlPoints ;
	
	// Indexing:
	public RGB this[ int index ] {
		get {
			_throwIfOutOfRange( index ) ;
			unsafe { return *_getAddressAt( index ) ; }
			
			/*unsafe { fixed ( __DXGI_RGB_1025* addr = &controlPoints ) {
					DXGI_RGB* pRGB = (DXGI_RGB *)addr ;
					return *( pRGB + index ) ;
			}}*/
		}
		set {
			_throwIfOutOfRange( index ) ;
			unsafe { *_getAddressAt( index ) = value ; }
		}
	}
	
	public Span< RGB > this[ Range range ] {
		get {
			unsafe {
				fixed ( void* pStruct = &controlPoints ) {
					int         count = ( range.End.Value - range.Start.Value ) ;
					RGB* pStart = (RGB *)pStruct + range.Start.Value ;
					Span< RGB > s     = new( pStart, count ) ;
					return s[ range ] ;
				}
			}
		}
	}
	
	void _throwIfOutOfRange( int index ) {
		if( index < 0 || index > MAX_INDEX ) throw new
			IndexOutOfRangeException($"{nameof(GammaCurve)} :: " +
									 $"The index ({index}) is out of range." ) ;
	}
	unsafe DXGI_RGB* _getAddressAt( int offset = 0x00 ) {
		unsafe { fixed ( __DXGI_RGB_1025* addr = &controlPoints ) {
			DXGI_RGB* pRGB = (DXGI_RGB *)addr ;
			return ( pRGB + offset ) ;
		}}
	}
	
	// Enumeration:
	public IEnumerator< RGB > GetEnumerator( ) {
		for( int i = 0 ; i < ControlPointCount ; ++i ) 
			yield return this[ i ] ;
	}
	IEnumerator IEnumerable.GetEnumerator( ) => GetEnumerator( ) ;
	
	// Constructors:
	public GammaCurve( __DXGI_RGB_1025 controlPoints ) => this.controlPoints = controlPoints ;
	public GammaCurve( in __DXGI_RGB_1025 controlPoints ) => this.controlPoints = controlPoints ;
	public GammaCurve( in IEnumerable< RGB > collection ) {
		unsafe { fixed ( __DXGI_RGB_1025* addr = &controlPoints ) {
			DXGI_RGB* pRGB = (DXGI_RGB *)addr ; int i = -1 ;
			foreach( var rgb in collection ) {
				if( ++i >= ControlPointCount ) break ;
				*( pRGB + i ) = rgb ;
			}
		}}
	}
	public GammaCurve( params RGB[ ] values ): this( values.AsSpan( ) ) { }
	public GammaCurve( Span< RGB > valuesSpan ) {
		unsafe { fixed ( __DXGI_RGB_1025* addr = &controlPoints ) {
			DXGI_RGB* pRGB = (DXGI_RGB *)addr ; 
			for( int i = 0; i <= MAX_INDEX && i < valuesSpan.Length; ++i ) {
				*(pRGB + i) = valuesSpan[ i ] ;
			}
		}}
	}
	
	// Conversions:
	public static implicit operator GammaCurve( __DXGI_RGB_1025 controlPoints ) => new( controlPoints ) ;
	public static implicit operator __DXGI_RGB_1025( GammaCurve gammaCurve ) => gammaCurve.controlPoints ;
}


[StructLayout(LayoutKind.Sequential)]
public struct GammaControl {
	public RGB Scale, Offset ;
	public GammaCurve Curve ;
	
	// Constructors:
	public GammaControl( in RGB scale, in RGB offset, in GammaCurve curve ) {
		Scale = scale ; Offset = offset ; Curve = curve ;
	}
	public GammaControl( in DXGI_GAMMA_CONTROL gammaControl ) {
		this.Scale  = gammaControl.Scale ; 
		this.Offset = gammaControl.Offset ;
		this.Curve  = gammaControl.GammaCurve ;
	}
	public unsafe GammaControl( DXGI_GAMMA_CONTROL* pGammaControl ) {
		 fixed( GammaControl* pThis = &this ) 
			 *pThis = *(GammaControl *)pGammaControl ;
	}
	
	// Conversions:
	public static implicit operator GammaControl( in DXGI_GAMMA_CONTROL gammaControl ) => new( gammaControl ) ;
	public static implicit operator DXGI_GAMMA_CONTROL( in GammaControl gammaControl ) => new DXGI_GAMMA_CONTROL {
		Offset = gammaControl.Offset, Scale = gammaControl.Scale, GammaCurve = gammaControl.Curve,
	} ;
} ;


[StructLayout(LayoutKind.Sequential)]
public struct GammaControlCapabilities {
	public bool  ScaleAndOffsetSupported ;
	public float MaxConvertedValue, MinConvertedValue ;
	public uint  NumGammaControlPoints ;
	public GammaCurve ControlPoints ;
	
	public GammaControlCapabilities( bool scaleAndOffsetSupported, 
									 float maxConvertedValue, 
									 float minConvertedValue, 
									 uint numGammaControlPoints, 
									 float[ ] controlPoints ) {
		this.ScaleAndOffsetSupported = scaleAndOffsetSupported ;
		this.MaxConvertedValue       = maxConvertedValue ;
		this.MinConvertedValue       = minConvertedValue ;
		this.NumGammaControlPoints   = numGammaControlPoints ;
		
		unsafe { fixed ( float* pControlPoints = controlPoints ) {
				RGB* pRGB = (RGB *)pControlPoints ;
				Span< RGB > s = new( pRGB, (int)numGammaControlPoints ) ;
				this.ControlPoints = new GammaCurve( s ) ; 
		}}
	}

	public GammaControlCapabilities( in DXGI_GAMMA_CONTROL_CAPABILITIES caps ) {
		unsafe { fixed ( __float_1025* pControlPoints = &caps.ControlPointPositions ) {
				this.ControlPoints = *((GammaCurve*)pControlPoints) ; }
			this.ScaleAndOffsetSupported = caps.ScaleAndOffsetSupported ;
			this.MaxConvertedValue       = caps.MaxConvertedValue ;
			this.MinConvertedValue       = caps.MinConvertedValue ;
			this.NumGammaControlPoints   = caps.NumGammaControlPoints ;
		}
	}
	
	public GammaControlCapabilities( bool  scaleAndOffsetSupported,
									 float maxConvertedValue,
									 float minConvertedValue,
									 uint  numGammaControlPoints,
									 GammaCurve controlPoints ) {
		this.ScaleAndOffsetSupported = scaleAndOffsetSupported ;
		this.MaxConvertedValue       = maxConvertedValue ;
		this.MinConvertedValue       = minConvertedValue ;
		this.NumGammaControlPoints   = numGammaControlPoints ;
		this.ControlPoints           = controlPoints ;
	}

} ;