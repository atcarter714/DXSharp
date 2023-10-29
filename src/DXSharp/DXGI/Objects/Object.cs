﻿#region Using Directives
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Objects ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI ;


/// <summary>Wrapper interface for the native IDXGIObject COM interface</summary>
public abstract class Object: DXComObject, IObject {
	//public static Guid InterfaceGUID => typeof( IDXGIObject ).GUID ;
	public virtual ComPtr< IDXGIObject >? ComPointer { get ; protected set ; }
	public nint PointerToIUnknown => ComPointer?.BaseAddress ?? 0x0000 ;
	public IDXGIObject? COMObject => ComPointer?.Interface ;
	public override ComPtr? ComPtrBase => ComPointer ;
	
	
	public uint AddReference( ) => this.ComPointer?.Interface?.AddRef( ) ?? 0U ;
	public uint ReleaseReference( ) => this.ComPointer?.Interface?.Release( ) ?? 0U ;
	
	
	/*protected virtual void _throwIfDestroyed( ) {
		if ( ComPointer is null || ComPointer.Disposed || ComPointer.Interface is null )
			throw new
				ObjectDisposedException( nameof( Object ), $"{nameof( Object )} :: " + 
							$"Internal object \"{nameof( ComPointer )}\" is destroyed/null." ) ;
	}*/
	
	
	/*public static IObject ConstructInstance< TObject, TInterface >( TInterface pComObj )
		where TObject: class, IObject, IUnknownWrapper< TInterface > where TInterface: IDXGIObject {
		return TObject.ConstructInstance< TObject, TInterface >( pComObj ) ;
	}
	
	internal static T2 ConvertWrapper< T1, I1, T2, I2 >( T1 wrapper ) 
		where T1: class, IUnknownWrapper< I1 >
		where I1: class, IDXGIObject
		where T2: class, IObject, IUnknownWrapper< I2 >
		where I2: class, IDXGIObject {
		ArgumentNullException.ThrowIfNull( wrapper, nameof(wrapper) ) ;
		return
			(T2)T2.ConstructInstance< T2, I2 >
				( (I2)( (IDXGIObject)( wrapper.ComPointer?.Interface )! ) ) ;
	}*/

	
} ;



// ==================================================================================
//! --------- trash pile ------------------------------------------------------------

	
/*public void GetParent< T >( out T ppParent ) where T: IUnknownWrapper {
	_throwIfDestroyed( ) ;

	ppParent = default! ;
	unsafe {
		var riid = typeof( T ).GUID ;
		_interface?.GetParent( &riid, out IUnknown ppObj ) ;
	}
	ppParent = (T)ppParent ;
}*/
	
/*public new static TInterface Instantiate< TInterface >( )
	where TInterface: class, IDXCOMObject => TInterface.Instantiate< TInterface >( ) ;*/
	
/*public override void GetPrivateData< TData >( out uint pDataSize, nint pData ) where TData: unmanaged {
	_throwIfDestroyed( ) ;

	uint dataSize = 0U ;
	Guid name     = typeof( IDXGIObject ).GUID ;
	unsafe {
		_interface!.GetPrivateData( &name, ref dataSize, (void *)pData ) ;
	}
	pDataSize = dataSize ;
}
public override void SetPrivateData< T >( uint DataSize, nint pData ) {
	_throwIfDestroyed( ) ;

	uint dataSize = 0U ;
	Guid name     = typeof(IDXGIObject).GUID ;
	unsafe {
		_interface!.SetPrivateData( &name, dataSize, (void *)pData ) ;
	}
}
public override void SetPrivateDataInterface< T >( in T pUnknown ) where T: IUnknownWrapper< IUnknown > {
#if DEBUG || DEV_BUILD
	ArgumentNullException.ThrowIfNull( pUnknown, nameof(pUnknown) ) ;
	ArgumentNullException.ThrowIfNull( pUnknown.ComObject, nameof(pUnknown.ComObject) ) ;
#endif

	_throwIfDestroyed( ) ;
	Guid name = typeof(IDXGIObject).GUID ;
	unsafe {
		_interface!.SetPrivateDataInterface( &name, pUnknown.ComObject! ) ;
	}
}*/