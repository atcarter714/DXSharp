#region Using Directives

using System.Collections.ObjectModel ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
#endregion
namespace DXSharp.Direct3D12 ;


[ProxyFor( typeof( ID3D12CommandSignature ) )]
public interface ICommandSignature: IPageable, IInstantiable {
	// ---------------------------------------------------------------------------------
	//! Creation Functions:
	// ---------------------------------------------------------------------------------
	internal static readonly ReadOnlyDictionary< Guid, Func<ID3D12CommandSignature, IInstantiable> > _commandSignatureCreationFunctions =
		new( new Dictionary<Guid, Func<ID3D12CommandSignature, IInstantiable> > {
			{ ICommandSignature.IID, ( pComObj ) => new CommandSignature( pComObj ) },
		} ) ;
	// ---------------------------------------------------------------------------------
	
	new static Type ComType => typeof(ID3D12CommandSignature) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12CommandSignature).GUID
														  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	
	static IInstantiable IInstantiable.Instantiate( ) => new CommandSignature( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new CommandSignature( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom obj ) => new CommandSignature( ( obj as ID3D12CommandSignature )! ) ;
	// ==================================================================================
} ;