// ReSharper disable HeapView.BoxingAllocation

using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.System.Com ;
using DXSharp ;
using DXSharp.DXGI ;
using DXSharp.Windows.COM ;
using static DXSharp.Windows.COM.COMUtility ;
using IUnknown = DXSharp.Windows.COM.IUnknown ;

namespace BasicTests.COM ;


[TestFixture(Author = "Aaron T. Carter",
			 Category = "COM", Description = "Tests COM Utilities")]
public class Test_COMUtilities {
	static IFactory1? _factory ;
	
	[OneTimeSetUp]
	public void Setup( ) => 
		DXGIFunctions.CreateFactory1( IFactory1.IID, out _factory ) ;
	
	[OneTimeTearDown]
	public void Cleanup( ) => _factory?.Dispose( ) ;
	

	//! Makes sure all readonly IID_OF_* Guid fields are correct:
	[Test( Author = "Aaron T. Carter",
		   Description = "Checks the IID_OF_* static readonly fields " +
						 "against typeof(T).Guid for COM interface types." )]
	public void Test_IIDs_Utilities( ) {
		Assert.That( typeof(IUnknown).GUID, Is.EqualTo(IIDs.IID_OF_IUnknown)) ;
		
		// D3D12 IID's:
		Assert.That( typeof(ID3D12Object).GUID, Is.EqualTo(IIDs.IID_OF_ID3D12Object)) ;
		Assert.That( typeof(ID3D12DeviceChild).GUID, Is.EqualTo(IIDs.IID_OF_ID3D12DeviceChild)) ;
		Assert.That( typeof(ID3D12Pageable).GUID, Is.EqualTo(IIDs.IID_OF_ID3D12Pageable)) ;
		Assert.That( typeof(ID3D12RootSignature).GUID, Is.EqualTo(IIDs.IID_OF_ID3D12RootSignature)) ;
		Assert.That( typeof(ID3D12DescriptorHeap).GUID, Is.EqualTo(IIDs.IID_OF_ID3D12DescriptorHeap)) ;
		Assert.That( typeof(ID3D12PipelineState).GUID, Is.EqualTo(IIDs.IID_OF_ID3D12PipelineState)) ;
		Assert.That( typeof(ID3D12Device).GUID, Is.EqualTo(IIDs.IID_OF_ID3D12Device)) ;
		Assert.That( typeof(ID3D12CommandList).GUID, Is.EqualTo(IIDs.IID_OF_ID3D12CommandList)) ;
		Assert.That( typeof(ID3D12CommandQueue).GUID, Is.EqualTo(IIDs.IID_OF_ID3D12CommandQueue)) ;
		Assert.That( typeof(ID3D12Fence).GUID, Is.EqualTo(IIDs.IID_OF_ID3D12Fence)) ;
		Assert.That( typeof(ID3D12Resource).GUID, Is.EqualTo(IIDs.IID_OF_ID3D12Resource)) ;
		Assert.That( typeof(ID3D12Heap).GUID, Is.EqualTo(IIDs.IID_OF_ID3D12Heap)) ;
		Assert.That( typeof(ID3D12GraphicsCommandList).GUID, Is.EqualTo(IIDs.IID_OF_ID3D12GraphicsCommandList)) ;
		
		// DXGI IID's:
		Assert.That( typeof(IDXGIObject).GUID, Is.EqualTo(IIDs.IID_OF_IDXGIObject)) ;
		Assert.That( typeof(IDXGIDeviceSubObject).GUID, Is.EqualTo(IIDs.IID_OF_IDXGIDeviceSubObject)) ;
		Assert.That( typeof(IDXGIResource).GUID, Is.EqualTo(IIDs.IID_OF_IDXGIResource)) ;
		Assert.That( typeof(IDXGIKeyedMutex).GUID, Is.EqualTo(IIDs.IID_OF_IDXGIKeyedMutex)) ;
		Assert.That( typeof(IDXGISurface).GUID, Is.EqualTo(IIDs.IID_OF_IDXGISurface)) ;
		Assert.That( typeof(IDXGISurface1).GUID, Is.EqualTo(IIDs.IID_OF_IDXGISurface1)) ;
		Assert.That( typeof(IDXGIAdapter).GUID, Is.EqualTo(IIDs.IID_OF_IDXGIAdapter)) ;
		Assert.That( typeof(IDXGIOutput).GUID, Is.EqualTo(IIDs.IID_OF_IDXGIOutput)) ;
		Assert.That( typeof(IDXGISwapChain).GUID, Is.EqualTo(IIDs.IID_OF_IDXGISwapChain)) ;
		Assert.That( typeof(IDXGIFactory).GUID, Is.EqualTo(IIDs.IID_OF_IDXGIFactory)) ;
		Assert.That( typeof(IDXGIDevice).GUID, Is.EqualTo(IIDs.IID_OF_IDXGIDevice)) ;
		Assert.That( typeof(IDXGIOutput).GUID, Is.EqualTo(IIDs.IID_OF_IDXGIOutput)) ;
	}

	
	[Test( Author = "Aaron T. Carter",
		   Description = "Creates an IDXGIFactory and tests COMUtility methods " +
						 "on the native COM interface pointer." )]
	public void Test_Util_Methods( ) {
		// Get wrapper and COM object interfaces off the IFactory:
		var _factoryWrapper = _factory as IUnknownWrapper< IDXGIFactory1 > ;
		var _factoryObj     = _factoryWrapper as IComObjectRef< IDXGIFactory1 > ;
		Assert.Multiple( ( ) => {
							 Assert.That( _factoryWrapper, Is.Not.Null ) ;
							 Assert.That( _factoryObj, Is.Not.Null ) ;
							 Assert.That( _factoryObj!.ComObject, Is.Not.Null ) ;
						 } ) ;

		
		
		// Get the COM pointer and make sure it's not null:
		var comPtr   = _factoryWrapper!.ComPointer as ComPtr< IDXGIFactory1 > ;
        Assert.Multiple( ( ) => {
			Assert.That( comPtr, Is.Not.Null ) ;
            Assert.That(comPtr!.BaseAddress, Is.Not.EqualTo(nint.Zero));
            Assert.That(comPtr.InterfaceVPtr, Is.Not.EqualTo(nint.Zero));
        } ) ;
		
        // Get actual address values:
        nint baseAddr = comPtr!.BaseAddress ;
		nint vTableAddr   = comPtr!.InterfaceVPtr ;
		Assert.That( baseAddr.IsValid( ) && vTableAddr.IsValid( ) ) ;
		
		int counter00 = Marshal.AddRef( baseAddr ) ;
		int counter01 = Marshal.AddRef( vTableAddr ) ;
		int counter02 = Marshal.Release( baseAddr ) ;
		int counter03 = Marshal.Release( vTableAddr ) ;
		unsafe {
			var _marshalStyleCall =
				/* +1 is for the IUnknown.AddRef slot */
				( (delegate* unmanaged< nint, uint >)( *( *(void ***)vTableAddr + 1) ) ) ;
			
			uint r = _marshalStyleCall( vTableAddr ) ;
			Assert.That( r != 0 ) ;
			
			var cswinUnknown = *(Windows.Win32.System.Com.IUnknown *)baseAddr ;
			r = cswinUnknown.AddRef( ) ;
		}
		_factoryObj!.ComObject!.AddRef( ) ;
		
		
		// Test query for IDXGIFactory1:
		var _hr1 = QueryInterface< IDXGIFactory1 >( vTableAddr,
															   out nint ppvFactory1 ) ;
		Assert.Multiple( ( ) => {
							 Assert.That( _hr1.Succeeded ) ;
							 Assert.That( ppvFactory1, Is.Not.Zero ) ;
						 } ) ;
		
        // Test query for IUnknown:
        var _hr2 = QueryInterface< IUnknown >( vTableAddr, 
																	  out nint ppvUnknown ) ;
		Assert.Multiple( ( ) => {
							 Assert.That( _hr2.Succeeded ) ;
							 Assert.That( ppvUnknown, Is.Not.Zero ) ;
					         Assert.That( ppvUnknown  == baseAddr ) ;
							 Assert.That( ppvFactory1 == vTableAddr ) ;
						 } ) ;
		
		// Release the queried pointers:
		int unknownReleaseCount = Release( ppvUnknown ) ;
		int fact1ReleaseCount   = Release( ppvFactory1 ) ;
		
		
		// Check count on queried pointers:
		int unknownRefCount = GetRefCount( ppvUnknown ) ;
		int fact1RefCount   = GetRefCount( ppvFactory1 ) ;
        Assert.Multiple(( ) => {
            Assert.That( unknownRefCount, Is.Not.Zero ) ;
            Assert.That( fact1RefCount, Is.Not.Zero ) ;
        }) ;


        // Now try downcasting to IDXGIFactory4:

		var _factory4 = Cast< IFactory1, IFactory4 >( _factory! ) ;
		
		// Get the object references for wrapper and COM object:
		var _factory4Wrapper   = _factory4 as IUnknownWrapper< IDXGIFactory4 > ;
		var _factory4Obj       = _factory4Wrapper as IComObjectRef< IDXGIFactory4 > ;
		var dxgiF1 = _factoryObj!.ComObject! ;
		var dxgiF4 = (_factory4 as IComObjectRef< IDXGIFactory4 > )!.ComObject! ;

		uint value0 = _factory4Obj!.ComObject.AddRef( ) ;
		uint value1 = _factory4Obj!.ComObject.Release( ) ;
		
		unsafe {
			// Steal the pointer to the interface and IUnknown methods:
			var _pf4 = *(void ***)( (IDXGIFactory4 *)( &dxgiF4 ) ) ;
			var _vtb = (IUnknownUnsafe.VTable *)_pf4 ;
			nint _method0 = ( *_vtb )[ 0 ],
				 _method1 = ( *_vtb )[ 1 ],
				 _method2 = ( *_vtb )[ 2 ] ;
			Windows.Win32.System.Com.IUnknown kk ;
			// Do likewise for the IDXGIFactory1 object:
			var _pF1  = *(void ***)( (IDXGIFactory1 *)( &dxgiF1 ) ) ;
			var _vtb1 = (IUnknownUnsafe.VTable *)_pF1 ;
			nint _method0_1 = ( *_vtb1 )[ 0 ],
				 _method1_1 = ( *_vtb1 )[ 1 ],
				 _method2_1 = ( *_vtb1 )[ 2 ] ;
			
			// Use InteropUtils to pin & get pointer + handle:
			nint f4Address = InteropUtils.GetManagedAddress( dxgiF4, out var hF4 ) ;
			nint f1Address = InteropUtils.GetManagedAddress( dxgiF1, out var hF1 ) ;
			
			// Get v-table address from f4Address and f1Address:
			var vTableAddr4 = *( (void ***)f4Address ) ;
			var vTableAddr1 = *( (void ***)f1Address ) ;
			//( (IUnknownUnsafe *)f4Address )->lpVtbl ;
			//( (IUnknownUnsafe *)f1Address )->lpVtbl ;
			//var vtA4_B = (void ***)vTableAddr4 ;
			//var vtA1_B = (void ***)vTableAddr1 ;
			
			// Get the delegate pointers for the methods:
			QueryInterfaceDelegate* pQryInterface = (QueryInterfaceDelegate *)_method0 ;
			AddRefDelegate* pAddRef = (AddRefDelegate *)_method1 ;
			ReleaseDelegate* pRelease = (ReleaseDelegate *)_method2 ;

			uint ct0 = ( *pAddRef )( (IUnknownUnsafe *)_pf4 ) ;
			uint ct1 = ( *pAddRef )( (IUnknownUnsafe *)_pF1 ) ;
			
			// Verify the addresses:
			Assert.Multiple( ( ) => {
								 Assert.That( (nint)vTableAddr4, Is.EqualTo( (nint)_method0 ) ) ; //! address of QueryInterface (1st v-table method)
								 Assert.That( (nint)vTableAddr1, Is.EqualTo( (nint)_method0_1 ) ) ;
								 Assert.That( (nint)vTableAddr4, Is.EqualTo( (nint)pQryInterface ) ) ;
							 } ) ;
			
			// Test the methods:
			void* pResult0 = null ;
			var guid = IIDs.IID_OF_IDXGIFactory ;
			var _hr3 = ( *pQryInterface )
				( (IUnknownUnsafe *)f4Address, &guid, &pResult0 ) ;
		}
		Assert.That( _factory4, Is.Not.Null ) ;
		
		// Test ref and release on raw COM RCW object:
		int slotTest = Marshal.GetStartComSlot( typeof( IUnknown ) ) ;
		
		IDXGIFactory1 dxgiFactory4 = _factoryObj!.ComObject! ;
		ref IUnknownUnsafe @unsafe = ref IUnknownUnsafe.CreateUnsafeRef( dxgiFactory4 ) ;
		uint c1 = @unsafe.AddRef( ),
			 c2 = @unsafe.Release( ) ;
		/*uint c1 = dxgiFactory4.AddRef( ),
			 c2 = dxgiFactory4.Release( ) ;*/
		/*
        Assert.Multiple( ( ) => {
            Assert.That(c1, Is.Not.EqualTo(0));
            Assert.That(c2, Is.Not.EqualTo(0));
			Assert.That( c2 == c1 - 1 ) ;
			Assert.That( c2 == 1 ) ;
        } ) ;*/
		
		
		// Call some methods on it and make sure it works:
		var creationFlags = _factory4!.GetCreationFlags( ) ;
		var current = _factory4!.IsCurrent( ) ;
		
		// Check ref counts:
		int baseRefCount = GetRefCount( baseAddr ) ;
		int vtblRefCount = GetRefCount( vTableAddr ) ;
        Assert.Multiple( ( ) => {
            Assert.That(baseRefCount, Is.EqualTo(1));
            Assert.That(vtblRefCount, Is.EqualTo(1));
        } ) ;
    }
}