#region Using Directives
using System.Runtime.InteropServices ;
using DXSharp.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace Windows.Win32.Graphics.Direct3D12 ;


[ComImport, Guid( "7F91CE67-090C-4BB7-B78E-ED8FF2E31DA0" ), 
 InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
public interface ID3D12VersionedRootSignatureDeserializer: IUnknown {
	/// <summary>Converts root signature description structures to a requested version.</summary>
	/// <param name="convertToVersion">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d_root_signature_version">D3D_ROOT_SIGNATURE_VERSION</a></b> Specifies the required <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d_root_signature_version">D3D_ROOT_SIGNATURE_VERSION</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12versionedrootsignaturedeserializer-getrootsignaturedescatversion#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppDesc">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_versioned_root_signature_desc">D3D12_VERSIONED_ROOT_SIGNATURE_DESC</a>**</b> Contains the deserialized root signature in a  <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_versioned_root_signature_desc">D3D12_VERSIONED_ROOT_SIGNATURE_DESC</a> structure.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12versionedrootsignaturedeserializer-getrootsignaturedescatversion#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns an HRESULT success or error code. The method can fail with E_OUTOFMEMORY.</para>
	/// </returns>
	/// <remarks>
	/// <para>This method allocates additional storage if needed for the converted root signature (memory owned by the deserializer interface).  If conversion is done, the deserializer interface doesn’t free the original deserialized root signature memory – all versions the interface has been asked to convert to are available until the deserializer is destroyed. Converting a root signature from 1.1 to 1.0 will drop all <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_descriptor_range_flags">D3D12_DESCRIPTOR_RANGE_FLAGS</a> and <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ne-d3d12-d3d12_root_descriptor_flags">D3D12_ROOT_DESCRIPTOR_FLAGS</a> can be useful for generating compatible root signatures that need to run on old operating systems, though does lose optimization opportunities.  For instance, multiple root signature versions can be serialized and stored with application assets, with the appropriate version used at runtime based on the operating system capabilities. Converting a root signature from 1.0 to 1.1 just adds the appropriate flags to match 1.0 semantics.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12versionedrootsignaturedeserializer-getrootsignaturedescatversion#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	unsafe void GetRootSignatureDescAtVersion(
		RootSignatureVersion convertToVersion,
		VersionedRootSignatureDescription** ppDesc ) ;
	
	/// <summary>Gets the layout of the root signature, without converting between root signature versions.</summary>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_versioned_root_signature_desc">D3D12_VERSIONED_ROOT_SIGNATURE_DESC</a></b> This method returns a deserialized root signature in a <a href="https://docs.microsoft.com/windows/desktop/api/d3d12/ns-d3d12-d3d12_versioned_root_signature_desc">D3D12_VERSIONED_ROOT_SIGNATURE_DESC</a> structure that describes the layout of the root signature.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12versionedrootsignaturedeserializer-getunconvertedrootsignaturedesc">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	[PreserveSig] unsafe VersionedRootSignatureDescription* GetUnconvertedRootSignatureDesc( ) ;
} ;
		