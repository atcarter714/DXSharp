#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Objects ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[Wrapper(typeof(ID3D12Object))]
internal abstract class Object: DXComObject,
								IObject,
								IComObjectRef< ID3D12Object >,
								IUnknownWrapper< ID3D12Object > {
	ComPtr< ID3D12Object >? _comPointer ;
	public override ComPtr? ComPointer =>
		_comPointer ??= ComResources?.GetPointer< ID3D12Object >( )! ;
	
	public override ID3D12Object? ComObject => (ID3D12Object)ComPointer?.InterfaceObjectRef! ;

	public void SetName( string Name ) {
		using PCWSTR name = Name ;
		ComObject?.SetName( Name ) ;
	}

	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Object).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}

	public new static Type ComType => typeof(ID3D12Object) ;
} ;