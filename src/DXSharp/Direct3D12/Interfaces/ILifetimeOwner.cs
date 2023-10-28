#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[ProxyFor(typeof(ID3D12LifetimeOwner))]
public interface ILifetimeOwner: IUnknownWrapper< ID3D12LifetimeOwner >, 
								 IComObjectRef< ID3D12LifetimeOwner > {
	new ComPtr< ID3D12LifetimeOwner >? ComPointer { get ; }
	ComPtr< ID3D12LifetimeOwner >? IUnknownWrapper< ID3D12LifetimeOwner >.ComPointer => ComPointer ;
	ID3D12LifetimeOwner? IComObjectRef< ID3D12LifetimeOwner >.COMObject => COMObject ;
	
	static Type IUnknownWrapper.ComType => typeof(ID3D12LifetimeOwner) ;
	static Guid IUnknownWrapper.InterfaceGUID => typeof(ILifetimeOwner).GUID ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12LifetimeOwner).GUID
														  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}


	void LifetimeStateUpdated( LifetimeState NewState ) => 
		COMObject!.LifetimeStateUpdated( NewState ) ;
} ;


[Wrapper(typeof(ID3D12LifetimeOwner))]
internal class LifetimeOwner: DisposableObject, ILifetimeOwner {
	protected override ValueTask DisposeUnmanaged( ) {
		ComPointer?.Dispose( ) ;
		return ValueTask.CompletedTask ;
	}
	
	public int RefCount => (int)( ComPointer?.RefCount ?? 0 ) ;
	public ComPtr< ID3D12LifetimeOwner >? ComPointer { get ; }
	public ComPtr? ComPtrBase => ComPointer ;

	internal LifetimeOwner( ) { }
	internal LifetimeOwner( ComPtr< ID3D12LifetimeOwner > ptr ) => ComPointer = ptr ;
	internal LifetimeOwner( nint ptr ) => ComPointer = new( ptr ) ;
	internal LifetimeOwner( ID3D12LifetimeOwner comObj ) => ComPointer = new( comObj ) ;
} ;