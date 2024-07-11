#region Using Directives

using System.Collections.ObjectModel ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


[ProxyFor( typeof(ID3D12RootSignature) )]
public interface IRootSignature: IDeviceChild, IInstantiable {
	// ---------------------------------------------------------------------------------
	//! Creation Functions:
	// ---------------------------------------------------------------------------------
	internal static readonly ReadOnlyDictionary< Guid, Func<ID3D12RootSignature, IInstantiable> > _rootSignatureCreationFunctions =
		new( new Dictionary<Guid, Func<ID3D12RootSignature, IInstantiable> > {
			{ IRootSignature.IID, ( pComObj ) => new RootSignature( pComObj ) },
		} ) ;

	// ---------------------------------------------------------------------------------

	
	// ---------------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12RootSignature) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12RootSignature).GUID
														  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new RootSignature( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new RootSignature( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom obj ) => new RootSignature( ( obj as ID3D12RootSignature )! ) ;
	// ==================================================================================
} ;