#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32 ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D12 ;

using WinCom = Windows.Win32.System.Com ;
using static DXSharp.InteropUtils ;
#endregion
namespace DXSharp.Direct3D12 ;


[ProxyFor( typeof(ID3D12Resource_unmanaged) )]
public unsafe struct ResourceUnmanaged: IComIID {
	// ---------------------------------------------------------------------------------------------------
	// Constructors:
	// ---------------------------------------------------------------------------------------------------
	
	public ResourceUnmanaged( void** lpVtbl ) {
		this.lpVtbl = lpVtbl ;
	}
	
	public ResourceUnmanaged( nint lpVtbl ) {
		this.lpVtbl = (void**)lpVtbl;
	}

	public ResourceUnmanaged( ID3D12Resource d3D12Resource ) {
		Guid riid = IID_Guid ;
		d3D12Resource.QueryInterface( ref riid, out var pVtbl ) ;
		this.lpVtbl = (void **)pVtbl ;
	}
	
	public ResourceUnmanaged( IResource resource ): this( ( (Resource)resource ).ComObject! ) { }
	
	// ---------------------------------------------------------------------------------------------------
	
	
	public HRESULT QueryInterface( in Guid riid, out void* ppvObject ) {
		fixed ( void** ppvObjectLocal = &ppvObject ) {
			fixed ( Guid* riidLocal = &riid ) {
				HRESULT __result = this.QueryInterface( riidLocal, ppvObjectLocal ) ;
				return __result ;
			}
		}
	}

	public HRESULT QueryInterface( Guid* riid, void** ppvObject ) {
		return ( (delegate *unmanaged [Stdcall]<ResourceUnmanaged*, Guid*, void**, HRESULT>)lpVtbl[ 0 ])( 
				 (ResourceUnmanaged *)Unsafe.AsPointer( ref this ), 
				 riid,
				 ppvObject
			 ) ;
	}

	
	public uint AddRef( ) {
		return
			( (delegate *unmanaged [Stdcall]<ResourceUnmanaged*, uint>)lpVtbl[ 1 ] )( (ResourceUnmanaged*)
				Unsafe.AsPointer( ref this ) ) ;
	}

	public uint Release( ) {
		return
			( (delegate *unmanaged [Stdcall]<ResourceUnmanaged*, uint>)lpVtbl[ 2 ] )( (ResourceUnmanaged*)
				Unsafe.AsPointer( ref this ) ) ;
	}
	
	
	public void GetPrivateData( in Guid guid, ref uint pDataSize, void* pData ) {
		fixed ( Guid* guidLocal = &guid ) {
			this.GetPrivateData( guidLocal, ref pDataSize, pData ) ;
		}
	}

	
	public void GetPrivateData( Guid* guid, ref uint pDataSize, [Optional] void* pData ) {
		( (delegate *unmanaged [Stdcall]<ResourceUnmanaged*, Guid*, ref uint, void*, HRESULT>)lpVtbl
					[ 3 ] )( (ResourceUnmanaged*)Unsafe.AsPointer( ref this ), guid, ref pDataSize, pData )
			.ThrowOnFailure( ) ;
	}

	
	public void SetPrivateData( in Guid guid, uint DataSize, void* pData ) {
		fixed ( Guid* guidLocal = &guid ) {
			this.SetPrivateData( guidLocal, DataSize, pData ) ;
		}
	}

	
	public void SetPrivateData( Guid* guid, uint DataSize, [Optional] void* pData ) {
		( (delegate *unmanaged [Stdcall]<ResourceUnmanaged*, Guid*, uint, void*, HRESULT>)lpVtbl
					[ 4 ] )( (ResourceUnmanaged*)Unsafe.AsPointer( ref this ), guid, DataSize, pData )
			.ThrowOnFailure( ) ;
	}

	
	public void SetPrivateDataInterface( in Guid guid, WinCom.IUnknown* pData ) {
		fixed ( Guid* guidLocal = &guid ) {
			this.SetPrivateDataInterface( guidLocal, pData ) ;
		}
	}

	public void SetPrivateDataInterface( Guid* guid, [Optional] WinCom.IUnknown* pData ) {
		( (delegate *unmanaged [Stdcall]<ResourceUnmanaged*, Guid*, WinCom.IUnknown*, HRESULT>)
				lpVtbl[ 5 ] )( (ResourceUnmanaged*)Unsafe.AsPointer( ref this ), guid, pData )
			.ThrowOnFailure( ) ;
	}

	
	public void SetName( string Name ) {
		fixed ( char* NameLocal = Name ) {
			this.SetName( NameLocal ) ;
		}
	}

	public void SetName( PCWSTR Name ) {
		( (delegate *unmanaged [Stdcall]<ResourceUnmanaged*, PCWSTR, HRESULT>)lpVtbl
					[ 6 ] )( (ResourceUnmanaged*)Unsafe.AsPointer( ref this ), Name ).ThrowOnFailure( ) ;
	}

	
	public void GetDevice( in Guid riid, void** ppvDevice ) {
		fixed ( Guid* riidLocal = &riid ) {
			this.GetDevice( riidLocal, ppvDevice ) ;
		}
	}

	public void GetDevice( Guid* riid, [Optional] void** ppvDevice ) {
		( (delegate *unmanaged [Stdcall]<ResourceUnmanaged*, Guid*, void**, HRESULT>)lpVtbl
					[ 7 ] )( (ResourceUnmanaged*)Unsafe.AsPointer( ref this ), riid, ppvDevice )
			.ThrowOnFailure( ) ;
	}

	
	public void Map( uint Subresource, Range? pReadRange, void** ppData ) {
		Range pReadRangeLocal = pReadRange ?? default( Range ) ;
		this.Map( Subresource, pReadRange.HasValue ? &pReadRangeLocal : null, ppData ) ;
	}

	
	public void Map( uint Subresource,
					 [Optional] Range* pReadRange,
					 [Optional] void** ppData ) {
		( (delegate *unmanaged [Stdcall]<ResourceUnmanaged*, uint, Range*, void**, HRESULT>)lpVtbl
					[ 8 ] )( (ResourceUnmanaged*)Unsafe.AsPointer( ref this ), Subresource, pReadRange, ppData )
			.ThrowOnFailure( ) ;
	}

	
	
	public void Unmap( uint Subresource, Range? pWrittenRange ) {
		Range pWrittenRangeLocal = pWrittenRange ?? default( Range ) ;
		this.Unmap( Subresource, pWrittenRange.HasValue ? &pWrittenRangeLocal : null ) ;
	}

	public void Unmap( uint Subresource,
							  [Optional] Range* pWrittenRange ) {
		( (delegate *unmanaged [Stdcall]<ResourceUnmanaged*, uint, Range*, void>)lpVtbl
					[ 9 ] )( (ResourceUnmanaged*)Unsafe.AsPointer( ref this ), Subresource, pWrittenRange ) ;
	}

	
	public ResourceDescription GetDesc( ) {
		return
			( (delegate *unmanaged [Stdcall]<ResourceUnmanaged*, ResourceDescription>)lpVtbl
					[ 10 ] )( (ResourceUnmanaged*)Unsafe.AsPointer( ref this ) ) ;
	}

	public ulong GetGPUVirtualAddress( ) {
		return
			( (delegate *unmanaged [Stdcall]<ResourceUnmanaged*, ulong>)lpVtbl[ 11 ] )( (ResourceUnmanaged
				*)Unsafe.AsPointer( ref this ) ) ;
	}

	
	public void WriteToSubresource( uint DstSubresource, Box? pDstBox, 
										   void* pSrcData, uint SrcRowPitch,
										   uint SrcDepthPitch ) {
		Box pDstBoxLocal = pDstBox ?? default( Box ) ;
		this.WriteToSubresource( DstSubresource, pDstBox.HasValue ? &pDstBoxLocal : null, pSrcData, SrcRowPitch,
								 SrcDepthPitch ) ;
	}

	
	public void WriteToSubresource( uint DstSubresource, 
										   [Optional] Box* pDstBox, 
										   void* pSrcData,
										   uint SrcRowPitch, 
										   uint SrcDepthPitch ) {
		( (delegate *unmanaged [Stdcall]<ResourceUnmanaged*, uint, Box*, void*, uint, uint, HRESULT>)lpVtbl
					[ 12 ] )( (ResourceUnmanaged*)Unsafe.AsPointer( ref this ), DstSubresource, pDstBox,
							  pSrcData, SrcRowPitch, SrcDepthPitch ).ThrowOnFailure( ) ;
	}

	
	
	public void ReadFromSubresource( void* pDstData, 
											uint DstRowPitch, 
											uint DstDepthPitch, 
											uint SrcSubresource,
											Box? pSrcBox ) {
		Box pSrcBoxLocal = pSrcBox ?? default( Box ) ;
		this.ReadFromSubresource( pDstData, DstRowPitch, DstDepthPitch, SrcSubresource,
								  pSrcBox.HasValue ? &pSrcBoxLocal : null ) ;
	}

	
	public void ReadFromSubresource( void* pDstData, 
											uint DstRowPitch, 
											uint DstDepthPitch, 
											uint SrcSubresource,
											[Optional] Box* pSrcBox ) {
		( (delegate *unmanaged [Stdcall]<ResourceUnmanaged*, void*, uint, uint, uint, Box*, HRESULT>)lpVtbl
					[ 13 ] )( (ResourceUnmanaged*)Unsafe.AsPointer( ref this ), pDstData, DstRowPitch,
							  DstDepthPitch, SrcSubresource, pSrcBox ).ThrowOnFailure( ) ;
	}

	
	public void GetHeapProperties( [Optional] HeapProperties* pHeapProperties,
										  [Optional] HeapFlags* pHeapFlags ) {
		( (delegate *unmanaged [Stdcall]<ResourceUnmanaged*, HeapProperties*, HeapFlags*, HRESULT>)
				lpVtbl[ 14 ] )( (ResourceUnmanaged*)Unsafe.AsPointer( ref this ), pHeapProperties, pHeapFlags )
			.ThrowOnFailure( ) ;
	}

	
	void** lpVtbl ;
	
	public ref Vtbl VTable {
		[MethodImpl(_MAXOPT_)] get {
			fixed( void* pVtbl = &lpVtbl ) {
				return ref ( *( (Vtbl*)pVtbl ) ) ;
			}
		}
	}
	
	
	[DXSharp.DirectX( typeof(Vtbl), "V-Table",
					  nameof(ResourceUnmanaged) )]
	public struct Vtbl {
		internal delegate *unmanaged [Stdcall]<ResourceUnmanaged*, Guid*, void**, HRESULT> QueryInterface_1 ;

		internal delegate *unmanaged [Stdcall]<ResourceUnmanaged*, uint> AddRef_2 ;

		internal delegate *unmanaged [Stdcall]<ResourceUnmanaged*, uint> Release_3 ;

		internal delegate *unmanaged [Stdcall]<ResourceUnmanaged*, Guid*, ref uint, void*, HRESULT>
			GetPrivateData_4 ;

		internal delegate *unmanaged [Stdcall]<ResourceUnmanaged*, Guid*, uint, void*, HRESULT>
			SetPrivateData_5 ;

		internal delegate *unmanaged [Stdcall]
			< ResourceUnmanaged*, Guid*, WinCom.IUnknown*, HRESULT > SetPrivateDataInterface_6 ;
		
		internal delegate *unmanaged [Stdcall]< ResourceUnmanaged*, PCWSTR, HRESULT> SetName_7 ;

		internal delegate *unmanaged [Stdcall]<ResourceUnmanaged*, Guid*, void**, HRESULT> GetDevice_8 ;

		internal delegate *unmanaged [Stdcall]<ResourceUnmanaged*, uint, Range*, void**, HRESULT> Map_9 ;

		internal delegate *unmanaged [Stdcall]<ResourceUnmanaged*, uint, Range*, void> Unmap_10 ;

		internal delegate *unmanaged [Stdcall]<ResourceUnmanaged*, ResourceDescription> GetDesc_11 ;

		internal delegate *unmanaged [Stdcall]<ResourceUnmanaged*, ulong> GetGPUVirtualAddress_12 ;

		internal delegate *unmanaged [Stdcall]<ResourceUnmanaged*, uint, Box*, void*, uint, uint, HRESULT>
			WriteToSubresource_13 ;

		internal delegate *unmanaged [Stdcall]<ResourceUnmanaged*, void*, uint, uint, uint, Box*, HRESULT>
			ReadFromSubresource_14 ;

		internal delegate *unmanaged [Stdcall]<ResourceUnmanaged*, HeapProperties*, HeapFlags*,
			HRESULT> GetHeapProperties_15 ;
	}
	
	// ---------------------------------------------------------------------------------------------------
	// Static Members:
	// ---------------------------------------------------------------------------------------------------
	
	/// <summary>The IID guid for this interface.</summary>
	/// <value>{696442be-a72e-4059-bc79-5b5c98040fad}</value>
	public static readonly Guid IID_Guid =
		new Guid( 0x696442BE, 0xA72E, 0x4059, 0xBC,
				  0x79, 0x5B, 0x5C, 0x98, 0x04, 0x0F, 0xAD ) ;

	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = new byte[ ] {
				0xBE, 0x42, 0x64, 0x69, 0x2E, 0xA7, 
				0x59, 0x40, 0xBC, 0x79, 0x5B, 0x5C, 
				0x98, 0x04, 0x0F, 0xAD
			} ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	
	
	public static ResourceUnmanaged GetUnmanaged( IResource resource ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
		ArgumentNullException.ThrowIfNull( resource, nameof(resource) ) ;
#endif
		
		ResourceUnmanaged unmanaged = new( ( (Resource)resource )
												.ComPointer!.InterfaceVPtr) ;
		
		return unmanaged ;
	}
	
	public static IResource GetManaged( ResourceUnmanaged unmanaged ) => 
		new Resource( (nint)unmanaged.lpVtbl ) ;
	
	public static IResource GetManaged( ResourceUnmanaged* unmanaged ) => 
		new Resource( (nint)unmanaged->lpVtbl ) ;

	// ---------------------------------------------------------------------------------------------------
	// Operator Overloads:
	// ---------------------------------------------------------------------------------------------------
	
	
	// ====================================================================================================
} ;