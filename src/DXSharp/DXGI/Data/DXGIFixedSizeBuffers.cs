#region Using Directives
using System.Collections ;
using System.Diagnostics.CodeAnalysis ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using static DXSharp.InteropUtils ;
#endregion
namespace DXSharp.DXGI ;


/// <summary>An "inline" fixed buffer of DXGI.Format values.</summary>
/// <remarks>Has a fixed length of 8x elements (indicated by the <b>const</b> <see cref="DXSharp.DXGI.Format8.ElementCount"/>)</remarks>
[StructLayout(LayoutKind.Explicit, Size = (sizeof(Format) * 8) )]
public struct Format8: IEnumerable< Format > {
	public const int ElementCount = 8,
					 FormatSize   = sizeof(Format),
					 TotalSize    = FormatSize * ElementCount ;
	[FieldOffset(FormatSize * 0)] Format _f0 ;
	[FieldOffset(FormatSize * 1)] Format _f1 ;
	[FieldOffset(FormatSize * 2)] Format _f2 ;
	[FieldOffset(FormatSize * 3)] Format _f3 ;
	[FieldOffset(FormatSize * 4)] Format _f4 ;
	[FieldOffset(FormatSize * 5)] Format _f5 ;
	[FieldOffset(FormatSize * 6)] Format _f6 ;
	[FieldOffset(FormatSize * 7)] Format _f7 ;
	
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
} ;