#region Using Directives

using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Direct3D12.Shader ;
using DXSharp.DXGI ;
using DXSharp.Windows ;

#endregion
namespace DXSharp.Direct3D12 ;


[EquivalentOf(typeof(D3D_SHADER_MACRO))]
public struct ShaderMacro {
	/// <summary>The macro name.</summary>
	public PCSTR Name ;

	/// <summary>The macro definition.</summary>
	public PCSTR Definition ;
} ;


[EquivalentOf(typeof(ID3DInclude)),
 ProxyFor(typeof(ID3DInclude))]
public struct Include {
	
	public unsafe void Open( IncludeType includeType, string pFileName, void* pParentData, ref void* ppData, ref uint pBytes ) {
		fixed (void** ppDataLocal = &ppData) {
			fixed ( byte* pFileNameLocal = pFileName is object ? System.Text.Encoding.Default.GetBytes(pFileName) : null ) {
				this.Open( includeType, new PCSTR (pFileNameLocal), pParentData, ppDataLocal, ref pBytes ) ;
			}
		}
	}

	/// <summary>A user-implemented method for opening and reading the contents of a shader</summary>
	/// <param name="IncludeType">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/api/d3dcommon/ne-d3dcommon-d3d_include_type">D3D_INCLUDE_TYPE</a></b> A <a href="https://docs.microsoft.com/windows/desktop/api/d3dcommon/ne-d3dcommon-d3d_include_type">D3D_INCLUDE_TYPE</a>-typed value that indicates the location of the #include file.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3dcommon/nf-d3dcommon-id3dinclude-open#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pFileName">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">LPCSTR</a></b> Name of the #include file.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3dcommon/nf-d3dcommon-id3dinclude-open#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pParentData">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">LPCVOID</a></b> Pointer to the container that includes the #include file. The compiler might pass NULL in <i>pParentData</i>. For more information, see the "Searching for Include Files" section in <a href="https://docs.microsoft.com/windows/desktop/direct3d11/d3d11-graphics-programming-guide-effects-compile">Compile an Effect (Direct3D 11)</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3dcommon/nf-d3dcommon-id3dinclude-open#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppData">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">LPCVOID</a>*</b> Pointer to the buffer  that contains the include directives. This pointer remains valid until you call<a href="https://docs.microsoft.com/windows/desktop/api/d3dcommon/nf-d3dcommon-id3dinclude-close">ID3DInclude::Close</a>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3dcommon/nf-d3dcommon-id3dinclude-open#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pBytes">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a>*</b> Pointer to the number of bytes that <b>Open</b> returns in <i>ppData</i>.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3dcommon/nf-d3dcommon-id3dinclude-open#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> The user-implemented method must return S_OK. If <b>Open</b> fails when it reads the #include file, the application programming interface (API) that caused <b>Open</b> to be called fails. This failure can occur in one of the following situations:</para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3dcommon/nf-d3dcommon-id3dinclude-open">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	public unsafe void Open( IncludeType IncludeType, PCSTR pFileName, void* pParentData, void** ppData, ref uint pBytes ) {
		((delegate *unmanaged [Stdcall]< Include*, IncludeType, PCSTR, void*, void**, ref uint, HResult>)lpVtbl[0])( 
			(Include*)Unsafe.AsPointer(ref this), IncludeType, pFileName, pParentData, ppData, ref pBytes 
		 ).ThrowOnFailure();
	}

	/// <summary>A user-implemented method for closing a shader</summary>
	/// <param name="pData">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">LPCVOID</a></b> Pointer to the buffer that contains the include directives. This is the pointer that was returned by the corresponding <a href="https://docs.microsoft.com/windows/desktop/api/d3dcommon/nf-d3dcommon-id3dinclude-open">ID3DInclude::Open</a> call.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3dcommon/nf-d3dcommon-id3dinclude-close#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> The user-implemented <b>Close</b> method should return S_OK. If <b>Close</b> fails when it closes the #include file, the application programming interface (API) that caused <b>Close</b> to be called fails. This failure can occur in one of the following situations:</para>
	/// <para></para>
	/// <para>This doc was truncated.</para>
	/// </returns>
	/// <remarks>If <a href="https://docs.microsoft.com/windows/desktop/api/d3dcommon/nf-d3dcommon-id3dinclude-open">ID3DInclude::Open</a> was successful, <b>Close</b> is guaranteed to be called before the API using the <a href="https://docs.microsoft.com/windows/desktop/api/d3dcommon/nn-d3dcommon-id3dinclude">ID3DInclude</a> interface returns.</remarks>
	public unsafe void Close(void* pData) {
		((delegate *unmanaged [Stdcall]<Include*,void* ,HResult>)lpVtbl[1])(
																				(Include*)Unsafe.AsPointer(ref this), pData 
																			).ThrowOnFailure( ) ;
	}

	public unsafe struct Vtbl {
		internal delegate *unmanaged [Stdcall]<Include*, IncludeType, PCSTR, void*, void**, ref uint, HResult> Open_1 ;

		internal delegate *unmanaged [Stdcall]<Include*, void*, HResult> Close_2 ;
	} ;
	unsafe void** lpVtbl ;
}