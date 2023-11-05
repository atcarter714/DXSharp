#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Objects ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI ;


/// <summary>Wrapper interface for the native IDXGIObject COM interface</summary>
[Wrapper(typeof(IDXGIObject))]
internal abstract class Object: DXComObject, 
								IObject,
								IComObjectRef< IDXGIObject >,
								IUnknownWrapper< IDXGIObject > {
	//! ---------------------------------------------------------------------------------
	
	ComPtr< IDXGIObject >? _comPtr ;
	public override ComPtr? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< IDXGIObject >( )! ;
	
	public virtual IDXGIObject? COMObject => (IDXGIObject)ComPointer?.InterfaceObjectRef! ;
	
	//! ---------------------------------------------------------------------------------
	
	public uint AddReference( ) => _comPtr?.Interface?.AddRef( ) ?? 0U ;
	public uint ReleaseReference( ) => _comPtr?.Interface?.Release( ) ?? 0U ;
	
	public new static Type ComType => typeof( IDXGIObject ) ;
	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIObject).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	
	public void GetParent< T >( out T ppParent ) where T: IUnknownWrapper< IDXGIObject >, IInstantiable {
		unsafe {
			var riid = T.Guid ;
			COMObject!.GetParent( &riid, out var _parent ) ;
			ppParent = (T) T.Instantiate( _parent ) ;
		}
	}
	// ==================================================================================
} ;