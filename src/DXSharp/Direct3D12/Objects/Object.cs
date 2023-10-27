using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Objects ;
using DXSharp.Windows.COM ;
namespace DXSharp.Direct3D12 ;


public abstract class Object: DXComObject, 
							  IObject {
	public override ComPtr? ComPtrBase => ComPointer ;
	public ID3D12Object? COMObject => ComPointer?.Interface ;
	public nint PointerToIUnknown => ComPointer?.BaseAddress ?? 0x0000 ;
	public virtual ComPtr< ID3D12Object >? ComPointer { get ; protected set ; }


	public static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Object).GUID
														  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
} ;