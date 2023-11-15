#region Using Directives
using System.Collections.ObjectModel ;
using System.Diagnostics.CodeAnalysis ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
#endregion
namespace DXSharp.Direct3D12 ;


[ProxyFor(typeof(ID3D12CommandAllocator))]
public interface ICommandAllocator: IPageable, IInstantiable {
	// ---------------------------------------------------------------------------------
	//! Creation Functions:
	// ---------------------------------------------------------------------------------
	[SuppressMessage( "Interoperability", 
					  "CA1416:Validate platform compatibility" )] 
	internal static readonly ReadOnlyDictionary< Guid, Func<ID3D12CommandAllocator, IInstantiable> > _allocatorCreationFunctions =
		new( new Dictionary<Guid, Func<ID3D12CommandAllocator, IInstantiable> > {
			{ ICommandAllocator.IID, ( pComObj ) => new CommandAllocator( pComObj ) },
		} ) ;
	// ---------------------------------------------------------------------------------

	
	/// <summary>Indicates to re-use the memory that is associated with the command allocator.</summary>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns <b>E_FAIL</b> if there is an actively recording command list referencing the command allocator.  The debug layer will also issue an error in this case. See <a href="https://docs.microsoft.com/windows/desktop/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a> for other possible return values.</para>
	/// </returns>
	/// <remarks>
	/// <para>Apps call <b>Reset</b> to re-use the memory that is associated with a command allocator.  From this call to <b>Reset</b>, the runtime and driver determine that the graphics processing unit (GPU) is no longer executing any command lists that have recorded commands with the command allocator. Unlike <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/nf-d3d12-id3d12graphicscommandlist-reset">ID3D12GraphicsCommandList::Reset</a>, it is not recommended that you call <b>Reset</b>  on the command allocator while a command list is still being executed. The debug layer will issue a warning if it can't prove that there are no pending GPU references to command lists that have recorded commands in the allocator. The debug layer will issue an error if <b>Reset</b> is called concurrently by multiple threads (on the same allocator object).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandallocator-reset#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void Reset( ) ;
	
	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof(ID3D12CommandAllocator) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12CommandAllocator).GUID
																		   .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}

	static IInstantiable IInstantiable.Instantiate( ) => new CommandAllocator( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new CommandAllocator( ptr ) ;
	static IInstantiable IInstantiable.Instantiate<T>( T obj ) => new CommandAllocator( (obj as ID3D12CommandAllocator)! ) ;
	 
	// ==================================================================================
} ;

