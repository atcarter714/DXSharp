// ReSharper disable HeapView.BoxingAllocation

using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp.DXGI ;
using DXSharp.Windows.COM ;

namespace BasicTests.COM ;


[TestFixture(Author = "Aaron T. Carter",
			 Category = "COM", Description = "Tests COM Utilities")]
public class Test_COMUtilities {
	
	//! Makes sure all readonly IID_OF_* Guid fields are correct:
	[Test( Author = "Aaron T. Carter",
		   Description = "Checks the IID_OF_* static readonly fields " +
						 "against typeof(T).Guid for COM interface types." )]
	public void Test_IIDs_Utilities( ) {
		Assert.AreEqual( COMUtility.IIDs.IID_OF_IUnknown, typeof(IUnknown).GUID ) ;
		
		// D3D12 IID's:
		Assert.AreEqual( COMUtility.IIDs.IID_OF_ID3D12Object, typeof(ID3D12Object).GUID ) ;
		Assert.AreEqual( COMUtility.IIDs.IID_OF_ID3D12DeviceChild, typeof(ID3D12DeviceChild).GUID ) ;
		Assert.AreEqual( COMUtility.IIDs.IID_OF_ID3D12Pageable, typeof(ID3D12Pageable).GUID ) ;
		Assert.AreEqual( COMUtility.IIDs.IID_OF_ID3D12RootSignature, typeof(ID3D12RootSignature).GUID ) ;
		Assert.AreEqual( COMUtility.IIDs.IID_OF_ID3D12DescriptorHeap, typeof(ID3D12DescriptorHeap).GUID ) ;
		Assert.AreEqual( COMUtility.IIDs.IID_OF_ID3D12PipelineState, typeof(ID3D12PipelineState).GUID ) ;
		Assert.AreEqual( COMUtility.IIDs.IID_OF_ID3D12Device, typeof(ID3D12Device).GUID ) ;
		Assert.AreEqual( COMUtility.IIDs.IID_OF_ID3D12CommandList, typeof(ID3D12CommandList).GUID ) ;
		Assert.AreEqual( COMUtility.IIDs.IID_OF_ID3D12CommandQueue, typeof(ID3D12CommandQueue).GUID ) ;
		Assert.AreEqual( COMUtility.IIDs.IID_OF_ID3D12Fence, typeof(ID3D12Fence).GUID ) ;
		Assert.AreEqual( COMUtility.IIDs.IID_OF_ID3D12Resource, typeof(ID3D12Resource).GUID ) ;
		Assert.AreEqual( COMUtility.IIDs.IID_OF_ID3D12Heap, typeof(ID3D12Heap).GUID ) ;
		Assert.AreEqual( COMUtility.IIDs.IID_OF_ID3D12GraphicsCommandList, typeof(ID3D12GraphicsCommandList).GUID ) ;
		
		// DXGI IID's:
		Assert.AreEqual( COMUtility.IIDs.IID_OF_IDXGIObject, typeof(IDXGIObject).GUID ) ;
		Assert.AreEqual( COMUtility.IIDs.IID_OF_IDXGIDeviceSubObject, typeof(IDXGIDeviceSubObject).GUID ) ;
		Assert.AreEqual( COMUtility.IIDs.IID_OF_IDXGIResource, typeof(IDXGIResource).GUID ) ;
		Assert.AreEqual( COMUtility.IIDs.IID_OF_IDXGIKeyedMutex, typeof(IDXGIKeyedMutex).GUID ) ;
		Assert.AreEqual( COMUtility.IIDs.IID_OF_IDXGISurface, typeof(IDXGISurface).GUID ) ;
		Assert.AreEqual( COMUtility.IIDs.IID_OF_IDXGISurface1, typeof(IDXGISurface1).GUID ) ;
		Assert.AreEqual( COMUtility.IIDs.IID_OF_IDXGIAdapter, typeof(IDXGIAdapter).GUID ) ;
		Assert.AreEqual( COMUtility.IIDs.IID_OF_IDXGIOutput, typeof(IDXGIOutput).GUID ) ;
		Assert.AreEqual( COMUtility.IIDs.IID_OF_IDXGISwapChain, typeof(IDXGISwapChain).GUID ) ;
		Assert.AreEqual( COMUtility.IIDs.IID_OF_IDXGIFactory, typeof(IDXGIFactory).GUID ) ;
		Assert.AreEqual( COMUtility.IIDs.IID_OF_IDXGIDevice, typeof(IDXGIDevice).GUID ) ;
		Assert.AreEqual( COMUtility.IIDs.IID_OF_IDXGIOutput, typeof(IDXGIOutput).GUID ) ;
	}

	[Test( Author = "Aaron T. Carter",
		   Description = "Creates an IDXGIFactory and tests COMUtility methods " +
						 "on the native COM interface pointer." )]
	public void Test_Util_Methods( ) {
		DXGIFunctions.CreateDXGIFactory<IFactory1>( IFactory.IID, out var factory ) ;
	}
}