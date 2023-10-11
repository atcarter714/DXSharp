#region Using Directives
using System ;
#endregion
namespace DXSharp ;


/// <summary>Represents a size using unsigned (32-bit) integers.</summary>
/// <remarks>Convenient in some interop scenarios with Win32/COM and D3D.</remarks>
public struct USize {
	public uint Width, Height ;
	public USize( (uint width, uint height) size ) => ( Width, Height ) = size ;
	public USize( Size size ) => ( Width, Height ) = ( (uint)size.Width, (uint)size.Height ) ;
	public USize( uint width = 0x0000U, uint height = 0x0000U ) => ( Width, Height ) = ( width, height ) ;
	public static implicit operator USize( (uint width, uint height) size ) => new( size.width, size.height ) ;
	public static implicit operator (uint width, uint height)( USize size ) => ( size.Width, size.Height ) ;
	public static implicit operator USize( Size size ) => new( (uint)size.Width, (uint)size.Height ) ;
	public static implicit operator Size( USize size ) => new( (int)size.Width, (int)size.Height ) ;
} ;

