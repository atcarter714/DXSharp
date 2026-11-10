#pragma warning disable CS8500
// ReSharper disable HeapView.BoxingAllocation

using System.Diagnostics.CodeAnalysis ;
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Direct3D12 ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.System.Com ;
using DXSharp ;
using DXSharp.DXGI ;
using DXSharp.XTensions ;
using DXSharp.Windows.COM ;
using static DXSharp.Windows.COM.COMUtility ;
using IUnknown = DXSharp.Windows.COM.IUnknown ;

namespace BasicTests.COM ;


[TestFixture(Author = "Aaron T. Carter",
			 Category = "BuildBaseline", Description = "Tests COM Utilities")]
public class Test_COMUtilities {
	// ----------------------------------------------------------------------------------------------------
	
	static  IFactory1? _factory ;
	
	[SuppressMessage( "Structure", 
	"NUnit1032:An IDisposable field/property should be Disposed in a TearDown method" )]
	static IFactory4? _factory4 ;
	
	// ----------------------------------------------------------------------------------------------------
	static IComObjectRef<IDXGIFactory1> _comRef1 =>
		( _factory as IComObjectRef< IDXGIFactory1 > ?? throw new NullReferenceException( ) ) ;
	static IComObjectRef<IDXGIFactory4> _comRef4 =>
		( _factory4 as IComObjectRef< IDXGIFactory4 > ?? throw new NullReferenceException( ) ) ;
	static IUnknownWrapper< IDXGIFactory1 > _wrapper1 =>
		( _factory as IUnknownWrapper< IDXGIFactory1 > ?? throw new NullReferenceException( ) ) ;
	static IUnknownWrapper< IDXGIFactory4 > _wrapper4 =>
		( _factory4 as IUnknownWrapper< IDXGIFactory4 > ?? throw new NullReferenceException( ) ) ;
	// ----------------------------------------------------------------------------------------------------

	
	
	[OneTimeSetUp]
	public void Setup( ) {
		DXGIFunctions.CreateFactory1( IFactory1.IID, out _factory ).ThrowOnFailure() ;
	}
	
	[OneTimeTearDown]
	public void Cleanup( ) {
		_factory?.Dispose( ) ;
		_factory = null ;
		
		_factory4?.Dispose( ) ;
		_factory4 = null ;
	}
	
	
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
	
	
	
	
	[Order(1), Test( Author = "Aaron T. Carter",
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
		
		
		QueryInterface<IDXGIFactory1>(vTableAddr, out nint factory).ThrowOnFailure();
		try {
			QueryInterface<IUnknown>(factory, out nint identity).ThrowOnFailure();
			try { Assert.That(identity, Is.EqualTo(baseAddr)); }
			finally { Release(identity); }
		}
		finally { Release(factory); }

		_factory4 = Cast<IFactory1, IFactory4>(_factory!);
		var dxgiF4 = _comRef4.ComObject!;

		Assert.That( _factory4, Is.Not.Null ) ;
		Assert.That( dxgiF4 is not null ) ;
		
		nint pointer = GetIUnknownForObject(dxgiF4);
		try {
			unsafe {
				var native = (IUnknownUnmanaged*)pointer;
				native->AddRef();
				native->Release();
			}
		}
		finally { Release(pointer); }

		// Call some methods on it and make sure it works:
		var creationFlags = _factory4!.GetCreationFlags( ) ;
		var current   = _factory4!.IsCurrent( ) ;
		Assert.Multiple( ( ) => {
							 Assert.That( creationFlags, Is.EqualTo( FactoryCreateFlags.None ) ) ;
							 Assert.That( current, Is.True ) ;
						 } ) ;
	}
	
	
	[Order(2), Test] public void Test_IUnknownWrapper( ) {
		Assert.Multiple( ( ) => {
							 Assert.That( _wrapper1, Is.Not.Null ) ;
							 //Assert.That( _wrapper4, Is.Not.Null ) ;
						 } ) ;
	}
	[Order(3), Test] public void Test_IComObjectRef( ) {
		Assert.Multiple( ( ) => {
							 Assert.That( _comRef1, Is.Not.Null ) ;
							 //Assert.That( _comRef4, Is.Not.Null ) ;
						 } ) ;
	}
	
	// ====================================================================================================
} ;

