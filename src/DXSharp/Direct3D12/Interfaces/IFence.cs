using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
using DXSharp.Windows.Win32 ;

namespace DXSharp.Direct3D12 ;


[ProxyFor(typeof(ID3D12Fence))]
public interface IFence: IPageable,
						 IComObjectRef< ID3D12Fence >,
						 IUnknownWrapper< ID3D12Fence >, 
						 IInstantiable {
	// ---------------------------------------------------------------------------------
	new ComPtr< ID3D12Fence >? ComPointer { get ; }
	new ID3D12Fence? COMObject => ComPointer?.Interface ;
	ID3D12Fence? IComObjectRef< ID3D12Fence >.COMObject => COMObject ;
	ComPtr< ID3D12Fence >? IUnknownWrapper< ID3D12Fence >.ComPointer => ComPointer ;
	// ==================================================================================
	
	/// <summary>Gets the current value of the fence. (ID3D12Fence.GetCompletedValue)</summary>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/WinProg/windows-data-types">UINT64</a></b> Returns the current value of the fence. If the device has been removed, the return value will be <b>UINT64_MAX</b>.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12fence-getcompletedvalue">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	ulong GetCompletedValue( ) => COMObject!.GetCompletedValue( ) ;

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
	void SetEventOnCompletion( ulong Value, Win32Handle hEvent ) => COMObject!.SetEventOnCompletion( Value, hEvent ) ;

	/// <summary>Sets the fence to the specified value.</summary>
	/// <param name="Value">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/WinProg/windows-data-types">UINT64</a></b> The value to set the fence to.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12fence-signal#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> This method returns one of the <a href="https://docs.microsoft.com/windows/win32/direct3d12/d3d12-graphics-reference-returnvalues">Direct3D 12 Return Codes</a>.</para>
	/// </returns>
	/// <remarks>Use this method to set a fence value from the CPU side. Use <a href="https://docs.microsoft.com/windows/win32/api/d3d12/nf-d3d12-id3d12commandqueue-signal">ID3D12CommandQueue::Signal</a> to set a fence from the GPU side.</remarks>
	void Signal( ulong Value ) => COMObject!.Signal( Value ) ;
	
	// ---------------------------------------------------------------------------------
	static Type IUnknownWrapper.ComType => typeof(ID3D12Fence) ;
	static Guid IUnknownWrapper.InterfaceGUID => typeof(ID3D12Fence).GUID ;
	public new static Type ComType => typeof(ID3D12Fence) ;
	public new static Guid InterfaceGUID => typeof(ID3D12Fence).GUID ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = typeof(ID3D12Fence).GUID
														  .ToByteArray( ) ;
			
			return ref Unsafe
					   .As< byte, Guid >( ref MemoryMarshal
											  .GetReference(data) ) ;
		}
	}

	static IDXCOMObject IInstantiable.Instantiate( ) => new Fence( ) ;
	static IDXCOMObject IInstantiable.Instantiate( IntPtr pComObj ) => new Fence( pComObj ) ;
	static IDXCOMObject IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Fence( (ID3D12Fence)pComObj! ) ;

	// ==================================================================================
} ;


public interface IFence1: IFence,
						  IComObjectRef< ID3D12Fence1 >,
						  IUnknownWrapper< ID3D12Fence1 > {
	// ---------------------------------------------------------------------------------
	new ComPtr< ID3D12Fence1 >? ComPointer { get ; }
	new ID3D12Fence1? COMObject => ComPointer?.Interface ;
	ID3D12Fence1? IComObjectRef< ID3D12Fence1 >.COMObject => COMObject ;
	ComPtr< ID3D12Fence1 >? IUnknownWrapper< ID3D12Fence1 >.ComPointer => ComPointer ;
	// ---------------------------------------------------------------------------------
	
	/// <summary>Gets the flags used to create the fence represented by the current instance.</summary>
	/// <returns>
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/win32/api/d3d12/ne-d3d12-d3d12_fence_flags">D3D12_FENCE_FLAGS</a></b> The flags used to create the fence.</para>
	/// </returns>
	/// <remarks>The flags returned by <b>GetCreationFlags</b> are used mainly for opening a shared fence.</remarks>
	FenceFlags GetCreationFlags( ) ;
	
	
	// ---------------------------------------------------------------------------------
	public new static Type ComType => typeof(ID3D12Fence1) ;
	static Type IUnknownWrapper.ComType => typeof(ID3D12Fence1) ;
	public new static Guid InterfaceGUID => typeof(ID3D12Fence1).GUID ;
	static Guid IUnknownWrapper.InterfaceGUID => typeof(ID3D12Fence1).GUID ;
	
	static ref readonly Guid IComIID.Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = InterfaceGUID.ToByteArray( ) ;
			
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}
	
	static IDXCOMObject IInstantiable.Instantiate( ) => new Fence1( ) ;
	static IDXCOMObject IInstantiable.Instantiate( IntPtr pComObj ) => new Fence1( pComObj ) ;
	static IDXCOMObject IInstantiable.Instantiate< ICom >( ICom pComObj ) => new Fence1( (ID3D12Fence1)pComObj! ) ;	
	// ---------------------------------------------------------------------------------
	
	// ==================================================================================
} ;