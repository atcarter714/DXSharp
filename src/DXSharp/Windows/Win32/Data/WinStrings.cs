#define USE_STRING_MEM_POOL

#region Using Directives 
#pragma warning disable CS1591,CS1573,CS0465,CS0649,CS8019,CS1570,CS1584,CS1658,CS0436,CS8981
using System.Buffers ;
using System.Text ;
using global::System;
using global::System.Diagnostics;
using global::System.Diagnostics.CodeAnalysis;
using global::System.Runtime.CompilerServices;
using global::System.Runtime.InteropServices;
using global::System.Runtime.Versioning;
using winmdroot = global::Windows.Win32;
#endregion
namespace Windows.Win32.Foundation ;


/// <summary>A pointer to a null-terminated, constant character string.</summary>
[DebuggerDisplay( "{" + nameof( DebuggerDisplay ) + "}" )]
public readonly unsafe partial struct PCWSTR: IEquatable< PCWSTR >, 
											  IDisposable {
	public PCWSTR( string? value ) {
#if DEBUG || DEV_BUILD
		if ( string.IsNullOrEmpty(value) ) {
			Value = null ;
			return ;
		}
#endif
		int len = value?.Length ?? 0 ;
#if USE_STRING_MEM_POOL
		var mem = Pool.Rent( len + 1 ) ;
		mem.Memory.Span[ len ] = '\0' ;
		
		var strSpan = value.AsSpan( ) ;
		strSpan.CopyTo( mem.Memory.Span ) ;
		
		Value = (char *)mem.Memory.Pin( ).Pointer ;
		_PCWSTRAllocations.Add( (nint)Value, mem ) ;
#else
		this.Value = (char *)Marshal.StringToHGlobalUni( value ) ;
		_PCWSTRAllocations.Add( (nint)Value ) ;
#endif
		
		if ( !_PCWSTR_Sizes.ContainsKey( this ) )
			_PCWSTR_Sizes.Add( this, len ) ;
	}
	
	public void Dispose( ) {
#if USE_STRING_MEM_POOL
		if( Value is not null ) {
			if( !_PCWSTRAllocations.ContainsKey( (nint)Value ) ) return ;
			var mem = _PCWSTRAllocations[ (nint)Value ] ;
			mem.Dispose( ) ;
			_PCWSTRAllocations.Remove( (nint)Value ) ;
		}
#else
 		if( Value is not null ) {
			if( !_PCWSTRAllocations.Contains((nint)Value) ) return ;
			Marshal.FreeHGlobal( (nint)Value ) ;
			_PCWSTRAllocations.Remove( (nint)Value ) ;
		}
#endif
	}
	
	
	public static PCWSTR Create( in string? value ) => new( value ) ;
	
	public static bool operator ==( in PCWSTR left, in PCWSTR right ) => left.Equals( right ) ;
	public static bool operator !=( in PCWSTR left, in PCWSTR right ) => !left.Equals( right ) ;
	
	public static bool operator !=( in PCWSTR left, in string? right ) => !( left == right ) ;
	public static bool operator ==( in PCWSTR left, in string? right ) {
		if( right is null ) return left.Value is null ;
		if( left.Value is null ) return false ;
		
		if( _PCWSTR_Sizes.TryGetValue( left, out int len ) ) {
			if( len != right.Length ) return false ;
			fixed( char* ptr = right ) {
				 return CompareChars( left.Value, ptr, len ) ;
			}
		}
		
		len = left.Length ;
		_PCWSTR_Sizes.Add( left, len ) ;
		if ( len != right.Length ) return false ;
		fixed( char* ptr = right )
			 return CompareChars( left.Value, ptr, len ) ;
	}
	
	public static implicit operator PCWSTR( in string? value ) => new( value ) ;
	public static implicit operator string?( in PCWSTR value ) => value.ToString( ) ;

	static readonly Dictionary< PCWSTR, int > _PCWSTR_Sizes = new( ) ;

	public static bool CompareChars( char* left, char* right, int len ) {
		const int MAX_LEN = 2048 ;
		for( int i = 0 ; i < len && i < MAX_LEN ; ++i ) {
			if ( left[ i ] != right[ i ] ) return false ;
			if( left[ i ] is '\0' ) return true ;
		}
		return true ;
	}

	
#if USE_STRING_MEM_POOL
	static readonly MemoryPool< char > Pool = MemoryPool< char >.Shared ;
	static readonly Dictionary< nint, IMemoryOwner<char> > _PCWSTRAllocations = new( ) ;
	public static bool CleanupStringAllocations( ) {
		Pool?.Dispose( ) ;
		_PCWSTR_Sizes?.Clear( ) ;
		_PCWSTRAllocations?.Clear( ) ;
		return true ;
	}
#else
	static readonly HashSet< nint > _PCWSTRAllocations = new( ) ;
	public static bool CleanupStringAllocations( ) {
		if( _PCWSTRAllocations.Count is 0 ) return false ;
		try { foreach( var ptr in _PCWSTRAllocations )
			if( ptr is not 0x0000 )
				Marshal.FreeHGlobal( ptr ) ;
		}
		finally { 
			_PCWSTR_Sizes?.Clear( ) ;
			_PCWSTRAllocations?.Clear( ) ;
		}
		return true ;
	}
#endif
} ;



/// <summary>A pointer to a null-terminated, constant, ANSI character string.</summary>
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
public readonly unsafe partial struct PCSTR: IDisposable {
	public PCSTR( string? value ) {
#if DEBUG || DEV_BUILD
		if ( string.IsNullOrEmpty(value) ) {
			Value = null ;
			return ;
		}
#endif
		int len = value?.Length ?? 0 ;
#if USE_STRING_MEM_POOL
		var mem = Pool.Rent( len + 1 ) ;
		
		mem.Memory.Span[ len ] = 0x00 ;
		int c = Encoding.ASCII.GetBytes( value, mem.Memory.Span ) ;
		
		Value = (byte *)mem.Memory.Pin( ).Pointer ;
		_PCSTRAllocations.Add( (nint)Value, mem ) ;
#else
		this.Value = (char *)Marshal.StringToHGlobalUni( value ) ;
		_PCSTRAllocations.Add( (nint)Value ) ;
#endif
		
		if ( !_PCSTR_Sizes.ContainsKey( this ) )
			_PCSTR_Sizes.Add( this, c ) ;
	}
	
	public void Dispose( ) {
#if USE_STRING_MEM_POOL
		if( Value is not null ) {
			if( !_PCSTRAllocations.ContainsKey( (nint)Value ) ) return ;
			var mem = _PCSTRAllocations[ (nint)Value ] ;
			mem.Dispose( ) ;
			_PCSTRAllocations.Remove( (nint)Value ) ;
			_PCSTR_Sizes.Remove( this ) ;
		}
#else
 		if( Value is not null ) {
			if( !_PCSTRAllocations.Contains((nint)Value) ) return ;
			Marshal.FreeHGlobal( (nint)Value ) ;
			_PCSTRAllocations.Remove( (nint)Value ) ;
		}
#endif
	}

	
	public static PCSTR Create( in string? value ) => new( value ) ;
	
	public static bool operator ==( in PCSTR left, in PCSTR right ) => left.Equals( right ) ;
	public static bool operator !=( in PCSTR left, in PCSTR right ) => !left.Equals( right ) ;
	
	public static bool operator !=( in PCSTR left, in string? right ) => !( left == right ) ;
	
	public static bool operator ==( in PCSTR left, in string? right ) {
		if( right is null ) return left.Value is null ;
		if( left.Value is null ) return false ;
		
		if( _PCSTR_Sizes.TryGetValue( left, out int len ) ) {
			if( len != right.Length ) return false ;
			fixed( byte* ptr = Encoding.ASCII.GetBytes(right) )
				return CompareChars( left.Value, ptr, len ) ;
		}
		
		len = left.Length ;
		_PCSTR_Sizes.Add( left, len ) ;
		if ( len != right.Length ) return false ;
		fixed( byte* ptr = Encoding.ASCII.GetBytes(right) )
			return CompareChars( left.Value, ptr, len ) ;
	}
	
	/*public static bool operator ==( in PCSTR left, string? right ) {
		if( left.Value is null ) return right is null ;
		if( right is null ) return false ;
		
		if( _PCSTR_Sizes.TryGetValue( left, out int len ) ) {
			if( len != right.Length ) return false ;
			fixed( byte* ptr = Encoding.ASCII.GetBytes(right) )
				return CompareChars( left.Value, ptr, len ) ;
		}
		else {
			len = left.Length ;
			_PCSTR_Sizes.Add( left, len ) ;
			
			if ( len != right.Length ) return false ;
			fixed( byte* ptr = Encoding.ASCII.GetBytes(right) )
				return CompareChars( left.Value, ptr, len ) ;
		}
	}*/
	
	
	public static implicit operator PCSTR( in string? value ) => new( value ) ;
	public static implicit operator string?( in PCSTR value ) => value.ToString( ) ;

	static readonly Dictionary< PCSTR, int > _PCSTR_Sizes = new( ) ;

	public static bool CompareChars( byte* left, byte* right, int len ) {
		const int MAX_LEN = 2048 ;
		for( int i = 0 ; i < len && i < MAX_LEN ; ++i ) {
			if ( left[ i ] != right[ i ] ) return false ;
			if( left[ i ] is 0x00 ) return true ;
		}
		return true ;
	}
	
#if USE_STRING_MEM_POOL
	static readonly MemoryPool< byte > Pool = MemoryPool< byte >.Shared ;
	static readonly Dictionary< nint, IMemoryOwner<byte> > _PCSTRAllocations = new( ) ;
	public static bool CleanupStringAllocations( ) {
		Pool?.Dispose( ) ;
		_PCSTR_Sizes?.Clear( ) ;
		_PCSTRAllocations?.Clear( ) ;
		return true ;
	}
#else
	static readonly HashSet< nint > _PCSTRAllocations = new( ) ;
	public static bool CleanupStringAllocations( ) {
		if( _PCSTRAllocations.Count is 0 ) return false ;
		try { foreach( var ptr in _PCSTRAllocations )
			if( ptr is not 0x0000 )
				Marshal.FreeHGlobal( ptr ) ;
		}
		finally { 
			_PCSTR_Sizes?.Clear( ) ;
			_PCSTRAllocations?.Clear( ) ;
		}
		return true ;
	}
#endif
} ;