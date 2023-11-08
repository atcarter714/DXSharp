#region Using Directives
using System.Runtime.InteropServices ;
using System.Runtime.Versioning ;
using Windows.Win32.Foundation ;
using DXSharp ;
using DXSharp.DXGI ;
using DXSharp.Windows.COM ;

#endregion
namespace Windows.Win32.Graphics.Dxgi ;


[NativeLibrary("dxgi.dll", "IDXGIInfoQueue", "Dxgidebug.h")]
[ComImport, Guid("D67441C7-672A-476F-9E82-CD55B44949CE"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ]
[SupportedOSPlatform("windows8.0")]
public interface IDXGIInfoQueue: IUnknown {
	
	/// <summary>Sets the maximum number of messages that can be added to the message queue.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that sets the limit on the number of messages.</param>
	/// <param name="MessageCountLimit">The maximum number of messages that can be added to the queue. –1 means no limit.</param>
	/// <returns>Returns S_OK if successful; an error code otherwise. For a list of error codes, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-setmessagecountlimit#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void SetMessageCountLimit(Guid Producer, ulong MessageCountLimit);

	/// <summary>Clears all messages from the message queue.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that clears the messages.</param>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-clearstoredmessages#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void ClearStoredMessages(Guid Producer);

	/// <summary>Gets a message from the message queue.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that gets the message.</param>
	/// <param name="MessageIndex">An index into the message queue after an optional retrieval filter has been applied. This can be between 0 and the number of messages in the message queue that pass through the retrieval filter. Call <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-getnumstoredmessagesallowedbyretrievalfilters">IDXGIInfoQueue::GetNumStoredMessagesAllowedByRetrievalFilters</a> to obtain this number. 0 is the message at the beginning of the message queue.</param>
	/// <param name="pMessage">A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/ns-dxgidebug-dxgi_info_queue_message">DXGI_INFO_QUEUE_MESSAGE</a> structure that describes the message.</param>
	/// <param name="pMessageByteLength">A pointer to a variable that receives the size, in bytes, of the message description that <i>pMessage</i> points to. This size includes the size of the <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/ns-dxgidebug-dxgi_info_queue_message">DXGI_INFO_QUEUE_MESSAGE</a> structure in bytes.</param>
	/// <returns>Returns S_OK if successful; an error code otherwise. For a list of error codes, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</returns>
	/// <remarks>
	/// <para>This method doesn't remove any messages from the message queue. This method gets a message from the message queue after an optional retrieval filter has been applied. Call this method twice to retrieve a message, first to obtain the size of the message and second to get the message. Here is a typical example:</para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-getmessage#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	unsafe void GetMessage(Guid Producer, ulong MessageIndex, [Optional] InfoQueueMessage* pMessage, ref nuint pMessageByteLength);

	/// <summary>Gets the number of messages that can pass through a retrieval filter.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that gets the number.</param>
	/// <returns>Returns the number of messages that can pass through a retrieval filter.</returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-getnumstoredmessagesallowedbyretrievalfilters#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	ulong GetNumStoredMessagesAllowedByRetrievalFilters(Guid Producer);

	/// <summary>Gets the number of messages currently stored in the message queue.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that gets the number.</param>
	/// <returns>Returns the number of messages currently stored in the message queue.</returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-getnumstoredmessages#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	ulong GetNumStoredMessages(Guid Producer);

	/// <summary>Gets the number of messages that were discarded due to the message count limit.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that gets the number.</param>
	/// <returns>Returns the number of messages that were discarded.</returns>
	/// <remarks>
	/// <para>Get and set the message count limit with <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-getmessagecountlimit">IDXGIInfoQueue::GetMessageCountLimit</a> and <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-setmessagecountlimit">IDXGIInfoQueue::SetMessageCountLimit</a>, respectively.</para>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-getnummessagesdiscardedbymessagecountlimit#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	ulong GetNumMessagesDiscardedByMessageCountLimit(Guid Producer);

	/// <summary>Gets the maximum number of messages that can be added to the message queue.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that gets the number.</param>
	/// <returns>Returns the maximum number of messages that can be added to the queue. –1 means no limit.</returns>
	/// <remarks>
	/// <para>When the number of messages in the message queue reaches the maximum limit, new messages coming in push old messages out. <div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-getmessagecountlimit#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	ulong GetMessageCountLimit(Guid Producer);

	/// <summary>Gets the number of messages that a storage filter allowed to pass through.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that gets the number.</param>
	/// <returns>Returns the number of messages allowed by a storage filter.</returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-getnummessagesallowedbystoragefilter#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	ulong GetNumMessagesAllowedByStorageFilter(Guid Producer);

	/// <summary>Gets the number of messages that were denied passage through a storage filter.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that gets the number.</param>
	/// <returns>Returns the number of messages denied by a storage filter.</returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-getnummessagesdeniedbystoragefilter#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	ulong GetNumMessagesDeniedByStorageFilter(Guid Producer);

	/// <summary>Adds storage filters to the top of the storage-filter stack.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that produced the filters.</param>
	/// <param name="pFilter">An array of <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/ns-dxgidebug-dxgi_info_queue_filter">InfoQueueFilter</a> structures that describe the filters.</param>
	/// <returns>Returns S_OK if successful; an error code otherwise. For a list of error codes, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-addstoragefilterentries#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	unsafe void AddStorageFilterEntries(Guid Producer, InfoQueueFilter* pFilter);

	/// <summary>Gets the storage filter at the top of the storage-filter stack.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that gets the filter.</param>
	/// <param name="pFilter">A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/ns-dxgidebug-dxgi_info_queue_filter">InfoQueueFilter</a> structure that describes the filter.</param>
	/// <param name="pFilterByteLength">A pointer to a variable that receives the size, in bytes, of the filter description to which <i>pFilter</i> points. If <i>pFilter</i> is <b>NULL</b>, <b>GetStorageFilter</b> outputs the size of the storage filter.</param>
	/// <returns>Returns S_OK if successful; an error code otherwise. For a list of error codes, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-getstoragefilter#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	unsafe void GetStorageFilter(Guid Producer, [Optional] InfoQueueFilter* pFilter, ref nuint pFilterByteLength);

	/// <summary>Removes a storage filter from the top of the storage-filter stack.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that removes the filter.</param>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-clearstoragefilter#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void ClearStorageFilter(Guid Producer);

	/// <summary>Pushes an empty storage filter onto the storage-filter stack.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that pushes the empty storage filter.</param>
	/// <returns>Returns S_OK if successful; an error code otherwise. For a list of error codes, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</returns>
	/// <remarks>
	/// <para>An empty storage filter allows all messages to pass through. <div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-pushemptystoragefilter#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void PushEmptyStorageFilter(Guid Producer);

	/// <summary>Pushes a deny-all storage filter onto the storage-filter stack.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that pushes the filter.</param>
	/// <returns>Returns S_OK if successful; an error code otherwise. For a list of error codes, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</returns>
	/// <remarks>
	/// <para>A deny-all storage filter prevents all messages from passing through. <div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-pushdenyallstoragefilter#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void PushDenyAllStorageFilter(Guid Producer);

	/// <summary>Pushes a copy of the storage filter that is currently on the top of the storage-filter stack onto the storage-filter stack.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that pushes the copy of the storage filter.</param>
	/// <returns>Returns S_OK if successful; an error code otherwise. For a list of error codes, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-pushcopyofstoragefilter#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void PushCopyOfStorageFilter(Guid Producer);

	/// <summary>Pushes a storage filter onto the storage-filter stack.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that pushes the filter.</param>
	/// <param name="pFilter">A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/ns-dxgidebug-dxgi_info_queue_filter">InfoQueueFilter</a> structure that describes the filter.</param>
	/// <returns>Returns S_OK if successful; an error code otherwise. For a list of error codes, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-pushstoragefilter#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	unsafe void PushStorageFilter(Guid Producer, InfoQueueFilter* pFilter);

	/// <summary>Pops a storage filter from the top of the storage-filter stack.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that pops the filter.</param>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-popstoragefilter#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void PopStorageFilter(Guid Producer);

	/// <summary>Gets the size of the storage-filter stack in bytes.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that gets the size.</param>
	/// <returns>Returns the size of the storage-filter stack in bytes.</returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-getstoragefilterstacksize#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	uint GetStorageFilterStackSize(Guid Producer);

	/// <summary>Adds retrieval filters to the top of the retrieval-filter stack.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that produced the filters.</param>
	/// <param name="pFilter">An array of <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/ns-dxgidebug-dxgi_info_queue_filter">InfoQueueFilter</a> structures that describe the filters.</param>
	/// <returns>Returns S_OK if successful; an error code otherwise. For a list of error codes, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-addretrievalfilterentries#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	unsafe void AddRetrievalFilterEntries(Guid Producer, InfoQueueFilter* pFilter);

	/// <summary>Gets the retrieval filter at the top of the retrieval-filter stack.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that gets the filter.</param>
	/// <param name="pFilter">A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/ns-dxgidebug-dxgi_info_queue_filter">InfoQueueFilter</a> structure that describes the filter.</param>
	/// <param name="pFilterByteLength">A pointer to a variable that receives the size, in bytes, of the filter description to which <i>pFilter</i> points. If <i>pFilter</i> is <b>NULL</b>, <b>GetRetrievalFilter</b> outputs the size of the retrieval filter.</param>
	/// <returns>Returns S_OK if successful; an error code otherwise. For a list of error codes, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-getretrievalfilter#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	unsafe void GetRetrievalFilter(Guid Producer, [Optional] InfoQueueFilter* pFilter, ref nuint pFilterByteLength);

	/// <summary>Removes a retrieval filter from the top of the retrieval-filter stack.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that removes the filter.</param>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-clearretrievalfilter#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void ClearRetrievalFilter(Guid Producer);

	/// <summary>Pushes an empty retrieval filter onto the retrieval-filter stack.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that pushes the empty retrieval filter.</param>
	/// <returns>Returns S_OK if successful; an error code otherwise. For a list of error codes, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</returns>
	/// <remarks>
	/// <para>An empty retrieval filter allows all messages to pass through. <div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-pushemptyretrievalfilter#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void PushEmptyRetrievalFilter(Guid Producer);

	/// <summary>Pushes a deny-all retrieval filter onto the retrieval-filter stack.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that pushes the deny-all retrieval filter.</param>
	/// <returns>Returns S_OK if successful; an error code otherwise. For a list of error codes, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</returns>
	/// <remarks>
	/// <para>A deny-all retrieval filter prevents all messages from passing through. <div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-pushdenyallretrievalfilter#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void PushDenyAllRetrievalFilter(Guid Producer);

	/// <summary>Pushes a copy of the retrieval filter that is currently on the top of the retrieval-filter stack onto the retrieval-filter stack.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that pushes the copy of the retrieval filter.</param>
	/// <returns>Returns S_OK if successful; an error code otherwise. For a list of error codes, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-pushcopyofretrievalfilter#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void PushCopyOfRetrievalFilter(Guid Producer);

	/// <summary>Pushes a retrieval filter onto the retrieval-filter stack.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that pushes the filter.</param>
	/// <param name="pFilter">A pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/ns-dxgidebug-dxgi_info_queue_filter">InfoQueueFilter</a> structure that describes the filter.</param>
	/// <returns>Returns S_OK if successful; an error code otherwise. For a list of error codes, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-pushretrievalfilter#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	unsafe void PushRetrievalFilter(Guid Producer, InfoQueueFilter* pFilter);

	/// <summary>Pops a retrieval filter from the top of the retrieval-filter stack.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that pops the filter.</param>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-popretrievalfilter#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void PopRetrievalFilter(Guid Producer);

	/// <summary>Gets the size of the retrieval-filter stack in bytes.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that gets the size.</param>
	/// <returns>Returns the size of the retrieval-filter stack in bytes.</returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-getretrievalfilterstacksize#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	uint GetRetrievalFilterStackSize(Guid Producer);

	/// <summary>Adds a debug message to the message queue and sends that message to the debug output.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that produced the message.</param>
	/// <param name="Category">A <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/ne-dxgidebug-dxgi_info_queue_message_category">InfoQueueMessageCategory</a>-typed value that specifies the category of the message.</param>
	/// <param name="Severity">A <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/ne-dxgidebug-dxgi_info_queue_message_severity">InfoQueueMessageSeverity</a>-typed value that specifies the severity of the message.</param>
	/// <param name="ID">An integer that uniquely identifies the message.</param>
	/// <param name="pDescription">The message string.</param>
	/// <returns>Returns S_OK if successful; an error code otherwise. For a list of error codes, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-addmessage#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void AddMessage(Guid Producer, InfoQueueMessageCategory Category, InfoQueueMessageSeverity Severity, int ID, PCSTR pDescription);

	/// <summary>Adds a user-defined message to the message queue and sends that message to the debug output.</summary>
	/// <param name="Severity">A <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/ne-dxgidebug-dxgi_info_queue_message_severity">InfoQueueMessageSeverity</a>-typed value that specifies the severity of the message.</param>
	/// <param name="pDescription">The message string.</param>
	/// <returns>Returns S_OK if successful; an error code otherwise. For a list of error codes, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-addapplicationmessage#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void AddApplicationMessage(InfoQueueMessageSeverity Severity, PCSTR pDescription);

	/// <summary>Sets a message category to break on when a message with that category passes through the storage filter.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that sets the breaking condition.</param>
	/// <param name="Category">A <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/ne-dxgidebug-dxgi_info_queue_message_category">InfoQueueMessageCategory</a>-typed value that specifies the category of the message.</param>
	/// <param name="bEnable">A Boolean value that specifies whether <b>SetBreakOnCategory</b> turns on or off this breaking condition (<b>TRUE</b> for on, <b>FALSE</b> for off).</param>
	/// <returns>Returns S_OK if successful; an error code otherwise. For a list of error codes, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-setbreakoncategory#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void SetBreakOnCategory(Guid Producer, InfoQueueMessageCategory Category, BOOL bEnable);

	/// <summary>Sets a message severity level to break on when a message with that severity level passes through the storage filter.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that sets the breaking condition.</param>
	/// <param name="Severity">A <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/ne-dxgidebug-dxgi_info_queue_message_severity">InfoQueueMessageSeverity</a>-typed value that specifies the severity of the message.</param>
	/// <param name="bEnable">A Boolean value that specifies whether <b>SetBreakOnSeverity</b> turns on or off this breaking condition (<b>TRUE</b> for on, <b>FALSE</b> for off).</param>
	/// <returns>Returns S_OK if successful; an error code otherwise. For a list of error codes, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-setbreakonseverity#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void SetBreakOnSeverity(Guid Producer, InfoQueueMessageSeverity Severity, BOOL bEnable);

	/// <summary>Sets a message identifier to break on when a message with that identifier passes through the storage filter.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that sets the breaking condition.</param>
	/// <param name="ID">An integer value that specifies the identifier of the message.</param>
	/// <param name="bEnable">A Boolean value that specifies whether <b>SetBreakOnID</b> turns on or off this breaking condition (<b>TRUE</b> for on, <b>FALSE</b> for off).</param>
	/// <returns>Returns S_OK if successful; an error code otherwise. For a list of error codes, see <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-setbreakonid#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void SetBreakOnID(Guid Producer, int ID, BOOL bEnable);

	/// <summary>Determines whether the break on a message category is turned on or off.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that gets the breaking status.</param>
	/// <param name="Category">A <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/ne-dxgidebug-dxgi_info_queue_message_category">InfoQueueMessageCategory</a>-typed value that specifies the category of the message.</param>
	/// <returns>Returns a Boolean value that specifies whether this category of breaking condition is turned on or off (<b>TRUE</b> for on, <b>FALSE</b> for off).</returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-getbreakoncategory#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	BOOL GetBreakOnCategory(Guid Producer, InfoQueueMessageCategory Category);

	/// <summary>Determines whether the break on a message severity level is turned on or off.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that gets the breaking status.</param>
	/// <param name="Severity">A <a href="https://docs.microsoft.com/windows/desktop/api/dxgidebug/ne-dxgidebug-dxgi_info_queue_message_severity">InfoQueueMessageSeverity</a>-typed value that specifies the severity of the message.</param>
	/// <returns>Returns a Boolean value that specifies whether this severity of breaking condition is turned on or off (<b>TRUE</b> for on, <b>FALSE</b> for off).</returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-getbreakonseverity#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	BOOL GetBreakOnSeverity(Guid Producer, InfoQueueMessageSeverity Severity);

	/// <summary>Determines whether the break on a message identifier is turned on or off.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that gets the breaking status.</param>
	/// <param name="ID">An integer value that specifies the identifier of the message.</param>
	/// <returns>Returns a Boolean value that specifies whether this break on a message identifier is turned on or off (<b>TRUE</b> for on, <b>FALSE</b> for off).</returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-getbreakonid#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	BOOL GetBreakOnID(Guid Producer, int ID);
	
	/// <summary>Turns the debug output on or off.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that gets the mute status.</param>
	/// <param name="bMute">A Boolean value that specifies whether to turn the debug output on or off (<b>TRUE</b> for on, <b>FALSE</b> for off).</param>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-setmutedebugoutput#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void SetMuteDebugOutput(Guid Producer, BOOL bMute);

	/// <summary>Determines whether the debug output is turned on or off.</summary>
	/// <param name="Producer">A <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-debug-id">DXGI_DEBUG_ID</a> value that identifies the entity that gets the mute status.</param>
	/// <returns>Returns a Boolean value that specifies whether the debug output is turned on or off (<b>TRUE</b> for on, <b>FALSE</b> for off).</returns>
	/// <remarks>
	/// <para><div class="alert"><b>Note</b>  This API requires the Windows Software Development Kit (SDK) for Windows 8.</div> <div> </div></para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgidebug/nf-dxgidebug-idxgiinfoqueue-getmutedebugoutput#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	BOOL GetMuteDebugOutput(Guid Producer);
} ;
