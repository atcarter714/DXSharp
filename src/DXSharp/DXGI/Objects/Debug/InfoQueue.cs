#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using System.Runtime.Versioning;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI.Debugging ;


[SupportedOSPlatform( "windows8.0" )]
[Wrapper(typeof(IDXGIInfoQueue))]
internal class InfoQueue: DisposableObject, 
						  IInfoQueue,
						  IComObjectRef< IDXGIInfoQueue >,
						  IUnknownWrapper< IDXGIInfoQueue > {
	protected COMResource? ComResources { get ; set ; }
	void _initOrAdd< T >( ComPtr< T > ptr ) where T: IUnknown {
		if( ComResources is null ) ComResources = new( ptr ) ;
		else ComResources.AddPointer<T>( ptr ) ;
	}
	
	protected ComPtr< IDXGIInfoQueue >? _comptr ;
	public virtual ComPtr< IDXGIInfoQueue >? ComPointer => _comptr ??= ComResources?.GetPointer< IDXGIInfoQueue >( ) ;
	public virtual ComPtr? ComPtrBase => this.ComPointer ;

	public virtual IDXGIInfoQueue? ComObject => 
		ComPointer?.InterfaceObjectRef as IDXGIInfoQueue ;

	// ---------------------------------------------------------------------------
	
	public InfoQueue( ) {
		_comptr = ComResources?.GetPointer< IDXGIInfoQueue >(  ) ;
	}
	public InfoQueue( nint ptr ) {
		_comptr = new( ptr ) ;
		_initOrAdd( _comptr ) ;
	}
	public InfoQueue( in IDXGIInfoQueue dxgiObj ) {
		_comptr = new( dxgiObj ) ;
		_initOrAdd( _comptr ) ;
	}
	public InfoQueue( ComPtr<IDXGIInfoQueue> otherPtr ) {
		_comptr = otherPtr ;
		_initOrAdd( _comptr ) ;
	}
	~InfoQueue( ) => Dispose( false ) ;
	
	// ---------------------------------------------------------------------------
	protected override ValueTask DisposeUnmanaged( ) {
		_comptr?.Dispose( ) ;
		return ValueTask.CompletedTask ;
	}

	public override async ValueTask DisposeAsync( ) {
		await base.DisposeAsync( ) ;
		await Task.Run( Dispose ) ;
	}
	
	// ---------------------------------------------------------------------------

	public bool GetMuteDebugOutput( Guid Producer ) => 
		ComObject!.GetMuteDebugOutput( Producer ) ;

	public void AddMessage( Guid Producer,
							InfoQueueMessageCategory Category, 
							InfoQueueMessageSeverity Severity, 
							int ID, PCSTR pDescription ) {
		ComObject!.AddMessage( Producer, Category, Severity, ID, pDescription ) ;
	}

	public void GetMessage( Guid Producer,
							ulong MessageIndex,
							ref nuint pMessageByteLength,
							in InfoQueueMessage? pMessage = null ) {
		unsafe {
			if ( pMessage is not null ) {
				fixed( void* pMsg = &pMessage ) {
                				ComObject!.GetMessage( Producer, MessageIndex, 
                										(InfoQueueMessage *)pMsg, 
                												ref pMessageByteLength ) ;
				}
			}
			else ComObject!.GetMessage( Producer, MessageIndex, 
										null, ref pMessageByteLength ) ;
		}
	}

	public void AddApplicationMessage( InfoQueueMessageSeverity Severity, PCSTR pDescription ) {
		ComObject!.AddApplicationMessage( Severity, pDescription ) ;
	}

	public void ClearRetrievalFilter( Guid Producer ) {
		ComObject!.ClearRetrievalFilter( Producer ) ;
	}

	public void ClearStorageFilter( Guid Producer ) {
		ComObject!.ClearStorageFilter( Producer ) ;
	}

	public void ClearStoredMessages( Guid Producer ) {
		ComObject!.ClearStoredMessages( Producer ) ;
	}

	public unsafe void GetRetrievalFilter( Guid Producer, ref nuint pFilterByteLength,
										   in InfoQueueFilter? pFilter = null ) {
		if ( pFilter is not null ) {
			fixed( void* pFilt = &pFilter ) {
				ComObject!.GetRetrievalFilter( Producer,
												   (InfoQueueFilter *)pFilt, 
														ref pFilterByteLength ) ;
			}
		}
		else ComObject!.GetRetrievalFilter( Producer, null, ref pFilterByteLength ) ;
	}

	public void GetStorageFilter( Guid Producer, 
								  ref nuint pFilterByteLength, 
								  in Span< InfoQueueFilter > pFilter = default ) {
		unsafe {
			if( pFilter is { Length: > 0 } ) {
				fixed ( InfoQueueFilter* pFilt = &pFilter[ 0 ] ) {
					ComObject!.GetStorageFilter( Producer, pFilt,
												 ref pFilterByteLength ) ;
				}
			}
			else ComObject!.GetStorageFilter( Producer, null, ref pFilterByteLength ) ;
		}
	}

	public void PopRetrievalFilter( Guid Producer ) => ComObject!.PopRetrievalFilter( Producer ) ;
	
	public void PopStorageFilter( Guid Producer ) => ComObject!.PopStorageFilter( Producer ) ;

	public void PushRetrievalFilter( Guid Producer, in InfoQueueFilter pFilter ) {
		unsafe {
			fixed ( InfoQueueFilter* pFilt = &pFilter ) {
				ComObject!.PushRetrievalFilter( Producer, pFilt ) ;
			}
		}
	}

	public void PushStorageFilter( Guid Producer, in InfoQueueFilter pFilter ) {
		unsafe {
			fixed ( InfoQueueFilter* pFilt = &pFilter ) {
				ComObject!.PushStorageFilter( Producer, pFilt ) ;
			}
		}
	}

	public void AddRetrievalFilterEntries( Guid Producer, in InfoQueueFilter pFilter ) {
		unsafe {
			fixed ( InfoQueueFilter* pFilt = &pFilter ) {
				ComObject!.AddRetrievalFilterEntries( Producer, pFilt ) ;
			}
		}
	}

	public void AddStorageFilterEntries( Guid Producer, in Span< InfoQueueFilter > pFilter ) {
		unsafe {
			if( pFilter is { Length: > 0 } ) {
				fixed ( InfoQueueFilter* pFilt = &pFilter[ 0 ] ) {
					ComObject!.AddStorageFilterEntries( Producer, pFilt ) ;
				}
			}
		}
	}

	public bool GetBreakOnCategory( Guid Producer, InfoQueueMessageCategory Category ) => 
		ComObject!.GetBreakOnCategory( Producer, Category ) ;

	public bool GetBreakOnSeverity( Guid Producer, InfoQueueMessageSeverity Severity ) => 
		ComObject!.GetBreakOnSeverity( Producer, Severity ) ;

	public ulong GetMessageCountLimit( Guid Producer ) => ComObject!.GetMessageCountLimit( Producer ) ;

	public ulong GetNumStoredMessages( Guid Producer ) => ComObject!.GetNumStoredMessages( Producer ) ;

	public void PushEmptyRetrievalFilter( Guid Producer ) => ComObject!.PushEmptyRetrievalFilter( Producer ) ;

	public void PushEmptyStorageFilter( Guid Producer ) => ComObject!.PushEmptyStorageFilter( Producer ) ;

	public void SetBreakOnCategory( Guid Producer, InfoQueueMessageCategory Category, bool bEnable ) => 
		ComObject!.SetBreakOnCategory( Producer, Category, bEnable ) ;

	public void SetBreakOnSeverity( Guid Producer, InfoQueueMessageSeverity Severity, bool bEnable ) => 
		ComObject!.SetBreakOnSeverity( Producer, Severity, bEnable ) ;

	public void SetMessageCountLimit( Guid Producer, ulong MessageCountLimit ) => 
		ComObject!.SetMessageCountLimit( Producer, MessageCountLimit ) ;

	public void SetMuteDebugOutput( Guid Producer, bool bMute ) => 
		ComObject!.SetMuteDebugOutput( Producer, bMute ) ;

	public bool GetBreakOnID( Guid Producer, int ID ) => 
		ComObject!.GetBreakOnID( Producer, ID ) ;

	public uint GetRetrievalFilterStackSize( Guid Producer ) => 
		ComObject!.GetRetrievalFilterStackSize( Producer ) ;

	public uint GetStorageFilterStackSize( Guid Producer ) => 
		ComObject!.GetStorageFilterStackSize( Producer ) ;

	public void PushCopyOfRetrievalFilter( Guid Producer ) => 
		ComObject!.PushCopyOfRetrievalFilter( Producer ) ;

	public void PushCopyOfStorageFilter( Guid Producer ) => 
		ComObject!.PushCopyOfStorageFilter( Producer ) ;

	public void PushDenyAllRetrievalFilter( Guid Producer ) => 
		ComObject!.PushDenyAllRetrievalFilter( Producer ) ;

	public void PushDenyAllStorageFilter( Guid Producer ) {
		ComObject!.PushDenyAllStorageFilter( Producer ) ;
	}

	public void SetBreakOnID( Guid Producer, int ID, bool bEnable ) {
		ComObject!.SetBreakOnID( Producer, ID, bEnable ) ;
	}

	public ulong GetNumMessagesAllowedByStorageFilter( Guid Producer ) {
		return ComObject!.GetNumMessagesAllowedByStorageFilter( Producer ) ;
	}

	public ulong GetNumMessagesDeniedByStorageFilter( Guid Producer ) {
		return ComObject!.GetNumMessagesDeniedByStorageFilter( Producer ) ;
	}

	public ulong GetNumMessagesDiscardedByMessageCountLimit( Guid Producer ) {
		return ComObject!.GetNumMessagesDiscardedByMessageCountLimit( Producer ) ;
	}

	public ulong GetNumStoredMessagesAllowedByRetrievalFilters( Guid Producer ) {
		return ComObject!.GetNumStoredMessagesAllowedByRetrievalFilters( Producer ) ;
	}

	// ---------------------------------------------------------------------------
	public static Type ComType => typeof(IDXGIInfoQueue) ;
	public static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )] get {
			ReadOnlySpan< byte > data = typeof( IDXGIInfoQueue ).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference( data ) ) ;
		}
	}
	// ===========================================================================
} ;





	/*
	public void ClearStoredMessages( ) =>
		ComObject!.ClearStoredMessages( ) ;
		 
	public uint GetNumStoredMessages( ) =>
	 		ComObject!.GetNumStoredMessages( ) ;
	 		
	public void GetStoredMessage( uint messageIndex, out InfoQueueMessage message, out uint messageByteLength ) =>
	 		ComObject!.GetStoredMessage( messageIndex, out message, out messageByteLength ) ;
	 		*/
	 		