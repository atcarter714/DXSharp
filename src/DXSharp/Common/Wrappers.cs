#region Using Directives
using System ;
using System.Runtime.InteropServices ;
using Windows.Win32;
using Windows.Foundation;
using DXSharp.Windows.COM ;

#endregion
namespace DXSharp ;

/*public interface IDirectXObject: IDisposable {
	public int RefCount { get ; }
	public nint BasePointer { get ; }
	public ComPtr< IUnknown >? ComPointer { get ; }
	
	protected void Dispose( bool disposing ) ;
	
	public uint AddRef( ) => ComPointer?.Interface?.AddRef( ) 
							 ?? throw new NullReferenceException( ) ;
	public uint Release( ) => ComPointer?.Interface?.Release( ) 
							 ?? throw new NullReferenceException( ) ;
	
	protected int _GetRefCount( ) {
		uint r = ComPointer?.Interface?.AddRef( ) ?? 0U ;
		if( r > 0U ) ComPointer?.Interface?.Release( ) ;
		return (int)r ;
	}

	
	public static abstract IDirectXObject? ConstructInstance< T >( nint ptr )
		where T: class, IDirectXObject ;
}*/