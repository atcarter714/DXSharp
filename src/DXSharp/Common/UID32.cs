#region Using Directives
using System ;
using System.Numerics ;
#endregion
namespace DXSharp ;

[Serializable] public struct UID32: IEquatable< UID32 > {
	#region Constants & Static Data
	public static UID32 New => new( _last++ ) ;
	public static readonly UID32 Null = new UID32( 0x00000000U ) ;
	public static readonly UID32 Max = new UID32( uint.MaxValue ) ;
	static uint  _last = 0U ;
	#endregion
	
	
	public uint Value ;
	public bool IsNull => (Value is 0x00U) ;
	

	#region Constructors
    
	public UID32( uint value = 0x00 ) =>
		this.Value = value ;

	public UID32( int value = 0x00 ) =>
		this.Value = (uint)value ;

	public UID32( ushort value = 0x00 ) =>
		this.Value = (uint)value ;

	public UID32( short value = 0x00 ) =>
		this.Value = (uint)value ;

	public UID32( Guid guid ) => 
		CreateUIDFrom( guid, out this ) ;

	public UID32( string str ) =>
		this.Value = (uint)str.GetHashCode( ) ;

	#endregion
	
	#region IEquatable< UID > Implementation
		
	public override int GetHashCode( ) => (int)Value ;
	public override string ToString( ) => Value.ToString( "X" ) ;

	public override bool Equals( object? obj ) =>
		obj is UID32 other && Equals( other ) ;

	public bool Equals( UID32 other ) => this.Value == other.Value ;
	
	#endregion
	
	#region Operator Overloads
	public static bool operator ==( UID32  a, UID32  b ) => a.Value == b.Value ;
	public static bool operator !=( UID32  a, UID32  b ) => a.Value != b.Value ;
	public static bool operator ==( UID32  a, uint b ) => a.Value == b ;
	public static bool operator !=( UID32  a, uint b ) => a.Value != b ;
	public static bool operator ==( uint a, UID32  b ) => a == b.Value ;
	public static bool operator !=( uint a, UID32  b ) => a != b.Value ;

	public static implicit operator ulong( UID32  uid ) => uid.Value ;
	public static implicit operator long( UID32   uid ) => uid.Value ;
	public static implicit operator uint( UID32   uid ) => uid.Value ;
	public static explicit operator int( UID32    uid ) => (int)uid.Value ;
	public static explicit operator ushort( UID32 uid ) => (ushort)uid.Value ;
	public static explicit operator short( UID32  uid ) => (short)uid.Value ;
	public static explicit operator byte( UID32   uid ) => (byte)uid.Value ;
	public static explicit operator sbyte( UID32  uid ) => (sbyte)uid.Value ;

	public static implicit operator UID32( uint   id ) => new( id ) ;
	public static implicit operator UID32( int    id ) => new( (uint)id ) ;
	public static implicit operator UID32( ushort id ) => new( id ) ;
	public static implicit operator UID32( short  id ) => new( id ) ;
	public static implicit operator UID32( byte   id ) => new( id ) ;
	public static implicit operator UID32( sbyte  id ) => new( id ) ;

	public static int operator +( UID32  uid ) => (int)uid.Value ;
	public static int operator -( UID32  uid ) => -(int)uid.Value ;
	public static UID32 operator ++( UID32 uid ) => new( uid.Value + 1 ) ;
	public static UID32 operator --( UID32 uid ) => new( uid.Value - 1 ) ;

	public static UID32 operator +( UID32 uid, uint i ) => new( uid.Value + i ) ;
	public static UID32 operator -( UID32 uid, uint i ) => new( uid.Value - i ) ;
	public static UID32 operator *( UID32 uid, uint i ) => new( uid.Value * i ) ;
	public static UID32 operator /( UID32 uid, uint i ) => new( uid.Value / i ) ;

	public static UID32 operator |( UID32   a,   UID32  b )      => new( a.Value | b.Value ) ;
	public static UID32 operator |( UID32   uid, uint value )  => new( uid.Value | value ) ;
	public static UID32 operator ^( UID32   a,   UID32  b )      => new( a.Value ^ b.Value ) ;
	public static UID32 operator ^( UID32   uid, uint value )  => new( uid.Value ^ value ) ;
	public static UID32 operator >> ( UID32 uid, int  offset ) => new( uid.Value >> offset ) ;
	public static UID32 operator <<( UID32  uid, int  offset ) => new( uid.Value << offset ) ;
	#endregion
		
	#region Static Helper API
	public static UID32 CreateUIDFrom( in Guid guid ) {
		CreateUIDFrom( guid, out UID32 id ) ;
		return id ;
	}
	public static void CreateUIDFrom( in Guid guid, out UID32 id ) {
		unsafe { fixed( Guid* p = &guid ) {
			long* q = (long*)p + 1 ;
			id.Value = *(uint *)q ;
		}}
	}
	public static UID32 CreateInstanceID( )                   => new( Guid.NewGuid() ) ;
	public static UID32 CreateFromTypeGUID< T >( )            => new( typeof(T).GUID ) ;
	public static UID32 GetTypeHashUID< T >( )                => typeof(T).GetHashCode( ) ;
	public static UID32 CreateInstanceIDFrom< T >( in T obj ) => ( obj?.GetHashCode() ?? Null ).Value ;
	#endregion
} ;
