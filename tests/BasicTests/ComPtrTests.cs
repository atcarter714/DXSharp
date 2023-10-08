#region Using Directives
using DXSharp.Windows.COM;

using System.Runtime.InteropServices;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dxgi;

#endregion

namespace BasicTests;



[TestFixture, FixtureLifeCycle( LifeCycle.SingleInstance )]
internal class ComPtrTests
{
	static HResult hr;
	static IntPtr address;
	static IDXGIFactory7? factory7;

#pragma warning disable NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method
	static ComPtr<IDXGIFactory7>? factory7Ptr;
#pragma warning restore NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method

	[OneTimeSetUp]
	public void SetUp( ) {
		hr = PInvoke.CreateDXGIFactory2(
			0x00u, typeof( IDXGIFactory7 ).GUID, out object? ppFactory );

		factory7 = (IDXGIFactory7)ppFactory;
	}

	[OneTimeTearDown]
	public void Cleanup() {

		// Release the ComPtr:
		if( factory7 is not null ) Marshal.ReleaseComObject( factory7 ) ;
		if( factory7Ptr is not null) factory7Ptr?.Dispose( ) ;

		hr = default;
		address = default;
		factory7 = null;
		factory7Ptr = null;
	}



	[Test, Order( 0 )]
	public void Test_Create_ComPtr() {
		// Ensure PInvoke call succeeded:
		Assert.True( hr.Succeeded );
		Assert.IsNotNull( factory7 );

		// Create the ComPtr object:
		factory7Ptr = create_ComPtr( Marshal.GetIUnknownForObject( factory7 ) ) ;
		Assert.IsNotNull( factory7Ptr );
		Assert.IsFalse( factory7Ptr.Disposed );
		Assert.IsNotNull( factory7Ptr.Interface );
		//Assert.That( factory7Ptr.Interface, Is.EqualTo( typeof( IDXGIFactory7 ).GUID ) );

		// Save & verify the COM interface address:
		address = factory7Ptr.IUnknownAddress;
		Assert.That( address, Is.Not.EqualTo( IntPtr.Zero ) );
	}

	[Test, Order( 1 )]
	public void Test_Dispose_ComPtr() {
		// Validate that the objects are alive:
		Assert.IsNotNull( factory7 );
		Assert.IsNotNull( factory7Ptr );

		release_ComPtr( factory7Ptr );
	}

	[Test, Order( 2 )]
	public void Test_Native_Interface_Freed() {
		// Verify the initial state:
		Assert.IsNotNull( factory7Ptr );            // should be non-null
		Assert.IsTrue( factory7Ptr.Disposed );      // should be disposed
		Assert.IsFalse( address == IntPtr.Zero );   // should have old ptr
	}





	ComPtr<T> create_ComPtr< T >( T? comObj ) where T: IUnknownWrapper 
	{
		Assert.NotNull( comObj ) ;
		ComPtr< T >? comPtr = null ;

		// Create a ComPtr<T> and validate constructor:
		Assert.DoesNotThrow( () =>
		{
			comPtr = new ComPtr<T>( comObj! ) ;
		} );

		// Assert that the ComPtr object is non-null:
		Assert.IsNotNull( comPtr );

		// Assert that the "disposed" state is *false* as it should be:
		Assert.IsFalse( comPtr.Disposed );

		// Assert that the ComPtr has a valid internal pointer:
		Assert.That( comPtr.IUnknownAddress, Is.Not.EqualTo( IntPtr.Zero ) );

		// Assert that the ComPtr interface reference is valid:
		Assert.NotNull( comPtr.Interface );

		// Assert that we've obtained the GUID of the COM interface:
		//! TODO: This is not working anymore because of changes and won't compile ...
		//Assert.That( comPtr.GUID, Is.EqualTo( typeof( T ).GUID ) );

		// Check if the object is actually a valid COM interface:
		Assert.IsTrue( Marshal.IsComObject( comPtr.Interface! ) );

		return comPtr;
	}

	void release_ComPtr( ComPtr? comPtr ) {
		// Ensure the ComPtr is alive and call Dispose:
		Assert.NotNull( comPtr );
		comPtr?.Dispose();

		// Ensure it has disposed and cleared its internal state:
		Assert.IsTrue( comPtr?.Disposed );
		Assert.That( IntPtr.Zero, Is.EqualTo( comPtr?.IUnknownAddress ) );
		Assert.IsTrue( comPtr?.Disposed );
	}
}

//void create_ComPtr_Factory7()
//{
//	// Create a ComPtr<IDXGIFactory7>:
//	factory7Ptr = new ComPtr<IDXGIFactory7>(factory7);
//	address = factory7Ptr.Pointer;

//	// Assert that the ComPtr has a valid internal pointer:
//	Assert.That(factory7Ptr.Pointer, Is.Not.EqualTo(IntPtr.Zero));

//	// Assert that the ComPtr interface reference is valid:
//	Assert.NotNull(factory7Ptr.Interface);
//}
// NOTE: I've discovered that create an IDXGIFactory interface
// gives you a reference to a COM object that already has a ref
// count of 1, which becomes 2 after calling the DXGICreateFactoryX
// function. Apparently, Windows already has an instance in memory?

//IntPtr adapterPtr = IntPtr.Zero;
//IDXGIAdapter1 ppAdapter = default;
//factory7?.EnumAdapters1( 0x00u, out ppAdapter );

//if(ppAdapter is not null)
//	adapterPtr = Marshal.GetComInterfaceForObject( ppAdapter, typeof(IDXGIAdapter4) );

//if(adapterPtr != IntPtr.Zero)
//{
//	var adptr4 = Marshal.GetObjectForIUnknown(adapterPtr) as IDXGIAdapter4;

//	int refCount = Marshal.AddRef(adapterPtr);
//	int n = refCount;
//	refCount = Marshal.Release(adapterPtr);
//}

//int refCount = Marshal.AddRef( comPtr.Pointer );
//Assert.IsTrue( refCount == 2 );
//refCount = Marshal.Release( comPtr.Pointer );
//Assert.IsTrue( refCount == 1 );