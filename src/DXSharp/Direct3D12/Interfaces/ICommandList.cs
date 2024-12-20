﻿#region Using Directives

using System.Collections.ObjectModel ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
#endregion
namespace DXSharp.Direct3D12 ;


[ProxyFor(typeof(ID3D12CommandList))]
public interface ICommandList: IDeviceChild,
							   IInstantiable {
	// ---------------------------------------------------------------------------------
	//! Creation Functions:
	// ---------------------------------------------------------------------------------
	internal static readonly ReadOnlyDictionary< Guid, Func<ID3D12CommandList, IInstantiable> > _listCreationFunctions =
		new( new Dictionary<Guid, Func<ID3D12CommandList, IInstantiable> > {
			{ ICommandList.IID, ( pComObj ) => new CommandList( pComObj ) },
			{ IGraphicsCommandList.IID, ( pComObj ) => new GraphicsCommandList( (pComObj as ID3D12GraphicsCommandList)! ) },
			{ IGraphicsCommandList1.IID, ( pComObj ) => new GraphicsCommandList1( (pComObj as ID3D12GraphicsCommandList1)! ) },
			{ IGraphicsCommandList2.IID, ( pComObj ) => new GraphicsCommandList2( (pComObj as ID3D12GraphicsCommandList2)! ) },
			{ IGraphicsCommandList3.IID, ( pComObj ) => new GraphicsCommandList3( (pComObj as ID3D12GraphicsCommandList3)! ) },
			{ IGraphicsCommandList4.IID, ( pComObj ) => new GraphicsCommandList4( (pComObj as ID3D12GraphicsCommandList4)! ) },
			{ IGraphicsCommandList5.IID, ( pComObj ) => new GraphicsCommandList5( (pComObj as ID3D12GraphicsCommandList5)! ) },
			{ IGraphicsCommandList6.IID, ( pComObj ) => new GraphicsCommandList6( (pComObj as ID3D12GraphicsCommandList6)! ) },
			{ IGraphicsCommandList7.IID, ( pComObj ) => new GraphicsCommandList7( (pComObj as ID3D12GraphicsCommandList7)! ) },
			{ IGraphicsCommandList8.IID, ( pComObj ) => new GraphicsCommandList8( (pComObj as ID3D12GraphicsCommandList8)! ) },
			{ IGraphicsCommandList9.IID, ( pComObj ) => new GraphicsCommandList9( (pComObj as ID3D12GraphicsCommandList9)! ) },
		} ) ;

	// ---------------------------------------------------------------------------------

	
	// ---------------------------------------------------------------------------------
	
	/// <summary>Gets the type of the command list, such as direct, bundle, compute, or copy.</summary>
	/// <returns>
	/// This method returns the type of the command list, as an enumeration constant, such as direct, bundle, compute, or copy.
	/// The <see cref="CommandListType"/> enumeration specifies the possible values and is the equivalent of
	/// the native <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_command_list_type">D3D12_COMMAND_LIST_TYPE</a>
	/// enumeration in the core Direct3D 12 API.
	/// </returns>
	/// <remarks>
	/// <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandlist-gettype">
	/// Learn more about this API from docs.microsoft.com</a>.
	/// </remarks>
	CommandListType GetListType( ) ;
	
	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof(ID3D12CommandList) ;
	new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12CommandList).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable. Instantiate( ) => new CommandList( ) ;
	static IInstantiable IInstantiable.Instantiate( nint pComObj ) => new CommandList( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => new CommandList( (ID3D12CommandList?)pComObj! ) ;
	// ==================================================================================
} ;