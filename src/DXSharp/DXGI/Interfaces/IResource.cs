#region Using Directives

using System.Collections.ObjectModel ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.Windows.Win32 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.DXGI ;


[ProxyFor(typeof(IDXGIResource))]
public interface IResource: IDeviceSubObject,
							IInstantiable {
	// ---------------------------------------------------------------------------------
	
	internal static readonly ReadOnlyDictionary< Guid, Func<IDXGIResource, IInstantiable> > _resourceCreationFunctions =
		new( new Dictionary<Guid, Func<IDXGIResource, IInstantiable> > {
			{ IResource.IID, ( pComObj ) => new Resource( pComObj ) },
			{ IResource1.IID, ( pComObj ) => new Resource1( (pComObj as IDXGIResource1)! ) },
		} ) ;

	// ---------------------------------------------------------------------------------
	void GetEvictionPriority( [Out] out ResourcePriority pEvictionPriority ) ;
	
	void SetEvictionPriority( ResourcePriority evictionPriority ) ;
	
	void GetUsage( [Out] out Usage pUsage ) ;
	
	void GetSharedHandle( [Out] out Win32Handle pSharedHandle ) ;
	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof(IDXGIResource) ;
	public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIResource).GUID
															  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	
	static IInstantiable IInstantiable. Instantiate( ) => new Resource( ) ;
	static IInstantiable IInstantiable.Instantiate( IntPtr pComObj ) => new Resource( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => 
		new Resource( (IDXGIResource)pComObj! ) ;
	// ==================================================================================
} ;


[ProxyFor(typeof(IDXGIResource1))]
public interface IResource1: IResource {
	// ---------------------------------------------------------------------------------
	/// <summary>Creates a subresource surface object.</summary>
	/// <param name="index">The index of the subresource surface object to enumerate.</param>
	/// <param name="ppSurface">The address of a pointer to a <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nn-dxgi1_2-idxgisurface2">IDXGISurface2</a> interface that represents the created subresource surface object at the position specified by the <i>index</i> parameter.</param>
	/// <returns>
	/// <para>Returns S_OK if successful; otherwise, returns one of the following values: </para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para>Subresource surface objects implement the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi1_2/nn-dxgi1_2-idxgisurface2">IDXGISurface2</a> interface, which inherits from  <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgisurface1">IDXGISurface1</a> and indirectly <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nn-dxgi-idxgisurface">IDXGISurface</a>.  Therefore, the GDI-interoperable methods of <b>IDXGISurface1</b> work if the original resource interface object was created with the GDI-interoperable flag (<a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ne-d3d11-d3d11_resource_misc_flag">D3D11_RESOURCE_MISC_GDI_COMPATIBLE</a>). <b>CreateSubresourceSurface</b> creates a subresource surface that is based on the resource interface on which <b>CreateSubresourceSurface</b> is called. For example, if the original resource interface object is a 2D texture, the created subresource surface is also a 2D texture. You can use <b>CreateSubresourceSurface</b> to create parts of  a stereo resource so you can use Direct2D on either the left or right part of the stereo resource.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgiresource1-createsubresourcesurface#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void CreateSubresourceSurface( uint index, out ISurface2 ppSurface ) ;

	/// <summary>Creates a handle to a shared resource. You can then use the returned handle with multiple Direct3D devices.</summary>
	/// <param name="pAttributes">
	/// <para>A pointer to a <a href="https://docs.microsoft.com/previous-versions/windows/desktop/legacy/aa379560(v=vs.85)">SECURITY_ATTRIBUTES</a> structure that contains two separate but related data members: an optional security descriptor, and a Boolean value that determines whether child processes can inherit the returned handle. Set this parameter to <b>NULL</b> if you want child processes that the application might create to not  inherit  the handle returned by <b>CreateSharedHandle</b>, and if you want the resource that is associated with the returned handle to get a default security descriptor. The <b>lpSecurityDescriptor</b> member of the structure specifies a <a href="https://docs.microsoft.com/windows/desktop/api/winnt/ns-winnt-security_descriptor">SECURITY_DESCRIPTOR</a> for the resource. Set this member to <b>NULL</b> if you want the runtime to assign a default security descriptor to the resource that is associated with the returned handle. The ACLs in the default security descriptor for the resource come from the primary or impersonation token of the creator. For more info, see <a href="https://docs.microsoft.com/windows/desktop/Sync/synchronization-object-security-and-access-rights">Synchronization Object Security and Access Rights</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgiresource1-createsharedhandle#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="dwAccess">
	/// <para>The requested access rights to the resource.  In addition to the <a href="https://docs.microsoft.com/windows/desktop/SecAuthZ/generic-access-rights">generic access rights</a>, DXGI defines the following values: </para>
	/// <para>This doc was truncated.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgiresource1-createsharedhandle#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="lpName">
	/// <para>The name of the resource to share. The name is limited to MAX_PATH characters. Name comparison is case sensitive. You will need the resource name if you call the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11_1/nf-d3d11_1-id3d11device1-opensharedresourcebyname">ID3D11Device1::OpenSharedResourceByName</a> method to access the shared resource by name. If you instead call the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11_1/nf-d3d11_1-id3d11device1-opensharedresource1">ID3D11Device1::OpenSharedResource1</a> method to access the shared resource by handle, set this parameter to <b>NULL</b>. If <i>lpName</i> matches the name of an existing resource, <b>CreateSharedHandle</b> fails with <a href="https://docs.microsoft.com/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR_NAME_ALREADY_EXISTS</a>. This occurs because these objects share the same namespace. The name can have a "Global\" or "Local\" prefix to explicitly create the object in the global or session namespace. The remainder of the name can contain any character except the backslash character (\\). For more information, see <a href="https://docs.microsoft.com/windows/desktop/TermServ/kernel-object-namespaces">Kernel Object Namespaces</a>. Fast user switching is implemented using Terminal Services sessions. Kernel object names must follow the guidelines outlined for Terminal Services so that applications can support multiple users. The object can be created in a private namespace. For more information, see <a href="https://docs.microsoft.com/windows/desktop/Sync/object-namespaces">Object Namespaces</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgiresource1-createsharedhandle#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pHandle">A pointer to a variable that receives the NT HANDLE value to the resource to share.  You can  use this handle in calls to access the resource.</param>
	/// <returns>
	/// <para>Returns S_OK if successful; otherwise, returns one of the following values: </para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para><b>CreateSharedHandle</b> only returns the NT handle when you  created the resource as shared and specified that it uses NT handles (that is, you set the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ne-d3d11-d3d11_resource_misc_flag">D3D11_RESOURCE_MISC_SHARED_NTHANDLE</a> and <a href="https://docs.microsoft.com/windows/desktop/api/d3d11/ne-d3d11-d3d11_resource_misc_flag">D3D11_RESOURCE_MISC_SHARED_KEYEDMUTEX</a> flags). If you  created the resource as shared and specified that it uses NT handles, you must use <b>CreateSharedHandle</b> to get a handle for sharing.  In this situation, you can't use the <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiresource-getsharedhandle">IDXGIResource::GetSharedHandle</a> method because it will fail. You can pass the handle that  <b>CreateSharedHandle</b> returns in a call to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11_1/nf-d3d11_1-id3d11device1-opensharedresource1">ID3D11Device1::OpenSharedResource1</a> method to give a device access to a shared resource that you created on a different device. Because the handle that  <b>CreateSharedHandle</b> returns is an NT handle, you can use the handle with <a href="https://docs.microsoft.com/windows/desktop/api/handleapi/nf-handleapi-closehandle">CloseHandle</a>, <a href="https://docs.microsoft.com/windows/desktop/api/handleapi/nf-handleapi-duplicatehandle">DuplicateHandle</a>, and so on. You can call <b>CreateSharedHandle</b> only once for a shared resource; later calls fail.  If you need more handles to the same shared resource, call <b>DuplicateHandle</b>. When you no longer need the shared resource handle, call <b>CloseHandle</b> to close the handle, in order to avoid memory leaks. If you pass a name for the resource to <i>lpName</i> when you call <b>CreateSharedHandle</b> to share the resource, you can subsequently pass this name in a call to the <a href="https://docs.microsoft.com/windows/desktop/api/d3d11_1/nf-d3d11_1-id3d11device1-opensharedresourcebyname">ID3D11Device1::OpenSharedResourceByName</a> method to give another device access to the shared resource. If you use a named resource, a malicious user can use this named resource before you do and prevent your app from starting. To prevent this situation, create a randomly named resource and store the name so that it can only be obtained by an authorized user. Alternatively, you can use a file for this purpose. To limit your app to one instance per user, create a locked file in the user's profile directory. If you  created the resource as shared and did not specify that it uses NT handles, you cannot use <b>CreateSharedHandle</b> to get a handle for sharing because <b>CreateSharedHandle</b> will fail.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgiresource1-createsharedhandle#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void CreateSharedHandle( [Optional] in SecurityAttributes pAttributes, uint dwAccess, string lpName, in Win32Handle pHandle ) ;

	// ---------------------------------------------------------------------------------
	new static Type ComType => typeof(IDXGIResource1) ;public new static Guid IID => (ComType.GUID) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(IDXGIResource1).GUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	static IInstantiable IInstantiable.Instantiate( ) => new Resource1( ) ;
	static IInstantiable IInstantiable.Instantiate( IntPtr pComObj ) => new Resource1( pComObj ) ;
	static IInstantiable IInstantiable.Instantiate< ICom >( ICom pComObj ) => 
		new Resource1( (IDXGIResource1)pComObj! ) ;
	// ==================================================================================
} ;
