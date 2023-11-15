#region Using Directives

using System.Collections.ObjectModel ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
#endregion
namespace DXSharp.Direct3D12 ;


[ProxyFor( typeof(ID3D12Heap) )]
public interface IHeap: IPageable, IInstantiable {
	// ---------------------------------------------------------------------------------
	//! Creation Functions:
	// ---------------------------------------------------------------------------------
	internal static readonly ReadOnlyDictionary< Guid, Func<ID3D12Heap, IInstantiable> > _heapCreationFunctions =
		new( new Dictionary<Guid, Func<ID3D12Heap, IInstantiable> > {
			{ IHeap.IID, ( pComObj ) => new Heap( pComObj ) },
			{ IHeap1.IID, ( pComObj ) => new Heap1( (pComObj as ID3D12Heap1)! ) },
		} ) ;

	// ---------------------------------------------------------------------------------

	
	
	// ---------------------------------------------------------------------------------
	
	/// <summary>Gets the properties of a heap.</summary>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues">HRESULT</a></b></para>
	/// <para>Returns one of the following values:</para>
	/// <list type="bullet">
	/// <item>
	/// <term>S_OK</term>
	/// </item>
	/// <item>
	/// <term>E_OUTOFMEMORY</term>
	/// </item>
	/// </list>
	/// </returns>
	/// <remarks>
	/// <para>For more info about the heap properties, see <a href="https://learn.microsoft.com/en-us/windows/win32/api/d3d12/nn-d3d12-id3d12heap">
	/// ID3D12Heap interface</a> documentation (defined in: "d3d12.h").</para>
	/// </remarks>
	/// <seealso href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12heap-getdesc">ID3D12Heap::GetDesc method</seealso>
	/// <seealso href="https://docs.microsoft.com/windows/win32/api/d3d12/ns-d3d12-d3d12_heap_desc">D3D12_HEAP_DESC structure</seealso>
	HeapDescription GetDesc( ) ;
	
	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof(ID3D12Heap) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Heap).GUID
																	 .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new Heap( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new Heap( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom obj ) => new Heap( ( obj as ID3D12Heap )! ) ;
	// ==================================================================================
} ;


[Wrapper( typeof( ID3D12Heap1 ) )]
public interface IHeap1: IHeap {
	// ---------------------------------------------------------------------------------
	
	void GetProtectedResourceSession( in Guid riid, out IProtectedResourceSession ppProtectedSession ) ;
	
	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof(ID3D12Heap1) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Heap1).GUID
																	 .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable.Instantiate( ) => new Heap1( ) ;
	static IInstantiable IInstantiable.Instantiate( nint ptr ) => new Heap1( ptr ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom obj ) => new Heap1( ( obj as ID3D12Heap1 )! ) ;
	// ==================================================================================
} ;

