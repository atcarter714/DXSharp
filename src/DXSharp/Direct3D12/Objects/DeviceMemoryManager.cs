#region Using Directives
using System ;
using System.Linq ;
using System.Collections ;
using System.Collections.Generic ;
//! Interop:
using System.Runtime.InteropServices ;
using System.Runtime.CompilerServices ;
//! Multi-Threading:
using System.Threading ;
using System.Threading.Tasks ;
using System.Collections.Concurrent ;
//! Memory Management:
using System.Buffers ;
using System.Buffers.Binary ;
using System.Collections.ObjectModel ;

#endregion
namespace DXSharp.Direct3D12 ;


#warning NOT ready to use yet!

public sealed class DeviceMemoryManager {
	public static readonly DeviceMemoryManager Shared = new( ) ;
	readonly object _lock = new( ) ;
	readonly ConcurrentDictionary<
				Guid, ConcurrentDictionary< UID32, nint >
					> _usedIDLookup = new( ) ;
	
	MemoryPool< AllocationInfo > _allocInfoPool = 
		MemoryPool< AllocationInfo >.Shared ;
	
	//! Stores allocation data by type in thread-safe storage:
	readonly ConcurrentDictionary<
			Guid, ConcurrentBag< AllocationInfo >
			> allocationsByType = new( ) ;
	
	public ReadOnlyMemory< AllocationInfo > GetAllocationsFor< T >( ) =>
										GetAllocationsFor( typeof(T).GUID ) ;
	
	public ReadOnlyMemory< AllocationInfo > GetAllocationsFor( Guid key ) {
		unsafe {
			if ( allocationsByType.TryGetValue(key, out var list) ) {
				var allocInfo = list.ToArray( ) ;
				ReadOnlyMemory< AllocationInfo > span = allocInfo ;
				return allocInfo ;
			}
			return ReadOnlyMemory< AllocationInfo >.Empty ;
		}
	}
	
	/// <summary>
	/// Gets a read-only dictionary of allocations by type. Note that this creates a copy
	/// of the allocations data dictionary, which lives in unordered memory for concurrency.
	/// So you should try to access this once and cache it, rather than calling it repeatedly.
	/// </summary>
	public ReadOnlyDictionary< Guid, ReadOnlySequence<AllocationInfo> > AllocationsByTypes {
		get => new( allocationsByType.ToDictionary( 
					kvp => kvp.Key, 
						kvp => {
							 var allocInfo = kvp.Value.ToArray( ) ;
							 ReadOnlyMemory< AllocationInfo > mem = allocInfo ;
							 ReadOnlySequence< AllocationInfo > seq = new ( mem ) ;
							 return seq ;
					}) 
				) ;
	}
	
	public void AddAllocation< T >( nint address, ulong size ) =>
					AddAllocation( typeof(T).GUID, address, size ) ;
	
	public void AddAllocation( in Guid typeGuid, nint address, ulong size ) {
		UID32 _id = _getAvailableUID( typeGuid ) ;
		
		if ( allocationsByType.TryGetValue(typeGuid, out var list) )
			list.Add( new AllocationInfo( typeGuid, _id, size, address ) ) ;
		else list = new( ) {
			new( typeGuid, _id, size, address )
		} ;
	}
	
	public void RemoveAllocation< T >( nint address ) {
		var guid = typeof(T).GUID ;
		if ( allocationsByType.TryGetValue(guid, out var allocList) ) {
			var allocInfo = allocList.ToArray( ) ;
			
			var alloc = allocInfo.FirstOrDefault( a => a.Address == address ) ;
			if ( alloc.Address == address ) {
				bool success = allocList.TryTake( out var entry ) ;
				_usedIDLookup.TryGetValue( guid , out var usedIDList ) ;
				
				// Remove from used UID collection:
				if( success && usedIDList is not null ) {
					usedIDList.TryRemove( alloc.UID, out var _ ) ;
				}
			}
		}
	}

	UID32 _getAvailableUID( Guid key ) {
		var _id = UID32.Randomize( ) ;
		bool hasEntriesForGUID = _usedIDLookup
			.TryGetValue( key, out var _idLookup ) ;
		if ( _idLookup?.ContainsKey( _id ) ?? false ) {
			while ( _idLookup.ContainsKey(_id) ) {
				_id = UID32.Randomize( ) ;
			}
		}
		return _id ;
	}
} ;


[StructLayout( LayoutKind.Sequential )]
public readonly unsafe struct AllocationInfo {
	readonly UID32 _uid ;
	readonly Guid  _guid ;
	readonly ulong _size ;
	readonly nint  _address ;
	
	public UID32 UID => _uid ;
	public Guid Key => _guid ;
	public ulong Size => _size ;
	public long Address => _address ;
	public nint  Pointer => _address ;
	
	public AllocationInfo( Guid typeGuid, UID32 id, ulong size, nint pointer ) {
		_guid = typeGuid ; _uid  = id ; _size = size ; _address = pointer ;
	}
	public AllocationInfo( Guid typeGuid, UID32 id, ulong size, long address ) {
		_guid = typeGuid ; _uid = id ; _size = size ; _address = (nint)address ;
	}
	public AllocationInfo( Guid typeGuid, UID32 id, ulong size, void* pointer ) {
		_guid = typeGuid ; _uid = id ; _size = size ; _address = (nint)pointer ;
	}
} ;