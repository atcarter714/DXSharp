using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D12 ;

namespace DXSharp.Direct3D12 ;


[Wrapper(typeof(ID3D12Fence))]
public interface IFence: IPageable< ID3D12Fence > {
	/// <summary>Gets the current value of the fence. (ID3D12Fence.GetCompletedValue)</summary>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/WinProg/windows-data-types">UINT64</a></b> Returns the current value of the fence. If the device has been removed, the return value will be <b>UINT64_MAX</b>.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12fence-getcompletedvalue">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	ulong GetCompletedValue( ) ;

	/// <summary>Specifies an event that should be fired when the fence reaches a certain value. (ID3D12Fence.SetEventOnCompletion)</summary>
	/// <param name="Value">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/WinProg/windows-data-types">UINT64</a></b> The fence value when the event is to be signaled.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12fence-seteventoncompletion#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="hEvent">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/WinProg/windows-data-types">HANDLE</a></b> A handle to the event object.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12fence-seteventoncompletion#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b>HRESULT</b> This method returns <b>E_OUTOFMEMORY</b> if the kernel components don’t have sufficient memory to store the event in a list. See <a href="https://docs.microsoft.com/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 return codes</a> for other possible return values.</para>
	/// </returns>
	/// <remarks>
	/// <para>To specify multiple fences before an event is triggered, refer to <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12device1-seteventonmultiplefencecompletion">SetEventOnMultipleFenceCompletion</a>. If *hEvent* is a null handle, then this API will not return until the specified fence value(s) have been reached. This method can be safely called from multiple threads at one time.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12fence-seteventoncompletion#">Read more on docs.microsoft.com</see>.</para>
	/// </remarks>
	void SetEventOnCompletion( ulong Value, HANDLE hEvent ) ;

	/// <summary>Sets the fence to the specified value.</summary>
	/// <param name="Value">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/WinProg/windows-data-types">UINT64</a></b> The value to set the fence to.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12fence-signal#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>Use this method to set a fence value from the CPU side. Use <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-signal">ID3D12CommandQueue::Signal</a> to set a fence from the GPU side.</remarks>
	void Signal( ulong Value ) ;
} ;