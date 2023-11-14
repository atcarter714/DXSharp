using System.Numerics ;
namespace DXSharp.Math ;


/// <summary>
/// Defines a contract for a range of <typeparamref name="T"/> values from a
/// minimum (<see cref="Min"/>) to a maximum (<see cref="Max"/>).
/// </summary>
/// <typeparam name="T">The type of value.</typeparam>
public interface IValueRange< out T > where T: unmanaged, INumber< T >,
					IComparable<T>, IEquatable<T>, IMinMaxValue<T> {
	/// <summary>The minimum value of the range.</summary>
	T Min { get ; }
	/// <summary>The maximum value of the range.</summary>
	T Max { get ; }
} ;



/// <summary>
/// Represents a range of values from a minimum
/// (<see cref="Min"/>) to a maximum (<see cref="Max"/>)
/// value of <typeparam name="T"></typeparam>.
/// </summary>
public struct ValueRange< T >: IValueRange< T >, IEquatable< ValueRange< T > > 
			                          where T: unmanaged, INumber< T >,
								IComparable<T>, IEquatable<T>, IMinMaxValue<T> {
	// ----------------------------------------------------------------------------------------------------
	internal T _min, _max ;
	
	public T Min { get => _min ; set => _min = value ; }
	public T Max { get => _max ; set => _max = value ; }
	
	
	/// <summary>Gets the <typeparamref name="T"/> value at the specified offset in the structure.</summary>
	/// <remarks>Only <c><value>0</value></c> and <c><value>1</value></c> are valid indices.</remarks>
	public T this[ int index ] {
		get => index switch {
			0 => _min,
			1 => _max,
			_ => throw new IndexOutOfRangeException( ) } ;
		set {
			switch( index ) {
				case 0: _min = value ; break ;
				case 1: _max = value ; break ;
				default: throw new IndexOutOfRangeException( ) ;
			}
		}
	}
	
	/// <summary>
	/// Gets the "length" or total number of possible (whole integer) values in the range.
	/// </summary>
	public T Length => ( _max - _min ) ;
	
	/// <summary>Gets the median (i.e., middle or "center") value of the range.</summary>
	public T Median => ( _min + _max ) / (T.One + T.One) ;
	
	
	// ----------------------------------------------------------------------------------------------------
	
	public ValueRange( in T min = default, in T max = default ) {
#if !STRIP_CHECKS || DEBUG
		if ( min > max )
			throw new ArgumentOutOfRangeException( $"{nameof(ValueRange<T> )} :: " +
												   $"The {nameof(min)} value ({min}) cannot be greater than " +
												   $"the {nameof(max)} value ({max})." ) ;
#endif
		
		_min = min ; _max = max ;
	}
	
	
	// ----------------------------------------------------------------------------------------------------
	//! Range Checking: ----------------------------------------------------
	public bool InRange( T value ) => value >= _min && value <= _max ;
	public bool InRange( in T value ) => value >= _min && value <= _max ;

	public bool InRange( ValueRange< T > range ) => InRange( in range ) ;
	public bool InRange( in ValueRange< T > range ) => 
		range._min >= _min && range._max <= _max ;

	public bool InRange( IValueRange< T > range ) => InRange( in range ) ;
	public bool InRange( in IValueRange< T > range ) =>
	 		range.Min >= _min && range.Max <= _max ;
	
	
	//! Clamping Values: ---------------------------------------------------
	public T Clamp( T value ) => Clamp( in value ) ;
	public T Clamp( in T value ) => value < _min 
										  ? _min : value > _max ?
														   _max : value ;
	
	
	//! Remapping Values: --------------------------------------------------
	public T Remap( T value, ValueRange< T > range ) => 
								Remap( in value, in range ) ;

	public T Remap( in T value, in ValueRange< T > range ) {
		T v = value - _min ;
		T r = _max - _min ;
		T t = range._max - range._min ;
		return range._min + ( v / r ) * t ;
	}
	
	
	public T Remap( T value, ValueRange< T > originalRange,
					 							ValueRange< T > newRange ) =>
													Remap( in value, in originalRange, in newRange ) ;
	
	public T Remap( in T value,
						in ValueRange< T > originalRange, 
							in ValueRange< T > newRange ) {
		T v = value - originalRange._min ;
		T r = originalRange._max - originalRange._min ;
		T t = newRange._max - newRange._min ;
		return newRange._min + ( v / r ) * t ;
	}
	
	
	// ----------------------------------------------------------------------------------------------------
	public bool Equals( ValueRange< T > other )  => _min.Equals( other._min ) && _max.Equals( other._max ) ;
	public override bool Equals( object? obj ) => obj is ValueRange< T > other && Equals( other ) ;
	public override int  GetHashCode( )      => HashCode.Combine( _min, _max ) ;
	
	// ----------------------------------------------------------------------------------------------------
	#region Conversion Ops
	public static implicit operator ValueRange< T >( in (T min, T max) range ) => new( range.min, range.max ) ;
	public static implicit operator (T min, T max)( in ValueRange< T > range ) => ( range._min, range._max ) ;
	#endregion
	#region Comparison Ops
	public static bool operator ==( in ValueRange< T > left, in ValueRange< T > right ) => 
																left._min == right._min && 
																	left._max == right._max ;
	
	public static bool operator !=( in ValueRange< T > left, in ValueRange< T > right ) => 
																left._min != right._min || 
																	left._max != right._max ;
	#endregion
	#region Arithmetic Ops
	public static ValueRange< T > operator +( in ValueRange< T > left, in ValueRange< T > right ) => 
																new( left._min + right._min, 
																	 left._max + right._max ) ;
	public static ValueRange< T > operator -( in ValueRange< T > left, in ValueRange< T > right ) =>
	 																new( left._min - right._min, 
																	 left._max - right._max ) ;
	
	public static ValueRange< T > operator *( in ValueRange< T > left, in ValueRange< T > right ) =>
	 																new( left._min * right._min, 
																	 left._max * right._max ) ;
	public static ValueRange< T > operator /( in ValueRange< T > left, in ValueRange< T > right ) =>
	 																new( left._min / right._min, 
																	 left._max / right._max ) ;
	
	public static ValueRange< T > operator +( in ValueRange< T > left, in T right ) =>
	 																new( left._min + right, 
																	 left._max + right ) ;
	public static ValueRange< T > operator -( in ValueRange< T > left, in T right ) =>
	 																new( left._min - right, 
																	 left._max - right ) ;
	
	public static ValueRange< T > operator *( in ValueRange< T > left, in T right ) =>
	 																new( left._min * right, 
																	 left._max * right ) ;
	
	public static ValueRange< T > operator /( in ValueRange< T > left, in T right ) =>
	 																new( left._min / right, 
																	 left._max / right ) ;
	
	public static ValueRange< T > operator +( in T left, in ValueRange< T > right ) =>
	 																new( left + right._min, 
																	 left + right._max ) ;
	
	public static ValueRange< T > operator -( in T left, in ValueRange< T > right ) =>
	 																new( left - right._min, 
																	 left - right._max ) ;
	
	public static ValueRange< T > operator *( in T left, in ValueRange< T > right ) =>
	 																new( left * right._min, 
																	 left * right._max ) ;
	
	public static ValueRange< T > operator /( in T left, in ValueRange< T > right ) =>
	 																new( left / right._min, 
																	 left / right._max ) ;
	#endregion
	// ====================================================================================================
} ;