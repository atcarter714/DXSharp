#region Using Directives
using System.Collections ;
using System.Diagnostics.CodeAnalysis ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Dxgi.Common ;
using static DXSharp.InteropUtils ;
#endregion
namespace DXSharp.DXGI ;


/// <summary>An "inline" fixed buffer of DXGI.Format values.</summary>
/// <remarks>Has a fixed length of 8x elements (indicated by the <b>const</b> <see cref="DXSharp.DXGI.Format8.ElementCount"/>)</remarks>
[StructLayout(LayoutKind.Explicit, Size = TotalSize ),
 EquivalentOf(typeof(__DXGI_FORMAT_8))]
public struct Format8: IEnumerable< Format > {
	public const int ElementCount = 8,
					 ElementSize  = sizeof(Format),
					 TotalSize    = ElementSize * ElementCount ;
	
	[FieldOffset(ElementSize * 0)] Format _f0 ;
	[FieldOffset(ElementSize * 1)] Format _f1 ;
	[FieldOffset(ElementSize * 2)] Format _f2 ;
	[FieldOffset(ElementSize * 3)] Format _f3 ;
	[FieldOffset(ElementSize * 4)] Format _f4 ;
	[FieldOffset(ElementSize * 5)] Format _f5 ;
	[FieldOffset(ElementSize * 6)] Format _f6 ;
	[FieldOffset(ElementSize * 7)] Format _f7 ;
	
	public int Length => ElementCount ;
	
	public Format this[ int index ] {
		[MethodImpl(_MAXOPT_)] get {
#if DEBUG || DEV_BUILD // Strip bounds checking in release builds
			if( index < 0 || index >= ElementCount ) 
				throw new ArgumentOutOfRangeException( ) ;
#endif
			unsafe { fixed ( Format8* p = &this ) {
					return ( (Format *)p )[ index ] ;
				}
			}
		}
		
		[MethodImpl(_MAXOPT_)] set {
#if DEBUG || DEV_BUILD // Strip bounds checking in release builds
			if( index < 0 || index >= ElementCount ) throw new ArgumentOutOfRangeException( ) ;
#endif
			unsafe { fixed ( Format8* p = &this ) {
					((Format*)p)[ index ] = value ;
				}
			}
		}
	}

	public Format this[ Index index ] {
		[MethodImpl( _MAXOPT_ )] get => this[ index.Value ] ;
		[MethodImpl( _MAXOPT_ )] set => this[ index.Value ] = value ;
	}
	
	
	public Format8( params Format[ ] formats) {
		for( int i = 0 ; i < ElementCount ; ++i ) 
			this[ i ] = formats[ i ] ;
	}
	
	public Format8( Format f0 = default , Format f1 = default , 
					Format f2 = default , Format f3 = default , 
					Format f4 = default , Format f5 = default , 
					Format f6 = default , Format f7 = default ) {
		_f0 = f0 ; _f1 = f1 ; _f2 = f2 ; _f3 = f3 ;
		_f4 = f4 ; _f5 = f5 ; _f6 = f6 ; _f7 = f7 ;
	}
	
	
	/// <summary>Gets this inline array as a span.</summary>
	/// <remarks>⚠ Important ⚠: When this struct is on the stack, do not let the returned span outlive the stack frame that defines it.</remarks>
	[UnscopedRef] public Span< Format > AsSpan( ) {
		unsafe { fixed ( Format8* p = &this ) {
				return new Span< Format >( p, ElementCount ) ;
			}
		}
	}
	
	/// <summary>Gets this inline array as a span.</summary>
	/// <remarks>⚠ Important ⚠: When this struct is on the stack, do not let the returned span outlive the stack frame that defines it.</remarks>
	[UnscopedRef] public readonly ReadOnlySpan<Format> AsReadOnlySpan( ) => 
		MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef( _f0), ElementCount ) ;
	
	public IEnumerator< Format > GetEnumerator( ) {
		yield return _f0 ; yield return _f1 ; yield return _f2 ; yield return _f3 ;
		yield return _f4 ; yield return _f5 ; yield return _f6 ; yield return _f7 ;
	}
	IEnumerator IEnumerable.GetEnumerator( ) => GetEnumerator( ) ;
	
	public override string ToString( ) => $"{{ {string.Join( ", ", this )} }}" ;
	public override int GetHashCode( ) => HashCode.Combine( _f0, _f1, _f2, _f3, _f4, _f5, _f6, _f7 ) ;
	
	public static implicit operator Format8( in __DXGI_FORMAT_8 data) => new( ) {
		_f0 = (Format)data._0, _f1 = (Format)data._1, _f2 = (Format)data._2, _f3 = (Format)data._3,
		_f4 = (Format)data._4, _f5 = (Format)data._5, _f6 = (Format)data._6, _f7 = (Format)data._7,
	} ;
	public static implicit operator __DXGI_FORMAT_8( in Format8 data) => new( ) {
		_0 = (DXGI_FORMAT)data._f0, _1 = (DXGI_FORMAT)data._f1, _2 = (DXGI_FORMAT)data._f2, _3 = (DXGI_FORMAT)data._f3,
		_4 = (DXGI_FORMAT)data._f4, _5 = (DXGI_FORMAT)data._f5, _6 = (DXGI_FORMAT)data._f6, _7 = (DXGI_FORMAT)data._f7,
	} ;
} ;