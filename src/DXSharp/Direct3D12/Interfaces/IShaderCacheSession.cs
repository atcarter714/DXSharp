#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;

using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;

using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


/// <summary>Represents a shader cache session.</summary>
[ProxyFor(typeof(ID3D12ShaderCacheSession))]
public interface IShaderCacheSession: IDeviceChild {
	
	/// <summary>Looks up an entry in the cache whose key exactly matches the provided key.</summary>
	/// <param name="pKey">
	/// <para>Type: \_In\_reads\_bytes\_(KeySize) **const void \*** The key of the entry to look up.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12shadercachesession-findvalue#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <param name="KeySize">
	/// <para>Type: **[UINT](/windows/win32/winprog/windows-data-types)** The size of the key, in bytes.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12shadercachesession-findvalue#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <param name="pValue">
	/// <para>Type: \_Out\_writes\_bytes\_(*pValueSize) **void \*** A pointer to a memory block that receives the cached entry.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12shadercachesession-findvalue#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <param name="pValueSize">
	/// <para>Type: \_Inout\_ **[UINT](/windows/win32/winprog/windows-data-types)\*** A pointer to a **UINT** that receives the size of the cached entry, in bytes.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12shadercachesession-findvalue#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[HRESULT](/windows/win32/com/structure-of-com-error-codes)** If the function succeeds, it returns **S_OK**. Otherwise, it returns an [**HRESULT**](/windows/desktop/com/structure-of-com-error-codes) [error code](/windows/win32/com/com-error-codes-10). |Return value|Description| |-|-| | DXGI_ERROR_CACHE_HASH_COLLISION | There's an entry with the same hash as the provided key, but the key doesn't exactly match. | | DXGI_ERROR_NOT_FOUND | The entry isn't present. |</para>
	/// </returns>
	void FindValue( nint pKey, uint KeySize, nint pValue, ref uint pValueSize) ;

	/// <summary>Adds an entry to the cache.</summary>
	/// <param name="pKey">
	/// <para>Type: \_In\_reads\_bytes\_(KeySize) **const void \*** The key of the entry to add.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12shadercachesession-storevalue#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <param name="KeySize">
	/// <para>Type: **[UINT](/windows/win32/winprog/windows-data-types)** The size of the key, in bytes.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12shadercachesession-storevalue#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <param name="pValue">
	/// <para>Type: \_In\_reads\_bytes\_(ValueSize) **void \*** A pointer to a memory block containing the entry to add.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12shadercachesession-storevalue#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <param name="ValueSize">
	/// <para>Type: **[UINT](/windows/win32/winprog/windows-data-types)** The size of the entry to add, in bytes.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12shadercachesession-storevalue#parameters">Read more on docs.microsoft.com </a>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: **[HRESULT](/windows/win32/com/structure-of-com-error-codes)** If the function succeeds, it returns **S_OK**. Otherwise, it returns an [**HRESULT**](/windows/desktop/com/structure-of-com-error-codes) [error code](/windows/win32/com/com-error-codes-10). |Return value|Description| |-|-| | DXGI_ERROR_ALREADY_EXISTS | There's an entry with the same key. | | DXGI_ERROR_CACHE_HASH_COLLISION | There's an entry with the same hash as the provided key, but the key doesn't match. | | DXGI_ERROR_CACHE_FULL | Adding this entry would cause the cache to become larger than its maximum size. |</para>
	/// </returns>
	/// <remarks></remarks>
	unsafe void StoreValue( nint pKey, uint KeySize, nint pValue, uint ValueSize ) ;

	/// <summary>When all cache session objects corresponding to a given cache are destroyed, the cache is cleared.</summary>
	/// <remarks>
	/// <para>A disk cache can be cleared in one of the following ways. * Explicitly, by calling **SetDeleteOnDestroy** on the session object, and then releasing the session. * Explicitly, in developer mode, by calling [ID3D12Device9::ShaderCacheControl](nf-d3d12-id3d12device9-shadercachecontrol.md) with [D3D12_SHADER_CACHE_KIND_FLAG_APPLICATION_MANAGED](ne-d3d12-d3d12_shader_cache_kind_flags.md). * Implicitly, by creating a session object with a version that doesn't match the version used to create it. * Externally, by the disk cleanup utility enumerating it and clearing it. This won't happen for caches created with the [D3D12_SHADER_CACHE_FLAG_USE_WORKING_DIR](ne-d3d12-d3d12_shader_cache_flags.md) flag. * Manually, by deleting the files (`*.idx`, `*.val`, and `*.lock`) stored on disk for [D3D12_SHADER_CACHE_FLAG_USE_WORKING_DIR](ne-d3d12-d3d12_shader_cache_flags.md) caches. Your application shouldn't attempt to do this for caches stored outside of the working directory.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12shadercachesession-setdeleteondestroy#">Read more on docs.microsoft.com </a>.</para>
	/// </remarks>
	void SetDeleteOnDestroy( ) ;

	/// <summary>Retrieves the description used to create the cache session.</summary>
	/// <returns>A [D3D12_SHADER_CACHE_SESSION_DESC](ns-d3d12-d3d12_shader_cache_session_desc.md) structure representing the description used to create the cache session.</returns>
	/// <remarks></remarks>
	ShaderCacheSessionDescription GetDesc( ) ;
	
	// ---------------------------------------------------------------------------
	new static Type ComType => typeof(ID3D12ShaderCacheSession) ;
	public new static Guid IID => ( ComType.GUID ) ;
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12ShaderCacheSession).GUID
																		.ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	// ===========================================================================
} ;


[Wrapper( typeof( ID3D12ShaderCacheSession ) )]
internal class ShaderCacheSession: DeviceChild,
								   IShaderCacheSession,
								   IComObjectRef<ID3D12ShaderCacheSession>,
								   IUnknownWrapper<ID3D12ShaderCacheSession> {

	ComPtr< ID3D12ShaderCacheSession >? _comPtr ;
	public new virtual ComPtr< ID3D12ShaderCacheSession >? ComPointer =>
		_comPtr ??= ComResources?.GetPointer< ID3D12ShaderCacheSession >( ) ;
	public override ID3D12ShaderCacheSession? ComObject => ComPointer?.Interface ;
	ID3D12ShaderCacheSession _shaderCacheSession => ComObject ?? throw new NullReferenceException( ) ;
	
	internal ShaderCacheSession( ) {
		_comPtr = ComResources?.GetPointer< ID3D12ShaderCacheSession >( ) ;
		if ( _comPtr is not null ) _initOrAdd( _comPtr ) ;
	}
	internal ShaderCacheSession( nint pointer ) {
		_comPtr = new( pointer ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal ShaderCacheSession( ID3D12ShaderCacheSession pointer ) {
		_comPtr = new( pointer ) ;
		_initOrAdd( _comPtr ) ;
	}
	internal ShaderCacheSession( ComPtr<ID3D12ShaderCacheSession> ptr ) {
		_comPtr = ptr ;
		_initOrAdd( _comPtr ) ;
	}
	
	// ---------------------------------------------------------------------------

	public unsafe void FindValue( nint pKey, uint KeySize, nint pValue, ref uint pValueSize ) => 
		_shaderCacheSession.FindValue( (void*)pKey, KeySize, (void*)pValue, ref pValueSize ) ;
	
	public ShaderCacheSessionDescription GetDesc( ) => _shaderCacheSession.GetDesc(  ) ;

	public unsafe void StoreValue( nint pKey, uint KeySize, nint pValue, uint ValueSize ) => 
		_shaderCacheSession.StoreValue( (void*)pKey, KeySize, (void*)pValue, ValueSize ) ;

	public void SetDeleteOnDestroy( ) => _shaderCacheSession.SetDeleteOnDestroy(  ) ;
	
	// ---------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12ShaderCacheSession) ;
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12ShaderCacheSession).GUID
																		.ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}
	// ===========================================================================
}