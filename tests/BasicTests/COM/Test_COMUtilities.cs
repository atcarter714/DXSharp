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
	// ----------------------------------------------------------------------------------------------------
	
	static IFactory1? _factory ;
	
	// ----------------------------------------------------------------------------------------------------
	static IComObjectRef<IDXGIFactory1> _comRef1 =>
		( _factory as IComObjectRef< IDXGIFactory1 > ?? throw new NullReferenceException( ) ) ;
	static IComObjectRef<IDXGIFactory4> _comRef4 =>
		( _factory as IComObjectRef< IDXGIFactory4 > ?? throw new NullReferenceException( ) ) ;
	static IUnknownWrapper< IDXGIFactory1 > _wrapper1 =>
		( _factory as IUnknownWrapper< IDXGIFactory1 > ?? throw new NullReferenceException( ) ) ;
	static IUnknownWrapper< IDXGIFactory4 > _wrapper4 =>
		( _factory as IUnknownWrapper< IDXGIFactory4 > ?? throw new NullReferenceException( ) ) ;
	// ----------------------------------------------------------------------------------------------------
	
	
	
	[OneTimeSetUp]
	public void Setup( ) => 
		DXGIFunctions.CreateFactory1( IFactory1.IID, out _factory ) ;
	
	[OneTimeTearDown]
	public void Cleanup( ) => _factory?.Dispose( ) ;
	

	//! Makes sure all readonly IID_OF_* Guid fields are correct:
	[Order(0), Test( Author = "Aaron T. Carter",
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
	
	[Order(1), Test] public void Test_IUnknownWrapper( ) {
		Assert.Multiple( ( ) => {
							 Assert.That( _wrapper1, Is.Not.Null ) ;
							 Assert.That( _wrapper4, Is.Not.Null ) ;
						 } ) ;
	}
	
	[Order(2), Test] public void Test_IComObjectRef( ) {
		Assert.Multiple( ( ) => {
							 Assert.That( _comRef1, Is.Not.Null ) ;
							 Assert.That( _comRef4, Is.Not.Null ) ;
						 } ) ;
	}
	
	
	
	[Test( Author = "Aaron T. Carter",
		   Description = "Creates an IDXGIFactory and tests COMUtility methods " +
						 "on the native COM interface pointer." )]
	public void Test_Util_Methods( ) {
		
		// Get the COM pointer and make sure it's not null:
		var comPtr   = _wrapper1!.ComPointer as ComPtr< IDXGIFactory1 > ;
        Assert.Multiple( ( ) => {
			Assert.That( comPtr, Is.Not.Null ) ;
            Assert.That(comPtr!.BaseAddress, Is.Not.EqualTo(nint.Zero));
            Assert.That(comPtr.InterfaceVPtr, Is.Not.EqualTo(nint.Zero));
        } ) ;
		
        // Get actual address values:
        nint baseAddr = comPtr!.BaseAddress ;
		nint vTableAddr   = comPtr!.InterfaceVPtr ;
		Assert.That( baseAddr.IsValid( ) && vTableAddr.IsValid( ) ) ;
		
		
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
		var dxgiF1 = _comRef1!.ComObject! ;
		var dxgiF4 = _comRef4.ComObject! ;

		uint value0 = dxgiF4.AddRef( ) ;
		uint value1 = dxgiF4.Release( ) ;
		unsafe {
			// -----------------------------------------------------------------
			//! ADDRESS CHECKS:
			// Steal the pointer to the interface and IUnknown methods:
			var _pf4 = *(void ***)( (IDXGIFactory4 *)( &dxgiF4 ) ) ;
			var _vtb = (IUnknownUnsafe.VTable *)_pf4 ;
			nint _method0 = ( *_vtb )[ 0 ],
				 _method1 = ( *_vtb )[ 1 ],
				 _method2 = ( *_vtb )[ 2 ] ;
			
			// Do likewise for the IDXGIFactory1 object:
			var _pF1  = *(void ***)( (IDXGIFactory1 *)( &dxgiF1 ) ) ;
			var _vtb1 = (IUnknownUnsafe.VTable *)_pF1 ;
			nint _method0_1 = ( *_vtb1 )[ 0 ],
				 _method1_1 = ( *_vtb1 )[ 1 ],
				 _method2_1 = ( *_vtb1 )[ 2 ] ;
			Assert.Multiple( ( ) => {
								 Assert.That( _method0_1, Is.EqualTo( _method0 ) ) ;
								 Assert.That( _method1_1, Is.EqualTo( _method1 ) ) ;
								 Assert.That( _method2_1, Is.EqualTo( _method2 ) ) ;
							 } ) ;
			// -----------------------------------------------------------------

            // Use InteropUtils to pin & get pointer + handle:
			// (This gives us the address of the managed object in RAM ...)
            nint f4ManagedAddress = InteropUtils.GetManagedAddress( dxgiF4, out var hF4 ) ;
			nint f1ManagedAddress = InteropUtils.GetManagedAddress( dxgiF1, out var hF1 ) ;
			List< IDisposable > _cleanUpList = new( ) ;
			_cleanUpList.Add( hF4 ) ;
			_cleanUpList.Add( hF1 ) ;
			
			
			/* WTF NOTES ::
			 * A v-table is an array (pointer to a collection/buffer) of function pointers.
			 * A COM object is really just a pointer to a v-table, in terms of memory ...
			 * So, if we steal the address of the managed RCW object, we can steal the v-table address!
			 * Since the "v-table" field of type `void**` (pointer to pointer or "pointer to array"),
			 * We can consider the managed object as a pointer to a pointer to a pointer ... ?!!!
			 * Yes, it's a little confusing, but it's just a pointer to a pointer to a pointer!
			 * So we reinterpret the managed object pointer* as a "pointer* to a pointer-pointer**",
			 * and dereference it to get the "pointer-pointer" value (i.e., the v-table address) ...
			 * It's less confusing when you bear in mind that the "v-table pointer" is just a *number* (or "address")
			 * that we are after, because that number is the RAM location of the v-table function pointer list ...
			 */
			// Get v-table address from f4Address and f1Address:
			var vTableAddr4 = *( (void ***)f4ManagedAddress ) ;
			var vTableAddr1 = *( (void ***)f1ManagedAddress ) ;
			
			
			// Get the delegate pointers for the methods:
			QueryInterfaceDelegate* pQryInterface = (QueryInterfaceDelegate *)_method0 ;
			AddRefDelegate* pAddRef = (AddRefDelegate *)_method1 ;
			ReleaseDelegate* pRelease = (ReleaseDelegate *)_method2 ;

			// Test the methods:
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
			var guid = IFactory7.IID ;
			var _hr3 = ( *pQryInterface )
				( (IUnknownUnsafe *)f4ManagedAddress, &guid, &pResult0 ) ; //! QueryInterface for IDXGIFactory7
		}
		Assert.That( _factory4, Is.Not.Null ) ;
		
		// Test ref and release on raw COM RCW object:
		int slotTest = Marshal.GetStartComSlot( typeof( IUnknown ) ) ;
		
		ref IUnknownUnsafe @unsafe = ref IUnknownUnsafe.CreateUnsafeRef( dxgiF4 ) ;
		uint c1 = @unsafe.AddRef( ),
			 c2 = @unsafe.Release( ) ;
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



/*int counter00 = Marshal.AddRef( baseAddr ) ;
int counter01 = Marshal.AddRef( vTableAddr ) ;
int counter02 = Marshal.Release( baseAddr ) ;
int counter03 = Marshal.Release( vTableAddr ) ;
unsafe {
	var _marshalStyleCall =
		( (delegate* unmanaged< nint, uint >)( *( *(void ***)vTableAddr + 1) ) ) ;
		/* +1 is for the IUnknown.AddRef slot #1#
	uint r = _marshalStyleCall( vTableAddr ) ;
	Assert.That( r != 0 ) ;

	var cswinUnknown = *(Windows.Win32.System.Com.IUnknown *)baseAddr ;
	r = cswinUnknown.AddRef( ) ;
}
_factoryObj!.ComObject!.AddRef( ) ;*/



		/*uint c1 = dxgiFactory4.AddRef( ),
			 c2 = dxgiFactory4.Release( ) ;*/
		/*
        Assert.Multiple( ( ) => {
            Assert.That(c1, Is.Not.EqualTo(0));
            Assert.That(c2, Is.Not.EqualTo(0));
			Assert.That( c2 == c1 - 1 ) ;
			Assert.That( c2 == 1 ) ;
        } ) ;*/
		
		